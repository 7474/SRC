namespace SRCCore.Events
{
    public partial class Event
    {
        // 描画の基準座標位置を保存
        public void SaveBasePoint()
        {
            BasePointIndex = BasePointIndex + 1;
            if (BasePointIndex >= SavedBaseX.Length)
            {
                BasePointIndex = 0;
            }

            SavedBaseX[BasePointIndex] = BaseX;
            SavedBaseY[BasePointIndex] = BaseY;
        }

        // 描画の基準座標位置を復元
        public void RestoreBasePoint()
        {
            if (BasePointIndex < 0)
            {
                BasePointIndex = SavedBaseX.Length;
            }

            BaseX = SavedBaseX[BasePointIndex];
            BaseY = SavedBaseY[BasePointIndex];
            BasePointIndex = BasePointIndex - 1;
        }

        // 描画の基準座標位置をリセット
        public void ResetBasePoint()
        {
            BaseX = 0;
            BaseY = 0;
            BasePointIndex = 0;
            for (var i = 0; i < SavedBaseX.Length; i++)
            {
                SavedBaseX[i] = 0;
                SavedBaseY[i] = 0;
            }
        }
    }
}
