using System.Linq;

namespace SRCCore.Units
{
    public partial class Unit
    {
        // === 召喚ユニット関連処理 ===

        // 召喚ユニットを追加
        public void AddServant(Unit u)
        {
            // 既に登録している？
            if (Servant(u.ID) != null)
            {
                return;
            }

            colServant.Add(u, u.ID);
        }

        // 召喚ユニットを削除
        public void DeleteServant(string Index)
        {
            if (colServant.Remove(Index))
            {
                return;
            }
            else
            {
                var u = colServant.List.FirstOrDefault(x => x.Name == Index);
                if (u != null)
                {
                    colServant.Remove(u);
                }
            }
        }

        // 召喚ユニット総数
        public int CountServant()
        {
            return colServant.Count;
        }

        // 召喚ユニット
        public Unit Servant(string Index)
        {
            Unit u = colServant[Index];
            if (u != null)
            {
                return u;
            }
            else
            {
                u = colServant.List.FirstOrDefault(x => x.Name == Index);
                if (u != null)
                {
                    return u;
                }
            }
            return null;
        }

        // 召喚ユニットを解放する
        public void DismissServant()
        {
            foreach (var u in colServant.List)
            {
                var cf = u.CurrentForm();
                switch (cf.Status ?? "")
                {
                    case "出撃":
                    case "格納":
                        cf.Escape();
                        cf.Status = "破棄";
                        break;

                    case "旧主形態":
                    case "旧形態":
                        foreach (var fd in cf.Features.Where(x => x.Name == "合体"))
                        {
                            var uname = fd.DataL.Skip(1).FirstOrDefault() ?? "";
                            if (SRC.UList.IsDefined(uname))
                            {
                                SRC.UList.Item(uname).CurrentForm().Split();
                            }
                        }
                        var lastf = cf.CurrentForm();
                        if (lastf.Status == "出撃" || lastf.Status == "格納")
                        {
                            lastf.Escape();
                            lastf.Status = "破棄";
                        }
                        break;
                }
            }
            colServant.Clear();
        }


        // === 隷属ユニット関連処理 ===

        // 隷属ユニットを追加
        public void AddSlave(Unit u)
        {
            colSlave.Add(u, u.ID);
        }

        // 隷属ユニットを削除
        public void DeleteSlave(string Index)
        {
            if (colServant.Remove(Index))
            {
                return;
            }
            else
            {
                var u = colSlave.List.FirstOrDefault(x => x.Name == Index);
                if (u != null)
                {
                    colSlave.Remove(u);
                }
            }
        }

        // 隷属ユニット総数
        public int CountSlave()
        {
            return colSlave.Count;
        }

        // 隷属ユニット
        public Unit Slave(string Index)
        {
            Unit u = colSlave[Index];
            if (u != null)
            {
                return u;
            }
            else
            {
                u = colSlave.List.FirstOrDefault(x => x.Name == Index);
                if (u != null)
                {
                    return u;
                }
            }
            return null;
        }

        // 隷属ユニットを解放する
        public void DismissSlave()
        {
            foreach (var u in colSlave.List)
            {
                var cf = u.CurrentForm();
                if (cf.IsConditionSatisfied("魅了") && cf.Master?.CurrentForm() == this)
                {
                    cf.DeleteCondition("魅了");
                    cf.Master = null;
                }
                if (cf.IsConditionSatisfied("憑依") && cf.Master?.CurrentForm() == this)
                {
                    cf.DeleteCondition("憑依");
                    cf.Master = null;
                }
            }
            colSlave.Clear();
        }
    }
}
