using Microsoft.JSInterop;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SRCTestBlazor.Models
{
    public class DataViewerSettingsService
    {
        public const string LocalStorageKey = "dataViewerSettings";
        private readonly IJSRuntime _jsRuntime;
        private DataViewerSettings _settings = new();

        public DataViewerSettings Settings => _settings;

        public event Action OnChange;

        public DataViewerSettingsService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task InitializeAsync()
        {
            _settings = await LoadAsync(_jsRuntime);
            await ApplyThemeAsync();
            NotifyStateChanged();
        }

        /// <summary>localStorage から設定を読む。無いか壊れていれば既定の設定</summary>
        public static async Task<DataViewerSettings> LoadAsync(IJSRuntime jsRuntime)
        {
            var json = await jsRuntime.InvokeAsync<string>("localStorage.getItem", LocalStorageKey);
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    return JsonSerializer.Deserialize<DataViewerSettings>(json) ?? new DataViewerSettings();
                }
                catch
                {
                    return new DataViewerSettings();
                }
            }
            return new DataViewerSettings();
        }

        public async Task SaveAsync()
        {
            var json = JsonSerializer.Serialize(_settings);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", LocalStorageKey, json);
            await ApplyThemeAsync();
            NotifyStateChanged();
        }

        private async Task ApplyThemeAsync()
        {
            await _jsRuntime.InvokeVoidAsync("dataViewerSettings.applyTheme", _settings.ThemeMode.ToString());
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
