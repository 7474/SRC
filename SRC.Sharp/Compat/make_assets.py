#!/usr/bin/env python3
# SPDX-License-Identifier: MIT
# Copyright (c) 2026 koudenpa
"""
互換性の検証用のシナリオ「トレス島試験記録」（torres/）の素材を作る（#1549）。

- torres/Data/System/terrain.txt: 地形データ（番号 1000 番台。SRC 本体の地形データと重ならない）
- torres/Bitmap/Map/<名前>/<名前>0000.bmp: 地形の画像（32×32、8 ビット）。色と記号で地形を見分ける
- torres/Bitmap/Unit/cp_u_*.bmp: ユニットの画像（32×32、8 ビット、白が透過色）。味方は青、敵は赤。形と英字で機体を見分ける
- torres/Bitmap/Pilot/cp_*.bmp: 顔の画像（64×64、8 ビット）。背景の色と頭文字で人を見分ける
- torres/Bitmap/Cutin/cp_cut_*.bmp: カットインの画像
- torres/Sound/cp_*.wav: 効果音（22050 Hz・16 ビット・モノラル）。波形を数式で合成する
- torres/Midi/cp_*.mid: BGM（標準 MIDI ファイル、フォーマット 0）。決まった音の並びを書く
- torres/ep*.map: マップ。下の MAPS の文字の絵から作る

使い方: python3 make_assets.py（Pillow が必要）。乱数を使わないので、何度作っても同じになる。
文字は Pillow のフォントに頼らず、このファイルの 5×7 の点の字で描く（Pillow の版で絵が変わらないように）。
"""
import math
import os
import struct
import wave

from PIL import Image, ImageDraw

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.join(HERE, "torres")

# ---------------------------------------------------------------
# 地形
# ---------------------------------------------------------------
# 文字, 番号, 名前, 画像の名前, 地形タイプ, 移動コスト, 回避修正, ダメージ修正, 地形効果, 地の色, 記号の色, 記号
TERRAINS = [
    (".", 1000, "草原", "cp_grass", "陸", "1", 0, 0, [], (96, 176, 72), (64, 136, 48), "dots"),
    ("f", 1001, "林", "cp_forest", "陸", "2", 15, 10, [], (48, 120, 48), (16, 72, 24), "trees"),
    ("^", 1002, "岩山", "cp_mount", "陸", "3", 30, 30, [], (150, 120, 80), (90, 64, 40), "peak"),
    ("=", 1003, "道", "cp_road", "陸", "1", 0, 0, [], (176, 168, 150), (130, 124, 110), "lane"),
    ("~", 1004, "浅瀬", "cp_shoal", "水", "2", 5, 5, [], (110, 180, 220), (60, 130, 190), "wave"),
    ("w", 1005, "深い水路", "cp_deep", "深水", "2", 20, 20, [], (30, 70, 150), (90, 130, 210), "wave"),
    ("B", 1006, "学舎", "cp_base", "陸", "2", 25, 25, ["ＨＰ回復Lv2", "ＥＮ回復Lv2"], (230, 150, 60), (120, 60, 20), "H"),
    ("|", 1007, "断崖", "cp_cliff", "空", "1", 0, 0, [], (120, 110, 130), (70, 60, 80), "stripe"),
    ("#", 1008, "防壁", "cp_wall", "空", "-", 0, 0, [], (60, 60, 64), (110, 110, 116), "brick"),
    ("r", 1009, "錆の原", "cp_rust", "陸", "2", 5, 5, ["ＨＰ減少Lv1"], (170, 90, 50), (110, 50, 30), "spots"),
    ("s", 1010, "砂地", "cp_sand", "陸", "1", 0, 0, [], (226, 206, 140), (190, 168, 100), "dots"),
    ("b", 1011, "橋", "cp_bridge", "陸", "1", 0, 5, [], (150, 100, 60), (90, 60, 30), "plank"),
    ("S", 1012, "宇宙", "cp_space", "宇宙", "1", 0, 0, [], (10, 10, 40), (220, 220, 160), "stars"),
    ("d", 1013, "暗礁", "cp_debris", "宇宙", "2", 20, 20, [], (30, 30, 60), (130, 120, 110), "rocks"),
    ("m", 1014, "月面", "cp_moon", "月面", "1", 0, 0, [], (170, 170, 176), (120, 120, 128), "crater"),
    ("F", 1015, "床", "cp_floor", "屋内", "1", 0, 0, [], (190, 190, 200), (150, 150, 160), "grid"),
    ("k", 1016, "空", "cp_sky", "空", "1", 0, 0, [], (150, 200, 250), (250, 250, 250), "cloud"),
    ("t", 1017, "坑道", "cp_tunnel", "陸", "2", 10, 10, [], (90, 70, 50), (40, 30, 20), "arch"),
]

