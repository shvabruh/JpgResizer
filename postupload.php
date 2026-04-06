<?php
header('Content-Type: application/json');

require_once 'db.php';

$uploadDir = 'uploads/';
$maxFileSize = 10 * 1024 * 1024; // 10 МБ
$allowedType = IMAGETYPE_JPEG;

// Создание папки для загрузок, если её нет
if (!is_dir($uploadDir)) {
    if (!mkdir($uploadDir, 0777, true)) {
        http_response_code(500);
        echo json_encode(['success' => false, 'error' => 'Не удалось создать папку для загрузок']);
        exit;
    }
}

// Проверка наличия файла
if (!isset($_FILES['image'])) {
    http_response_code(400);
    echo json_encode(['success' => false, 'error' => 'Файл не передан']);
    exit;
}

$file = $_FILES['image'];
if ($file['error'] !== UPLOAD_ERR_OK) {
    $uploadErrors = [
        UPLOAD_ERR_INI_SIZE   => 'Файл превышает размер, заданный в php.ini',
        UPLOAD_ERR_FORM_SIZE  => 'Файл превышает размер, заданный в HTML-форме',
        UPLOAD_ERR_PARTIAL    => 'Файл загружен частично',
        UPLOAD_ERR_NO_FILE    => 'Файл не загружен',
        UPLOAD_ERR_NO_TMP_DIR => 'Отсутствует временная папка',
        UPLOAD_ERR_CANT_WRITE => 'Не удалось записать файл на диск',
        UPLOAD_ERR_EXTENSION  => 'Загрузка остановлена расширением PHP',
    ];
    $errorMsg = $uploadErrors[$file['error']] ?? 'Неизвестная ошибка загрузки';
    http_response_code(400);
    echo json_encode(['success' => false, 'error' => $errorMsg]);
    exit;
}

// Проверка размера файла
if ($file['size'] > $maxFileSize) {
    http_response_code(400);
    echo json_encode(['success' => false, 'error' => 'Файл превышает допустимый размер (10 МБ)']);
    exit;
}

// Проверка типа файла (только JPEG)
$imageType = exif_imagetype($file['tmp_name']);
if ($imageType !== $allowedType) {
    http_response_code(400);
    echo json_encode(['success' => false, 'error' => 'Допустимы только изображения в формате JPG/JPEG']);
    exit;
}

// Получение исходных размеров
list($origW, $origH) = getimagesize($file['tmp_name']);

// Проверка целевой ширины
$userW = isset($_POST['width']) ? (int)$_POST['width'] : 0;
if ($userW <= 0) {
    http_response_code(400);
    echo json_encode(['success' => false, 'error' => 'Целевая ширина должна быть положительным числом']);
    exit;
}
if ($origW < $userW) {
    http_response_code(400);
    echo json_encode(['success' => false, 'error' => 'Исходное изображение уже меньше заданной ширины']);
    exit;
}

// Вычисление новых размеров с сохранением пропорций
$ratio = $origH / $origW;
$finalW = $userW;
$finalH = round($finalW * $ratio);

// Ресайз изображения
$src = imagecreatefromjpeg($file['tmp_name']);
if (!$src) {
    http_response_code(500);
    echo json_encode(['success' => false, 'error' => 'Не удалось загрузить изображение для обработки']);
    exit;
}

$dst = imagecreatetruecolor($finalW, $finalH);
if (!$dst) {
    imagedestroy($src);
    http_response_code(500);
    echo json_encode(['success' => false, 'error' => 'Не удалось создать целевое изображение']);
    exit;
}

if (!imagecopyresampled($dst, $src, 0, 0, 0, 0, $finalW, $finalH, $origW, $origH)) {
    imagedestroy($src);
    imagedestroy($dst);
    http_response_code(500);
    echo json_encode(['success' => false, 'error' => 'Ошибка при изменении размера изображения']);
    exit;
}

// Генерация уникального имени
$originalName = basename($file['name']);
$pathInfo = pathinfo($originalName);
$storedName = $pathInfo['filename'] . '_' . time() . '.jpg';

// Сохранение на диск
$targetPath = $uploadDir . $storedName;
if (!imagejpeg($dst, $targetPath, 90)) {
    imagedestroy($src);
    imagedestroy($dst);
    http_response_code(500);
    echo json_encode(['success' => false, 'error' => 'Не удалось сохранить обработанное изображение']);
    exit;
}

// Освобождение ресурсов GD
imagedestroy($src);
imagedestroy($dst);

// Запись в БД
$clientIP = $_SERVER['REMOTE_ADDR'] ?? 'unknown';
$status = 'success';
$errorMessage = null;

$conn = getDbConnection();
if (!$conn) {
    // БД недоступна – возвращаем успех с предупреждением
    echo json_encode([
        'success' => true,
        'fileName' => $storedName,
        'warning' => 'Файл сохранён, но база данных недоступна'
    ]);
    exit;
}

$stmt = $conn->prepare("
    INSERT INTO history 
    (originalName, storedName, uploadTime, originalWidth, originalHeight, targetSize, finalWidth, finalHeight, status, errorMessage, clientIP)
    VALUES (?, ?, NOW(), ?, ?, ?, ?, ?, ?, ?, ?)
");
if (!$stmt) {
    $conn->close();
    echo json_encode([
        'success' => true,
        'fileName' => $storedName,
        'warning' => 'Файл сохранён, но не удалось записать историю (ошибка подготовки запроса)'
    ]);
    exit;
}

$stmt->bind_param(
    "ssiiiiisss",
    $originalName,
    $storedName,
    $origW,
    $origH,
    $userW,
    $finalW,
    $finalH,
    $status,
    $errorMessage,
    $clientIP
);

if ($stmt->execute()) {
    echo json_encode(['success' => true, 'fileName' => $storedName]);
} else {
    echo json_encode([
        'success' => true,
        'fileName' => $storedName,
        'warning' => 'Файл сохранён, но не удалось записать историю (ошибка выполнения запроса)'
    ]);
}

$stmt->close();
$conn->close();