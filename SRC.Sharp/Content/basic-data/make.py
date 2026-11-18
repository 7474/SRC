#!/usr/bin/env python3
# SPDX-License-Identifier: MIT
# Copyright (c) 2026 koudenpa
"""
SRC# の最小の汎用データ（basic-data）を作る。

SRC のフルパッケージの汎用データ（グラフィック集など）は再配布できないため（docs/decisions/2026-10-12-asset-redistribution.md）、
同じファイル名で差し替えられる素材を自作する。ライセンスは ../README.md の「ライセンス」を参照（このスクリプトは MIT、作る素材は CC0 1.0）。

- Data/System/terrain.txt: 地形データ。地形の番号と名前は汎用データと同じにする（汎用データ用のマップをそのまま使える）
- Data/System/sp.txt: スペシャルパワーデータ。ヘルプ（スペシャルパワー.md）の効果の説明から書いた
- Bitmap/Map/<名前>/<名前><番号:04d>.bmp: 地形の画像（32×32。基本 0000 と、汎用グラフィック集と同じ意味の変種・境目）
- Bitmap/Unit/OBJ_*.bmp: ユニットの画像（32×32、白が透過色）
- Sound/*.wav: 効果音（汎用効果音集と同じ名前）。ノイズと波形を数式で合成する（録音や音声生成 AI は使わない）
- Midi/*.mid: SRC が場面ごとに鳴らす BGM（Opening・Briefing・Intermission・Subtitle・Map1〜6・default）。規則で音符を並べて作曲する
- Lib/ユニットステータス表示.eve・Lib/パイロットステータス表示.eve: インターミッションの「ユニットステータス」「パイロットステータス」。
  ヘルプ（ユニットステータス.md・MakeUnitListコマンド.md など）から書いた手書きのファイルで、このスクリプトは作らない

使い方: python3 make.py（Pillow が必要）。出力はこのフォルダに書く。乱数の種を固定しているので、何度作っても同じになる。
"""
import math
import os
import random
import struct
import wave

from PIL import Image, ImageDraw

HERE = os.path.dirname(os.path.abspath(__file__))
SIZE = 32

# 番号, 名前, 画像の名前, 地形タイプ, 移動コスト, 命中修正, ダメージ修正, 特殊効果
TERRAINS = [
    (0, "平地", "plain", "陸", 1, 0, 0, ""),
    (2, "道路", "road", "陸", 1, 0, 5, ""),
    (10, "荒れ地", "waste_land", "陸", 1, 0, 0, ""),
    (11, "林", "woods", "陸", 2, 15, 10, ""),
    (15, "山", "mountain", "陸", 3, 30, 30, ""),
    (17, "海", "sea", "水", 2, 5, 5, ""),
    (19, "川", "river", "水", 2, 5, 5, ""),
    (21, "街", "town", "陸", 2, 25, 25, "ＥＮ回復Lv1"),
    (22, "ビル", "building", "陸", 2, 25, 25, "ＥＮ回復Lv1"),
    (25, "橋", "bridge", "陸", 1, 0, 5, ""),
    (29, "岩", "rock", "陸", 2, 5, 5, ""),
    (31, "丘", "hill", "陸", 2, 5, 5, ""),
    (32, "低木", "bush", "陸", 2, 5, 5, ""),
    (33, "基地", "base", "陸", 2, 30, 30, "ＨＰ回復Lv1 ＥＮ回復Lv2"),
    (42, "遺跡", "ruins", "陸", 2, 20, 20, ""),
    (47, "城塞", "castle", "陸", 2, 30, 30, "ＨＰ回復Lv1 ＥＮ回復Lv2"),
    (48, "発電所", "powerplant", "陸", 2, 15, 15, "ＥＮ回復Lv2"),
    (49, "廃墟", "remains", "陸", 2, 20, 20, ""),
    (83, "マグマ", "lava", "陸", 3, -10, 10, "ＨＰ減少Lv3=火"),
    # 宇宙・月面（7474/SRC#1659。かんたん作成の宇宙の場面）。番号・名前・地形タイプ・修正は汎用データと同じ
    (5, "宇宙空間", "space", "宇宙", 1, 0, 0, ""),
    (12, "暗礁空間", "asteroids", "宇宙", 2, 20, 20, ""),
    (13, "残骸", "wrecks", "宇宙", 2, 20, 20, ""),
    (28, "コロニー", "colony", "宇宙", 2, 20, 20, "ＥＮ回復Lv1"),
    (35, "宇宙要塞", "space_fortress", "宇宙", 2, 30, 30, "ＨＰ回復Lv1 ＥＮ回復Lv2"),
    (36, "宇宙ドック", "space_dock", "宇宙", 2, 10, 10, "ＨＰ回復Lv2 ＥＮ回復Lv3"),
    (6, "地球", "earth", "宇宙", 1, 0, 0, ""),
    (7, "月", "moon", "宇宙", 1, 0, 0, ""),
    (8, "月面", "moon_ground", "月面", 1, 0, 0, ""),
    (14, "クレーター(小)", "small_crater", "月面", 2, 20, 20, ""),
    (24, "月面都市", "moon_city", "月面", 2, 25, 25, "ＥＮ回復Lv1"),
    (62, "月面基地", "moon_base", "月面", 2, 30, 30, "ＨＰ回復Lv1 ＥＮ回復Lv2"),
]

# 名称, 読み, 短縮名, 消費ＳＰ, 対象, 持続期間, 適用条件, 効果, 解説
SPECIAL_POWERS = [
    ("加速", "かそく", "加", 10, "自分", "ターン", "-", "移動力強化Lv2", "１ターンの間、移動力+2"),
    ("集中", "しゅうちゅう", "集", 15, "自分", "ターン", "-", "命中強化Lv3 回避強化Lv3", "１ターンの間、命中＆回避+30"),
    ("ひらめき", "", "閃", 15, "自分", "防御", "-", "絶対回避", "相手の命中率を１回だけ0%にする"),
    ("必中", "ひっちゅう", "必", 25, "自分", "ターン", "-", "絶対命中", "１ターンの間、命中率が100%になる（ひらめきには無効）"),
    ("根性", "こんじょう", "根", 20, "自分", "即効", "-", "ＨＰ回復Lv3.3", "ＨＰを最大ＨＰの1/3回復"),
    ("ド根性", "どこんじょう", "ド", 40, "自分", "即効", "-", "ＨＰ回復Lv10", "ＨＰを全快する"),
    ("鉄壁", "てっぺき", "壁", 30, "自分", "ターン", "-", "装甲強化Lv10", "１ターンの間、装甲を2倍にする"),
    ("気合", "きあい", "気", 40, "自分", "即効", "-", "気力増加Lv1", "気力+10"),
    ("熱血", "ねっけつ", "熱", 40, "自分", "攻撃", "-", "ダメージ増加Lv10", "次の攻撃のダメージを2倍にする"),
    ("魂", "たましい", "魂", 60, "自分", "攻撃", "-", "ダメージ増加Lv20", "次の攻撃のダメージを3倍にする"),
    ("直撃", "ちょくげき", "直", 20, "自分", "攻撃", "-", "回避能力無効化 シールド防御無効化 サポートガード無効化",
     "次の攻撃で、相手の回避系の特殊能力・シールド防御・サポートガードを無効にする"),
    ("幸運", "こううん", "幸", 45, "自分", "敵破壊", "-", "獲得資金増加Lv10", "次に敵を破壊したとき、獲得資金が2倍"),
    ("信頼", "しんらい", "信", 30, "味方", "即効", "支援", "ＨＰ回復Lv3.3", "指定したユニットのＨＰを最大ＨＰの1/3回復"),
    ("激励", "げきれい", "励", 70, "全味方", "即効", "-", "気力増加Lv1", "味方全員の気力+10"),
    ("補給", "ほきゅう", "補", 70, "味方", "即効", "支援", "ＥＮ回復Lv10 装填 気力増加Lv-1",
     "指定したユニットのＥＮ・弾数を全快する。気力-10"),
    ("癒し", "いやし", "癒", 100, "味方", "即効", "支援", "ＳＰ回復Lv5", "指定したユニットのパイロットのＳＰを50回復"),
    ("祝福", "しゅくふく", "祝", 60, "味方", "敵破壊", "支援", "獲得資金増加Lv10", "指定したユニットに幸運の効果を与える"),
    ("祈り", "いのり", "祈", 120, "破壊味方", "即効", "-", "復活", "破壊されたユニットを１つ復活させる"),
    ("夢", "ゆめ", "夢", 0, "自分", "即効", "-", "味方スペシャルパワー実行",
     "出撃中の味方パイロットが持つスペシャルパワーを、消費ＳＰ２倍で実行する"),
    ("奇跡", "きせき", "奇", 100, "自分", "即効", "-",
     "スペシャルパワー=加速 スペシャルパワー=魂 スペシャルパワー=必中 スペシャルパワー=ひらめき "
     "スペシャルパワー=幸運 スペシャルパワー=ド根性 気力増加Lv3",
     "加速＋魂＋必中＋ひらめき＋幸運＋ド根性、気力+30"),
]

# ユニットの画像の名前（汎用データの Bitmap/Unit と同じ名前）
UNITS = [
    "OBJ_Airplane01", "OBJ_Beast01", "OBJ_BikeRider01", "OBJ_Bird01", "OBJ_Dinosaur01", "OBJ_Etc01",
    "OBJ_Etc02", "OBJ_Etc03", "OBJ_Etc04", "OBJ_Fish01", "OBJ_Horse01", "OBJ_HorseRider01", "OBJ_Human01",
    "OBJ_Robot01", "OBJ_Robot02", "OBJ_Robot03", "OBJ_SailShip01", "OBJ_SpaceShip01", "OBJ_SubMarine01",
    "OBJ_Tank01", "OBJ_UFO01", "OBJ_WingHuman01",
]


