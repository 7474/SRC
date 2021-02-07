// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Units;
using System;
using System.Drawing;

namespace SRCCore.Maps
{
    // マップデータに関する各種処理を行うモジュール
    public class Map
    {
        protected SRC SRC { get; }
        private IGUI GUI => SRC.GUI;
        private IGUIMap GUIMap => SRC.GUIMap;

        public Map(SRC src)
        {
            SRC = src;
        }

        // 管理可能な地形データの総数
        public const int MAX_TERRAIN_DATA_NUM = 2000;

        // ADD START 240a
        // レイヤー無しの固定値
        public const int NO_LAYER_NUM = 10000;
        // ADD  END  240a

        // マップファイル名
        public string MapFileName;
        // マップの横サイズ
        public int MapWidth;
        // マップの縦サイズ
        public int MapHeight;

        // マップの描画モード
        public string MapDrawMode;
        // フィルタ色
        public int MapDrawFilterColor;
        // フィルタの透過度
        public double MapDrawFilterTransPercent;
        // フィルタやSepiaコマンドなどでユニットの色を変更するか
        public bool MapDrawIsMapOnly;

        // マップに画像の書き込みがなされたか
        public bool IsMapDirty;

        // マップデータを記録する配列
        // MapData(*,*,0)は地形の種類
        // MapData(*,*,1)はビットマップの番号
        // ADD START 240a
        // 2～3はマップ上層レイヤーデータ
        // MapData(*,*,2)は地形の種類。未設定はNO_LAYER_NUM
        // MapData(*,*,3)はビットマップの番号。未設定はNO_LAYER_NUM
        // MapData(*,*,4)はマスのデータタイプ。1:下層 2:上層 3:上層データのみ 4:上層見た目のみ
        // ADD  END  240a
        public MapCell[][] MapData;

        public MapImageFileType[] MapImageFileTypeData;

        // マップ上に存在するユニットを記録する配列
        public Unit[] MapDataForUnit;

        // マップ上でターゲットを選択する際のマスク情報
        public bool[] MaskData;

        // 現在地点からその地点まで移動するのに必要な移動力の配列
        public int[] TotalMoveCost;

        // 各地点がＺＯＣの影響下にあるかどうか
        public int[] PointInZOC;

        // 地形情報テーブルを初期化
        public void InitMap()
        {
            SetMapSize(GUI.MainWidth, GUI.MainHeight);
            // TODO Impl
            //int i, j;
            //var loopTo = MapWidth;
            //for (i = 1; i <= loopTo; i++)
            //{
            //    var loopTo1 = MapHeight;
            //    for (j = 1; j <= loopTo1; j++)
            //    {
            //        // MOD START 240a
            //        // MapData(i, j, 0) = 0
            //        // MapData(i, j, 1) = 0
            //        MapData[i, j, MapDataIndex.TerrainType] = 0;
            //        MapData[i, j, MapDataIndex.BitmapNo] = 0;
            //        MapData[i, j, MapDataIndex.LayerType] = 0;
            //        MapData[i, j, MapDataIndex.LayerBitmapNo] = 0;
            //        MapData[i, j, MapDataIndex.BoxType] = 0;
            //        // ADD  END  240a
            //    }
            //}
        }

        // (X,Y)地点の命中修正
        public int TerrainEffectForHit(int X, int Y)
        {
            throw new NotImplementedException();
            //    int TerrainEffectForHitRet = default;
            //    // MOD START 240a
            //    // TerrainEffectForHit = TDList.HitMod(MapData(X, Y, 0))
            //    switch (MapData[X, Y, MapDataIndex.BoxType])
            //    {
            //        case BoxTypes.Under:
            //        case BoxTypes.UpperBmpOnly:
            //            {
            //                // 上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
            //                TerrainEffectForHitRet = SRC.TDList.HitMod(MapData[X, Y, MapDataIndex.TerrainType]);
            //                break;
            //            }

            //        default:
            //            {
            //                // 上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
            //                TerrainEffectForHitRet = SRC.TDList.HitMod(MapData[X, Y, MapDataIndex.LayerType]);
            //                break;
            //            }
            //    }

            //    return TerrainEffectForHitRet;
            //    // MOD  END  240a
        }

        // (X,Y)地点のダメージ修正
        public int TerrainEffectForDamage(int X, int Y)
        {
            throw new NotImplementedException();
            //    int TerrainEffectForDamageRet = default;
            //    // MOD START 240a
            //    // TerrainEffectForDamage = TDList.DamageMod(MapData(X, Y, 0))
            //    switch (MapData[X, Y, MapDataIndex.BoxType])
            //    {
            //        case BoxTypes.Under:
            //        case BoxTypes.UpperBmpOnly:
            //            {
            //                // 上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
            //                TerrainEffectForDamageRet = SRC.TDList.DamageMod(MapData[X, Y, MapDataIndex.TerrainType]);
            //                break;
            //            }

            //        default:
            //            {
            //                // 上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
            //                TerrainEffectForDamageRet = SRC.TDList.DamageMod(MapData[X, Y, MapDataIndex.LayerType]);
            //                break;
            //            }
            //    }

            //    return TerrainEffectForDamageRet;
            //    // MOD  END  240a
        }

        // (X,Y)地点のＨＰ回復率
        public int TerrainEffectForHPRecover(int X, int Y)
        {
            throw new NotImplementedException();
            //int TerrainEffectForHPRecoverRet = default;
            //// MOD START 240a
            //// TerrainEffectForHPRecover = 10 * TDList.FeatureLevel(MapData(X, Y, 0), "ＨＰ回復")
            //switch (MapData[X, Y, MapDataIndex.BoxType])
            //{
            //    case BoxTypes.Under:
            //    case BoxTypes.UpperBmpOnly:
            //        {
            //            // 上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
            //            string argftype = "ＨＰ回復";
            //            TerrainEffectForHPRecoverRet = (10d * SRC.TDList.FeatureLevel(MapData[X, Y, MapDataIndex.TerrainType], ref argftype));
            //            break;
            //        }

            //    default:
            //        {
            //            // 上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
            //            string argftype1 = "ＨＰ回復";
            //            TerrainEffectForHPRecoverRet = (10d * SRC.TDList.FeatureLevel(MapData[X, Y, MapDataIndex.LayerType], ref argftype1));
            //            break;
            //        }
            //}

            //return TerrainEffectForHPRecoverRet;
            //// MOD  END  240a
        }

        // (X,Y)地点のＥＮ回復率
        public int TerrainEffectForENRecover(int X, int Y)
        {
            throw new NotImplementedException();
            //int TerrainEffectForENRecoverRet = default;
            //// MOD START 240a
            //// TerrainEffectForENRecover = 10 * TDList.FeatureLevel(MapData(X, Y, 0), "ＥＮ回復")
            //switch (MapData[X, Y, MapDataIndex.BoxType])
            //{
            //    case BoxTypes.Under:
            //    case BoxTypes.UpperBmpOnly:
            //        {
            //            // 上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
            //            string argftype = "ＥＮ回復";
            //            TerrainEffectForENRecoverRet = (10d * SRC.TDList.FeatureLevel(MapData[X, Y, MapDataIndex.TerrainType], ref argftype));
            //            break;
            //        }

            //    default:
            //        {
            //            // 上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
            //            string argftype1 = "ＥＮ回復";
            //            TerrainEffectForENRecoverRet = (10d * SRC.TDList.FeatureLevel(MapData[X, Y, MapDataIndex.LayerType], ref argftype1));
            //            break;
            //        }
            //}

            //return TerrainEffectForENRecoverRet;
            //// MOD  END  240a
        }

        // (X,Y)地点の地形名称
        public string TerrainName(int X, int Y)
        {
            throw new NotImplementedException();
            //string TerrainNameRet = default;
            //// MOD START 240a
            //// TerrainName = TDList.Name(MapData(X, Y, 0))
            //switch (MapData[X, Y, MapDataIndex.BoxType])
            //{
            //    case BoxTypes.Under:
            //    case BoxTypes.UpperBmpOnly:
            //        {
            //            // 上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
            //            TerrainNameRet = SRC.TDList.Name(MapData[X, Y, MapDataIndex.TerrainType]);
            //            break;
            //        }

            //    default:
            //        {
            //            // 上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
            //            TerrainNameRet = SRC.TDList.Name(MapData[X, Y, MapDataIndex.LayerType]);
            //            break;
            //        }
            //}

            //return TerrainNameRet;
            //// MOD  END  240a
        }

        // (X,Y)地点の地形クラス
        public string TerrainClass(int X, int Y)
        {
            throw new NotImplementedException();
            //string TerrainClassRet = default;
            //// MOD START 240a
            //// TerrainClass = TDList.Class(MapData(X, Y, 0))
            //switch (MapData[X, Y, MapDataIndex.BoxType])
            //{
            //    case BoxTypes.Under:
            //    case BoxTypes.UpperBmpOnly:
            //        {
            //            // 上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
            //            TerrainClassRet = SRC.TDList.Class_Renamed(MapData[X, Y, MapDataIndex.TerrainType]);
            //            break;
            //        }

            //    default:
            //        {
            //            // 上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
            //            TerrainClassRet = SRC.TDList.Class_Renamed(MapData[X, Y, MapDataIndex.LayerType]);
            //            break;
            //        }
            //}

            //return TerrainClassRet;
            //// MOD  END  240a
        }

        // (X,Y)地点の移動コスト
        public int TerrainMoveCost(int X, int Y)
        {
            throw new NotImplementedException();
            //int TerrainMoveCostRet = default;
            //// MOD START 240a
            //// TerrainMoveCost = TDList.MoveCost(MapData(X, Y, 0))
            //switch (MapData[X, Y, MapDataIndex.BoxType])
            //{
            //    case BoxTypes.Under:
            //    case BoxTypes.UpperBmpOnly:
            //        {
            //            // 上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
            //            TerrainMoveCostRet = SRC.TDList.MoveCost(MapData[X, Y, MapDataIndex.TerrainType]);
            //            break;
            //        }

            //    default:
            //        {
            //            // 上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
            //            TerrainMoveCostRet = SRC.TDList.MoveCost(MapData[X, Y, MapDataIndex.LayerType]);
            //            break;
            //        }
            //}

            //return TerrainMoveCostRet;
            //// MOD  END  240a
        }

        // (X,Y)地点に障害物があるか (吹き飛ばし時に衝突するか)
        public bool TerrainHasObstacle(int X, int Y)
        {
            throw new NotImplementedException();
            //bool TerrainHasObstacleRet = default;
            //// MOD START 240a
            //// TerrainHasObstacle = TDList.IsFeatureAvailable(MapData(X, Y, 0), "衝突")
            //switch (MapData[X, Y, MapDataIndex.BoxType])
            //{
            //    case BoxTypes.Under:
            //    case BoxTypes.UpperBmpOnly:
            //        {
            //            // 上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
            //            string argftype = "衝突";
            //            TerrainHasObstacleRet = SRC.TDList.IsFeatureAvailable(MapData[X, Y, MapDataIndex.TerrainType], ref argftype);
            //            break;
            //        }

            //    default:
            //        {
            //            // 上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
            //            string argftype1 = "衝突";
            //            TerrainHasObstacleRet = SRC.TDList.IsFeatureAvailable(MapData[X, Y, MapDataIndex.LayerType], ref argftype1);
            //            break;
            //        }
            //}

            //return TerrainHasObstacleRet;
            //// MOD  END  240a
        }

        // ADD START 240a
        // (X,Y)地点が移動停止か
        public bool TerrainHasMoveStop(int X, int Y)
        {
            throw new NotImplementedException();
            //bool TerrainHasMoveStopRet = default;
            //switch (MapData[X, Y, MapDataIndex.BoxType])
            //{
            //    case BoxTypes.Under:
            //    case BoxTypes.UpperBmpOnly:
            //        {
            //            // 上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
            //            string argftype = "移動停止";
            //            TerrainHasMoveStopRet = SRC.TDList.IsFeatureAvailable(MapData[X, Y, MapDataIndex.TerrainType], ref argftype);
            //            break;
            //        }

            //    default:
            //        {
            //            // 上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
            //            string argftype1 = "移動停止";
            //            TerrainHasMoveStopRet = SRC.TDList.IsFeatureAvailable(MapData[X, Y, MapDataIndex.LayerType], ref argftype1);
            //            break;
            //        }
            //}

            //return TerrainHasMoveStopRet;
        }

        // (X,Y)地点が進入禁止か
        public bool TerrainDoNotEnter(int X, int Y)
        {
            throw new NotImplementedException();
            //bool ret;
            //switch (MapData[X, Y, MapDataIndex.BoxType])
            //{
            //    case BoxTypes.Under:
            //    case BoxTypes.UpperBmpOnly:
            //        {
            //            // 上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
            //            string argftype = "進入禁止";
            //            ret = SRC.TDList.IsFeatureAvailable(MapData[X, Y, MapDataIndex.TerrainType], ref argftype);
            //            if (!ret)
            //            {
            //                // 互換性維持のため残している
            //                string argftype1 = "侵入禁止";
            //                ret = SRC.TDList.IsFeatureAvailable(MapData[X, Y, MapDataIndex.TerrainType], ref argftype1);
            //            }

            //            break;
            //        }

            //    default:
            //        {
            //            // 上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
            //            string argftype2 = "進入禁止";
            //            ret = SRC.TDList.IsFeatureAvailable(MapData[X, Y, MapDataIndex.LayerType], ref argftype2);
            //            if (!ret)
            //            {
            //                // 互換性維持のため残している
            //                string argftype3 = "侵入禁止";
            //                ret = SRC.TDList.IsFeatureAvailable(MapData[X, Y, MapDataIndex.LayerType], ref argftype3);
            //            }

            //            break;
            //        }
            //}

            //return default;
        }

        // (X,Y)地点が指定した能力を持っているか
        public bool TerrainHasFeature(int X, int Y, ref string Feature)
        {
            throw new NotImplementedException();
            //bool TerrainHasFeatureRet = default;
            //switch (MapData[X, Y, MapDataIndex.BoxType])
            //{
            //    case BoxTypes.Under:
            //    case BoxTypes.UpperBmpOnly:
            //        {
            //            // 上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
            //            TerrainHasFeatureRet = SRC.TDList.IsFeatureAvailable(MapData[X, Y, MapDataIndex.TerrainType], ref Feature);
            //            break;
            //        }

            //    default:
            //        {
            //            // 上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
            //            TerrainHasFeatureRet = SRC.TDList.IsFeatureAvailable(MapData[X, Y, MapDataIndex.LayerType], ref Feature);
            //            break;
            //        }
            //}

            //return TerrainHasFeatureRet;
        }
        // ADD  END  240a

        // (X,Y)地点にいるユニット
        public Unit UnitAtPoint(int X, int Y)
        {
            throw new NotImplementedException();
            //Unit UnitAtPointRet = default;
            //if (X < 1 | MapWidth < X)
            //{
            //    // UPGRADE_NOTE: オブジェクト UnitAtPoint をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    UnitAtPointRet = null;
            //    return UnitAtPointRet;
            //}

            //if (Y < 1 | MapHeight < Y)
            //{
            //    // UPGRADE_NOTE: オブジェクト UnitAtPoint をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    UnitAtPointRet = null;
            //    return UnitAtPointRet;
            //}

            //UnitAtPointRet = MapDataForUnit[X, Y];
            //return UnitAtPointRet;
        }

        // 指定したマップ画像を検索する
        public string SearchTerrainImageFile(int tid, int tbitmap, int tx, int ty)
        {
            throw new NotImplementedException();
            //            string SearchTerrainImageFileRet = default;
            //            string tbmpname;
            //            string fname2, fname1, fname3;
            //            ;
            //#error Cannot convert LocalDeclarationStatementSyntax - see comment for details
            //            /* Cannot convert LocalDeclarationStatementSyntax, System.NotSupportedException: StaticKeyword not supported!
            //               場所 ICSharpCode.CodeConverter.CSharp.SyntaxKindExtensions.ConvertToken(SyntaxKind t, TokenContext context)
            //               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifier(SyntaxToken m, TokenContext context)
            //               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.<ConvertModifiersCore>d__43.MoveNext()
            //               場所 System.Linq.Enumerable.<ConcatIterator>d__59`1.MoveNext()
            //               場所 System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
            //               場所 System.Linq.Buffer`1..ctor(IEnumerable`1 source)
            //               場所 System.Linq.OrderedEnumerable`1.<GetEnumerator>d__1.MoveNext()
            //               場所 Microsoft.CodeAnalysis.SyntaxTokenList.CreateNode(IEnumerable`1 tokens)
            //               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifiers(SyntaxNode node, IReadOnlyCollection`1 modifiers, TokenContext context, Boolean isVariableOrConst, SyntaxKind[] extraCsModifierKinds)
            //               場所 ICSharpCode.CodeConverter.CSharp.MethodBodyExecutableStatementVisitor.<VisitLocalDeclarationStatement>d__31.MoveNext()
            //            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
            //               場所 ICSharpCode.CodeConverter.CSharp.HoistedNodeStateVisitor.<AddLocalVariablesAsync>d__6.MoveNext()
            //            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
            //               場所 ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.<DefaultVisitInnerAsync>d__3.MoveNext()

            //            Input:
            //                    init_setup_background As Boolean

            //             */
            //            ;
            //#error Cannot convert LocalDeclarationStatementSyntax - see comment for details
            //            /* Cannot convert LocalDeclarationStatementSyntax, System.NotSupportedException: StaticKeyword not supported!
            //               場所 ICSharpCode.CodeConverter.CSharp.SyntaxKindExtensions.ConvertToken(SyntaxKind t, TokenContext context)
            //               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifier(SyntaxToken m, TokenContext context)
            //               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.<ConvertModifiersCore>d__43.MoveNext()
            //               場所 System.Linq.Enumerable.<ConcatIterator>d__59`1.MoveNext()
            //               場所 System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
            //               場所 System.Linq.Buffer`1..ctor(IEnumerable`1 source)
            //               場所 System.Linq.OrderedEnumerable`1.<GetEnumerator>d__1.MoveNext()
            //               場所 Microsoft.CodeAnalysis.SyntaxTokenList.CreateNode(IEnumerable`1 tokens)
            //               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifiers(SyntaxNode node, IReadOnlyCollection`1 modifiers, TokenContext context, Boolean isVariableOrConst, SyntaxKind[] extraCsModifierKinds)
            //               場所 ICSharpCode.CodeConverter.CSharp.MethodBodyExecutableStatementVisitor.<VisitLocalDeclarationStatement>d__31.MoveNext()
            //            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
            //               場所 ICSharpCode.CodeConverter.CSharp.HoistedNodeStateVisitor.<AddLocalVariablesAsync>d__6.MoveNext()
            //            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
            //               場所 ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.<DefaultVisitInnerAsync>d__3.MoveNext()

            //            Input:
            //                    scenario_map_dir_exists As Boolean

            //             */
            //            ;
            //#error Cannot convert LocalDeclarationStatementSyntax - see comment for details
            //            /* Cannot convert LocalDeclarationStatementSyntax, System.NotSupportedException: StaticKeyword not supported!
            //               場所 ICSharpCode.CodeConverter.CSharp.SyntaxKindExtensions.ConvertToken(SyntaxKind t, TokenContext context)
            //               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifier(SyntaxToken m, TokenContext context)
            //               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.<ConvertModifiersCore>d__43.MoveNext()
            //               場所 System.Linq.Enumerable.<ConcatIterator>d__59`1.MoveNext()
            //               場所 System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
            //               場所 System.Linq.Buffer`1..ctor(IEnumerable`1 source)
            //               場所 System.Linq.OrderedEnumerable`1.<GetEnumerator>d__1.MoveNext()
            //               場所 Microsoft.CodeAnalysis.SyntaxTokenList.CreateNode(IEnumerable`1 tokens)
            //               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifiers(SyntaxNode node, IReadOnlyCollection`1 modifiers, TokenContext context, Boolean isVariableOrConst, SyntaxKind[] extraCsModifierKinds)
            //               場所 ICSharpCode.CodeConverter.CSharp.MethodBodyExecutableStatementVisitor.<VisitLocalDeclarationStatement>d__31.MoveNext()
            //            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
            //               場所 ICSharpCode.CodeConverter.CSharp.HoistedNodeStateVisitor.<AddLocalVariablesAsync>d__6.MoveNext()
            //            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
            //               場所 ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.<DefaultVisitInnerAsync>d__3.MoveNext()

            //            Input:
            //                    extdata_map_dir_exists As Boolean

            //             */
            //            ;

            //            // ADD START 240a
            //            // 画像無が確定してるなら処理しない
            //            if (tid == NO_LAYER_NUM)
            //            {
            //                return SearchTerrainImageFileRet;
            //            }
            //            else if (tbitmap == NO_LAYER_NUM)
            //            {
            //                return SearchTerrainImageFileRet;
            //            }
            //            // ADD  END  240a

            //            // 初めて実行する際に、各フォルダにBitmap\Mapフォルダがあるかチェック
            //            if (!init_setup_background)
            //            {
            //                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                if (Strings.Len(FileSystem.Dir(SRC.ScenarioPath + @"Bitmap\Map", FileAttribute.Directory)) > 0)
            //                {
            //                    scenario_map_dir_exists = true;
            //                }
            //                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath + @"Bitmap\Map", FileAttribute.Directory)) > 0)
            //                {
            //                    extdata_map_dir_exists = true;
            //                }
            //                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath2 + @"Bitmap\Map", FileAttribute.Directory)) > 0)
            //                {
            //                    extdata2_map_dir_exists = true;
            //                }

            //                init_setup_background = true;
            //            }

            //            // マップ画像のファイル名を作成
            //            tbmpname = SRC.TDList.Bitmap(tid);
            //            fname1 = @"\Bitmap\Map\" + tbmpname + @"\" + tbmpname + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(tbitmap, "0000") + ".bmp";
            //            fname2 = @"\Bitmap\Map\" + tbmpname + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(tbitmap, "0000") + ".bmp";
            //            fname3 = @"\Bitmap\Map\" + tbmpname + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(tbitmap) + ".bmp";

            //            // ビットマップを探す
            //            if (scenario_map_dir_exists)
            //            {
            //                string argfname = SRC.ScenarioPath + fname1;
            //                if (GeneralLib.FileExists(ref argfname))
            //                {
            //                    SearchTerrainImageFileRet = SRC.ScenarioPath + fname1;
            //                    MapImageFileTypeData[tx, ty] = MapImageFileType.SeparateDirMapImageFileType;
            //                    return SearchTerrainImageFileRet;
            //                }

            //                string argfname1 = SRC.ScenarioPath + fname2;
            //                if (GeneralLib.FileExists(ref argfname1))
            //                {
            //                    SearchTerrainImageFileRet = SRC.ScenarioPath + fname2;
            //                    MapImageFileTypeData[tx, ty] = MapImageFileType.FourFiguresMapImageFileType;
            //                    return SearchTerrainImageFileRet;
            //                }

            //                string argfname2 = SRC.ScenarioPath + fname3;
            //                if (GeneralLib.FileExists(ref argfname2))
            //                {
            //                    SearchTerrainImageFileRet = SRC.ScenarioPath + fname3;
            //                    MapImageFileTypeData[tx, ty] = MapImageFileType.OldMapImageFileType;
            //                    return SearchTerrainImageFileRet;
            //                }
            //            }

            //            if (extdata_map_dir_exists)
            //            {
            //                string argfname3 = SRC.ExtDataPath + fname1;
            //                if (GeneralLib.FileExists(ref argfname3))
            //                {
            //                    SearchTerrainImageFileRet = SRC.ExtDataPath + fname1;
            //                    MapImageFileTypeData[tx, ty] = MapImageFileType.SeparateDirMapImageFileType;
            //                    return SearchTerrainImageFileRet;
            //                }

            //                string argfname4 = SRC.ExtDataPath + fname2;
            //                if (GeneralLib.FileExists(ref argfname4))
            //                {
            //                    SearchTerrainImageFileRet = SRC.ExtDataPath + fname2;
            //                    MapImageFileTypeData[tx, ty] = MapImageFileType.FourFiguresMapImageFileType;
            //                    return SearchTerrainImageFileRet;
            //                }

            //                string argfname5 = SRC.ExtDataPath + fname3;
            //                if (GeneralLib.FileExists(ref argfname5))
            //                {
            //                    SearchTerrainImageFileRet = SRC.ExtDataPath + fname3;
            //                    MapImageFileTypeData[tx, ty] = MapImageFileType.OldMapImageFileType;
            //                    return SearchTerrainImageFileRet;
            //                }
            //            }

            //            if (extdata2_map_dir_exists)
            //            {
            //                string argfname6 = SRC.ExtDataPath2 + fname1;
            //                if (GeneralLib.FileExists(ref argfname6))
            //                {
            //                    SearchTerrainImageFileRet = SRC.ExtDataPath2 + fname1;
            //                    MapImageFileTypeData[tx, ty] = MapImageFileType.SeparateDirMapImageFileType;
            //                    return SearchTerrainImageFileRet;
            //                }

            //                string argfname7 = SRC.ExtDataPath2 + fname2;
            //                if (GeneralLib.FileExists(ref argfname7))
            //                {
            //                    SearchTerrainImageFileRet = SRC.ExtDataPath2 + fname2;
            //                    MapImageFileTypeData[tx, ty] = MapImageFileType.FourFiguresMapImageFileType;
            //                    return SearchTerrainImageFileRet;
            //                }

            //                string argfname8 = SRC.ExtDataPath2 + fname3;
            //                if (GeneralLib.FileExists(ref argfname8))
            //                {
            //                    SearchTerrainImageFileRet = SRC.ExtDataPath2 + fname3;
            //                    MapImageFileTypeData[tx, ty] = MapImageFileType.OldMapImageFileType;
            //                    return SearchTerrainImageFileRet;
            //                }
            //            }

            //            string argfname9 = SRC.AppPath + fname1;
            //            if (GeneralLib.FileExists(ref argfname9))
            //            {
            //                SearchTerrainImageFileRet = SRC.AppPath + fname1;
            //                MapImageFileTypeData[tx, ty] = MapImageFileType.SeparateDirMapImageFileType;
            //                return SearchTerrainImageFileRet;
            //            }

            //            string argfname10 = SRC.AppPath + fname2;
            //            if (GeneralLib.FileExists(ref argfname10))
            //            {
            //                SearchTerrainImageFileRet = SRC.AppPath + fname2;
            //                MapImageFileTypeData[tx, ty] = MapImageFileType.FourFiguresMapImageFileType;
            //                return SearchTerrainImageFileRet;
            //            }

            //            string argfname11 = SRC.AppPath + fname3;
            //            if (GeneralLib.FileExists(ref argfname11))
            //            {
            //                SearchTerrainImageFileRet = SRC.AppPath + fname3;
            //                MapImageFileTypeData[tx, ty] = MapImageFileType.OldMapImageFileType;
            //                return SearchTerrainImageFileRet;
            //            }

            //            return SearchTerrainImageFileRet;
        }


