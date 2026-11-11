# SPDX-License-Identifier: MIT
# Copyright (c) 2026 koudenpa
#
# 画面と音の見比べ（#1553）で、場面ごとに Src.exe（または SRC#Form）のウィンドウを撮る。手順は SRC.Sharp/Compat/PROBE.md と SCREENS.md。
# これはひな形。probe_pack.py export が、$scenes の中の印の行（SCENES を @@ で囲んだもの）に screens-scenes.txt の場面を埋め、UTF-8（BOM 付き）・CRLF で書き出す。
#
# 使い方（書き出したフォルダで）:
#   powershell -NoProfile -ExecutionPolicy Bypass -File capture_screens.ps1 -SrcExe C:\SRC\Src.exe
#     Src.exe に screens\screens.eve を開かせ、場面ごとに操作を示す。場面を出してこのウィンドウで Enter を押すと、
#     Src.exe のウィンドウを前に出して screens-shots\<ID>.png に保存する。音の場面は聞いたことを screens-sounds.txt に書く
#   -From <場面のID>       その場面から始める（撮り直し。-SrcExe を省くと、起動しているものを撮る）
#   -SrcExe <SRCSharpForm.exe> -ProcessName SRCSharpForm -OutDir screens-form-shots
#                         SRC#Form の画面を撮る（音のメモは screens-form-shots\sounds.txt）
param(
  [string]$SrcExe = "",
  [string]$From = "",
  [string]$OutDir = "",
  [string]$ProcessName = "Src"
)
$ErrorActionPreference = "Stop"
$here = Split-Path -Parent $MyInvocation.MyCommand.Path
if ($OutDir -eq "") { $OutDir = Join-Path $here "screens-shots" }
if (-not [System.IO.Path]::IsPathRooted($OutDir)) { $OutDir = Join-Path $here $OutDir }
New-Item -ItemType Directory -Force -Path $OutDir | Out-Null
if ($ProcessName -eq "Src") { $soundsPath = Join-Path $here "screens-sounds.txt" } else { $soundsPath = Join-Path $OutDir "sounds.txt" }

Add-Type -AssemblyName System.Drawing
Add-Type @"
using System;
using System.Runtime.InteropServices;
public static class CaptureWin32 {
  [StructLayout(LayoutKind.Sequential)] public struct RECT { public int Left, Top, Right, Bottom; }
  [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
  [DllImport("user32.dll")] public static extern bool SetProcessDPIAware();
}
"@
[CaptureWin32]::SetProcessDPIAware() | Out-Null

# (ID, 種類, 場面, 実機での操作)
$scenes = @(
@@SCENES@@
)

function Get-TargetWindow {
  $p = Get-Process -Name $ProcessName -ErrorAction SilentlyContinue | Where-Object { $_.MainWindowHandle -ne 0 } | Select-Object -First 1
  if ($p -eq $null) { return [IntPtr]::Zero }
  return $p.MainWindowHandle
}

function Save-Window([string]$path) {
  $h = Get-TargetWindow
  if ($h -eq [IntPtr]::Zero) {
    Write-Host "  $ProcessName のウィンドウが見つかりません" -ForegroundColor Red
    return $false
  }
  # 前に出してから撮る（このウィンドウが重なっていても写らないように）。Src.exe はクリックしない（Wait Click の場面が進む）
  [CaptureWin32]::SetForegroundWindow($h) | Out-Null
  Start-Sleep -Milliseconds 500
  $r = New-Object CaptureWin32+RECT
  [CaptureWin32]::GetWindowRect($h, [ref]$r) | Out-Null
  $w = $r.Right - $r.Left
  $hh = $r.Bottom - $r.Top
  $bmp = New-Object System.Drawing.Bitmap $w, $hh
  $g = [System.Drawing.Graphics]::FromImage($bmp)
  $g.CopyFromScreen($r.Left, $r.Top, 0, 0, $bmp.Size)
  $bmp.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
  $g.Dispose()
  $bmp.Dispose()
  Write-Host "  保存した: $path（$w x $hh）"
  return $true
}

if ($SrcExe -ne "") {
  $eve = Join-Path $here "screens\screens.eve"
  Write-Host "$SrcExe で $eve を開きます"
  Start-Process -FilePath $SrcExe -ArgumentList ('"' + $eve + '"') -WorkingDirectory (Split-Path -Parent $SrcExe) | Out-Null
}

Write-Host ""
Write-Host "画面と音の見比べ（#1553）。場面ごとに、示した操作でその場面を出し、このウィンドウに戻って Enter を押してください。"
Write-Host "撮るときはウィンドウを前に出すので、Src.exe をクリックして戻らないでください（Wait Click の場面は、クリックで次へ進みます）。"
Write-Host "s で飛ばす、q で終わる。撮ったあと r で撮り直し。"
$started = ($From -eq "")
foreach ($s in $scenes) {
  $id = $s[0]; $kind = $s[1]; $title = $s[2]; $steps = $s[3]
  if (-not $started) {
    if ($id -eq $From) { $started = $true } else { continue }
  }
  Write-Host ""
  Write-Host "[$id] $title（$kind）" -ForegroundColor Cyan
  Write-Host "  操作: $steps"
  if ($kind -eq "音") {
    $note = Read-Host "  聞いたこと（鳴ったか・切り替わったか・繰り返すか。例: cp_map.mid に切り替わり、繰り返す）を書いて Enter（q で終わる）"
    if ($note -eq "q") { break }
    Add-Content -Path $soundsPath -Value ($id + "`t" + $note) -Encoding UTF8
    continue
  }
  $done = $false
  while (-not $done) {
    $a = Read-Host "  場面を出したら Enter（s で飛ばす・q で終わる）"
    if ($a -eq "q") { Write-Host "途中で終わった。-From $id で続きから始められる"; exit 0 }
    if ($a -eq "s") { break }
    if (Save-Window (Join-Path $OutDir ($id + ".png"))) {
      $b = Read-Host "  よければ Enter、撮り直すなら r"
      if ($b -ne "r") { $done = $true }
    }
  }
}
Write-Host ""
Write-Host "終わり。$OutDir と $soundsPath を持ち帰ってください。"
