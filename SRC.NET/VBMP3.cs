using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    static class VBMP3
    {

        // VBMP3.BAS
        // VisualBasic用 MP3操作DLL 関数宣言ファイル
        // UPGRADE_WARNING: 構造体 WAVE_FORM に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_encodeOpen(string pszWaveName, ref WAVE_FORM pWaveForm);
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_encodeStart(string pszMp3Name);
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_encodeStop();
        [DllImport("VBMP3.dll")]
        static extern int vbmp3_getEncodeState(ref int readSize, ref int encodeSize);
        [DllImport("VBMP3.dll")]
        static extern int vbmp3_getPlaySamples();
        [DllImport("VBMP3.dll")]
        static extern int vbmp3_getTotalSamples();
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_setPlaySamples(int samples);
        [DllImport("VBMP3.dll")]
        static extern int vbmp3_getPlayFlames();
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_setPlayFlames(int flames);
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_setLyricsFile(string pszLyricsName);
        // UPGRADE_WARNING: 構造体 LYRICS_INFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_getLyrics(ref LYRICS_INFO pLyricsInfo);
        [DllImport("VBMP3.dll")]
        static extern void vbmp3_getSpectrum(ref int pSpecL, ref int pSpecR);
        [DllImport("VBMP3.dll")]
        static extern void vbmp3_getWave(ref int pWaveL, ref int pWaveR);
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_decodeWave(string pszWaveName);
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_startCallback();
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_stopCallback();
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_callback(int pProc);
        [DllImport("VBMP3.dll")]
        static extern int vbmp3_getFileType(string pszName);
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_cutMacBinary(string pszName);
        // UPGRADE_WARNING: 構造体 LIST_INFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_setListInfo(string pszName, ref LIST_INFO pListInfo);
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_changeRmp(string pszName);
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_changeMp3(string pszName);
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_changeWav(string pszName);
        // UPGRADE_WARNING: 構造体 LIST_INFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_getListInfo(ref LIST_INFO pListInfo);
        // UPGRADE_WARNING: 構造体 TAG_INFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_getFileTagInfo(string pszName, ref TAG_INFO pTagInfo);
        // UPGRADE_WARNING: 構造体 MPEG_INFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        // UPGRADE_WARNING: 構造体 TAG_INFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_getFileInfo(string pszName, ref TAG_INFO pTagInfo, ref MPEG_INFO pMpegTagInfo);
        // UPGRADE_WARNING: 構造体 LIST_INFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        // UPGRADE_WARNING: 構造体 MPEG_INFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        // UPGRADE_WARNING: 構造体 TAG_INFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_getFileInfo2(string pszName, ref TAG_INFO pTagInfo, ref MPEG_INFO pMpegTagInfo, ref LIST_INFO pListInfo);
        [DllImport("VBMP3.dll")]
        static extern int vbmp3_debug();
        // UPGRADE_WARNING: 構造体 TAG_INFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_getGenre(ref TAG_INFO pTagInfo);
        [DllImport("VBMP3.dll")]
        static extern int vbmp3_getWinampPlayMs();
        [DllImport("VBMP3.dll")]
        static extern int vbmp3_getWinampTotalSec();
        [DllImport("VBMP3.dll")]
        static extern int vbmp3_getPlayBitRate();
        [DllImport("VBMP3.dll")]
        static extern int vbmp3_getLastErrorNo();
        // UPGRADE_WARNING: 構造体 VBMP3_OPTION に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("VBMP3.dll")]
        static extern int vbmp3_setVbmp3Option(ref VBMP3_OPTION pVbmp3Option);
        // UPGRADE_WARNING: 構造体 VBMP3_OPTION に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("VBMP3.dll")]
        static extern void vbmp3_getVbmp3Option(ref VBMP3_OPTION pVbmp3Option);
        // UPGRADE_WARNING: 構造体 DEC_OPTION に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("VBMP3.dll")]
        static extern int vbmp3_setDecodeOption(ref DEC_OPTION pDecOption);
        // UPGRADE_WARNING: 構造体 DEC_OPTION に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("VBMP3.dll")]
        static extern void vbmp3_getDecodeOption(ref DEC_OPTION pDecOption);
        [DllImport("VBMP3.dll")]
        static extern void vbmp3_setEqualizer(ref int pTable);
        [DllImport("VBMP3.dll")]
        static extern int vbmp3_getVersion();
        // UPGRADE_WARNING: 構造体 TAG_INFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_getTagInfo(ref TAG_INFO pTagInfo);
        // UPGRADE_WARNING: 構造体 TAG_INFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_setTagInfo(string pszName, ref TAG_INFO pTagInfo, int tagSet = 0, int tagAdd = 0);
        // UPGRADE_WARNING: 構造体 MPEG_INFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_getMpegInfo(ref MPEG_INFO pMpegTagInfo);
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_init();
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_free();
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_setStepPitch(int pitch, int frames = 5);
        [DllImport("VBMP3.dll")]
        static extern int vbmp3_getStepPitch();
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_reload();
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_setPitch(int pitch);
        [DllImport("VBMP3.dll")]
        static extern int vbmp3_getPitch();
        // UPGRADE_WARNING: 構造体 InputInfo に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_open(string pszName, ref InputInfo pInfo);
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_close();
        [DllImport("VBMP3.dll")]
        static extern int vbmp3_getState(ref int sec);
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_play();
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_stop();
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_pause();
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_restart();
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_seek(int sec);
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_setVolume(int lVol, int rVol);
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_getVolume(ref int lVol, ref int rVol);
        [DllImport("VBMP3.dll")]
        static extern void vbmp3_setFadeIn(int fin);
        [DllImport("VBMP3.dll")]
        static extern void vbmp3_setFadeOut(int fout);
        [DllImport("VBMP3.dll")]
        static extern void vbmp3_fadeOut();

        // 未使用
        [DllImport("VBMP3.dll")]
        static extern void vbmp3_startAnalyzeThread();
        [DllImport("VBMP3.dll")]
        static extern void vbmp3_stopAnalyzeThread();
        [DllImport("VBMP3.dll")]
        static extern void vbmp3_startAnalyze();
        [DllImport("VBMP3.dll")]
        static extern void vbmp3_stopAnalyze();
        // UPGRADE_WARNING: 構造体 WAVE_DATA に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("VBMP3.dll")]
        static extern bool vbmp3_getWaveData(ref WAVE_DATA pWaveData);

        public struct InputInfo
        {
            // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] szTrackName; // 曲名
                                       // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] szArtistName; // ｱｰﾃｨｽﾄ名
            public int channels; // ﾁｬﾝﾈﾙ数
            public int bitRate; // ﾋﾞｯﾄﾚｰﾄ(kbit/s)
            public int samplingRate; // ｻﾝﾌﾟﾙﾚｰﾄ(Hz)
            public int totalSec; // 演奏時間(s)
        }

        public struct TAG_INFO
        {
            // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] szTrackName; // 曲名
                                       // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] szArtistName; // ｱｰﾃｨｽﾄ名
                                        // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] szAlbumName; // ｱﾙﾊﾞﾑ名
                                       // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(5)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public char[] szYear; // ﾘﾘｰｽ年号
                                  // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] szComment; // ｺﾒﾝﾄ
            public int genre; // ｼﾞｬﾝﾙ
                              // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] szGenreName; // ｼﾞｬﾝﾙ名
        }

        public struct MPEG_INFO
        {
            public int version; // ﾊﾞｰｼﾞｮﾝ        1:MPEG-1, 2:MPEG-2, 3:MPEG-2.5
            public int layer; // ﾚｲﾔ            1:Layer1. 2:Layer2, 3:Layer3
            public int crcDisable; // ｴﾗｰ保護        0:無効, 1:有効
            public int extension; // ｴｸｽﾃﾝｼｮﾝ       0:なし, 1:ﾌﾟﾗｲﾍﾞｰﾄ
            public int Mode; // ﾁｬﾝﾈﾙﾓｰﾄﾞ      0:Stereo, 1:Joint stereo, 3:Dual channel, 4:Mono
            public int copyright; // 著作権         0:著作権保護あり, 1:著作権保護なし
            public int original; // ｵﾘｼﾞﾅﾙ         0:ｺﾋﾟｰ, 1:ｵﾘｼﾞﾅﾙ
            public int emphasis; // ｴﾝﾌｫｼｽ         0:None, 1:50/15ms, 2:Reserved, 3:CCITT j.17
            public int channels; // ﾁｬﾝﾈﾙ数
            public int bitRate; // ﾋﾞｯﾄﾚｰﾄ(kbit/s)(0 なら VBR形式)
            public int samplingRate; // ｻﾝﾌﾟﾙﾚｰﾄ(Hz)
            public int fileSize; // ﾌｧｲﾙｻｲｽﾞ(Byte)
            public int flames; // ﾌﾚｰﾑ数
            public int totalSec; // 演奏時間(s)
        }

        public struct DEC_OPTION
        {
            public int reduction; // サンプリング 0:1/1 1:1/2 2:1/4 [Default = 0]
            public int convert; // チャンネル 0:ステレオ 1:モノラル[Default = 0]
            public int freqLimit; // 周波数[Default = 24000]
        }

        public struct VBMP3_OPTION
        {
            public int inputBlock; // 入力フレーム数[Default = 40]
            public int outputBlock; // 出力フレーム数[Default = 30]
            public int inputSleep; // 入力直後のｽﾘｰﾌﾟ時間(ﾐﾘ秒)[Default = 5]
            public int outputSleep; // 出力直後のｽﾘｰﾌﾟ時間(ﾐﾘ秒)[Default = 0]
        }

        public struct LIST_INFO
        {
            // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] INAM; // 曲名
                                // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] IART; // アーティスト名
                                // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] IPRD; // 製品名
                                // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] ICMT; // コメント文字列
                                // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] ICRD; // リリース年号
                                // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] IGNR; // ジャンル名
                                // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] ICOP; // 著作権
                                // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] IENG; // エンジニア
                                // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] ISRC; // ソース
                                // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] ISFT; // ソフトウェア
                                // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] IKEY; // キーワード
                                // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] ITCH; // 技術者
                                // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] ILYC; // 歌詞
                                // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] ICMS; // コミッション
        }

        public struct LYRICS_INFO
        {
            public int sec;
            // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] LyricsNext2;
            // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] LyricsNext1;
            // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] LyricsCurrent;
            // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] LyricsPrev1;
            // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] LyricsPrev2;
        }

        public struct WAVE_DATA
        {
            public int channels;
            public int bitsPerSample;
            // UPGRADE_NOTE: Left は Left_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
            public int Left_Renamed;
            // UPGRADE_NOTE: Right は Right_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
            public int Right_Renamed;
        }

        public struct WAVE_FORM
        {
            public int channels;
            public int bitsPerSample;
            public int samplingRate;
            public int dataSize;
        }

        // ファイルタイプ定数
        public const short FT_NOMAL = 0;
        public const short FT_WAVE = 1;
        public const short FT_RMP = 2;
        public const short FT_ID3V2 = 4;
        public const short FT_MAC = 8;
        public const short FT_ID3V1 = 16;

        // コールバック関数用
        public const short MSG_ERROR = 0;
        public const short MSG_STOPING = 1;
        public const short MSG_PLAYING = 2;
        public const short MSG_PAUSING = 3;
        public const short MSG_PLAYDONE = 4;

        public enum vbmp3_errNo
        {
            ERR_MP3_FILE_OPEN = 1,
            ERR_MP3_FILE_NOT_OPEN = 2,
            ERR_MP3_FILE_READ = 3,
            ERR_MP3_FILE_WRITE = 4,
            ERR_WAV_FILE_OPEN = 5,
            ERR_WAV_FORMAT = 6,
            ERR_ENCODE_FILE_OPEN = 7,
            ERR_LYRICS_FILE_OPEN = 8,
            ERR_LYRICS_NON_DATA = 9,
            ERR_FRAME_HEADER_NOT_FOUND = 10,
            ERR_FRAME_HEADER_READ = 11,
            ERR_STATE_STOP = 12,
            ERR_NOT_STATE_STOP = 13,
            ERR_NOT_STATE_PLAY = 14,
            ERR_STATE_NON_ENCODE = 15,
            ERR_PLAY = 16,
            ERR_STOP = 17,
            ERR_INVALID_VALUE = 18,
            ERR_MALLOC = 19,
            ERR_NON_RIFF = 20,
            ERR_RIFF = 21,
            ERR_NOT_MP3 = 22,
            ERR_MAC_BIN = 23,
            ERR_UNKNOWN_FILE = 24,
            ERR_OPEN_OUT_DEVICE = 25,
            ERR_DECODE = 26,
            ERR_DECODE_THREAD = 27,
            ERR_ENCODE_THREAD = 28,
            ERR_CREATE_EVENT = 29,
            ERR_CODEC_NOT_FOUND = 30,
            ERR_WAVE_TABLE_NOT_FOUND = 31,
            ERR_ACM_OPEN = 32
        }


        // ---------------------------------------------------------
        // 関数：Function NTrim()
        // 機能：\0 以降の文字列削除
        // 引数：Word  : 変換元文字列
        // 戻り値：変換後文字列
        // ---------------------------------------------------------
        public static string NTrim(ref string Word)
        {
            string NTrimRet = default;
            if (Strings.InStr(Word, Conversions.ToString('\0')) > 0)
            {
                NTrimRet = Strings.Left(Word, Strings.InStr(Word, Conversions.ToString('\0')) - 1);
            }
            else
            {
                NTrimRet = Word;
            }

            return NTrimRet;
        }
    }
}