        // マップファイル fname のデータをロード
        public void LoadMapData(string fname)
        {
            throw new NotImplementedException();
            //    int FileNumber;
            //    int i = default, j = default;
            //    var buf = default(string);

            //    // ファイルが存在しない場合
            //    if (string.IsNullOrEmpty(fname) | !GeneralLib.FileExists(ref fname))
            //    {
            //        MapFileName = "";
            //        if (Strings.InStr(SRC.ScenarioFileName, "ステータス表示.") > 0 | Strings.InStr(SRC.ScenarioFileName, "ランキング.") > 0)
            //        {
            //            SetMapSize(GUI.MainWidth, 40);
            //        }
            //        else
            //        {
            //            SetMapSize(GUI.MainWidth, GUI.MainHeight);
            //        }

            //        var loopTo = MapWidth;
            //        for (i = 1; i <= loopTo; i++)
            //        {
            //            var loopTo1 = MapHeight;
            //            for (j = 1; j <= loopTo1; j++)
            //            {
            //                // MOD START 240a
            //                // MapData(i, j, 0) = MAX_TERRAIN_DATA_NUM
            //                // MapData(i, j, 1) = 0
            //                // ファイルが無い場合
            //                MapData[i, j, MapDataIndex.TerrainType] = MAX_TERRAIN_DATA_NUM;
            //                MapData[i, j, MapDataIndex.BitmapNo] = 0;
            //                MapData[i, j, MapDataIndex.LayerType] = NO_LAYER_NUM;
            //                MapData[i, j, MapDataIndex.LayerBitmapNo] = NO_LAYER_NUM;
            //                MapData[i, j, MapDataIndex.BoxType] = BoxTypes.Under;
            //                // MOD  END  240a
            //            }
            //        }

            //        return;
            //    };

            //    // ファイルを開く
            //    FileNumber = FileSystem.FreeFile();
            //    FileSystem.FileOpen(FileNumber, fname, OpenMode.Input);

            //    // ファイル名を記録しておく
            //    MapFileName = fname;

            //    // ファイルの先頭にあるマップサイズ情報を収得
            //    FileSystem.Input(FileNumber, ref buf);
            //    if (buf != "MapData")
            //    {
            //        // 旧形式のマップデータ
            //        SetMapSize(20, 20);
            //        FileSystem.FileClose(FileNumber);
            //        FileNumber = FileSystem.FreeFile();
            //        FileSystem.FileOpen(FileNumber, fname, OpenMode.Input);
            //    }
            //    else
            //    {
            //        FileSystem.Input(FileNumber, ref buf);
            //        FileSystem.Input(FileNumber, ref i);
            //        FileSystem.Input(FileNumber, ref j);
            //        SetMapSize(i, j);
            //    }

            //    // マップデータを読み込み
            //    var loopTo2 = MapWidth;
            //    for (i = 1; i <= loopTo2; i++)
            //    {
            //        var loopTo3 = MapHeight;
            //        for (j = 1; j <= loopTo3; j++)
            //        {
            //            // MOD START 240a
            //            // Input #FileNumber, MapData(i, j, 0), MapData(i, j, 1)
            //            // If Not TDList.IsDefined(MapData(i, j, 0)) Then
            //            // MsgBox "定義されていない" & Format$(MapData(i, j, 0)) _
            //            // '                    & "番の地形データが使われています"
            //            Input(FileNumber, MapData[i, j, MapDataIndex.TerrainType]);
            //            Input(FileNumber, MapData[i, j, MapDataIndex.BitmapNo]);
            //            if (!SRC.TDList.IsDefined(MapData[i, j, MapDataIndex.TerrainType]))
            //            {
            //                Interaction.MsgBox("定義されていない" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(MapData[i, j, MapDataIndex.TerrainType]) + "番の地形データが使われています");
            //                // MOD  END  240a
            //                FileSystem.FileClose(FileNumber);
            //                Environment.Exit(0);
            //            }
            //        }
            //    }

            //    // ADD START 240a
            //    // レイヤーデータ読み込み
            //    if (!FileSystem.EOF(FileNumber))
            //    {
            //        FileSystem.Input(FileNumber, ref buf);
            //        if (buf == "Layer")
            //        {
            //            var loopTo4 = MapWidth;
            //            for (i = 1; i <= loopTo4; i++)
            //            {
            //                var loopTo5 = MapHeight;
            //                for (j = 1; j <= loopTo5; j++)
            //                {
            //                    Input(FileNumber, MapData[i, j, MapDataIndex.LayerType]);
            //                    Input(FileNumber, MapData[i, j, MapDataIndex.LayerBitmapNo]);
            //                    if (MapData[i, j, MapDataIndex.LayerType] != NO_LAYER_NUM)
            //                    {
            //                        // 定義されていたらデータの妥当性チェック
            //                        if (!SRC.TDList.IsDefined(MapData[i, j, MapDataIndex.LayerType]))
            //                        {
            //                            Interaction.MsgBox("定義されていない" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(MapData[i, j, MapDataIndex.LayerType]) + "番の地形データが使われています");
            //                            FileSystem.FileClose(FileNumber);
            //                            Environment.Exit(0);
            //                        }
            //                        // マスのタイプを上層に
            //                        MapData[i, j, MapDataIndex.BoxType] = BoxTypes.Upper;
            //                    }
            //                    else
            //                    {
            //                        // マスのタイプを下層に（初期化時下層だが、再度明示的に設定する）
            //                        MapData[i, j, MapDataIndex.BoxType] = BoxTypes.Under;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    // ADD  END  240a

            //    FileSystem.FileClose(FileNumber);
            //    return;
            //ErrorHandler:
            //    ;
            //    string argmsg = "マップファイル「" + fname + "」のデータが不正です";
            //    GUI.ErrorMessage(ref argmsg);
            //    FileSystem.FileClose(FileNumber);
            //    Environment.Exit(0);
        }

        // マップサイズを設定
        public void SetMapSize(int w, int h)
        {
            //int i, j;
            //int ret;
            MapWidth = w;
            MapHeight = h;
            // XXX MapのGUIインタフェースと実装に追い出す
            //GUI.MapPWidth = (32 * w);
            //GUI.MapPHeight = (32 * h);
            GUI.MapX = ((GUI.MainWidth + 1) / 2);
            GUI.MapY = ((GUI.MainHeight + 1) / 2);
            GUIMap.SetMapSize(w, h);

            //// マップデータ用配列の領域確保
            //// MOD START 240a
            //// ReDim MapData(1 To MapWidth, 1 To MapHeight, 1)
            //// UPGRADE_WARNING: 配列 MapData の下限が 1,1,0 から 0,0,0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
            //MapData = new int[(MapWidth + 1), (MapHeight + 1), 5];
            //// MOD  END  240a
            //// UPGRADE_WARNING: 配列 MapDataForUnit の下限が 1,1 から 0,0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
            //MapDataForUnit = new Unit[(MapWidth + 1), (MapHeight + 1)];
            //// UPGRADE_WARNING: 配列 MaskData の下限が 1,1 から 0,0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
            //MaskData = new bool[(MapWidth + 1), (MapHeight + 1)];
            //TotalMoveCost = new int[MapWidth + 1 + 1, MapHeight + 1 + 1];
            //PointInZOC = new int[MapWidth + 1 + 1, MapHeight + 1 + 1];
            //// UPGRADE_WARNING: 配列 MapImageFileTypeData の下限が 1,1 から 0,0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
            //MapImageFileTypeData = new MapImageFileType[(MapWidth + 1), (MapHeight + 1)];

            //// マップデータ配列の初期化
            //var loopTo = MapWidth;
            //for (i = 1; i <= loopTo; i++)
            //{
            //    var loopTo1 = MapHeight;
            //    for (j = 1; j <= loopTo1; j++)
            //    {
            //        // MOD START 240a
            //        // MapData(i, j, 0) = 0
            //        // MapData(i, j, 1) = 0
            //        MapData[i, j, MapDataIndex.TerrainType] = 0;
            //        MapData[i, j, MapDataIndex.BitmapNo] = 0;
            //        MapData[i, j, MapDataIndex.LayerType] = NO_LAYER_NUM;
            //        MapData[i, j, MapDataIndex.LayerBitmapNo] = NO_LAYER_NUM;
            //        MapData[i, j, MapDataIndex.BoxType] = BoxTypes.Under;
            //        // MOD  END  240a
            //        MaskData[i, j] = true;
            //        // UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //        MapDataForUnit[i, j] = null;
            //    }
            //}
        }

        // マップデータをクリア
        public void ClearMap()
        {
            throw new NotImplementedException();
            //int ret;
            //MapFileName = "";
            //MapWidth = 1;
            //MapHeight = 1;

            //// MOD START 240a
            //// ReDim MapData(1, 1, 1)
            //MapData = new int[2, 2, 5];
            //// MOD  END  240a
            //MapDataForUnit = new Unit[2, 2];
            //MaskData = new bool[2, 2];

            //// MOD START 240a
            //// MapData(1, 1, 0) = 0
            //// MapData(1, 1, 1) = 0
            //MapData[1, 1, MapDataIndex.TerrainType] = 0;
            //MapData[1, 1, MapDataIndex.BitmapNo] = 0;
            //MapData[1, 1, MapDataIndex.LayerType] = 0;
            //MapData[1, 1, MapDataIndex.LayerBitmapNo] = 0;
            //MapData[1, 1, MapDataIndex.BoxType] = 0;
            //// MOD  END  240a
            //// UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //MapDataForUnit[1, 1] = null;

            //// マップ画面を消去
            //// UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //{
            //    var withBlock = GUI.MainForm.picBack;
            //    // UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //    ret = GUI.PatBlt(withBlock.hDC, 0, 0, withBlock.width, withBlock.Height, GUI.BLACKNESS);
            //}
            //// UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //{
            //    var withBlock1 = GUI.MainForm.picMain(0);
            //    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //    ret = GUI.PatBlt(withBlock1.hDC, 0, 0, withBlock1.width, withBlock1.Height, GUI.BLACKNESS);
            //}

            //// ユニット画像をリセット
            //if (!string.IsNullOrEmpty(MapDrawMode) & !MapDrawIsMapOnly)
            //{
            //    SRC.UList.ClearUnitBitmap();
            //}

            //MapDrawMode = "";
            //MapDrawIsMapOnly = false;
            //MapDrawFilterColor = 0;
            //MapDrawFilterTransPercent = 0d;
        }


        // 中断用セーブデータにマップデータをセーブ
        public void DumpMapData()
        {
            throw new NotImplementedException();
            //int i, j;
            //string fname;
            //if (Strings.InStr(MapFileName, SRC.ScenarioPath) == 1)
            //{
            //    fname = Strings.Right(MapFileName, Strings.Len(MapFileName) - Strings.Len(SRC.ScenarioPath));
            //}
            //else
            //{
            //    fname = MapFileName;
            //}

            //if (MapDrawIsMapOnly)
            //{
            //    FileSystem.WriteLine(SRC.SaveDataFileNumber, fname, MapDrawMode + "(マップ限定)");
            //}
            //else
            //{
            //    FileSystem.WriteLine(SRC.SaveDataFileNumber, fname, MapDrawMode);
            //}

            //FileSystem.WriteLine(SRC.SaveDataFileNumber, MapWidth, MapHeight);
            //var loopTo = MapWidth;
            //for (i = 1; i <= loopTo; i++)
            //{
            //    var loopTo1 = MapHeight;
            //    for (j = 1; j <= loopTo1; j++)
            //        // MOD START 240a
            //        // Write #SaveDataFileNumber, MapData(i, j, 0), MapData(i, j, 1)
            //        // ADD  END  240a
            //        FileSystem.WriteLine(SRC.SaveDataFileNumber, MapData[i, j, MapDataIndex.TerrainType], MapData[i, j, MapDataIndex.BitmapNo]);
            //}

            //FileSystem.WriteLine(SRC.SaveDataFileNumber, GUI.MapX, GUI.MapY);

            //// ADD START 240a
            //// レイヤ情報を書き込む
            //FileSystem.WriteLine(SRC.SaveDataFileNumber, "Layer");
            //var loopTo2 = MapWidth;
            //for (i = 1; i <= loopTo2; i++)
            //{
            //    var loopTo3 = MapHeight;
            //    for (j = 1; j <= loopTo3; j++)
            //        FileSystem.WriteLine(SRC.SaveDataFileNumber, MapData[i, j, MapDataIndex.LayerType], MapData[i, j, MapDataIndex.LayerBitmapNo], MapData[i, j, MapDataIndex.BoxType]);
            //}
            //// ADD  END  240a

        }

        // 中断用セーブデータからマップデータをロード
        // MOD START 240a
        // Sub→Functionに
        // Public Sub RestoreMapData() As String
        public string RestoreMapData()
        {
            throw new NotImplementedException();
            //string RestoreMapDataRet = default;
            //// MOD  END  240a
            //string sbuf1 = default, sbuf2 = default;
            //int ibuf1 = default, ibuf2 = default;
            //// ADD START 240a
            //int ibuf3 = default, ibuf4;
            //var buf = default(string);
            //// ADD  END  240a
            //int i, j;
            //var is_map_changed = default(bool);

            //// マップファイル名, マップ描画方法
            //FileSystem.Input(SRC.SaveDataFileNumber, ref sbuf1);
            //FileSystem.Input(SRC.SaveDataFileNumber, ref sbuf2);
            //if (Strings.InStr(sbuf1, ":") == 0)
            //{
            //    sbuf1 = SRC.ScenarioPath + sbuf1;
            //}

            //if ((sbuf1 ?? "") != (MapFileName ?? ""))
            //{
            //    MapFileName = sbuf1;
            //    is_map_changed = true;
            //}

            //if (MapDrawIsMapOnly)
            //{
            //    if ((sbuf2 ?? "") != (MapDrawMode + "(マップ限定)" ?? ""))
            //    {
            //        if (Strings.Right(sbuf2, 7) == "(マップ限定)")
            //        {
            //            MapDrawMode = Strings.Left(sbuf2, Strings.Len(sbuf2) - 7);
            //            MapDrawIsMapOnly = true;
            //        }
            //        else
            //        {
            //            MapDrawMode = sbuf2;
            //            MapDrawIsMapOnly = false;
            //        }

            //        SRC.UList.ClearUnitBitmap();
            //        is_map_changed = true;
            //    }
            //}
            //else if ((sbuf2 ?? "") != (MapDrawMode ?? ""))
            //{
            //    if (Strings.Right(sbuf2, 7) == "(マップ限定)")
            //    {
            //        MapDrawMode = Strings.Left(sbuf2, Strings.Len(sbuf2) - 7);
            //        MapDrawIsMapOnly = true;
            //    }
            //    else
            //    {
            //        MapDrawMode = sbuf2;
            //        MapDrawIsMapOnly = false;
            //    }

            //    SRC.UList.ClearUnitBitmap();
            //    is_map_changed = true;
            //}

            //// マップ幅, マップ高さ
            //FileSystem.Input(SRC.SaveDataFileNumber, ref ibuf1);
            //FileSystem.Input(SRC.SaveDataFileNumber, ref ibuf2);
            //if (ibuf1 != MapWidth | ibuf2 != MapHeight)
            //{
            //    SetMapSize(ibuf1, ibuf2);
            //}

            //// 各地形
            //var loopTo = MapWidth;
            //for (i = 1; i <= loopTo; i++)
            //{
            //    var loopTo1 = MapHeight;
            //    for (j = 1; j <= loopTo1; j++)
            //    {
            //        FileSystem.Input(SRC.SaveDataFileNumber, ref ibuf1);
            //        FileSystem.Input(SRC.SaveDataFileNumber, ref ibuf2);
            //        // MOD START 240a
            //        // If ibuf1 <> MapData(i, j, 0) Then
            //        // MapData(i, j, 0) = ibuf1
            //        // is_map_changed = True
            //        // End If
            //        // If ibuf2 <> MapData(i, j, 1) Then
            //        // MapData(i, j, 1) = ibuf2
            //        // is_map_changed = True
            //        // End If
            //        if (ibuf1 != MapData[i, j, MapDataIndex.TerrainType])
            //        {
            //            MapData[i, j, MapDataIndex.TerrainType] = ibuf1;
            //            is_map_changed = true;
            //        }

            //        if (ibuf2 != MapData[i, j, MapDataIndex.BitmapNo])
            //        {
            //            MapData[i, j, MapDataIndex.BitmapNo] = ibuf2;
            //            is_map_changed = true;
            //        }
            //        // MOD  END  240a
            //    }
            //}

            //// MOV START 240a
            //// '背景書き換えの必要有り？
            //// If is_map_changed Then
            //// IsMapDirty = True
            //// End If
            //// MOV  END  240a

            //// 表示位置
            //// SetupBackgroundでMapX,MapYが書き換えられてしまうため、この位置で
            //// 値を参照する必要がある。
            //FileSystem.Input(SRC.SaveDataFileNumber, ref GUI.MapX);
            //FileSystem.Input(SRC.SaveDataFileNumber, ref GUI.MapY);

            //// ADD START 240a
            //FileSystem.Input(SRC.SaveDataFileNumber, ref buf);
            //if ("Layer" == buf)
            //{
            //    // 各レイヤ
            //    var loopTo2 = MapWidth;
            //    for (i = 1; i <= loopTo2; i++)
            //    {
            //        var loopTo3 = MapHeight;
            //        for (j = 1; j <= loopTo3; j++)
            //        {
            //            FileSystem.Input(SRC.SaveDataFileNumber, ref ibuf1);
            //            FileSystem.Input(SRC.SaveDataFileNumber, ref ibuf2);
            //            FileSystem.Input(SRC.SaveDataFileNumber, ref ibuf3);
            //            if (ibuf1 != MapData[i, j, MapDataIndex.LayerType])
            //            {
            //                MapData[i, j, MapDataIndex.LayerType] = ibuf1;
            //                is_map_changed = true;
            //            }

            //            if (ibuf2 != MapData[i, j, MapDataIndex.LayerBitmapNo])
            //            {
            //                MapData[i, j, MapDataIndex.LayerBitmapNo] = ibuf2;
            //                is_map_changed = true;
            //            }

            //            if (ibuf3 != MapData[i, j, MapDataIndex.BoxType])
            //            {
            //                MapData[i, j, MapDataIndex.BoxType] = ibuf3;
            //                is_map_changed = true;
            //            }
            //        }
            //    }
            //    // ＢＧＭ関連情報の1行目を読み込む
            //    FileSystem.Input(SRC.SaveDataFileNumber, ref buf);
            //}

            //RestoreMapDataRet = buf;
            //// 背景書き換えの必要有り？
            //if (is_map_changed)
            //{
            //    IsMapDirty = true;
            //}
            //// ADD  END  240a

            //// ユニット配置
            //var loopTo4 = MapWidth;
            //for (i = 1; i <= loopTo4; i++)
            //{
            //    var loopTo5 = MapHeight;
            //    for (j = 1; j <= loopTo5; j++)
            //        // UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //        MapDataForUnit[i, j] = null;
            //}

            //foreach (Unit u in SRC.UList)
            //{
            //    if (u.Status == "出撃")
            //    {
            //        MapDataForUnit[u.x, u.y] = u;
            //    }
            //}

            //return RestoreMapDataRet;
        }