def write(path, text):
    full = os.path.join(HERE, path)
    os.makedirs(os.path.dirname(full), exist_ok=True)
    with open(full, "w", encoding="utf-8", newline="\r\n") as f:
        f.write(text)


def save(image, path):
    full = os.path.join(HERE, path)
    os.makedirs(os.path.dirname(full), exist_ok=True)
    # 8 ビットの BMP（汎用データと同じ形式）。色数が少ないので減色しても見た目は変わらない
    image.convert("RGB").quantize(colors=256, method=Image.Quantize.MEDIANCUT, dither=Image.Dither.NONE).save(full, "BMP")


def _clamp8(v):
    return 0 if v < 0 else (255 if v > 255 else int(round(v)))


def _mix(c1, c2, t):
    if t <= 0.0:
        return c1
    if t >= 1.0:
        return c2
    return (_clamp8(c1[0] + (c2[0] - c1[0]) * t),
            _clamp8(c1[1] + (c2[1] - c1[1]) * t),
            _clamp8(c1[2] + (c2[2] - c1[2]) * t))


def _shade(c, delta):
    return (_clamp8(c[0] + delta), _clamp8(c[1] + delta), _clamp8(c[2] + delta))


def _hash2(ix, iy, seed):
    h = (ix * 374761393 + iy * 668265263 + seed * 1442695041) & 0xFFFFFFFF
    h = ((h ^ (h >> 13)) * 1274126177) & 0xFFFFFFFF
    h = h ^ (h >> 16)
    return (h / 0xFFFFFFFF) * 2.0 - 1.0


def pnoise(x, y, freq, seed):
    """周期 SIZE (=32) で継ぎ目の出ない 2D バリューノイズ（[-1, 1]）"""
    fx = (x % SIZE) * freq / SIZE
    fy = (y % SIZE) * freq / SIZE
    ix = int(math.floor(fx))
    iy = int(math.floor(fy))
    tx = fx - ix
    ty = fy - iy
    ux = tx * tx * (3.0 - 2.0 * tx)
    uy = ty * ty * (3.0 - 2.0 * ty)
    x0 = ix % freq
    x1 = (ix + 1) % freq
    y0 = iy % freq
    y1 = (iy + 1) % freq
    n00 = _hash2(x0, y0, seed)
    n10 = _hash2(x1, y0, seed)
    n01 = _hash2(x0, y1, seed)
    n11 = _hash2(x1, y1, seed)
    nx0 = n00 + (n10 - n00) * ux
    nx1 = n01 + (n11 - n01) * ux
    return nx0 + (nx1 - nx0) * uy


def fbm(x, y, seed, base_freq=4, octaves=3):
    """継ぎ目の出ないマルチオクターブ・ノイズ（おおむね [-1, 1]）"""
    total = 0.0
    amp = 0.55
    norm = 0.0
    freq = base_freq
    for o in range(octaves):
        total += pnoise(x, y, freq, seed + o * 97) * amp
        norm += amp
        amp *= 0.5
        freq *= 2
    return total / norm


def _seed(key):
    h = 2166136261
    for ch in str(key).encode("utf-8"):
        h = ((h ^ ch) * 16777619) & 0xFFFFFFFF
    return h


def _draw_canvas(fn):
    im = Image.new("RGB", (SIZE, SIZE))
    px = im.load()
    for y in range(SIZE):
        for x in range(SIZE):
            px[x, y] = fn(x, y)
    return im


def plain_color(x, y, no=0):
    """明るい緑の草地。全地形の草地縁で共通して使うため継ぎ目が出ない"""
    n = fbm(x, y, 101, base_freq=4, octaves=3)
    fine = pnoise(x, y, 16, 131)
    # 1〜5: タイル内側にだけ濃い草・茂みの揺らぎを出す（境界の 4px は no=0 と一致させて継ぎ目を無くす）
    dark_bias = 0.0
    edge_dist = min(x, SIZE - 1 - x, y, SIZE - 1 - y)
    edge_fade = min(1.0, edge_dist / 4.0)
    if 1 <= no <= 5 and edge_fade > 0.0:
        nx = (x - 15.5) / 14.0
        ny = (y - 15.5) / 14.0
        if no == 1:
            dark_bias = max(0.0, -nx * 0.7 + 0.3)
        elif no == 2:
            dark_bias = max(0.0, (-nx + ny) * 0.55 + 0.2)
        elif no == 3:
            dark_bias = max(0.0, -nx * 0.6 + 0.25 + 0.15 * n)
        elif no == 4:
            dark_bias = max(0.0, (nx - ny) * 0.55 + 0.2)
        elif no == 5:
            dark_bias = max(0.0, nx * 0.6 + 0.25 + 0.15 * n)
        dark_bias = max(0.0, dark_bias - 0.15) * (0.6 + 0.4 * max(0.0, n)) * edge_fade
    base = (88, 158, 66)
    c = _shade(base, n * 16 + fine * 6 - dark_bias * 18)
    # さりげない草の穂（周期的シードで配置）
    h = _hash2(x, y, 177 + no * 19)
    if h > 0.945 and no != 0 and edge_fade > 0.5:
        c = _shade(c, -15)
    elif _hash2(x, y, 177) > 0.965:
        c = _shade(c, 16)
    return c


def water_color(x, y, deep=False, river=False):
    """青ユニット枠（#2f6fd6 等）と明度・色相で分離する落ち着いた青緑（ティール）の水面"""
    n = fbm(x, y, 211, base_freq=4, octaves=3)
    # わずかな横方向の波紋（周期 32 に合わせる）
    wave_v = math.sin((y / SIZE) * math.tau * 4 + n * 2.4 + math.sin((x / SIZE) * math.tau * 2) * 0.8)
    if deep:
        base = (19, 68, 80)
        c = _shade(base, n * 10 + wave_v * 5)
    elif river:
        base = (26, 94, 102)
        c = _shade(base, n * 11 + wave_v * 7)
    else:
        base = (24, 84, 94)
        c = _shade(base, n * 11 + wave_v * 7)
    # 小さな水面のきらめき（青過多にせず緑寄りティールに留める）
    if wave_v > 0.86 and n > 0.12:
        c = _mix(c, (96, 162, 160), 0.45)
    return c


def moon_color(x, y):
    """灰色の月面レゴリス"""
    n = fbm(x, y, 307, base_freq=4, octaves=3)
    grad = fbm((x - 1) % SIZE, (y - 1) % SIZE, 307, base_freq=4, octaves=3) - n
    return _shade((144, 146, 154), n * 16 - grad * 18)


def space_color(x, y, no=0):
    """黒地の宇宙背景とまばらな星"""
    neb = fbm(x, y, 401, base_freq=2, octaves=2)
    base = (8 + max(0, int((neb + 0.5) * 6)), 11 + max(0, int((neb + 0.5) * 6)), 28 + max(0, int((neb + 0.5) * 10)))
    cell_x, cell_y = x // 8, y // 8
    sx = (cell_x * 8 + int((_hash2(cell_x, cell_y, 409 + no * 31) * 0.5 + 0.5) * 6) + 1) % SIZE
    sy = (cell_y * 8 + int((_hash2(cell_x, cell_y, 433 + no * 37) * 0.5 + 0.5) * 6) + 1) % SIZE
    if (x, y) == (sx, sy) and _hash2(cell_x, cell_y, 449 + no * 13) > -0.15:
        tint = _hash2(cell_x, cell_y, 461 + no * 17)
        if tint > 0.4:
            return (248, 240, 200)
        if tint < -0.3:
            return (196, 212, 255)
        return (236, 240, 248)
    return base


def _side_dist(x, y, sides, width=7, corner_r=11):
    """
    指定した方角の「境界からの内側への距離」を返す。
    N/E/S/W は辺からの距離、NE/SE/SW/NW はその角からの距離の補数（内角用）。
    返り値 s > 0 なら「その方角の陸地/隣地形側」、0 前後が境界線。
    """
    wobble = fbm(x, y, 503, base_freq=4, octaves=2) * 1.6
    best = -99.0
    if "N" in sides:
        best = max(best, width - y + wobble)
    if "S" in sides:
        best = max(best, width - (SIZE - 1 - y) + wobble)
    if "W" in sides:
        best = max(best, width - x + wobble)
    if "E" in sides:
        best = max(best, width - (SIZE - 1 - x) + wobble)
    # 隣り合う 2 辺の場合は角のカーブを滑らかにする
    for cy_s, sy in (("N", 0), ("S", SIZE - 1)):
        for cx_s, sx in (("W", 0), ("E", SIZE - 1)):
            c_key = cy_s + cx_s
            if c_key in sides:
                dist = math.hypot(x - sx, y - sy)
                best = max(best, corner_r - dist + wobble * 0.8)
    return best


# basic-data が生成するマップチップ番号（docs/assets/map-chips/*.json に準拠）
TILE_NUMBERS = {
    "plain": [0, 1, 2, 3, 4, 5, 10, 11, 12, 13],
    "woods": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15],
    "sea": [0, 1, 2, 3, 4, 5, 6, 24, 25, 26, 27, 28, 29, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44],
    "river": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 82, 145, 146, 147, 150],
    "road": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
    "bridge": [0, 1, 2, 500, 501],
    "mountain": [0, 1, 2, 3, 4, 5, 49],
    "waste_land": [0, 17, 18, 19, 20, 21, 22, 23, 24],
    "lava": [0, 1, 2, 3, 4, 5, 6, 7, 8],
    "space": list(range(0, 14)),
    "asteroids": [0, 1, 2, 3],
    "town": [0, 1, 2],
    "moon_ground": [0, 1, 2],
    "small_crater": [0, 6, 7],
}

