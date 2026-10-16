# 生成した画像の指示（プロンプト）

「灯の巡航船ランタン号」の顔グラフィックとユニットの画像を、[PixelLab](https://www.pixellab.ai/) の Pixen モデルで生成したときの指示。生成AIの利用の概要は [README.md](README.md) を参照。

顔グラフィックの共通部分（64×64）:

> pixel art character face portrait, extreme close-up of the face only, head fills the whole frame, cropped at chin and forehead, no shoulders, front view, anime JRPG style, plain dark background

ユニットの画像の共通部分（32×32、背景は透過、黒の 1 色の輪郭線）:

> pixel art game unit sprite, single object centered, top-down 3/4 view, small strategy RPG map unit, transparent background

| ファイル | 使うところ | 指示 |
|---|---|---|
| `Bitmap/Pilot/Lantern_Sora.bmp` | ソラ | 共通 + young boy hero, short messy brown hair, determined dark eyes, blue background |
| `Bitmap/Pilot/Lantern_Rio.bmp` | リオ | 共通 + young woman pilot, orange hair with side ponytail, green eyes, cheerful, green background |
| `Bitmap/Pilot/Lantern_Gustav.bmp` | グスタフ | pixel art character face portrait, close-up of the face, head fills most of the frame, neck and top of uniform collar visible at the bottom edge, front view, anime JRPG style, plain dark background, old airship captain, weathered face, white beard and mustache, navy captain cap with gold emblem, navy captain uniform collar, calm eyes, brown background |
| `Bitmap/Pilot/Lantern_Myu.bmp` | ミュウ、謎の少女 | 共通 + gentle shrine maiden girl, long lavender silver hair, violet eyes, shy expression, purple background |
| `Bitmap/Pilot/Lantern_MyuAwakened.bmp` | ミュウ(覚醒) | 共通 + shrine maiden girl awakened, long lavender silver hair glowing, shining golden violet eyes, small golden circlet on forehead, serene, luminous purple background |
| `Bitmap/Pilot/Lantern_Wolf.bmp` | ヴォルフ | 共通 + grey haired mercenary man, spiky grey hair, sharp golden eyes, scar on cheek, stern, dark grey background |
| `Bitmap/Pilot/Lantern_Dora.bmp` | 教官ドーラ、ドーラ教官 | 共通 + strict female drill instructor, short dark red hair, headband, confident smirk, olive background |
| `Bitmap/Pilot/Lantern_Soot.bmp` | 煤兵(ザコ)、煤鴉兵(ザコ) | 共通 + faceless enemy soldier wearing soot black gas mask helmet with glowing orange goggles, dark smoky background |
| `Bitmap/Pilot/Lantern_Crow.bmp` | クロウ | 共通 + enemy ace pilot, black wide brim hat, black visor goggles with glowing red lenses, sinister grin, dark red background |
| `Bitmap/Pilot/Lantern_Nox.bmp` | ノクス | 共通 + demonic soot emperor, black spiky crown of smoke, shadowed dark face, glowing red eyes, menacing, black red background |
| `Bitmap/Pilot/Lantern_Marta.bmp` | マルタ町長 | 共通 + kind elderly town mayor woman, grey hair in a bun, round glasses, warm smile, green grey background |
| `Bitmap/Pilot/Lantern_Popo.bmp` | よろず屋ポポ | 共通 + cheerful young shopkeeper boy, brown hair, freckles, merchant bandana, big smile, warm green background |
| `Bitmap/Unit/Lantern_Hotaru.bmp` | 灯機ホタル | 共通 + slim blue and white humanoid mecha robot, firefly motif, small antenna, glowing yellow lantern light on chest, sleek armor |
| `Bitmap/Unit/Lantern_HotaruFly.bmp` | 灯機ホタル(飛行形態) | 共通 + blue and white jet fighter mecha in flight mode seen from above, firefly motif, glowing yellow lantern light at the tail, sleek |
| `Bitmap/Unit/Lantern_Tsubame.bmp` | 灯機ツバメ | 共通 + slim white and teal humanoid mecha robot with swallow-like wings on back, lantern light on chest |
| `Bitmap/Unit/Lantern_Hikario.bmp` | 灯翼ヒカリオー | 共通 + large heroic combined super robot, blue white and teal armor, big swallow wings, glowing lantern core on chest |
| `Bitmap/Unit/Lantern_Ship.bmp` | ランタン号（タイトル・終話の一枚絵にも使う） | 共通 + airship with a large cream colored canvas balloon envelope on top and a small wooden gondola hanging below, warm glowing lanterns, side view |
| `Bitmap/Unit/Lantern_Inorite.bmp` | 祈り手 | 共通 + shrine maiden girl in white and red robes holding a glowing lantern staff, full body |
| `Bitmap/Unit/Lantern_Tousei.bmp` | 灯精 | 共通 + small glowing orange ball of light like a firefly lantern flame with soft glow and tiny sparkles, not a person, no face, no ground, no shadow |
| `Bitmap/Unit/Lantern_Hairou.bmp` | 灰狼 | 共通 + grey wolf shaped beast mecha robot, four legs, metal armor |
| `Bitmap/Unit/Lantern_Ginrou.bmp` | 灰狼改 | 共通 + silver wolf shaped beast mecha robot upgraded, four legs, sleek silver armor with blue lights |
| `Bitmap/Unit/Lantern_Kagerou.bmp` | 予備機カゲロウ | 共通 + plain olive green spare humanoid mecha robot, simple design |
| `Bitmap/Unit/Lantern_Dummy.bmp` | 模擬戦用ダミー | 共通 + wooden training dummy target on a post with a red bullseye, no ground, no base tile, no shadow, floating object only |
| `Bitmap/Unit/Lantern_Sukihei.bmp` | 煤機兵 | 共通 + soot black small tank-like walking machine with smoke stack, glowing orange eye |
| `Bitmap/Unit/Lantern_Susugarasu.bmp` | ススガラス | 共通 + black mechanical crow made of soot and iron, wings spread, glowing red eye |
| `Bitmap/Unit/Lantern_Susuro.bmp` | 煤炉 | 共通 + black iron furnace with burning orange fire and smoke chimneys, industrial |
| `Bitmap/Unit/Lantern_Kyohei.bmp` | 煤の巨兵 | 共通 + giant hulking soot black iron golem robot, glowing orange cracks |
| `Bitmap/Unit/Lantern_Kurogane.bmp` | 黒鉄鴉 | 共通 + black iron humanoid with crow-like black wings, sinister knight mecha, red visor |
| `Bitmap/Unit/Lantern_Nox1.bmp` | 煤帝ノクス(第一形態) | 共通 + bold simple silhouette of a black walking castle fortress with two towers and a gate mouth, two glowing red eyes, low detail, readable at small size |
| `Bitmap/Unit/Lantern_Nox2.bmp` | 煤帝ノクス(第二形態) | 共通 + flying demonic black iron warship emperor, spiky silhouette, red glowing core, smoke trails |
