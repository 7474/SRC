Option Strict Off
Option Explicit On
Module VBMP3
	
	'VBMP3.BAS
	'VisualBasic用 MP3操作DLL 関数宣言ファイル
	'UPGRADE_WARNING: 構造体 WAVE_FORM に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Declare Function vbmp3_encodeOpen Lib "VBMP3.dll" (ByVal pszWaveName As String, ByRef pWaveForm As WAVE_FORM) As Boolean
	Declare Function vbmp3_encodeStart Lib "VBMP3.dll" (ByVal pszMp3Name As String) As Boolean
	Declare Function vbmp3_encodeStop Lib "VBMP3.dll" () As Boolean
	Declare Function vbmp3_getEncodeState Lib "VBMP3.dll" (ByRef readSize As Integer, ByRef encodeSize As Integer) As Integer
	Declare Function vbmp3_getPlaySamples Lib "VBMP3.dll" () As Integer
	Declare Function vbmp3_getTotalSamples Lib "VBMP3.dll" () As Integer
	Declare Function vbmp3_setPlaySamples Lib "VBMP3.dll" (ByVal samples As Integer) As Boolean
	Declare Function vbmp3_getPlayFlames Lib "VBMP3.dll" () As Integer
	Declare Function vbmp3_setPlayFlames Lib "VBMP3.dll" (ByVal flames As Integer) As Boolean
	Declare Function vbmp3_setLyricsFile Lib "VBMP3.dll" (ByVal pszLyricsName As String) As Boolean
	'UPGRADE_WARNING: 構造体 LYRICS_INFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Declare Function vbmp3_getLyrics Lib "VBMP3.dll" (ByRef pLyricsInfo As LYRICS_INFO) As Boolean
	Declare Sub vbmp3_getSpectrum Lib "VBMP3.dll" (ByRef pSpecL As Integer, ByRef pSpecR As Integer)
	Declare Sub vbmp3_getWave Lib "VBMP3.dll" (ByRef pWaveL As Integer, ByRef pWaveR As Integer)
	Declare Function vbmp3_decodeWave Lib "VBMP3.dll" (ByVal pszWaveName As String) As Boolean
	Declare Function vbmp3_startCallback Lib "VBMP3.dll" () As Boolean
	Declare Function vbmp3_stopCallback Lib "VBMP3.dll" () As Boolean
	Declare Function vbmp3_callback Lib "VBMP3.dll" (ByVal pProc As Integer) As Boolean
	Declare Function vbmp3_getFileType Lib "VBMP3.dll" (ByVal pszName As String) As Integer
	Declare Function vbmp3_cutMacBinary Lib "VBMP3.dll" (ByVal pszName As String) As Boolean
	'UPGRADE_WARNING: 構造体 LIST_INFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Declare Function vbmp3_setListInfo Lib "VBMP3.dll" (ByVal pszName As String, ByRef pListInfo As LIST_INFO) As Boolean
	Declare Function vbmp3_changeRmp Lib "VBMP3.dll" (ByVal pszName As String) As Boolean
	Declare Function vbmp3_changeMp3 Lib "VBMP3.dll" (ByVal pszName As String) As Boolean
	Declare Function vbmp3_changeWav Lib "VBMP3.dll" (ByVal pszName As String) As Boolean
	'UPGRADE_WARNING: 構造体 LIST_INFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Declare Function vbmp3_getListInfo Lib "VBMP3.dll" (ByRef pListInfo As LIST_INFO) As Boolean
	'UPGRADE_WARNING: 構造体 TAG_INFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Declare Function vbmp3_getFileTagInfo Lib "VBMP3.dll" (ByVal pszName As String, ByRef pTagInfo As TAG_INFO) As Boolean
	'UPGRADE_WARNING: 構造体 MPEG_INFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	'UPGRADE_WARNING: 構造体 TAG_INFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Declare Function vbmp3_getFileInfo Lib "VBMP3.dll" (ByVal pszName As String, ByRef pTagInfo As TAG_INFO, ByRef pMpegTagInfo As MPEG_INFO) As Boolean
	'UPGRADE_WARNING: 構造体 LIST_INFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	'UPGRADE_WARNING: 構造体 MPEG_INFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	'UPGRADE_WARNING: 構造体 TAG_INFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Declare Function vbmp3_getFileInfo2 Lib "VBMP3.dll" (ByVal pszName As String, ByRef pTagInfo As TAG_INFO, ByRef pMpegTagInfo As MPEG_INFO, ByRef pListInfo As LIST_INFO) As Boolean
	Declare Function vbmp3_debug Lib "VBMP3.dll" () As Integer
	'UPGRADE_WARNING: 構造体 TAG_INFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Declare Function vbmp3_getGenre Lib "VBMP3.dll" (ByRef pTagInfo As TAG_INFO) As Boolean
	Declare Function vbmp3_getWinampPlayMs Lib "VBMP3.dll" () As Integer
	Declare Function vbmp3_getWinampTotalSec Lib "VBMP3.dll" () As Integer
	Declare Function vbmp3_getPlayBitRate Lib "VBMP3.dll" () As Integer
	Declare Function vbmp3_getLastErrorNo Lib "VBMP3.dll" () As Integer
	'UPGRADE_WARNING: 構造体 VBMP3_OPTION に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Declare Function vbmp3_setVbmp3Option Lib "VBMP3.dll" (ByRef pVbmp3Option As VBMP3_OPTION) As Integer
	'UPGRADE_WARNING: 構造体 VBMP3_OPTION に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Declare Sub vbmp3_getVbmp3Option Lib "VBMP3.dll" (ByRef pVbmp3Option As VBMP3_OPTION)
	'UPGRADE_WARNING: 構造体 DEC_OPTION に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Declare Function vbmp3_setDecodeOption Lib "VBMP3.dll" (ByRef pDecOption As DEC_OPTION) As Integer
	'UPGRADE_WARNING: 構造体 DEC_OPTION に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Declare Sub vbmp3_getDecodeOption Lib "VBMP3.dll" (ByRef pDecOption As DEC_OPTION)
	Declare Sub vbmp3_setEqualizer Lib "VBMP3.dll" (ByRef pTable As Integer)
	Declare Function vbmp3_getVersion Lib "VBMP3.dll" () As Integer
	'UPGRADE_WARNING: 構造体 TAG_INFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Declare Function vbmp3_getTagInfo Lib "VBMP3.dll" (ByRef pTagInfo As TAG_INFO) As Boolean
	'UPGRADE_WARNING: 構造体 TAG_INFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Declare Function vbmp3_setTagInfo Lib "VBMP3.dll" (ByVal pszName As String, ByRef pTagInfo As TAG_INFO, Optional ByVal tagSet As Integer = 0, Optional ByVal tagAdd As Integer = 0) As Boolean
	'UPGRADE_WARNING: 構造体 MPEG_INFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Declare Function vbmp3_getMpegInfo Lib "VBMP3.dll" (ByRef pMpegTagInfo As MPEG_INFO) As Boolean
	Declare Function vbmp3_init Lib "VBMP3.dll" () As Boolean
	Declare Function vbmp3_free Lib "VBMP3.dll" () As Boolean
	Declare Function vbmp3_setStepPitch Lib "VBMP3.dll" (ByVal pitch As Integer, Optional ByVal frames As Integer = 5) As Boolean
	Declare Function vbmp3_getStepPitch Lib "VBMP3.dll" () As Integer
	Declare Function vbmp3_reload Lib "VBMP3.dll" () As Boolean
	Declare Function vbmp3_setPitch Lib "VBMP3.dll" (ByVal pitch As Integer) As Boolean
	Declare Function vbmp3_getPitch Lib "VBMP3.dll" () As Integer
	'UPGRADE_WARNING: 構造体 InputInfo に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Declare Function vbmp3_open Lib "VBMP3.dll" (ByVal pszName As String, ByRef pInfo As InputInfo) As Boolean
	Declare Function vbmp3_close Lib "VBMP3.dll" () As Boolean
	Declare Function vbmp3_getState Lib "VBMP3.dll" (ByRef sec As Integer) As Integer
	Declare Function vbmp3_play Lib "VBMP3.dll" () As Boolean
	Declare Function vbmp3_stop Lib "VBMP3.dll" () As Boolean
	Declare Function vbmp3_pause Lib "VBMP3.dll" () As Boolean
	Declare Function vbmp3_restart Lib "VBMP3.dll" () As Boolean
	Declare Function vbmp3_seek Lib "VBMP3.dll" (ByVal sec As Integer) As Boolean
	Declare Function vbmp3_setVolume Lib "VBMP3.dll" (ByVal lVol As Integer, ByVal rVol As Integer) As Boolean
	Declare Function vbmp3_getVolume Lib "VBMP3.dll" (ByRef lVol As Integer, ByRef rVol As Integer) As Boolean
	Declare Sub vbmp3_setFadeIn Lib "VBMP3.dll" (ByVal fin As Integer)
	Declare Sub vbmp3_setFadeOut Lib "VBMP3.dll" (ByVal fout As Integer)
	Declare Sub vbmp3_fadeOut Lib "VBMP3.dll" ()
	
	'未使用
	Declare Sub vbmp3_startAnalyzeThread Lib "VBMP3.dll" ()
	Declare Sub vbmp3_stopAnalyzeThread Lib "VBMP3.dll" ()
	Declare Sub vbmp3_startAnalyze Lib "VBMP3.dll" ()
	Declare Sub vbmp3_stopAnalyze Lib "VBMP3.dll" ()
	'UPGRADE_WARNING: 構造体 WAVE_DATA に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Declare Function vbmp3_getWaveData Lib "VBMP3.dll" (ByRef pWaveData As WAVE_DATA) As Boolean
	
	Public Structure InputInfo
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public szTrackName() As Char '曲名
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public szArtistName() As Char 'ｱｰﾃｨｽﾄ名
		Dim channels As Integer 'ﾁｬﾝﾈﾙ数
		Dim bitRate As Integer 'ﾋﾞｯﾄﾚｰﾄ(kbit/s)
		Dim samplingRate As Integer 'ｻﾝﾌﾟﾙﾚｰﾄ(Hz)
		Dim totalSec As Integer '演奏時間(s)
	End Structure
	
	Public Structure TAG_INFO
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public szTrackName() As Char '曲名
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public szArtistName() As Char 'ｱｰﾃｨｽﾄ名
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public szAlbumName() As Char 'ｱﾙﾊﾞﾑ名
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(5),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=5)> Public szYear() As Char 'ﾘﾘｰｽ年号
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public szComment() As Char 'ｺﾒﾝﾄ
		Dim genre As Integer 'ｼﾞｬﾝﾙ
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public szGenreName() As Char 'ｼﾞｬﾝﾙ名
	End Structure
	
	Public Structure MPEG_INFO
		Dim version As Integer 'ﾊﾞｰｼﾞｮﾝ        1:MPEG-1, 2:MPEG-2, 3:MPEG-2.5
		Dim layer As Integer 'ﾚｲﾔ            1:Layer1. 2:Layer2, 3:Layer3
		Dim crcDisable As Integer 'ｴﾗｰ保護        0:無効, 1:有効
		Dim extension As Integer 'ｴｸｽﾃﾝｼｮﾝ       0:なし, 1:ﾌﾟﾗｲﾍﾞｰﾄ
		Dim Mode As Integer 'ﾁｬﾝﾈﾙﾓｰﾄﾞ      0:Stereo, 1:Joint stereo, 3:Dual channel, 4:Mono
		Dim copyright As Integer '著作権         0:著作権保護あり, 1:著作権保護なし
		Dim original As Integer 'ｵﾘｼﾞﾅﾙ         0:ｺﾋﾟｰ, 1:ｵﾘｼﾞﾅﾙ
		Dim emphasis As Integer 'ｴﾝﾌｫｼｽ         0:None, 1:50/15ms, 2:Reserved, 3:CCITT j.17
		Dim channels As Integer 'ﾁｬﾝﾈﾙ数
		Dim bitRate As Integer 'ﾋﾞｯﾄﾚｰﾄ(kbit/s)(0 なら VBR形式)
		Dim samplingRate As Integer 'ｻﾝﾌﾟﾙﾚｰﾄ(Hz)
		Dim fileSize As Integer 'ﾌｧｲﾙｻｲｽﾞ(Byte)
		Dim flames As Integer 'ﾌﾚｰﾑ数
		Dim totalSec As Integer '演奏時間(s)
	End Structure
	
	Public Structure DEC_OPTION
		Dim reduction As Integer 'サンプリング 0:1/1 1:1/2 2:1/4 [Default = 0]
		Dim convert As Integer 'チャンネル 0:ステレオ 1:モノラル[Default = 0]
		Dim freqLimit As Integer '周波数[Default = 24000]
	End Structure
	
	Public Structure VBMP3_OPTION
		Dim inputBlock As Integer '入力フレーム数[Default = 40]
		Dim outputBlock As Integer '出力フレーム数[Default = 30]
		Dim inputSleep As Integer '入力直後のｽﾘｰﾌﾟ時間(ﾐﾘ秒)[Default = 5]
		Dim outputSleep As Integer '出力直後のｽﾘｰﾌﾟ時間(ﾐﾘ秒)[Default = 0]
	End Structure
	
	Public Structure LIST_INFO
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public INAM() As Char '曲名
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public IART() As Char 'アーティスト名
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public IPRD() As Char '製品名
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public ICMT() As Char 'コメント文字列
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public ICRD() As Char 'リリース年号
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public IGNR() As Char 'ジャンル名
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public ICOP() As Char '著作権
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public IENG() As Char 'エンジニア
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public ISRC() As Char 'ソース
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public ISFT() As Char 'ソフトウェア
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public IKEY() As Char 'キーワード
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public ITCH() As Char '技術者
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public ILYC() As Char '歌詞
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public ICMS() As Char 'コミッション
	End Structure
	
	Public Structure LYRICS_INFO
		Dim sec As Integer
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public LyricsNext2() As Char
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public LyricsNext1() As Char
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public LyricsCurrent() As Char
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public LyricsPrev1() As Char
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public LyricsPrev2() As Char
	End Structure
	
	Public Structure WAVE_DATA
		Dim channels As Integer
		Dim bitsPerSample As Integer
		'UPGRADE_NOTE: Left は Left_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
		Dim Left_Renamed As Integer
		'UPGRADE_NOTE: Right は Right_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
		Dim Right_Renamed As Integer
	End Structure
	
	Public Structure WAVE_FORM
		Dim channels As Integer
		Dim bitsPerSample As Integer
		Dim samplingRate As Integer
		Dim dataSize As Integer
	End Structure
	
	'ファイルタイプ定数
	Public Const FT_NOMAL As Short = 0
	Public Const FT_WAVE As Short = 1
	Public Const FT_RMP As Short = 2
	Public Const FT_ID3V2 As Short = 4
	Public Const FT_MAC As Short = 8
	Public Const FT_ID3V1 As Short = 16
	
	'コールバック関数用
	Public Const MSG_ERROR As Short = 0
	Public Const MSG_STOPING As Short = 1
	Public Const MSG_PLAYING As Short = 2
	Public Const MSG_PAUSING As Short = 3
	Public Const MSG_PLAYDONE As Short = 4
	
	Enum vbmp3_errNo
		ERR_MP3_FILE_OPEN = 1
		ERR_MP3_FILE_NOT_OPEN = 2
		ERR_MP3_FILE_READ = 3
		ERR_MP3_FILE_WRITE = 4
		ERR_WAV_FILE_OPEN = 5
		ERR_WAV_FORMAT = 6
		ERR_ENCODE_FILE_OPEN = 7
		ERR_LYRICS_FILE_OPEN = 8
		ERR_LYRICS_NON_DATA = 9
		ERR_FRAME_HEADER_NOT_FOUND = 10
		ERR_FRAME_HEADER_READ = 11
		ERR_STATE_STOP = 12
		ERR_NOT_STATE_STOP = 13
		ERR_NOT_STATE_PLAY = 14
		ERR_STATE_NON_ENCODE = 15
		ERR_PLAY = 16
		ERR_STOP = 17
		ERR_INVALID_VALUE = 18
		ERR_MALLOC = 19
		ERR_NON_RIFF = 20
		ERR_RIFF = 21
		ERR_NOT_MP3 = 22
		ERR_MAC_BIN = 23
		ERR_UNKNOWN_FILE = 24
		ERR_OPEN_OUT_DEVICE = 25
		ERR_DECODE = 26
		ERR_DECODE_THREAD = 27
		ERR_ENCODE_THREAD = 28
		ERR_CREATE_EVENT = 29
		ERR_CODEC_NOT_FOUND = 30
		ERR_WAVE_TABLE_NOT_FOUND = 31
		ERR_ACM_OPEN = 32
	End Enum
	
	
	'---------------------------------------------------------
	'関数：Function NTrim()
	'機能：\0 以降の文字列削除
	'引数：Word  : 変換元文字列
	'戻り値：変換後文字列
	'---------------------------------------------------------
	Function NTrim(ByRef Word As String) As String
		If InStr(Word, Chr(0)) > 0 Then
			NTrim = Left(Word, InStr(Word, Chr(0)) - 1)
		Else
			NTrim = Word
		End If
	End Function
End Module