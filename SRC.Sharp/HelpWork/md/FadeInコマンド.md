**FadeInコマンド**

画面をフェードイン

**書式**

**FadeIn** [*times*]

**指定項目説明**

*times*フェードインの段階数(省略可能)

**解説**

画面をフェードイン(真っ黒な画面から段階的に画像を浮かび上がらせる)します。[**PaintPicture**コマンド](PaintPictureコマンド)や[**PaintString**コマンド](PaintStringコマンド)で画面を描画した後、[**Refresh**コマンド](Refreshコマンド)を使わずに**FadeIn**コマンドを使用して下さい。

*times* を指定すると指定した段階数でフェードインを行います。省略した場合は10段階でフェードインします。1段階の描画にかかる時間は0.05秒です(描画処理が遅いPCではもっと時間がかかる場合があります)。

**例**

#フォントサイズ・種類を変更

Font 24pt Ｐゴシック Regular

#色を変更

Font RGB(80,160,255)

#メッセージを描画

PaintString - 200 A long time ago in a galaxy far,

PaintString far away ....

#フェードイン

FadeIn

#デフォルトのフォント設定に戻しておく

Font
