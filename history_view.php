<?php
require_once 'db.php';

$conn = getDbConnection();
if (!$conn) 
{
    die("Ошибка подключения к базе данных.");
}

$sql = "SELECT 
            id, 
            originalName, 
            storedName,
            uploadTime, 
            originalWidth, 
            originalHeight, 
            targetSize, 
            finalWidth, 
            finalHeight,
            errorMessage,
            status,
            clientIP
        FROM history 
        ORDER BY uploadTime DESC";

$result = $conn->query($sql);
$history = [];
if ($result && $result->num_rows > 0) 
{
    while ($row = $result->fetch_assoc()) 
    {
        $history[] = $row;
    }
}
$conn->close();
?>
<!DOCTYPE html>
<html lang="ru">
<head>
    <meta charset="UTF-8">
    <title>История загрузок</title>
    <style>
        body 
        {
            font-family: Arial, sans-serif;
            margin: 20px;
            background-color: #f5f5f5;
        }
        h2 
        {
            color: #333;
        }
        table 
        {
            width: 100%;
            border-collapse: collapse;
            background-color: white;
            box-shadow: 0 1px 3px rgba(0,0,0,0.1);
        }
        th, td 
        {
            border: 1px solid #ddd;
            padding: 8px 10px;
            text-align: left;
            vertical-align: top;
        }
        th 
        {
            background-color: #DDA0DD;
            color: black;
            font-weight: bold;
        }
        tr:nth-child(even) 
        {
            background-color: #f9f9f9;
        }
        tr.success 
        {
            background-color: #90EE90;
        }
        tr.error 
        {
            background-color: #FFCCCC;
        }
        .filename a 
        {
            color: #0066cc;
            text-decoration: none;
            border-bottom: 1px dashed #0066cc;
        }
        .filename a:hover 
        {
            text-decoration: underline;
        }
        .error-msg 
        {
            color: #d32f2f;
            font-size: 0.9em;
        }
        .status-badge 
        {
            display: inline-block;
            padding: 2px 8px;
            border-radius: 12px;
            font-size: 0.85em;
            font-weight: bold;
        }
        .status-success 
        {
            background-color: #2e7d32;
            color: white;
        }
        .status-error 
        {
            background-color: #c62828;
            color: white;
        }
    </style>
</head>
<body>
    <h2>История загрузок</h2>
    <table>
        <thead>
            <tr>
                <th>ID</th>
                <th>Исходное имя</th>
                <th>Время загрузки</th>
                <th>Ширина (исх.)</th>
                <th>Высота (исх.)</th>
                <th>Целевая ширина</th>
                <th>Финальная ширина</th>
                <th>Финальная высота</th>
                <th>Статус</th>
                <th>Ошибка</th>
            </tr>
        </thead>
        <tbody>
            <?php if (empty($history)): ?>
                <tr>
                    <td colspan="10" style="text-align: center;">Нет записей</td>
                </tr>
            <?php else: ?>
                <?php foreach ($history as $row): ?>
                    <?php
                        $rowClass = '';
                        $statusClass = '';
                        if ($row['status'] === 'success') 
                        {
                            $rowClass = 'success';
                            $statusClass = 'status-success';
                        } elseif ($row['status'] === 'error') 
                        {
                            $rowClass = 'error';
                            $statusClass = 'status-error';
                        }

                        $originalName = htmlspecialchars($row['originalName'] ?? '');
                        $storedName = htmlspecialchars($row['storedName'] ?? '');
                        $uploadTime = htmlspecialchars($row['uploadTime'] ?? '');
                        $origW = $row['originalWidth'] ?? '—';
                        $origH = $row['originalHeight'] ?? '—';
                        $targetSize = $row['targetSize'] ?? '—';
                        $finalW = $row['finalWidth'] ?? '—';
                        $finalH = $row['finalHeight'] ?? '—';
                        $statusText = htmlspecialchars($row['status'] ?? '');
                        $errorMsg = htmlspecialchars($row['errorMessage'] ?? '');

                        // Формируем ссылку на изображение только для успешных загрузок, у которых есть storedName
                        $imageLink = '';
                        if ($row['status'] === 'success' && !empty($storedName)) 
                        {
                            $imageLink = "getuploads.php?file=" . urlencode($storedName);
                        }
                    ?>
                    <tr class="<?= $rowClass ?>">
                        <td><?= $row['id'] ?></td>
                        <td class="filename">
                            <?php if ($imageLink): ?>
                                <a href="<?= $imageLink ?>" target="_blank" title="Открыть изображение"><?= $originalName ?></a>
                            <?php else: ?>
                                <?= $originalName ?>
                            <?php endif; ?>
                        </td>
                        <td><?= $uploadTime ?></td>
                        <td><?= $origW ?></td>
                        <td><?= $origH ?></td>
                        <td><?= $targetSize ?></td>
                        <td><?= $finalW ?></td>
                        <td><?= $finalH ?></td>
                        <td>
                            <span class="status-badge <?= $statusClass ?>">
                                <?= $statusText ?>
                            </span>
                        </td>
                        <td class="error-msg"><?= $errorMsg !== '' ? $errorMsg : '—' ?></td>
                    </tr>
                <?php endforeach; ?>
            <?php endif; ?>
        </tbody>
    </table>
</body>
</html>