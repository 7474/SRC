using System.Linq;

namespace SRCCore.Events
{
    public enum EventDataSource
    {
        Unknown,
        System,
        Scenario,
    }
    public class EventDataLine
    {
        public static readonly EventDataLine Empty = new EventDataLine(-1, EventDataSource.Unknown, "-", -1, "-");

        public int ID { get; }
        public EventDataSource Source { get; }
        // イベントファイルのファイル名
        public string File { get; }
        // イベントファイルの何行目に位置するか
        public int LineNum { get; }
        // イベントデータ
        public string Data { get; }

        public bool IsSystemData => Source == EventDataSource.System;

        // 常時イベントラベルらしき行かどうか
        public bool IsAlwaysEventLabel => Data.FirstOrDefault() == '*';

        public int NextID => ID + 1;

        public EventDataLine(int id, EventDataSource source, string file, int lineNum, string data)
        {
            ID = id;
            Source = source;
            File = file;
            LineNum = lineNum;
            Data = data;
        }

        public override string ToString()
        {
            return $"{ID}: {Data}";
        }
    }
}
