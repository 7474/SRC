# GUI.bas

VB 版の `SRC/SRC_20121125/GUI.bas` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub LoadMainFormAndRegisterFlash | 337 | 24 | `SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.LoadMainFormAndRegisterFlash | 未確認 |  |
| Sub LoadForms | 364 | 112 | `SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.LoadForms | 未確認 |  |
| Sub SetNewGUIMode | 479 | 8 | `SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.SetNewGUIMode | 未確認 |  |
| Sub OpenMessageForm | 493 | 158 | `SRCRender/ScreenGUI.message.cs` ScreenGUI.OpenMessageForm<br>`SRCSharpForm/SRCSharpFormGUI.message.cs` SRCSharpFormGUI.OpenMessageForm | 未確認 |  |
| Sub CloseMessageForm | 653 | 7 | `SRCRender/ScreenGUI.message.cs` ScreenGUI.CloseMessageForm<br>`SRCSharpForm/SRCSharpFormGUI.message.cs` SRCSharpFormGUI.CloseMessageForm | 未確認 |  |
| Sub ClearMessageForm | 662 | 10 | `SRCRender/ScreenGUI.message.cs` ScreenGUI.ClearMessageForm<br>`SRCSharpForm/SRCSharpFormGUI.message.cs` SRCSharpFormGUI.ClearMessageForm | 未確認 |  |
| Sub UpdateMessageForm | 674 | 419 | `SRCRender/ScreenGUI.message.cs` ScreenGUI.UpdateMessageForm<br>`SRCSharpForm/SRCSharpFormGUI.message.cs` SRCSharpFormGUI.UpdateMessageForm | 未確認 |  |
| Sub SaveMessageFormStatus | 1095 | 5 | `SRCRender/ScreenGUI.message.cs` ScreenGUI.SaveMessageFormStatus<br>`SRCSharpForm/SRCSharpFormGUI.message.cs` SRCSharpFormGUI.SaveMessageFormStatus | 未確認 |  |
| Sub KeepMessageFormStatus | 1102 | 18 | `SRCRender/ScreenGUI.message.cs` ScreenGUI.KeepMessageFormStatus<br>`SRCSharpForm/SRCSharpFormGUI.message.cs` SRCSharpFormGUI.KeepMessageFormStatus | 未確認 |  |
| Sub DisplayMessage | 1125 | 486 | `SRCRender/ScreenGUI.message.cs` ScreenGUI.DisplayMessage<br>`SRCSharpForm/SRCSharpFormGUI.message.cs` SRCSharpFormGUI.DisplayMessage | 未確認 |  |
| Sub PrintMessage | 1613 | 212 | `SRCRender/ScreenGUI.message.cs` ScreenGUI.PrintMessage<br>`SRCSharpForm/SRCSharpFormGUI.message.cs` SRCSharpFormGUI.PrintMessage | 未確認 |  |
| Function MessageLen | 1827 | 29 | `SRCRender/ScreenGUI.message.cs` ScreenGUI.MessageLen<br>`SRCSharpForm/SRCSharpFormGUI.message.cs` SRCSharpFormGUI.MessageLen | 未確認 |  |
| Sub DisplayBattleMessage | 1858 | 724 | `SRCRender/ScreenGUI.message.cs` ScreenGUI.DisplayBattleMessage<br>`SRCSharpForm/SRCSharpFormGUI.message.cs` SRCSharpFormGUI.DisplayBattleMessage | 未確認 |  |
| Sub DisplaySysMessage | 2584 | 145 | `SRCRender/ScreenGUI.message.cs` ScreenGUI.DisplaySysMessage<br>`SRCSharpForm/SRCSharpFormGUI.message.cs` SRCSharpFormGUI.DisplaySysMessage | 未確認 |  |
| Sub SetupBackground | 2734 | 224 | `SRCRender/ScreenGUI.map.cs` ScreenGUI.SetupBackground<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.SetupBackground | 未確認 |  |
| Sub RedrawScreen | 2960 | 13 | `SRCRender/ScreenGUI.map.cs` ScreenGUI.RedrawScreen<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.RedrawScreen | 未確認 |  |
| Sub MaskScreen | 2975 | 6 | `SRCRender/ScreenGUI.map.cs` ScreenGUI.MaskScreen<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.MaskScreen | 未確認 |  |
| Sub RefreshScreen | 2984 | 9 | `SRCRender/ScreenGUI.map.cs` ScreenGUI.RefreshScreen<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.RefreshScreen | 未確認 |  |
| Sub RefreshScreenOld | 3000 | 272 | （見つからない） | 未確認 |  |
| Sub RefreshScreenNew | 3275 | 401 | （見つからない） | 未確認 |  |
| Sub DottedLine | 3680 | 22 | （見つからない） | 未確認 |  |
| Sub Center | 3704 | 24 | `SRCRender/ScreenGUI.map.cs` ScreenGUI.Center<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.Center | 未確認 |  |
| Function MapToPixelX | 3733 | 10 | `SRCRender/ScreenGUI.map.cs` ScreenGUI.MapToPixelX<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.MapToPixelX | 未確認 |  |
| Function MapToPixelY | 3744 | 3 | `SRCRender/ScreenGUI.map.cs` ScreenGUI.MapToPixelY<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.MapToPixelY | 未確認 |  |
| Function PixelToMapX | 3749 | 16 | `SRCRender/ScreenGUI.map.cs` ScreenGUI.PixelToMapX<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.PixelToMapX | 未確認 |  |
| Function PixelToMapY | 3766 | 9 | `SRCRender/ScreenGUI.map.cs` ScreenGUI.PixelToMapY<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.PixelToMapY | 未確認 |  |
| Function FindUnitBitmap | 3780 | 132 | （見つからない） | 未確認 |  |
| Function MakeUnitBitmap | 3914 | 75 | （見つからない） | 未確認 |  |
| Sub LoadUnitBitmap | 3991 | 183 | （見つからない） | 未確認 |  |
| Sub PaintUnitBitmap | 4176 | 89 | `SRCRender/ScreenGUI.map.cs` ScreenGUI.PaintUnitBitmap<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.PaintUnitBitmap | 未確認 |  |
| Sub EraseUnitBitmap | 4267 | 39 | `SRCRender/ScreenGUI.map.cs` ScreenGUI.EraseUnitBitmap<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.EraseUnitBitmap | 未確認 |  |
| Sub MoveUnitBitmap | 4308 | 151 | `SRCRender/ScreenGUI.map.cs` ScreenGUI.MoveUnitBitmap<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.MoveUnitBitmap | 未確認 |  |
| Sub MoveUnitBitmap2 | 4462 | 90 | `SRCRender/ScreenGUI.map.cs` ScreenGUI.MoveUnitBitmap2<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.MoveUnitBitmap2 | 未確認 |  |
| Function ListBox | 4557 | 197 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.ListBox<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.ListBox | 未確認 |  |
| Sub EnlargeListBoxHeight | 4756 | 15 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.EnlargeListBoxHeight<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.EnlargeListBoxHeight | 未確認 |  |
| Sub ReduceListBoxHeight | 4773 | 15 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.ReduceListBoxHeight<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.ReduceListBoxHeight | 未確認 |  |
| Sub EnlargeListBoxWidth | 4790 | 16 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.EnlargeListBoxWidth<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.EnlargeListBoxWidth | 未確認 |  |
| Sub ReduceListBoxWidth | 4808 | 16 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.ReduceListBoxWidth<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.ReduceListBoxWidth | 未確認 |  |
| Sub AddPartsToListBox | 4826 | 191 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.AddPartsToListBox<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.AddPartsToListBox | 未確認 |  |
| Sub RemovePartsOnListBox | 5019 | 32 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.RemovePartsOnListBox<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.RemovePartsOnListBox | 未確認 |  |
| Function WeaponListBox | 5053 | 369 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.WeaponListBox<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.WeaponListBox | 未確認 |  |
| Function AbilityListBox | 5424 | 229 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.AbilityListBox<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.AbilityListBox | 未確認 |  |
| Function LIPS | 5655 | 43 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.LIPS<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.LIPS | 未確認 |  |
| Function MultiColumnListBox | 5700 | 68 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.MultiColumnListBox<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.MultiColumnListBox | 未確認 |  |
| Function MultiSelectListBox | 5770 | 35 | `SRCRender/ScreenGUI.list.cs` ScreenGUI.MultiSelectListBox<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.MultiSelectListBox | 未確認 |  |
| Function DrawPicture | 5810 | 1868 | `SRCRender/ScreenGUI.draw.cs` ScreenGUI.DrawPicture<br>`SRCSharpForm/SRCSharpFormGUI.draw.cs` SRCSharpFormGUI.DrawPicture | 未確認 |  |
| Sub MakePicBuf | 7680 | 24 | （見つからない） | 未確認 |  |
| Function GetPicBuf | 7706 | 52 | （見つからない） | 未確認 |  |
| Sub ReleasePicBuf | 7760 | 8 | （見つからない） | 未確認 |  |
| Sub UsePicBuf | 7770 | 4 | （見つからない） | 未確認 |  |
| Sub DrawString | 7779 | 151 | `SRCRender/ScreenGUI.draw.cs` ScreenGUI.DrawString<br>`SRCSharpForm/SRCSharpFormGUI.draw.cs` SRCSharpFormGUI.DrawString | 未確認 |  |
| Sub DrawSysString | 7932 | 75 | `SRCRender/ScreenGUI.draw.cs` ScreenGUI.DrawSysString<br>`SRCSharpForm/SRCSharpFormGUI.draw.cs` SRCSharpFormGUI.DrawSysString | 未確認 |  |
| Sub SaveScreen | 8012 | 12 | `SRCRender/ScreenGUI.draw.cs` ScreenGUI.SaveScreen<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.SaveScreen | 未確認 |  |
| Sub ClearPicture | 8026 | 26 | `SRCRender/ScreenGUI.draw.cs` ScreenGUI.ClearPicture<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.ClearPicture | 未確認 |  |
| Sub ClearPicture2 | 8054 | 16 | `SRCRender/ScreenGUI.draw.cs` ScreenGUI.ClearPicture2<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.ClearPicture2 | 未確認 |  |
| Sub LockGUI | 8075 | 7 | `SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.LockGUI | 未確認 |  |
| Sub UnlockGUI | 8084 | 7 | `SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.UnlockGUI | 未確認 |  |
| Sub SaveCursorPos | 8096 | 9 | `SRCRender/ScreenGUI.animation.cs` ScreenGUI.SaveCursorPos<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.SaveCursorPos | 未確認 |  |
| Sub MoveCursorPos | 8107 | 127 | `SRCRender/ScreenGUI.animation.cs` ScreenGUI.MoveCursorPos<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.MoveCursorPos | 未確認 |  |
| Sub RestoreCursorPos | 8236 | 40 | `SRCRender/ScreenGUI.animation.cs` ScreenGUI.RestoreCursorPos<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.RestoreCursorPos | 未確認 |  |
| Sub OpenTitleForm | 8281 | 9 | `SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.OpenTitleForm | 未確認 |  |
| Sub CloseTitleForm | 8292 | 4 | `SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.CloseTitleForm | 未確認 |  |
| Sub OpenNowLoadingForm | 8301 | 10 | `SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.OpenNowLoadingForm | 未確認 |  |
| Sub CloseNowLoadingForm | 8313 | 5 | `SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.CloseNowLoadingForm | 未確認 |  |
| Sub DisplayLoadingProgress | 8320 | 4 | `SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.DisplayLoadingProgress | 未確認 |  |
| Sub SetLoadImageSize | 8326 | 6 | `SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.SetLoadImageSize | 未確認 |  |
| Sub ChangeDisplaySize | 8336 | 68 | `SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.ChangeDisplaySize | 未確認 |  |
| Sub ErrorMessage | 8409 | 22 | `SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.ErrorMessage | 未確認 |  |
| Sub DataErrorMessage | 8433 | 28 | `SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.DataErrorMessage | 未確認 |  |
| Function IsRButtonPressed | 8464 | 43 | `SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.IsRButtonPressed | 未確認 |  |
| Sub DisplayTelop | 8510 | 34 | `SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.DisplayTelop | 未確認 |  |
