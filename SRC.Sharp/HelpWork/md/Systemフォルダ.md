**Systemフォルダ**

**Systemフォルダ**は特殊なデータフォルダであり、以下のファイルが含まれます。

**GameOver.eve**ゲームオーバー時の処理

**Exit.eve**SRC終了時の処理

**alias.txt**標準エリアスデータ

**item.txt**汎用アイテムデータ

**sp.txt**標準スペシャルパワーデータ

これらのファイルは開発パックに含まれている場合が多いので、まずは開発パックを調べてみてください。ただし、GameOver.eve と Exit.eve は登場キャラをシナリオの登場キャラに合わせるため、自作した方が良いでしょう。

alias.txt 及び sp.txt はVer.1.6系との互換性維持のためにSRC本体に添付されているデータが流用できるようになっています。これらのファイルはSRC本体側の Data\System フォルダに置かれています。

GameOver.eve や Exit.eve を自作する場合は下記の例を参考にして下さい。なお、両ファイルでは処理の最後に[Quitコマンド](Quitコマンド.md)を実行させてください。また、GameOver.eveからゲームオーバー時のコンティニューをさせる場合[はQuickLoadコマンド](はQuickLoadコマンド.md)を使用して下さい。

GameOver.eve や Exit.eve を作るのが面倒 or 作り方がよく分からないという場合はこれらのファイルをSystemフォルダ内に置かなくてもかまいません。なくてもゲーム終了時やゲームオーバー時にメッセージが表示されないだけでプレイは可能です(ただしGameOver.eveがないとゲームオーバー時にコンティニューできなくなります)。

############### Exit.eve ###############

プロローグ:

#終了時のBGM

StartBGM End.mid

#用意したメッセージパターンからランダムに１つを選択

Switch Random(2)

Case 1

StartBGM xxxxxxx.mid

Talk ○○

終了時メッセージです

End

Case 2

StartBGM xxxxxx2.mid

StartBGM xxxxxx3.mid

Talk ○○２

またお会いしましょう～

End

EndSw

#SRCを終了

Quit

############### GameOver.eve ###############

プロローグ:

#用意したゲームオーバーメッセージからランダムに一つを選択

Switch Random(2)

Case 1

Talk ○○

ＧＡＭＥ ＯＶＥＲメッセージです

End

Case 2

Talk ○○

ＧＡＭＥ ＯＶＥＲだ

End

EndSw

#コンティニュー時ＢＧＭ

StartBGM Continue.mid

Wait 10

#用意したコンティニューメッセージからランダムに一つを選択

Switch Random(2)

Case 1

Talk ○○

コンテニューする？

End

#プレイヤーにコンティニューするか尋ねる

Ask

ようし！ リベンジだ！！

いや、もういい…

End

PlaySound Type.wav

If 選択 = 1 Then

Talk ○○

そうこなくっちゃ！ それじゃいくよ！

End

#クイックロードしてやり直す

QuickLoad

Endif

Talk ○○

え～っ！ やめちゃうの！？

End

Case 2

Talk ○○２

コンティニューする？

End

#プレイヤーにコンティニューするか尋ねる

Ask

無論コンティニュー

やる気無くなったよ…

End

PlaySound Type.wav

If 選択 = 1 Then

Talk ○○２

お～、やっぱそうでなくっちゃね！ それじゃ頑張ってね～

End

#クイックロードしてやり直す

QuickLoad

Endif

Talk ○○

え？ もう寝ちゃうわけ？ ちょっと早いんじゃない？

End

EndSw

Quit

########################################

GameOver.eve と Exit.eve 内に登場するキャラは、Systemフォルダ内に non\_pilot.txt 作って登録しておけば「@」を使って対応する作品のデータをロードしておかなくても済みます。
