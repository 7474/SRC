using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.VB;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCCore
{
    public partial class SRC
    {
        private IList<InvalidSrcData> continuesErrors = new List<InvalidSrcData>();
        public IList<InvalidSrcData> DataErrors => continuesErrors;
        public bool HasDataError => continuesErrors.Any();
        public void ClearDataError()
        {
            continuesErrors.Clear();
        }
        public void AddDataError(InvalidSrcData error)
        {
            continuesErrors.Add(error);
        }

        // 作品new_titleのデータを読み込み
        public void IncludeData(string new_title)
        {
            string fpath;

            // ロードのインジケータ表示を行う
            // XXX frmNowLoading
            //if (My.MyProject.Forms.frmNowLoading.Visible)
            //{
            GUI.DisplayLoadingProgress();
            //}

            // Dataフォルダの場所を探す
            fpath = SearchDataFolder(new_title);
            if (Strings.Len(fpath) == 0)
            {
                GUI.ErrorMessage("データ「" + new_title + "」のフォルダが見つかりません");
                TerminateSRC();
            };
            LoadDataDirectory(fpath);

            return;
            //ErrorHandler:
            //    ;
            //    GUI.ErrorMessage(ref "Src.ini内のExtDataPathの値が不正です");
            //    TerminateSRC();
        }

        public void LoadDataDirectory(string fpath)
        {
            var aliasFilePath = FileSystem.PathCombine(fpath, "alias.txt");
            if (GeneralLib.FileExists(aliasFilePath))
            {
                ALDList.Load(aliasFilePath);
            }

            var mindFilePath = FileSystem.PathCombine(fpath, "mind.txt");
            var spFilePath = FileSystem.PathCombine(fpath, "sp.txt");
            if (GeneralLib.FileExists(mindFilePath))
            {
                SPDList.Load(mindFilePath);
            }
            else if (GeneralLib.FileExists(spFilePath))
            {
                SPDList.Load(spFilePath);
            }

            var pilotFilePath = FileSystem.PathCombine(fpath, "pilot.txt");
            if (GeneralLib.FileExists(pilotFilePath))
            {
                PDList.Load(pilotFilePath);
            }

            var nonPilotFilePath = FileSystem.PathCombine(fpath, "non_pilot.txt");
            if (GeneralLib.FileExists(nonPilotFilePath))
            {
                NPDList.Load(nonPilotFilePath);
            }

            var robotFilePath = FileSystem.PathCombine(fpath, "robot.txt");
            if (GeneralLib.FileExists(robotFilePath))
            {
                UDList.Load(robotFilePath);
            }

            var unitFilePath = FileSystem.PathCombine(fpath, "unit.txt");
            if (GeneralLib.FileExists(unitFilePath))
            {
                UDList.Load(unitFilePath);
            }

            //// ロードのインジケータ表示を行う
            //if (My.MyProject.Forms.frmNowLoading.Visible)
            //{
            //    GUI.DisplayLoadingProgress();
            //}

            var pilotMessageFilePath = FileSystem.PathCombine(fpath, "pilot_message.txt");
            if (GeneralLib.FileExists(pilotMessageFilePath))
            {
                MDList.Load(pilotMessageFilePath, false);
            }

            var pilotDialogFilePath = FileSystem.PathCombine(fpath, "pilot_dialog.txt");
            if (GeneralLib.FileExists(pilotDialogFilePath))
            {
                DDList.Load(pilotDialogFilePath);
            }

            var effectFilePath = FileSystem.PathCombine(fpath, "effect.txt");
            if (GeneralLib.FileExists(effectFilePath))
            {
                EDList.Load(effectFilePath, true);
            }

            var animationFilePath = FileSystem.PathCombine(fpath, "animation.txt");
            if (GeneralLib.FileExists(animationFilePath))
            {
                ADList.Load(animationFilePath, false);
            }

            var ext_animationFilePath = FileSystem.PathCombine(fpath, "ext_animation.txt");
            if (GeneralLib.FileExists(ext_animationFilePath))
            {
                EADList.Load(ext_animationFilePath, false);
            }

            var itemFilePath = FileSystem.PathCombine(fpath, "item.txt");
            if (GeneralLib.FileExists(itemFilePath))
            {
                IDList.Load(itemFilePath);
            }
        }

        private bool init_search_data_folder = false;
        private bool scenario_data_dir_exists = false;
        private bool extdata_data_dir_exists = false;
        private bool extdata2_data_dir_exists = false;
        private bool src_data_dir_exists = false;
        // データフォルダ fname を検索
        public string SearchDataFolder(string fname)
        {
            string SearchDataFolderRet = default;
            string fname2;

            // 初めて実行する際に、各フォルダにDataフォルダがあるかチェック
            if (!init_search_data_folder)
            {
                if (Strings.Len(Lib.FileSystem.Dir(FileSystem.PathCombine(ScenarioPath, "Data"), FileAttribute.Directory)) > 0)
                {
                    scenario_data_dir_exists = true;
                }

                if (Strings.Len(ExtDataPath) > 0 & (ScenarioPath ?? "") != (ExtDataPath ?? ""))
                {
                    if (Strings.Len(Lib.FileSystem.Dir(FileSystem.PathCombine(ExtDataPath, "Data"), FileAttribute.Directory)) > 0)
                    {
                        extdata_data_dir_exists = true;
                    }
                }

                if (Strings.Len(ExtDataPath2) > 0 & (ScenarioPath ?? "") != (ExtDataPath2 ?? ""))
                {
                    if (Strings.Len(Lib.FileSystem.Dir(FileSystem.PathCombine(ExtDataPath2, "Data"), FileAttribute.Directory)) > 0)
                    {
                        extdata2_data_dir_exists = true;
                    }
                }

                if ((ScenarioPath ?? "") != (AppPath ?? ""))
                {
                    if (Strings.Len(Lib.FileSystem.Dir(FileSystem.PathCombine(AppPath, "Data"), FileAttribute.Directory)) > 0)
                    {
                        src_data_dir_exists = true;
                    }
                }

                init_search_data_folder = true;
            }

            // フォルダを検索
            fname2 = FileSystem.PathCombine("Data", fname);
            if (scenario_data_dir_exists)
            {
                SearchDataFolderRet = FileSystem.PathCombine(ScenarioPath, fname2);
                if (Strings.Len(Lib.FileSystem.Dir(SearchDataFolderRet, FileAttribute.Directory)) > 0)
                {
                    return SearchDataFolderRet;
                }
            }

            if (extdata_data_dir_exists)
            {
                SearchDataFolderRet = FileSystem.PathCombine(ExtDataPath, fname2);
                if (Strings.Len(Lib.FileSystem.Dir(SearchDataFolderRet, FileAttribute.Directory)) > 0)
                {
                    return SearchDataFolderRet;
                }
            }

            if (extdata2_data_dir_exists)
            {
                SearchDataFolderRet = FileSystem.PathCombine(ExtDataPath2, fname2);
                if (Strings.Len(Lib.FileSystem.Dir(SearchDataFolderRet, FileAttribute.Directory)) > 0)
                {
                    return SearchDataFolderRet;
                }
            }

            if (src_data_dir_exists)
            {
                SearchDataFolderRet = FileSystem.PathCombine(AppPath, fname2);
                if (Strings.Len(Lib.FileSystem.Dir(SearchDataFolderRet, FileAttribute.Directory)) > 0)
                {
                    return SearchDataFolderRet;
                }
            }

            // フォルダが見つからなかった
            SearchDataFolderRet = "";
            return SearchDataFolderRet;
        }
    }
}