        // (X,Y)を中心とする min_range - max_range のエリアを選択
        // エリア内のユニットは uparty の指示に従い選択
        public void AreaInRange(int X, int Y, int max_range, int min_range, ref string uparty)
        {
            throw new NotImplementedException();
            //int x1, y1;
            //int x2, y2;
            //int i, j;
            //int n;

            //// 選択情報をクリア
            //var loopTo = MapWidth;
            //for (i = 1; i <= loopTo; i++)
            //{
            //    var loopTo1 = MapHeight;
            //    for (j = 1; j <= loopTo1; j++)
            //        MaskData[i, j] = true;
            //}

            //x1 = GeneralLib.MaxLng(X - max_range, 1);
            //x2 = GeneralLib.MinLng(X + max_range, MapWidth);
            //y1 = GeneralLib.MaxLng(Y - max_range, 1);
            //y2 = GeneralLib.MinLng(Y + max_range, MapHeight);

            //// max_range内かつmin_range外のエリアを選択
            //var loopTo2 = x2;
            //for (i = x1; i <= loopTo2; i++)
            //{
            //    var loopTo3 = y2;
            //    for (j = y1; j <= loopTo3; j++)
            //    {
            //        n = (Math.Abs((X - i)) + Math.Abs((Y - j)));
            //        if (n <= max_range)
            //        {
            //            if (n >= min_range)
            //            {
            //                MaskData[i, j] = false;
            //            }
            //        }
            //    }
            //}

            //// エリア内のユニットを選択するかそれぞれ判定
            //switch (uparty ?? "")
            //{
            //    case "味方":
            //    case "ＮＰＣ":
            //        {
            //            var loopTo4 = x2;
            //            for (i = x1; i <= loopTo4; i++)
            //            {
            //                var loopTo5 = y2;
            //                for (j = y1; j <= loopTo5; j++)
            //                {
            //                    if (!MaskData[i, j])
            //                    {
            //                        if (MapDataForUnit[i, j] is object)
            //                        {
            //                            if (!(MapDataForUnit[i, j].Party == "味方") & !(MapDataForUnit[i, j].Party == "ＮＰＣ"))
            //                            {
            //                                MaskData[i, j] = true;
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "味方の敵":
            //    case "ＮＰＣの敵":
            //        {
            //            var loopTo6 = x2;
            //            for (i = x1; i <= loopTo6; i++)
            //            {
            //                var loopTo7 = y2;
            //                for (j = y1; j <= loopTo7; j++)
            //                {
            //                    if (!MaskData[i, j])
            //                    {
            //                        if (MapDataForUnit[i, j] is object)
            //                        {
            //                            {
            //                                var withBlock = MapDataForUnit[i, j];
            //                                object argIndex1 = "暴走";
            //                                object argIndex2 = "魅了";
            //                                object argIndex3 = "混乱";
            //                                object argIndex4 = "憑依";
            //                                object argIndex5 = "睡眠";
            //                                if ((withBlock.Party == "味方" | withBlock.Party == "ＮＰＣ") & !withBlock.IsConditionSatisfied(ref argIndex1) & !withBlock.IsConditionSatisfied(ref argIndex2) & !withBlock.IsConditionSatisfied(ref argIndex3) & !withBlock.IsConditionSatisfied(ref argIndex4) & !withBlock.IsConditionSatisfied(ref argIndex5))
            //                                {
            //                                    MaskData[i, j] = true;
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "敵":
            //        {
            //            var loopTo8 = x2;
            //            for (i = x1; i <= loopTo8; i++)
            //            {
            //                var loopTo9 = y2;
            //                for (j = y1; j <= loopTo9; j++)
            //                {
            //                    if (!MaskData[i, j])
            //                    {
            //                        if (MapDataForUnit[i, j] is object)
            //                        {
            //                            if (!(MapDataForUnit[i, j].Party == "敵"))
            //                            {
            //                                MaskData[i, j] = true;
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "敵の敵":
            //        {
            //            var loopTo10 = x2;
            //            for (i = x1; i <= loopTo10; i++)
            //            {
            //                var loopTo11 = y2;
            //                for (j = y1; j <= loopTo11; j++)
            //                {
            //                    if (!MaskData[i, j])
            //                    {
            //                        if (MapDataForUnit[i, j] is object)
            //                        {
            //                            {
            //                                var withBlock1 = MapDataForUnit[i, j];
            //                                if (withBlock1.Party == "敵")
            //                                {
            //                                    MaskData[i, j] = true;
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "中立":
            //        {
            //            var loopTo12 = x2;
            //            for (i = x1; i <= loopTo12; i++)
            //            {
            //                var loopTo13 = y2;
            //                for (j = y1; j <= loopTo13; j++)
            //                {
            //                    if (!MaskData[i, j])
            //                    {
            //                        if (MapDataForUnit[i, j] is object)
            //                        {
            //                            if (!(MapDataForUnit[i, j].Party == "中立"))
            //                            {
            //                                MaskData[i, j] = true;
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "中立の敵":
            //        {
            //            var loopTo14 = x2;
            //            for (i = x1; i <= loopTo14; i++)
            //            {
            //                var loopTo15 = y2;
            //                for (j = y1; j <= loopTo15; j++)
            //                {
            //                    if (!MaskData[i, j])
            //                    {
            //                        if (MapDataForUnit[i, j] is object)
            //                        {
            //                            {
            //                                var withBlock2 = MapDataForUnit[i, j];
            //                                if (withBlock2.Party == "中立")
            //                                {
            //                                    MaskData[i, j] = true;
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "空間":
            //        {
            //            var loopTo16 = x2;
            //            for (i = x1; i <= loopTo16; i++)
            //            {
            //                var loopTo17 = y2;
            //                for (j = y1; j <= loopTo17; j++)
            //                {
            //                    if (!MaskData[i, j])
            //                    {
            //                        if (MapDataForUnit[i, j] is object)
            //                        {
            //                            MaskData[i, j] = true;
            //                        }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "全て":
            //    case "無差別":
            //        {
            //            break;
            //        }
            //}

            //// エリアの中心は常に選択
            //MaskData[X, Y] = false;
        }

        // ユニット u から移動後使用可能な射程 max_range の武器／アビリティを使う場合の効果範囲
        // エリア内のユニットは Party の指示に従い選択
        public void AreaInReachable(ref Unit u, int max_range, ref string uparty)
        {
            throw new NotImplementedException();
            //bool[] tmp_mask_data;
            //int j, i, k;

            //// まずは移動範囲を選択
            //AreaInSpeed(ref u);

            //// 選択範囲をmax_rangeぶんだけ拡大
            //tmp_mask_data = new bool[MapWidth + 1 + 1, MapHeight + 1 + 1];
            //var loopTo = (MapWidth + 1);
            //for (i = 0; i <= loopTo; i++)
            //{
            //    var loopTo1 = (MapHeight + 1);
            //    for (j = 0; j <= loopTo1; j++)
            //        tmp_mask_data[i, j] = true;
            //}

            //var loopTo2 = max_range;
            //for (i = 1; i <= loopTo2; i++)
            //{
            //    var loopTo3 = MapWidth;
            //    for (j = 1; j <= loopTo3; j++)
            //    {
            //        var loopTo4 = MapHeight;
            //        for (k = 1; k <= loopTo4; k++)
            //            tmp_mask_data[j, k] = MaskData[j, k];
            //    }

            //    var loopTo5 = MapWidth;
            //    for (j = 1; j <= loopTo5; j++)
            //    {
            //        var loopTo6 = MapHeight;
            //        for (k = 1; k <= loopTo6; k++)
            //            MaskData[j, k] = tmp_mask_data[j, k] & tmp_mask_data[j - 1, k] & tmp_mask_data[j + 1, k] & tmp_mask_data[j, k - 1] & tmp_mask_data[j, k + 1];
            //    }
            //}

            //// エリア内のユニットを選択するかそれぞれ判定
            //switch (uparty ?? "")
            //{
            //    case "味方":
            //    case "ＮＰＣ":
            //        {
            //            var loopTo7 = MapWidth;
            //            for (i = 1; i <= loopTo7; i++)
            //            {
            //                var loopTo8 = MapHeight;
            //                for (j = 1; j <= loopTo8; j++)
            //                {
            //                    if (!MaskData[i, j])
            //                    {
            //                        if (MapDataForUnit[i, j] is object)
            //                        {
            //                            if (!(MapDataForUnit[i, j].Party == "味方") & !(MapDataForUnit[i, j].Party == "ＮＰＣ"))
            //                            {
            //                                MaskData[i, j] = true;
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "味方の敵":
            //    case "ＮＰＣの敵":
            //        {
            //            var loopTo9 = MapWidth;
            //            for (i = 1; i <= loopTo9; i++)
            //            {
            //                var loopTo10 = MapHeight;
            //                for (j = 1; j <= loopTo10; j++)
            //                {
            //                    if (!MaskData[i, j])
            //                    {
            //                        if (MapDataForUnit[i, j] is object)
            //                        {
            //                            {
            //                                var withBlock = MapDataForUnit[i, j];
            //                                object argIndex1 = "暴走";
            //                                object argIndex2 = "魅了";
            //                                object argIndex3 = "憑依";
            //                                if ((withBlock.Party == "味方" | withBlock.Party == "ＮＰＣ") & !withBlock.IsConditionSatisfied(ref argIndex1) & !withBlock.IsConditionSatisfied(ref argIndex2) & !withBlock.IsConditionSatisfied(ref argIndex3))
            //                                {
            //                                    MaskData[i, j] = true;
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "敵":
            //        {
            //            var loopTo11 = MapWidth;
            //            for (i = 1; i <= loopTo11; i++)
            //            {
            //                var loopTo12 = MapHeight;
            //                for (j = 1; j <= loopTo12; j++)
            //                {
            //                    if (!MaskData[i, j])
            //                    {
            //                        if (MapDataForUnit[i, j] is object)
            //                        {
            //                            if (!(MapDataForUnit[i, j].Party == "敵"))
            //                            {
            //                                MaskData[i, j] = true;
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "敵の敵":
            //        {
            //            var loopTo13 = MapWidth;
            //            for (i = 1; i <= loopTo13; i++)
            //            {
            //                var loopTo14 = MapHeight;
            //                for (j = 1; j <= loopTo14; j++)
            //                {
            //                    if (!MaskData[i, j])
            //                    {
            //                        if (MapDataForUnit[i, j] is object)
            //                        {
            //                            {
            //                                var withBlock1 = MapDataForUnit[i, j];
            //                                if (withBlock1.Party == "敵")
            //                                {
            //                                    MaskData[i, j] = true;
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "中立":
            //        {
            //            var loopTo15 = MapWidth;
            //            for (i = 1; i <= loopTo15; i++)
            //            {
            //                var loopTo16 = MapHeight;
            //                for (j = 1; j <= loopTo16; j++)
            //                {
            //                    if (!MaskData[i, j])
            //                    {
            //                        if (MapDataForUnit[i, j] is object)
            //                        {
            //                            if (!(MapDataForUnit[i, j].Party == "中立"))
            //                            {
            //                                MaskData[i, j] = true;
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "中立の敵":
            //        {
            //            var loopTo17 = MapWidth;
            //            for (i = 1; i <= loopTo17; i++)
            //            {
            //                var loopTo18 = MapHeight;
            //                for (j = 1; j <= loopTo18; j++)
            //                {
            //                    if (!MaskData[i, j])
            //                    {
            //                        if (MapDataForUnit[i, j] is object)
            //                        {
            //                            {
            //                                var withBlock2 = MapDataForUnit[i, j];
            //                                if (withBlock2.Party == "中立")
            //                                {
            //                                    MaskData[i, j] = true;
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "空間":
            //        {
            //            var loopTo19 = MapWidth;
            //            for (i = 1; i <= loopTo19; i++)
            //            {
            //                var loopTo20 = MapHeight;
            //                for (j = 1; j <= loopTo20; j++)
            //                {
            //                    if (!MaskData[i, j])
            //                    {
            //                        if (MapDataForUnit[i, j] is object)
            //                        {
            //                            MaskData[i, j] = true;
            //                        }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "全て":
            //    case "無差別":
            //        {
            //            break;
            //        }
            //}

            //// エリアの中心は常に選択
            //MaskData[u.x, u.y] = false;
        }

        // マップ全域に渡ってupartyに属するユニットが存在する場所を選択
        public void AreaWithUnit(ref string uparty)
        {
            throw new NotImplementedException();
            //int i, j;
            //Unit u;
            //var loopTo = MapWidth;
            //for (i = 1; i <= loopTo; i++)
            //{
            //    var loopTo1 = MapHeight;
            //    for (j = 1; j <= loopTo1; j++)
            //        MaskData[i, j] = true;
            //}

            //var loopTo2 = MapWidth;
            //for (i = 1; i <= loopTo2; i++)
            //{
            //    var loopTo3 = MapHeight;
            //    for (j = 1; j <= loopTo3; j++)
            //    {
            //        u = MapDataForUnit[i, j];
            //        if (u is null)
            //        {
            //            goto NextLoop;
            //        }

            //        switch (uparty ?? "")
            //        {
            //            case "味方":
            //                {
            //                    if (u.Party == "味方" | u.Party == "ＮＰＣ")
            //                    {
            //                        MaskData[i, j] = false;
            //                    }

            //                    break;
            //                }

            //            case "味方の敵":
            //                {
            //                    if (u.Party != "味方" & u.Party != "ＮＰＣ")
            //                    {
            //                        MaskData[i, j] = false;
            //                    }

            //                    break;
            //                }

            //            case "敵":
            //                {
            //                    if (u.Party == "敵")
            //                    {
            //                        MaskData[i, j] = false;
            //                    }

            //                    break;
            //                }

            //            case "敵の敵":
            //                {
            //                    if (u.Party != "敵")
            //                    {
            //                        MaskData[i, j] = false;
            //                    }

            //                    break;
            //                }

            //            case "中立":
            //                {
            //                    if (u.Party == "中立")
            //                    {
            //                        MaskData[i, j] = false;
            //                    }

            //                    break;
            //                }

            //            case "中立の敵":
            //                {
            //                    if (u.Party != "中立")
            //                    {
            //                        MaskData[i, j] = false;
            //                    }

            //                    break;
            //                }

            //            default:
            //                {
            //                    MaskData[i, j] = false;
            //                    break;
            //                }
            //        }

            //    NextLoop:
            //        ;
            //    }
            //}
        }

        // 十字状のエリアを選択 (Ｍ直の攻撃方向選択用)
        public void AreaInCross(int X, int Y, int min_range, ref int max_range)
        {
            throw new NotImplementedException();
            //int i, j;
            //var loopTo = MapWidth;
            //for (i = 1; i <= loopTo; i++)
            //{
            //    var loopTo1 = MapHeight;
            //    for (j = 1; j <= loopTo1; j++)
            //        MaskData[i, j] = true;
            //}

            //var loopTo2 = (Y - min_range);
            //for (i = (Y - max_range); i <= loopTo2; i++)
            //{
            //    if (i >= 1)
            //    {
            //        MaskData[X, i] = false;
            //    }
            //}

            //var loopTo3 = (Y + max_range);
            //for (i = (Y + min_range); i <= loopTo3; i++)
            //{
            //    if (i <= MapHeight)
            //    {
            //        MaskData[X, i] = false;
            //    }
            //}

            //var loopTo4 = (X - min_range);
            //for (i = (X - max_range); i <= loopTo4; i++)
            //{
            //    if (i >= 1)
            //    {
            //        MaskData[i, Y] = false;
            //    }
            //}

            //var loopTo5 = (X + max_range);
            //for (i = (X + min_range); i <= loopTo5; i++)
            //{
            //    if (i <= MapWidth)
            //    {
            //        MaskData[i, Y] = false;
            //    }
            //}

            //MaskData[X, Y] = false;
        }

        // 直線状のエリアを選択 (Ｍ直の攻撃範囲設定用)
        public void AreaInLine(int X, int Y, int min_range, ref int max_range, ref string direction)
        {
            throw new NotImplementedException();
            //int i, j;
            //var loopTo = MapWidth;
            //for (i = 1; i <= loopTo; i++)
            //{
            //    var loopTo1 = MapHeight;
            //    for (j = 1; j <= loopTo1; j++)
            //        MaskData[i, j] = true;
            //}

            //switch (direction ?? "")
            //{
            //    case "N":
            //        {
            //            var loopTo2 = (Y - min_range);
            //            for (i = (Y - max_range); i <= loopTo2; i++)
            //            {
            //                if (i >= 1)
            //                {
            //                    MaskData[X, i] = false;
            //                }
            //            }

            //            break;
            //        }

            //    case "S":
            //        {
            //            var loopTo3 = (Y + max_range);
            //            for (i = (Y + min_range); i <= loopTo3; i++)
            //            {
            //                if (i <= MapHeight)
            //                {
            //                    MaskData[X, i] = false;
            //                }
            //            }

            //            break;
            //        }

            //    case "W":
            //        {
            //            var loopTo4 = (X - min_range);
            //            for (i = (X - max_range); i <= loopTo4; i++)
            //            {
            //                if (i >= 1)
            //                {
            //                    MaskData[i, Y] = false;
            //                }
            //            }

            //            break;
            //        }

            //    case "E":
            //        {
            //            var loopTo5 = (X + max_range);
            //            for (i = (X + min_range); i <= loopTo5; i++)
            //            {
            //                if (i <= MapWidth)
            //                {
            //                    MaskData[i, Y] = false;
            //                }
            //            }

            //            break;
            //        }
            //}

            //MaskData[X, Y] = false;
        }

        // 幅３マスの十字状のエリアを選択 (Ｍ拡の攻撃方向選択用)
        public void AreaInWideCross(int X, int Y, int min_range, ref int max_range)
        {
            throw new NotImplementedException();
            //int i, j;
            //var loopTo = MapWidth;
            //for (i = 1; i <= loopTo; i++)
            //{
            //    var loopTo1 = MapHeight;
            //    for (j = 1; j <= loopTo1; j++)
            //        MaskData[i, j] = true;
            //}

            //var loopTo2 = (Y - min_range);
            //for (i = (Y - max_range); i <= loopTo2; i++)
            //{
            //    if (i >= 1)
            //    {
            //        MaskData[X, i] = false;
            //    }
            //}

            //var loopTo3 = (Y - (min_range + 1));
            //for (i = (Y - max_range + 1); i <= loopTo3; i++)
            //{
            //    if (i >= 1)
            //    {
            //        if (X > 1)
            //        {
            //            MaskData[X - 1, i] = false;
            //        }

            //        if (X < MapWidth)
            //        {
            //            MaskData[X + 1, i] = false;
            //        }
            //    }
            //}

            //var loopTo4 = (Y + max_range);
            //for (i = (Y + min_range); i <= loopTo4; i++)
            //{
            //    if (i <= MapHeight)
            //    {
            //        MaskData[X, i] = false;
            //    }
            //}

            //var loopTo5 = (Y + max_range - 1);
            //for (i = (Y + (min_range + 1)); i <= loopTo5; i++)
            //{
            //    if (i <= MapHeight)
            //    {
            //        if (X > 1)
            //        {
            //            MaskData[X - 1, i] = false;
            //        }

            //        if (X < MapWidth)
            //        {
            //            MaskData[X + 1, i] = false;
            //        }
            //    }
            //}

            //var loopTo6 = (X - min_range);
            //for (i = (X - max_range); i <= loopTo6; i++)
            //{
            //    if (i >= 1)
            //    {
            //        MaskData[i, Y] = false;
            //    }
            //}

            //var loopTo7 = (X - (min_range + 1));
            //for (i = (X - max_range + 1); i <= loopTo7; i++)
            //{
            //    if (i >= 1)
            //    {
            //        if (Y > 1)
            //        {
            //            MaskData[i, Y - 1] = false;
            //        }

            //        if (Y < MapHeight)
            //        {
            //            MaskData[i, Y + 1] = false;
            //        }
            //    }
            //}

            //var loopTo8 = (X + max_range);
            //for (i = (X + min_range); i <= loopTo8; i++)
            //{
            //    if (i <= MapWidth)
            //    {
            //        MaskData[i, Y] = false;
            //    }
            //}

            //var loopTo9 = (X + max_range - 1);
            //for (i = (X + (min_range + 1)); i <= loopTo9; i++)
            //{
            //    if (i <= MapWidth)
            //    {
            //        if (Y > 1)
            //        {
            //            MaskData[i, Y - 1] = false;
            //        }

            //        if (Y < MapHeight)
            //        {
            //            MaskData[i, Y + 1] = false;
            //        }
            //    }
            //}

            //MaskData[X, Y] = false;
        }

        // 幅３マスの直線状のエリアを選択 (Ｍ拡の攻撃範囲設定用)
        public void AreaInCone(int X, int Y, int min_range, ref int max_range, ref string direction)
        {
            throw new NotImplementedException();
            //int i, j;
            //var loopTo = MapWidth;
            //for (i = 1; i <= loopTo; i++)
            //{
            //    var loopTo1 = MapHeight;
            //    for (j = 1; j <= loopTo1; j++)
            //        MaskData[i, j] = true;
            //}

            //switch (direction ?? "")
            //{
            //    case "N":
            //        {
            //            var loopTo2 = (Y - min_range);
            //            for (i = (Y - max_range); i <= loopTo2; i++)
            //            {
            //                if (i >= 1)
            //                {
            //                    MaskData[X, i] = false;
            //                }
            //            }

            //            var loopTo3 = (Y - (min_range + 1));
            //            for (i = (Y - max_range + 1); i <= loopTo3; i++)
            //            {
            //                if (i >= 1)
            //                {
            //                    if (X > 1)
            //                    {
            //                        MaskData[X - 1, i] = false;
            //                    }

            //                    if (X < MapWidth)
            //                    {
            //                        MaskData[X + 1, i] = false;
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "S":
            //        {
            //            var loopTo4 = (Y + max_range);
            //            for (i = (Y + min_range); i <= loopTo4; i++)
            //            {
            //                if (i <= MapHeight)
            //                {
            //                    MaskData[X, i] = false;
            //                }
            //            }

            //            var loopTo5 = (Y + max_range - 1);
            //            for (i = (Y + (min_range + 1)); i <= loopTo5; i++)
            //            {
            //                if (i <= MapHeight)
            //                {
            //                    if (X > 1)
            //                    {
            //                        MaskData[X - 1, i] = false;
            //                    }

            //                    if (X < MapWidth)
            //                    {
            //                        MaskData[X + 1, i] = false;
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "W":
            //        {
            //            var loopTo6 = (X - min_range);
            //            for (i = (X - max_range); i <= loopTo6; i++)
            //            {
            //                if (i >= 1)
            //                {
            //                    MaskData[i, Y] = false;
            //                }
            //            }

            //            var loopTo7 = (X - (min_range + 1));
            //            for (i = (X - max_range + 1); i <= loopTo7; i++)
            //            {
            //                if (i >= 1)
            //                {
            //                    if (Y > 1)
            //                    {
            //                        MaskData[i, Y - 1] = false;
            //                    }

            //                    if (Y < MapHeight)
            //                    {
            //                        MaskData[i, Y + 1] = false;
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "E":
            //        {
            //            var loopTo8 = (X + max_range);
            //            for (i = (X + min_range); i <= loopTo8; i++)
            //            {
            //                if (i <= MapWidth)
            //                {
            //                    MaskData[i, Y] = false;
            //                }
            //            }

            //            var loopTo9 = (X + max_range - 1);
            //            for (i = (X + (min_range + 1)); i <= loopTo9; i++)
            //            {
            //                if (i <= MapWidth)
            //                {
            //                    if (Y > 1)
            //                    {
            //                        MaskData[i, Y - 1] = false;
            //                    }

            //                    if (Y < MapHeight)
            //                    {
            //                        MaskData[i, Y + 1] = false;
            //                    }
            //                }
            //            }

            //            break;
            //        }
            //}

            //MaskData[X, Y] = false;
        }

