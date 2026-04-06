using System.Text;
using System.Text.Json;

namespace JpgResizer
{
    public partial class Form1 : Form
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _uploadUrl = "http://localhost/postupload.php";
        private Bitmap _originalImage;

        public Form1()
        {
            InitializeComponent();
            btnSelectFile.Click += BtnSelectFile_Click;
            btnUpload.Click += BtnUpload_Click;
            this.FormClosed += Form1_FormClosed;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _originalImage?.Dispose();
            _httpClient?.Dispose();
        }

        private void BtnSelectFile_Click(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JPEG files (*.jpg;*.jpeg)|*.jpg;*.jpeg";
            ofd.Title = "Выберите JPG изображение";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = ofd.FileName;
                try
                {
                    using (var fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read))
                    {
                        var img = Image.FromStream(fs);
                        _originalImage = new Bitmap(img);
                        picPreview.Image = _originalImage;
                        lblOriginalSize.Text = $"Размеры: {_originalImage.Width} x {_originalImage.Height}";
                    }
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

        // Загрузка файла на сервер
        private async void BtnUpload_Click(object sender, EventArgs e)
        {
            // Проверки
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

            // Блокировка UI
            btnSelectFile.Enabled = false;
            btnUpload.Enabled = false;
            statusLabel.Text = "Загрузка...";

            try
            {
                using var content = new MultipartFormDataContent();

                // Добавление файла
                byte[] fileBytes = File.ReadAllBytes(txtFilePath.Text);
                var fileContent = new ByteArrayContent(fileBytes);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                content.Add(fileContent, "image", Path.GetFileName(txtFilePath.Text));

                // Добавление параметра ширины
                content.Add(new StringContent(nudTargetWidth.Value.ToString(), Encoding.UTF8), "width");

                // Отправка POST-запроса
                HttpResponseMessage response = await _httpClient.PostAsync(_uploadUrl, content);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using JsonDocument doc = JsonDocument.Parse(responseBody);
                    JsonElement root = doc.RootElement;
                    bool success = root.GetProperty("success").GetBoolean();

                    if (success)
                    {
                        string fileName = root.GetProperty("fileName").GetString();
                        statusLabel.Text = $"Файл сохранён как {fileName}";
                        MessageBox.Show($"Изображение успешно загружено\nСохранённое имя: {fileName}", "Готово",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // доделать (шаг 3)
                    }
                    else
                    {
                        string error = root.GetProperty("error").GetString();
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
                else
                {
                    statusLabel.Text = $"HTTP ошибка: {response.StatusCode}";
                    MessageBox.Show($"Сервер ответил кодом {response.StatusCode}\n{responseBody}", "Ошибка HTTP",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (HttpRequestException ex)
            {
                statusLabel.Text = "Ошибка сети";
                MessageBox.Show($"Не удалось соединиться с сервером:\n{ex.Message}\n\nПроверьте, что сервер запущен (XAMPP) и postupload.php доступен по адресу {_uploadUrl}",
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
    }
}