#!/usr/bin/env python3
# SPDX-License-Identifier: MIT
# Copyright (c) 2026 koudenpa
"""
SRC# の最小の汎用データ（basic-data）を作る。

SRC のフルパッケージの汎用データ（グラフィック集など）は再配布できないため（docs/assets/redistribution-study.md）、
同じファイル名で差し替えられる素材を自作する。ライセンスは ../README.md の「ライセンス」を参照（このスクリプトは MIT、作る素材は CC0 1.0）。

- Data/System/terrain.txt: 地形データ。地形の番号と名前は汎用データと同じにする（汎用データ用のマップをそのまま使える）
- Data/System/sp.txt: スペシャルパワーデータ。ヘルプ（スペシャルパワー.md）の効果の説明から書いた
- Bitmap/Map/<名前>/<名前>0000.bmp: 地形の画像（32×32）
- Bitmap/Unit/OBJ_*.bmp: ユニットの画像（32×32、白が透過色）
- Lib/ユニットステータス表示.eve・Lib/パイロットステータス表示.eve: インターミッションの「ユニットステータス」「パイロットステータス」。
  ヘルプ（ユニットステータス.md・MakeUnitListコマンド.md など）から書いた手書きのファイルで、このスクリプトは作らない

使い方: python3 make.py（Pillow が必要）。出力はこのフォルダに書く。乱数の種を固定しているので、何度作っても同じになる。
"""
import os
import random

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


def main():
    write("Data/System/terrain.txt", terrain_txt())
    write("Data/System/sp.txt", sp_txt())
    for _, _, bitmap, *_ in TERRAINS:
        save(tile(bitmap), f"Bitmap/Map/{bitmap}/{bitmap}0000.bmp")
    for name in UNITS:
        save(unit(name), f"Bitmap/Unit/{name}.bmp")


if __name__ == "__main__":
    main()
