# かんたん作成（SRC#Studio）

ソフトウェア開発の知識があまり無くても、好きな作品のキャラを好きな場面で戦わせ、すぐにブラウザで遊べるようにする（7474/SRC#1659。ゴールは親の 7474/SRC#1649）。SRC#Studio の「かんたん作成」のタブで、次の流れを 1 つの画面で行う。

0. **サンプルの場面から始める**（選ばなくてもよい）: 一番上の一覧（下の「場面のカタログ」）から、押して開き、縮図・説明・タグを見て選ぶ（既定は閉じていて、縮図は開いたときにまとめて読む）。マップ・作品・ユニット・セリフがまとめて入り、選ぶと閉じる
1. **作品を選ぶ**: 作品のデータ（7474/SRC-Data の GSC データパック）を名前で探して選ぶ。同じデータパックの `System`（共通の別名・アイテム・スペシャルパワー）も一緒に置く
2. **味方と敵を選ぶ**: 選んだ作品のユニット・パイロットを、名前の候補から選ぶ。レベル・ランク（改造の段階。0〜20）・ボスランク（ボスにしないか 0〜5）を決める（広い画面では味方と敵を左右に並べる）
3. **マップと配置**: マップ（平地・場面のサンプル・開いたシナリオのフォルダのマップ・手元の `.map`）と、ユニットを置く位置。マップは手元の汎用データ（既定は最小の汎用データ）の画像で描く
4. **セリフを書く**: 戦闘の前・勝ったとき・負けたときの会話。話す人は置いたユニットのパイロットかナレーション。戦闘中のセリフは選んだパイロットのデータ（`pilot_message.txt`）のもの
5. **題名を付けて遊ぶ**: 題名を決め、「SRC#Web で遊ぶ」で同じサイトの SRC#Web に渡してすぐ始める。「zip で保存」で、VS Code で開いて続きを書ける形の zip を保存する

入力は場面の組み立て（誰が・どこで・何を言うか）だけで、データのフォームは作らない（[作成環境の補完](../decisions/2026-11-14-authoring-environment.md)の方針）。書いた内容はそのブラウザに下書きとして残る（`localStorage`。作品のデータは開き直したときに読み直す）。

## 作るもの

zip の中身は普通のシナリオで、`srcsharp lint`・VS Code 拡張・SRC#Avalonia でもそのまま使える。

| ファイル | 中身 |
|---|---|
| `題名.eve` | `@作品`（作品のデータの読み込み）・`スタート:`（`ChangeMap`・`Create`（ランクは 3 番目の引数）・ボスなら続けて `BossRank`・戦闘の前の `Talk`）・`全滅 敵:`（勝ち。`GameClear`）・`全滅 味方:`（負け。`GameOver`） |
| `題名.map` | 選んだマップ。平地なら大きさを決めた平地 |
| `Data/作品/…` | 選んだ作品のデータ（SRCCore が作品のフォルダから読むファイルだけ。名前は小文字にそろえる） |

イベントは `SRCAuthoring.Templates.QuickBattleTemplate`（SRC#Studio の `BuildQuickBattle`）が作る。セリフのうち、命令や注釈に読まれる行（`End`・`Talk` などで始まる行・`#` で始まる行）は頭に全角の空白を足す。話す人が置いたユニットのパイロットでなければ作らずに知らせる（`Talk` は定義の無い名前で止まるため）。

## サイトごとの設定（`studio.json`）

SRC#Studio の隣の `studio.json` に書く。無ければ既定（下の値）を使う。

```json
{
  "web": "../web/",
  "requires": ["basic-data"],
  "data": "https://raw.githubusercontent.com/7474/SRC-Data/master/",
  "scenes": ["https://raw.githubusercontent.com/7474/SRC-Data/master/scenes/index.json"]
}
```

| 項目 | 意味 |
|---|---|
| `web` | SRC#Web の場所（SRC#Studio からの相対 URL か絶対 URL）。SRC#Studio と同じサイト（オリジン）に無ければ「SRC#Web で遊ぶ」は使えない（zip の保存だけ） |
| `requires` | 遊ぶときに重ねる SRC#Web のカタログの素材の `id`（汎用データ）。GitHub Pages は `basic-data`、SRC-Site は `src-basic` など |
| `data` | 作品のデータの場所。`index.json`（7474/SRC-Data の形）と、その `path` の下の各ファイル |
| `scenes` | 場面のカタログの URL の一覧（下の「場面のカタログ」）。読めないものは飛ばす |

| 置き場所 | `web` | `requires` |
|---|---|---|
| GitHub Pages（開発版。`/SRC/studio/`） | `../web/` | `basic-data` |
| SRC-Site（`/studio/`。7474/SRC-Site の `scripts/assemble.sh` が書く） | `/play/` | `src-basic` |

SRC#Studio は開発版のリリース（タグ `dev`）の `SRCStudio.zip` として配る（[運用の全体像](../operations.md)）。

