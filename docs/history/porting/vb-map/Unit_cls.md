# Unit.cls

VB 版の `SRC/SRC_20121125/Unit.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Initialize | 169 | 8 | `SRCCore/Units/Unit.cs` Unit.Unit | 一致 | コンストラクターで Status="待機"・intBossRank=-1 にしている。ReDim WData(0)・adata(0) は List の初期化が代わる |
| Sub Class_Terminate | 179 | 81 | （見つからない） | SRC#で不要 | コレクションの解放だけなので、GC が代わる |
| Property Get Name | 265 | 3 | `SRCCore/Units/Unit.props.cs` Unit.Name | 一致 |  |
| Property Let Name | 269 | 5 | `SRCCore/Units/Unit.props.cs` Unit.Name | 一致 | Data は UnitName（データの名前）を持ち、参照のたびに UDList から引く |
| Property Get Nickname0 | 276 | 30 | `SRCCore/Units/Unit.props.cs` Unit.Nickname0 | 一致 | 式が自分の愛称を参照したときの再入を止める処理（#1411）は SRC# で足したもの。VB では再帰が止まらない |
| Property Get Nickname | 308 | 15 | `SRCCore/Units/Unit.props.cs` Unit.Nickname | 一致 |  |
| Property Get KanaName | 325 | 48 | `SRCCore/Units/Unit.props.cs` Unit.KanaName | 一致 | 「$(読み仮名)」（7 文字）を idx+5、「$(パイロット読み仮名)」（12 文字）を idx+10 で切るため「名)」が残る。VB の不具合だが VB のとおりにしている |
| Property Get Rank | 375 | 3 | `SRCCore/Units/Unit.props.cs` Unit.Rank | 一致 |  |
| Property Let Rank | 379 | 15 | `SRCCore/Units/Unit.props.cs` Unit.Rank | 一致 |  |
| Property Get BossRank | 396 | 3 | `SRCCore/Units/Unit.props.cs` Unit.BossRank | 一致 |  |
| Property Let BossRank | 400 | 10 | `SRCCore/Units/Unit.props.cs` Unit.BossRank | 一致 |  |
| Property Get Class | 412 | 3 | `SRCCore/Units/Unit.props.cs` Unit.Class | 一致 |  |
| Property Get Class0 | 417 | 29 | `SRCCore/Units/Unit.props.cs` Unit.Class0 | 一致 | 対になる「(」が無いとき、VB は `Loop While i > 0` で Mid$(…, 0, 1) を評価してエラー 5 になる。C# は `while (i > 1)` で止まり、元の文字列を返す（VB がエラーになる場合だけの違い） |
| Property Get Size | 449 | 8 | `SRCCore/Units/Unit.props.cs` Unit.Size | 一致 |  |
| Property Get MaxHP | 460 | 21 | `SRCCore/Units/Unit.props.cs` Unit.MaxHP | 一致 | 実数の加算は VBMath.CLng で丸めている |
| Property Get MaxEN | 483 | 21 | `SRCCore/Units/Unit.props.cs` Unit.MaxEN | 一致 | 0.5*PlanaLevel は CLng（偶数丸め） |
| Property Get HP | 506 | 3 | `SRCCore/Units/Unit.props.cs` Unit.HP | 一致 |  |
| Property Let HP | 510 | 8 | `SRCCore/Units/Unit.props.cs` Unit.HP | 一致 |  |
| Property Get EN | 520 | 3 | `SRCCore/Units/Unit.props.cs` Unit.EN | 一致 |  |
| Property Let EN | 524 | 9 | `SRCCore/Units/Unit.props.cs` Unit.EN | 一致 | VB の intEN は Integer（Long を代入する）。あふれのチェックをしない設定だが、Long から Integer への変換のあふれは実機でエラーになった（#1550 の RankUp。[説明](README.md#実機で確かめた結果)）ので、32767 を超える ＥＮ は VB ではエラーになると考えられる。C# は int（あふれない） |
| Property Get Speed0 | 536 | 3 | `SRCCore/Units/Unit.props.cs` Unit.Speed0 | 一致 |  |
| Property Get Speed | 540 | 56 | `SRCCore/Units/Unit.props.cs` Unit.Speed | 一致 |  |
| Property Get Transportation | 599 | 30 | `SRCCore/Units/Unit.props.cs` Unit.Transportation | 一致 |  |
| Function IsTransAvailable | 631 | 17 | `SRCCore/Units/Unit.action.cs` Unit.IsTransAvailable | 一致 |  |
| Property Get Adaption | 651 | 66 | `SRCCore/Units/Unit.props.cs` Unit.get_Adaption | 一致 |  |
| Property Get AdaptionMod | 719 | 79 | `SRCCore/Units/Unit.props.cs` Unit.get_AdaptionMod | 一致 | 省略できる ad_mod は C# では省略できない（呼び出し元はすべて渡している） |
| Property Get Armor | 801 | 53 | `SRCCore/Units/Unit.props.cs` Unit.get_Armor | 一致 |  |
| Property Get Mobility | 856 | 30 | `SRCCore/Units/Unit.props.cs` Unit.get_Mobility | 一致 | `(SyncLevel() - 50) \ 2` は VBMath.IntDiv |
| Property Get Bitmap | 888 | 17 | `SRCCore/Units/Unit.props.cs` Unit.get_Bitmap | 一致 |  |
| Property Get Value | 907 | 13 | `SRCCore/Units/Unit.props.cs` Unit.Value | 一致 |  |
| Property Get ExpValue | 922 | 11 | `SRCCore/Units/Unit.props.cs` Unit.ExpValue | 一致 |  |
| Function CountFeature | 938 | 3 | `SRCCore/Units/Unit.feature.cs` Unit.CountFeature | 一致 |  |
| Function Feature | 943 | 6 | `SRCCore/Units/Unit.feature.cs` Unit.Feature | 一致 | SRC# は名称ではなく FeatureData を返す（呼び出し元は .Name を参照する）。見つからないとき、VB はエラー、C# は null |
| Function FeatureName | 951 | 3 | `SRCCore/Units/Unit.feature.cs` Unit.FeatureName | 修正済み | 見つからない・番号が範囲外のとき、VB と同じく Index をそのまま返す（空文字を返す・例外にするのをやめた）。#1498 |
| Function FeatureNameInt | 955 | 101 | `SRCCore/Models/FeatureData.cs` FeatureData.FeatureName | 一致 | 見つからないときの CStr(Index) は Unit.FeatureName で扱う |
| Function FeatureName0 | 1057 | 6 | `SRCCore/Units/Unit.feature.cs` Unit.FeatureName0 | 修正済み | 見つからないとき、Index から「Lv」以降を除いた文字列を返す（空文字を返していた）。#1498 |
| Function FeatureLevel | 1065 | 17 | `SRCCore/Units/Unit.feature.cs` Unit.FeatureLevel | 修正済み | 番号が範囲外のとき、例外にせず 0 を返す。#1498 |
| Function FeatureData | 1084 | 11 | `SRCCore/Units/Unit.feature.cs` Unit.FeatureData | 修正済み | 番号が範囲外のとき、例外にせず "" を返す。#1498 |
| Function FeatureNecessarySkill | 1097 | 11 | （見つからない） | SRC#で不要 | VB でも Unit の FeatureNecessarySkill を呼ぶところが無い（呼ばれるのは Item・UnitData のもの）。コメントアウトしたコードは除いた |
| Function IsFeatureAvailable | 1110 | 11 | `SRCCore/Units/Unit.feature.cs` Unit.IsFeatureAvailable | 一致 |  |
| Function IsFeatureLevelSpecified | 1123 | 16 | `SRCCore/Units/Unit.feature.cs` Unit.IsFeatureLevelSpecified | 一致 |  |
| Function CountAllFeature | 1141 | 3 | `SRCCore/Units/Unit.feature.cs` Unit.AllFeatures | SRC#で不要 | AllFeatures.Count で参照する |
| Function AllFeature | 1146 | 6 | `SRCCore/Units/Unit.feature.cs` Unit.AllFeature | 一致 | FeatureData を返す（Feature と同じ） |
| Function AllFeatureName | 1154 | 3 | `SRCCore/Models/FeatureData.cs` FeatureData.FeatureName | SRC#で不要 | AllFeature(i).FeatureName(u) で参照する。見つからないときに CStr(Index) を返す処理は呼び出し元の側にある（下の「手がかり」） |
| Function AllFeatureLevel | 1159 | 17 | `SRCCore/Models/FeatureData.cs` FeatureData.FeatureLevel | SRC#で不要 | AllFeature(i).FeatureLevel で参照する |
| Function AllFeatureLevelSpecified | 1178 | 14 | `SRCCore/Models/FeatureData.cs` FeatureData.HasLevel | SRC#で不要 | AllFeature(i).HasLevel で参照する |
| Function AllFeatureData | 1194 | 11 | `SRCCore/Models/FeatureData.cs` FeatureData.Data | SRC#で不要 | AllFeature(i).Data で参照する |
| Function IsAllFeatureLevelSpecified | 1207 | 16 | `SRCCore/Models/FeatureData.cs` FeatureData.HasLevel | SRC#で不要 | AllFeature(i).HasLevel で参照する |
| Function IsFeatureActivated | 1225 | 19 | `SRCCore/Units/Unit.feature.cs` Unit.IsFeatureActivated | 修正済み | AllFeatures で探していたので常に真だった。colFeature に同じインスタンスがあるかで判定する。#1498 |
| Sub Update | 1250 | 2188 | `SRCCore/Units/Unit.status.cs` Unit.Update | 修正済み | #1498。Update 前半（〜2336 行、攻撃属性まで）: 修正済み。パイロット能力付加のあとでアイテムの使用可否とランクアップを調べ直し、変わっていれば TryAgain からやり直す処理を入れた。画像のファイル名が変わったら出撃中のユニットを描き直す処理と、空を飛べない「空中」のユニットを地上・水上・水中に戻す処理も入れた（いずれもコメントアウトされていた）。BitmapID の作り直しは SRC# では要らない（描画のときに画像を引く）。最大ＨＰがまだ無いとき（lngMaxHP=0、セーブから戻した直後）に比率で戻さない処理は SRC# の都合で残した。追加サポートの Pilot が無いときに飛ばす処理も残した（VB はエラー 91）。そのほか（ランクアップのループ、特殊能力の組み立て、付加２・強化２、エリアス、ボスランク、成長オプション、強化・割合強化の丸め、アイテム、Ｖ－ＵＰ（ユニット）、追加移動力、上限、ＨＰ・ＥＮの比率と偶数丸め、地形適応、耐性と重複の除去、武器データの組み立ての順、攻撃属性の照合・魔の付加・非表示の属性）は一致 Update 後半（2354〜）: 修正済み。直したのは 3 点。(1) 武器強化・武器割合強化で、固属性の武器は各指定を武器名・表示名・「固」だけで判定するようにした。「!」の指定と指定無しの扱いはほかの武器と同じで、指定無しなら固属性の武器も強化する。(2) 固属性の武器に攻撃力の最高値 99999・最低値 1 を適用しない（`UnitWeapon.SetPowerCorrection` に applyLimits を足した）。(3) 出撃中に制御不可・不安定を得たときの暴走の付加と、失ったときの解除。C# ではコメントアウトされていて動いていなかった。射程・命中率・ＣＴ率・最大弾数・弾数と使用回数の引き継ぎ（NewOrRef・AData の構築）・アビリティを並べる順序は一致。ＣＴ率強化の「物」は VB が `found = Not with_not` なので「!物」で除外されないが、VB の明らかな不具合として合わせていない |
| Sub AddFeatures | 3440 | 36 | `SRCCore/Units/Unit.feature.cs` Unit.AddFeatures | 一致 |  |
| Function IsFeatureRegistered | 3478 | 12 | `SRCCore/Units/Unit.feature.cs` Unit.IsFeatureRegistered | 一致 | キーの比較は SrcCollection が VB の Collection と同じく大文字・小文字などを区別しない |
| Function IsAllFeatureRegistered | 3492 | 12 | `SRCCore/Units/Unit.feature.cs` Unit.UpdateFeatures | 一致 | UpdateFeatures の中で colAllFeature.ContainsKey にしている。コメントアウトされたコードは説明に置き換えた |
| Sub UpdateFeatures | 3507 | 94 | `SRCCore/Units/Unit.feature.cs` Unit.UpdateFeatures | 一致 | List は写しなので、1 つずつ判定して消す順序は VB と同じ。再登録のキー「名前:位置」も同じ。fname を指定したとき colAllFeature を更新しないのも VB と同じ |
| Sub AddOtherForm | 3606 | 3 | `SRCCore/Units/Unit.otherform.cs` Unit.AddOtherForm | 一致 |  |
| Sub DeleteOtherForm | 3611 | 16 | `SRCCore/Units/Unit.otherform.cs` Unit.DeleteOtherForm | 修正済み | SrcCollection.Remove は例外を投げないので、キーで見つからないときにユニット名称で探す分岐が働いていなかった。C# は ID（文字列）だけを受け取る。VB で位置の番号を渡すのは DeleteTemporaryOtherForm だけ |
| Function CountOtherForm | 3629 | 3 | `SRCCore/Units/Unit.otherform.cs` Unit.CountOtherForm | 一致 |  |
| Function OtherForm | 3634 | 45 | `SRCCore/Units/Unit.otherform.cs` Unit.OtherForm | 一致 | C# は文字列だけを受け取る（位置の番号で引く呼び出しは OtherForms を使う）。他形態の相互登録は順序が違うが、それぞれ別の Collection に足すので結果は同じ |
| Function IsOtherFormDefined | 3681 | 12 | `SRCCore/Units/Unit.otherform.cs` Unit.IsOtherFormDefined | 一致 |  |
| Sub DeleteTemporaryOtherForm | 3695 | 80 | `SRCCore/Units/Unit.otherform.cs` Unit.DeleteTemporaryOtherForm | 修正済み | データを FeatureData(i) で引いていた。VB と同じく FeatureData(fname)（同じ名前の特殊能力が複数あれば最初のもの）で引くようにした |
| Sub AddPilot | 3780 | 3 | `SRCCore/Units/Unit.pilot.cs` Unit.AddPilot | 一致 |  |
| Sub DeletePilot | 3785 | 3 | `SRCCore/Units/Unit.pilot.cs` Unit.DeletePilot | 一致 | C# はパイロットを受け取って削除する（VB は位置の番号）。同じパイロットが 2 回登録されることは無いので結果は同じ |
| Sub ReplacePilot | 3790 | 26 | `SRCCore/Units/Unit.pilot.cs` Unit.ReplacePilot | 一致 | 前のパイロットが乗っていないとき、VB は Collection のエラーで止まり、C# は何もしない（呼び出し元で判定済み） |
| Function CountPilot | 3818 | 3 | `SRCCore/Units/Unit.pilot.cs` Unit.CountPilot | 一致 |  |
| Function Pilot | 3823 | 3 | `SRCCore/Units/Unit.pilot.cs` Unit.Pilot | 一致 | C# はキーだけで引く。位置の番号で引く呼び出しは Pilots を使う |
| Function MainPilot | 3829 | 195 | `SRCCore/Units/Unit.pilot.cs` Unit.MainPilot | 修正済み | 作成済みの暴走時パイロット: 先にユニットを設定し、Update・UpdateSupportMod を呼ばない（VB はユニットを設定した直後に「このユニットのものでなければ」と判定するので呼ばれない）。登録済みの追加パイロット: 余分な Update をやめた。他形態の追加パイロット: 陣営とユニットを設定するだけにした（気力・レベル・経験値を合わせない。前は IsAdditionalPilot が偽のときにユニットを設定していなかった）。データの無い暴走時パイロットで先頭のパイロットを返す処理・CountPilot()>0 のガード・再入を止める仕組み（#1411）は SRC# の利便性として残した |
| Sub AddSupport | 4027 | 3 | `SRCCore/Units/Unit.pilot.cs` Unit.AddSupport | 一致 | VB と同じく名前をキーにする |
| Sub DeleteSupport | 4032 | 3 | `SRCCore/Units/Unit.pilot.cs` Unit.DeleteSupport | 一致 | C# はパイロットを受け取る（DeletePilot と同じ） |
| Sub ReplaceSupport | 4037 | 26 | `SRCCore/Units/Unit.pilot.cs` Unit.ReplaceSupport | 修正済み | VB と同じく、入れ替えのあとは ID をキーにして登録し直すようにした（前は AddSupport で名前をキーにしていた） |
| Function CountSupport | 4065 | 3 | `SRCCore/Units/Unit.pilot.cs` Unit.CountSupport | 一致 |  |
| Function Support | 4070 | 3 | `SRCCore/Units/Unit.pilot.cs` Unit.Support | 一致 | C# はキーだけで引く。位置の番号で引く呼び出しは Supports を使う |
| Function AdditionalSupport | 4075 | 77 | `SRCCore/Units/Unit.pilot.cs` Unit.AdditionalSupport | 一致 |  |
| Function IsSkillAvailable | 4154 | 41 | `SRCCore/Units/Unit.pilot.cs` Unit.IsSkillAvailable | 一致 | Pilot(2..) は SubPilots で回している |
| Function SkillLevel | 4197 | 33 | `SRCCore/Units/Unit.pilot.cs` Unit.SkillLevel | 一致 | VB の AliasType(1) は Elements.First().strAliasType。Ｓ防御・切り払いの ref_mode は "1" で、VB と同じ |
| Function MaxSkillLevel | 4232 | 69 | `SRCCore/Units/Unit.pilot.cs` Unit.MaxSkillLevel | 一致 |  |
| Function AuraLevel | 4303 | 26 | `SRCCore/Units/Unit.pilot.cs` Unit.AuraLevel | 一致 | Pilot(2) は Pilots[1] |
| Function PsychicLevel | 4331 | 27 | `SRCCore/Units/Unit.pilot.cs` Unit.PsychicLevel | 一致 |  |
| Function SyncLevel | 4360 | 14 | `SRCCore/Units/Unit.pilot.cs` Unit.SyncLevel | 一致 |  |
| Function PlanaLevel | 4376 | 14 | `SRCCore/Units/Unit.pilot.cs` Unit.PlanaLevel | 一致 | MinDbl の代わりに Math.Min を使っているが、結果は同じ |
| Function SkillName0 | 4392 | 41 | `SRCCore/Units/Unit.pilot.cs` Unit.SkillName0 | 一致 |  |
| Function MaxAction | 4438 | 42 | `SRCCore/Units/Unit.action.cs` Unit.MaxAction | 一致 |  |
| Property Get Action | 4482 | 7 | `SRCCore/Units/Unit.props.cs` Unit.Action | 修正済み | MaxAction が 0 のときは、UsedAction が負（再動など）でも 0 にした |
| Sub UseAction | 4491 | 23 | `SRCCore/Units/Unit.action.cs` Unit.UseAction | 修正済み | 上限に MaxAction() を使っていたのを、VB と同じく特殊状態とＥＮを見ない最大行動数（パイロットが 0 人なら 1）にした |
| Function SpecialPowerInEffect | 4519 | 34 | `SRCCore/Units/Unit.sp.cs` Unit.SpecialPowerInEffect | 一致 |  |
| Function IsSpecialPowerInEffect | 4555 | 13 | `SRCCore/Units/Unit.sp.cs` Unit.IsSpecialPowerInEffect | 修正済み | 名前の完全一致で探していたのを、Collection のキーと同じ比較（ContainsKey）にした |
| Function IsUnderSpecialPowerEffect | 4570 | 16 | `SRCCore/Units/Unit.sp.cs` Unit.IsUnderSpecialPowerEffect | 一致 |  |
| Function CountSpecialPower | 4588 | 3 | `SRCCore/Units/Unit.sp.cs` Unit.CountSpecialPower | 一致 |  |
| Function SpecialPower | 4593 | 13 | `SRCCore/Units/Unit.sp.cs` Unit.SpecialPower | 一致 | VB で Variant を渡す呼び出しはどれも番号なので、C# が int だけでも足りる |
| Function SpecialPowerEffectLevel | 4608 | 22 | `SRCCore/Units/Unit.sp.cs` Unit.SpecialPowerEffectLevel | 修正済み | VB の Exit For に合わせ、1 つのスペシャルパワーでは最初に該当した効果だけを見るようにした |
| Function SpecialPowerData | 4632 | 10 | `SRCCore/Units/Unit.sp.cs` Unit.SpecialPowerData | 一致 | RemoveSpecialPowerInEffect の中では、VB の SpecialPowerData(sd.Name) を番号 i で呼んでいるが、指すものは同じ |
| Sub MakeSpecialPowerInEffect | 4644 | 15 | `SRCCore/Units/Unit.sp.cs` Unit.MakeSpecialPowerInEffect | 修正済み | IsSpecialPowerInEffect を直したので、表記だけが違う名前を足しても、キーが重複して例外になることは無くなった |
| Sub RemoveSpecialPowerInEffect | 4661 | 51 | `SRCCore/Units/Unit.sp.cs` Unit.RemoveSpecialPowerInEffect | 一致 |  |
| Sub RemoveSpecialPowerInEffect2 | 4714 | 6 | `SRCCore/Units/Unit.sp.cs` Unit.RemoveSpecialPowerInEffect2 | 修正済み | 範囲外の番号で例外になっていたのを、VB の On Error と同じく無視するようにした |
| Sub RemoveAllSpecialPowerInEffect | 4722 | 9 | `SRCCore/Units/Unit.sp.cs` Unit.RemoveAllSpecialPowerInEffect | 一致 |  |
| Sub CopySpecialPowerInEffect | 4733 | 9 | `SRCCore/Units/Unit.sp.cs` Unit.CopySpecialPowerInEffect | 一致 |  |
| Sub AddCondition | 4747 | 32 | `SRCCore/Units/Unit.condition.cs` Unit.AddCondition | 一致 | キーの表記だけが違う名前を Add すると例外になるのは、VB（エラー 457）と同じ |
| Sub DeleteCondition | 4781 | 12 | `SRCCore/Units/Unit.condition.cs` Unit.DeleteCondition | 一致 | C# の引数は名前（string）だけ。VB の番号で呼んでいる箇所（Unit.cls 1382・15299・19187・27247）がどう移されたかは、それぞれの手続きの担当で見る |
| Sub DeleteCondition0 | 4794 | 3 | `SRCCore/Units/Unit.condition.cs` Unit.DeleteCondition0 | 一致 | VB の `DeleteCondition0 1`（番号）の呼び出しも、上と同じくそれぞれの手続きの担当で見る |
| Function CountCondition | 4799 | 3 | `SRCCore/Units/Unit.condition.cs` Unit.CountCondition | 一致 |  |
| Function Condition | 4804 | 3 | `SRCCore/Units/Unit.condition.cs` Unit.Condition | 一致 | C# は名前ではなく Condition オブジェクトを返し、呼び出し側が .Name を読む。キーが無いとき、VB はエラー、C# は null |
| Function ConditionLifetime | 4809 | 8 | `SRCCore/Units/Unit.condition.cs` Unit.ConditionLifetime | 一致 |  |
| Function IsConditionSatisfied | 4819 | 11 | `SRCCore/Units/Unit.condition.cs` Unit.IsConditionSatisfied | 一致 | SrcCollection のキーの比較は VB と同じ |
| Function ConditionLevel | 4832 | 8 | `SRCCore/Units/Unit.condition.cs` Unit.ConditionLevel | 一致 |  |
| Sub SetConditionLevel | 4842 | 5 | （見つからない） | SRC#で不要 | 呼び出し元（Unit.attackcheck.cs のダミー・盾）が IsConditionSatisfied で確かめてから `Condition(name).Level` に代入している。C# ではコメントアウトされている |
| Function ConditionData | 4849 | 8 | `SRCCore/Units/Unit.condition.cs` Unit.ConditionData | 一致 |  |
| Sub UpdateCondition | 4859 | 59 | `SRCCore/Units/Unit.condition.cs` Unit.UpdateCondition | 一致 | VB は For Each の中で Remove している。C# は CloneList で全部を回す。VB5 の Collection は今の要素を消しても次へ進む（すべて消え、次の要素を飛ばさない）。VB5 の実機で確かめた（#1550 の VB5.ForEach.特殊状態.*） |
| Function Weapon | 4923 | 3 | `SRCCore/Units/Unit.weapon.cs` Unit.Weapon | 一致 | 範囲外は null を返す（VB は実行時エラー。SRC# の利便性） |
| Function CountWeapon | 4928 | 3 | `SRCCore/Units/Unit.weapon.cs` Unit.CountWeapon | 一致 |  |
| Function WeaponNickname | 4933 | 9 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.WeaponNickname | 一致 |  |
| Function WeaponPower | 4945 | 152 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.WeaponPower | 修正済み | バトルコンフィグの「攻撃補正」の前に BCVariable.AtkUnit を設定していなかった |
| Function WeaponAdaption | 5099 | 201 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.WeaponAdaption | 一致 |  |
| Function WeaponMaxRange | 5302 | 31 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.WeaponMaxRange<br>`SRCCore/Units/UnitWeapon.cs` UnitWeapon.SetMaxRangeCorrection | 修正済み | 補正後の最低値を 1 に（WeaponMinRange() だった）。Ｍ・武/突/接・有の武器でもサ属性の射程延長を残す |
| Function WeaponENConsumption | 5335 | 80 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.WeaponENConsumption | 一致 |  |
| Function WeaponPrecision | 5417 | 3 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.WeaponPrecision | 一致 |  |
| Function WeaponCritical | 5422 | 3 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.WeaponCritical | 一致 |  |
| Function WeaponClass | 5427 | 3 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.WeaponClass | 一致 |  |
| Function IsWeaponClassifiedAs | 5432 | 65 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.IsWeaponClassifiedAs | 修正済み | 格闘系・射撃系・移動後攻撃可の判定で、補正後ではなく武器データの MaxRange を見る |
| Function WeaponLevel | 5499 | 44 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.WeaponLevel | 修正済み | 数値として読めないレベルでエラーメッセージを出す。メッセージの先頭をユニット名に |
| Function IsWeaponLevelSpecified | 5545 | 6 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.IsWeaponLevelSpecified | 一致 |  |
| Function IsNormalWeapon | 5553 | 17 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.IsNormalWeapon | 一致 |  |
| Function CountWeaponEffect | 5572 | 31 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.CountWeaponEffect | 一致 |  |
| Function IsSpellWeapon | 5605 | 22 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.IsSpellWeapon | 一致 |  |
| Function IsFeatWeapon | 5629 | 22 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.IsFeatWeapon | 一致 |  |
| Function IsWeaponAvailable | 5654 | 322 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.IsWeaponAvailable | 修正済み | 「○属性使用不能」の判定を VB の Len>6・Right$・Left$ に合わせた。変形技の判定の順序を VB に合わせた |
| Function IsWeaponMastered | 5978 | 3 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.IsWeaponMastered | 一致 |  |
| Function IsWeaponEnabled | 5983 | 3 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.IsWeaponEnabled | 一致 |  |
| Function IsWeaponUseful | 5988 | 81 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.IsWeaponUseful | 一致 |  |
| Function IsTargetWithinRange | 6072 | 70 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.IsTargetWithinRange | 修正済み | ステルスのレベルを偶数丸めしてから距離と比べる（VB の lv は Integer） |
| Function IsTargetReachable | 6144 | 70 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.IsTargetReachable | 一致 | VB の `Abs(.y - i)`（本来は j）も同じにしている |
| Function HitProbability | 6218 | 481 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.HitProbability | 修正済み | 地形適応は 1 つ目の要素（名称）を比べず、能力ごとに -10。未知のサイズの補正を 0 倍に。ECM（マイナスのレベル・範囲）は #612・90fd4dd0 の意図した違い（SRCとの互換性）。「地形適応命中率修正」のＪは VB では wclass が空で効かない不具合なので再現しない |
| Function Damage | 6703 | 785 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.Damage | 修正済み | 対属性のレベルが実数のとき、VB の Mod と同じく偶数丸めの整数で割る |
| Function CriticalProbability | 7490 | 340 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.CriticalProbability | 修正済み | 対属性のレベルが実数のとき、VB の Mod と同じく偶数丸めの整数で割る（Damage と同じ） |
| Function ExpDamage | 7832 | 546 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.ExpDamage | 一致 | 末尾の抹殺の判定は VB では `InStr(w, "殺")`（w は武器の番号）で働かない。VB の不具合なので再現しない（C# は武器属性を見る） |
| Function Absorb | 8383 | 52 | `SRCCore/Units/Unit.attribute.cs` Unit.Absorb | 修正済み | Exit Do を return にした（戻り値は以前と同じ） |
| Function Immune | 8437 | 51 | `SRCCore/Units/Unit.attribute.cs` Unit.Immune | 修正済み | Absorb と同じ |
| Function Resist | 8490 | 51 | `SRCCore/Units/Unit.attribute.cs` Unit.Resist | 修正済み | Absorb と同じ |
| Function Weakness | 8543 | 37 | `SRCCore/Units/Unit.attribute.cs` Unit.Weakness | 修正済み | Absorb と同じ |
| Function Effective | 8582 | 37 | `SRCCore/Units/Unit.attribute.cs` Unit.Effective | 修正済み | Absorb と同じ |
| Function SpecialEffectImmune | 8621 | 26 | `SRCCore/Units/Unit.attribute.cs` Unit.SpecialEffectImmune | 一致 |  |
| Function IsAttributeClassified | 8650 | 65 | `SRCCore/Units/Unit.attribute.cs` Unit.IsAttributeClassified | 修正済み | Exit Do が switch の break でループを抜けていなかった。該当した属性の後ろの「!」と、「魔」で魔法武器のときの判定が VB と違っていた |
| Sub Attack | 8724 | 2147 | `SRCCore/Units/Unit.attack.cs` Unit.Attack | 修正済み | 浄化を探す相手から追加サポートを外した。レベルの無い連属性は攻撃回数 1 回（VB は実行時エラー。SRC# の利便性）。消費したアイテムは VB と同じく武器の番号で決める（#1508 で直した。VB の数え方の不具合は再現しない）。BitmapID・新ＧＵＩの ClearUnitStatus は SRC# では不要 |
| Function CheckDodgeFeature | 10873 | 450 | `SRCCore/Units/Unit.attackcheck.cs` Unit.CheckDodgeFeature | 一致 |  |
| Function CheckParryFeature | 11326 | 1054 | `SRCCore/Units/Unit.attackcheck.cs` Unit.CheckParryFeature | 修正済み | 迎撃用の武器の自動選択と永続武器の条件を、残弾ではなく武器データの弾数（VB の Weapon(i).Bullet）で見る。迎撃用の武器が無くても Dice(16) を振る（VB の And は短絡しない） |
| Function CheckDummyFeature | 12382 | 114 | `SRCCore/Units/Unit.attackcheck.cs` Unit.CheckDummyFeature | 一致 |  |
| Function CheckShieldFeature | 12498 | 105 | `SRCCore/Units/Unit.attackcheck.cs` Unit.CheckShieldFeature | 一致 |  |
| Function CheckDefenseFeature | 12605 | 1728 | `SRCCore/Units/Unit.attackcheck.cs` Unit.CheckDefenseFeature | 修正済み | 融合の判定で、格闘系（武・突・接）の武器でも VB と同じく Dice(16) を振る（VB の And は短絡しない） |
| Sub CheckAutoAttack | 14335 | 256 | `SRCCore/Units/Unit.attackcheck.cs` Unit.CheckAutoAttack | 一致 |  |
| Sub CheckAdditionalAttack | 14594 | 175 | `SRCCore/Units/Unit.attackcheck.cs` Unit.CheckAdditionalAttack | 一致 |  |
| Function IsSameCategory | 14772 | 19 | `SRCCore/Units/Unit.attackcheck.cs` Unit.IsSameCategory | 一致 | VB は Private、C# はテストのため public |
| Function CauseEffect | 14793 | 983 | `SRCCore/Units/Unit.attackcheck.cs` Unit.CauseEffect | 修正済み | 衰・滅のレベルを VB の CInt と同じく偶数丸めしてから選ぶ。除は前から順に消す。ラーニングは VB と同じくグローバル変数を直接書き換える |
| Function CheckBlowAttack | 15778 | 313 | `SRCCore/Units/Unit.attackcheck.cs` Unit.CheckBlowAttack | 一致 | 摩擦の 3 つ目の条件は VB では `t.Area = Class`（With td の中でドットが無く、攻撃側のユニットクラスを見る誤り）。C# は地形のクラスと比べる。VB の不具合なので再現しない |
| Function CheckDrawAttack | 16093 | 191 | `SRCCore/Units/Unit.attackcheck.cs` Unit.CheckDrawAttack | 修正済み | 隣接の判定は VB では tx・ty を設定する前の (0,0) と比べるので働かない。VB と同じにし、Ｍ投で攻撃側が隣接していても目標地点へ引き寄せる |
| Function CheckTeleportAwayAttack | 16286 | 85 | `SRCCore/Units/Unit.attackcheck.cs` Unit.CheckTeleportAwayAttack | 一致 |  |
| Function CheckMetamorphAttack | 16373 | 103 | `SRCCore/Units/Unit.attackcheck.cs` Unit.CheckMetamorphAttack | 一致 |  |
| Sub MapAttack | 16479 | 1040 | `SRCCore/Units/Unit.attackmap.cs` Unit.MapAttack | 修正済み | 標的の並べ替え（VB と同じ安定でない選択ソート）、かばったユニットへの置き換え、資金の用語名、「敵破壊」の解除の条件、特殊能力のレベルアップ表示、合体技パートナーの自・失・変・霊・プ、使い捨てアイテムの削除（番号で決める。VB の数え方の不具合は再現しない）、イベントで標的の情報を番号で引く |
| Sub UseWeapon | 17521 | 87 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.UseWeapon | 一致 |  |
| Function Bullet | 17610 | 3 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.Bullet | 一致 |  |
| Function MaxBullet | 17615 | 3 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.MaxBullet | 一致 |  |
| Sub SetBullet | 17620 | 9 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.SetBullet | 一致 |  |
| Sub SyncBullet | 17631 | 126 | `SRCCore/Units/Unit.recover.cs` Unit.SyncBullet | 修正済み | 他の形態の同名の武器・アビリティを、VB と同じく直前に合わせたものの次から探して順に対応させる |
| Function Ability | 17763 | 3 | `SRCCore/Units/Unit.ability.cs` Unit.Ability | 一致 |  |
| Function CountAbility | 17768 | 3 | `SRCCore/Units/Unit.ability.cs` Unit.CountAbility | 一致 |  |
| Function AbilityNickname | 17773 | 9 | `SRCCore/Units/UnitAbility.cs` UnitAbility.AbilityNickname | 一致 |  |
| Function AbilityMinRange | 17784 | 6 | `SRCCore/Units/UnitAbility.cs` UnitAbility.AbilityMinRange | 一致 |  |
| Function AbilityMaxRange | 17792 | 3 | `SRCCore/Units/UnitAbility.cs` UnitAbility.AbilityMaxRange | 一致 |  |
| Function AbilityENConsumption | 17797 | 82 | `SRCCore/Units/UnitAbility.cs` UnitAbility.AbilityENConsumption | 修正済み | レベル指定の無いＥＮ消費減少を FeatureLevel（1）で数える（生の Level の -1000 を使っていた） |
| Function IsAbilityClassifiedAs | 17881 | 7 | `SRCCore/Units/UnitAbility.cs` UnitAbility.IsAbilityClassifiedAs | 一致 |  |
| Function AbilityLevel | 17890 | 44 | `SRCCore/Units/UnitAbility.cs` UnitAbility.AbilityLevel | 修正済み | 数値として読めないレベル指定でエラーメッセージを出す（VB の CDbl）。メッセージの頭をユニット名に。検索は VB と同じく InStr |
| Function IsSpellAbility | 17936 | 22 | `SRCCore/Units/UnitAbility.cs` UnitAbility.IsSpellAbility | 一致 |  |
| Function IsFeatAbility | 17960 | 22 | `SRCCore/Units/UnitAbility.cs` UnitAbility.IsFeatAbility | 一致 |  |
| Function IsAbilityAvailable | 17985 | 340 | `SRCCore/Units/UnitAbility.cs` UnitAbility.IsAbilityAvailable | 一致 | VB の不具合は再現しない: 禁の判定 `IsAbilityClassifiedAs(a, "禁") > 0`（True=-1 で効かない）、共有の充填中の確認でアビリティを 2 回走査する書き誤り |
| Function IsAbilityMastered | 18327 | 3 | `SRCCore/Units/UnitAbility.cs` UnitAbility.IsAbilityMastered | 一致 |  |
| Function IsAbilityEnabled | 18332 | 3 | `SRCCore/Units/UnitAbility.cs` UnitAbility.IsAbilityEnabled | 一致 |  |
| Function IsAbilityUseful | 18337 | 52 | `SRCCore/Units/UnitAbility.cs` UnitAbility.IsAbilityUseful | 一致 |  |
| Function IsAbilityEffective | 18391 | 330 | `SRCCore/Units/UnitAbility.cs` UnitAbility.IsAbilityEffective | 一致 | 能力コピーの `.IsConditionSatisfied("混乱") > 0`（常に偽）は VB の不具合なので再現しない |
| Function IsAbilityApplicable | 18723 | 93 | `SRCCore/Units/UnitAbility.cs` UnitAbility.IsAbilityApplicable | 修正済み | 対属性のレベルを VB の Mod と同じく偶数丸めの整数にしてから割る。VB の `For i = 2 To t.FeatureData("修理不可")` の不具合は再現しない |
| Function IsTargetWithinAbilityRange | 18818 | 32 | `SRCCore/Units/UnitAbility.cs` UnitAbility.IsTargetWithinAbilityRange | 一致 |  |
| Function IsTargetReachableForAbility | 18852 | 21 | `SRCCore/Units/UnitAbility.cs` UnitAbility.IsTargetReachableForAbility | 一致 |  |
| Function Stock | 18875 | 3 | `SRCCore/Units/UnitAbility.cs` UnitAbility.Stock | 一致 |  |
| Function MaxStock | 18880 | 7 | `SRCCore/Units/UnitAbility.cs` UnitAbility.MaxStock | 一致 |  |
| Sub SetStock | 18889 | 9 | `SRCCore/Units/UnitAbility.cs` UnitAbility.SetStock | 一致 |  |
| Function ExecuteAbility | 18904 | 1737 | `SRCCore/Units/Unit.ability.cs` Unit.ExecuteAbility | 修正済み | 変身と召喚の Exit For（後ろの効果を適用していた）、メッセージ欄を開く判定（MainFormVisible を見ていた）、ＳＰ回復の表示はメインパイロットだけ、合体技パートナーの自・失・変・霊・プ（ほぼ未実装だった）、変形技の照合名。使い切ったアイテムは VB と同じく番号で決める（#1508 で直した）。強化のエリアスの `.AliasType(i)`（j の書き誤り）は再現しない |
| Sub ExecuteMapAbility | 20643 | 627 | `SRCCore/Units/Unit.ability.cs` Unit.ExecuteMapAbility | 修正済み | 対象の並べ替え（VB と同じ選択ソート）、パートナーは「合」のときだけ探す、同名のアビリティのパートナーの後の break でループごと抜けていた、パーツ分離の LIndex を 2 に、自爆・HP 消費の Die の後に「破壊」イベント、VB に無い終了アニメと画像の復元を除いた、変形技の名前は LIndex で取る。使い切ったアイテムは VB と同じく番号で決める（#1508 で直した） |
| Sub UseAbility | 21272 | 87 | `SRCCore/Units/UnitAbility.cs` UnitAbility.UseAbility | 修正済み | 「斉」を VB の `MinLng(MaxStock(i) * Stock(a) \ MaxStock(a), Stock(i))` に。VB に無い「永」の判定（武器の属性）を除いた |
| Function MaxItemNum | 21364 | 15 | `SRCCore/Units/Unit.item.cs` Unit.MaxItemNum | 一致 |  |
| Function CountItem | 21381 | 3 | `SRCCore/Units/Unit.item.cs` Unit.CountItem | 一致 |  |
| Function Item | 21386 | 19 | `SRCCore/Units/Unit.item.cs` Unit.Item | 一致 |  |
| Sub AddItem | 21407 | 265 | `SRCCore/Units/Unit.item.cs` Unit.AddItem | 修正済み | ハードポイント付きのものしか無いとき、何も外さない（VB の For Each は最後まで回ると変数が Nothing になる） |
| Sub AddItem0 | 21673 | 4 | `SRCCore/Units/Unit.item.cs` Unit.AddItem0 | 一致 |  |
| Sub DeleteItem | 21679 | 256 | `SRCCore/Units/Unit.item.cs` Unit.DeleteItem | 修正済み | 外すアイテムの武器・アビリティを外してから Update する（同じアイテムを 2 つ装備しているとき残弾数が引き継がれていた） |
| Function ItemSlotSize | 21937 | 31 | `SRCCore/Units/Unit.item.cs` Unit.ItemSlotSize | 一致 | FeatureLevel を足すたびに Integer へ丸める処理（VBMath.CLng）も同じ |
| Function IsEquiped | 21970 | 11 | `SRCCore/Units/Unit.item.cs` Unit.IsEquiped | 一致 |  |
| Function WeaponProficiency | 21983 | 11 | `SRCCore/Units/Unit.item.cs` Unit.WeaponProficiency | 修正済み | 武器クラスが無いときに null を返していたのを、VB の String の既定値と同じ "" にした |
| Function ArmorProficiency | 21996 | 11 | `SRCCore/Units/Unit.item.cs` Unit.ArmorProficiency | 修正済み | 同上 |
| Function IsAbleToEquip | 22009 | 259 | `SRCCore/Units/Unit.item.cs` Unit.IsAbleToEquip | 一致 |  |
| Sub PilotMessage | 22273 | 453 | `SRCCore/Units/Unit.message.cs` Unit.PilotMessage | 修正済み | 話すパイロットを VB と同じ番号 i（Dice(CountPilot+CountSupport)、追加サポートは候補に入れない）で選ぶようにした。スペシャルパワーを使ったサブパイロット・サポートにメッセージが無いと止まらなくなっていたのを直した。VB が結果を使わずに呼ぶ SelectMessage（乱数を 1 回分進める）を移した。武器が見つからないときは例外にせず射撃として扱う |
| Sub PlayDialog | 22728 | 167 | `SRCCore/Units/Unit.message.cs` Unit.PlayDialog | 修正済み | 「-」での打ち切りを、VB と同じく置換の前に判定する |
| Sub PlayMessage | 22897 | 154 | `SRCCore/Units/Unit.message.cs` Unit.PlayMessage | 一致 | 置換は SubstituteMessageVariables にまとめてある。結果は VB と同じ |
| Function IsMessageDefined | 23053 | 100 | `SRCCore/Units/Unit.message.cs` Unit.IsMessageDefined | 修正済み | pnames(4) = Pilot(1).MessageType を、追加パイロットが無くても調べるようにした |
| Sub SysMessage | 23155 | 418 | `SRCCore/Units/Unit.message.cs` Unit.SysMessage | 修正済み | メッセージウィンドウが開いていなければ開く。add_msg を "." を挟んで連結する。変数の置換を VB に合わせた（置換は SubstituteSysMessageVariables に分けた）。「専用」の扱いは VB の不具合として直さない（5 に記載） |
| Function IsSysMessageDefined | 23575 | 252 | `SRCCore/Units/Unit.message.cs` Unit.IsSysMessageDefined | 一致 | 検索は FindSysMessage・SearchMessageInList にまとめてある。愛称から余分な部分を除く処理の IndexOf・EndsWith を Ordinal にした（動作は同じ） |
| Function SpecialEffectData | 23832 | 87 | `SRCCore/Units/Unit.se.cs` Unit.SpecialEffectData | 一致 |  |
| Sub SpecialEffect | 23921 | 325 | `SRCCore/Units/Unit.se.cs` Unit.SpecialEffect | 修正済み | #1498。前後の空白の除去を TrimString と同じくタブ・空白・全角空白だけにした（C# の Trim() は改行も除いていた）。glass.wav は VB でも w=0 のとき strWeaponClass(0)="" なので何も再生しない（C# と同じ）。エラーのとき C# は例外のメッセージを出す（VB は EventErrorMessage か空。SRC# の利便性として残す） |
| Function IsSpecialEffectDefined | 24248 | 10 | `SRCCore/Units/Unit.se.cs` Unit.IsSpecialEffectDefined | 一致 |  |
| Function AnimationData | 24265 | 182 | `SRCCore/Units/Unit.anime.cs` Unit.AnimationData | 修正済み | #1498。見つからないとき・戦闘アニメが無効なときに null を返していたのを、VB と同じく空文字列にした |
| Sub PlayAnimation | 24449 | 357 | `SRCCore/Units/Unit.anime.cs` Unit.PlayAnimation | 修正済み | #1498。引数の無い指定でも「,」を付けて Call していたのを VB の `For j = 2 To ListLength` に合わせた（一括指定の「一括指定」が Args(2) になっていた）。戦闘アニメが無効なときに AnimationData の null で例外になっていたのを直した。TrimString の範囲も SpecialEffect と同じに直した。エラーのときのメッセージの扱いは SpecialEffect と同じ |
| Function IsAnimationDefined | 24811 | 16 | `SRCCore/Units/Unit.anime.cs` Unit.IsAnimationDefined | 一致 |  |
| Sub StandBy | 24831 | 264 | `SRCCore/Units/Unit.map.cs` Unit.StandBy | 修正済み | #1498。登場時アニメの条件 `BitmapID > 0` を `!IsBitmapHidden()` にした（以前は画像のファイル名が空でないかで判定していた）。コマごとの `picMain(0).Refresh` に当たる UpdateScreen を足した。BitmapID の作成は SRC# では要らない |
| Sub Move | 25097 | 143 | `SRCCore/Units/Unit.map.cs` Unit.Move | 修正済み | #1498。合体の判定を VB のとおり「移動先のユニットの合体能力（3 要素）の相手の CurrentForm が自分」にした（以前は自分の合体能力と合体制限・行動不能などで判定していた）。最後の Update で、空を飛べない空中のユニットが戻る前提は変えていない |
| Sub Jump | 25242 | 119 | `SRCCore/Units/Unit.map.cs` Unit.Jump | 修正済み | #1498。探す前に x,y を移動先にしていたため、空き位置が無いと移動先にいるユニットの上に置いていた。VB のとおり元の位置に戻す |
| Sub Escape | 25363 | 60 | `SRCCore/Units/Unit.map.cs` Unit.Escape | 一致 | 載せているユニットを降ろす処理（VB は For Each の中で Remove する）は、C# ではすべて降ろす。VB も 2 機ともが降りる（次の要素を飛ばさない）ことを VB5 の実機で確かめた（#1550 の VB5.ForEach.母艦.*） |
| Sub Land | 25425 | 136 | `SRCCore/Units/Unit.map.cs` Unit.Land | 修正済み | #1498。気力の低下を `SubPilots`（1 人目を除く）から、メインパイロット以外の全パイロットにした。メインが追加パイロットのとき 1 人目も下がる。is_event のときの BitmapID の処理は SRC# では要らない |
| Sub Transform | 25563 | 343 | `SRCCore/Units/Unit.map.cs` Unit.Transform | 一致 | VB の BitmapID の作り直し（MakeUnitBitmap）は、SRC# では描画のときに画像を引くので要らない。使っていない変数 `counter` を消した |
| Sub Combine | 25908 | 427 | `SRCCore/Units/Unit.map.cs` Unit.Combine | 修正済み | 合体形態を指定しないときの相手の探し方を VB に合わせた（#1498）。まずユニット名が一致するものを探し、無ければ IsEqual（他形態の名前）で探す。データが 3 つでない「合体」は、例外にせず飛ばす（以前は CombineFeature が例外を出していた）。変形した形態から合体すると、合体後のユニットが装備したアイテムの Item.Unit は空になる（AddItem で合体後のユニットに付けた後、「デフォルトの形態に変形させておく」の Transform が AddItem0 で元の形態に付け直し、「１番目のユニットのアイテムを外す」の DeleteItem が空にする）。VB も同じなので変えない。セーブデータを読むと装備しているユニットが持ち主になる（#1579） |
| Sub Split | 26337 | 290 | `SRCCore/Units/Unit.map.cs` Unit.Split | 修正済み | １番機へのアイテムの受け渡しを AddItem0 から AddItem に直した（装備個所が足りなければ外す）。行動回数を受け継ぐ先を、１番機ではなく並びの１番目（VB の `i = 1`）で決めるように直した（#1498）。VB の「格納」の分岐は `.CountOtherForm` で `.UnitOnBoard(j)` を回す不具合があり、たいてい 91 のエラーになる。SRC# は LoadSameUnit で母艦に正しく載せるので、そのままにした。分離先が無いときのメッセージは VB では `LIndex(buf, i - 1)`（一つ前の名前）を出すが、SRC# は正しい名前を出す（VB の不具合なので直していない） |
| Function GetExp | 26632 | 234 | `SRCCore/Units/Unit.action2.cs` Unit.GetExp | 一致 | 追加サポートが Nothing のとき、VB は 91 のエラーになり、SRC# は飛ばす |
| Sub ChangeParty | 26868 | 41 | `SRCCore/Units/Unit.action2.cs` Unit.ChangeParty | 一致 | BitmapID の作り直しは SRC# では要らない |
| Sub IncreaseMorale | 26912 | 43 | `SRCCore/Units/Unit.action2.cs` Unit.IncreaseMorale | 一致 |  |
| Sub Die | 26957 | 68 | `SRCCore/Units/Unit.action2.cs` Unit.Die | 一致 | SRC# では PreviousSummonerId も記録する（VB には無い。#627 で、祈りで復活したときに召喚主に戻すため）。動作が変わるのはスペシャルパワーの側 |
| Sub SuicidalExplosion | 27027 | 179 | `SRCCore/Units/Unit.action2.cs` Unit.SuicidalExplosion | 修正済み | 自分が自爆したあとにパーツ分離したとき、VB は HP を全快にするだけ。SRC# は `UsedAction = MaxAction` で行動済みにもしていたので、その処理を消した（#1498）。周りのユニットがパーツ分離したときの行動済みは VB にもあるので、そのままにした |
| Sub FullRecover | 27211 | 63 | `SRCCore/Units/Unit.recover.cs` Unit.FullRecover | 修正済み | 他形態の回復を `OtherForms` から `cf.OtherForms` に変えた（VB は With CurrentForm の中で .OtherForm(i) を回復する）。VB に無い冒頭の Update を外した。手がかりの DeleteCondition の番号指定は、C# のように名前で消しても同じ（状態の名前はキーと同じで重複しない。消す状態は名前が「付加」で終わらないので、DeleteCondition の中で Update が呼ばれず、状態の並びも変わらない） |
| Sub FullSupply | 27276 | 27 | `SRCCore/Units/Unit.recover.cs` Unit.FullSupply | 一致 | 処理の順は違う（他形態の EN を先に回復し、弾数は BulletSupply・StockSupply に分けている）が、結果は同じ。SetBulletFull・SetStockFull は dblBullet・dblStock を 1 にするのと同じ |
| Sub BulletSupply | 27305 | 16 | `SRCCore/Units/Unit.recover.cs` Unit.BulletSupply | 一致 |  |
| Sub RecoverHP | 27323 | 15 | `SRCCore/Units/Unit.recover.cs` Unit.RecoverHP | 一致 | 実数から Long への丸めは CLng |
| Sub RecoverEN | 27340 | 6 | `SRCCore/Units/Unit.recover.cs` Unit.RecoverEN | 一致 |  |
| Sub Rest | 27348 | 1008 | `SRCCore/Units/Unit.recover.cs` Unit.Rest | 一致 | 活動限界による退却を C# は UpdateCondition だけで行う。VB は Rest と UpdateCondition の両方で行うので、退却のメッセージと「破壊」イベントが 2 回出る。VB の明らかな不具合なので合わせない。地形効果の判定の順（弱点・有効・吸収・耐性・無効以外）、BoxType による地形の選び方、霊力・ＨＰ・ＥＮの `\` の丸め、MainPilot と Pilot(2..) の扱いは一致 |
| Sub CheckAutoHyperMode | 28358 | 178 | `SRCCore/Units/Unit.recover.cs` Unit.CheckAutoHyperMode | 一致 | CInt は Math.Round（偶数への丸め）で同じ。VB は `With UDList.Item(uname)` の中で `.` を付けずに IsFeatureAvailable("追加パイロット") を呼ぶので、自分の特殊能力を見ている。C# の CreateAdditionalPilotForMessage も同じ |
| Function CheckAutoNormalMode | 28538 | 139 | `SRCCore/Units/Unit.recover.cs` Unit.CheckAutoNormalMode | 一致 |  |
| Sub Reset | 28680 | 33 | `SRCCore/Units/Unit.recover.cs` Unit.Reset | 一致 | VB の DeleteCondition0 1 の繰り返しは、C# の ClearCondition と同じ |
| Property Get Party0 | 28718 | 3 | `SRCCore/Units/Unit.action.cs` Unit.Party0 | 一致 |  |
| Property Get Party | 28722 | 29 | `SRCCore/Units/Unit.action.cs` Unit.Party | 一致 |  |
| Property Let Party | 28752 | 3 | `SRCCore/Units/Unit.action.cs` Unit.Party | 一致 |  |
| Function IsEnemy | 28757 | 106 | `SRCCore/Units/Unit.party.cs` Unit.IsEnemy | 一致 |  |
| Function IsAlly | 28865 | 39 | `SRCCore/Units/Unit.party.cs` Unit.IsAlly | 一致 |  |
| Property Get Mode | 28906 | 38 | `SRCCore/Units/Unit.action.cs` Unit.Mode | 一致 |  |
| Property Let Mode | 28945 | 3 | `SRCCore/Units/Unit.action.cs` Unit.Mode | 一致 |  |
| Sub LoadUnit | 28953 | 3 | `SRCCore/Units/Unit.onboard.cs` Unit.LoadUnit | 一致 |  |
| Sub UnloadUnit | 28958 | 15 | `SRCCore/Units/Unit.onboard.cs` Unit.UnloadUnit | 一致 | キーで消せないときに名前で探す（VB の ErrorHandler 以降）のも同じ。VB の Index は Variant で番号も受けるが、C# は文字列だけを受け、C# の呼び出し元はすべて ID を渡している |
| Function CountUnitOnBoard | 28975 | 3 | `SRCCore/Units/Unit.onboard.cs` Unit.CountUnitOnBoard | 一致 |  |
| Function UnitOnBoard | 28980 | 15 | `SRCCore/Units/Unit.onboard.cs` Unit.UnitOnBoard | 一致 |  |
| Sub AddServant | 29000 | 8 | `SRCCore/Units/Unit.summon.cs` Unit.AddServant | 一致 |  |
| Sub DeleteServant | 29010 | 15 | `SRCCore/Units/Unit.summon.cs` Unit.DeleteServant | 一致 |  |
| Function CountServant | 29027 | 3 | `SRCCore/Units/Unit.summon.cs` Unit.CountServant | 一致 |  |
| Function Servant | 29032 | 15 | `SRCCore/Units/Unit.summon.cs` Unit.Servant | 一致 |  |
| Sub DismissServant | 29049 | 29 | `SRCCore/Units/Unit.summon.cs` Unit.DismissServant | 一致 | VB は Servant(1) を処理して DeleteServant 1 を繰り返す。C# はリストの写しを回して最後に Clear する。Escape・Split が呼んだユニットの colServant を変えないので、結果は同じ |
| Sub AddSlave | 29083 | 3 | `SRCCore/Units/Unit.summon.cs` Unit.AddSlave | 一致 |  |
| Sub DeleteSlave | 29088 | 15 | `SRCCore/Units/Unit.summon.cs` Unit.DeleteSlave | 一致 |  |
| Function CountSlave | 29105 | 3 | `SRCCore/Units/Unit.summon.cs` Unit.CountSlave | 一致 |  |
| Function Slave | 29110 | 15 | `SRCCore/Units/Unit.summon.cs` Unit.Slave | 一致 |  |
| Sub DismissSlave | 29127 | 22 | `SRCCore/Units/Unit.summon.cs` Unit.DismissSlave | 一致 |  |
| Sub Dump | 29153 | 84 | `SRCCore/Units/Unit.cs` Unit.LinkData | 修正済み | SRC# は JSON で保存。特殊状態・効果中の SP・アビリティの残り回数（番号順）・追加サポートの ID を保存していなかったので足した（このブランチ） |
| Sub Restore | 29239 | 163 | `SRCCore/Units/Unit.cs` Unit.Restore | 修正済み | 特殊状態・SP・追加サポート（Unit と IsAdditionalSupport も設定）を戻す。サポートは VB の AddSupport と同じく名前をキーにする（ID をキーにしていたので Support(名前) で引けなかった）。VB はレベルを StrToLng で整数に丸めるが、clevel を Double で宣言しており誤りとみなして丸めない |
| Sub RestoreLinkInfo | 29404 | 107 | `SRCCore/Units/Unit.cs` Unit.RestoreLinkInfo | 修正済み | 新しく作った。Update の後で SetStock を番号で戻す。SRC.RestoreData（`SRCCore/SRC.save.cs`）から呼ぶ。つながり（他形態・パイロット・アイテムなど）は JSON の ID から Unit.Restore で戻す。アイテムは VB の AddItem0 と同じく持ち主もこのユニットにする（#1579）。武器の残弾数は WData と一緒に保存し、Update で名前で照合する（VB は番号。データを変えない限り同じ） |
| Sub RestoreParameter | 29513 | 95 | `SRCCore/Units/Unit.cs` Unit.RestoreLinkInfo | 一致 | HP・EN は JSON の lngHP・intEN で戻し、RestoreLinkInfo の Update を通る。読み込んだ後の値は同じ |
| Function IsOperational | 29613 | 17 | `SRCCore/Units/Unit.lookup.cs` Unit.IsOperational | 一致 |  |
| Function IsEqual | 29632 | 17 | `SRCCore/Units/Unit.lookup.cs` Unit.IsEqual | 一致 |  |
| Function CurrentForm | 29651 | 14 | `SRCCore/Units/Unit.otherform.cs` Unit.CurrentForm | 一致 |  |
| Function IsHero | 29667 | 9 | `SRCCore/Units/Unit.lookup.cs` Unit.IsHero | 一致 |  |
| Function LookForAttackHelp | 29678 | 73 | `SRCCore/Units/Unit.lookup.cs` Unit.LookForAttackHelp | 一致 |  |
| Function LookForGuardHelp | 29753 | 135 | `SRCCore/Units/Unit.lookup.cs` Unit.LookForGuardHelp | 一致 |  |
| Function MaxSupportAttack | 29890 | 5 | `SRCCore/Units/Unit.lookup.cs` Unit.MaxSupportAttack | 一致 |  |
| Function MaxSupportGuard | 29897 | 5 | `SRCCore/Units/Unit.lookup.cs` Unit.MaxSupportGuard | 一致 |  |
| Function MaxSyncAttack | 29904 | 3 | `SRCCore/Units/Unit.lookup.cs` Unit.MaxSyncAttack | 一致 |  |
| Function MaxCounterAttack | 29909 | 17 | `SRCCore/Units/Unit.lookup.cs` Unit.MaxCounterAttack | 一致 |  |
| Function LookForSupportAttack | 29928 | 151 | `SRCCore/Units/Unit.lookup.cs` Unit.LookForSupportAttack | 一致 |  |
| Function LookForSupportGuard | 30082 | 217 | `SRCCore/Units/Unit.lookup.cs` Unit.LookForSupportGuard | 一致 |  |
| Function LookForSupport | 30301 | 109 | `SRCCore/Units/Unit.lookup.cs` Unit.LookForSupport | 一致 |  |
| Sub CombinationPartner | 30412 | 406 | `SRCCore/Units/Unit.lookup.cs` Unit.CombinationPartner | 修正済み | 合体技の種類（ctype）を引数に足した。武装なら同名の武器、アビリティなら同名のアビリティだけを調べる（以前は種類に関わらず武器、次にアビリティの順に探していた）。弾数・回数の有無はデータの値（Weapon(k).Bullet・Ability(k).Stock）で見る。出撃していないときに実際のユニットを返すのは以前からの C# の拡張（VB は要素数だけ）。パートナーの並びを VB と同じく ListTail(…, 2)・LLength・LIndex で読む（#1534 で直した） |
| Function IsCombinationAttackAvailable | 30820 | 56 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.IsCombinationAttackAvailable | 修正済み | 射程１で隣の敵を基点にするときも check_formation を渡す。調べる順を VB と同じ左・右・上・下にした（SelectedPartners に残る組が変わる） |
| Function IsCombinationAbilityAvailable | 30878 | 56 | `SRCCore/Units/UnitAbility.cs` UnitAbility.IsCombinationAbilityAvailable | 修正済み | 射程１は隣の味方（IsAlly）を基点にする（以前は IsEnemy）。check_formation を渡し、左・右・上・下の順に調べる。VB は下を調べる条件が `y > MapHeight` で常に偽だが、誤記とみなして下も調べる |
| Function IsAbleToEnter | 30936 | 99 | `SRCCore/Units/Unit.lookup.cs` Unit.IsAbleToEnter | 一致 |  |
| Function IsAvailable | 31037 | 45 | `SRCCore/Units/Unit.lookup.cs` Unit.IsAvailable | 一致 |  |
| Function IsNecessarySkillSatisfied | 31084 | 50 | `SRCCore/Units/Unit.lookup.cs` Unit.IsNecessarySkillSatisfied | 一致 |  |
| Function IsNecessarySkillSatisfied2 | 31135 | 570 | `SRCCore/Units/Unit.lookup.cs` Unit.IsNecessarySkillSatisfied2 | 修正済み | 「男性」「女性」は PilotNum > 1 なら Pilot(1) から調べる（以前は SubPilots で 1 人目を飛ばしていた） |
| Function IsDisabled | 31707 | 18 | `SRCCore/Units/Unit.lookup.cs` Unit.IsDisabled | 一致 |  |
| Function IsDefense | 31728 | 7 | `SRCCore/Units/Unit.lookup.cs` Unit.IsDefense | 一致 |  |
