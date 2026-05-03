using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SRCTestBlazor.Models
{
    // State管理試してみる
    // https://docs.microsoft.com/ja-jp/aspnet/core/blazor/state-management?view=aspnetcore-5.0&pivots=webassembly#in-memory-state-container-service-wasm
    public class SrcDataContainer
    {
        private bool indexBusy = false;

        public SrcBitmapIndex BitmapIndex { get; private set; } = new SrcBitmapIndex();
        public SrcDataIndex DataIndex { get; private set; } = new SrcDataIndex();

        public bool BitmapIndexLoaded => BitmapIndex.Folders.Any();
        public bool DataIndexLoaded => DataIndex.Titles.Any();
        public bool IndexBusy
        {
            get => indexBusy;
            set
            {
                var changed = indexBusy != value;
                indexBusy = value;
                if (changed)
                {
                    NotifyStateChanged();
                }
            }
        }

        public event Action OnChange;

        public async Task LoadBitmapIndex(HttpClient http, string uri, string baseUri)
        {
            var newIndex = await http.GetFromJsonAsync(uri, SrcJsonSerializerContext.Default.SrcBitmapIndex)
                ?? throw new InvalidOperationException($"Failed to load bitmap index from {uri}");
            newIndex.BuildIndex(baseUri);
            BitmapIndex = newIndex;

            NotifyStateChanged();
        }

        public async Task LoadDataIndex(HttpClient http, string uri)
        {
            var newIndex = await http.GetFromJsonAsync(uri, SrcJsonSerializerContext.Default.SrcDataIndex)
                ?? throw new InvalidOperationException($"Failed to load data index from {uri}");
            DataIndex = newIndex;

            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
