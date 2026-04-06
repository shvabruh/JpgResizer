<?php
header('Content-Type: application/json');

require_once 'db.php';

$limit = (int)($_GET['limit'] ?? 10);
if ($limit <= 0) $limit = 10;

$conn = getDbConnection();
if (!$conn) {
    http_response_code(500);
    echo json_encode(['error' => 'БД не доступна']);
    exit;
}

$stmt = $conn->prepare("
    SELECT
        id, 
        originalName, 
        uploadTime, 
        originalWidth, 
        originalHeight, 
        targetSize, 
        finalWidth, 
        finalHeight,
        errorMessage,
        storedName,
        status,
        clientIP
    FROM history 
    ORDER BY uploadTime DESC 
    LIMIT ?
");

$stmt->bind_param("i", $limit);
$stmt->execute();
$result = $stmt->get_result();
$history = $result->fetch_all(MYSQLI_ASSOC);
echo json_encode($history);

$stmt->close();
$conn->close();