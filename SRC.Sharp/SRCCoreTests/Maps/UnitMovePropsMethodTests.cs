using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Maps;
using SRCCore.Models;
using System.Collections.Generic;

namespace SRCCore.Maps.Tests
{
    /// <summary>
    /// UnitMoveProps の IsAdopted / IsAllowed / IsProhibited メソッドのユニットテスト
    /// （リストの内部状態を直接インスタンス化して検証する）
    /// </summary>
    [TestClass]
    public class UnitMovePropsMethodTests
    {
        // ──────────────────────────────────────────────
        // ヘルパー：テスト対象インスタンスを手動で作成するためのサブクラス
        // ──────────────────────────────────────────────

        /// <summary>
        /// UnitMoveProps の protected フィールドを制御するためのテスト専用 Proxy
        /// </summary>
        private class TestableUnitMoveProps
        {
            public IList<string> AdoptedTerrain { get; } = new List<string>();
            public IList<string> AllowedTerrains { get; } = new List<string>();
            public IList<string> ProhibitedTerrains { get; } = new List<string>();

            public bool IsAdopted(TerrainData td) => AdoptedTerrain.Contains(td.Name);

            public bool IsAllowed(TerrainData td)
                => AllowedTerrains.Count == 0 || AllowedTerrains.Contains(td.Name);

            public bool IsProhibited(TerrainData td) => ProhibitedTerrains.Contains(td.Name);
        }

        // ──────────────────────────────────────────────
        // IsAdopted
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsAdopted_TerrainInList_ReturnsTrue()
        {
            var props = new TestableUnitMoveProps();
            props.AdoptedTerrain.Add("平地");
            var td = new TerrainData { Name = "平地" };
            Assert.IsTrue(props.IsAdopted(td));
        }

        [TestMethod]
        public void IsAdopted_TerrainNotInList_ReturnsFalse()
        {
            var props = new TestableUnitMoveProps();
            props.AdoptedTerrain.Add("平地");
            var td = new TerrainData { Name = "山岳" };
            Assert.IsFalse(props.IsAdopted(td));
        }

        [TestMethod]
        public void IsAdopted_EmptyList_ReturnsFalse()
        {
            var props = new TestableUnitMoveProps();
            var td = new TerrainData { Name = "平地" };
            Assert.IsFalse(props.IsAdopted(td));
        }

        [TestMethod]
        public void IsAdopted_MultipleTerrains_MatchesCorrectTerrain()
        {
            var props = new TestableUnitMoveProps();
            props.AdoptedTerrain.Add("平地");
            props.AdoptedTerrain.Add("宇宙");
            props.AdoptedTerrain.Add("水中");

            Assert.IsTrue(props.IsAdopted(new TerrainData { Name = "宇宙" }));
            Assert.IsTrue(props.IsAdopted(new TerrainData { Name = "水中" }));
            Assert.IsFalse(props.IsAdopted(new TerrainData { Name = "山岳" }));
        }

        // ──────────────────────────────────────────────
        // IsAllowed
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsAllowed_EmptyAllowedList_AlwaysTrue()
        {
            var props = new TestableUnitMoveProps();
            var td = new TerrainData { Name = "任意の地形" };
            // 移動制限が無ければ許可とみなす
            Assert.IsTrue(props.IsAllowed(td));
        }

        [TestMethod]
        public void IsAllowed_TerrainInAllowedList_ReturnsTrue()
        {
            var props = new TestableUnitMoveProps();
            props.AllowedTerrains.Add("平地");
            props.AllowedTerrains.Add("宇宙");
            var td = new TerrainData { Name = "平地" };
            Assert.IsTrue(props.IsAllowed(td));
        }

        [TestMethod]
        public void IsAllowed_TerrainNotInAllowedList_ReturnsFalse()
        {
            var props = new TestableUnitMoveProps();
            props.AllowedTerrains.Add("平地");
            var td = new TerrainData { Name = "山岳" };
            Assert.IsFalse(props.IsAllowed(td));
        }

        [TestMethod]
        public void IsAllowed_AllowedListHasMultipleItems_ChecksCorrectly()
        {
            var props = new TestableUnitMoveProps();
            props.AllowedTerrains.Add("平地");
            props.AllowedTerrains.Add("砂漠");

            Assert.IsTrue(props.IsAllowed(new TerrainData { Name = "平地" }));
            Assert.IsTrue(props.IsAllowed(new TerrainData { Name = "砂漠" }));
            Assert.IsFalse(props.IsAllowed(new TerrainData { Name = "水中" }));
        }

        // ──────────────────────────────────────────────
        // IsProhibited
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsProhibited_TerrainInProhibitedList_ReturnsTrue()
        {
            var props = new TestableUnitMoveProps();
            props.ProhibitedTerrains.Add("山岳");
            var td = new TerrainData { Name = "山岳" };
            Assert.IsTrue(props.IsProhibited(td));
        }

        [TestMethod]
        public void IsProhibited_TerrainNotInProhibitedList_ReturnsFalse()
        {
            var props = new TestableUnitMoveProps();
            props.ProhibitedTerrains.Add("山岳");
            var td = new TerrainData { Name = "平地" };
            Assert.IsFalse(props.IsProhibited(td));
        }

        [TestMethod]
        public void IsProhibited_EmptyList_ReturnsFalse()
        {
            var props = new TestableUnitMoveProps();
            var td = new TerrainData { Name = "山岳" };
            Assert.IsFalse(props.IsProhibited(td));
        }

        [TestMethod]
        public void IsProhibited_MultipleProhibited_MatchesCorrectly()
        {
            var props = new TestableUnitMoveProps();
            props.ProhibitedTerrains.Add("山岳");
            props.ProhibitedTerrains.Add("水中");

            Assert.IsTrue(props.IsProhibited(new TerrainData { Name = "山岳" }));
            Assert.IsTrue(props.IsProhibited(new TerrainData { Name = "水中" }));
            Assert.IsFalse(props.IsProhibited(new TerrainData { Name = "平地" }));
        }

        // ──────────────────────────────────────────────
        // 組み合わせテスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TerrainAdoptedButNotAllowed_CombinationWorks()
        {
            var props = new TestableUnitMoveProps();
            props.AdoptedTerrain.Add("宇宙");
            props.AllowedTerrains.Add("平地"); // 移動制限あり（平地のみ）

            var spaceTd = new TerrainData { Name = "宇宙" };
            // 採用地形だが、移動制限により許可されていない
            Assert.IsTrue(props.IsAdopted(spaceTd));
            Assert.IsFalse(props.IsAllowed(spaceTd));
        }

        [TestMethod]
        public void TerrainProhibitedButAdopted_CombinationWorks()
        {
            var props = new TestableUnitMoveProps();
            props.AdoptedTerrain.Add("山岳");
            props.ProhibitedTerrains.Add("山岳");

            var mountainTd = new TerrainData { Name = "山岳" };
            Assert.IsTrue(props.IsAdopted(mountainTd));
            Assert.IsTrue(props.IsProhibited(mountainTd));
        }
    }
}
