# Effect.bas

VB 版の `SRC/SRC_20121125/Effect.bas` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub ShowAnimation | 20 | 60 | `SRCCore/Effect.cs` Effect.ShowAnimation | 一致 | FindNormalLabel の「無い」は C# では負の数（VB は 0）。VB の ErrorHandler は On Error が無いので使われていない。C# は例外を受けて DisplayEventErrorMessage で出す（SRC# のエラーの仕組み）。picMain(0).Refresh は GUI.UpdateScreen に当たる |
| Sub PrepareWeaponEffect | 83 | 12 | `SRCCore/Effect.cs` Effect.PrepareWeaponEffect | 一致 |  |
| Sub PrepareWeaponAnimation | 97 | 323 | `SRCCore/Effect.cs` Effect.PrepareWeaponAnimation | 一致 | with_face_up は VB でも代入がコメントアウトされていて使われない。アイテムから探すループ（最初に当たった装備で Exit For）も一致 |
| Function CheckWeaponType | 422 | 532 | `SRCCore/Effect.cs` Effect.CheckWeaponType | 一致 | 判定と選ぶ順が全行一致 |
| Sub PrepareWeaponSound | 956 | 27 | `SRCCore/Effect.cs` Effect.PrepareWeaponSound | 一致 |  |
| Sub AttackEffect | 986 | 12 | `SRCCore/Effect.cs` Effect.AttackEffect | 一致 |  |
| Sub AttackAnimation | 1000 | 1288 | `SRCCore/Effect.cs` Effect.AttackAnimation | 一致 | 武器の種類の判定・Select Case のラベル・bmpname・回数が一致 |
| Sub AttackSound | 2290 | 352 | `SRCCore/Effect.cs` Effect.AttackSound | 一致 | Sleep の値・回数のループが一致 |
| Sub HitEffect | 2645 | 14 | `SRCCore/Effect.cs` Effect.HitEffect | 一致 |  |
| Sub HitAnimation | 2661 | 1006 | `SRCCore/Effect.cs` Effect.HitAnimation | 一致 | VB の Or の中の > 0 の無い InStr は、ビットの Or でも 0 でなければ真になるので同じ |
| Sub HitSound | 3669 | 339 | `SRCCore/Effect.cs` Effect.HitSound | 一致 | t.IsHero の判定、Sleep の値と回数が一致 |
| Sub DodgeEffect | 4010 | 51 | `SRCCore/Effect.cs` Effect.DodgeEffect | 一致 |  |
| Sub ParryEffect | 4063 | 56 | `SRCCore/Effect.cs` Effect.ParryEffect | 一致 |  |
| Sub ShieldEffect | 4121 | 20 | `SRCCore/Effect.cs` Effect.ShieldEffect | 一致 |  |
| Sub AbsorbEffect | 4143 | 30 | `SRCCore/Effect.cs` Effect.AbsorbEffect | 一致 |  |
| Sub CriticalEffect | 4176 | 36 | `SRCCore/Effect.cs` Effect.CriticalEffect | 一致 | VB は SelectedUnit.IsWeaponClassifiedAs(w, "冷") で調べるので、反撃のときなどは別のユニットの武器番号を見てしまう（VB の不具合）。C# は攻撃に使った武器 w で調べる（合わせない） |
| Function CountAttack | 4215 | 16 | `SRCCore/Effect.cs` Effect.CountAttack | 一致 |  |
| Function CountAttack0 | 4232 | 150 | `SRCCore/Effect.cs` Effect.CountAttack0 | 一致 | Integer の関数への代入は VBMath.CLng で偶数丸め |
| Function IsBeamWeapon | 4384 | 81 | `SRCCore/Effect.cs` Effect.IsBeamWeapon | 一致 |  |
| Function SpellColor | 4467 | 104 | `SRCCore/Effect.cs` Effect.SpellColor | 一致 |  |
| Sub DieAnimation | 4574 | 112 | `SRCCore/Effect.cs` Effect.DieAnimation | 修正済み | 中身は GUI（SRCRender の ScreenGUI.DieAnimation と SRCSharpFormGUI.DieAnimation）。VB は EraseUnitBitmap の do_refresh を省略している（既定は True）のに、C# は false を渡していて、消したユニットがすぐ画面に出なかった（#1498）。右クリックの判定は IsRButtonPressed にまとめてある |
| Sub ExplodeAnimation | 4688 | 192 | `SRCCore/Effect.cs` Effect.ExplodeAnimation | 修正済み | 中身は GUI。SRCRender は戦闘アニメ版の画像かを "Anime" を含むかで見分けていたので、VB と同じく \Anime\ で見分けるようにした（#1498）。画像が 1 枚も無いとき、VB は 1 回分待ってから ClearPicture するが、C# はすぐ戻る（画像が無いときだけの違いなので合わせない） |
| Sub NegateEffect | 4882 | 115 | `SRCCore/Effect.cs` Effect.NegateEffect | 一致 |  |