        // 扇状のエリアを選択 (Ｍ扇の攻撃範囲設定用)
        public void AreaInSector(int X, int Y, int min_range, ref int max_range, ref string direction, int lv, bool without_refresh = false)
        {
            throw new NotImplementedException();
            //int xx, i, yy;
            //if (!without_refresh)
            //{
            //    var loopTo = MapWidth;
            //    for (xx = 1; xx <= loopTo; xx++)
            //    {
            //        var loopTo1 = MapHeight;
            //        for (yy = 1; yy <= loopTo1; yy++)
            //            MaskData[xx, yy] = true;
            //    }
            //}

            //switch (direction ?? "")
            //{
            //    case "N":
            //        {
            //            var loopTo2 = max_range;
            //            for (i = min_range; i <= loopTo2; i++)
            //            {
            //                yy = (Y - i);
            //                if (yy < 1)
            //                {
            //                    break;
            //                }

            //                switch (lv)
            //                {
            //                    case 1:
            //                        {
            //                            var loopTo3 = GeneralLib.MinLng(X + i / 3, MapWidth);
            //                            for (xx = GeneralLib.MaxLng(X - i / 3, 1); xx <= loopTo3; xx++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }

            //                    case 2:
            //                        {
            //                            var loopTo4 = GeneralLib.MinLng(X + i / 2, MapWidth);
            //                            for (xx = GeneralLib.MaxLng(X - i / 2, 1); xx <= loopTo4; xx++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }

            //                    case 3:
            //                        {
            //                            var loopTo5 = GeneralLib.MinLng(X + (i - 1), MapWidth);
            //                            for (xx = GeneralLib.MaxLng(X - (i - 1), 1); xx <= loopTo5; xx++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }

            //                    case 4:
            //                        {
            //                            var loopTo6 = GeneralLib.MinLng(X + i, MapWidth);
            //                            for (xx = GeneralLib.MaxLng(X - i, 1); xx <= loopTo6; xx++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }
            //                }
            //            }

            //            break;
            //        }

            //    case "S":
            //        {
            //            var loopTo7 = max_range;
            //            for (i = min_range; i <= loopTo7; i++)
            //            {
            //                yy = (Y + i);
            //                if (yy > MapHeight)
            //                {
            //                    break;
            //                }

            //                switch (lv)
            //                {
            //                    case 1:
            //                        {
            //                            var loopTo8 = GeneralLib.MinLng(X + i / 3, MapWidth);
            //                            for (xx = GeneralLib.MaxLng(X - i / 3, 1); xx <= loopTo8; xx++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }

            //                    case 2:
            //                        {
            //                            var loopTo9 = GeneralLib.MinLng(X + i / 2, MapWidth);
            //                            for (xx = GeneralLib.MaxLng(X - i / 2, 1); xx <= loopTo9; xx++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }

            //                    case 3:
            //                        {
            //                            var loopTo10 = GeneralLib.MinLng(X + (i - 1), MapWidth);
            //                            for (xx = GeneralLib.MaxLng(X - (i - 1), 1); xx <= loopTo10; xx++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }

            //                    case 4:
            //                        {
            //                            var loopTo11 = GeneralLib.MinLng(X + i, MapWidth);
            //                            for (xx = GeneralLib.MaxLng(X - i, 1); xx <= loopTo11; xx++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }
            //                }
            //            }

            //            break;
            //        }

            //    case "W":
            //        {
            //            var loopTo12 = max_range;
            //            for (i = min_range; i <= loopTo12; i++)
            //            {
            //                xx = (X - i);
            //                if (xx < 1)
            //                {
            //                    break;
            //                }

            //                switch (lv)
            //                {
            //                    case 1:
            //                        {
            //                            var loopTo13 = GeneralLib.MinLng(Y + i / 3, MapHeight);
            //                            for (yy = GeneralLib.MaxLng(Y - i / 3, 1); yy <= loopTo13; yy++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }

            //                    case 2:
            //                        {
            //                            var loopTo14 = GeneralLib.MinLng(Y + i / 2, MapHeight);
            //                            for (yy = GeneralLib.MaxLng(Y - i / 2, 1); yy <= loopTo14; yy++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }

            //                    case 3:
            //                        {
            //                            var loopTo15 = GeneralLib.MinLng(Y + (i - 1), MapHeight);
            //                            for (yy = GeneralLib.MaxLng(Y - (i - 1), 1); yy <= loopTo15; yy++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }

            //                    case 4:
            //                        {
            //                            var loopTo16 = GeneralLib.MinLng(Y + i, MapHeight);
            //                            for (yy = GeneralLib.MaxLng(Y - i, 1); yy <= loopTo16; yy++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }
            //                }
            //            }

            //            break;
            //        }

            //    case "E":
            //        {
            //            var loopTo17 = max_range;
            //            for (i = min_range; i <= loopTo17; i++)
            //            {
            //                xx = (X + i);
            //                if (xx > MapWidth)
            //                {
            //                    break;
            //                }

            //                switch (lv)
            //                {
            //                    case 1:
            //                        {
            //                            var loopTo18 = GeneralLib.MinLng(Y + i / 3, MapHeight);
            //                            for (yy = GeneralLib.MaxLng(Y - i / 3, 1); yy <= loopTo18; yy++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }

            //                    case 2:
            //                        {
            //                            var loopTo19 = GeneralLib.MinLng(Y + i / 2, MapHeight);
            //                            for (yy = GeneralLib.MaxLng(Y - i / 2, 1); yy <= loopTo19; yy++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }

            //                    case 3:
            //                        {
            //                            var loopTo20 = GeneralLib.MinLng(Y + (i - 1), MapHeight);
            //                            for (yy = GeneralLib.MaxLng(Y - (i - 1), 1); yy <= loopTo20; yy++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }

            //                    case 4:
            //                        {
            //                            var loopTo21 = GeneralLib.MinLng(Y + i, MapHeight);
            //                            for (yy = GeneralLib.MaxLng(Y - i, 1); yy <= loopTo21; yy++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }
            //                }
            //            }

            //            break;
            //        }
            //}

            //MaskData[X, Y] = false;
        }

        // 十字状の扇状のエリアを選択 (Ｍ扇の攻撃方向選択用)
        public void AreaInSectorCross(int X, int Y, int min_range, ref int max_range, int lv)
        {
            throw new NotImplementedException();
            //int xx, yy;
            //var loopTo = MapWidth;
            //for (xx = 1; xx <= loopTo; xx++)
            //{
            //    var loopTo1 = MapHeight;
            //    for (yy = 1; yy <= loopTo1; yy++)
            //        MaskData[xx, yy] = true;
            //}

            //string argdirection = "N";
            //AreaInSector(X, Y, min_range, ref max_range, ref argdirection, lv, true);
            //string argdirection1 = "S";
            //AreaInSector(X, Y, min_range, ref max_range, ref argdirection1, lv, true);
            //string argdirection2 = "W";
            //AreaInSector(X, Y, min_range, ref max_range, ref argdirection2, lv, true);
            //string argdirection3 = "E";
            //AreaInSector(X, Y, min_range, ref max_range, ref argdirection3, lv, true);
        }

        // ２点間を結ぶ直線状のエリアを選択 (Ｍ線の範囲設定用)
        public void AreaInPointToPoint(int x1, int y1, int x2, int y2)
        {
            throw new NotImplementedException();
            //int xx, yy;

            //// まず全領域をマスク
            //var loopTo = MapWidth;
            //for (xx = 1; xx <= loopTo; xx++)
            //{
            //    var loopTo1 = MapHeight;
            //    for (yy = 1; yy <= loopTo1; yy++)
            //        MaskData[xx, yy] = true;
            //}

            //// 起点のマスクを解除
            //MaskData[x1, y1] = false;
            //xx = x1;
            //yy = y1;
            //if (Math.Abs((x1 - x2)) > Math.Abs((y1 - y2)))
            //{
            //    do
            //    {
            //        if (x1 > x2)
            //        {
            //            xx = (xx - 1);
            //        }
            //        else
            //        {
            //            xx = (xx + 1);
            //        }

            //        MaskData[xx, yy] = false;
            //        yy = (y1 + (y2 - y1) * (x1 - xx + 0d) / (x1 - x2));
            //        MaskData[xx, yy] = false;
            //    }
            //    while (xx != x2);
            //}
            //else
            //{
            //    do
            //    {
            //        if (y1 > y2)
            //        {
            //            yy = (yy - 1);
            //        }
            //        else
            //        {
            //            yy = (yy + 1);
            //        }

            //        MaskData[xx, yy] = false;
            //        xx = (x1 + (x2 - x1) * (y1 - yy + 0d) / (y1 - y2));
            //        MaskData[xx, yy] = false;
            //    }
            //    while (yy != y2);
            //}
        }

        // ユニット u の移動範囲を選択
        // ジャンプする場合は ByJump = True
        public void AreaInSpeed(ref Unit u, bool ByJump = false)
        {
            throw new NotImplementedException();
            //int l, j, i, k, n = default;
            //var cur_cost = new int[52, 52];
            //var move_cost = new int[52, 52];
            //string move_area;
            //int tmp;
            //string buf;
            //bool is_trans_available_on_ground;
            //bool is_trans_available_in_water;
            //var is_trans_available_on_water = default(bool);
            //bool is_trans_available_in_sky;
            //bool is_trans_available_in_moon_sky;
            //var is_adaptable_in_water = default(bool);
            //var is_adaptable_in_space = default(bool);
            //var is_swimable = default(bool);
            //string[] adopted_terrain;
            //string[] allowed_terrains;
            //string[] prohibited_terrains;
            //int uspeed;
            //Unit u2;
            //int x1, y1;
            //int x2, y2;
            //var zarea = default;
            //bool is_zoc;
            //bool is_vzoc, is_hzoc;
            //// ADD START MARGE
            //TerrainData td;
            //bool is_terrain_effective;
            //// ADD END MARGE

            //bool blocked;
            //{
            //    var withBlock = u;
            //    // 移動時に使用するエリア
            //    if (ByJump)
            //    {
            //        move_area = "空中";
            //    }
            //    else
            //    {
            //        move_area = withBlock.Area;
            //    }

            //    // 移動能力の可否を調べておく
            //    string argarea_name = "陸";
            //    is_trans_available_on_ground = withBlock.IsTransAvailable(ref argarea_name) & withBlock.get_Adaption(2) != 0;
            //    string argarea_name1 = "水";
            //    is_trans_available_in_water = withBlock.IsTransAvailable(ref argarea_name1) & withBlock.get_Adaption(3) != 0;
            //    string argarea_name2 = "空";
            //    is_trans_available_in_sky = withBlock.IsTransAvailable(ref argarea_name2) & withBlock.get_Adaption(1) != 0;
            //    string argarea_name3 = "空";
            //    string argarea_name4 = "宇宙";
            //    is_trans_available_in_moon_sky = withBlock.IsTransAvailable(ref argarea_name3) & withBlock.get_Adaption(1) != 0 | withBlock.IsTransAvailable(ref argarea_name4) & withBlock.get_Adaption(4) != 0;
            //    string argfname = "水中移動";
            //    if (Strings.Mid(withBlock.Data.Adaption, 3, 1) != "-" | withBlock.IsFeatureAvailable(ref argfname))
            //    {
            //        is_adaptable_in_water = true;
            //    }

            //    string argfname1 = "宇宙移動";
            //    if (Strings.Mid(withBlock.Data.Adaption, 4, 1) != "-" | withBlock.IsFeatureAvailable(ref argfname1))
            //    {
            //        is_adaptable_in_space = true;
            //    }

            //    string argfname2 = "水上移動";
            //    string argfname3 = "ホバー移動";
            //    if (withBlock.IsFeatureAvailable(ref argfname2) | withBlock.IsFeatureAvailable(ref argfname3))
            //    {
            //        is_trans_available_on_water = true;
            //    }

            //    string argfname4 = "水泳";
            //    if (withBlock.IsFeatureAvailable(ref argfname4))
            //    {
            //        is_swimable = true;
            //    }

            //    // 地形適応のある地形のリストを作成
            //    adopted_terrain = new string[1];
            //    string argfname5 = "地形適応";
            //    if (withBlock.IsFeatureAvailable(ref argfname5))
            //    {
            //        var loopTo = withBlock.CountFeature();
            //        for (i = 1; i <= loopTo; i++)
            //        {
            //            object argIndex2 = i;
            //            if (withBlock.Feature(ref argIndex2) == "地形適応")
            //            {
            //                object argIndex1 = i;
            //                buf = withBlock.FeatureData(ref argIndex1);
            //                if (GeneralLib.LLength(ref buf) == 0)
            //                {
            //                    string argmsg = "ユニット「" + withBlock.Name + "」の地形適応能力に対応地形が指定されていません";
            //                    GUI.ErrorMessage(ref argmsg);
            //                    SRC.TerminateSRC();
            //                }

            //                n = GeneralLib.LLength(ref buf);
            //                Array.Resize(ref adopted_terrain, Information.UBound(adopted_terrain) + n);
            //                var loopTo1 = n;
            //                for (j = 2; j <= loopTo1; j++)
            //                    adopted_terrain[Information.UBound(adopted_terrain) - j + 2] = GeneralLib.LIndex(ref buf, j);
            //            }
            //        }
            //    }

            //    // 移動力
            //    if (ByJump)
            //    {
            //        object argIndex3 = "ジャンプ";
            //        uspeed = (withBlock.Speed + withBlock.FeatureLevel(ref argIndex3));
            //    }
            //    else
            //    {
            //        uspeed = withBlock.Speed;
            //    }

            //    object argIndex4 = "移動不能";
            //    if (withBlock.IsConditionSatisfied(ref argIndex4))
            //    {
            //        uspeed = 0;
            //    }

            //    // 移動コストは実際の２倍の値で記録されているため、移動力もそれに合わせて
            //    // ２倍にして移動範囲を計算する
            //    uspeed = (2 * uspeed);

            //    // ADD START MARGE
            //    // 再移動時は最初の移動の分だけ移動力を減少させる
            //    if (Commands.SelectedCommand == "再移動")
            //    {
            //        uspeed = (uspeed - Commands.SelectedUnitMoveCost);
            //    }

            //    object argIndex5 = "移動不能";
            //    if (withBlock.IsConditionSatisfied(ref argIndex5))
            //    {
            //        uspeed = 0;
            //    }
            //    // ADD END MARGE

            //    // 移動範囲をチェックすべき領域
            //    x1 = GeneralLib.MaxLng(1, withBlock.x - uspeed);
            //    y1 = GeneralLib.MaxLng(1, withBlock.y - uspeed);
            //    x2 = GeneralLib.MinLng(withBlock.x + uspeed, MapWidth);
            //    y2 = GeneralLib.MinLng(withBlock.y + uspeed, MapHeight);

            //    // 移動コストとＺＯＣをリセット
            //    var loopTo2 = (MapWidth + 1);
            //    for (i = 0; i <= loopTo2; i++)
            //    {
            //        var loopTo3 = (MapHeight + 1);
            //        for (j = 0; j <= loopTo3; j++)
            //        {
            //            move_cost[i, j] = 1000000;
            //            PointInZOC[i, j] = 0;
            //        }
            //    }

            //    // 各地形の移動コストを算出しておく
            //    switch (move_area ?? "")
            //    {
            //        case "空中":
            //            {
            //                var loopTo4 = x2;
            //                for (i = x1; i <= loopTo4; i++)
            //                {
            //                    var loopTo5 = y2;
            //                    for (j = y1; j <= loopTo5; j++)
            //                    {
            //                        switch (TerrainClass(i, j) ?? "")
            //                        {
            //                            case "空":
            //                                {
            //                                    move_cost[i, j] = TerrainMoveCost(i, j);
            //                                    break;
            //                                }

            //                            case "宇宙":
            //                                {
            //                                    if (is_adaptable_in_space)
            //                                    {
            //                                        move_cost[i, j] = TerrainMoveCost(i, j);
            //                                        var loopTo6 = Information.UBound(adopted_terrain);
            //                                        for (k = 1; k <= loopTo6; k++)
            //                                        {
            //                                            if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
            //                                            {
            //                                                move_cost[i, j] = GeneralLib.MinLng(move_cost[i, j], 2);
            //                                                break;
            //                                            }
            //                                        }
            //                                    }
            //                                    else
            //                                    {
            //                                        move_cost[i, j] = 1000000;
            //                                    }

            //                                    break;
            //                                }

            //                            default:
            //                                {
            //                                    move_cost[i, j] = GeneralLib.MinLng(move_cost[i, j], 2);
            //                                    break;
            //                                }
            //                        }
            //                    }
            //                }

            //                break;
            //            }

            //        case "地上":
            //            {
            //                var loopTo7 = x2;
            //                for (i = x1; i <= loopTo7; i++)
            //                {
            //                    var loopTo8 = y2;
            //                    for (j = y1; j <= loopTo8; j++)
            //                    {
            //                        switch (TerrainClass(i, j) ?? "")
            //                        {
            //                            case "陸":
            //                            case "屋内":
            //                            case "月面":
            //                                {
            //                                    if (is_trans_available_on_ground)
            //                                    {
            //                                        move_cost[i, j] = TerrainMoveCost(i, j);
            //                                        var loopTo9 = Information.UBound(adopted_terrain);
            //                                        for (k = 1; k <= loopTo9; k++)
            //                                        {
            //                                            if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
            //                                            {
            //                                                move_cost[i, j] = GeneralLib.MinLng(move_cost[i, j], 2);
            //                                                break;
            //                                            }
            //                                        }
            //                                    }
            //                                    else
            //                                    {
            //                                        move_cost[i, j] = 1000000;
            //                                    }

            //                                    break;
            //                                }

            //                            case "水":
            //                                {
            //                                    if (is_trans_available_in_water | is_trans_available_on_water)
            //                                    {
            //                                        move_cost[i, j] = 2;
            //                                    }
            //                                    else if (is_adaptable_in_water)
            //                                    {
            //                                        move_cost[i, j] = TerrainMoveCost(i, j);
            //                                        var loopTo10 = Information.UBound(adopted_terrain);
            //                                        for (k = 1; k <= loopTo10; k++)
            //                                        {
            //                                            if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
            //                                            {
            //                                                move_cost[i, j] = GeneralLib.MinLng(move_cost[i, j], 2);
            //                                                break;
            //                                            }
            //                                        }
            //                                    }
            //                                    else
            //                                    {
            //                                        move_cost[i, j] = 1000000;
            //                                    }

            //                                    break;
            //                                }

            //                            case "深水":
            //                                {
            //                                    if (is_trans_available_in_water | is_trans_available_on_water)
            //                                    {
            //                                        move_cost[i, j] = 2;
            //                                    }
            //                                    else if (is_swimable)
            //                                    {
            //                                        move_cost[i, j] = TerrainMoveCost(i, j);
            //                                    }
            //                                    else
            //                                    {
            //                                        move_cost[i, j] = 1000000;
            //                                    }

            //                                    break;
            //                                }

            //                            case "空":
            //                                {
            //                                    move_cost[i, j] = 1000000;
            //                                    break;
            //                                }

            //                            case "宇宙":
            //                                {
            //                                    if (is_adaptable_in_space)
            //                                    {
            //                                        move_cost[i, j] = TerrainMoveCost(i, j);
            //                                        var loopTo11 = Information.UBound(adopted_terrain);
            //                                        for (k = 1; k <= loopTo11; k++)
            //                                        {
            //                                            if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
            //                                            {
            //                                                move_cost[i, j] = GeneralLib.MinLng(move_cost[i, j], 2);
            //                                                break;
            //                                            }
            //                                        }
            //                                    }
            //                                    else
            //                                    {
            //                                        move_cost[i, j] = 1000000;
            //                                    }

            //                                    break;
            //                                }
            //                        }
            //                    }
            //                }

            //                break;
            //            }

            //        case "水上":
            //            {
            //                var loopTo12 = x2;
            //                for (i = x1; i <= loopTo12; i++)
            //                {
            //                    var loopTo13 = y2;
            //                    for (j = y1; j <= loopTo13; j++)
            //                    {
            //                        switch (TerrainClass(i, j) ?? "")
            //                        {
            //                            case "陸":
            //                            case "屋内":
            //                            case "月面":
            //                                {
            //                                    if (is_trans_available_on_ground)
            //                                    {
            //                                        move_cost[i, j] = TerrainMoveCost(i, j);
            //                                        var loopTo14 = Information.UBound(adopted_terrain);
            //                                        for (k = 1; k <= loopTo14; k++)
            //                                        {
            //                                            if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
            //                                            {
            //                                                move_cost[i, j] = GeneralLib.MinLng(move_cost[i, j], 2);
            //                                                break;
            //                                            }
            //                                        }
            //                                    }
            //                                    else
            //                                    {
            //                                        move_cost[i, j] = 1000000;
            //                                    }

            //                                    break;
            //                                }

            //                            case "水":
            //                            case "深水":
            //                                {
            //                                    move_cost[i, j] = 2;
            //                                    break;
            //                                }

            //                            case "空":
            //                                {
            //                                    move_cost[i, j] = 1000000;
            //                                    break;
            //                                }

            //                            case "宇宙":
            //                                {
            //                                    if (is_adaptable_in_space)
            //                                    {
            //                                        move_cost[i, j] = TerrainMoveCost(i, j);
            //                                        var loopTo15 = Information.UBound(adopted_terrain);
            //                                        for (k = 1; k <= loopTo15; k++)
            //                                        {
            //                                            if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
            //                                            {
            //                                                move_cost[i, j] = GeneralLib.MinLng(move_cost[i, j], 2);
            //                                                break;
            //                                            }
            //                                        }
            //                                    }
            //                                    else
            //                                    {
            //                                        move_cost[i, j] = 1000000;
            //                                    }

            //                                    break;
            //                                }
            //                        }
            //                    }
            //                }

            //                break;
            //            }

            //        case "水中":
            //            {
            //                var loopTo16 = x2;
            //                for (i = x1; i <= loopTo16; i++)
            //                {
            //                    var loopTo17 = y2;
            //                    for (j = y1; j <= loopTo17; j++)
            //                    {
            //                        switch (TerrainClass(i, j) ?? "")
            //                        {
            //                            case "陸":
            //                            case "屋内":
            //                            case "月面":
            //                                {
            //                                    if (is_trans_available_on_ground)
            //                                    {
            //                                        move_cost[i, j] = TerrainMoveCost(i, j);
            //                                        var loopTo18 = Information.UBound(adopted_terrain);
            //                                        for (k = 1; k <= loopTo18; k++)
            //                                        {
            //                                            if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
            //                                            {
            //                                                move_cost[i, j] = GeneralLib.MinLng(move_cost[i, j], 2);
            //                                                break;
            //                                            }
            //                                        }
            //                                    }
            //                                    else
            //                                    {
            //                                        move_cost[i, j] = 1000000;
            //                                    }

            //                                    break;
            //                                }

            //                            case "水":
            //                                {
            //                                    if (is_trans_available_in_water)
            //                                    {
            //                                        move_cost[i, j] = 2;
            //                                    }
            //                                    else
            //                                    {
            //                                        move_cost[i, j] = TerrainMoveCost(i, j);
            //                                        var loopTo19 = Information.UBound(adopted_terrain);
            //                                        for (k = 1; k <= loopTo19; k++)
            //                                        {
            //                                            if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
            //                                            {
            //                                                move_cost[i, j] = GeneralLib.MinLng(move_cost[i, j], 2);
            //                                                break;
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }

            //                            case "深水":
            //                                {
            //                                    if (is_trans_available_in_water)
            //                                    {
            //                                        move_cost[i, j] = 2;
            //                                    }
            //                                    else if (is_swimable)
            //                                    {
            //                                        move_cost[i, j] = TerrainMoveCost(i, j);
            //                                    }
            //                                    else
            //                                    {
            //                                        move_cost[i, j] = 1000000;
            //                                    }

            //                                    break;
            //                                }

            //                            case "空":
            //                                {
            //                                    move_cost[i, j] = 1000000;
            //                                    break;
            //                                }

            //                            case "宇宙":
            //                                {
            //                                    if (is_adaptable_in_space)
            //                                    {
            //                                        move_cost[i, j] = TerrainMoveCost(i, j);
            //                                        var loopTo20 = Information.UBound(adopted_terrain);
            //                                        for (k = 1; k <= loopTo20; k++)
            //                                        {
            //                                            if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
            //                                            {
            //                                                move_cost[i, j] = GeneralLib.MinLng(move_cost[i, j], 2);
            //                                                break;
            //                                            }
            //                                        }
            //                                    }
            //                                    else
            //                                    {
            //                                        move_cost[i, j] = 1000000;
            //                                    }

            //                                    break;
            //                                }
            //                        }
            //                    }
            //                }

            //                break;
            //            }

            //        case "宇宙":
            //            {
            //                var loopTo21 = x2;
            //                for (i = x1; i <= loopTo21; i++)
            //                {
            //                    var loopTo22 = y2;
            //                    for (j = y1; j <= loopTo22; j++)
            //                    {
            //                        switch (TerrainClass(i, j) ?? "")
            //                        {
            //                            case "宇宙":
            //                                {
            //                                    move_cost[i, j] = TerrainMoveCost(i, j);
            //                                    var loopTo23 = Information.UBound(adopted_terrain);
            //                                    for (k = 1; k <= loopTo23; k++)
            //                                    {
            //                                        if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
            //                                        {
            //                                            move_cost[i, j] = GeneralLib.MinLng(move_cost[i, j], 2);
            //                                            break;
            //                                        }
            //                                    }

            //                                    break;
            //                                }

            //                            case "陸":
            //                            case "屋内":
            //                                {
            //                                    if (is_trans_available_in_sky)
            //                                    {
            //                                        move_cost[i, j] = 2;
            //                                    }
            //                                    else if (is_trans_available_on_ground)
            //                                    {
            //                                        move_cost[i, j] = TerrainMoveCost(i, j);
            //                                        var loopTo24 = Information.UBound(adopted_terrain);
            //                                        for (k = 1; k <= loopTo24; k++)
            //                                        {
            //                                            if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
            //                                            {
            //                                                move_cost[i, j] = GeneralLib.MinLng(move_cost[i, j], 2);
            //                                                break;
            //                                            }
            //                                        }
            //                                    }
            //                                    else
            //                                    {
            //                                        move_cost[i, j] = 1000000;
            //                                    }

