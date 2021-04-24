using SRCCore.Lib;

namespace SRCCore.Units
{
    public partial class Unit
    {
        // 相手ユニットが敵かどうかを判定
        public bool IsEnemy(Unit t, bool for_move = false)
        {
            bool IsEnemyRet = default;
            string myparty, tparty;

            // 自分自身は常に味方
            if (ReferenceEquals(t, this))
            {
                IsEnemyRet = false;
                return IsEnemyRet;
            }

            // 暴走したユニットにとってはすべてが敵
            if (IsConditionSatisfied("暴走"))
            {
                IsEnemyRet = true;
                return IsEnemyRet;
            }

            // 混乱した場合はランダムで判定
            if (IsConditionSatisfied("混乱"))
            {
                if (for_move)
                {
                    IsEnemyRet = true;
                }
                else if (GeneralLib.Dice(2) == 1)
                {
                    IsEnemyRet = true;
                }
                else
                {
                    IsEnemyRet = false;
                }

                return IsEnemyRet;
            }

            myparty = Party;
            tparty = t.Party;

            // 味方ユニットは暴走、憑依、魅了したユニットを排除可能
            // (暴走した味方ユニットのPartyはＮＰＣとみなされる)
            if (myparty == "味方" & tparty == "ＮＰＣ")
            {
                if (t.IsConditionSatisfied("暴走") | t.IsConditionSatisfied("憑依") | t.IsConditionSatisfied("魅了"))
                {
                    IsEnemyRet = true;
                    return IsEnemyRet;
                }
            }

            if (myparty != "味方")
            {
                // ターゲットの陣営が限定されている場合、敵対関係にない陣営の
                // ユニットは味方と見なされる。
                // ただし、プレイヤーがコントロールするユニットはこのような自
                // 分を攻撃してこないユニットも排除可能。

                // 特定の陣営のみを狙う場合
                switch (Mode ?? "")
                {
                    case "味方":
                    case "ＮＰＣ":
                        {
                            switch (tparty ?? "")
                            {
                                case "味方":
                                case "ＮＰＣ":
                                    {
                                        IsEnemyRet = true;
                                        break;
                                    }

                                default:
                                    {
                                        IsEnemyRet = false;
                                        break;
                                    }
                            }

                            return IsEnemyRet;
                        }

                    case "敵":
                    case "中立":
                        {
                            if ((tparty ?? "") == (Mode ?? ""))
                            {
                                IsEnemyRet = true;
                            }
                            else
                            {
                                IsEnemyRet = false;
                            }

                            return IsEnemyRet;
                        }
                }

                // 相手が特定の陣営のみを狙う場合
                switch (t.Mode ?? "")
                {
                    case "味方":
                    case "ＮＰＣ":
                        {
                            switch (myparty ?? "")
                            {
                                case "味方":
                                case "ＮＰＣ":
                                    {
                                        IsEnemyRet = true;
                                        break;
                                    }

                                default:
                                    {
                                        IsEnemyRet = false;
                                        break;
                                    }
                            }

                            return IsEnemyRet;
                        }

                    case "敵":
                    case "中立":
                        {
                            if ((myparty ?? "") == (t.Mode ?? ""))
                            {
                                IsEnemyRet = true;
                            }
                            else
                            {
                                IsEnemyRet = false;
                            }

                            return IsEnemyRet;
                        }
                }
            }

            // 敵味方を判定
            switch (myparty ?? "")
            {
                case "味方":
                case "ＮＰＣ":
                    {
                        switch (tparty ?? "")
                        {
                            case "味方":
                            case "ＮＰＣ":
                                {
                                    IsEnemyRet = false;
                                    break;
                                }

                            default:
                                {
                                    IsEnemyRet = true;
                                    break;
                                }
                        }

                        break;
                    }

                default:
                    {
                        if ((myparty ?? "") == (tparty ?? ""))
                        {
                            IsEnemyRet = false;
                        }
                        else
                        {
                            IsEnemyRet = true;
                        }

                        break;
                    }
            }

            return IsEnemyRet;
        }

        // 相手ユニットが味方かどうかを判定
        public bool IsAlly(Unit t)
        {
            bool IsAllyRet = default;
            // 自分自身は常に味方
            if (ReferenceEquals(t, this))
            {
                IsAllyRet = true;
                return IsAllyRet;
            }

            // 暴走したユニットにとってはすべてが敵
            if (IsConditionSatisfied("暴走"))
            {
                IsAllyRet = false;
                return IsAllyRet;
            }

            // 混乱した場合はランダムで判定
            if (IsConditionSatisfied("混乱"))
            {
                if (GeneralLib.Dice(2) == 1)
                {
                    IsAllyRet = true;
                }
                else
                {
                    IsAllyRet = false;
                }

                return IsAllyRet;
            }

            // 敵味方を判定
            switch (Party ?? "")
            {
                case "味方":
                case "ＮＰＣ":
                    {
                        if (t.Party == "味方" | t.Party == "ＮＰＣ")
                        {
                            IsAllyRet = true;
                        }
                        else
                        {
                            IsAllyRet = false;
                        }

                        break;
                    }

                default:
                    {
                        if ((Party ?? "") == (t.Party ?? ""))
                        {
                            IsAllyRet = true;
                        }
                        else
                        {
                            IsAllyRet = false;
                        }

                        break;
                    }
            }

            return IsAllyRet;
        }
    }
}