## SRC#Web への受け渡し

SRC#Studio は、作った zip を SRC#Web の「追加したアーカイブ」と同じところ（ブラウザの IndexedDB の `srcweb` の `archives`。`SRCSharpWeb/wwwroot/js/store.js`）に、`requires` を付けて入れ、SRC#Web を `?stored=名前` で開く（`SRCStudio/wwwroot/web-handoff.js`）。SRC#Web は `requires` の素材を重ねてすぐに始める。IndexedDB はサイトごとなので、同じサイトの SRC#Studio と SRC#Web の間だけで使える。

- 入れたシナリオは SRC#Web の一覧に残り、次からはそこから遊べる（`requires` も引き継ぐ）
- `requires` の素材がそのサイトのカタログに無ければ、重ねずに遊ぶ（別のサイトで作った zip を入れたときなど）
- SRC#Web の側は `?stored=`（[SRC#Web を置く](../gui/web-hosting.md)）

## 場面のカタログ

場面のサンプル（マップと、おすすめの置く位置。有名作品の場面なら作品・ユニット・セリフも）は、作品のデータと同じく SRC# のリポジトリの外に置き、`studio.json` の `scenes` で読む。カタログは JSON で、パスはカタログの URL からの相対で書く。

```json
{
  "scenes": [
    {
      "id": "harbor",
      "title": "港の戦い",
      "description": "港での小競り合い（20×20）",
      "tags": ["海", "小規模"],
      "map": "maps/harbor.map",
      "allies": [{ "x": 3, "y": 10 }, { "x": 4, "y": 10 }],
      "enemies": [{ "x": 15, "y": 4 }, { "x": 16, "y": 5 }],
      "titles": ["GSCデータパック/ロボ/∀ガンダム"],
      "units": {
        "allies": [{ "unit": "∀ガンダム", "pilot": "ロラン＝セアック", "level": 25, "x": 3, "y": 10 }],
        "enemies": [{ "unit": "ボルジャーノン", "pilot": "ミリシャ兵(ザコ)", "level": 5, "bossRank": 1, "x": 15, "y": 4 }]
      },
      "talk": {
        "opening": [{ "speaker": "", "text": "月の光が照らす夜。" }, { "speaker": "ロラン＝セアック", "text": "行きます！" }],
        "victory": [{ "speaker": "ロラン＝セアック", "text": "終わった……" }],
        "defeat": []
      }
    }
  ]
}
```

| 項目 | 必須 | 意味 |
|---|---|---|
| `id` | ○ | カタログの中で一意の名前 |
| `title`・`description`・`tags` | | 一覧に出す名前と説明 |
| `map` | ○ | マップ（`.map`）。地形の番号は SRC の汎用データ（`Data/System/terrain.txt`）のもの。どのサイトでも出せるよう、SRC# の最小の汎用データ（`SRC.Sharp/Content/basic-data`）にある地形（地上と、宇宙空間・暗礁空間・コロニー・宇宙要塞・月面など）で描く |
| `allies`・`enemies` | | おすすめの置く位置（1 始まり）。置いたユニットをこの順に移し、足したユニットは空いている位置に置く |
| `titles` | | 一緒に選ぶ作品（作品のデータの `index.json` の `path`） |
| `units` | | 置くユニット（`unit`・`pilot`・`level`・`x`・`y`。任意で `rank`（既定 0）・`bossRank`（既定 -1 はボスにしない。0〜5）） |
| `talk` | | セリフ（`opening`・`victory`・`defeat`。`speaker` が空ならナレーション） |

`titles`・`units`・`talk` のある場面を選ぶと、何も選んでいなければそのまま入れ、選んでいれば置き換えるかを尋ねる（「キャンセル」ならマップと置く位置だけを使う）。遊ぶときのマップの画像は、遊ぶ SRC#Web の汎用グラフィック集のものが出る（無ければ地形の色）。SRC#Studio の画面では、手元の汎用データの画像で描く（無いマスは地形タイプの色）。

知らせ（読み込み中・保存した・足りないもの）は画面の下に出す。

## テスト

- `SRCAuthoringTests/QuickBattleTemplateTests.cs`: イベント・マップの書き方、足りないものの知らせ
- `SRCAuthoringTests/CliTests.cs` の `QuickBattle_LintsCleanAndPlaysToTheEnd`: 作ったシナリオが最小の汎用データで lint を通り、最後まで自動で遊べる
- `SRCSharpWeb.E2E/tests/studio.spec.js` の「かんたん作成」: 作品選び・配置・セリフ・zip・SRC#Web への受け渡し・下書き・場面のカタログ（一覧のカードとマップの欄。7474/SRC-Data の代わりを返す）
- `SRCSharpWeb.E2E/tests/web.spec.js`: `?stored=` で開き、求める素材を重ねて始める