            //                                    break;
            //                                }

            //                            case "月面":
            //                                {
            //                                    if (is_trans_available_in_moon_sky)
            //                                    {
            //                                        move_cost[i, j] = 2;
            //                                    }
            //                                    else if (is_trans_available_on_ground)
            //                                    {
            //                                        move_cost[i, j] = TerrainMoveCost(i, j);
            //                                        var loopTo25 = Information.UBound(adopted_terrain);
            //                                        for (k = 1; k <= loopTo25; k++)
            //                                        {
            //                                            if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
            //                                            {
            //                                                move_cost[i, j] = GeneralLib.MinLng(move_cost[i, j], 2);
            //                                                break;
            //                                            }
            //                                        }
            //                                    }
            //                                    else
            //                                    {
            //                                        move_cost[i, j] = 1000000;
            //                                    }

            //                                    break;
            //                                }

            //                            case "水":
            //                                {
            //                                    if (is_trans_available_in_water | is_trans_available_on_water)
            //                                    {
            //                                        move_cost[i, j] = 2;
            //                                    }
            //                                    else if (is_adaptable_in_water)
            //                                    {
            //                                        move_cost[i, j] = TerrainMoveCost(i, j);
            //                                        var loopTo26 = Information.UBound(adopted_terrain);
            //                                        for (k = 1; k <= loopTo26; k++)
            //                                        {
            //                                            if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
            //                                            {
            //                                                move_cost[i, j] = GeneralLib.MinLng(move_cost[i, j], 2);
            //                                                break;
            //                                            }
            //                                        }
            //                                    }
            //                                    else
            //                                    {
            //                                        move_cost[i, j] = 1000000;
            //                                    }

            //                                    break;
            //                                }

            //                            case "深水":
            //                                {
            //                                    if (is_trans_available_in_water | is_trans_available_on_water)
            //                                    {
            //                                        move_cost[i, j] = 2;
            //                                    }
            //                                    else if (is_swimable)
            //                                    {
            //                                        move_cost[i, j] = TerrainMoveCost(i, j);
            //                                    }
            //                                    else
            //                                    {
            //                                        move_cost[i, j] = 1000000;
            //                                    }

            //                                    break;
            //                                }

            //                            case "空":
            //                                {
            //                                    if (is_trans_available_in_sky)
            //                                    {
            //                                        move_cost[i, j] = TerrainMoveCost(i, j);
            //                                        var loopTo27 = Information.UBound(adopted_terrain);
            //                                        for (k = 1; k <= loopTo27; k++)
            //                                        {
            //                                            if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
            //                                            {
            //                                                move_cost[i, j] = GeneralLib.MinLng(move_cost[i, j], 2);
            //                                                break;
            //                                            }
            //                                        }
            //                                    }
            //                                    else
            //                                    {
            //                                        move_cost[i, j] = 1000000;
            //                                    }

            //                                    break;
            //                                }
            //                        }
            //                    }
            //                }

            //                break;
            //            }

            //        case "地中":
            //            {
            //                var loopTo28 = x2;
            //                for (i = x1; i <= loopTo28; i++)
            //                {
            //                    var loopTo29 = y2;
            //                    for (j = y1; j <= loopTo29; j++)
            //                    {
            //                        switch (TerrainClass(i, j) ?? "")
            //                        {
            //                            case "陸":
            //                            case "月面":
            //                                {
            //                                    move_cost[i, j] = 2;
            //                                    break;
            //                                }

            //                            default:
            //                                {
            //                                    move_cost[i, j] = 1000000;
            //                                    break;
            //                                }
            //                        }
            //                    }
            //                }

            //                break;
            //            }
            //    }

            //    // 線路移動
            //    string argfname6 = "線路移動";
            //    if (withBlock.IsFeatureAvailable(ref argfname6))
            //    {
            //        if (withBlock.Area == "地上" & !ByJump)
            //        {
            //            var loopTo30 = x2;
            //            for (i = x1; i <= loopTo30; i++)
            //            {
            //                var loopTo31 = y2;
            //                for (j = y1; j <= loopTo31; j++)
            //                {
            //                    if (TerrainName(i, j) == "線路")
            //                    {
            //                        move_cost[i, j] = 2;
            //                    }
            //                    else
            //                    {
            //                        move_cost[i, j] = 1000000;
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // 移動制限
            //    allowed_terrains = new string[1];
            //    string argfname7 = "移動制限";
            //    if (withBlock.IsFeatureAvailable(ref argfname7))
            //    {
            //        if (withBlock.Area != "空中" & withBlock.Area != "地中")
            //        {
            //            object argIndex6 = "移動制限";
            //            string arglist = withBlock.FeatureData(ref argIndex6);
            //            n = GeneralLib.LLength(ref arglist);
            //            allowed_terrains = new string[(n + 1)];
            //            var loopTo32 = n;
            //            for (i = 2; i <= loopTo32; i++)
            //            {
            //                object argIndex7 = "移動制限";
            //                string arglist1 = withBlock.FeatureData(ref argIndex7);
            //                allowed_terrains[i] = GeneralLib.LIndex(ref arglist1, i);
            //            }

            //            if (!ByJump)
            //            {
            //                var loopTo33 = x2;
            //                for (i = x1; i <= loopTo33; i++)
            //                {
            //                    var loopTo34 = y2;
            //                    for (j = y1; j <= loopTo34; j++)
            //                    {
            //                        var loopTo35 = n;
            //                        for (k = 2; k <= loopTo35; k++)
            //                        {
            //                            if ((TerrainName(i, j) ?? "") == (allowed_terrains[k] ?? ""))
            //                            {
            //                                break;
            //                            }
            //                        }

            //                        if (k > n)
            //                        {
            //                            move_cost[i, j] = 1000000;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // 進入不可
            //    prohibited_terrains = new string[1];
            //    string argfname8 = "進入不可";
            //    if (withBlock.IsFeatureAvailable(ref argfname8))
            //    {
            //        if (withBlock.Area != "空中" & withBlock.Area != "地中")
            //        {
            //            object argIndex8 = "進入不可";
            //            string arglist2 = withBlock.FeatureData(ref argIndex8);
            //            n = GeneralLib.LLength(ref arglist2);
            //            prohibited_terrains = new string[(n + 1)];
            //            var loopTo36 = n;
            //            for (i = 2; i <= loopTo36; i++)
            //            {
            //                object argIndex9 = "進入不可";
            //                string arglist3 = withBlock.FeatureData(ref argIndex9);
            //                prohibited_terrains[i] = GeneralLib.LIndex(ref arglist3, i);
            //            }

            //            if (!ByJump)
            //            {
            //                var loopTo37 = x2;
            //                for (i = x1; i <= loopTo37; i++)
            //                {
            //                    var loopTo38 = y2;
            //                    for (j = y1; j <= loopTo38; j++)
            //                    {
            //                        var loopTo39 = n;
            //                        for (k = 2; k <= loopTo39; k++)
            //                        {
            //                            if ((TerrainName(i, j) ?? "") == (prohibited_terrains[k] ?? ""))
            //                            {
            //                                move_cost[i, j] = 1000000;
            //                                break;
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // ホバー移動
            //    string argfname9 = "ホバー移動";
            //    if (withBlock.IsFeatureAvailable(ref argfname9))
            //    {
            //        if (move_area == "地上" | move_area == "水上")
            //        {
            //            var loopTo40 = x2;
            //            for (i = x1; i <= loopTo40; i++)
            //            {
            //                var loopTo41 = y2;
            //                for (j = y1; j <= loopTo41; j++)
            //                {
            //                    switch (TerrainName(i, j) ?? "")
            //                    {
            //                        case "砂漠":
            //                        case "雪原":
            //                            {
            //                                move_cost[i, j] = 2;
            //                                break;
            //                            }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // 透過移動
            //    string argfname10 = "透過移動";
            //    string argsptype = "透過移動";
            //    if (withBlock.IsFeatureAvailable(ref argfname10) | withBlock.IsUnderSpecialPowerEffect(ref argsptype))
            //    {
            //        var loopTo42 = x2;
            //        for (i = x1; i <= loopTo42; i++)
            //        {
            //            var loopTo43 = y2;
            //            for (j = y1; j <= loopTo43; j++)
            //                move_cost[i, j] = 2;
            //        }
            //    }

            //    // ユニットがいるため通り抜け出来ない場所をチェック
            //    string argfname13 = "すり抜け移動";
            //    string argsptype1 = "すり抜け移動";
            //    if (!withBlock.IsFeatureAvailable(ref argfname13) & !withBlock.IsUnderSpecialPowerEffect(ref argsptype1))
            //    {
            //        foreach (Unit currentU2 in SRC.UList)
            //        {
            //            u2 = currentU2;
            //            {
            //                var withBlock1 = u2;
            //                if (withBlock1.Status == "出撃")
            //                {
            //                    blocked = false;

            //                    // 敵対する場合は通り抜け不可
            //                    if (withBlock1.IsEnemy(ref u, true))
            //                    {
            //                        blocked = true;
            //                    }

            //                    // 陣営が合わない場合も通り抜け不可
            //                    switch (withBlock1.Party0 ?? "")
            //                    {
            //                        case "味方":
            //                        case "ＮＰＣ":
            //                            {
            //                                if (u.Party0 != "味方" & u.Party0 != "ＮＰＣ")
            //                                {
            //                                    blocked = true;
            //                                }

            //                                break;
            //                            }

            //                        default:
            //                            {
            //                                if ((withBlock1.Party0 ?? "") != (u.Party0 ?? ""))
            //                                {
            //                                    blocked = true;
            //                                }

            //                                break;
            //                            }
            //                    }

            //                    // 通り抜けられない場合
            //                    if (blocked)
            //                    {
            //                        move_cost[withBlock1.x, withBlock1.y] = 1000000;
            //                    }

            //                    // ＺＯＣ
            //                    if (blocked & !ByJump)
            //                    {
            //                        is_zoc = false;
            //                        zarea = 0;
            //                        string argfname12 = "ＺＯＣ";
            //                        string argoname = "ＺＯＣ";
            //                        if (withBlock1.IsFeatureAvailable(ref argfname12) | Expression.IsOptionDefined(ref argoname))
            //                        {
            //                            is_zoc = true;
            //                            zarea = 1;

            //                            // ＺＯＣ側のＺＯＣレベル
            //                            object argIndex10 = "ＺＯＣ";
            //                            n = withBlock1.FeatureLevel(ref argIndex10);
            //                            if (n == 1)
            //                                n = 10000;

            //                            // Option「ＺＯＣ」が指定されている
            //                            n = GeneralLib.MaxLng(1, n);
            //                            string argfname11 = "ＺＯＣ無効化";
            //                            if (u.IsFeatureAvailable(ref argfname11))
            //                            {
            //                                // 移動側のＺＯＣ無効化レベル
            //                                // レベル指定なし、またはLv1はLv10000として扱う
            //                                object argIndex11 = "ＺＯＣ無効化";
            //                                l = u.FeatureLevel(ref argIndex11);
            //                                if (l == 1)
            //                                    l = 10000;

            //                                // 移動側のＺＯＣ無効化レベルの方が高い場合、
            //                                // ＺＯＣ不可能
            //                                if (l >= n)
            //                                {
            //                                    is_zoc = false;
            //                                }
            //                            }

            //                            // 隣接するユニットが「隣接ユニットＺＯＣ無効化」を持っている場合
            //                            if (is_zoc)
            //                            {
            //                                for (i = -1; i <= 1; i++)
            //                                {
            //                                    var loopTo44 = Math.Abs(Math.Abs(i) - 1);
            //                                    for (j = (Math.Abs(i) - 1); j <= loopTo44; j++)
            //                                    {
            //                                        if ((i != 0 | j != 0) & withBlock1.x + i >= 1 & (withBlock1.x + i) <= MapWidth & withBlock1.y + j >= 1 & (withBlock1.y + j) <= MapHeight)
            //                                        {
            //                                            // 隣接ユニットが存在する？
            //                                            if (MapDataForUnit[(withBlock1.x + i), (withBlock1.y + j)] is object)
            //                                            {
            //                                                buf = withBlock1.Party0;
            //                                                {
            //                                                    var withBlock2 = MapDataForUnit[(withBlock1.x + i), (withBlock1.y + j)];
            //                                                    // 敵対陣営？
            //                                                    switch (withBlock2.Party0 ?? "")
            //                                                    {
            //                                                        case "味方":
            //                                                        case "ＮＰＣ":
            //                                                            {
            //                                                                if (buf == "味方" | buf == "ＮＰＣ")
            //                                                                {
            //                                                                    break;
            //                                                                }

            //                                                                break;
            //                                                            }

            //                                                        default:
            //                                                            {
            //                                                                if ((withBlock2.Party0 ?? "") == (buf ?? ""))
            //                                                                {
            //                                                                    break;
            //                                                                }

            //                                                                break;
            //                                                            }
            //                                                    }

            //                                                    object argIndex12 = "隣接ユニットＺＯＣ無効化";
            //                                                    l = withBlock2.FeatureLevel(ref argIndex12);
            //                                                    if (l == 1)
            //                                                        l = 10000;

            //                                                    // 移動側のＺＯＣ無効化レベルの方が高い場合、
            //                                                    // ＺＯＣ不可能
            //                                                    if (l >= n)
            //                                                    {
            //                                                        is_zoc = false;
            //                                                        break;
            //                                                    }
            //                                                }
            //                                            }
            //                                        }
            //                                    }
            //                                }
            //                            }
            //                        }

            //                        if (is_zoc)
            //                        {
            //                            // 特殊能力「ＺＯＣ」が指定されているなら、そのデータの2つ目の値をＺＯＣの範囲に設定
            //                            // 2つ目の値が省略されている場合は1を設定
            //                            // ＺＯＣLvが0以下の場合、オプション「ＺＯＣ」が指定されていても範囲を0に設定
            //                            object argIndex13 = "ＺＯＣ";
            //                            string arglist4 = withBlock1.FeatureData(ref argIndex13);
            //                            if (GeneralLib.LLength(ref arglist4) >= 2)
            //                            {
            //                                string localLIndex() { object argIndex1 = "ＺＯＣ"; string arglist = withBlock1.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

            //                                string localLIndex1() { object argIndex1 = "ＺＯＣ"; string arglist = withBlock1.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

            //                                zarea = GeneralLib.MaxLng(Conversions.ToInteger(localLIndex1()), 0);
            //                            }

            //                            // 相対距離＋ＺＯＣの範囲が移動力以内のとき、ＺＯＣを設定
            //                            if (((Math.Abs((u.x - withBlock1.x)) + Math.Abs((u.y - withBlock1.y))) - zarea) <= uspeed)
            //                            {
            //                                // 水平・垂直方向のみのＺＯＣかどうかを判断
            //                                is_hzoc = false;
            //                                is_vzoc = false;
            //                                object argIndex16 = "ＺＯＣ";
            //                                if (Conversions.ToBoolean(Strings.InStr(withBlock1.FeatureData(ref argIndex16), "直線")))
            //                                {
            //                                    is_hzoc = true;
            //                                    is_vzoc = true;
            //                                }
            //                                else
            //                                {
            //                                    object argIndex14 = "ＺＯＣ";
            //                                    if (Conversions.ToBoolean(Strings.InStr(withBlock1.FeatureData(ref argIndex14), "水平")))
            //                                    {
            //                                        is_hzoc = true;
            //                                    }

            //                                    object argIndex15 = "ＺＯＣ";
            //                                    if (Conversions.ToBoolean(Strings.InStr(withBlock1.FeatureData(ref argIndex15), "垂直")))
            //                                    {
            //                                        is_vzoc = true;
            //                                    }
            //                                }

            //                                if (is_hzoc | is_vzoc)
            //                                {
            //                                    var loopTo45 = zarea;
            //                                    for (i = (zarea * -1); i <= loopTo45; i++)
            //                                    {
            //                                        if (i == 0)
            //                                        {
            //                                            if (PointInZOC[withBlock1.x, withBlock1.y] < 0)
            //                                            {
            //                                                if (n > Math.Abs(PointInZOC[withBlock1.x, withBlock1.y]))
            //                                                {
            //                                                    PointInZOC[withBlock1.x, withBlock1.y] = n;
            //                                                }
            //                                            }
            //                                            else
            //                                            {
            //                                                PointInZOC[withBlock1.x, withBlock1.y] = GeneralLib.MaxLng(n, PointInZOC[withBlock1.x, withBlock1.y]);
            //                                            }
            //                                        }
            //                                        else
            //                                        {
            //                                            // 水平ＺＯＣ
            //                                            if (is_hzoc & withBlock1.x + i >= 1 & (withBlock1.x + i) <= MapWidth)
            //                                            {
            //                                                if (PointInZOC[(withBlock1.x + i), withBlock1.y] < 0)
            //                                                {
            //                                                    if (n > Math.Abs(PointInZOC[(withBlock1.x + i), withBlock1.y]))
            //                                                    {
            //                                                        PointInZOC[(withBlock1.x + i), withBlock1.y] = n;
            //                                                    }
            //                                                }
            //                                                else
            //                                                {
            //                                                    PointInZOC[(withBlock1.x + i), withBlock1.y] = GeneralLib.MaxLng(n, PointInZOC[(withBlock1.x + i), withBlock1.y]);
            //                                                }
            //                                            }
            //                                            // 垂直ＺＯＣ
            //                                            if (is_vzoc & withBlock1.y + i >= 1 & (withBlock1.y + i) <= MapHeight)
            //                                            {
            //                                                if (PointInZOC[withBlock1.x, (withBlock1.y + i)] < 0)
            //                                                {
            //                                                    if (n > Math.Abs(PointInZOC[withBlock1.x, (withBlock1.y + i)]))
            //                                                    {
            //                                                        PointInZOC[withBlock1.x, (withBlock1.y + i)] = n;
            //                                                    }
            //                                                }
            //                                                else
            //                                                {
            //                                                    PointInZOC[withBlock1.x, (withBlock1.y + i)] = GeneralLib.MaxLng(n, PointInZOC[withBlock1.x, (withBlock1.y + i)]);
            //                                                }
            //                                            }
            //                                        }
            //                                    }
            //                                }
            //                                else
            //                                {
            //                                    // 全方位ＺＯＣ
            //                                    var loopTo46 = zarea;
            //                                    for (i = (zarea * -1); i <= loopTo46; i++)
            //                                    {
            //                                        var loopTo47 = Math.Abs((Math.Abs(i) - zarea));
            //                                        for (j = (Math.Abs(i) - zarea); j <= loopTo47; j++)
            //                                        {
            //                                            if (withBlock1.x + i >= 1 & (withBlock1.x + i) <= MapWidth & withBlock1.y + j >= 1 & (withBlock1.y + j) <= MapHeight)
            //                                            {
            //                                                if (PointInZOC[(withBlock1.x + i), (withBlock1.y + j)] < 0)
            //                                                {
            //                                                    if (n > Math.Abs(PointInZOC[(withBlock1.x + i), (withBlock1.y + j)]))
            //                                                    {
            //                                                        PointInZOC[(withBlock1.x + i), (withBlock1.y + j)] = n;
            //                                                    }
            //                                                }
            //                                                else
            //                                                {
            //                                                    PointInZOC[(withBlock1.x + i), (withBlock1.y + j)] = GeneralLib.MaxLng(n, PointInZOC[(withBlock1.x + i), (withBlock1.y + j)]);
            //                                                }
            //                                            }
            //                                        }
            //                                    }
            //                                }
            //                            }
            //                        }
            //                    }
            //                    // 「広域ＺＯＣ無効化」を所持している場合の処理
            //                    else if (((Math.Abs((u.x - withBlock1.x)) + Math.Abs((u.y - withBlock1.y))) - zarea) <= uspeed)
            //                    {
            //                        // レベル指定なし、またはLv1はLv10000として扱う
            //                        object argIndex17 = "広域ＺＯＣ無効化";
            //                        l = withBlock1.FeatureLevel(ref argIndex17);
            //                        if (l == 1)
            //                            l = 10000;
            //                        if (l > 0)
            //                        {
            //                            string localLIndex2() { object argIndex1 = "広域ＺＯＣ無効化"; string arglist = withBlock1.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

            //                            int localStrToLng() { string argexpr = hs1020e5bbaf214f5a820fb8d152076551(); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

            //                            n = GeneralLib.MaxLng(localStrToLng(), 1);
            //                            var loopTo48 = n;
            //                            for (i = (n * -1); i <= loopTo48; i++)
            //                            {
            //                                var loopTo49 = Math.Abs((Math.Abs(i) - n));
            //                                for (j = (Math.Abs(i) - n); j <= loopTo49; j++)
            //                                {
            //                                    if (withBlock1.x + i >= 1 & (withBlock1.x + i) <= MapWidth & withBlock1.y + j >= 1 & (withBlock1.y + j) <= MapHeight)
            //                                    {
            //                                        PointInZOC[(withBlock1.x + i), (withBlock1.y + j)] = PointInZOC[(withBlock1.x + i), (withBlock1.y + j)] - l;
            //                                    }
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // 移動停止地形はＺＯＣして扱う
            //    if (!ByJump)
            //    {
            //        {
            //            var withBlock3 = SRC.TDList;
            //            var loopTo50 = x2;
            //            for (i = x1; i <= loopTo50; i++)
            //            {
            //                var loopTo51 = y2;
            //                for (j = y1; j <= loopTo51; j++)
            //                {
            //                    // MOD START 240a
            //                    // If .IsFeatureAvailable(MapData(i, j, 0), "移動停止") Then
            //                    // PointInZOC(i, j) = 20000
            //                    // End If
            //                    if (TerrainHasMoveStop(i, j))
            //                    {
            //                        PointInZOC[i, j] = 20000;
            //                    }
            //                    // MOD  END  240a
            //                }
            //            }
            //        }
            //    }

            //    // マップ上の各地点に到達するのに必要な移動力を計算する

            //    // まず移動コスト計算用の配列を初期化
            //    var loopTo52 = (MapWidth + 1);
            //    for (i = 0; i <= loopTo52; i++)
            //    {
            //        var loopTo53 = (MapHeight + 1);
            //        for (j = 0; j <= loopTo53; j++)
            //            TotalMoveCost[i, j] = 1000000;
            //    }

            //    // 現在いる場所は移動する必要がないため、必要移動力が0
            //    TotalMoveCost[withBlock.x, withBlock.y] = 0;

            //    // 必要移動力の計算
            //    var loopTo54 = uspeed;
            //    for (i = 1; i <= loopTo54; i++)
            //    {
            //        // 現在の必要移動力を保存
            //        var loopTo55 = GeneralLib.MinLng(withBlock.x + i + 1, MapWidth + 1);
            //        for (j = GeneralLib.MaxLng(0, withBlock.x - i - 1); j <= loopTo55; j++)
            //        {
            //            var loopTo56 = GeneralLib.MinLng(withBlock.y + i + 1, MapHeight + 1);
            //            for (k = GeneralLib.MaxLng(0, withBlock.y - i - 1); k <= loopTo56; k++)
            //                cur_cost[j, k] = TotalMoveCost[j, k];
            //        }

