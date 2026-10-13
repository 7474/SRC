namespace SRCSound
{
    /// <summary>
    /// 再生する音声（float、インタリーブ）。
    /// </summary>
    public interface ISampleSource
    {
        int SampleRate { get; }

        /// <summary>1 または 2</summary>
        int Channels { get; }

        /// <summary>
        /// buffer に最大 count 個のサンプルを書き込み、書き込んだ数を返す。終端では 0 を返す。
        /// </summary>
        int Read(float[] buffer, int offset, int count);

        /// <summary>先頭に戻す（ループ再生）。</summary>
        void Rewind();
    }
}
