using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                // TODO Impl
                //string argmsg = "ユニットデータ「" + uname + "」が見つかりません";
                //GUI.ErrorMessage(argmsg);
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
            // TODO Impl
            //    string[] uarray;
            //    string fname, fdata;
            //    int k, i, j, n;

            //    // 必要な形態の一覧を作成
            //    n = 1;
            //    uarray = new string[2];
            //    uarray[1] = Name;
            //    var loopTo = CountFeature();
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        object argIndex1 = i;
            //        fname = Feature(argIndex1);
            //        switch (fname ?? "")
            //        {
            //            case "変形":
            //                {
            //                    object argIndex2 = fname;
            //                    fdata = FeatureData(argIndex2);
            //                    n = (n + GeneralLib.LLength(fdata) - 1);
            //                    Array.Resize(uarray, n + 1);
            //                    var loopTo1 = (GeneralLib.LLength(fdata) - 1);
            //                    for (j = 1; j <= loopTo1; j++)
            //                        uarray[n - j + 1] = GeneralLib.LIndex(fdata, (j + 1));
            //                    break;
            //                }

            //            case "換装":
            //            case "他形態":
            //                {
            //                    object argIndex3 = fname;
            //                    fdata = FeatureData(argIndex3);
            //                    n = (n + GeneralLib.LLength(fdata));
            //                    Array.Resize(uarray, n + 1);
            //                    var loopTo2 = GeneralLib.LLength(fdata);
            //                    for (j = 1; j <= loopTo2; j++)
            //                        uarray[n - j + 1] = GeneralLib.LIndex(fdata, j);
            //                    break;
            //                }

            //            case "ハイパーモード":
            //            case "パーツ分離":
            //            case "変形技":
            //                {
            //                    object argIndex4 = fname;
            //                    fdata = FeatureData(argIndex4);
            //                    n = (n + 1);
            //                    Array.Resize(uarray, n + 1);
            //                    uarray[n] = GeneralLib.LIndex(fdata, 2);
            //                    break;
            //                }

            //            case "ノーマルモード":
            //            case "パーツ合体":
            //                {
            //                    object argIndex5 = fname;
            //                    fdata = FeatureData(argIndex5);
            //                    n = (n + 1);
            //                    Array.Resize(uarray, n + 1);
            //                    uarray[n] = GeneralLib.LIndex(fdata, 1);
            //                    break;
            //                }
            //        }
            //    }

            //    // 他形態から必要ない形態へのリンクを削除
            //    var loopTo3 = CountOtherForm();
            //    for (i = 1; i <= loopTo3; i++)
            //    {
            //        object argIndex8 = i;
            //        {
            //            var withBlock = OtherForm(argIndex8);
            //            if (withBlock.Status_Renamed == "他形態")
            //            {
            //                j = 1;
            //                while (j <= withBlock.CountOtherForm())
            //                {
            //                    object argIndex6 = j;
            //                    {
            //                        var withBlock1 = withBlock.OtherForm(argIndex6);
            //                        var loopTo4 = n;
            //                        for (k = 1; k <= loopTo4; k++)
            //                        {
            //                            if ((withBlock1.Name ?? "") == (uarray[k] ?? ""))
            //                            {
            //                                break;
            //                            }
            //                        }
            //                    }

            //                    if (k > n)
            //                    {
            //                        object argIndex7 = j;
            //                        withBlock.DeleteOtherForm(argIndex7);
            //                    }
            //                    else
            //                    {
            //                        j = (j + 1);
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // 必要ない形態を破棄し、リンクを削除
            //    i = 1;
            //    while (i <= CountOtherForm())
            //    {
            //        object argIndex9 = i;
            //        {
            //            var withBlock2 = OtherForm(argIndex9);
            //            var loopTo5 = n;
            //            for (j = 1; j <= loopTo5; j++)
            //            {
            //                if ((withBlock2.Name ?? "") == (uarray[j] ?? ""))
            //                {
            //                    break;
            //                }
            //            }
            //        }

            //        if (j > n)
            //        {
            //            Unit localOtherForm() { object argIndex1 = i; var ret = OtherForm(argIndex1); return ret; }

            //            localOtherForm().Status_Renamed = "破棄";
            //            object argIndex10 = i;
            //            DeleteOtherForm(argIndex10);
            //        }
            //        else
            //        {
            //            i = (i + 1);
            //        }
            //    }
        }
    }
}
