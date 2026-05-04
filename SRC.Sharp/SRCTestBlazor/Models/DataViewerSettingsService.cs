using Microsoft.JSInterop;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SRCTestBlazor.Models
{
    public class DataViewerSettingsService
    {
        private const string LocalStorageKey = "dataViewerSettings";
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
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", LocalStorageKey);
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    _settings = JsonSerializer.Deserialize<DataViewerSettings>(json) ?? new DataViewerSettings();
                }
                catch
                {
                    _settings = new DataViewerSettings();
                }
            }
            await ApplyThemeAsync();
            NotifyStateChanged();
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
