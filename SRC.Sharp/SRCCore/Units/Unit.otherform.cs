using SRCCore.Lib;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Units
{
    // === 他形態関連処理 ===
    public partial class Unit
    {
        // ユニットが現在とっている形態
        public Unit CurrentForm()
        {
            if (Status == "他形態")
            {
                var cf = OtherForms.FirstOrDefault(x => x.Status != "他形態");
                if (cf != null)
                {
                    return cf;
                }
            }

            return this;
        }

        // 他形態を登録
        public void AddOtherForm(Unit u)
        {
            colOtherForm.Add(u, u.ID);
        }

        // 他形態を削除
        public void DeleteOtherForm(string Index)
        {
            try
            {
                colOtherForm.Remove(Index);
            }
            catch
            {
                // 見つからなければユニット名称で検索
                foreach (var u in OtherForms)
                {
                    if (u.Name == Index)
                    {
                        colOtherForm.Remove(u);
                        return;
                    }
                }
            }
        }

        // 他形態の総数
        public int CountOtherForm()
        {
            return colOtherForm.Count;
        }

        // 他形態
        public Unit OtherForm(string Index)
        {
            var u = colOtherForm[Index];
            if (u != null)
            {
                return u;
            }

            // 見つからなければユニット名称で検索
            var uname = Index;
            u = colOtherForm.Values.FirstOrDefault(x => x.Name == uname);
            if (u != null)
            {
                return u;
            }

            // 該当するユニットがなければ作成して追加
            if (SRC.UDList.IsDefined(uname))
            {
                u = new Unit(SRC)
                {
                    Name = SRC.UDList.Item(uname).Name,
                    Rank = Rank,
                    BossRank = BossRank,
                    Party = Party0,
                    ID = SRC.UList.CreateID(uname),
                    Status = "他形態",
                    x = x,
                    y = y,
                };
                foreach (var af in colOtherForm)
                {
                    af.AddOtherForm(u);
                    u.AddOtherForm(af);
                }
                u.AddOtherForm(this);
                AddOtherForm(u);

                SRC.UList.Add2(u);
                return u;
            }
            else
            {
                GUI.ErrorMessage("ユニットデータ「" + uname + "」が見つかりません");
            }
            return null;
        }

        // 指定した他形態が登録されているか？
        public bool IsOtherFormDefined(string uname)
        {
            bool IsOtherFormDefinedRet = default;
            foreach (Unit u in colOtherForm)
            {
                if ((u.Name ?? "") == (uname ?? ""))
                {
                    IsOtherFormDefinedRet = true;
                    return IsOtherFormDefinedRet;
                }
            }

            IsOtherFormDefinedRet = false;
            return IsOtherFormDefinedRet;
        }

        // 不要な形態を削除
        public void DeleteTemporaryOtherForm()
        {
            // 必要な形態の一覧を作成
            var neededForms = new HashSet<string> { Name };
            for (int i = 1; i <= CountFeature(); i++)
            {
                string fname = Feature(i).Name;
                string fdata = FeatureData(i);
                switch (fname)
                {
                    case "変形":
                        for (int j = 2; j <= GeneralLib.LLength(fdata); j++)
                            neededForms.Add(GeneralLib.LIndex(fdata, j));
                        break;
                    case "換装":
                    case "他形態":
                        for (int j = 1; j <= GeneralLib.LLength(fdata); j++)
                            neededForms.Add(GeneralLib.LIndex(fdata, j));
                        break;
                    case "ハイパーモード":
                    case "パーツ分離":
                    case "変形技":
                        neededForms.Add(GeneralLib.LIndex(fdata, 2));
                        break;
                    case "ノーマルモード":
                    case "パーツ合体":
                        neededForms.Add(GeneralLib.LIndex(fdata, 1));
                        break;
                }
            }

            // 他形態から必要ない形態へのリンクを削除
            foreach (var of in OtherForms.ToList())
            {
                if (of.Status == "他形態")
                {
                    foreach (var innerOf in of.OtherForms.ToList())
                    {
                        if (!neededForms.Contains(innerOf.Name))
                        {
                            of.DeleteOtherForm(innerOf.ID);
                        }
                    }
                }
            }

            // 必要ない形態を破棄し、リンクを削除
            foreach (var of in OtherForms.ToList())
            {
                if (!neededForms.Contains(of.Name))
                {
                    of.Status = "破棄";
                    DeleteOtherForm(of.ID);
                }
            }
        }
    }
}
