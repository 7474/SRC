<#
実機との比較のプローブ（#1550）を、Src.exe で自動で流す。手順は PROBE.md の「自動で流す」。

probe_pack.py export で書き出した probe-pack のフォルダで、すべてのケースを順に Src.exe で開く。
元の SRC のエラーのフォーム（frmErrorMessage。タイトルが「エラー」）が出たら、文字の欄を読み取って
errors_<イベント>.txt に書き、フォームを閉じて続ける（手で OK を押すのと同じ）。

  powershell -NoProfile -ExecutionPolicy Bypass -File run_probe_auto.ps1 -SrcExe C:\SRC\Src.exe

Src.exe はイベントファイルを起動の引数で受け取る。PowerShell の Start-Process はコマンドラインの
末尾に空白を足し、元の SRC の Main（Mid$(Command$(), 2, Len - 2) で前後の " を外す）が .eve と
判定できずにファイルの選択ダイアログを出すので、ProcessStartInfo で起動する。
#>
param(
  [Parameter(Mandatory)] [string]$SrcExe,
  [string]$PackDir,
  [int]$TimeoutSec = 300
)
$ErrorActionPreference = 'Stop'
if (-not $PackDir) { $PackDir = Split-Path -Parent $MyInvocation.MyCommand.Path }
Add-Type @"
using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
public static class ProbeWin {
  public delegate bool EnumProc(IntPtr h, IntPtr l);
  [DllImport("user32.dll")] public static extern bool EnumWindows(EnumProc f, IntPtr l);
  [DllImport("user32.dll")] public static extern bool EnumChildWindows(IntPtr p, EnumProc f, IntPtr l);
  [DllImport("user32.dll")] public static extern uint GetWindowThreadProcessId(IntPtr h, out uint pid);
  [DllImport("user32.dll", CharSet=CharSet.Unicode)] public static extern int GetClassName(IntPtr h, StringBuilder s, int n);
  [DllImport("user32.dll", CharSet=CharSet.Unicode)] public static extern IntPtr SendMessage(IntPtr h, int msg, IntPtr w, StringBuilder l);
  [DllImport("user32.dll")] public static extern IntPtr SendMessage(IntPtr h, int msg, IntPtr w, IntPtr l);
  [DllImport("user32.dll")] public static extern bool PostMessage(IntPtr h, int msg, IntPtr w, IntPtr l);
  [DllImport("user32.dll")] public static extern bool IsWindowVisible(IntPtr h);
  public static string Text(IntPtr h) {
    int n = (int)SendMessage(h, 0x000E, IntPtr.Zero, IntPtr.Zero);
    var sb = new StringBuilder(n + 2);
    SendMessage(h, 0x000D, (IntPtr)(n + 1), sb);
    return sb.ToString();
  }
  public static string Cls(IntPtr h) { var sb = new StringBuilder(256); GetClassName(h, sb, 256); return sb.ToString(); }
  public static List<IntPtr> Tops(uint pid) {
    var r = new List<IntPtr>();
    EnumWindows((h, l) => { uint p; GetWindowThreadProcessId(h, out p); if (p == pid && IsWindowVisible(h)) r.Add(h); return true; }, IntPtr.Zero);
    return r;
  }
  public static List<IntPtr> Kids(IntPtr parent) {
    var r = new List<IntPtr>();
    EnumChildWindows(parent, (h, l) => { r.Add(h); return true; }, IntPtr.Zero);
    return r;
  }
}
"@

function Invoke-Case([string]$eve) {
  $name = [IO.Path]::GetFileNameWithoutExtension($eve)
  $errFile = Join-Path (Split-Path $eve) "errors_$name.txt"
  $psi = New-Object System.Diagnostics.ProcessStartInfo $SrcExe, "`"$eve`""
  $psi.UseShellExecute = $false
  $psi.WorkingDirectory = Split-Path $SrcExe
  $p = [System.Diagnostics.Process]::Start($psi)
  $dialogs = New-Object System.Collections.Generic.List[string]
  $start = Get-Date
  while (-not $p.HasExited) {
    Start-Sleep -Milliseconds 300
    if (((Get-Date) - $start).TotalSeconds -gt $TimeoutSec) {
      Write-Warning "時間切れ。Src.exe を終わらせる: $eve"
      $p.Kill()
      break
    }
    foreach ($h in [ProbeWin]::Tops([uint32]$p.Id)) {
      $cls = [ProbeWin]::Cls($h)
      $title = [ProbeWin]::Text($h)
      if ($cls -like 'Thunder*Form*' -and $title -eq 'エラー') {
        $body = ''
        foreach ($k in [ProbeWin]::Kids($h)) {
          if (([ProbeWin]::Cls($k)) -like '*TextBox*') { $body = [ProbeWin]::Text($k) }
        }
        $dialogs.Add($body.TrimEnd("`r", "`n"))
        Write-Host ("  エラー: " + ($body -split "`r?`n")[1])
        # ErrorMessage は frmErrorMessage.Visible が偽になるまで待つ。閉じれば次へ進む
        [void][ProbeWin]::PostMessage($h, 0x0010, [IntPtr]::Zero, [IntPtr]::Zero)
        Start-Sleep -Milliseconds 400
      }
      elseif ($cls -eq '#32770' -and $title -like '*ファイルの指定*') {
        Write-Warning "ファイルの選択ダイアログが出た（イベントファイルを開けなかった）: $eve"
        $p.Kill()
        break
      }
    }
  }
  if ($dialogs.Count -gt 0) {
    $enc = [System.Text.Encoding]::GetEncoding(932)
    [IO.File]::WriteAllText($errFile, (($dialogs -join "`r`n----`r`n") + "`r`n"), $enc)
  }
}

if (Get-Process -Name Src -ErrorAction SilentlyContinue) {
  throw 'Src.exe がすでに起動している。終わらせてから流す'
}
$cases = @(Join-Path $PackDir 'probe\probe.eve') +
  @(Get-ChildItem (Join-Path $PackDir 'probe-bounds') -Filter *.eve | Sort-Object Name | ForEach-Object FullName) +
  @(Get-ChildItem (Join-Path $PackDir 'probe-errors') -Directory | Sort-Object Name | ForEach-Object { Join-Path $_.FullName 'probe.eve' })
$i = 0
foreach ($eve in $cases) {
  $i++
  Write-Host "[$i/$($cases.Count)] $($eve.Substring($PackDir.Length).TrimStart('\'))"
  Invoke-Case $eve
}
Write-Host '終わり。probe_pack.py import でこのフォルダを取り込む'
