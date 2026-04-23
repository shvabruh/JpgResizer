using System.Text;
using System.Text.Json;

namespace JpgResizer
{
    public partial class Form1 : Form
    {
        private readonly HttpClient _httpClient = new();
        private readonly string _uploadUrl = "http://localhost/dashboard/Kursovaya_scripts/postupload.php";
        private readonly string _historyUrl = "http://localhost/dashboard/Kursovaya_scripts/gethistory.php";
        private readonly string _getUploadUrl = "http://localhost/dashboard/Kursovaya_scripts/getuploads.php";
        private readonly string _webHistoryUrl = "http://localhost/dashboard/Kursovaya_scripts/history_view.php";
        private Bitmap? _originalImage;

        public Form1()
        {
            InitializeComponent();
            btnSelectFile.Click += BtnSelectFile_Click;
            btnUpload.Click += BtnUpload_Click;
            btnHistory.Click += BtnHistory_Click;
            btnWebHistory.Click += BtnWebHistory_Click;
            FormClosed += Form1_FormClosed;
        }

        private void Form1_FormClosed(object? sender, FormClosedEventArgs e)
        {
            _originalImage?.Dispose();
            _httpClient.Dispose();
        }

        private void BtnSelectFile_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog ofd = new()
            {
                Filter = "JPEG files (*.jpg;*.jpeg)|*.jpg;*.jpeg",
                Title = "Выберите JPG изображение"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string ext = Path.GetExtension(ofd.FileName).ToLower();
                if (ext != ".jpg" && ext != ".jpeg")
                {
                    MessageBox.Show("Выберите файл формата JPG или JPEG.", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                txtFilePath.Text = ofd.FileName;
                try
                {
                    using var fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read);
                    var img = Image.FromStream(fs);
                    _originalImage?.Dispose();
                    _originalImage = new Bitmap(img);
                    picPreview.Image = _originalImage;
                    lblOriginalSize.Text = $"Размеры: {_originalImage.Width} x {_originalImage.Height}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось загрузить изображение: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtFilePath.Clear();
                    picPreview.Image = null;
                    lblOriginalSize.Text = "Размеры: ";
                }
            }
        }

        private async void BtnUpload_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFilePath.Text))
            {
                statusLabel.Text = "Ошибка: файл не выбран";
                MessageBox.Show("Сначала выберите файл.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!File.Exists(txtFilePath.Text))
            {
                statusLabel.Text = "Ошибка: файл не существует";
                MessageBox.Show("Файл не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (nudTargetWidth.Value <= 0)
            {
                statusLabel.Text = "Ошибка: ширина должна быть больше 0";
                MessageBox.Show("Целевая ширина должна быть положительным числом.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnSelectFile.Enabled = false;
            btnUpload.Enabled = false;
            statusLabel.Text = "Загрузка...";

            try
            {
                using var content = new MultipartFormDataContent();
                byte[] fileBytes = File.ReadAllBytes(txtFilePath.Text);
                var fileContent = new ByteArrayContent(fileBytes);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                content.Add(fileContent, "image", Path.GetFileName(txtFilePath.Text));
                content.Add(new StringContent(nudTargetWidth.Value.ToString(), Encoding.UTF8), "width");

                HttpResponseMessage response = await _httpClient.PostAsync(_uploadUrl, content);
                string responseBody = await response.Content.ReadAsStringAsync();

                statusLabel.Text = $"Код: {response.StatusCode}";

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        using JsonDocument doc = JsonDocument.Parse(responseBody);
                        JsonElement root = doc.RootElement;
                        bool success = root.GetProperty("success").GetBoolean();

                        if (success)
                        {
                            string fileName = root.GetProperty("fileName").GetString()!;
                            statusLabel.Text = $"Файл сохранён как {fileName}";
                            MessageBox.Show($"Изображение успешно загружено\nСохранённое имя: {fileName}", "Готово",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Предложение открыть историю
                            DialogResult openHistory = MessageBox.Show(
                                "Открыть историю загрузок?",
                                "История",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);
                            if (openHistory == DialogResult.Yes)
                            {
                                BtnHistory_Click(sender, e);
                            }
                        }
                        else
                        {
                            string error = root.GetProperty("error").GetString()!;
                            statusLabel.Text = $"Ошибка: {error}";
                            MessageBox.Show($"Сервер вернул ошибку:\n{error}", "Ошибка загрузки",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        if (root.TryGetProperty("warning", out JsonElement warning))
                        {
                            statusLabel.Text = $"Предупреждение: {warning.GetString()}";
                            MessageBox.Show(warning.GetString(), "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch (JsonException jsonEx)
                    {
                        string preview = responseBody.Length > 500 ? responseBody.Substring(0, 500) : responseBody;
                        MessageBox.Show($"Сервер вернул не JSON, а:\n{preview}\n\nОшибка: {jsonEx.Message}",
                            "Ошибка формата ответа", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        statusLabel.Text = "Ошибка: сервер вернул не JSON";
                    }
                }
                else
                {
                    string preview = responseBody.Length > 500 ? responseBody.Substring(0, 500) : responseBody;
                    MessageBox.Show($"HTTP {response.StatusCode}\n{preview}", "Ошибка сервера",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (HttpRequestException ex)
            {
                statusLabel.Text = "Ошибка сети";
                MessageBox.Show($"Не удалось соединиться с сервером:\n{ex.Message}\n\nПроверьте, что сервер запущен и URL верен:\n{_uploadUrl}",
                    "Ошибка сети", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Ошибка";
                MessageBox.Show($"Неизвестная ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSelectFile.Enabled = true;
                btnUpload.Enabled = true;
                if (statusLabel.Text == "Загрузка...") statusLabel.Text = "Готово";
            }
        }

        private void BtnHistory_Click(object? sender, EventArgs e)
        {
            HistoryForm historyForm = new(_historyUrl, _getUploadUrl);
            historyForm.ShowDialog(this);
        }

        private void BtnWebHistory_Click(object? sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = _webHistoryUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть браузер: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}