            //        var loopTo57 = GeneralLib.MinLng(withBlock.x + i, MapWidth);
            //        for (j = GeneralLib.MaxLng(1, withBlock.x - i); j <= loopTo57; j++)
            //        {
            //            var loopTo58 = GeneralLib.MinLng(withBlock.y + i, MapHeight);
            //            for (k = GeneralLib.MaxLng(1, withBlock.y - i); k <= loopTo58; k++)
            //            {
            //                // 隣接する地点と比較して最も低い必要移動力を求める
            //                tmp = cur_cost[j, k];
            //                if (i > 1)
            //                {
            //                    {
            //                        var withBlock4 = SRC.TDList;
            //                        tmp = GeneralLib.MinLng(tmp, Operators.AddObject(cur_cost[j - 1, k], Interaction.IIf(PointInZOC[j - 1, k] > 0, 10000, 0)));
            //                        tmp = GeneralLib.MinLng(tmp, Operators.AddObject(cur_cost[j + 1, k], Interaction.IIf(PointInZOC[j + 1, k] > 0, 10000, 0)));
            //                        tmp = GeneralLib.MinLng(tmp, Operators.AddObject(cur_cost[j, k - 1], Interaction.IIf(PointInZOC[j, k - 1] > 0, 10000, 0)));
            //                        tmp = GeneralLib.MinLng(tmp, Operators.AddObject(cur_cost[j, k + 1], Interaction.IIf(PointInZOC[j, k + 1] > 0, 10000, 0)));
            //                    }
            //                }
            //                else
            //                {
            //                    tmp = GeneralLib.MinLng(tmp, cur_cost[j - 1, k]);
            //                    tmp = GeneralLib.MinLng(tmp, cur_cost[j + 1, k]);
            //                    tmp = GeneralLib.MinLng(tmp, cur_cost[j, k - 1]);
            //                    tmp = GeneralLib.MinLng(tmp, cur_cost[j, k + 1]);
            //                }
            //                // 地形に進入するのに必要な移動力を加算
            //                tmp = tmp + move_cost[j, k];
            //                // 前回の値とどちらが低い？
            //                TotalMoveCost[j, k] = GeneralLib.MinLng(tmp, cur_cost[j, k]);
            //            }
            //        }
            //    }

            //    // 算出された必要移動力を元に進入可能か判定
            //    var loopTo59 = MapWidth;
            //    for (i = 1; i <= loopTo59; i++)
            //    {
            //        var loopTo60 = MapHeight;
            //        for (j = 1; j <= loopTo60; j++)
            //        {
            //            MaskData[i, j] = true;

            //            // 必要移動力が移動力以内？
            //            if (TotalMoveCost[i, j] > uspeed)
            //            {
            //                goto NextLoop;
            //            }

            //            u2 = MapDataForUnit[i, j];

            //            // ユニットが存在？
            //            if (u2 is null)
            //            {
            //                MaskData[i, j] = false;
            //                goto NextLoop;
            //            }

            //            // 合体＆着艦するのは味方のみ
            //            if (withBlock.Party0 != "味方")
            //            {
            //                goto NextLoop;
            //            }

            //            switch (u2.Party0 ?? "")
            //            {
            //                case "味方":
            //                    {
            //                        string argfname17 = "母艦";
            //                        string argfname18 = "合体";
            //                        string argfname19 = "合体";
            //                        if (u2.IsFeatureAvailable(ref argfname17))
            //                        {
            //                            // 母艦に着艦？
            //                            string argfname15 = "母艦";
            //                            if (!withBlock.IsFeatureAvailable(ref argfname15) & u2.Area != "地中")
            //                            {
            //                                string argfname14 = "格納不可";
            //                                if (!withBlock.IsFeatureAvailable(ref argfname14))
            //                                {
            //                                    MaskData[i, j] = false;
            //                                }
            //                            }
            //                        }
            //                        else if (withBlock.IsFeatureAvailable(ref argfname18) & u2.IsFeatureAvailable(ref argfname19))
            //                        {
            //                            // ２体合体？
            //                            MaskData[i, j] = true;
            //                            var loopTo61 = withBlock.CountFeature();
            //                            for (k = 1; k <= loopTo61; k++)
            //                            {
            //                                string localFeature() { object argIndex1 = k; var ret = withBlock.Feature(ref argIndex1); return ret; }

            //                                string localFeatureName() { object argIndex1 = k; var ret = withBlock.FeatureName(ref argIndex1); return ret; }

            //                                if (localFeature() == "合体" & !string.IsNullOrEmpty(localFeatureName()))
            //                                {
            //                                    object argIndex18 = k;
            //                                    buf = withBlock.FeatureData(ref argIndex18);
            //                                    bool localIsDefined() { object argIndex1 = GeneralLib.LIndex(ref buf, 2); var ret = SRC.UList.IsDefined(ref argIndex1); return ret; }

            //                                    bool localIsDefined1() { object argIndex1 = GeneralLib.LIndex(ref buf, 3); var ret = SRC.UList.IsDefined(ref argIndex1); return ret; }

            //                                    if (GeneralLib.LLength(ref buf) == 3 & localIsDefined() & localIsDefined1())
            //                                    {
            //                                        object argIndex20 = GeneralLib.LIndex(ref buf, 2);
            //                                        {
            //                                            var withBlock5 = SRC.UList.Item(ref argIndex20);
            //                                            object argIndex19 = "行動不能";
            //                                            if (withBlock5.IsConditionSatisfied(ref argIndex19))
            //                                            {
            //                                                break;
            //                                            }

            //                                            if (withBlock5.Status == "破棄")
            //                                            {
            //                                                break;
            //                                            }
            //                                        }

            //                                        Unit localItem() { object argIndex1 = GeneralLib.LIndex(ref buf, 3); var ret = SRC.UList.Item(ref argIndex1); return ret; }

            //                                        string argfname16 = "合体制限";
            //                                        if ((u2.Name ?? "") == (GeneralLib.LIndex(ref buf, 3) ?? ""))
            //                                        {
            //                                            MaskData[i, j] = false;
            //                                            break;
            //                                        }
            //                                        else if ((u2.Name ?? "") == (localItem().CurrentForm().Name ?? "") & !u2.IsFeatureAvailable(ref argfname16))
            //                                        {
            //                                            MaskData[i, j] = false;
            //                                            break;
            //                                        }
            //                                    }
            //                                }
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "ＮＰＣ":
            //                    {
            //                        string argfname21 = "合体";
            //                        string argfname22 = "合体";
            //                        if (withBlock.IsFeatureAvailable(ref argfname21) & u2.IsFeatureAvailable(ref argfname22))
            //                        {
            //                            // ２体合体？
            //                            MaskData[i, j] = true;
            //                            var loopTo62 = withBlock.CountFeature();
            //                            for (k = 1; k <= loopTo62; k++)
            //                            {
            //                                object argIndex24 = k;
            //                                if (withBlock.Feature(ref argIndex24) == "合体")
            //                                {
            //                                    object argIndex21 = k;
            //                                    buf = withBlock.FeatureData(ref argIndex21);
            //                                    bool localIsDefined2() { object argIndex1 = GeneralLib.LIndex(ref buf, 2); var ret = SRC.UList.IsDefined(ref argIndex1); return ret; }

            //                                    bool localIsDefined3() { object argIndex1 = GeneralLib.LIndex(ref buf, 3); var ret = SRC.UList.IsDefined(ref argIndex1); return ret; }

            //                                    if (GeneralLib.LLength(ref buf) == 3 & localIsDefined2() & localIsDefined3())
            //                                    {
            //                                        object argIndex23 = GeneralLib.LIndex(ref buf, 2);
            //                                        {
            //                                            var withBlock6 = SRC.UList.Item(ref argIndex23);
            //                                            object argIndex22 = "行動不能";
            //                                            if (withBlock6.IsConditionSatisfied(ref argIndex22))
            //                                            {
            //                                                break;
            //                                            }

            //                                            if (withBlock6.Status == "破棄")
            //                                            {
            //                                                break;
            //                                            }
            //                                        }

            //                                        Unit localItem1() { object argIndex1 = GeneralLib.LIndex(ref buf, 3); var ret = SRC.UList.Item(ref argIndex1); return ret; }

            //                                        string argfname20 = "合体制限";
            //                                        if ((u2.Name ?? "") == (GeneralLib.LIndex(ref buf, 3) ?? ""))
            //                                        {
            //                                            MaskData[i, j] = false;
            //                                            break;
            //                                        }
            //                                        else if ((u2.Name ?? "") == (localItem1().CurrentForm().Name ?? "") & !u2.IsFeatureAvailable(ref argfname20))
            //                                        {
            //                                            MaskData[i, j] = false;
            //                                            break;
            //                                        }
            //                                    }
            //                                }
            //                            }
            //                        }

            //                        break;
            //                    }
            //            }

            //        NextLoop:
            //            ;
            //        }
            //    }

            //    // ジャンプ＆透過移動先は進入可能？
            //    string argfname23 = "透過移動";
            //    string argsptype2 = "透過移動";
            //    if (ByJump | withBlock.IsFeatureAvailable(ref argfname23) | withBlock.IsUnderSpecialPowerEffect(ref argsptype2))
            //    {
            //        var loopTo63 = x2;
            //        for (i = x1; i <= loopTo63; i++)
            //        {
            //            var loopTo64 = y2;
            //            for (j = y1; j <= loopTo64; j++)
            //            {
            //                if (MaskData[i, j])
            //                {
            //                    goto NextLoop2;
            //                }

            //                // ユニットがいる地形に進入出来るということは
            //                // 合体or着艦可能ということなので地形は無視
            //                if (MapDataForUnit[i, j] is object)
            //                {
            //                    goto NextLoop2;
            //                }

            //                switch (withBlock.Area ?? "")
            //                {
            //                    case "地上":
            //                        {
            //                            switch (TerrainClass(i, j) ?? "")
            //                            {
            //                                case "空":
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                        break;
            //                                    }

            //                                case "水":
            //                                    {
            //                                        if (!is_adaptable_in_water & !is_trans_available_on_water & !is_trans_available_in_water)
            //                                        {
            //                                            MaskData[i, j] = true;
            //                                        }

            //                                        break;
            //                                    }

            //                                case "深水":
            //                                    {
            //                                        if (!is_trans_available_on_water & !is_trans_available_in_water)
            //                                        {
            //                                            MaskData[i, j] = true;
            //                                        }

            //                                        break;
            //                                    }

            //                                case "宇宙":
            //                                    {
            //                                        if (!is_adaptable_in_space)
            //                                        {
            //                                            MaskData[i, j] = true;
            //                                        }

            //                                        break;
            //                                    }
            //                            }

            //                            break;
            //                        }

            //                    case "水上":
            //                        {
            //                            switch (TerrainClass(i, j) ?? "")
            //                            {
            //                                case "空":
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                        break;
            //                                    }

            //                                case "宇宙":
            //                                    {
            //                                        if (!is_adaptable_in_space)
            //                                        {
            //                                            MaskData[i, j] = true;
            //                                        }

            //                                        break;
            //                                    }
            //                            }

            //                            break;
            //                        }

            //                    case "水中":
            //                        {
            //                            switch (TerrainClass(i, j) ?? "")
            //                            {
            //                                case "空":
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                        break;
            //                                    }

            //                                case "深水":
            //                                    {
            //                                        if (!is_trans_available_on_water & !is_trans_available_in_water)
            //                                        {
            //                                            MaskData[i, j] = true;
            //                                        }

            //                                        break;
            //                                    }

            //                                case "宇宙":
            //                                    {
            //                                        if (!is_adaptable_in_space)
            //                                        {
            //                                            MaskData[i, j] = true;
            //                                        }

            //                                        break;
            //                                    }
            //                            }

            //                            break;
            //                        }

            //                    case "空中":
            //                        {
            //                            switch (TerrainClass(i, j) ?? "")
            //                            {
            //                                case "空":
            //                                    {
            //                                        if (TerrainMoveCost(i, j) > 100)
            //                                        {
            //                                            MaskData[i, j] = true;
            //                                        }

            //                                        break;
            //                                    }

            //                                case "宇宙":
            //                                    {
            //                                        if (!is_adaptable_in_space)
            //                                        {
            //                                            MaskData[i, j] = true;
            //                                        }

            //                                        break;
            //                                    }
            //                            }

            //                            break;
            //                        }

            //                    case "地中":
            //                        {
            //                            if (TerrainClass(i, j) != "陸")
            //                            {
            //                                MaskData[i, j] = true;
            //                            }

            //                            break;
            //                        }

            //                    case "宇宙":
            //                        {
            //                            switch (TerrainClass(i, j) ?? "")
            //                            {
            //                                case "陸":
            //                                case "屋内":
            //                                    {
            //                                        if (!is_trans_available_in_sky & !is_trans_available_on_ground)
            //                                        {
            //                                            MaskData[i, j] = true;
            //                                        }

            //                                        break;
            //                                    }

            //                                case "空":
            //                                    {
            //                                        if (!is_trans_available_in_sky | TerrainMoveCost(i, j) > 10)
            //                                        {
            //                                            MaskData[i, j] = true;
            //                                        }

            //                                        break;
            //                                    }

            //                                case "水":
            //                                    {
            //                                        if (!is_trans_available_in_water & !is_trans_available_on_water & !is_adaptable_in_water)
            //                                        {
            //                                            MaskData[i, j] = true;
            //                                        }

            //                                        break;
            //                                    }

            //                                case "深水":
            //                                    {
            //                                        if (!is_trans_available_on_water & !is_trans_available_in_water)
            //                                        {
            //                                            MaskData[i, j] = true;
            //                                        }

            //                                        break;
            //                                    }
            //                            }

            //                            break;
            //                        }
            //                }

            //                // 移動制限
            //                if (Information.UBound(allowed_terrains) > 0)
            //                {
            //                    var loopTo65 = Information.UBound(allowed_terrains);
            //                    for (k = 2; k <= loopTo65; k++)
            //                    {
            //                        if ((TerrainName(i, j) ?? "") == (allowed_terrains[k] ?? ""))
            //                        {
            //                            break;
            //                        }
            //                    }

            //                    if (k > Information.UBound(allowed_terrains))
            //                    {
            //                        MaskData[i, j] = true;
            //                    }
            //                }

            //                // 進入不可
            //                var loopTo66 = Information.UBound(prohibited_terrains);
            //                for (k = 2; k <= loopTo66; k++)
            //                {
            //                    if ((TerrainName(i, j) ?? "") == (prohibited_terrains[k] ?? ""))
            //                    {
            //                        MaskData[i, j] = true;
            //                        break;
            //                    }
            //                }

            //            NextLoop2:
            //                ;
            //            }
            //        }
            //    }

            //    // 現在いる場所は常に進入可能
            //    MaskData[withBlock.x, withBlock.y] = false;
            //}
        }

        // ユニット u がテレポートして移動できる範囲を選択
        // 最大距離 lv を指定可能。(省略時は移動力＋テレポートレベル)
        public void AreaInTeleport(ref Unit u, int lv = 0)
        {
            throw new NotImplementedException();
            //bool is_trans_available_on_ground;
            //bool is_trans_available_in_water;
            //var is_trans_available_on_water = default(bool);
            //bool is_trans_available_in_sky;
            //var is_adaptable_in_water = default(bool);
            //var is_adaptable_in_space = default(bool);
            //string[] allowed_terrains;
            //string[] prohibited_terrains;
            //int n, j, i, k, r;
            //string buf;
            //Unit u2;
            //// 移動能力の可否を調べておく
            //string argarea_name = "陸";
            //is_trans_available_on_ground = u.IsTransAvailable(ref argarea_name) & u.get_Adaption(2) != 0;
            //string argarea_name1 = "水";
            //is_trans_available_in_water = u.IsTransAvailable(ref argarea_name1) & u.get_Adaption(3) != 0;
            //string argarea_name2 = "空";
            //is_trans_available_in_sky = u.IsTransAvailable(ref argarea_name2) & u.get_Adaption(1) != 0;
            //string argfname = "水中移動";
            //if (Strings.Mid(u.Data.Adaption, 3, 1) != "-" | u.IsFeatureAvailable(ref argfname))
            //{
            //    is_adaptable_in_water = true;
            //}

            //string argfname1 = "宇宙移動";
            //if (Strings.Mid(u.Data.Adaption, 4, 1) != "-" | u.IsFeatureAvailable(ref argfname1))
            //{
            //    is_adaptable_in_space = true;
            //}

            //string argfname2 = "水上移動";
            //string argfname3 = "ホバー移動";
            //if (u.IsFeatureAvailable(ref argfname2) | u.IsFeatureAvailable(ref argfname3))
            //{
            //    is_trans_available_on_water = true;
            //}

            //// 移動制限
            //allowed_terrains = new string[1];
            //string argfname4 = "移動制限";
            //if (u.IsFeatureAvailable(ref argfname4))
            //{
            //    if (u.Area != "空中" & u.Area != "地中")
            //    {
            //        object argIndex1 = "移動制限";
            //        string arglist = u.FeatureData(ref argIndex1);
            //        n = GeneralLib.LLength(ref arglist);
            //        allowed_terrains = new string[(n + 1)];
            //        var loopTo = n;
            //        for (i = 2; i <= loopTo; i++)
            //        {
            //            object argIndex2 = "移動制限";
            //            string arglist1 = u.FeatureData(ref argIndex2);
            //            allowed_terrains[i] = GeneralLib.LIndex(ref arglist1, i);
            //        }
            //    }
            //}

            //// 進入不可
            //prohibited_terrains = new string[1];
            //string argfname5 = "進入不可";
            //if (u.IsFeatureAvailable(ref argfname5))
            //{
            //    if (u.Area != "空中" & u.Area != "地中")
            //    {
            //        object argIndex3 = "進入不可";
            //        string arglist2 = u.FeatureData(ref argIndex3);
            //        n = GeneralLib.LLength(ref arglist2);
            //        prohibited_terrains = new string[(n + 1)];
            //        var loopTo1 = n;
            //        for (i = 2; i <= loopTo1; i++)
            //        {
            //            object argIndex4 = "進入不可";
            //            string arglist3 = u.FeatureData(ref argIndex4);
            //            prohibited_terrains[i] = GeneralLib.LIndex(ref arglist3, i);
            //        }
            //    }
            //}

            //// テレポートによる移動距離を算出
            //if (lv > 0)
            //{
            //    r = lv;
            //}
            //else
            //{
            //    object argIndex5 = "テレポート";
            //    r = (u.Speed + u.FeatureLevel(ref argIndex5));
            //}

            //object argIndex6 = "移動不能";
            //if (u.IsConditionSatisfied(ref argIndex6))
            //{
            //    r = 0;
            //}

            //// 選択解除
            //var loopTo2 = MapWidth;
            //for (i = 1; i <= loopTo2; i++)
            //{
            //    var loopTo3 = MapHeight;
            //    for (j = 1; j <= loopTo3; j++)
            //        MaskData[i, j] = true;
            //}

            //// 移動可能な地点を調べる
            //var loopTo4 = GeneralLib.MinLng(MapWidth, u.x + r);
            //for (i = GeneralLib.MaxLng(1, u.x - r); i <= loopTo4; i++)
            //{
            //    var loopTo5 = GeneralLib.MinLng(MapHeight, u.y + r);
            //    for (j = GeneralLib.MaxLng(1, u.y - r); j <= loopTo5; j++)
            //    {
            //        // 移動範囲内？
            //        if ((Math.Abs((u.x - i)) + Math.Abs((u.y - j))) > r)
            //        {
            //            goto NextLoop;
            //        }

            //        u2 = MapDataForUnit[i, j];
            //        if (u2 is null)
            //        {
            //            // ユニットがいない地点は地形から進入可能かチェック
            //            MaskData[i, j] = false;
            //            switch (u.Area ?? "")
            //            {
            //                case "地上":
            //                    {
            //                        switch (TerrainClass(i, j) ?? "")
            //                        {
            //                            case "空":
            //                                {
            //                                    MaskData[i, j] = true;
            //                                    break;
            //                                }

            //                            case "水":
            //                                {
            //                                    if (!is_adaptable_in_water & !is_trans_available_on_water & !is_trans_available_in_water)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }

            //                            case "深水":
            //                                {
            //                                    if (!is_trans_available_on_water & !is_trans_available_in_water)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }

            //                            case "宇宙":
            //                                {
            //                                    if (!is_adaptable_in_space)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        break;
            //                    }

            //                case "水中":
            //                    {
            //                        switch (TerrainClass(i, j) ?? "")
            //                        {
            //                            case "空":
            //                                {
            //                                    MaskData[i, j] = true;
            //                                    break;
            //                                }

            //                            case "深水":
            //                                {
            //                                    if (!is_trans_available_on_water & !is_trans_available_in_water)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }

            //                            case "宇宙":
            //                                {
            //                                    if (!is_adaptable_in_space)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        break;
            //                    }

            //                case "空中":
            //                    {
            //                        switch (TerrainClass(i, j) ?? "")
            //                        {
            //                            case "空":
            //                                {
            //                                    if (TerrainMoveCost(i, j) > 100)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }

            //                            case "宇宙":
            //                                {
            //                                    if (!is_adaptable_in_space)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        break;
            //                    }

            //                case "地中":
            //                    {
            //                        if (TerrainClass(i, j) != "陸")
            //                        {
            //                            MaskData[i, j] = true;
            //                        }

            //                        break;
            //                    }

            //                case "宇宙":
            //                    {
            //                        switch (TerrainClass(i, j) ?? "")
            //                        {
            //                            case "陸":
            //                            case "屋内":
            //                                {
            //                                    if (!is_trans_available_in_sky & !is_trans_available_on_ground)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }

            //                            case "空":
            //                                {
            //                                    if (!is_trans_available_in_sky | TerrainMoveCost(i, j) > 100)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }

            //                            case "水":
            //                                {
            //                                    if (!is_trans_available_in_water & !is_trans_available_on_water & !is_adaptable_in_water)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }

            //                            case "深水":
            //                                {
            //                                    if (!is_trans_available_on_water & !is_trans_available_in_water)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        break;
            //                    }
            //            }

            //            // 移動制限
            //            if (Information.UBound(allowed_terrains) > 0)
            //            {
            //                var loopTo6 = Information.UBound(allowed_terrains);
            //                for (k = 2; k <= loopTo6; k++)
            //                {
            //                    if ((TerrainName(i, j) ?? "") == (allowed_terrains[k] ?? ""))
            //                    {
            //                        break;
            //                    }
            //                }

            //                if (k > Information.UBound(allowed_terrains))
            //                {
            //                    MaskData[i, j] = true;
            //                }
            //            }

            //            // 進入不可
            //            var loopTo7 = Information.UBound(prohibited_terrains);
            //            for (k = 2; k <= loopTo7; k++)
            //            {
            //                if ((TerrainName(i, j) ?? "") == (prohibited_terrains[k] ?? ""))
            //                {
            //                    MaskData[i, j] = true;
            //                    break;
            //                }
            //            }

            //            goto NextLoop;
            //        }

            //        // 合体＆着艦するのは味方のみ
            //        if (u.Party0 != "味方")
            //        {
            //            goto NextLoop;
            //        }

            //        switch (u2.Party0 ?? "")
            //        {
            //            case "味方":
            //                {
            //                    string argfname10 = "母艦";
            //                    string argfname11 = "合体";
            //                    string argfname12 = "合体";
            //                    if (u2.IsFeatureAvailable(ref argfname10))
            //                    {
            //                        // 母艦に着艦？
            //                        string argfname6 = "母艦";
            //                        string argfname7 = "格納不可";
            //                        string argfname8 = "母艦";
            //                        if (!u.IsFeatureAvailable(ref argfname6) & !u.IsFeatureAvailable(ref argfname7) & u2.Area != "地中" & !u2.IsDisabled(ref argfname8))
            //                        {
            //                            MaskData[i, j] = false;
            //                        }
            //                    }
            //                    else if (u.IsFeatureAvailable(ref argfname11) & u2.IsFeatureAvailable(ref argfname12))
            //                    {
            //                        // ２体合体？
            //                        MaskData[i, j] = true;
            //                        var loopTo8 = u.CountFeature();
            //                        for (k = 1; k <= loopTo8; k++)
            //                        {
            //                            string localFeature() { object argIndex1 = k; var ret = u.Feature(ref argIndex1); return ret; }

            //                            string localFeatureName() { object argIndex1 = k; var ret = u.FeatureName(ref argIndex1); return ret; }

            //                            if (localFeature() == "合体" & !string.IsNullOrEmpty(localFeatureName()))
            //                            {
            //                                object argIndex7 = k;
            //                                buf = u.FeatureData(ref argIndex7);
            //                                bool localIsDefined() { object argIndex1 = GeneralLib.LIndex(ref buf, 2); var ret = SRC.UList.IsDefined(ref argIndex1); return ret; }

            //                                bool localIsDefined1() { object argIndex1 = GeneralLib.LIndex(ref buf, 3); var ret = SRC.UList.IsDefined(ref argIndex1); return ret; }

            //                                if (GeneralLib.LLength(ref buf) == 3 & localIsDefined() & localIsDefined1())
            //                                {
            //                                    object argIndex9 = GeneralLib.LIndex(ref buf, 2);
            //                                    {
            //                                        var withBlock = SRC.UList.Item(ref argIndex9);
            //                                        object argIndex8 = "行動不能";
            //                                        if (withBlock.IsConditionSatisfied(ref argIndex8))
            //                                        {
            //                                            break;
            //                                        }

            //                                        if (withBlock.Status == "破棄")
            //                                        {
            //                                            break;
            //                                        }
            //                                    }

            //                                    Unit localItem() { object argIndex1 = GeneralLib.LIndex(ref buf, 3); var ret = SRC.UList.Item(ref argIndex1); return ret; }

            //                                    string argfname9 = "合体制限";
            //                                    if ((u2.Name ?? "") == (GeneralLib.LIndex(ref buf, 3) ?? ""))
            //                                    {
            //                                        MaskData[i, j] = false;
            //                                        break;
            //                                    }
            //                                    else if ((u2.Name ?? "") == (localItem().CurrentForm().Name ?? "") & !u2.IsFeatureAvailable(ref argfname9))
            //                                    {
            //                                        MaskData[i, j] = false;
            //                                        break;
            //                                    }
            //                                }
            //                            }
            //                        }
            //                    }

            //                    break;
            //                }

            //            case "ＮＰＣ":
            //                {
            //                    string argfname14 = "合体";
            //                    string argfname15 = "合体";
            //                    if (u.IsFeatureAvailable(ref argfname14) & u2.IsFeatureAvailable(ref argfname15))
            //                    {
            //                        // ２体合体？
            //                        MaskData[i, j] = true;
            //                        var loopTo9 = u.CountFeature();
            //                        for (k = 1; k <= loopTo9; k++)
            //                        {
            //                            object argIndex13 = k;
            //                            if (u.Feature(ref argIndex13) == "合体")
            //                            {
            //                                object argIndex10 = k;
            //                                buf = u.FeatureData(ref argIndex10);
            //                                bool localIsDefined2() { object argIndex1 = GeneralLib.LIndex(ref buf, 2); var ret = SRC.UList.IsDefined(ref argIndex1); return ret; }

            //                                bool localIsDefined3() { object argIndex1 = GeneralLib.LIndex(ref buf, 3); var ret = SRC.UList.IsDefined(ref argIndex1); return ret; }

            //                                if (GeneralLib.LLength(ref buf) == 3 & localIsDefined2() & localIsDefined3())
            //                                {
            //                                    object argIndex12 = GeneralLib.LIndex(ref buf, 2);
            //                                    {
            //                                        var withBlock1 = SRC.UList.Item(ref argIndex12);
            //                                        object argIndex11 = "行動不能";
            //                                        if (withBlock1.IsConditionSatisfied(ref argIndex11))
            //                                        {
            //                                            break;
            //                                        }

            //                                        if (withBlock1.Status == "破棄")
            //                                        {
            //                                            break;
            //                                        }
            //                                    }

            //                                    Unit localItem1() { object argIndex1 = GeneralLib.LIndex(ref buf, 3); var ret = SRC.UList.Item(ref argIndex1); return ret; }

            //                                    string argfname13 = "合体制限";
            //                                    if ((u2.Name ?? "") == (GeneralLib.LIndex(ref buf, 3) ?? ""))
            //                                    {
            //                                        MaskData[i, j] = false;
            //                                        break;
            //                                    }
            //                                    else if ((u2.Name ?? "") == (localItem1().CurrentForm().Name ?? "") & !u2.IsFeatureAvailable(ref argfname13))
            //                                    {
            //                                        MaskData[i, j] = false;
            //                                        break;
            //                                    }
            //                                }
            //                            }
            //                        }
            //                    }

            //                    break;
            //                }
            //        }

            //    NextLoop:
            //        ;
            //    }
            //}

            //// 現在いる場所は常に進入可能
            //MaskData[u.x, u.y] = false;
        }

