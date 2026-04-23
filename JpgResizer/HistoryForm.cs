using System.Text.Json;
using System.Text.Json.Serialization;

namespace JpgResizer
{
    public partial class HistoryForm : Form
    {
        private readonly string _historyUrl;
        private readonly string _getUploadUrl;

        public HistoryForm(string historyUrl, string getUploadUrl)
        {
            _historyUrl = historyUrl;
            _getUploadUrl = getUploadUrl;
            InitializeComponent();
            this.Load += async (s, e) => await LoadHistoryAsync();
            dgvHistory.CellClick += DgvHistory_CellClick;
            btnRefresh.Click += async (s, e) => await LoadHistoryAsync();
        }

        private async Task LoadHistoryAsync()
        {
            try
            {
                using HttpClient client = new();
                HttpResponseMessage response = await client.GetAsync(_historyUrl);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var history = JsonSerializer.Deserialize<List<HistoryRecord>>(json);
                    dgvHistory.DataSource = null;
                    dgvHistory.DataSource = history;

                    // Настройка колонок
                    if (dgvHistory.Columns["Id"] != null)
                        dgvHistory.Columns["Id"].Visible = false;
                    if (dgvHistory.Columns["StoredName"] != null)
                        dgvHistory.Columns["StoredName"].Visible = false;
                    if (dgvHistory.Columns["ClientIP"] != null)
                        dgvHistory.Columns["ClientIP"].Visible = false;

                    if (dgvHistory.Columns["OriginalName"] != null)
                        dgvHistory.Columns["OriginalName"].HeaderText = "Исходное имя";
                    if (dgvHistory.Columns["UploadTime"] != null)
                        dgvHistory.Columns["UploadTime"].HeaderText = "Время загрузки";
                    if (dgvHistory.Columns["OriginalWidth"] != null)
                        dgvHistory.Columns["OriginalWidth"].HeaderText = "Ширина (исх.)";
                    if (dgvHistory.Columns["OriginalHeight"] != null)
                        dgvHistory.Columns["OriginalHeight"].HeaderText = "Высота (исх.)";
                    if (dgvHistory.Columns["TargetSize"] != null)
                        dgvHistory.Columns["TargetSize"].HeaderText = "Целевая ширина";
                    if (dgvHistory.Columns["FinalWidth"] != null)
                        dgvHistory.Columns["FinalWidth"].HeaderText = "Финальная ширина";
                    if (dgvHistory.Columns["FinalHeight"] != null)
                        dgvHistory.Columns["FinalHeight"].HeaderText = "Финальная высота";
                    if (dgvHistory.Columns["Status"] != null)
                        dgvHistory.Columns["Status"].HeaderText = "Статус";
                    if (dgvHistory.Columns["ErrorMessage"] != null)
                    {
                        dgvHistory.Columns["ErrorMessage"].HeaderText = "Ошибка";
                        dgvHistory.Columns["ErrorMessage"].Width = 200;
                    }

                    ApplyStatusColors();
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить историю.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке истории: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyStatusColors()
        {
            foreach (DataGridViewRow row in dgvHistory.Rows)
            {
                var statusCell = row.Cells["Status"];
                if (statusCell?.Value != null)
                {
                    string status = statusCell.Value.ToString() ?? "";
                    if (status.Equals("error", StringComparison.OrdinalIgnoreCase))
                    {
                        statusCell.Style.BackColor = Color.Red;
                        statusCell.Style.ForeColor = Color.White;
                        // цвет не меняется на базовый, при выделении строки
                        statusCell.Style.SelectionBackColor = Color.DarkRed;
                        statusCell.Style.SelectionForeColor = Color.White;
                    }
                    else if (status.Equals("success", StringComparison.OrdinalIgnoreCase))
                    {
                        statusCell.Style.BackColor = Color.LightGreen;
                        statusCell.Style.ForeColor = Color.Black;
                        // цвет не меняется на базовый, при выделении строки
                        statusCell.Style.SelectionBackColor = Color.Green;
                        statusCell.Style.SelectionForeColor = Color.White;
                    }
                }
            }
        }

        private async void DgvHistory_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvHistory.Rows[e.RowIndex];
            string? storedName = row.Cells["StoredName"]?.Value?.ToString();
            string? status = row.Cells["Status"]?.Value?.ToString();

            if (string.IsNullOrEmpty(storedName) || status != "success")
            {
                picPreview.Image?.Dispose();
                picPreview.Image = null;
                picPreview.BackColor = status != "success" ? Color.LightCoral : Color.LightGray;
                return;
            }

            try
            {
                using HttpClient client = new();
                string url = $"{_getUploadUrl}?file={Uri.EscapeDataString(storedName)}";
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    byte[] imageData = await response.Content.ReadAsByteArrayAsync();
                    using var ms = new MemoryStream(imageData);
                    var img = Image.FromStream(ms);
                    picPreview.Image?.Dispose();
                    picPreview.Image = new Bitmap(img);
                    picPreview.BackColor = Color.Transparent;
                }
                else
                {
                    picPreview.Image?.Dispose();
                    picPreview.Image = null;
                    picPreview.BackColor = Color.LightCoral;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class HistoryRecord
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("originalName")]
        public string OriginalName { get; set; } = "";

        [JsonPropertyName("uploadTime")]
        public string UploadTime { get; set; } = "";

        [JsonPropertyName("originalWidth")]
        public int OriginalWidth { get; set; }

        [JsonPropertyName("originalHeight")]
        public int OriginalHeight { get; set; }

        [JsonPropertyName("targetSize")]
        public int TargetSize { get; set; }

        [JsonPropertyName("finalWidth")]
        public int FinalWidth { get; set; }

        [JsonPropertyName("finalHeight")]
        public int FinalHeight { get; set; }

        [JsonPropertyName("errorMessage")]
        public string? ErrorMessage { get; set; }

        [JsonPropertyName("storedName")]
        public string? StoredName { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = "";

        [JsonPropertyName("clientIP")]
        public string ClientIP { get; set; } = "";
    }
}