SEA_SIDES = {
    1: ("S",), 2: ("W",), 3: ("N",), 4: ("E",),
    24: ("E",), 25: ("N",), 26: ("W",), 27: ("S",),
    28: ("NW",), 29: ("NE",),
    31: ("SW",), 32: ("SE",), 33: ("NW",), 34: ("NE",),
    35: ("N", "W"), 36: ("SW",), 37: ("SE",),
    38: ("S", "W"), 39: ("N", "W"), 40: ("N", "E"), 41: ("N", "E"),
    42: ("S", "W"), 43: ("E", "S"), 44: ("N", "E", "S"),
}

RIVER_SIDES = {
    0: ("N", "W"), 1: ("N",), 2: ("N", "E"),
    3: ("W",), 4: ("NE", "SE", "SW", "NW"), 5: ("E",),
    6: ("S", "W"), 7: ("S",), 8: ("S", "E"),
    9: ("N", "S"), 10: ("N", "S"), 11: ("W", "E"),
    82: (),
}

WOODS_SIDES = {
    2: ("NE",), 3: ("S",), 4: ("S", "W"), 5: ("N",), 6: ("NE",),
    7: ("E", "S", "W"), 8: ("N", "E", "S", "W"), 9: ("NW",),
    10: ("N",), 11: ("E",), 12: ("S",), 13: ("W",),
}

MOUNTAIN_SIDES = {
    0: ("N", "W"), 1: ("N",), 2: ("SW",), 3: ("S", "W"), 4: ("S",), 5: ("SE",),
    49: ("N", "E", "S", "W"),
}

WASTE_SIDES = {
    17: ("N", "W"), 18: ("N",), 19: ("N", "E"),
    20: ("W",), 21: ("E",),
    22: ("S", "W"), 23: ("S",), 24: ("S", "E"),
}

LAVA_SIDES = {
    0: ("N", "W"), 1: ("N",), 2: ("N", "E"),
    3: ("W",), 4: (), 5: ("E",),
    6: ("S", "W"), 7: ("S",), 8: ("S", "E"),
}

ROAD_SIDES = {
    0: ("E", "W"), 1: ("N", "S"),
    2: ("S", "W"), 3: ("N", "W"), 4: ("N", "E"), 5: ("E", "S"),
    6: ("E", "S", "W"), 7: ("N", "S", "W"), 8: ("N", "E", "W"), 9: ("N", "E", "S"),
    10: ("N", "E", "S", "W"),
}


def _paint_trees(im, sides, variant):
    """森（woods）を描く。周囲の草地 edge を残しつつ、左上光・影付きの樹冠を重ねる"""
    px = im.load()
    d = ImageDraw.Draw(im)
    # まず草地背景で始める（木々の隙間も草地に馴染む）
    for y in range(SIZE):
        for x in range(SIZE):
            px[x, y] = plain_color(x, y)
    # 木を置く領域に濃い下草を敷く
    is_edge = bool(sides)
    for y in range(SIZE):
        for x in range(SIZE):
            sd = _side_dist(x, y, sides, width=6, corner_r=10) if is_edge else -5.0
            if sd < -0.5:
                px[x, y] = _mix(px[x, y], (36, 86, 36), 0.72)
            elif sd < 1.2:
                px[x, y] = _mix(px[x, y], (52, 114, 46), 0.45)

    # 樹冠の配置（境界を避けつつ自然に詰める）
    clusters = [
        (7, 7, 7), (17, 6, 7), (26, 8, 6),
        (5, 16, 7), (14, 15, 8), (24, 17, 7),
        (8, 25, 7), (18, 26, 7), (27, 25, 6),
    ]
    if variant == 1:
        clusters = [
            (9, 6, 7), (20, 7, 7), (27, 10, 6),
            (6, 15, 7), (16, 14, 8), (25, 18, 7),
            (9, 25, 7), (19, 25, 7), (26, 26, 6),
        ]
    for cx, cy, r in clusters:
        if is_edge and _side_dist(cx, cy, sides, width=7, corner_r=11) > 1.5:
            continue
        # 木の影（右下）
        d.ellipse([cx - r + 1, cy - r + 2, cx + r + 1, cy + r + 2], fill=(24, 58, 26))
        # 本体（深緑）
        d.ellipse([cx - r, cy - r, cx + r, cy + r], fill=(38, 106, 44), outline=(22, 62, 26))
        # 左上からのハイライト
        d.ellipse([cx - r + 2, cy - r + 1, cx + r - 3, cy + r - 4], fill=(64, 138, 56))
        d.ellipse([cx - r + 3, cy - r + 2, cx, cy], fill=(104, 176, 74))

    # 14: 木の実（赤い実）、15: 小さな花
    if variant == 14:
        for bx, by in [(8, 7), (15, 14), (24, 16), (11, 24), (20, 25), (18, 6)]:
            d.point([bx, by], fill=(216, 56, 52))
            d.point([bx + 1, by + 1], fill=(168, 36, 36))
    elif variant == 15:
        for fx, fy, fc in [(7, 8, (244, 224, 96)), (16, 15, (248, 244, 220)), (23, 17, (244, 224, 96)), (10, 24, (248, 244, 220)), (19, 25, (244, 210, 120))]:
            d.point([fx, fy], fill=fc)


def _paint_mountain(sides, standalone=False):
    def pixel(x, y):
        c = plain_color(x, y)
        sd = _side_dist(x, y, sides, width=5 if not standalone else 3, corner_r=8)
        if sd > 0.8:
            return c
        cx = 16.0
        cy = 17.0 if standalone else 16.0
        gx = fbm((x + 1) % SIZE, y, 601, base_freq=8, octaves=2) - fbm((x - 1) % SIZE, y, 601, base_freq=8, octaves=2)
        gy = fbm(x, (y + 1) % SIZE, 601, base_freq=8, octaves=2) - fbm(x, (y - 1) % SIZE, 601, base_freq=8, octaves=2)
        detail = fbm(x, y, 601, base_freq=4, octaves=3)
        if standalone:
            rx = abs(x - cx) / 13.0
            ry = abs(y - cy) / 13.0
            ridge = 1.0 - max(rx * 0.85 + ry * 0.55, math.hypot(rx, ry) * 0.78)
            h = ridge + detail * 0.18
            if h < 0.05:
                return c
            slope_light = ((cx - x) + (cy - y)) * 0.04 - (gx + gy) * 1.4
            if sd > -0.8:
                return _mix(c, (88, 74, 58), 0.65)
            if h > 0.64:
                return _shade((224, 226, 232), slope_light * 18)
            col = _shade((132, 114, 94), slope_light * 36 + detail * 18)
            return _mix(col, (76, 62, 48), 0.35) if h < 0.22 else col

        # 山脈タイル（0..5）: 周期ノイズによる岩の尾根がタイル間でそのままつながる
        ridge = 0.55 + detail * 0.35
        # 裾野（sides 方向）に近づくと高度が下がり、北西/南東の斜面陰影が付く
        if sd > -3.5:
            ridge -= (sd + 3.5) * 0.10
        ridge_line = abs(fbm(x, y, 613, base_freq=2, octaves=2))
        slope_light = -(gx + gy) * 1.9 + ((-0.4 if "N" in sides else 0.0) + (-0.4 if "W" in sides else 0.0))
        if sd > -0.8:
            return _mix(c, (88, 74, 58), 0.65)
        if ridge > 0.66 and ridge_line < 0.18:
            # 尾根筋の明るいハイライト（控えめな岩の稜線）
            return _shade((196, 186, 174), slope_light * 18)
        col = _shade((128, 110, 90), slope_light * 34 + detail * 24)
        if ridge < 0.34:
            col = _mix(col, (76, 62, 48), 0.38)
        return col

    return _draw_canvas(pixel)


def _paint_road(sides):
    px_im = _draw_canvas(lambda x, y: plain_color(x, y))
    px = px_im.load()
    lo, hi = 7, 24  # 道幅 18px
    cx1, cx2 = 15, 16
    for y in range(SIZE):
        for x in range(SIZE):
            in_center = (lo <= x <= hi) and (lo <= y <= hi)
            in_arm = (
                ("N" in sides and lo <= x <= hi and y <= hi)
                or ("S" in sides and lo <= x <= hi and y >= lo)
                or ("W" in sides and lo <= y <= hi and x <= hi)
                or ("E" in sides and lo <= y <= hi and x >= lo)
            )
            if not (in_center or in_arm):
                continue
            # 路肩縁の判定（隣接 4 近傍が道外なら縁石）
            on_border = False
            for nx, ny in ((x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1)):
                c_ok = (lo <= nx <= hi) and (lo <= ny <= hi)
                a_ok = (
                    ("N" in sides and lo <= nx <= hi and ny <= hi)
                    or ("S" in sides and lo <= nx <= hi and ny >= lo)
                    or ("W" in sides and lo <= ny <= hi and nx <= hi)
                    or ("E" in sides and lo <= ny <= hi and nx >= lo)
                )
                if not (c_ok or a_ok):
                    on_border = True
                    break
            if on_border:
                px[x, y] = (86, 90, 96)
                continue
            noise_v = fbm(x, y, 701, base_freq=8, octaves=2)
            col = _shade((138, 140, 146), noise_v * 10)
            # 白い側線（路面縁から 2px 内側）
            # 中央線（オレンジ・白破線）
            on_h_line = ("W" in sides and x <= 14 and y in (cx1, cx2)) or ("E" in sides and x >= 17 and y in (cx1, cx2))
            on_v_line = ("N" in sides and y <= 14 and x in (cx1, cx2)) or ("S" in sides and y >= 17 and x in (cx1, cx2))
            is_straight_h = set(sides) == {"E", "W"}
            is_straight_v = set(sides) == {"N", "S"}
            if is_straight_h and y in (cx1, cx2) and (x % 8 < 5):
                col = (232, 188, 76)
            elif is_straight_v and x in (cx1, cx2) and (y % 8 < 5):
                col = (232, 188, 76)
            elif (on_h_line and (x % 6 < 4)) or (on_v_line and (y % 6 < 4)):
                col = (230, 232, 224)
            px[x, y] = col
    return px_im


