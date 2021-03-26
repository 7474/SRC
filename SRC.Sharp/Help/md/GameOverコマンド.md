** 内容はSRC2.2.33のものです **
**GameOverコマンド**

ゲームオーバー

**書式**

**GameOver**

**解説**

ゲームオーバーの処理を行います。

シナリオのあるフォルダ\Data\System\GameOver.eve

Src.exeのあるフォルダ\Data\System\GameOver.eve

を実行した後、SRCを終了します。この時、最終セーブ時またはスタートイベント時からのコンティニューが選択できます。コンティニューの処理の関係上、プロローグイベント中に**GameOver**コマンドを使うことはできません。**GameClear**コマンドを使ってください。

GameOver.eveを自作する場合にはGameOver.eve終了時に**Quit**コマンドを実行するよう注意してください。

**例**

#味方ユニットの全滅時

全滅 味方:

Talk ライト

くっ、敵の戦力がこれほどとは……

End

#ゲームオーバー

GameOver
