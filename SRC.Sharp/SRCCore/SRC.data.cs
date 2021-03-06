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
            //if (My.MyProject.Forms.frmNowLoading.Visible)
            //{
            GUI.DisplayLoadingProgress();
            //}

            // Dataフォルダの場所を探す
            fpath = SearchDataFolder(new_title);
            if (Strings.Len(fpath) == 0)
            {
                string argmsg = "データ「" + new_title + "」のフォルダが見つかりません";
                GUI.ErrorMessage(argmsg);
                TerminateSRC();
            };
            LoadDataDirectory(fpath);

            return;
            //ErrorHandler:
            //    ;
            //    string argmsg1 = "Src.ini内のExtDataPathの値が不正です";
            //    GUI.ErrorMessage(ref argmsg1);
            //    TerminateSRC();
        }

        public void LoadDataDirectory(string fpath)
        {
            var aliasFilePath = Path.Combine(fpath, "alias.txt");
            if (GeneralLib.FileExists(aliasFilePath))
            {
                ALDList.Load(aliasFilePath);
            }

            //string argfname3 = fpath + @"\mind.txt";
            //string argfname4 = fpath + @"\sp.txt";
            //if (GeneralLib.FileExists(ref argfname4))
            //{
            //    string argfname2 = fpath + @"\sp.txt";
            //    SPDList.Load(ref argfname2);
            //}
            //else if (GeneralLib.FileExists(ref argfname3))
            //{
            //    SPDList.Load(ref argfname3);
            //}

            var pilotFilePath = Path.Combine(fpath, "pilot.txt");
            if (GeneralLib.FileExists(pilotFilePath))
            {
                PDList.Load(pilotFilePath);
            }

            var nonPilotFilePath = Path.Combine(fpath, "non_pilot.txt");
            if (GeneralLib.FileExists(nonPilotFilePath))
            {
                NPDList.Load(nonPilotFilePath);
            }

            var robotFilePath = Path.Combine(fpath, "robot.txt");
            if (GeneralLib.FileExists(robotFilePath))
            {
                UDList.Load(robotFilePath);
            }

            var unitFilePath = Path.Combine(fpath, "unit.txt");
            if (GeneralLib.FileExists(unitFilePath))
            {
                UDList.Load(unitFilePath);
            }

            //// ロードのインジケータ表示を行う
            //if (My.MyProject.Forms.frmNowLoading.Visible)
            //{
            //    GUI.DisplayLoadingProgress();
            //}

            var pilotMessageFilePath = Path.Combine(fpath, "pilot_message.txt");
            if (GeneralLib.FileExists(pilotMessageFilePath))
            {
                MDList.Load(pilotMessageFilePath, false);
            }

            var pilotDialogFilePath = Path.Combine(fpath, "pilot_dialog.txt");
            if (GeneralLib.FileExists(pilotDialogFilePath))
            {
                DDList.Load(pilotDialogFilePath);
            }

            //string argfname18 = fpath + @"\effect.txt";
            //if (GeneralLib.FileExists(ref argfname18))
            //{
            //    string argfname17 = fpath + @"\effect.txt";
            //    EDList.Load(ref argfname17);
            //}

            //string argfname20 = fpath + @"\animation.txt";
            //if (GeneralLib.FileExists(ref argfname20))
            //{
            //    string argfname19 = fpath + @"\animation.txt";
            //    ADList.Load(ref argfname19);
            //}

            //string argfname22 = fpath + @"\ext_animation.txt";
            //if (GeneralLib.FileExists(ref argfname22))
            //{
            //    string argfname21 = fpath + @"\ext_animation.txt";
            //    EADList.Load(ref argfname21);
            //}

            var itemFilePath = Path.Combine(fpath, "item.txt");
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
                if (Strings.Len(FileSystem.Dir(Path.Combine(ScenarioPath, "Data"), FileAttribute.Directory)) > 0)
                {
                    scenario_data_dir_exists = true;
                }

                if (Strings.Len(ExtDataPath) > 0 & (ScenarioPath ?? "") != (ExtDataPath ?? ""))
                {
                    if (Strings.Len(FileSystem.Dir(Path.Combine(ExtDataPath, "Data"), FileAttribute.Directory)) > 0)
                    {
                        extdata_data_dir_exists = true;
                    }
                }

                if (Strings.Len(ExtDataPath2) > 0 & (ScenarioPath ?? "") != (ExtDataPath2 ?? ""))
                {
                    if (Strings.Len(FileSystem.Dir(Path.Combine(ExtDataPath2, "Data"), FileAttribute.Directory)) > 0)
                    {
                        extdata2_data_dir_exists = true;
                    }
                }

                if ((ScenarioPath ?? "") != (AppPath ?? ""))
                {
                    if (Strings.Len(FileSystem.Dir(Path.Combine(AppPath, "Data"), FileAttribute.Directory)) > 0)
                    {
                        src_data_dir_exists = true;
                    }
                }

                init_search_data_folder = true;
            }

            // フォルダを検索
            fname2 = Path.Combine("Data", fname);
            if (scenario_data_dir_exists)
            {
                SearchDataFolderRet = Path.Combine(ScenarioPath, fname2);
                if (Strings.Len(FileSystem.Dir(SearchDataFolderRet, FileAttribute.Directory)) > 0)
                {
                    return SearchDataFolderRet;
                }
            }

            if (extdata_data_dir_exists)
            {
                SearchDataFolderRet = Path.Combine(ExtDataPath, fname2);
                if (Strings.Len(FileSystem.Dir(SearchDataFolderRet, FileAttribute.Directory)) > 0)
                {
                    return SearchDataFolderRet;
                }
            }

            if (extdata2_data_dir_exists)
            {
                SearchDataFolderRet = Path.Combine(ExtDataPath2, fname2);
                if (Strings.Len(FileSystem.Dir(SearchDataFolderRet, FileAttribute.Directory)) > 0)
                {
                    return SearchDataFolderRet;
                }
            }

            if (src_data_dir_exists)
            {
                SearchDataFolderRet = Path.Combine(AppPath, fname2);
                if (Strings.Len(FileSystem.Dir(SearchDataFolderRet, FileAttribute.Directory)) > 0)
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