# ---------------------------------------------------------------
# マップ（文字の絵。1 行が y、1 文字が x。左上が (1,1)）
# ---------------------------------------------------------------
MAPS = {
    # 第一話「試験場の朝」20×16。左に学舎、右から錆蟲が来る
    "ep01": [
        "....ff......^^^^rrrr",
        ".BB.ff..........rr.r",
        ".BB....=====....rrrr",
        ".......=...=........",
        "..ff...=...=...ff...",
        "..ff...=...=...ff..r",
        "=======b===========.",
        "......~~~...........",
        ".....~~w~~....^^....",
        "......~~~.....^^.r..",
        "..f.........f.......",
        "..f...==========....",
        ".........ff....rr...",
        ".BB......ff....rrr..",
        ".BB.............rr..",
        "....................",
    ],
    # 第二話「錆の巣」22×18。深い水路と断崖の向こうに錆の巣。橋か浅瀬を渡るか、飛んで越える
    "ep02": [
        "BB..ff....ww||rrrrrrrr",
        "BB..ff....ww||rrrrrrrr",
        "..........ww||rr..rrrr",
        "..==......ww||rr.rr.rr",
        "..=.......~~..rrrrrrrr",
        "..=..ff...~~..rrrrrrrr",
        "..=..ff...~~..rrrrrrrr",
        "..========bb==rrrrrrrr",
        "..=.......~~..rrrrrrrr",
        "..=..^^...~~..rrrrrrrr",
        "..=..^^...ww||rrrrrrrr",
        "..........ww||rrrrrrrr",
        ".ff.......ww||rr..rrrr",
        ".ff.......ww||rrrrrrrr",
        "..........ww||rrrrrrrr",
        "..ss......ww||rrrrrrrr",
        ".sss......ww||rrrrrrrr",
        "..ss......ww||rrrrrrrr",
    ],
}

# ---------------------------------------------------------------
# 5×7 の点の字（英字・数字の一部）
# ---------------------------------------------------------------
FONT = {
    "A": ["01110", "10001", "10001", "11111", "10001", "10001", "10001"],
    "B": ["11110", "10001", "10001", "11110", "10001", "10001", "11110"],
    "C": ["01111", "10000", "10000", "10000", "10000", "10000", "01111"],
    "D": ["11110", "10001", "10001", "10001", "10001", "10001", "11110"],
    "G": ["01111", "10000", "10000", "10011", "10001", "10001", "01111"],
    "H": ["10001", "10001", "10001", "11111", "10001", "10001", "10001"],
    "I": ["11111", "00100", "00100", "00100", "00100", "00100", "11111"],
    "K": ["10001", "10010", "10100", "11000", "10100", "10010", "10001"],
    "M": ["10001", "11011", "10101", "10101", "10001", "10001", "10001"],
    "N": ["10001", "11001", "10101", "10011", "10001", "10001", "10001"],
    "R": ["11110", "10001", "10001", "11110", "10100", "10010", "10001"],
    "S": ["01111", "10000", "10000", "01110", "00001", "00001", "11110"],
    "T": ["11111", "00100", "00100", "00100", "00100", "00100", "00100"],
    "W": ["10001", "10001", "10001", "10101", "10101", "11011", "10001"],
    "Y": ["10001", "10001", "01010", "00100", "00100", "00100", "00100"],
    "X": ["10001", "10001", "01010", "00100", "01010", "10001", "10001"],
}


