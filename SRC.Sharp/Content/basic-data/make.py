#!/usr/bin/env python3
# SPDX-License-Identifier: MIT
# Copyright (c) 2026 koudenpa
"""
SRC# の最小の汎用データ（basic-data）を作る。

SRC のフルパッケージの汎用データ（グラフィック集など）は再配布できないため（docs/decisions/2026-10-12-asset-redistribution.md）、
同じファイル名で差し替えられる素材を自作する。ライセンスは ../README.md の「ライセンス」を参照（このスクリプトは MIT、作る素材は CC0 1.0）。

- Data/System/terrain.txt: 地形データ。地形の番号と名前は汎用データと同じにする（汎用データ用のマップをそのまま使える）
- Data/System/sp.txt: スペシャルパワーデータ。ヘルプ（スペシャルパワー.md）の効果の説明から書いた
- Bitmap/Map/<名前>/<名前>0000.bmp: 地形の画像（32×32）
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
    image.convert("RGB").quantize(colors=64, method=Image.Quantize.MEDIANCUT, dither=Image.Dither.NONE).save(full, "BMP")


def speckle(d, rnd, colors, count, size=1):
    for _ in range(count):
        x, y = rnd.randrange(SIZE), rnd.randrange(SIZE)
        d.rectangle([x, y, x + size - 1, y + size - 1], fill=rnd.choice(colors))


def tile(name):
    rnd = random.Random(name)
    grass = (96, 168, 72)
    im = Image.new("RGB", (SIZE, SIZE), grass)
    d = ImageDraw.Draw(im)
    if name == "plain":
        speckle(d, rnd, [(84, 152, 62), (112, 184, 84)], 40)
        for _ in range(4):
            x, y = rnd.randrange(2, 29), rnd.randrange(3, 30)
            d.line([x, y, x - 1, y - 2], fill=(70, 130, 52))
            d.line([x + 1, y, x + 2, y - 2], fill=(70, 130, 52))
    elif name == "bush":
        speckle(d, rnd, [(84, 152, 62), (112, 184, 84)], 30)
        for cx, cy in [(8, 10), (22, 8), (14, 22), (26, 24)]:
            d.ellipse([cx - 5, cy - 4, cx + 5, cy + 4], fill=(56, 120, 48), outline=(36, 84, 32))
            d.point([cx - 2, cy - 2], fill=(110, 170, 80))
    elif name == "woods":
        im.paste((62, 128, 52), [0, 0, SIZE, SIZE])
        for cx, cy in [(8, 9), (23, 7), (15, 20), (6, 26), (26, 25)]:
            d.polygon([(cx, cy - 7), (cx - 6, cy + 4), (cx + 6, cy + 4)], fill=(30, 96, 40), outline=(20, 64, 28))
            d.rectangle([cx - 1, cy + 4, cx, cy + 6], fill=(96, 64, 32))
    elif name == "mountain":
        im.paste((120, 156, 84), [0, 0, SIZE, SIZE])
        d.polygon([(2, 30), (14, 4), (26, 30)], fill=(140, 110, 80), outline=(90, 68, 48))
        d.polygon([(14, 30), (23, 12), (31, 30)], fill=(122, 94, 66), outline=(90, 68, 48))
        d.polygon([(11, 10), (14, 4), (17, 10)], fill=(240, 240, 240))
    elif name == "waste_land":
        im.paste((176, 156, 108), [0, 0, SIZE, SIZE])
        speckle(d, rnd, [(150, 130, 88), (196, 178, 128), (130, 112, 76)], 70)
        d.line([4, 20, 10, 23, 16, 21], fill=(120, 100, 68))
    elif name == "rock":
        im.paste((176, 156, 108), [0, 0, SIZE, SIZE])
        speckle(d, rnd, [(150, 130, 88)], 30)
        for box in [[3, 6, 15, 16], [16, 14, 29, 26], [5, 20, 13, 28]]:
            d.ellipse(box, fill=(128, 128, 128), outline=(72, 72, 72))
            d.arc(box, 200, 260, fill=(200, 200, 200))
    elif name == "hill":
        speckle(d, rnd, [(84, 152, 62), (112, 184, 84)], 30)
        d.pieslice([-2, 8, 33, 44], 180, 360, fill=(132, 180, 88), outline=(80, 128, 56))
        d.arc([4, 14, 27, 40], 210, 330, fill=(160, 204, 112))
    elif name == "sea":
        im.paste((48, 96, 192), [0, 0, SIZE, SIZE])
        for y in range(4, SIZE, 8):
            for x in range((y // 8 % 2) * 8, SIZE, 16):
                d.arc([x, y, x + 8, y + 5], 200, 340, fill=(140, 184, 240))
    elif name == "river":
        im.paste((64, 128, 208), [0, 0, SIZE, SIZE])
        d.line([0, 0, 0, SIZE], fill=(96, 168, 72), width=3)
        d.line([31, 0, 31, SIZE], fill=(96, 168, 72), width=3)
        for y in range(4, SIZE, 9):
            d.line([8, y, 14, y + 2], fill=(160, 200, 244))
            d.line([18, y + 4, 24, y + 6], fill=(160, 200, 244))
    elif name == "road":
        im.paste((150, 150, 150), [0, 0, SIZE, SIZE])
        speckle(d, rnd, [(136, 136, 136), (164, 164, 164)], 40)
        d.line([0, 15, SIZE, 15], fill=(236, 236, 220), width=2)
        for x in range(2, SIZE, 8):
            d.line([x, 15, x + 3, 15], fill=(150, 150, 150), width=2)
    elif name == "bridge":
        im.paste((64, 128, 208), [0, 0, SIZE, SIZE])
        d.rectangle([0, 6, SIZE, 25], fill=(160, 112, 64))
        for x in range(0, SIZE, 4):
            d.line([x, 6, x, 25], fill=(120, 80, 44))
        d.line([0, 6, SIZE, 6], fill=(80, 52, 28))
        d.line([0, 25, SIZE, 25], fill=(80, 52, 28))
    elif name == "town":
        speckle(d, rnd, [(84, 152, 62)], 20)
        for x, y, roof in [(3, 5, (196, 64, 48)), (17, 3, (64, 96, 176)), (9, 18, (176, 120, 48)), (21, 18, (196, 64, 48))]:
            d.rectangle([x, y + 4, x + 9, y + 11], fill=(232, 220, 196), outline=(96, 80, 64))
            d.polygon([(x - 1, y + 4), (x + 5, y), (x + 10, y + 4)], fill=roof, outline=(80, 40, 32))
            d.rectangle([x + 4, y + 8, x + 5, y + 11], fill=(96, 64, 40))
    elif name == "building":
        im.paste((150, 150, 150), [0, 0, SIZE, SIZE])
        for x, top in [(2, 4), (17, 10)]:
            d.rectangle([x, top, x + 12, 30], fill=(176, 188, 204), outline=(72, 80, 96))
            for wy in range(top + 3, 28, 4):
                for wx in range(x + 2, x + 11, 4):
                    d.rectangle([wx, wy, wx + 1, wy + 1], fill=(72, 112, 176))
    elif name == "base":
        im.paste((150, 150, 150), [0, 0, SIZE, SIZE])
        d.rectangle([3, 12, 28, 29], fill=(112, 124, 104), outline=(56, 64, 48))
        d.chord([3, 4, 28, 20], 180, 360, fill=(128, 140, 120), outline=(56, 64, 48))
        d.rectangle([12, 20, 19, 29], fill=(64, 64, 64))
        d.line([16, 2, 16, 8], fill=(56, 64, 48))
        d.rectangle([17, 2, 21, 4], fill=(200, 48, 48))
    elif name == "castle":
        speckle(d, rnd, [(84, 152, 62)], 20)
        d.rectangle([4, 10, 27, 29], fill=(176, 172, 160), outline=(88, 84, 76))
        for x in range(4, 28, 6):
            d.rectangle([x, 6, x + 3, 10], fill=(176, 172, 160), outline=(88, 84, 76))
        d.rectangle([13, 20, 18, 29], fill=(72, 52, 36))
        d.rectangle([9, 14, 10, 16], fill=(40, 40, 40))
        d.rectangle([21, 14, 22, 16], fill=(40, 40, 40))
    elif name == "powerplant":
        im.paste((150, 150, 150), [0, 0, SIZE, SIZE])
        d.rectangle([3, 14, 20, 29], fill=(188, 180, 160), outline=(80, 76, 64))
        d.polygon([(22, 29), (24, 6), (29, 6), (31, 29)], fill=(200, 196, 188), outline=(80, 76, 64))
        d.polygon([(8, 10), (12, 2), (11, 8), (15, 8), (10, 16), (11, 10)], fill=(248, 216, 48), outline=(160, 120, 0))
    elif name == "remains":
        im.paste((150, 140, 120), [0, 0, SIZE, SIZE])
        speckle(d, rnd, [(110, 104, 92), (176, 168, 148)], 60)
        d.polygon([(4, 28), (4, 10), (9, 6), (12, 14), (12, 28)], fill=(128, 128, 128), outline=(64, 64, 64))
        d.polygon([(18, 28), (18, 16), (24, 12), (28, 20), (28, 28)], fill=(112, 112, 112), outline=(64, 64, 64))
    elif name == "ruins":
        im.paste((176, 160, 112), [0, 0, SIZE, SIZE])
        speckle(d, rnd, [(150, 136, 92)], 40)
        for x in (5, 14, 23):
            d.rectangle([x, 9, x + 4, 28], fill=(216, 204, 172), outline=(120, 108, 84))
        d.rectangle([3, 5, 20, 9], fill=(216, 204, 172), outline=(120, 108, 84))
    elif name == "lava":
        im.paste((168, 40, 16), [0, 0, SIZE, SIZE])
        for _ in range(8):
            x, y = rnd.randrange(SIZE), rnd.randrange(SIZE)
            d.ellipse([x - 3, y - 2, x + 3, y + 2], fill=rnd.choice([(248, 136, 24), (252, 200, 64)]))
        speckle(d, rnd, [(96, 24, 16)], 20, 2)
    else:
        raise ValueError(name)
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
        save(tile(bitmap), f"Bitmap/Map/{bitmap}/{bitmap}0000.bmp")
    for name in UNITS:
        save(unit(name), f"Bitmap/Unit/{name}.bmp")
    for name in BGMS:
        events, tempo = compose(name)
        save_midi(events, tempo, f"Midi/{name}.mid")
    for name, make in sounds().items():
        save_wav(make(random.Random(name)), f"Sound/{name}.wav", 0.0 if name == "Null" else 0.8)


if __name__ == "__main__":
    main()