        // ユニット u のＭ移武器、アビリティのターゲット座標選択用
        public void AreaInMoveAction(ref Unit u, int max_range)
        {
            throw new NotImplementedException();
            //int k, i, j, n;
            //// ADD START MARGE
            //string buf;
            //// ADD END MARGE
            //bool is_trans_available_on_ground;
            //bool is_trans_available_in_water;
            //var is_trans_available_on_water = default(bool);
            //bool is_trans_available_in_sky;
            //var is_adaptable_in_water = default(bool);
            //var is_adaptable_in_space = default(bool);
            //var is_able_to_penetrate = default(bool);
            //// ADD START MARGE
            //string[] adopted_terrain;
            //// ADD END MARGE
            //string[] allowed_terrains;
            //string[] prohibited_terrains;
            //int x1, y1;
            //int x2, y2;
            //// ADD START MARGE
            //TerrainData td;
            //// ADD END MARGE

            //// 全領域マスク
            //var loopTo = MapWidth;
            //for (i = 1; i <= loopTo; i++)
            //{
            //    var loopTo1 = MapHeight;
            //    for (j = 1; j <= loopTo1; j++)
            //        MaskData[i, j] = true;
            //}

            //// 移動能力の可否を調べておく
            //string argarea_name = "陸";
            //is_trans_available_on_ground = u.IsTransAvailable(ref argarea_name) & u.get_Adaption(2) != 0;
            //string argarea_name1 = "水";
            //is_trans_available_in_water = u.IsTransAvailable(ref argarea_name1) & u.get_Adaption(3) != 0;
            //string argarea_name2 = "空";
            //is_trans_available_in_sky = u.IsTransAvailable(ref argarea_name2) & u.get_Adaption(1) != 0;
            //string argfname = "水中移動";
            //if (Strings.Mid(u.Data.Adaption, 3, 1) != "-" | u.IsFeatureAvailable(ref argfname))
            //{
            //    is_adaptable_in_water = true;
            //}

            //string argfname1 = "宇宙移動";
            //if (Strings.Mid(u.Data.Adaption, 4, 1) != "-" | u.IsFeatureAvailable(ref argfname1))
            //{
            //    is_adaptable_in_space = true;
            //}

            //string argfname2 = "水上移動";
            //string argfname3 = "ホバー移動";
            //if (u.IsFeatureAvailable(ref argfname2) | u.IsFeatureAvailable(ref argfname3))
            //{
            //    is_trans_available_on_water = true;
            //}

            //string argfname4 = "透過移動";
            //string argsptype = "透過移動";
            //if (u.IsFeatureAvailable(ref argfname4) | u.IsUnderSpecialPowerEffect(ref argsptype))
            //{
            //    is_able_to_penetrate = true;
            //}

            //// ADD START MARGE
            //// 地形適応のある地形のリストを作成
            //adopted_terrain = new string[1];
            //string argfname5 = "地形適応";
            //if (u.IsFeatureAvailable(ref argfname5))
            //{
            //    var loopTo2 = u.CountFeature();
            //    for (i = 1; i <= loopTo2; i++)
            //    {
            //        object argIndex2 = i;
            //        if (u.Feature(ref argIndex2) == "地形適応")
            //        {
            //            object argIndex1 = i;
            //            buf = u.FeatureData(ref argIndex1);
            //            if (GeneralLib.LLength(ref buf) == 0)
            //            {
            //                string argmsg = "ユニット「" + u.Name + "」の地形適応能力に対応地形が指定されていません";
            //                GUI.ErrorMessage(ref argmsg);
            //                SRC.TerminateSRC();
            //            }

            //            n = GeneralLib.LLength(ref buf);
            //            Array.Resize(ref adopted_terrain, Information.UBound(adopted_terrain) + n);
            //            var loopTo3 = n;
            //            for (j = 2; j <= loopTo3; j++)
            //                adopted_terrain[Information.UBound(adopted_terrain) - j + 2] = GeneralLib.LIndex(ref buf, j);
            //        }
            //    }
            //}
            //// ADD END MARGE

            //// 移動制限
            //allowed_terrains = new string[1];
            //string argfname6 = "移動制限";
            //if (u.IsFeatureAvailable(ref argfname6))
            //{
            //    if (u.Area != "空中" & u.Area != "地中")
            //    {
            //        object argIndex3 = "移動制限";
            //        string arglist = u.FeatureData(ref argIndex3);
            //        n = GeneralLib.LLength(ref arglist);
            //        allowed_terrains = new string[(n + 1)];
            //        var loopTo4 = n;
            //        for (i = 2; i <= loopTo4; i++)
            //        {
            //            object argIndex4 = "移動制限";
            //            string arglist1 = u.FeatureData(ref argIndex4);
            //            allowed_terrains[i] = GeneralLib.LIndex(ref arglist1, i);
            //        }
            //    }
            //}

            //// 進入不可
            //prohibited_terrains = new string[1];
            //string argfname7 = "進入不可";
            //if (u.IsFeatureAvailable(ref argfname7))
            //{
            //    if (u.Area != "空中" & u.Area != "地中")
            //    {
            //        object argIndex5 = "進入不可";
            //        string arglist2 = u.FeatureData(ref argIndex5);
            //        n = GeneralLib.LLength(ref arglist2);
            //        prohibited_terrains = new string[(n + 1)];
            //        var loopTo5 = n;
            //        for (i = 2; i <= loopTo5; i++)
            //        {
            //            object argIndex6 = "進入不可";
            //            string arglist3 = u.FeatureData(ref argIndex6);
            //            prohibited_terrains[i] = GeneralLib.LIndex(ref arglist3, i);
            //        }
            //    }
            //}

            //// 移動範囲をチェックすべき領域
            //x1 = GeneralLib.MaxLng(1, u.x - max_range);
            //y1 = GeneralLib.MaxLng(1, u.y - max_range);
            //x2 = GeneralLib.MinLng(u.x + max_range, MapWidth);
            //y2 = GeneralLib.MinLng(u.y + max_range, MapHeight);

            //// 進入可能か判定
            //var loopTo6 = x2;
            //for (i = x1; i <= loopTo6; i++)
            //{
            //    var loopTo7 = y2;
            //    for (j = y1; j <= loopTo7; j++)
            //    {
            //        // 移動力の範囲内？
            //        if ((Math.Abs((u.x - i)) + Math.Abs((u.y - j))) > max_range)
            //        {
            //            goto NextLoop;
            //        }

            //        // ユニットが存在？
            //        if (MapDataForUnit[i, j] is object)
            //        {
            //            goto NextLoop;
            //        }

            //        // 適応あり？
            //        switch (u.Area ?? "")
            //        {
            //            case "地上":
            //                {
            //                    switch (TerrainClass(i, j) ?? "")
            //                    {
            //                        case "空":
            //                            {
            //                                goto NextLoop;
            //                                break;
            //                            }

            //                        case "水":
            //                            {
            //                                if (!is_adaptable_in_water & !is_trans_available_on_water & !is_trans_available_in_water)
            //                                {
            //                                    goto NextLoop;
            //                                }

            //                                break;
            //                            }

            //                        case "深水":
            //                            {
            //                                if (!is_trans_available_on_water & !is_trans_available_in_water)
            //                                {
            //                                    goto NextLoop;
            //                                }

            //                                break;
            //                            }

            //                        case "宇宙":
            //                            {
            //                                if (!is_adaptable_in_space)
            //                                {
            //                                    goto NextLoop;
            //                                }

            //                                break;
            //                            }
            //                    }

            //                    break;
            //                }

            //            case "水中":
            //                {
            //                    switch (TerrainClass(i, j) ?? "")
            //                    {
            //                        case "空":
            //                            {
            //                                goto NextLoop;
            //                                break;
            //                            }

            //                        case "深水":
            //                            {
            //                                if (!is_trans_available_on_water & !is_trans_available_in_water)
            //                                {
            //                                    goto NextLoop;
            //                                }

            //                                break;
            //                            }

            //                        case "宇宙":
            //                            {
            //                                if (!is_adaptable_in_space)
            //                                {
            //                                    goto NextLoop;
            //                                }

            //                                break;
            //                            }
            //                    }

            //                    break;
            //                }

            //            case "空中":
            //                {
            //                    switch (TerrainClass(i, j) ?? "")
            //                    {
            //                        case "空":
            //                            {
            //                                if (TerrainMoveCost(i, j) > 100)
            //                                {
            //                                    goto NextLoop;
            //                                }

            //                                break;
            //                            }

            //                        case "宇宙":
            //                            {
            //                                if (!is_adaptable_in_space)
            //                                {
            //                                    goto NextLoop;
            //                                }

            //                                break;
            //                            }
            //                    }

            //                    break;
            //                }

            //            case "地中":
            //                {
            //                    if (TerrainClass(i, j) != "陸")
            //                    {
            //                        goto NextLoop;
            //                    }

            //                    break;
            //                }

            //            case "宇宙":
            //                {
            //                    switch (TerrainClass(i, j) ?? "")
            //                    {
            //                        case "陸":
            //                        case "屋内":
            //                            {
            //                                if (!is_trans_available_in_sky & !is_trans_available_on_ground)
            //                                {
            //                                    goto NextLoop;
            //                                }

            //                                break;
            //                            }

            //                        case "空":
            //                            {
            //                                if (!is_trans_available_in_sky | TerrainMoveCost(i, j) > 100)
            //                                {
            //                                    goto NextLoop;
            //                                }

            //                                break;
            //                            }

            //                        case "水":
            //                            {
            //                                if (!is_trans_available_in_water & !is_trans_available_on_water & !is_adaptable_in_water)
            //                                {
            //                                    goto NextLoop;
            //                                }

            //                                break;
            //                            }

            //                        case "深水":
            //                            {
            //                                if (!is_trans_available_on_water & !is_trans_available_in_water)
            //                                {
            //                                    goto NextLoop;
            //                                }

            //                                break;
            //                            }
            //                    }

            //                    // 移動制限
            //                    if (Information.UBound(allowed_terrains) > 0)
            //                    {
            //                        var loopTo8 = Information.UBound(allowed_terrains);
            //                        for (k = 2; k <= loopTo8; k++)
            //                        {
            //                            if ((TerrainName(i, j) ?? "") == (allowed_terrains[k] ?? ""))
            //                            {
            //                                break;
            //                            }
            //                        }

            //                        if (k > Information.UBound(allowed_terrains))
            //                        {
            //                            goto NextLoop;
            //                        }
            //                    }

            //                    // 進入不可
            //                    var loopTo9 = Information.UBound(prohibited_terrains);
            //                    for (k = 2; k <= loopTo9; k++)
            //                    {
            //                        if ((TerrainName(i, j) ?? "") == (prohibited_terrains[k] ?? ""))
            //                        {
            //                            goto NextLoop;
            //                        }
            //                    }

            //                    break;
            //                }
            //        }

            //        // 侵入（進入）禁止地形？
            //        // MOD START 240a
            //        // Set td = TDList.Item(MapData(i, j, 0))
            //        // With td
            //        // If .IsFeatureAvailable("侵入禁止") Then
            //        // For k = 1 To UBound(adopted_terrain)
            //        // If .Name = adopted_terrain(k) Then
            //        // Exit For
            //        // End If
            //        // Next
            //        // If k > UBound(adopted_terrain) Then
            //        // GoTo NextLoop
            //        // End If
            //        // End If
            //        // End With
            //        if (TerrainDoNotEnter(i, j))
            //        {
            //            var loopTo10 = Information.UBound(adopted_terrain);
            //            for (k = 1; k <= loopTo10; k++)
            //            {
            //                if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
            //                {
            //                    break;
            //                }
            //            }

            //            if (k > Information.UBound(adopted_terrain))
            //            {
            //                goto NextLoop;
            //            }
            //        }
            //        // MOD START 240a

            //        // 進路上に壁がある？
            //        if (!is_able_to_penetrate)
            //        {
            //            if (IsLineBlocked(u.x, u.y, i, j, u.Area == "空中"))
            //            {
            //                goto NextLoop;
            //            }
            //        }

            //        // マスク解除
            //        MaskData[i, j] = false;
            //    NextLoop:
            //        ;
            //    }
            //}

            //// 現在いる場所は常に進入可能
            //MaskData[u.x, u.y] = false;
        }

        // ２点間を結ぶ直線が壁でブロックされているか判定
        public bool IsLineBlocked(int x1, int y1, int x2, int y2, bool is_flying = false)
        {
            throw new NotImplementedException();
            //bool IsLineBlockedRet = default;
            //int xx, yy;
            //int xx2, yy2;
            //xx = x1;
            //yy = y1;
            //if (Math.Abs((x1 - x2)) > Math.Abs((y1 - y2)))
            //{
            //    do
            //    {
            //        if (x1 > x2)
            //        {
            //            xx = (xx - 1);
            //        }
            //        else
            //        {
            //            xx = (xx + 1);
            //        }

            //        yy2 = yy;
            //        yy = (y1 + (y2 - y1) * (x1 - xx + 0d) / (x1 - x2));

            //        // 壁？
            //        if (is_flying)
            //        {
            //            if (TerrainName(xx, yy) == "壁" | TerrainName(xx, yy2) == "壁")
            //            {
            //                IsLineBlockedRet = true;
            //                return IsLineBlockedRet;
            //            }
            //        }
            //        else
            //        {
            //            switch (TerrainName(xx, yy) ?? "")
            //            {
            //                case "壁":
            //                case "防壁":
            //                    {
            //                        IsLineBlockedRet = true;
            //                        return IsLineBlockedRet;
            //                    }
            //            }

            //            switch (TerrainName(xx, yy2) ?? "")
            //            {
            //                case "壁":
            //                case "防壁":
            //                    {
            //                        IsLineBlockedRet = true;
            //                        return IsLineBlockedRet;
            //                    }
            //            }
            //        }
            //    }
            //    while (xx != x2);
            //}
            //else
            //{
            //    do
            //    {
            //        if (y1 > y2)
            //        {
            //            yy = (yy - 1);
            //        }
            //        else
            //        {
            //            yy = (yy + 1);
            //        }

            //        xx2 = xx;
            //        xx = (x1 + (x2 - x1) * (y1 - yy + 0d) / (y1 - y2));

            //        // 壁？
            //        if (is_flying)
            //        {
            //            if (TerrainName(xx, yy) == "壁" | TerrainName(xx2, yy) == "壁")
            //            {
            //                IsLineBlockedRet = true;
            //                return IsLineBlockedRet;
            //            }
            //        }
            //        else
            //        {
            //            switch (TerrainName(xx, yy) ?? "")
            //            {
            //                case "壁":
            //                case "防壁":
            //                    {
            //                        IsLineBlockedRet = true;
            //                        return IsLineBlockedRet;
            //                    }
            //            }

            //            switch (TerrainName(xx2, yy) ?? "")
            //            {
            //                case "壁":
            //                case "防壁":
            //                    {
            //                        IsLineBlockedRet = true;
            //                        return IsLineBlockedRet;
            //                    }
            //            }
            //        }
            //    }
            //    while (yy != y2);
            //}

            //IsLineBlockedRet = false;
            //return IsLineBlockedRet;
        }