def text(d, x, y, s, color, scale=1):
    for n, ch in enumerate(s):
        rows = FONT[ch]
        for j, row in enumerate(rows):
            for i, bit in enumerate(row):
                if bit == "1":
                    px = x + (n * 6 + i) * scale
                    py = y + j * scale
                    d.rectangle([px, py, px + scale - 1, py + scale - 1], fill=color)


def path(*parts):
    full = os.path.join(ROOT, *parts)
    os.makedirs(os.path.dirname(full), exist_ok=True)
    return full


def write_text(rel, content):
    # リポジトリの中では UTF-8・LF で持つ。元の SRC に渡すときは export_sjis.py で Shift_JIS・CRLF にする
    with open(path(*rel.split("/")), "w", encoding="utf-8", newline="\n") as f:
        f.write(content)


def save_bmp(image, rel):
    # 256 色以下の 8 ビットの BMP（元の SRC は透過表示に 256 色以下の画像を使う）
    image.convert("RGB").quantize(colors=64, method=Image.Quantize.MEDIANCUT, dither=Image.Dither.NONE).save(path(*rel.split("/")), "BMP")


# ---------------------------------------------------------------
# 地形の画像・地形データ・マップ
# ---------------------------------------------------------------
def tile(base, mark, kind):
    im = Image.new("RGB", (32, 32), base)
    d = ImageDraw.Draw(im)
    if kind == "dots":
        for y in range(4, 32, 8):
            for x in range(4 + (y // 8 % 2) * 4, 32, 8):
                d.point((x, y), fill=mark)
    elif kind == "trees":
        for cx, cy in ((9, 12), (23, 12), (16, 26)):
            d.polygon([(cx, cy - 8), (cx - 6, cy + 4), (cx + 6, cy + 4)], fill=mark)
    elif kind == "peak":
        d.polygon([(16, 4), (3, 28), (29, 28)], fill=mark)
        d.polygon([(16, 4), (12, 12), (20, 12)], fill=(240, 240, 240))
    elif kind == "lane":
        d.rectangle([0, 0, 31, 2], fill=mark)
        d.rectangle([0, 29, 31, 31], fill=mark)
        for x in range(2, 32, 10):
            d.rectangle([x, 15, x + 5, 16], fill=(240, 230, 120))
    elif kind == "wave":
        for y in (8, 18, 28):
            for x in range(0, 32, 8):
                d.arc([x, y - 4, x + 8, y + 4], 180, 360, fill=mark)
    elif kind == "H":
        d.rectangle([2, 2, 29, 29], outline=mark)
        text(d, 11, 9, "H", mark, 2)
    elif kind == "stripe":
        for x in range(-32, 32, 6):
            d.line([(x, 31), (x + 31, 0)], fill=mark, width=2)
    elif kind == "brick":
        for y in range(0, 32, 8):
            d.line([(0, y), (31, y)], fill=mark)
            off = 0 if y // 8 % 2 == 0 else 8
            for x in range(off, 32, 16):
                d.line([(x, y), (x, y + 7)], fill=mark)
    elif kind == "spots":
        for cx, cy, r in ((8, 8, 3), (22, 10, 2), (14, 22, 4), (26, 26, 2), (5, 27, 2)):
            d.ellipse([cx - r, cy - r, cx + r, cy + r], fill=mark)
    elif kind == "plank":
        for y in range(0, 32, 6):
            d.line([(0, y), (31, y)], fill=mark, width=2)
    elif kind == "stars":
        for x, y in ((5, 6), (20, 3), (27, 17), (12, 20), (3, 28), (24, 28), (16, 11)):
            d.point((x, y), fill=mark)
    elif kind == "rocks":
        for cx, cy, r in ((9, 10, 5), (23, 20, 6), (10, 26, 3)):
            d.ellipse([cx - r, cy - r, cx + r, cy + r], fill=mark)
    elif kind == "crater":
        for cx, cy, r in ((11, 11, 6), (24, 23, 5)):
            d.ellipse([cx - r, cy - r, cx + r, cy + r], outline=mark, width=2)
    elif kind == "grid":
        for v in range(0, 32, 8):
            d.line([(v, 0), (v, 31)], fill=mark)
            d.line([(0, v), (31, v)], fill=mark)
    elif kind == "cloud":
        for cx, cy, r in ((10, 16, 6), (18, 13, 7), (24, 18, 5)):
            d.ellipse([cx - r, cy - r, cx + r, cy + r], fill=(254, 254, 254))
    elif kind == "arch":
        d.rectangle([0, 0, 31, 31], outline=mark, width=3)
        d.pieslice([6, 8, 26, 40], 180, 360, fill=mark)
    return im


def terrains():
    lines = [
        "# トレス島試験記録 地形データ（make_assets.py が作る）",
        "# 番号は 1000 番台（シナリオ側で足す地形の番号）。SRC 本体の地形データと重ならない。",
        "",
    ]
    for _ch, num, name, bmp, ttype, cost, hit, dmg, effects, base, mark, kind in TERRAINS:
        lines += [str(num), f"{name}, {bmp}", f"{ttype}, {cost}, {hit}, {dmg}"] + effects + [""]
        save_bmp(tile(base, mark, kind), f"Bitmap/Map/{bmp}/{bmp}0000.bmp")
    write_text("Data/System/terrain.txt", "\n".join(lines))


def maps():
    numbers = {t[0]: t[1] for t in TERRAINS}
    for name, rows in MAPS.items():
        width, height = len(rows[0]), len(rows)
        assert all(len(r) == width for r in rows), name
        assert width >= 15 and height >= 15, name
        out = ["MapData", "", f"{width},{height}"]
        # 元の SRC の LoadMapData と同じく、x（列）ごとに y（行）を並べる
        for x in range(width):
            for y in range(height):
                out.append(f"{numbers[rows[y][x]]},0")
        write_text(f"{name}.map", "\n".join(out) + "\n")


# ---------------------------------------------------------------
# ユニット・顔・カットイン
# ---------------------------------------------------------------
BLUE = (40, 90, 200)
RED = (200, 50, 40)
EDGE = (16, 16, 24)

# 画像の名前, 陣営の色, 形, 英字
UNITS = [
    ("cp_u_tobiuo", BLUE, "fish", "T"),
    ("cp_u_tobiuo_f", BLUE, "arrow", "T"),
    ("cp_u_iwagane", BLUE, "block", "I"),
    ("cp_u_mikazuchi", BLUE, "diamond", "M"),
    ("cp_u_kasumi", BLUE, "circle", "K"),
    ("cp_u_yamase", BLUE, "ship", "Y"),
    ("cp_u_hibari", BLUE, "bird", "H"),
    ("cp_u_worker", RED, "bug", "W"),
    ("cp_u_spitter", RED, "bug", "S"),
    ("cp_u_bomber", RED, "hexagon", "B"),
    ("cp_u_guard", RED, "shield", "G"),
    ("cp_u_chrome", RED, "star", "C"),
    ("cp_u_nest", RED, "nest", "N"),
    ("cp_u_target", RED, "target", "X"),
]


def unit(color, shape, letter):
    im = Image.new("RGB", (32, 32), (255, 255, 255))
    d = ImageDraw.Draw(im)
    if shape == "fish":
        d.polygon([(3, 16), (16, 6), (26, 16), (16, 26)], fill=color, outline=EDGE)
        d.polygon([(26, 16), (31, 9), (31, 23)], fill=color, outline=EDGE)
    elif shape == "arrow":
        d.polygon([(2, 16), (30, 3), (22, 16), (30, 29)], fill=color, outline=EDGE)
    elif shape == "block":
        d.rectangle([5, 5, 26, 26], fill=color, outline=EDGE)
    elif shape == "diamond":
        d.polygon([(16, 1), (30, 16), (16, 30), (1, 16)], fill=color, outline=EDGE)
    elif shape == "circle":
        d.ellipse([4, 4, 27, 27], fill=color, outline=EDGE)
    elif shape == "ship":
        d.polygon([(1, 12), (25, 12), (31, 18), (25, 24), (1, 24)], fill=color, outline=EDGE)
        d.rectangle([8, 6, 18, 12], fill=color, outline=EDGE)
    elif shape == "bird":
        d.polygon([(1, 5), (16, 12), (31, 5), (26, 26), (16, 30), (6, 26)], fill=color, outline=EDGE)
    elif shape == "bug":
        for y in (10, 16, 22):
            d.line([(3, y), (29, y)], fill=EDGE, width=1)
        d.ellipse([7, 6, 25, 26], fill=color, outline=EDGE)
    elif shape == "hexagon":
        d.polygon([(9, 3), (23, 3), (30, 16), (23, 29), (9, 29), (2, 16)], fill=color, outline=EDGE)
    elif shape == "shield":
        d.polygon([(4, 4), (28, 4), (28, 16), (16, 30), (4, 16)], fill=color, outline=EDGE)
    elif shape == "star":
        pts = []
        for k in range(10):
            r = 15 if k % 2 == 0 else 7
            a = math.pi / 2 + k * math.pi / 5
            pts.append((16 + r * math.cos(a), 16 - r * math.sin(a)))
        d.polygon(pts, fill=color, outline=EDGE)
    elif shape == "nest":
        d.ellipse([1, 4, 30, 30], fill=color, outline=EDGE)
        d.ellipse([10, 1, 22, 12], fill=color, outline=EDGE)
    elif shape == "target":
        for r, c in ((14, color), (10, (240, 220, 200)), (6, color)):
            d.ellipse([16 - r, 16 - r, 16 + r, 16 + r], fill=c, outline=EDGE)
    # 白は透過色なので、字は真っ白にしない
    text(d, 14 - 1, 12, letter, (254, 254, 200))
    return im


# 画像の名前, 背景の色, 髪の色, 頭文字, 表情
FACES = [
    ("cp_kai", (70, 110, 200), (60, 40, 30), "K", "normal"),
    ("cp_kai_smile", (70, 110, 200), (60, 40, 30), "K", "smile"),
    ("cp_noa", (200, 110, 160), (230, 220, 200), "N", "normal"),
    ("cp_noa_angry", (200, 110, 160), (230, 220, 200), "N", "angry"),
    ("cp_daichi", (110, 160, 70), (30, 30, 30), "D", "normal"),
    ("cp_goro", (120, 120, 120), (200, 200, 200), "G", "normal"),
    ("cp_rin", (90, 170, 170), (40, 60, 120), "R", "normal"),
    ("cp_mizuho", (150, 100, 60), (90, 60, 120), "M", "normal"),
    ("cp_control", (60, 60, 90), (20, 20, 20), "Y", "normal"),
    ("cp_rust", (130, 60, 40), None, "X", "machine"),
    ("cp_guard", (110, 70, 50), None, "G", "machine"),
    ("cp_chrome", (60, 60, 70), None, "C", "machine"),
]


def face(bg, hair, letter, mood):
    im = Image.new("RGB", (64, 64), bg)
    d = ImageDraw.Draw(im)
    if mood == "machine":
        d.rectangle([14, 14, 50, 54], fill=(80, 80, 88), outline=EDGE, width=2)
        d.rectangle([20, 26, 30, 32], fill=(255, 200, 40))
        d.rectangle([34, 26, 44, 32], fill=(255, 200, 40))
        d.line([(22, 44), (42, 44)], fill=EDGE, width=2)
    else:
        d.ellipse([14, 12, 50, 56], fill=(240, 200, 170), outline=EDGE)
        d.pieslice([12, 8, 52, 40], 180, 360, fill=hair)
        d.ellipse([23, 30, 27, 34], fill=EDGE)
        d.ellipse([37, 30, 41, 34], fill=EDGE)
        if mood == "smile":
            d.arc([24, 38, 40, 50], 20, 160, fill=EDGE, width=2)
        elif mood == "angry":
            d.line([(21, 26), (29, 29)], fill=EDGE, width=2)
            d.line([(43, 26), (35, 29)], fill=EDGE, width=2)
            d.arc([24, 44, 40, 54], 200, 340, fill=EDGE, width=2)
        else:
            d.line([(27, 46), (37, 46)], fill=EDGE, width=2)
    text(d, 3, 3, letter, (254, 254, 254), 2)
    return im


def cutin():
    im = Image.new("RGB", (128, 64), (255, 255, 255))
    d = ImageDraw.Draw(im)
    d.line([(8, 8), (120, 56)], fill=(40, 90, 200), width=8)
    d.line([(8, 56), (120, 8)], fill=(200, 160, 40), width=8)
    text(d, 52, 26, "X", (200, 40, 40), 2)
    return im


def images():
    for name, color, shape, letter in UNITS:
        save_bmp(unit(color, shape, letter), f"Bitmap/Unit/{name}.bmp")
    for name, bg, hair, letter, mood in FACES:
        save_bmp(face(bg, hair, letter, mood), f"Bitmap/Pilot/{name}.bmp")
    save_bmp(cutin(), "Bitmap/Cutin/cp_cut_cross.bmp")


# ---------------------------------------------------------------
# 効果音
# ---------------------------------------------------------------
RATE = 22050


def lcg_noise(n, seed):
    # 擬似乱数（線形合同法）。乱数のモジュールに頼らず、どの環境でも同じ波形にする
    out, x = [], seed
    for _ in range(n):
        x = (x * 1103515245 + 12345) & 0x7FFFFFFF
        out.append(x / 0x3FFFFFFF - 1.0)
    return out


def tone(freq_from, freq_to, dur, shape="sine"):
    n = int(RATE * dur)
    out, phase = [], 0.0
    for i in range(n):
        f = freq_from + (freq_to - freq_from) * i / n
        phase += 2 * math.pi * f / RATE
        if shape == "square":
            v = 1.0 if math.sin(phase) >= 0 else -1.0
        elif shape == "saw":
            v = (phase / math.pi) % 2 - 1
        else:
            v = math.sin(phase)
        out.append(v)
    return out


def envelope(samples, attack=0.01):
    n = len(samples)
    a = max(1, int(RATE * attack))
    return [s * min(1.0, i / a) * (1 - i / n) ** 2 for i, s in enumerate(samples)]


def lowpass(samples, k):
    out, y = [], 0.0
    for s in samples:
        y += k * (s - y)
        out.append(y)
    return out


def mix(*tracks):
    n = max(len(t) for t in tracks)
    return [sum(t[i] for t in tracks if i < len(t)) for i in range(n)]


def save_wav(samples, name):
    peak = max(abs(s) for s in samples) or 1.0
    with wave.open(path("Sound", name + ".wav"), "wb") as w:
        w.setnchannels(1)
        w.setsampwidth(2)
        w.setframerate(RATE)
        w.writeframes(b"".join(struct.pack("<h", int(s / peak * 0.8 * 32767)) for s in samples))


def sounds():
    save_wav(envelope(tone(300, 1200, 0.4, "saw")), "cp_charge")
    save_wav(envelope(lowpass(lcg_noise(int(RATE * 0.2), 1), 0.5)), "cp_slash")
    save_wav(envelope(mix(lowpass(lcg_noise(int(RATE * 0.7), 2), 0.08), tone(90, 40, 0.7))), "cp_explode")
    save_wav(envelope(tone(1400, 500, 0.35, "square")), "cp_beam")
    save_wav(envelope(mix(lowpass(lcg_noise(int(RATE * 0.12), 3), 0.6), tone(700, 200, 0.12))), "cp_shot")
    save_wav(envelope(lowpass(lcg_noise(int(RATE * 0.35), 4), 0.15), 0.1), "cp_dash")
    save_wav(envelope(mix(lowpass(lcg_noise(int(RATE * 0.1), 5), 0.3), tone(200, 120, 0.1, "square"))), "cp_bite")
    save_wav(envelope(tone(500, 500, 0.25, "square")), "cp_guard")
    save_wav(envelope(mix(tone(660, 660, 0.15), tone(880, 880, 0.3))), "cp_repair")
    save_wav(envelope(mix(tone(200, 800, 0.5, "square"), tone(400, 1600, 0.5))), "cp_transform")
    save_wav(envelope(mix(tone(523, 523, 0.15), tone(659, 659, 0.3), tone(784, 784, 0.45))), "cp_levelup")


# ---------------------------------------------------------------
# BGM
# ---------------------------------------------------------------
TPQ = 480


def vlq(n):
    out = [n & 0x7F]
    n >>= 7
    while n:
        out.insert(0, (n & 0x7F) | 0x80)
        n >>= 7
    return bytes(out)


def save_midi(name, tempo, program, melody, bass, loops=2):
    """melody・bass: (音の高さ（MIDI の番号。0 は休符）, 長さ（4 分音符 = 1）) の並び"""
    events = [(0, 0xC0, program), (0, 0xC1, 33)]
    for ch, part, vel in ((0, melody, 96), (1, bass, 80)):
        t = 0
        for _ in range(loops):
            for key, beats in part:
                length = int(beats * TPQ)
                if key:
                    events.append((t, 0x90 | ch, key, vel))
                    events.append((t + length - 10, 0x80 | ch, key, 0))
                t += length
    body = b"\x00\xff\x51\x03" + int(60_000_000 / tempo).to_bytes(3, "big")
    now = 0
    for ev in sorted(events, key=lambda e: (e[0], 0 if (e[1] & 0xF0) == 0x80 else 1)):
        body += vlq(ev[0] - now) + bytes(ev[1:])
        now = ev[0]
    body += b"\x00\xff\x2f\x00"
    with open(path("Midi", name + ".mid"), "wb") as f:
        f.write(b"MThd" + struct.pack(">IHHH", 6, 0, 1, TPQ))
        f.write(b"MTrk" + struct.pack(">I", len(body)) + body)


def music():
    # 長調の明るい曲（ブリーフィング・インターミッション）、短調の曲（味方・敵のフェイズ）
    save_midi("cp_briefing", 96, 0,
              [(60, 1), (64, 1), (67, 1), (72, 1), (71, 1), (67, 1), (64, 2), (65, 1), (69, 1), (72, 1), (69, 1), (67, 4)],
              [(48, 4), (53, 4), (48, 4), (55, 4)])
    save_midi("cp_intermission", 88, 4,
              [(67, 1), (69, 1), (71, 2), (72, 1), (71, 1), (69, 2), (67, 1), (64, 1), (62, 2), (60, 4)],
              [(43, 4), (48, 4), (43, 4), (48, 4)])
    save_midi("cp_map", 132, 81,
              [(57, 0.5), (60, 0.5), (64, 1), (62, 0.5), (60, 0.5), (57, 1), (55, 0.5), (57, 0.5), (60, 1), (64, 2),
               (65, 0.5), (64, 0.5), (62, 1), (60, 0.5), (62, 0.5), (64, 2), (57, 2)],
              [(45, 2), (45, 2), (41, 2), (43, 2), (45, 2), (41, 2), (43, 2), (45, 2)])
    save_midi("cp_enemy", 120, 30,
              [(52, 1), (55, 1), (58, 1), (55, 1), (53, 1), (52, 1), (50, 2), (52, 1), (58, 1), (57, 1), (55, 1), (52, 4)],
              [(40, 2), (40, 2), (38, 2), (38, 2), (40, 2), (40, 2), (35, 4)])
    save_midi("cp_boss", 144, 29,
              [(50, 0.5), (50, 0.5), (53, 0.5), (50, 0.5), (56, 1), (55, 1), (50, 0.5), (50, 0.5), (53, 0.5), (50, 0.5), (58, 1), (57, 1)],
              [(38, 1), (38, 1), (38, 1), (38, 1), (36, 1), (36, 1), (37, 1), (37, 1)])


def main():
    terrains()
    maps()
    images()
    sounds()
    music()


if __name__ == "__main__":
    main()
