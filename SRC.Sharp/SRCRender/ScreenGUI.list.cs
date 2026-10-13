using SRCCore;
using SRCCore.Commands;
using SRCCore.Lib;
using SRCCore.Units;
using System.Collections.Generic;
using System.Linq;

namespace SRCRender
{
    public partial class ScreenGUI
    {
        private volatile bool waitingList;
        private IList<UiCommand> menuCommands;
        private bool isUnitCommandMenu;

        public int ListBox(ListBoxArgs args)
        {
            return ShowList(args, -1);
        }

        private int ShowList(ListBoxArgs args, int timeLimitMillis)
        {
            var list = new ListState
            {
                Caption = args.lb_caption,
                Info = args.lb_info,
                Items = args.Items.Select(x => new ListItemState(x.Text, args.HasFlag && x.ListItemFlag)).ToList(),
                Highlight = -1,
            };
            // 選択待ちの状態にしてから表示する（表示直後のクリックを取りこぼさないため）
            var displayOnly = args.lb_mode == "表示のみ";
            listResult = -1;
            waitingList = !displayOnly;
            State.List = list;
            Present();

            if (displayOnly)
            {
                // 表示だけ行い、CloseListBox で閉じる
                Commands.SelectedItem = 0;
                return 0;
            }

            WaitUntil(() => listResult >= 0, timeLimitMillis);
            waitingList = false;
            var result = listResult < 0 ? 0 : listResult;

            if (args.lb_mode != "連続表示")
            {
                State.List = null;
            }
            Present();
            Commands.SelectedItem = result;
            return result;
        }

        public void CloseListBox()
        {
            State.List = null;
            Present();
        }

        public void EnlargeListBoxHeight()
        {
        }

        public void ReduceListBoxHeight()
        {
        }

        public void EnlargeListBoxWidth()
        {
        }

        public void ReduceListBoxWidth()
        {
        }

        public void AddPartsToListBox()
        {
        }

        public void RemovePartsOnListBox()
        {
        }

        public UnitWeapon WeaponListBox(Unit u, UnitWeaponList weapons, string caption_msg, string lb_mode, string BGM = "")
        {
            var ret = ListBox(new ListBoxArgs
            {
                Items = weapons.Items.Select(x => new ListBoxItem
                {
                    Text = GeneralLib.RightPaddedString(x.Weapon.Name, 24)
                        + " " + GeneralLib.LeftPaddedString(x.Weapon.WeaponPower("") + "", 5)
                        + " " + GeneralLib.LeftPaddedString(x.Weapon.WeaponPrecision() + "", 4),
                    ListItemFlag = !x.CanUse,
                }).ToList(),
                HasFlag = true,
                lb_caption = caption_msg,
                lb_info = GeneralLib.RightPaddedString("名称", 24) + "  攻撃 命中",
                lb_mode = lb_mode,
            });
            return ret > 0 ? weapons.Items[ret - 1].Weapon : null;
        }

        public UnitAbility AbilityListBox(Unit u, UnitAbilityList abilities, string caption_msg, string lb_mode, bool is_item = false)
        {
            var ret = ListBox(new ListBoxArgs
            {
                Items = abilities.Items.Select(x => new ListBoxItem
                {
                    Text = x.Ability.AbilityNickname(),
                    ListItemFlag = !x.CanUse,
                }).ToList(),
                HasFlag = true,
                lb_caption = caption_msg,
                lb_mode = lb_mode,
            });
            return ret > 0 ? abilities.Items[ret - 1].Ability : null;
        }

        public int LIPS(ListBoxArgs args, int time_limit)
        {
            return ShowList(args, time_limit * 1000);
        }

        public int MultiColumnListBox(ListBoxArgs args, bool is_center)
        {
            return ListBox(args);
        }

        public int MultiSelectListBox(ListBoxArgs args, int max_num)
        {
            // TODO 複数選択。現状は1件のみ選択できる
            var ret = ListBox(args);
            if (ret > 0)
            {
                args.Items[ret - 1].ListItemFlag = true;
                return 1;
            }
            return 0;
        }

        public void ShowUnitCommandMenu(IList<UiCommand> commands)
        {
            ShowMenu(commands, true);
        }

        public void ShowMapCommandMenu(IList<UiCommand> commands)
        {
            ShowMenu(commands, false);
        }

        private void ShowMenu(IList<UiCommand> commands, bool isUnit)
        {
            menuCommands = commands;
            isUnitCommandMenu = isUnit;
            State.Menu = new MenuState
            {
                Items = commands.Select(x => x.Label).ToList(),
                Checked = commands.Select(x => x.IsChecked).ToList(),
                X = (int)MouseX,
                Y = (int)MouseY + 6,
            };
            Present();
        }
    }
}
