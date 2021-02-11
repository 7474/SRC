using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SRCTestBlazor.Models
{
    // State管理試してみる
    // https://docs.microsoft.com/ja-jp/aspnet/core/blazor/state-management?view=aspnetcore-5.0&pivots=webassembly#in-memory-state-container-service-wasm
    public class SrcDataContainer
    {
        public SrcBitmapIndex BitmapIndex { get; private set; } = new SrcBitmapIndex();
        public event Action OnChange;

        public async Task LoadBitmapIndex(HttpClient http, string uri, string baseUri)
        {
            var newIndex = await http.GetFromJsonAsync<SrcBitmapIndex>(uri);
            newIndex.BuildIndex(baseUri);
            BitmapIndex = newIndex;

            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
