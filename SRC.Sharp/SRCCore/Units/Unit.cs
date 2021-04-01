// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.VB;
using System.Collections.Generic;

namespace SRCCore.Units
{
    public partial class Unit
    {
        private SRC SRC { get; }
        private Map Map => SRC.Map;
        private IGUI GUI => SRC.GUI;
        private Events.Event Event => SRC.Event;
        private Commands.Command Commands => SRC.Commands;
        private Expressions.Expression Expression => SRC.Expression;
        private Sound Sound => SRC.Sound;

        public Unit(SRC src)
        {
            SRC = src;
        }

        // データ
        public UnitData Data;

        // 識別用ＩＤ
        public string ID;

        // ビットマップID
        // 同種のユニットは同じIDを共有
        public int BitmapID;

        // Ｘ座標
        public int x;
        // Ｙ座標
        public int y;

        // ユニットの場所（地上、水上、水中、空中、地中、宇宙）
        public string Area;

        // 使用済み行動数
        public int UsedAction;

        // 思考モード
        private string strMode;

        // ステータス
        // 出撃：マップ上に出撃
        // 他形態：他の形態に変形(ハイパーモード)中
        // 破壊：破壊されている
        // 破棄：イベントコマンド RemoveUnit などによりイベントで破壊されている
        // 格納：母艦内に格納されている
        // 待機：待機中
        // 旧形態：分離ユニットが合体前に取っていた形態
        // 離脱：Leaveコマンドにより戦線を離脱。Organizeコマンドでも表示されない
        public string Status;

        // ユニットに対して使用されているスペシャルパワー
        private SrcCollection<Condition> colSpecialPowerInEffect = new SrcCollection<Condition>();

        // サポートアタック＆ガードの使用回数
        public int UsedSupportAttack;
        public int UsedSupportGuard;

        // 同時援護攻撃の使用回数
        public int UsedSyncAttack;

        // カウンター攻撃の使用回数
        public int UsedCounterAttack;

        // ユニット名称
        private string strName;
        // 陣営
        private string strParty;
        // ユニットランク
        private int intRank;
        // ボスランク
        private int intBossRank;
        // ＨＰ
        private int lngMaxHP;
        private int lngHP;
        // ＥＮ
        private int intMaxEN;
        private int intEN;
        // 装甲
        private int lngArmor;
        // 運動性
        private int intMobility;
        // 移動力
        private int intSpeed;

        // 搭乗しているパイロット
        private SrcCollection<Pilot> colPilot = new SrcCollection<Pilot>();
        public IList<Pilot> Pilots => colPilot.List;

        // 搭乗しているサポートパイロット
        private SrcCollection<Pilot> colSupport = new SrcCollection<Pilot>();
        public IList<Pilot> SupportPilots => colSupport.List;

        // 関連するユニット
        // 変形ユニットにおける他形態等
        private SrcCollection<Unit> colOtherForm = new SrcCollection<Unit>();
        public IList<Unit> OtherForms => colOtherForm.List;

        // 格納したユニット
        private SrcCollection<Unit> colUnitOnBoard = new SrcCollection<Unit>();
        public IList<Unit> UnitOnBoards => colUnitOnBoard.List;

        //// 装備しているアイテム
        //private Collection colItem = new Collection();

        // 現在の特殊ステータス
        private SrcCollection<Condition> colCondition = new SrcCollection<Condition>();
        public IList<Condition> Conditions => colCondition.List;

        // 各武器の残弾数
        private double[] dblBullet;

        // アビリティの残り使用回数
        private double[] dblStock;

        // 特殊能力
        private SrcCollection<FeatureData> colFeature = new SrcCollection<FeatureData>();

        // 特殊能力(必要条件を満たさないものを含む)
        private SrcCollection<FeatureData> colAllFeature = new SrcCollection<FeatureData>();

        // 付加された特殊能力数
        public int AdditionalFeaturesNum;

        // 地形適応
        private string strAdaption;

        // 攻撃への耐性
        public string strAbsorb;
        public string strImmune;
        public string strResist;
        public string strWeakness;
        public string strEffective;
        public string strSpecialEffectImmune;

        // 武器データ
        private List<UnitWeapon> WData = new List<UnitWeapon>();
        public IList<UnitWeapon> Weapons => WData.AsReadOnly();

        // アビリティデータ
        private List<UnitAbility> AData = new List<UnitAbility>();
        public IList<UnitAbility> Abilities => AData.AsReadOnly();

        // 選択したマップ攻撃の攻撃力
        public int SelectedMapAttackPower;

        // 選択したマップ攻撃の攻撃力
        private bool IsMapAttackCanceled;

        // 召喚したユニット
        private SrcCollection<Unit> colServant = new SrcCollection<Unit>();

        // 魅了・憑依したユニット
        private SrcCollection<Unit> colSlave = new SrcCollection<Unit>();

        // 召喚主
        public Unit Summoner;

        // ご主人様
        public Unit Master;

        // 追加パイロット
        public Pilot pltAdditionalPilot;

        // 追加サポート
        public Pilot pltAdditionalSupport;
    }
}
