namespace SRCCore.Events
{
    // ホットポイント
    public struct HotPoint
    {
        public string Name;
        public int Left;
        public int Top;
        public int Width;
        public int Height;
        public string Caption;

        public override string ToString()
        {
            return $"{Name}({Left},{Top},{Width},{Height}): {Caption}";
        }
    }
}