def _paint_bridge(no):
    horizontal = no in (1, 501)
    road_style = no in (0, 500, 501)
    im = _draw_canvas(lambda x, y: water_color(x, y, river=True))
    px = im.load()
    lo, hi = 5, 26
    for y in range(SIZE):
        for x in range(SIZE):
            u, v = (y, x) if horizontal else (x, y)  # u=横断, v=進行方向
            if u < lo or u > hi:
                # 橋の落ちる影（右下側）
                if u in (hi + 1, hi + 2):
                    px[x, y] = _shade(px[x, y], -16)
                continue
            if road_style:
                if u in (lo, hi):
                    px[x, y] = (72, 76, 84)
                elif u in (lo + 1, hi - 1):
                    px[x, y] = (188, 192, 200) if u == lo + 1 else (120, 124, 132)
                elif u in (15, 16) and v % 8 < 5:
                    px[x, y] = (232, 188, 76)
                else:
                    px[x, y] = _shade((144, 146, 152), fbm(x, y, 719, base_freq=8, octaves=2) * 8)
            else:
                # 木の橋（石の縁＋木の横板）
                if u in (lo, hi):
                    px[x, y] = (68, 50, 32)
                elif u in (lo + 1, hi - 1):
                    px[x, y] = (176, 156, 120) if u == lo + 1 else (112, 88, 56)
                else:
                    plank = (164, 118, 68) if (v // 4) % 2 == 0 else (152, 108, 60)
                    if v % 4 == 0:
                        plank = (108, 74, 40)
                    px[x, y] = _shade(plank, fbm(x, y, 733, base_freq=8, octaves=2) * 8)
    return im


def tile(name, no=0):
    rnd = random.Random(f"{name}:{no}")
    if name == "plain":
        if 10 <= no <= 13:
            c_dir = {10: ("NE",), 11: ("SE",), 12: ("SW",), 13: ("NW",)}[no]
            return _draw_canvas(lambda x, y: (
                water_color(x, y) if _side_dist(x, y, c_dir, corner_r=10) > 1.2
                else ((188, 182, 140) if _side_dist(x, y, c_dir, corner_r=10) > 0.0 else plain_color(x, y))
            ))
        return _draw_canvas(lambda x, y: plain_color(x, y, no))

    if name == "bush":
        im = _draw_canvas(lambda x, y: plain_color(x, y))
        d = ImageDraw.Draw(im)
        for cx, cy in [(8, 10), (22, 8), (14, 22), (25, 24)]:
            d.ellipse([cx - 4, cy - 3, cx + 6, cy + 5], fill=(38, 86, 36))
            d.ellipse([cx - 5, cy - 4, cx + 5, cy + 4], fill=(56, 126, 48), outline=(32, 76, 30))
            d.ellipse([cx - 3, cy - 3, cx + 1, cy], fill=(104, 174, 76))
        return im

    if name == "woods":
        sides = WOODS_SIDES.get(no, ())
        im = Image.new("RGB", (SIZE, SIZE))
        _paint_trees(im, sides, no)
        return im

    if name == "mountain":
        sides = MOUNTAIN_SIDES.get(no, ("N", "W"))
        return _paint_mountain(sides, standalone=(no == 49))

    if name == "waste_land":
        sides = WASTE_SIDES.get(no, ())
        def wpx(x, y):
            n = fbm(x, y, 811, base_freq=4, octaves=3)
            grad = fbm((x - 1) % SIZE, (y - 1) % SIZE, 811, base_freq=4, octaves=3) - n
            wc = _shade((176, 144, 92), n * 22 - grad * 20)
            if _hash2(x, y, 823) > 0.91:
                wc = _shade(wc, -24)
            if sides:
                sd = _side_dist(x, y, sides, width=6, corner_r=9)
                if sd > 0.8:
                    return plain_color(x, y)
                if sd > -0.7:
                    return _mix(wc, plain_color(x, y), 0.45)
            return wc
        return _draw_canvas(wpx)

    if name == "rock":
        im = _draw_canvas(lambda x, y: _shade((168, 144, 98), fbm(x, y, 829, base_freq=4, octaves=3) * 18))
        d = ImageDraw.Draw(im)
        for box in [[3, 6, 15, 16], [16, 14, 29, 26], [5, 20, 14, 28]]:
            d.ellipse([box[0] + 1, box[1] + 2, box[2] + 1, box[3] + 2], fill=(96, 82, 56))
            d.ellipse(box, fill=(132, 132, 136), outline=(64, 64, 68))
            d.arc(box, 195, 275, fill=(196, 198, 204), width=2)
        return im

    if name == "hill":
        def hpx(x, y):
            c = plain_color(x, y)
            dx = (x - 16.0) / 14.0
            dy = (y - 18.0) / 12.0
            d2 = dx * dx + dy * dy
            if d2 < 1.0:
                elev = (1.0 - d2)
                light = (-dx - dy) * 20 * elev
                hc = _shade((122, 182, 82), light + fbm(x, y, 839, base_freq=6, octaves=2) * 10)
                if d2 > 0.78:
                    return _mix(c, (72, 120, 48), 0.7)
                return _mix(c, hc, min(1.0, elev * 1.8))
            return c
        return _draw_canvas(hpx)

    if name == "sea":
        if no == 6:
            return _draw_canvas(lambda x, y: water_color(x, y, deep=True))
        if no == 5:
            im = _draw_canvas(lambda x, y: water_color(x, y))
            d = ImageDraw.Draw(im)
            d.line([16, 7, 16, 25], fill=(168, 212, 212))
            d.line([16, 7, 10, 23], fill=(208, 232, 232))
            d.line([16, 7, 22, 23], fill=(208, 232, 232))
            d.line([13, 19, 19, 19], fill=(144, 196, 196))
            return im
        sides = SEA_SIDES.get(no, ())
        thin = 24 <= no <= 27
        w = 4 if thin else 6
        def spx(x, y):
            wc = water_color(x, y)
            if not sides:
                return wc
            sd = _side_dist(x, y, sides, width=w, corner_r=10)
            if sd > 0.5:
                return plain_color(x, y)
            if sd > -0.7:
                return (184, 178, 134)
            if sd > -2.0:
                return _mix(wc, (186, 216, 214), 0.6)
            return wc
        return _draw_canvas(spx)

    if name == "river":
        if no in (145, 146, 147, 150):
            # 川の先端（指定方向のみ水が抜け、他方向は草地に丸くすぼまる）
            open_dir = {145: "E", 146: "W", 147: "S", 150: "N"}[no]
            closed = tuple(s for s in ("N", "E", "S", "W") if s != open_dir)
            def endpx(x, y):
                sd = _side_dist(x, y, closed, width=7, corner_r=11)
                if sd > 0.6:
                    return plain_color(x, y)
                if sd > -0.8:
                    return (94, 134, 68)
                return water_color(x, y, river=True)
            return _draw_canvas(endpx)
        sides = RIVER_SIDES.get(no, ())
        def rpx(x, y):
            wc = water_color(x, y, river=True)
            if not sides:
                return wc
            sd = _side_dist(x, y, sides, width=5, corner_r=4.5 if no == 4 else 8)
            if sd > 0.5:
                return plain_color(x, y)
            if sd > -0.7:
                return (96, 136, 70)
            if sd > -1.7:
                return _mix(wc, (164, 206, 204), 0.45)
            return wc
        return _draw_canvas(rpx)

    if name == "road":
        return _paint_road(ROAD_SIDES.get(no, ("E", "W")))

    if name == "bridge":
        return _paint_bridge(no)

    if name == "town":
        im = _draw_canvas(lambda x, y: plain_color(x, y))
        d = ImageDraw.Draw(im)
        if no == 0:
            houses = [(3, 5, 9, 8, (188, 66, 52)), (18, 16, 9, 8, (64, 102, 168)), (5, 19, 8, 7, (176, 122, 54))]
        elif no == 1:
            houses = [(3, 4, 12, 10, (168, 76, 58)), (17, 13, 11, 10, (72, 112, 164))]
        else:
            houses = [(4, 6, 8, 7, (176, 118, 52)), (19, 17, 8, 7, (188, 66, 52))]
        for x, y, w, h, roof in houses:
            d.rectangle([x + 1, y + 5, x + w + 1, y + h + 4], fill=(56, 88, 40))
            d.rectangle([x, y + 4, x + w, y + h + 3], fill=(228, 218, 196), outline=(88, 74, 60))
            d.polygon([(x - 1, y + 4), (x + w // 2, y), (x + w + 1, y + 4)], fill=roof, outline=(72, 40, 32))
            d.rectangle([x + w // 2 - 1, y + h, x + w // 2 + 1, y + h + 3], fill=(92, 62, 42))
        return im

    if name == "building":
        im = _draw_canvas(lambda x, y: _shade((136, 140, 146), fbm(x, y, 853, base_freq=8, octaves=2) * 8))
        d = ImageDraw.Draw(im)
        for x, top, w in [(3, 4, 12), (17, 9, 12)]:
            d.rectangle([x + 2, top + 2, x + w + 2, 30], fill=(82, 86, 94))
            d.rectangle([x, top, x + w, 29], fill=(176, 188, 202), outline=(68, 76, 92))
            d.rectangle([x + 1, top + 1, x + w - 1, top + 2], fill=(208, 218, 230))
            for wy in range(top + 4, 27, 4):
                for wx in range(x + 2, x + w - 1, 4):
                    d.rectangle([wx, wy, wx + 2, wy + 2], fill=(76, 120, 176))
        return im

    if name == "base":
        im = _draw_canvas(lambda x, y: _shade((136, 140, 144), fbm(x, y, 859, base_freq=8, octaves=2) * 8))
        d = ImageDraw.Draw(im)
        d.rectangle([4, 13, 29, 30], fill=(76, 80, 76))
        d.rectangle([3, 12, 28, 29], fill=(116, 128, 108), outline=(52, 60, 48))
        d.chord([3, 4, 28, 20], 180, 360, fill=(136, 148, 128), outline=(52, 60, 48))
        d.rectangle([12, 20, 19, 29], fill=(56, 60, 64))
        d.line([16, 2, 16, 8], fill=(52, 60, 48))
        d.rectangle([17, 2, 22, 5], fill=(204, 52, 48))
        return im

    if name == "castle":
        im = _draw_canvas(lambda x, y: plain_color(x, y))
        d = ImageDraw.Draw(im)
        d.rectangle([5, 11, 29, 30], fill=(52, 80, 40))
        d.rectangle([4, 10, 27, 29], fill=(176, 172, 162), outline=(82, 78, 70))
        for x in range(4, 28, 6):
            d.rectangle([x, 6, x + 3, 10], fill=(186, 182, 172), outline=(82, 78, 70))
        d.rectangle([13, 20, 18, 29], fill=(72, 52, 36))
        d.rectangle([8, 14, 10, 16], fill=(40, 40, 44))
        d.rectangle([21, 14, 23, 16], fill=(40, 40, 44))
        return im

    if name == "powerplant":
        im = _draw_canvas(lambda x, y: _shade((138, 142, 146), fbm(x, y, 863, base_freq=8, octaves=2) * 8))
        d = ImageDraw.Draw(im)
        d.rectangle([3, 14, 20, 29], fill=(188, 180, 160), outline=(78, 74, 62))
        d.polygon([(22, 29), (24, 6), (29, 6), (31, 29)], fill=(202, 198, 190), outline=(78, 74, 62))
        d.polygon([(8, 10), (12, 2), (11, 8), (15, 8), (10, 16), (11, 10)], fill=(244, 212, 52), outline=(144, 104, 16))
        return im

    if name == "remains":
        im = _draw_canvas(lambda x, y: _shade((144, 134, 116), fbm(x, y, 877, base_freq=6, octaves=3) * 18))
        d = ImageDraw.Draw(im)
        d.polygon([(4, 28), (4, 10), (9, 6), (13, 14), (13, 28)], fill=(128, 126, 122), outline=(64, 62, 60))
        d.polygon([(18, 28), (18, 16), (24, 12), (28, 20), (28, 28)], fill=(112, 110, 106), outline=(64, 62, 60))
        return im

    if name == "ruins":
        im = _draw_canvas(lambda x, y: _shade((172, 154, 110), fbm(x, y, 881, base_freq=6, octaves=3) * 16))
        d = ImageDraw.Draw(im)
        for x in (5, 14, 23):
            d.rectangle([x, 9, x + 4, 28], fill=(214, 202, 170), outline=(116, 104, 80))
        d.rectangle([3, 5, 20, 9], fill=(220, 208, 178), outline=(116, 104, 80))
        return im

    if name == "lava":
        sides = LAVA_SIDES.get(no, ())
        def lpx(x, y):
            n = fbm(x, y, 907, base_freq=4, octaves=3)
            heat = max(0.0, min(1.0, (n + 0.65) / 1.3))
            lc = _mix((168, 36, 16), (248, 164, 36), heat)
            if heat > 0.78:
                lc = _mix(lc, (252, 220, 84), (heat - 0.78) * 3.0)
            if sides:
                sd = _side_dist(x, y, sides, width=5, corner_r=9)
                if sd > 0.6:
                    return _shade((54, 48, 52), n * 14)
                if sd > -0.8:
                    return _mix((86, 44, 32), lc, 0.4)
            return lc
        return _draw_canvas(lpx)

    if name in SPACE_TILES:
        return space_tile(name, no)

    raise ValueError(name)


SPACE_TILES = {"space", "asteroids", "wrecks", "colony", "space_fortress", "space_dock", "earth", "moon",
               "moon_ground", "small_crater", "moon_city", "moon_base"}


def _draw_crater(d, cx, cy, r, bg_light=172):
    d.ellipse([cx - r, cy - r, cx + r, cy + r], fill=(116, 118, 126), outline=(88, 90, 98))
    d.arc([cx - r, cy - r, cx + r, cy + r], 195, 345, fill=(bg_light, bg_light + 2, bg_light + 8), width=1)
    d.ellipse([cx - r // 2, cy - r // 2, cx + r // 2, cy + r // 2], fill=(100, 102, 110))


def space_tile(name, no=0):
    """宇宙（地形タイプ 宇宙）と月面（月面）の地形の画像"""
    if name in ("moon_ground", "small_crater", "moon_city", "moon_base"):
        im = _draw_canvas(moon_color)
        d = ImageDraw.Draw(im)
        if name == "moon_ground":
            if no == 1:
                _draw_crater(d, 16, 16, 7, 186)
            elif no == 2:
                _draw_crater(d, 9, 10, 4, 184)
                _draw_crater(d, 22, 21, 5, 184)
        elif name == "small_crater":
            if no == 0:
                _draw_crater(d, 16, 16, 10, 196)
            elif no == 6:
                _draw_crater(d, 16, 16, 6, 194)
            elif no == 7:
                _draw_crater(d, 10, 11, 5, 194)
                _draw_crater(d, 22, 21, 5, 194)
        elif name == "moon_city":
            for box in [[2, 14, 16, 30], [14, 6, 30, 22], [18, 20, 30, 32]]:
                d.chord(box, 180, 360, fill=(176, 208, 232), outline=(80, 100, 120))
                d.line([box[0], (box[1] + box[3]) // 2, box[2], (box[1] + box[3]) // 2], fill=(80, 100, 120))
            for x, y in [(6, 19), (10, 20), (19, 11), (24, 12), (22, 24)]:
                d.rectangle([x, y, x + 1, y + 1], fill=(248, 232, 120))
        elif name == "moon_base":
            d.rectangle([3, 13, 28, 29], fill=(120, 128, 136), outline=(60, 64, 72))
            d.chord([3, 5, 28, 21], 180, 360, fill=(136, 144, 152), outline=(60, 64, 72))
            d.rectangle([12, 20, 19, 29], fill=(56, 60, 68))
            d.line([24, 2, 24, 8], fill=(60, 64, 72))
            d.ellipse([21, 0, 27, 4], outline=(200, 204, 212))
        return im

    im = _draw_canvas(lambda x, y: space_color(x, y, no if name == "space" else 0))
    d = ImageDraw.Draw(im)
    if name == "space":
        if no >= 7:
            # 明るい星（十字の光）が多い変種
            rnd = random.Random(f"space-bright-{no}")
            for _ in range(3):
                sx, sy = rnd.randrange(3, 28), rnd.randrange(3, 28)
                c = rnd.choice([(240, 248, 255), (255, 244, 208), (200, 224, 255)])
                d.point([sx, sy], fill=(255, 255, 255))
                d.line([sx - 2, sy, sx + 2, sy], fill=c)
                d.line([sx, sy - 2, sx, sy + 2], fill=c)
        return im

    if name == "asteroids":
        rock_sets = {
            0: [[5, 6, 13, 13], [19, 18, 27, 25]],
            1: [[3, 5, 13, 14], [18, 4, 27, 12], [11, 18, 22, 27]],
            2: [[2, 4, 13, 13], [17, 2, 28, 11], [8, 17, 22, 29], [24, 18, 31, 25]],
            3: [[1, 3, 13, 13], [15, 2, 29, 13], [3, 15, 16, 28], [17, 16, 30, 29], [11, 10, 20, 18]],
        }
        for box in rock_sets.get(no, rock_sets[2]):
            d.ellipse(box, fill=(116, 98, 84), outline=(58, 48, 42))
            d.arc(box, 190, 275, fill=(176, 156, 136), width=2)
    elif name == "wrecks":
        d.polygon([(3, 8), (14, 4), (17, 10), (6, 14)], fill=(128, 132, 140), outline=(70, 72, 80))
        d.polygon([(18, 18), (29, 15), (30, 24), (20, 27)], fill=(112, 116, 124), outline=(70, 72, 80))
        d.line([8, 22, 14, 28], fill=(150, 154, 162), width=2)
        d.line([22, 5, 27, 9], fill=(150, 154, 162))
        d.point([(12, 7), (25, 20)], fill=(248, 160, 64))
    elif name == "colony":
        d.rectangle([4, 9, 27, 22], fill=(180, 184, 192), outline=(90, 94, 104))
        d.ellipse([0, 9, 8, 22], fill=(160, 164, 172), outline=(90, 94, 104))
        d.ellipse([24, 9, 31, 22], fill=(200, 204, 212), outline=(90, 94, 104))
        for x in range(7, 24, 4):
            d.line([x, 10, x, 21], fill=(70, 110, 170))
        d.polygon([(6, 9), (16, 2), (26, 9)], fill=(90, 120, 170), outline=(60, 80, 120))
        d.polygon([(6, 22), (16, 29), (26, 22)], fill=(90, 120, 170), outline=(60, 80, 120))
    elif name == "space_fortress":
        d.polygon([(4, 28), (2, 16), (10, 4), (22, 4), (30, 16), (28, 28)], fill=(88, 84, 92), outline=(48, 44, 52))
        d.polygon([(10, 20), (16, 10), (22, 20)], fill=(112, 106, 118), outline=(48, 44, 52))
        for x, y in [(7, 14), (24, 13), (12, 24), (20, 25), (16, 7)]:
            d.rectangle([x, y, x + 1, y + 1], fill=(248, 96, 64))
    elif name == "space_dock":
        d.rectangle([3, 3, 28, 6], fill=(150, 156, 166), outline=(80, 86, 96))
        d.rectangle([3, 25, 28, 28], fill=(150, 156, 166), outline=(80, 86, 96))
        for x in (4, 15, 26):
            d.rectangle([x, 6, x + 2, 25], fill=(130, 136, 146), outline=(80, 86, 96))
        for y in (10, 15, 20):
            d.line([7, y, 25, y], fill=(110, 116, 126))
        d.point([(9, 4), (22, 27)], fill=(120, 248, 120))
    elif name == "earth":
        d.ellipse([-14, 2, 30, 46], fill=(40, 96, 200), outline=(140, 190, 255))
        d.polygon([(2, 14), (10, 10), (14, 18), (8, 24), (4, 22)], fill=(72, 148, 72))
        d.polygon([(16, 22), (24, 20), (26, 30), (18, 31)], fill=(84, 156, 76))
        d.arc([-6, 8, 18, 30], 180, 260, fill=(236, 240, 248))
    elif name == "moon":
        d.ellipse([4, 4, 28, 28], fill=(200, 200, 196), outline=(140, 140, 136))
        for box in [[9, 10, 15, 16], [18, 15, 25, 22], [12, 20, 16, 24]]:
            d.ellipse(box, fill=(168, 168, 164))
    return im


def unit(name):
    """白の背景（透過色）に、種類の分かる簡単な形を描く"""
    im = Image.new("RGB", (SIZE, SIZE), (255, 255, 255))
    d = ImageDraw.Draw(im)
    ink = (24, 24, 40)
    main, light = (72, 104, 168), (152, 184, 232)
    kind = name[4:-2]
    if kind == "Robot":
        variant = name[-1]
        main = {"1": (64, 104, 184), "2": (176, 64, 56), "3": (80, 136, 88)}[variant]
        d.rectangle([12, 4, 19, 10], fill=main, outline=ink)
        d.rectangle([14, 6, 17, 7], fill=(248, 216, 64))
        d.rectangle([9, 11, 22, 20], fill=main, outline=ink)
        d.rectangle([5, 11, 8, 19], fill=light, outline=ink)
        d.rectangle([23, 11, 26, 19], fill=light, outline=ink)
        d.rectangle([10, 21, 14, 29], fill=light, outline=ink)
        d.rectangle([17, 21, 21, 29], fill=light, outline=ink)
        if variant == "2":
            d.polygon([(12, 4), (10, 0), (14, 4)], fill=main, outline=ink)
            d.polygon([(19, 4), (21, 0), (17, 4)], fill=main, outline=ink)
        if variant == "3":
            d.rectangle([24, 6, 29, 10], fill=(96, 96, 96), outline=ink)
    elif kind == "Human":
        d.ellipse([12, 3, 19, 10], fill=(240, 200, 160), outline=ink)
        d.rectangle([11, 11, 20, 21], fill=main, outline=ink)
        d.line([11, 12, 7, 19], fill=ink, width=2)
        d.line([20, 12, 24, 19], fill=ink, width=2)
        d.line([13, 22, 12, 29], fill=ink, width=3)
        d.line([18, 22, 19, 29], fill=ink, width=3)
    elif kind == "WingHuman":
        d.polygon([(10, 12), (1, 4), (3, 18)], fill=(240, 240, 248), outline=ink)
        d.polygon([(21, 12), (30, 4), (28, 18)], fill=(240, 240, 248), outline=ink)
        d.ellipse([12, 3, 19, 10], fill=(240, 200, 160), outline=ink)
        d.rectangle([11, 11, 20, 21], fill=(200, 168, 72), outline=ink)
        d.line([13, 22, 12, 29], fill=ink, width=3)
        d.line([18, 22, 19, 29], fill=ink, width=3)
    elif kind in ("Airplane", "SpaceShip", "UFO"):
        if kind == "UFO":
            d.ellipse([3, 12, 28, 22], fill=(168, 168, 184), outline=ink)
            d.ellipse([10, 7, 21, 16], fill=light, outline=ink)
        else:
            d.polygon([(16, 2), (19, 12), (30, 20), (19, 19), (18, 27), (22, 30), (10, 30), (14, 27), (13, 19), (2, 20), (13, 12)],
                      fill=main if kind == "Airplane" else (168, 176, 192), outline=ink)
            d.rectangle([15, 7, 16, 11], fill=light)
    elif kind in ("SailShip", "SubMarine"):
        if kind == "SailShip":
            d.polygon([(3, 21), (29, 21), (25, 28), (7, 28)], fill=(152, 96, 48), outline=ink)
            d.line([16, 3, 16, 21], fill=ink, width=2)
            d.polygon([(17, 4), (27, 18), (17, 18)], fill=(248, 236, 200), outline=ink)
            d.polygon([(15, 6), (7, 18), (15, 18)], fill=(248, 236, 200), outline=ink)
            d.ellipse([14, 1, 18, 5], fill=(252, 200, 64), outline=ink)
        else:
            d.ellipse([2, 14, 29, 24], fill=(72, 88, 112), outline=ink)
            d.rectangle([13, 9, 19, 15], fill=(72, 88, 112), outline=ink)
    elif kind == "Tank":
        d.rectangle([3, 18, 28, 27], fill=(88, 96, 72), outline=ink)
        for x in range(5, 27, 5):
            d.ellipse([x, 22, x + 3, 25], fill=(48, 48, 48))
        d.rectangle([9, 11, 22, 18], fill=(112, 124, 88), outline=ink)
        d.rectangle([22, 13, 30, 15], fill=(88, 96, 72), outline=ink)
    elif kind in ("Beast", "Dinosaur", "Horse", "HorseRider", "Fish", "Bird"):
        color = {"Beast": (152, 104, 64), "Dinosaur": (88, 144, 72), "Horse": (128, 88, 56),
                 "HorseRider": (128, 88, 56), "Fish": (72, 128, 192), "Bird": (200, 168, 72)}[kind]
        if kind == "Fish":
            d.ellipse([4, 10, 24, 22], fill=color, outline=ink)
            d.polygon([(23, 16), (30, 9), (30, 23)], fill=color, outline=ink)
            d.point([9, 14], fill=ink)
        elif kind == "Bird":
            d.ellipse([10, 12, 22, 22], fill=color, outline=ink)
            d.polygon([(14, 14), (2, 6), (8, 18)], fill=color, outline=ink)
            d.polygon([(18, 14), (30, 6), (24, 18)], fill=color, outline=ink)
            d.polygon([(10, 15), (6, 17), (10, 18)], fill=(232, 152, 32))
        else:
            d.ellipse([6, 12, 26, 23], fill=color, outline=ink)
            d.ellipse([20, 6, 30, 14] if kind != "Dinosaur" else [20, 2, 30, 11], fill=color, outline=ink)
            for x in (8, 12, 19, 23):
                d.line([x, 22, x, 29], fill=ink, width=2)
            if kind == "Dinosaur":
                d.polygon([(7, 16), (1, 24), (9, 20)], fill=color, outline=ink)
            if kind == "HorseRider":
                d.ellipse([12, 1, 18, 7], fill=(240, 200, 160), outline=ink)
                d.rectangle([12, 7, 18, 13], fill=main, outline=ink)
    elif kind == "BikeRider":
        d.ellipse([3, 20, 11, 28], outline=ink, width=2)
        d.ellipse([21, 20, 29, 28], outline=ink, width=2)
        d.polygon([(7, 24), (14, 16), (24, 16), (25, 24)], fill=(176, 48, 48), outline=ink)
        d.ellipse([13, 4, 19, 10], fill=(240, 200, 160), outline=ink)
        d.line([16, 10, 16, 16], fill=ink, width=3)
    else:
        # Etc: 種類の決まっていないもの（物資・装置など）
        color = {"1": (200, 152, 64), "2": (120, 120, 200), "3": (96, 168, 120), "4": (176, 96, 176)}[name[-1]]
        d.rectangle([7, 9, 24, 26], fill=color, outline=ink)
        d.line([7, 9, 12, 4], fill=ink)
        d.line([24, 9, 29, 4], fill=ink)
        d.line([12, 4, 29, 4], fill=ink)
        d.line([29, 4, 29, 21], fill=ink)
        d.line([24, 26, 29, 21], fill=ink)
        d.line([7, 17, 24, 17], fill=ink)
    return im


def terrain_txt():
    lines = ["# SRC# の最小の汎用データ（basic-data）の地形データ。番号と名前は汎用データと同じ", ""]
    for number, name, bitmap, kind, cost, hit, damage, effect in TERRAINS:
        # 特殊効果は 1 行に 1 つ
        lines += [str(number), f"{name}, {bitmap}", f"{kind}, {cost}, {hit}, {damage}"] + effect.split() + [""]
    return "\n".join(lines)


def sp_txt():
    lines = ["# SRC# の最小の汎用データ（basic-data）のスペシャルパワーデータ", ""]
    for name, reading, short, cost, target, duration, condition, effect, note in SPECIAL_POWERS:
        lines += [f"{name}, {reading}" if reading else name,
                  f"{short}, {cost}, {target}, {duration}, {condition}, -, {name}",
                  effect, note, ""]
    return "\n".join(lines)


# ── 効果音 ──
# 汎用効果音集（Sound/*.wav）と同じ名前の効果音を、数式で合成する。録音や生成 AI の音声は使わない。
# 22050 Hz・16 ビット・モノラルの WAV。音の部品（ノイズ・波形・周波数の変化・音量の変化）を組み合わせて作る。
RATE = 22050


def noise(rnd, dur, cutoff=1.0, highpass=0.0):
    """ホワイトノイズ。cutoff（0〜1）で高い音を削り（1 次のローパス）、highpass（0〜1）で低い音を削る"""
    out, low, prev = [], 0.0, 0.0
    for _ in range(int(RATE * dur)):
        x = rnd.uniform(-1, 1)
        low += cutoff * (x - low)
        y = low - highpass * prev
        prev = low
        out.append(y)
    return out


def osc(freq, dur, shape="sine", vibrato=0.0, vib_rate=0.0):
    """周波数が変わる波形。freq は時間（秒）から周波数（Hz）を返す関数か定数"""
    out, phase = [], 0.0
    f = freq if callable(freq) else (lambda t: freq)
    for i in range(int(RATE * dur)):
        t = i / RATE
        hz = f(t) * (1 + vibrato * math.sin(2 * math.pi * vib_rate * t))
        phase += hz / RATE
        p = phase % 1.0
        if shape == "square":
            out.append(1.0 if p < 0.5 else -1.0)
        elif shape == "saw":
            out.append(2 * p - 1)
        elif shape == "triangle":
            out.append(4 * abs(p - 0.5) - 1)
        else:
            out.append(math.sin(2 * math.pi * p))
    return out


def env(samples, attack=0.005, decay=None, curve=2.0):
    """音量の変化。立ち上がり attack 秒、その後は終わりに向けて curve 乗で減衰する"""
    n = len(samples)
    a = max(1, int(RATE * attack))
    out = []
    for i, x in enumerate(samples):
        if i < a:
            g = i / a
        else:
            g = (1 - (i - a) / max(1, n - a)) ** curve
        out.append(x * g)
    return out


def mix(*tracks, at=None):
    """重ねる。at は各トラックの開始時刻（秒）"""
    at = at or [0.0] * len(tracks)
    n = max(int(RATE * t) + len(x) for t, x in zip(at, tracks))
    out = [0.0] * n
    for t, x in zip(at, tracks):
        o = int(RATE * t)
        for i, v in enumerate(x):
            out[o + i] += v
    return out


def gain(samples, g):
    return [x * g for x in samples]


def repeat(make, count, interval):
    """同じ音を一定の間隔で繰り返す（連射など）。make は回数から音を返す"""
    return mix(*[make(i) for i in range(count)], at=[i * interval for i in range(count)])


def save_wav(samples, path, volume=0.8):
    full = os.path.join(HERE, path)
    os.makedirs(os.path.dirname(full), exist_ok=True)
    peak = max([abs(x) for x in samples] + [1e-9])
    with wave.open(full, "wb") as w:
        w.setnchannels(1)
        w.setsampwidth(2)
        w.setframerate(RATE)
        w.writeframes(b"".join(struct.pack("<h", int(x / peak * volume * 32767)) for x in samples))


def shot(rnd, dur=0.15, cutoff=0.6, pitch=900):
    """銃声: 短いノイズと、下がる音の打撃"""
    return mix(env(noise(rnd, dur, cutoff), 0.001, curve=3), gain(env(osc(lambda t: pitch * (1 - t * 4), dur * 0.6), 0.001, curve=3), 0.6))


def boom(rnd, dur=0.8, cutoff=0.08):
    """爆発: 低い音のノイズと、下がる低音"""
    return mix(env(noise(rnd, dur, cutoff), 0.004, curve=2.2), gain(env(osc(lambda t: 90 * (1 - t / dur * 0.6), dur), 0.002, curve=2.5), 0.8))


def whoosh(rnd, dur=0.35, up=True):
    """風切り: 高い音を残したノイズを、ふくらませて消す"""
    x = noise(rnd, dur, 0.5, 0.9)
    n = len(x)
    return [v * math.sin(math.pi * (i / n if up else 1 - i / n) ** 0.7) for i, v in enumerate(x)]


def chime(freqs, dur=0.6, step=0.06):
    """鈴のような音を順に鳴らす"""
    return mix(*[env(osc(f, dur), 0.002, curve=3) for f in freqs], at=[i * step for i in range(len(freqs))])


def sounds():
    """名前と合成のしかた。名前は汎用効果音集と同じ（SRCCore が武器の種類などから選ぶ名前と、ランタン号が使う名前）"""
    R = random.Random
    return {
        # 銃・砲
        "Gun": lambda r: shot(r),
        "Rifle": lambda r: shot(r, 0.22, 0.5, 700),
        "GunPod": lambda r: repeat(lambda i: shot(r, 0.12, 0.5, 800), 3, 0.09),
        "FastGun": lambda r: repeat(lambda i: shot(r, 0.08, 0.7, 1200), 4, 0.06),
        "MachineGun": lambda r: repeat(lambda i: shot(r, 0.08, 0.6, 1000), 8, 0.07),
        "MachineCannon": lambda r: repeat(lambda i: shot(r, 0.14, 0.3, 500), 5, 0.11),
        "LaserGun": lambda r: env(osc(lambda t: 2400 * (1 - t * 3), 0.25, "square"), 0.002, curve=2),
        "Cannon": lambda r: mix(shot(r, 0.1, 0.4, 600), boom(r, 0.6, 0.1)),
        "Bazooka": lambda r: mix(whoosh(r, 0.3), boom(r, 0.6, 0.1), at=[0, 0.25]),
        "Missile": lambda r: mix(whoosh(r, 0.5), boom(r, 0.7, 0.1), at=[0, 0.45]),
        "MicroMissile": lambda r: repeat(lambda i: mix(whoosh(r, 0.2), boom(r, 0.3, 0.15), at=[0, 0.18]), 4, 0.12),
        "AntiShipMissile": lambda r: mix(whoosh(r, 0.9), boom(r, 1.4, 0.06), at=[0, 0.8]),
        "MultipleRocketLauncher(Light)": lambda r: repeat(lambda i: whoosh(r, 0.25), 6, 0.08),
        "Bomb": lambda r: mix(gain(env(osc(lambda t: 1800 - 1400 * t / 0.6, 0.6), 0.01, curve=0.5), 0.4), boom(r, 0.9, 0.07), at=[0, 0.55]),
        "Bow": lambda r: mix(env(osc(lambda t: 220 * (1 + 0.3 * math.exp(-t * 30)), 0.3, "triangle"), 0.001, curve=3), whoosh(r, 0.2), at=[0, 0.05]),
        # 爆発・破壊
        "Explode": lambda r: boom(r, 0.9),
        "Explode(Small)": lambda r: boom(r, 0.4, 0.15),
        "Explode(Far)": lambda r: gain(boom(r, 1.0, 0.03), 0.6),
        "Explode(Long)": lambda r: mix(boom(r, 1.0), boom(r, 1.0), boom(r, 1.4), at=[0, 0.4, 0.8]),
        "Explode(Nuclear)": lambda r: mix(boom(r, 3.0, 0.03), gain(env(noise(r, 2.5, 0.2), 0.3, curve=1.5), 0.5)),
        "Crash": lambda r: mix(boom(r, 0.5, 0.2), env(noise(r, 0.5, 0.8, 0.5), 0.001, curve=4)),
        "Break": lambda r: mix(env(noise(r, 0.4, 0.7), 0.001, curve=3), *[env(osc(f, 0.3, "square"), 0.001, curve=4) for f in (330, 470)]),
        "BreakOff": lambda r: mix(env(noise(r, 0.3, 0.6, 0.3), 0.001, curve=3), env(osc(lambda t: 600 - 400 * t, 0.3, "saw"), 0.001, curve=3)),
        "Glass": lambda r: mix(env(noise(r, 0.3, 1.0, 0.95), 0.001, curve=3), chime([2637, 3136, 3520, 4186], 0.5, 0.03)),
        "Shock(Low)": lambda r: env(osc(lambda t: 60 + 40 * math.exp(-t * 8), 0.6, "square"), 0.002, curve=2),
        # ビーム・エネルギー
        "Beam": lambda r: env(osc(lambda t: 1800 - 1200 * t / 0.6, 0.6, "square", 0.05, 30), 0.005, curve=1.5),
        "BeamSaber": lambda r: env(mix(osc(lambda t: 110 + 300 * t, 0.5, "saw", 0.03, 8), gain(osc(220, 0.5), 0.5)), 0.01, curve=1.2),
        "Saber": lambda r: env(mix(osc(lambda t: 140 + 200 * t, 0.4, "saw", 0.04, 9), gain(whoosh(r, 0.4), 0.6)), 0.01, curve=1.2),
        "BeamCoat": lambda r: env(osc(lambda t: 400 + 1600 * t, 0.6, "sine", 0.1, 20), 0.05, curve=1),
        "Thunder": lambda r: mix(env(noise(r, 0.15, 1.0), 0.001, curve=2), boom(r, 1.2, 0.05), *[env(noise(r, 0.05, 1.0), 0.001, curve=2) for _ in range(3)], at=[0, 0.05, 0.2, 0.35, 0.5]),
        "Fire": lambda r: env(mix(noise(r, 0.8, 0.3), *[env(noise(r, 0.02, 1.0), 0.001) for _ in range(12)], at=[0] + [r.uniform(0, 0.75) for _ in range(12)]), 0.05, curve=1),
        "Breath": lambda r: env(noise(r, 1.0, 0.25), 0.1, curve=1),
        "Storm": lambda r: env([v * (0.6 + 0.4 * math.sin(i / RATE * 9)) for i, v in enumerate(noise(r, 1.4, 0.15))], 0.2, curve=1),
        "Shower": lambda r: env(noise(r, 1.0, 0.9, 0.7), 0.1, curve=1),
        "Splash": lambda r: mix(env(noise(r, 0.5, 0.6, 0.3), 0.003, curve=3), chime([700, 900, 1200], 0.2, 0.05)),
        "Bubble": lambda r: mix(*[env(osc(lambda t, b=b: b * (1 + t * 8), 0.08), 0.002, curve=2) for b in (300, 420, 360, 500, 450)], at=[0, 0.07, 0.15, 0.22, 0.3]),
        "Drill": lambda r: env(osc(lambda t: 90 + 60 * t, 1.0, "saw", 0.3, 40), 0.03, curve=0.8),
        # 格闘・刃物
        "Punch": lambda r: mix(env(osc(lambda t: 140 * (1 - t * 3), 0.15), 0.001, curve=3), env(noise(r, 0.08, 0.3), 0.001, curve=3)),
        "Slap": lambda r: env(noise(r, 0.12, 0.9, 0.6), 0.001, curve=4),
        "Combo": lambda r: repeat(lambda i: mix(env(osc(lambda t: 160 * (1 - t * 3), 0.12), 0.001, curve=3), env(noise(r, 0.06, 0.4), 0.001, curve=3)), 4, 0.1),
        "Swing": lambda r: whoosh(r, 0.3),
        "Whiz": lambda r: whoosh(r, 0.18),
        "Sword": lambda r: mix(whoosh(r, 0.25), chime([1500, 2250], 0.4, 0.0), at=[0, 0.2]),
        "Slash": lambda r: mix(whoosh(r, 0.2), env(noise(r, 0.15, 1.0, 0.8), 0.001, curve=3), at=[0, 0.15]),
        "Stab": lambda r: mix(whoosh(r, 0.12), env(osc(lambda t: 300 * (1 - t * 4), 0.12), 0.001, curve=3), at=[0, 0.1]),
        "Whip": lambda r: mix(whoosh(r, 0.2), env(noise(r, 0.04, 1.0, 0.9), 0.001, curve=2), at=[0, 0.18]),
        # その他
        "Charge": lambda r: env(osc(lambda t: 200 + 1200 * t, 0.8, "square", 0.02, 25), 0.05, curve=0.5),
        "Hide": lambda r: env(osc(lambda t: 1200 - 1000 * t / 0.5, 0.5, "triangle"), 0.01, curve=1),
        "Inori": lambda r: chime([523, 659, 784, 1047], 1.2, 0.15),
        "UnitOn": lambda r: chime([392, 523, 659], 0.25, 0.07),
        "Transform": lambda r: mix(env(osc(lambda t: 200 + 900 * t, 0.8, "square", 0.05, 12), 0.02, curve=0.6), chime([523, 659, 784, 1047, 1319], 0.4, 0.08), at=[0, 0.6]),
        "Sharara": lambda r: chime([1047, 1175, 1319, 1568, 1760, 2093, 2349, 2637], 0.7, 0.05),
        "Applause": lambda r: mix(*[env(noise(r, 0.03, 0.8, 0.5), 0.001, curve=3) for _ in range(160)], at=[r.uniform(0, 2.0) for _ in range(160)]),
        "Null": lambda r: [0.0] * int(RATE * 0.05),
    }


# ── BGM ──
# SRC が場面ごとに鳴らす BGM（Midi/<名前>.mid）を、規則で音符を並べて作曲する（音声生成 AI は使わない）。
# 名前と場面: Opening（タイトル）・Briefing（ブリーフィング）・Intermission（インターミッション）・Subtitle（サブタイトル）、
# Map1・Map2（地上の味方・敵フェイズ）、Map3・Map4（屋内の味方・敵）、Map5・Map6（宇宙の味方・敵）、default（戦闘）
TPQ = 480  # 4 分音符の分解能
MAJOR = [0, 2, 4, 5, 7, 9, 11]
MINOR = [0, 2, 3, 5, 7, 8, 10]

# 名前: (主音の MIDI ノート番号, 音階, テンポ, 和音の進行（音階の度数）, メロディの音色, 伴奏の音色, ドラム, 小節数)
BGMS = {
    "Opening": (60, MAJOR, 120, [0, 4, 5, 3], 56, 48, "march", 16),       # トランペット・弦
    "Briefing": (57, MINOR, 84, [0, 5, 3, 4], 73, 89, "soft", 16),        # フルート・パッド
    "Intermission": (65, MAJOR, 100, [0, 3, 4, 0], 11, 0, "soft", 16),    # ビブラフォン・ピアノ
    "Subtitle": (62, MINOR, 96, [0, 5, 6, 4], 48, 89, "none", 8),         # 弦・パッド
    "Map1": (62, MAJOR, 132, [0, 4, 5, 3], 81, 33, "rock", 16),           # シンセリード・ベース
    "Map2": (64, MINOR, 132, [0, 6, 5, 4], 30, 33, "rock", 16),           # ディストーションギター
    "Map3": (60, MAJOR, 112, [0, 5, 3, 4], 80, 33, "beat", 16),
    "Map4": (59, MINOR, 112, [0, 3, 5, 4], 87, 38, "beat", 16),
    "Map5": (67, MAJOR, 124, [0, 2, 3, 4], 81, 38, "beat", 16),
    "Map6": (63, MINOR, 124, [0, 5, 2, 4], 84, 38, "beat", 16),
    "default": (64, MINOR, 150, [0, 5, 6, 4], 30, 33, "rock", 16),
}


def vlq(n):
    """MIDI の可変長の数値"""
    out = [n & 0x7F]
    n >>= 7
    while n:
        out.insert(0, (n & 0x7F) | 0x80)
        n >>= 7
    return bytes(out)


def save_midi(events, tempo, path):
    """events: (時刻（tick）, 状態バイト, データ…) の並び。フォーマット 0 の標準 MIDI ファイルに書く"""
    full = os.path.join(HERE, path)
    os.makedirs(os.path.dirname(full), exist_ok=True)
    body = b"\x00\xff\x51\x03" + int(60_000_000 / tempo).to_bytes(3, "big")
    now = 0
    for ev in sorted(events, key=lambda e: (e[0], 0 if (e[1] & 0xF0) == 0x80 else 1)):
        body += vlq(ev[0] - now) + bytes(ev[1:])
        now = ev[0]
    body += b"\x00\xff\x2f\x00"
    with open(full, "wb") as f:
        f.write(b"MThd" + struct.pack(">IHHH", 6, 0, 1, TPQ))
        f.write(b"MTrk" + struct.pack(">I", len(body)) + body)


def note(events, ch, start, length, key, vel):
    events.append((start, 0x90 | ch, key, vel))
    events.append((start + length, 0x80 | ch, key, 0))


def compose(name):
    """主音・音階・和音の進行から、伴奏（和音）・ベース・メロディ・ドラムを作る。乱数の種は名前"""
    root, scale, tempo, progression, lead, pad, drums, bars = BGMS[name]
    rnd = random.Random(name)
    bar = TPQ * 4
    events = [(0, 0xC0, lead), (0, 0xC1, pad), (0, 0xC2, 33 if pad != 33 else 38),
              (0, 0xB0, 7, 100), (0, 0xB1, 7, 80), (0, 0xB2, 7, 95)]

    def pitch(degree, octave=0):
        return root + scale[degree % 7] + 12 * (degree // 7 + octave)

    # メロディのリズム（1 小節を 8 分音符 8 つに分け、鳴らす位置と長さ）を 2 つ作り、交互に使う
    rhythms = []
    for _ in range(2):
        pattern, pos = [], 0
        while pos < 8:
            length = rnd.choice([1, 1, 2, 2, 3, 4])
            length = min(length, 8 - pos)
            pattern.append((pos, length, rnd.random() < 0.85))
            pos += length
        rhythms.append(pattern)
    degree = 0
    for b in range(bars):
        chord = progression[b % len(progression)]
        start = b * bar
        # 伴奏: 和音（1・3・5 度）を全音符で
        for d in (0, 2, 4):
            note(events, 1, start, bar - 10, pitch(chord + d, -1), 60)
        # ベース: 和音の根音を 4 分音符で（1 拍目は 1 オクターブ下）
        for beat in range(4):
            note(events, 2, start + beat * TPQ, TPQ - 20, pitch(chord, -2 if beat % 2 else -3) + 12, 90)
        # メロディ: 和音の音を中心に、前の音から近い音へ動く
        for pos, length, on in rhythms[(b // 2) % 2]:
            if not on:
                continue
            target = chord + rnd.choice([0, 2, 4, 0, 2, 4, 1, 3, 5])
            degree += max(-2, min(2, target - degree + rnd.choice([-7, 0, 0, 7]) if abs(target - degree) > 4 else target - degree))
            degree = max(-2, min(9, degree))
            note(events, 0, start + pos * TPQ // 2, length * TPQ // 2 - 20, pitch(degree), 96 if pos % 4 == 0 else 84)
        # ドラム（チャンネル 10）
        for step in range(8):
            t = start + step * TPQ // 2
            if drums == "none":
                break
            if drums == "soft":
                if step % 4 == 0:
                    note(events, 9, t, 60, 36, 60)
                note(events, 9, t, 60, 42, 40)
            elif drums == "march":
                if step in (0, 4):
                    note(events, 9, t, 60, 36, 100)
                note(events, 9, t, 60, 38, 70 if step % 2 else 95)
            else:
                if step in (0, 3 if drums == "rock" else 5):
                    note(events, 9, t, 60, 36, 110)
                if step in (2, 6):
                    note(events, 9, t, 60, 38, 105)
                note(events, 9, t, 60, 42, 75)
            if step == 0 and b % 4 == 0:
                note(events, 9, t, 60, 49, 90)
    return events, tempo


def main():
    write("Data/System/terrain.txt", terrain_txt())
    write("Data/System/sp.txt", sp_txt())
    for _, _, bitmap, *_ in TERRAINS:
        tile_dir = os.path.join(HERE, "Bitmap", "Map", bitmap)
        if os.path.isdir(tile_dir):
            for entry in os.listdir(tile_dir):
                if entry.endswith(".bmp"):
                    os.remove(os.path.join(tile_dir, entry))
        for no in TILE_NUMBERS.get(bitmap, [0]):
            save(tile(bitmap, no), f"Bitmap/Map/{bitmap}/{bitmap}{no:04d}.bmp")
    for name in UNITS:
        save(unit(name), f"Bitmap/Unit/{name}.bmp")
    for name in BGMS:
        events, tempo = compose(name)
        save_midi(events, tempo, f"Midi/{name}.mid")
    for name, make in sounds().items():
        save_wav(make(random.Random(name)), f"Sound/{name}.wav", 0.0 if name == "Null" else 0.8)


if __name__ == "__main__":
    main()
