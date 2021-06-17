** 内容はSRC2.2.33のものです **

**ClearObjコマンド**

画面からオブジェクトを削除

**書式**

**ClearObj** [*name*]

**指定項目説明**

*name*削除するオブジェクトの名称 （省略可）

**解説**

画面から指定した名称のオブジェクトを削除します(オブジェクトとは画面上におかれるGUI部品です)。*name* を省略した場合は画面上からすべてのオブジェクトを削除します。

![](../images/bm0.gif) 現在のところオブジェクトとしては**ホットポイント**のみがサポートされています

**例**
```sh
Talk システム
主人公を選んでください
End
#画像の描画とホットポイントの作成
HotPoint     ジェイ     64  128 64 64
PaintPicture ジェイ     64  128 64 64
HotPoint     ミネルバ   160 128 64 64
PaintPicture ミネルバ   160 128 64 64
HotPoint     ロイ       256 128 64 64
PaintPicture ロイ       256 128 64 64
HotPoint     リンダ     352 128 64 64
PaintPicture リンダ     352 128 64 64
#画面の更新
Refresh
#ホットポイントがクリックされるのを待つ
Do
Wait Click
Loop While (選択 = "")
#画像とホットポイントを削除
ClearObj
ClearPicture
Talk システム
$(選択)が主人公に設定されました
End
```

