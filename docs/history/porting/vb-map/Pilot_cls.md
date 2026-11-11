# Pilot.cls

VB 版の `SRC/SRC_20121125/Pilot.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Terminate | 99 | 13 | （なし） | SRC#で不要 | Data・Unit・colSkill を解放するだけ。C# では GC が回収する |
| Property Get Name | 114 | 3 | `SRCCore/Pilots/Pilot.props.cs` Pilot.Name | 一致 |  |
| Property Let Name | 118 | 4 | `SRCCore/Pilots/Pilot.props.cs` Pilot.Name | 一致 | PDList.Item を代入してから Update |
| Property Get Nickname0 | 124 | 3 | `SRCCore/Pilots/Pilot.props.cs` Pilot.Nickname0 | 一致 |  |
| Property Get Nickname | 128 | 69 | `SRCCore/Pilots/Pilot.props.cs` Pilot.get_Nickname | 修正済み | dont_call_unit_nickname のとき、C# は正規表現（大文字・小文字を区別しない）で消していた。VB と同じく `ReplaceString` で "Nickname()" と "nickname()" だけを消すようにした。自分の愛称への再入を止める処理（#1411）は SRC# の安全策なので残した |
| Property Get KanaName | 199 | 70 | `SRCCore/Pilots/Pilot.props.cs` Pilot.KanaName | 一致 | "$(読み仮名)" は 7 文字なのに、VB は `Mid$(…, idx + 5)` で後ろを取るので「名)」が残る。C# も同じ動作（VB の不具合だが合わせたまま） |
| Property Get Sex | 271 | 10 | `SRCCore/Pilots/Pilot.props.cs` Pilot.Sex | 一致 |  |
| Property Get Class | 283 | 3 | `SRCCore/Pilots/Pilot.props.cs` Pilot.Class | 一致 |  |
| Property Get ExpValue | 288 | 3 | `SRCCore/Pilots/Pilot.props.cs` Pilot.ExpValue | 一致 |  |
| Property Get Personality | 293 | 25 | `SRCCore/Pilots/Pilot.props.cs` Pilot.Personality | 一致 | `.Pilot(1)` は Pilots.First() |
| Property Get Bitmap | 320 | 55 | `SRCCore/Pilots/Pilot.props.cs` Pilot.get_Bitmap | 一致 |  |
| Property Get BGM | 377 | 3 | `SRCCore/Pilots/Pilot.props.cs` Pilot.BGM | 一致 |  |
| Property Get MessageType | 382 | 17 | `SRCCore/Pilots/Pilot.props.cs` Pilot.MessageType | 一致 |  |
| Property Get Defense | 402 | 14 | `SRCCore/Pilots/Pilot.props.cs` Pilot.Defense | 一致 | 丸め（CLng・IntDiv・Int）も VB と同じ |
| Sub Update | 419 | 417 | `SRCCore/Pilots/Pilot.status.cs` Pilot.Update | 修正済み | 下の 4 を参照（同名の能力の置き換え位置、封印した能力の番号の扱い）。能力値の計算・ユニットとサポートによる修正・地形適応の変更・CountPilot=0 のときの Exit Sub は一致。追加サポートが Nothing のときに C# が null を確かめるのは SRC# の安全策 |
| Sub UpdateSupportMod | 838 | 315 | `SRCCore/Pilots/Pilot.status.cs` Pilot.UpdateSupportMod | 一致 | 広域サポート・指揮・信頼補正の分岐と丸めはすべて一致 |
| Property Get Level | 1158 | 3 | `SRCCore/Pilots/Pilot.level.cs` Pilot.Level | 一致 |  |
| Property Let Level | 1162 | 9 | `SRCCore/Pilots/Pilot.level.cs` Pilot.Level | 一致 |  |
| Property Get Exp | 1173 | 3 | `SRCCore/Pilots/Pilot.level.cs` Pilot.Exp | 一致 |  |
| Property Let Exp | 1177 | 39 | `SRCCore/Pilots/Pilot.level.cs` Pilot.Exp | 一致 | Mod と `\` は C# の % と / と同じく 0 の方向へ切り捨て |
| Property Get Morale | 1219 | 3 | `SRCCore/Pilots/Pilot.level.cs` Pilot.Morale | 一致 |  |
| Property Let Morale | 1223 | 3 | `SRCCore/Pilots/Pilot.level.cs` Pilot.Morale | 一致 |  |
| Sub SetMorale | 1227 | 15 | `SRCCore/Pilots/Pilot.level.cs` Pilot.SetMorale | 一致 |  |
| Property Get MaxMorale | 1243 | 8 | `SRCCore/Pilots/Pilot.level.cs` Pilot.MaxMorale | 一致 |  |
| Property Get MinMorale | 1252 | 8 | `SRCCore/Pilots/Pilot.level.cs` Pilot.MinMorale | 一致 |  |
| Property Get MaxSP | 1265 | 55 | `SRCCore/Pilots/Pilot.sp.cs` Pilot.MaxSP | 一致 | 追加パイロットかどうかは IsActiveAdditionalPilot で判定 |
| Property Get SP | 1322 | 36 | `SRCCore/Pilots/Pilot.sp.cs` Pilot.SP | 一致 |  |
| Property Let SP | 1359 | 51 | `SRCCore/Pilots/Pilot.sp.cs` Pilot.SP | 一致 |  |
| Property Get SP0 | 1411 | 3 | `SRCCore/Pilots/Pilot.sp.cs` Pilot.SP0 | 一致 |  |
| Property Let SP0 | 1415 | 3 | `SRCCore/Pilots/Pilot.sp.cs` Pilot.SP0 | 一致 |  |
| Function MaxPlana | 1423 | 34 | `SRCCore/Pilots/Pilot.plana.cs` Pilot.MaxPlana | 一致 |  |
| Property Get Plana | 1459 | 36 | `SRCCore/Pilots/Pilot.plana.cs` Pilot.Plana | 一致 |  |
| Property Let Plana | 1496 | 51 | `SRCCore/Pilots/Pilot.plana.cs` Pilot.Plana | 一致 | VB は霊力の判定に `.MaxSP > 0`（第 1 パイロットの最大ＳＰ）を使っている（VB の誤記と思われる）。C# も同じ判定 |
| Property Get Plana0 | 1548 | 3 | `SRCCore/Pilots/Pilot.plana.cs` Pilot.Plana0 | 一致 |  |
| Property Let Plana0 | 1552 | 3 | `SRCCore/Pilots/Pilot.plana.cs` Pilot.Plana0 | 一致 |  |
| Function CountSkill | 1560 | 3 | `SRCCore/Pilots/Pilot.skill.cs` Pilot.CountSkill | 一致 |  |
| Function Skill | 1565 | 6 | `SRCCore/Pilots/Pilot.skill.cs` Pilot.Skill | 一致 | 文字列で引いて見つからないとき、VB はエラー、C# は "" を返す（SRC# の利便性）。範囲外の番号はどちらもエラー |
| Function IsSkillAvailable | 1573 | 49 | `SRCCore/Pilots/Pilot.skill.cs` Pilot.IsSkillAvailable | 一致 |  |
| Function IsSkillAvailable2 | 1625 | 11 | （無し） | SRC#で不要 | VB でもどこからも呼ばれていない。コメントアウトされたコードを消した |
| Function SkillLevel | 1640 | 73 | `SRCCore/Pilots/Pilot.skill.cs` Pilot.SkillLevel | 修正済み | 番号で引いたとき IsNumeric(Index) を True として扱っていなかった（ハンターなどに付加のレベルが入っていた）。範囲外の番号で例外になっていたのを 0 にした。見つからない数値の文字列は付加を見ない |
| Function IsSkillLevelSpecified | 1715 | 56 | `SRCCore/Pilots/Pilot.skill.cs` Pilot.IsSkillLevelSpecified | 修正済み | 見つからない数値の文字列は False にした。範囲外の番号で例外にならないようにした |
| Function SkillData | 1773 | 62 | `SRCCore/Pilots/Pilot.skill.cs` Pilot.SkillData | 修正済み | SkillLevel と同じく番号を数値として扱う。見つからないとき null を返していたのを "" にした。範囲外の番号で例外にならないようにした |
| Function SkillName | 1837 | 272 | `SRCCore/Pilots/Pilot.skill.cs` Pilot.SkillName | 修正済み | 番号で引いたＳＰ消費減少の分岐（付加の名称にせず、括弧とレベル指定を除いて終わる）が無かった。見つからない数値の文字列は "" を返す。範囲外の番号で例外にならないようにした |
| Function SkillName0 | 2111 | 207 | `SRCCore/Pilots/Pilot.skill.cs` Pilot.SkillName0 | 修正済み | 見つからない数値の文字列は "" を返す。範囲外の番号で例外にならないようにした。エリアスの検索（ALDList.RefName）は VB と同じ |
| Function SkillNameForNS | 2322 | 137 | `SRCCore/Pilots/Pilot.skill.cs` Pilot.SkillNameForNS | 一致 |  |
| Function SkillType | 2461 | 77 | `SRCCore/Pilots/Pilot.skill.cs` Pilot.SkillType | 修正済み | エリアスのとき ReplaceTypeName を使っていた。VB と同じく AliasType(1)（最初の要素の種類）を返すようにした。空の名前のとき null を返していたのを "" にした |
| Property Get CountSpecialPower | 2543 | 19 | `SRCCore/Pilots/Pilot.sp.cs` Pilot.CountSpecialPower | 一致 | ForSP() に相当する処理は VB と同じ |
| Property Get SpecialPower | 2564 | 19 | `SRCCore/Pilots/Pilot.sp.cs` Pilot.get_SpecialPower | 一致 |  |
| Function IsSpecialPowerAvailable | 2585 | 20 | `SRCCore/Pilots/Pilot.sp.cs` Pilot.IsSpecialPowerAvailable | 一致 |  |
| Function IsSpecialPowerUseful | 2607 | 3 | `SRCCore/Pilots/Pilot.sp.cs` Pilot.IsSpecialPowerUseful | 一致 |  |
| Function SpecialPowerCost | 2612 | 66 | `SRCCore/Pilots/Pilot.sp.cs` Pilot.SpecialPowerCost | 一致 | 0.8・1.2 倍は CLng（偶数丸め）、`\` は VBMath.IntDiv で、VB と同じ |
| Sub UseSpecialPower | 2680 | 51 | `SRCCore/Pilots/Pilot.sp.cs` Pilot.UseSpecialPower | 一致 |  |
| Sub Ride | 2736 | 65 | `SRCCore/Pilots/Pilot.unit.cs` Pilot.Ride | 修正済み | VB は IsSupport の前に `Set Unit = u` をする。C# は後で設定していたので、「(愛称専用)」「(性別専用)」の判定に搭乗先のユニットの「パイロット愛称」「性別」が効いていなかった。MaxHP・MaxEN が 0 のときの割合を 100 にするのは SRC# で足した処理（VB は 0 で割る） |
| Sub GetOff | 2803 | 46 | `SRCCore/Pilots/Pilot.unit.cs` Pilot.GetOff | 一致 |  |
| Function IsSupport | 2851 | 81 | `SRCCore/Pilots/Pilot.unit.cs` Pilot.IsSupport | 一致 |  |
| Function IsAbleToRide | 2934 | 68 | `SRCCore/Pilots/Pilot.unit.cs` Pilot.IsAbleToRide | 一致 | 必要技能・不必要技能を見る順は違うが、結果は同じ |
| Sub Dump | 3007 | 11 | `SRCCore/Pilots/Pilot.cs` Pilot（JSON に書き出す項目） | SRC#で不要 | JSON で保存する。違い: SP・Plana は内部の値（proSP・proPlana）を保存する（VB はプロパティの値。RestoreParameter で吸収する）。VB に無い IsAdditionalPilot・IsAdditionalSupport も保存する |
| Sub Restore | 3020 | 36 | `SRCCore/Pilots/Pilot.cs` Pilot.Restore | 修正済み | VB は IsAdditionalPilot を保存しないので、読み込んだ直後は False になる。それに合わせて False に戻すようにした。(ザコ)・(汎用) の追加パイロットをロードのたびに作り直さないため |
| Sub RestoreLinkInfo | 3058 | 19 | `SRCCore/Pilots/Pilot.cs` Pilot.Unit | SRC#で不要 | Unit は UnitId から UList.Item で引くので、`Set Unit = UList.Item(sbuf)` と同じになる |
| Sub RestoreParameter | 3079 | 23 | `SRCCore/Pilots/Pilot.cs` Pilot.RestoreParameter | 修正済み | #1498。メソッドを足した（SP・Morale・Plana をプロパティの値で設定し直し、最大値・最小値で切り詰める）。SRC.save.cs の RestoreData から、特殊能力を設定した後・UpdateSupportMod の前に呼ぶ |
| Sub FullRecover | 3107 | 15 | `SRCCore/Pilots/Pilot.other.cs` Pilot.FullRecover | 一致 |  |
| Function SynchroRate | 3124 | 18 | `SRCCore/Pilots/Pilot.other.cs` Pilot.SynchroRate | 一致 |  |
| Function CommandRange | 3144 | 19 | `SRCCore/Pilots/Pilot.other.cs` Pilot.CommandRange | 一致 | 6.5 のような小数は VB と同じく Case Else（5）になる |
| Function TacticalTechnique0 | 3165 | 3 | `SRCCore/Pilots/Pilot.other.cs` Pilot.TacticalTechnique0 | 一致 |  |
| Function TacticalTechnique | 3169 | 15 | `SRCCore/Pilots/Pilot.other.cs` Pilot.TacticalTechnique | 一致 |  |
| Function Relation | 3186 | 3 | `SRCCore/Pilots/Pilot.other.cs` Pilot.Relation | 一致 |  |
| Function HasMana | 3191 | 13 | `SRCCore/Pilots/Pilot.other.cs` Pilot.HasMana | 一致 |  |
