<?php
$uploadDir = 'uploads/';
$fileName = basename($_GET['file'] ?? '');

if (!$fileName)
{
    http_response_code(400);
    die('Не указан файл');
}

// Проверка, что запрашивается именно JPG
$ext = strtolower(pathinfo($fileName, PATHINFO_EXTENSION));
if (!in_array($ext, ['jpg', 'jpeg'])) 
{
    http_response_code(400);
    die('Разрешены только JPG/JPEG файлы');
}

$filePath = $uploadDir . $fileName;
if (!file_exists($filePath)) 
{
    http_response_code(404);
    die('Файл не найден');
}

$mime = mime_content_type($filePath) ?: 'image/jpeg';
header('Content-Type: ' . $mime);
header('Content-Length: ' . filesize($filePath));
readfile($filePath);
exit;