        // ユニット u が (dst_x,dst_y) に行くのに最も近い移動範囲内の場所 (X,Y) はどこか検索
        public void NearestPoint(ref Unit u, int dst_x, int dst_y, ref int X, ref int Y)
        {
            throw new NotImplementedException();
            //int k, i, j, n;
            //var total_cost = new int[52, 52];
            //var cur_speed = new int[52, 52];
            //var move_cost = new int[52, 52];
            //int tmp;
            //bool is_trans_available_on_ground;
            //bool is_trans_available_in_water;
            //bool is_trans_available_on_water;
            //var is_adaptable_in_water = default(bool);
            //string[] adopted_terrain;
            //string[] allowed_terrains;
            //string[] prohibited_terrains;
            //bool is_changed;
            //int min_x, max_x;
            //int min_y, max_y;

            //// 目的地がマップ外にならないように
            //dst_x = GeneralLib.MaxLng(GeneralLib.MinLng(dst_x, MapWidth), 1);
            //dst_y = GeneralLib.MaxLng(GeneralLib.MinLng(dst_y, MapHeight), 1);

            //// 移動能力の可否を調べておく
            //X = u.x;
            //Y = u.y;
            //string argarea_name = "陸";
            //is_trans_available_on_ground = u.IsTransAvailable(ref argarea_name) & u.get_Adaption(2) != 0;
            //string argarea_name1 = "水";
            //is_trans_available_in_water = u.IsTransAvailable(ref argarea_name1) & u.get_Adaption(3) != 0;
            //string argfname = "水中移動";
            //if (Strings.Mid(u.Data.Adaption, 3, 1) != "-" | u.IsFeatureAvailable(ref argfname))
            //{
            //    is_adaptable_in_water = true;
            //}

            //string argfname1 = "水上移動";
            //string argfname2 = "ホバー移動";
            //if (u.IsFeatureAvailable(ref argfname1) | u.IsFeatureAvailable(ref argfname2))
            //{
            //    is_trans_available_on_water = true;
            //}

            //int localLLength() { object argIndex1 = "地形適応"; string arglist = u.FeatureData(ref argIndex1); var ret = GeneralLib.LLength(ref arglist); return ret; }

            //adopted_terrain = new string[(localLLength() + 1)];
            //var loopTo = Information.UBound(adopted_terrain);
            //for (i = 2; i <= loopTo; i++)
            //{
            //    object argIndex1 = "地形適応";
            //    string arglist = u.FeatureData(ref argIndex1);
            //    adopted_terrain[i] = GeneralLib.LIndex(ref arglist, i);
            //}

            //// 各地形の移動コストを算出しておく
            //switch (u.Area ?? "")
            //{
            //    case "空中":
            //        {
            //            var loopTo1 = MapWidth;
            //            for (i = 1; i <= loopTo1; i++)
            //            {
            //                var loopTo2 = MapHeight;
            //                for (j = 1; j <= loopTo2; j++)
            //                {
            //                    if (TerrainClass(i, j) == "空")
            //                    {
            //                        move_cost[i, j] = TerrainMoveCost(i, j);
            //                        var loopTo3 = Information.UBound(adopted_terrain);
            //                        for (k = 2; k <= loopTo3; k++)
            //                        {
            //                            if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
            //                            {
            //                                move_cost[i, j] = GeneralLib.MinLng(move_cost[i, j], 2);
            //                                break;
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        move_cost[i, j] = 2;
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "地上":
            //        {
            //            var loopTo4 = MapWidth;
            //            for (i = 1; i <= loopTo4; i++)
            //            {
            //                var loopTo5 = MapHeight;
            //                for (j = 1; j <= loopTo5; j++)
            //                {
            //                    switch (TerrainClass(i, j) ?? "")
            //                    {
            //                        case "陸":
            //                        case "屋内":
            //                        case "月面":
            //                            {
            //                                if (is_trans_available_on_ground)
            //                                {
            //                                    move_cost[i, j] = TerrainMoveCost(i, j);
            //                                    var loopTo6 = Information.UBound(adopted_terrain);
            //                                    for (k = 2; k <= loopTo6; k++)
            //                                    {
            //                                        if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
            //                                        {
            //                                            move_cost[i, j] = GeneralLib.MinLng(move_cost[i, j], 2);
            //                                            break;
            //                                        }
            //                                    }
            //                                }
            //                                else
            //                                {
            //                                    move_cost[i, j] = 1000000;
            //                                }

            //                                break;
            //                            }

            //                        case "水":
            //                            {
            //                                if (is_trans_available_in_water)
            //                                {
            //                                    move_cost[i, j] = 2;
            //                                }
            //                                else if (is_adaptable_in_water)
            //                                {
            //                                    move_cost[i, j] = TerrainMoveCost(i, j);
            //                                    var loopTo7 = Information.UBound(adopted_terrain);
            //                                    for (k = 2; k <= loopTo7; k++)
            //                                    {
            //                                        if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
            //                                        {
            //                                            move_cost[i, j] = GeneralLib.MinLng(move_cost[i, j], 2);
            //                                            break;
            //                                        }
            //                                    }
            //                                }
            //                                else
            //                                {
            //                                    move_cost[i, j] = 1000000;
            //                                }

            //                                break;
            //                            }

            //                        case "深水":
            //                            {
            //                                if (is_trans_available_in_water)
            //                                {
            //                                    move_cost[i, j] = 1;
            //                                }
            //                                else
            //                                {
            //                                    move_cost[i, j] = 1000000;
            //                                }

            //                                break;
            //                            }

            //                        case "空":
            //                            {
            //                                move_cost[i, j] = 1000000;
            //                                break;
            //                            }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "水上":
            //        {
            //            var loopTo8 = MapWidth;
            //            for (i = 1; i <= loopTo8; i++)
            //            {
            //                var loopTo9 = MapHeight;
            //                for (j = 1; j <= loopTo9; j++)
            //                {
            //                    switch (TerrainClass(i, j) ?? "")
            //                    {
            //                        case "陸":
            //                        case "屋内":
            //                        case "月面":
            //                            {
            //                                if (is_trans_available_on_ground)
            //                                {
            //                                    move_cost[i, j] = TerrainMoveCost(i, j);
            //                                    var loopTo10 = Information.UBound(adopted_terrain);
            //                                    for (k = 2; k <= loopTo10; k++)
            //                                    {
            //                                        if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
            //                                        {
            //                                            move_cost[i, j] = GeneralLib.MinLng(move_cost[i, j], 2);
            //                                            break;
            //                                        }
            //                                    }
            //                                }
            //                                else
            //                                {
            //                                    move_cost[i, j] = 1000000;
            //                                }

            //                                break;
            //                            }

            //                        case "水":
            //                        case "深水":
            //                            {
            //                                move_cost[i, j] = 2;
            //                                break;
            //                            }

            //                        case "空":
            //                            {
            //                                move_cost[i, j] = 1000000;
            //                                break;
            //                            }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "水中":
            //        {
            //            var loopTo11 = MapWidth;
            //            for (i = 1; i <= loopTo11; i++)
            //            {
            //                var loopTo12 = MapHeight;
            //                for (j = 1; j <= loopTo12; j++)
            //                {
            //                    switch (TerrainClass(i, j) ?? "")
            //                    {
            //                        case "陸":
            //                        case "屋内":
            //                        case "月面":
            //                            {
            //                                if (is_trans_available_on_ground)
            //                                {
            //                                    move_cost[i, j] = TerrainMoveCost(i, j);
            //                                    var loopTo13 = Information.UBound(adopted_terrain);
            //                                    for (k = 2; k <= loopTo13; k++)
            //                                    {
            //                                        if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
            //                                        {
            //                                            move_cost[i, j] = GeneralLib.MinLng(move_cost[i, j], 2);
            //                                            break;
            //                                        }
            //                                    }
            //                                }
            //                                else
            //                                {
            //                                    move_cost[i, j] = 1000000;
            //                                }

            //                                break;
            //                            }

            //                        case "水":
            //                            {
            //                                if (is_trans_available_in_water)
            //                                {
            //                                    move_cost[i, j] = 2;
            //                                }
            //                                else
            //                                {
            //                                    move_cost[i, j] = TerrainMoveCost(i, j);
            //                                    var loopTo14 = Information.UBound(adopted_terrain);
            //                                    for (k = 2; k <= loopTo14; k++)
            //                                    {
            //                                        if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
            //                                        {
            //                                            move_cost[i, j] = GeneralLib.MinLng(move_cost[i, j], 2);
            //                                            break;
            //                                        }
            //                                    }
            //                                }

            //                                break;
            //                            }

            //                        case "深水":
            //                            {
            //                                if (is_trans_available_in_water)
            //                                {
            //                                    move_cost[i, j] = 1;
            //                                }
            //                                else
            //                                {
            //                                    move_cost[i, j] = 1000000;
            //                                }

            //                                break;
            //                            }

            //                        case "空":
            //                            {
            //                                move_cost[i, j] = 1000000;
            //                                break;
            //                            }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "宇宙":
            //        {
            //            var loopTo15 = MapWidth;
            //            for (i = 1; i <= loopTo15; i++)
            //            {
            //                var loopTo16 = MapHeight;
            //                for (j = 1; j <= loopTo16; j++)
            //                {
            //                    move_cost[i, j] = TerrainMoveCost(i, j);
            //                    var loopTo17 = Information.UBound(adopted_terrain);
            //                    for (k = 2; k <= loopTo17; k++)
            //                    {
            //                        if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
            //                        {
            //                            move_cost[i, j] = GeneralLib.MinLng(move_cost[i, j], 2);
            //                            break;
            //                        }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "地中":
            //        {
            //            var loopTo18 = MapWidth;
            //            for (i = 1; i <= loopTo18; i++)
            //            {
            //                var loopTo19 = MapHeight;
            //                for (j = 1; j <= loopTo19; j++)
            //                {
            //                    if (TerrainClass(i, j) == "陸")
            //                    {
            //                        move_cost[i, j] = 2;
            //                    }
            //                    else
            //                    {
            //                        move_cost[i, j] = 1000000;
            //                    }
            //                }
            //            }

            //            break;
            //        }
            //}
            //// 線路移動
            //string argfname3 = "線路移動";
            //if (u.IsFeatureAvailable(ref argfname3))
            //{
            //    if (u.Area == "地上")
            //    {
            //        var loopTo20 = MapWidth;
            //        for (i = 1; i <= loopTo20; i++)
            //        {
            //            var loopTo21 = MapHeight;
            //            for (j = 1; j <= loopTo21; j++)
            //            {
            //                if (TerrainName(i, j) == "線路")
            //                {
            //                    move_cost[i, j] = 1;
            //                }
            //                else
            //                {
            //                    move_cost[i, j] = 1000000;
            //                }
            //            }
            //        }
            //    }
            //}

            //// 移動制限
            //allowed_terrains = new string[1];
            //string argfname4 = "移動制限";
            //if (u.IsFeatureAvailable(ref argfname4))
            //{
            //    if (u.Area != "空中" & u.Area != "地中")
            //    {
            //        object argIndex2 = "移動制限";
            //        string arglist1 = u.FeatureData(ref argIndex2);
            //        n = GeneralLib.LLength(ref arglist1);
            //        allowed_terrains = new string[(n + 1)];
            //        var loopTo22 = n;
            //        for (i = 2; i <= loopTo22; i++)
            //        {
            //            object argIndex3 = "移動制限";
            //            string arglist2 = u.FeatureData(ref argIndex3);
            //            allowed_terrains[i] = GeneralLib.LIndex(ref arglist2, i);
            //        }

            //        var loopTo23 = MapWidth;
            //        for (i = 1; i <= loopTo23; i++)
            //        {
            //            var loopTo24 = MapHeight;
            //            for (j = 1; j <= loopTo24; j++)
            //            {
            //                var loopTo25 = n;
            //                for (k = 2; k <= loopTo25; k++)
            //                {
            //                    if ((TerrainName(i, j) ?? "") == (allowed_terrains[k] ?? ""))
            //                    {
            //                        break;
            //                    }
            //                }

            //                if (k > n)
            //                {
            //                    move_cost[i, j] = 1000000;
            //                }
            //            }
            //        }
            //    }
            //}

            //// 進入不可
            //prohibited_terrains = new string[1];
            //string argfname5 = "進入不可";
            //if (u.IsFeatureAvailable(ref argfname5))
            //{
            //    if (u.Area != "空中" & u.Area != "地中")
            //    {
            //        object argIndex4 = "進入不可";
            //        string arglist3 = u.FeatureData(ref argIndex4);
            //        n = GeneralLib.LLength(ref arglist3);
            //        prohibited_terrains = new string[(n + 1)];
            //        var loopTo26 = n;
            //        for (i = 2; i <= loopTo26; i++)
            //        {
            //            object argIndex5 = "進入不可";
            //            string arglist4 = u.FeatureData(ref argIndex5);
            //            prohibited_terrains[i] = GeneralLib.LIndex(ref arglist4, i);
            //        }

            //        var loopTo27 = MapWidth;
            //        for (i = 1; i <= loopTo27; i++)
            //        {
            //            var loopTo28 = MapHeight;
            //            for (j = 1; j <= loopTo28; j++)
            //            {
            //                var loopTo29 = n;
            //                for (k = 2; k <= loopTo29; k++)
            //                {
            //                    if ((TerrainName(i, j) ?? "") == (prohibited_terrains[k] ?? ""))
            //                    {
            //                        move_cost[i, j] = 1000000;
            //                        break;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            //// ホバー移動
            //string argfname6 = "ホバー移動";
            //if (u.IsFeatureAvailable(ref argfname6))
            //{
            //    if (u.Area == "地上" | u.Area == "水上")
            //    {
            //        var loopTo30 = MapWidth;
            //        for (i = 1; i <= loopTo30; i++)
            //        {
            //            var loopTo31 = MapHeight;
            //            for (j = 1; j <= loopTo31; j++)
            //            {
            //                switch (TerrainName(i, j) ?? "")
            //                {
            //                    case "砂漠":
            //                    case "月面":
            //                        {
            //                            move_cost[i, j] = 1;
            //                            break;
            //                        }
            //                }
            //            }
            //        }
            //    }
            //}

            //// ジャンプ移動
            //string argfname7 = "ジャンプ移動";
            //if (u.IsFeatureAvailable(ref argfname7))
            //{
            //    if (u.Area == "地上" | u.Area == "水上" | u.Area == "水中")
            //    {
            //        var loopTo32 = MapWidth;
            //        for (i = 1; i <= loopTo32; i++)
            //        {
            //            var loopTo33 = MapHeight;
            //            for (j = 1; j <= loopTo33; j++)
            //            {
            //                switch (TerrainClass(i, j) ?? "")
            //                {
            //                    case "陸":
            //                    case "月面":
            //                        {
            //                            move_cost[i, j] = 1;
            //                            break;
            //                        }
            //                }
            //            }
            //        }
            //    }
            //}

            //var loopTo34 = (MapWidth + 1);
            //for (i = 0; i <= loopTo34; i++)
            //{
            //    var loopTo35 = (MapHeight + 1);
            //    for (j = 0; j <= loopTo35; j++)
            //        total_cost[i, j] = 1000000;
            //}

            //total_cost[dst_x, dst_y] = 0;

            //// 目的地から各地点に到達するのにかかる移動力を計算
            //i = 0;
            //do
            //{
            //    i = (i + 1);

            //    // タイムアウト
            //    if (i > 3 * (MapWidth + MapHeight))
            //    {
            //        break;
            //    }

            //    is_changed = false;
            //    var loopTo36 = (MapWidth + 1);
            //    for (j = 0; j <= loopTo36; j++)
            //    {
            //        var loopTo37 = (MapHeight + 1);
            //        for (k = 0; k <= loopTo37; k++)
            //            cur_speed[j, k] = total_cost[j, k];
            //    }

            //    min_x = GeneralLib.MaxLng(1, dst_x - i);
            //    max_x = GeneralLib.MinLng(dst_x + i, MapWidth);
            //    var loopTo38 = max_x;
            //    for (j = min_x; j <= loopTo38; j++)
            //    {
            //        min_y = GeneralLib.MaxLng(1, dst_y - (i - Math.Abs((dst_x - j))));
            //        max_y = GeneralLib.MinLng(dst_y + (i - Math.Abs((dst_x - j))), MapHeight);
            //        var loopTo39 = max_y;
            //        for (k = min_y; k <= loopTo39; k++)
            //        {
            //            tmp = cur_speed[j, k];
            //            tmp = GeneralLib.MinLng(tmp, cur_speed[j - 1, k]);
            //            tmp = GeneralLib.MinLng(tmp, cur_speed[j + 1, k]);
            //            tmp = GeneralLib.MinLng(tmp, cur_speed[j, k - 1]);
            //            tmp = GeneralLib.MinLng(tmp, cur_speed[j, k + 1]);
            //            tmp = tmp + move_cost[j, k];
            //            if (tmp < cur_speed[j, k])
            //            {
            //                is_changed = true;
            //                total_cost[j, k] = tmp;
            //            }
            //        }
            //    }

            //    // 最短経路を発見した
            //    if (total_cost[X, Y] <= Math.Abs((dst_x - X)) + Math.Abs((dst_y - Y)) + 2)
            //    {
            //        break;
            //    }
            //}
            //while (is_changed);

            //// 移動可能範囲内で目的地に最も近い場所を見付ける
            //tmp = total_cost[X, Y];
            //var loopTo40 = MapWidth;
            //for (i = 1; i <= loopTo40; i++)
            //{
            //    var loopTo41 = MapHeight;
            //    for (j = 1; j <= loopTo41; j++)
            //    {
            //        if (!MaskData[i, j])
            //        {
            //            if (MapDataForUnit[i, j] is null)
            //            {
            //                if (total_cost[i, j] < tmp)
            //                {
            //                    X = i;
            //                    Y = j;
            //                    tmp = total_cost[i, j];
            //                }
            //                else if (total_cost[i, j] == tmp)
            //                {
            //                    if (Math.Pow(Math.Abs((dst_x - i)), 2d) + Math.Pow(Math.Abs((dst_y - j)), 2d) < Math.Pow(Math.Abs((dst_x - X)), 2d) + Math.Pow(Math.Abs((dst_y - Y)), 2d))
            //                    {
            //                        X = i;
            //                        Y = j;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
        }

        // ユニット u が敵から最も遠くなる場所(X,Y)を検索
        public void SafetyPoint(ref Unit u, ref int X, ref int Y)
        {
            throw new NotImplementedException();
            //    int i, j;
            //    var total_cost = new int[52, 52];
            //    var cur_cost = new int[52, 52];
            //    int tmp;
            //    bool is_changed;

            //    // 作業用配列を初期化
            //    var loopTo = (MapWidth + 1);
            //    for (i = 0; i <= loopTo; i++)
            //    {
            //        var loopTo1 = (MapHeight + 1);
            //        for (j = 0; j <= loopTo1; j++)
            //            total_cost[i, j] = 1000000;
            //    }

            //    foreach (Unit t in SRC.UList)
            //    {
            //        if (u.IsEnemy(ref t))
            //        {
            //            total_cost[t.x, t.y] = 0;
            //        }
            //    }

            //    // 各地点の敵からの距離を計算
            //    do
            //    {
            //        is_changed = false;
            //        var loopTo2 = (MapWidth + 1);
            //        for (i = 0; i <= loopTo2; i++)
            //        {
            //            var loopTo3 = (MapHeight + 1);
            //            for (j = 0; j <= loopTo3; j++)
            //                cur_cost[i, j] = total_cost[i, j];
            //        }

            //        var loopTo4 = MapWidth;
            //        for (i = 1; i <= loopTo4; i++)
            //        {
            //            var loopTo5 = MapHeight;
            //            for (j = 1; j <= loopTo5; j++)
            //            {
            //                tmp = cur_cost[i, j];
            //                tmp = GeneralLib.MinLng(cur_cost[i - 1, j] + 1, tmp);
            //                tmp = GeneralLib.MinLng(cur_cost[i + 1, j] + 1, tmp);
            //                tmp = GeneralLib.MinLng(cur_cost[i, j - 1] + 1, tmp);
            //                tmp = GeneralLib.MinLng(cur_cost[i, j + 1] + 1, tmp);
            //                if (tmp < cur_cost[i, j])
            //                {
            //                    is_changed = true;
            //                    total_cost[i, j] = tmp;
            //                }
            //            }
            //        }
            //    }
            //    while (is_changed);

            //    // 移動可能範囲内で敵から最も遠い場所を見付ける
            //    tmp = 0;
            //    var loopTo6 = MapWidth;
            //    for (i = 1; i <= loopTo6; i++)
            //    {
            //        var loopTo7 = MapHeight;
            //        for (j = 1; j <= loopTo7; j++)
            //        {
            //            if (!MaskData[i, j])
            //            {
            //                if (MapDataForUnit[i, j] is null)
            //                {
            //                    if (total_cost[i, j] > tmp)
            //                    {
            //                        X = i;
            //                        Y = j;
            //                        tmp = total_cost[i, j];
            //                    }
            //                    else if (total_cost[i, j] == tmp)
            //                    {
            //                        if (Math.Pow(Math.Abs((u.x - i)), 2d) + Math.Pow(Math.Abs((u.y - j)), 2d) < Math.Pow(Math.Abs((u.x - X)), 2d) + Math.Pow(Math.Abs((u.y - Y)), 2d))
            //                        {
            //                            X = i;
            //                            Y = j;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            //// 現在位置から指定した場所までの移動経路を調べる
            //// 事前にAreaInSpeedを実行しておく事が必要
            //public void SearchMoveRoute(ref int tx, ref int ty, ref int[] move_route_x, ref int[] move_route_y)
            //{
            //    int xx, yy;
            //    int nx, ny;
            //    int ox = default, oy = default;
            //    int tmp;
            //    int i;
            //    string direction = default, prev_direction = default;
            //    object[] move_direction;
            //    move_route_x = new int[2];
            //    move_route_y = new int[2];
            //    move_direction = new object[2];
            //    move_route_x[1] = tx;
            //    move_route_y[1] = ty;

            //    // 現在位置を調べる
            //    var loopTo = MapWidth;
            //    for (xx = 1; xx <= loopTo; xx++)
            //    {
            //        var loopTo1 = MapHeight;
            //        for (yy = 1; yy <= loopTo1; yy++)
            //        {
            //            if (TotalMoveCost[xx, yy] == 0)
            //            {
            //                ox = xx;
            //                oy = yy;
            //            }
            //        }
            //    }

            //    // 現在位置のＺＯＣは無効化する
            //    PointInZOC[ox, oy] = 0;
            //    xx = tx;
            //    yy = ty;
            //    nx = tx;
            //    ny = ty;
            //    while (TotalMoveCost[xx, yy] > 0)
            //    {
            //        tmp = TotalMoveCost[xx, yy];

            //        // 周りの場所から最も必要移動力が低い場所を探す

            //        // なるべく直線方向に移動させるため、前回と同じ移動方向を優先させる
            //        switch (prev_direction ?? "")
            //        {
            //            case "N":
            //                {
            //                    if (TotalMoveCost[xx, yy - 1] < tmp & PointInZOC[xx, yy - 1] <= 0)
            //                    {
            //                        tmp = TotalMoveCost[xx, yy - 1];
            //                        nx = xx;
            //                        ny = (yy - 1);
            //                        direction = "N";
            //                    }

            //                    break;
            //                }

            //            case "S":
            //                {
            //                    if (TotalMoveCost[xx, yy + 1] < tmp & PointInZOC[xx, yy + 1] <= 0)
            //                    {
            //                        tmp = TotalMoveCost[xx, yy + 1];
            //                        nx = xx;
            //                        ny = (yy + 1);
            //                        direction = "S";
            //                    }

            //                    break;
            //                }

            //            case "W":
            //                {
            //                    if (TotalMoveCost[xx - 1, yy] < tmp & PointInZOC[xx - 1, yy] <= 0)
            //                    {
            //                        tmp = TotalMoveCost[xx - 1, yy];
            //                        nx = (xx - 1);
            //                        ny = yy;
            //                        direction = "W";
            //                    }

            //                    break;
            //                }

            //            case "E":
            //                {
            //                    if (TotalMoveCost[xx + 1, yy] < tmp & PointInZOC[xx + 1, yy] <= 0)
            //                    {
            //                        tmp = TotalMoveCost[xx + 1, yy];
            //                        nx = (xx + 1);
            //                        ny = yy;
            //                        direction = "E";
            //                    }

            //                    break;
            //                }
            //        }

            //        // なるべく目標位置付近で直進させるため、目標位置との距離差の小さい
            //        // 座標軸方向に優先して移動させる
            //        if (Math.Abs((xx - ox)) <= Math.Abs((yy - oy)))
            //        {
            //            if (TotalMoveCost[xx, yy - 1] < tmp & PointInZOC[xx, yy - 1] <= 0)
            //            {
            //                tmp = TotalMoveCost[xx, yy - 1];
            //                nx = xx;
            //                ny = (yy - 1);
            //                direction = "N";
            //            }

            //            if (TotalMoveCost[xx, yy + 1] < tmp & PointInZOC[xx, yy + 1] <= 0)
            //            {
            //                tmp = TotalMoveCost[xx, yy + 1];
            //                nx = xx;
            //                ny = (yy + 1);
            //                direction = "S";
            //            }

            //            if (TotalMoveCost[xx - 1, yy] < tmp & PointInZOC[xx - 1, yy] <= 0)
            //            {
            //                tmp = TotalMoveCost[xx - 1, yy];
            //                nx = (xx - 1);
            //                ny = yy;
            //                direction = "W";
            //            }

            //            if (TotalMoveCost[xx + 1, yy] < tmp & PointInZOC[xx + 1, yy] <= 0)
            //            {
            //                tmp = TotalMoveCost[xx + 1, yy];
            //                nx = (xx + 1);
            //                ny = yy;
            //                direction = "E";
            //            }
            //        }
            //        else
            //        {
            //            if (TotalMoveCost[xx - 1, yy] < tmp & PointInZOC[xx - 1, yy] <= 0)
            //            {
            //                tmp = TotalMoveCost[xx - 1, yy];
            //                nx = (xx - 1);
            //                ny = yy;
            //                direction = "W";
            //            }

            //            if (TotalMoveCost[xx + 1, yy] < tmp & PointInZOC[xx + 1, yy] <= 0)
            //            {
            //                tmp = TotalMoveCost[xx + 1, yy];
            //                nx = (xx + 1);
            //                ny = yy;
            //                direction = "E";
            //            }

            //            if (TotalMoveCost[xx, yy - 1] < tmp & PointInZOC[xx, yy - 1] <= 0)
            //            {
            //                tmp = TotalMoveCost[xx, yy - 1];
            //                nx = xx;
            //                ny = (yy - 1);
            //                direction = "N";
            //            }

            //            if (TotalMoveCost[xx, yy + 1] < tmp & PointInZOC[xx, yy + 1] <= 0)
            //            {
            //                tmp = TotalMoveCost[xx, yy + 1];
            //                nx = xx;
            //                ny = (yy + 1);
            //                direction = "S";
            //            }
            //        }

            //        if (nx == xx & ny == yy)
            //        {
            //            // これ以上必要移動力が低い場所が見つからなかったので終了
            //            break;
            //        }

            //        // 見つかった場所を記録
            //        Array.Resize(ref move_route_x, Information.UBound(move_route_x) + 1 + 1);
            //        Array.Resize(ref move_route_y, Information.UBound(move_route_y) + 1 + 1);
            //        move_route_x[Information.UBound(move_route_x)] = nx;
            //        move_route_y[Information.UBound(move_route_y)] = ny;

            //        // 移動方向を記録
            //        Array.Resize(ref move_direction, Information.UBound(move_direction) + 1 + 1);
            //        // UPGRADE_WARNING: オブジェクト move_direction(UBound()) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //        move_direction[Information.UBound(move_direction)] = direction;
            //        prev_direction = direction;

            //        // 次回は今回見つかった場所を起点に検索する
            //        xx = nx;
            //        yy = ny;
            //    }

            //    // 直線を走った距離を計算
            //    Commands.MovedUnitSpeed = 1;
            //    var loopTo2 = (Information.UBound(move_direction) - 1);
            //    for (i = 2; i <= loopTo2; i++)
            //    {
            //        // UPGRADE_WARNING: オブジェクト move_direction(i + 1) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //        // UPGRADE_WARNING: オブジェクト move_direction(i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //        if (Conversions.ToBoolean(Operators.ConditionalCompareObjectNotEqual(move_direction[i], move_direction[i + 1], false)))
            //        {
            //            break;
            //        }

            //        Commands.MovedUnitSpeed = (Commands.MovedUnitSpeed + 1);
            //    }
        }
    }
}