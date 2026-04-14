<?php
header('Content-Type: application/json');

require_once 'db.php';

$uploadDir = 'uploads/';
$maxFileSize = 10 * 1024 * 1024; // 10 МБ
$allowedType = IMAGETYPE_JPEG;

if (!is_dir($uploadDir)) 
{
    if (!mkdir($uploadDir, 0777, true)) 
    {
        http_response_code(500);
        echo json_encode(['success' => false, 'error' => 'Не удалось создать папку для загрузок']);
        exit;
    }
}

function logErrorToDB($originalName, $targetWidth, $errorMsg, $clientIP) 
{
    $conn = getDbConnection();
    if (!$conn) return false;
    $stmt = $conn->prepare("
        INSERT INTO history 
        (originalName, uploadTime, targetSize, status, errorMessage, clientIP)
        VALUES (?, NOW(), ?, 'error', ?, ?)
    ");
    if (!$stmt) 
    {
        $conn->close();
        return false;
    }
    
    $stmt->bind_param("siss", $originalName, $targetWidth, $errorMsg, $clientIP);
    $success = $stmt->execute();
    $stmt->close();
    $conn->close();
    return $success;
}

$clientIP = $_SERVER['REMOTE_ADDR'] ?? 'unknown';
$originalName = isset($_FILES['image']['name']) ? basename($_FILES['image']['name']) : 'unknown.jpg';
$targetWidth = isset($_POST['width']) ? (int)$_POST['width'] : 0;

if (!isset($_FILES['image']) || $_FILES['image']['error'] !== UPLOAD_ERR_OK) 
{
    $errorMsg = 'Файл не передан или ошибка загрузки';
    logErrorToDB($originalName, $targetWidth, $errorMsg, $clientIP);
    http_response_code(400);
    echo json_encode(['success' => false, 'error' => $errorMsg]);
    exit;
}

$file = $_FILES['image'];

if ($file['size'] > $maxFileSize) 
{
    $errorMsg = 'Файл превышает допустимый размер (10 МБ)';
    logErrorToDB($originalName, $targetWidth, $errorMsg, $clientIP);
    http_response_code(400);
    echo json_encode(['success' => false, 'error' => $errorMsg]);
    exit;
}

$imageType = exif_imagetype($file['tmp_name']);
if ($imageType !== $allowedType) 
{
    $errorMsg = 'Допустимы только изображения в формате JPG/JPEG';
    logErrorToDB($originalName, $targetWidth, $errorMsg, $clientIP);
    http_response_code(400);
    echo json_encode(['success' => false, 'error' => $errorMsg]);
    exit;
}

list($origW, $origH) = getimagesize($file['tmp_name']);
if ($targetWidth <= 0) 
{
    $errorMsg = 'Целевая ширина должна быть положительным числом';
    logErrorToDB($originalName, $targetWidth, $errorMsg, $clientIP);
    http_response_code(400);
    echo json_encode(['success' => false, 'error' => $errorMsg]);
    exit;
}
if ($origW < $targetWidth) 
{
    $errorMsg = 'Исходное изображение уже меньше заданной ширины';
    logErrorToDB($originalName, $targetWidth, $errorMsg, $clientIP);
    http_response_code(400);
    echo json_encode(['success' => false, 'error' => $errorMsg]);
    exit;
}

$ratio = $origH / $origW;
$finalW = $targetWidth;
$finalH = round($finalW * $ratio);

$src = imagecreatefromjpeg($file['tmp_name']);
if (!$src) 
{
    $errorMsg = 'Не удалось загрузить изображение для обработки';
    logErrorToDB($originalName, $targetWidth, $errorMsg, $clientIP);
    http_response_code(500);
    echo json_encode(['success' => false, 'error' => $errorMsg]);
    exit;
}

$dst = imagecreatetruecolor($finalW, $finalH);
if (!$dst) 
{
    imagedestroy($src);
    $errorMsg = 'Не удалось создать целевое изображение';
    logErrorToDB($originalName, $targetWidth, $errorMsg, $clientIP);
    http_response_code(500);
    echo json_encode(['success' => false, 'error' => $errorMsg]);
    exit;
}

if (!imagecopyresampled($dst, $src, 0, 0, 0, 0, $finalW, $finalH, $origW, $origH)) 
{
    imagedestroy($src);
    imagedestroy($dst);
    $errorMsg = 'Ошибка при изменении размера изображения';
    logErrorToDB($originalName, $targetWidth, $errorMsg, $clientIP);
    http_response_code(500);
    echo json_encode(['success' => false, 'error' => $errorMsg]);
    exit;
}

$pathInfo = pathinfo($originalName);
$storedName = $pathInfo['filename'] . '_' . time() . '.jpg';
$targetPath = $uploadDir . $storedName;

if (!imagejpeg($dst, $targetPath, 90)) 
{
    imagedestroy($src);
    imagedestroy($dst);
    $errorMsg = 'Не удалось сохранить обработанное изображение';
    logErrorToDB($originalName, $targetWidth, $errorMsg, $clientIP);
    http_response_code(500);
    echo json_encode(['success' => false, 'error' => $errorMsg]);
    exit;
}

imagedestroy($src);
imagedestroy($dst);

$status = 'success';
$errorMessage = null;
$conn = getDbConnection();
if ($conn) 
{
    $stmt = $conn->prepare("
        INSERT INTO history 
        (originalName, storedName, uploadTime, originalWidth, originalHeight, targetSize, finalWidth, finalHeight, status, errorMessage, clientIP)
        VALUES (?, ?, NOW(), ?, ?, ?, ?, ?, ?, ?, ?)
    ");
    if ($stmt) 
    {
        $stmt->bind_param("ssiiiiisss", $originalName, $storedName, $origW, $origH, $targetWidth, $finalW, $finalH, $status, $errorMessage, $clientIP);
        $stmt->execute();
        $stmt->close();
    }
    $conn->close();
}

echo json_encode(['success' => true, 'fileName' => $storedName]);