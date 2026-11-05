using Microsoft.JSInterop;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SRCDataViewer.Models
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
            string json;
            try
            {
                json = await jsRuntime.InvokeAsync<string>("localStorage.getItem", LocalStorageKey);
            }
            catch (JSException ex)
            {
                // localStorage を使えない（ブラウザの設定・プライベートブラウズなど）。既定の設定で動かす
                Console.Error.WriteLine($"設定を読めません: {ex.Message}");
                return new DataViewerSettings();
            }
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
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", LocalStorageKey, json);
            }
            catch (JSException ex)
            {
                // 保存できなくても、変えた設定はこの表示の間は効かせる
                Console.Error.WriteLine($"設定を保存できません: {ex.Message}");
            }
            await ApplyThemeAsync();
            NotifyStateChanged();
        }

        private async Task ApplyThemeAsync()
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("dataViewerSettings.applyTheme", _settings.ThemeMode.ToString());
            }
            catch (JSException ex)
            {
                Console.Error.WriteLine($"テーマを変えられません: {ex.Message}");
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
