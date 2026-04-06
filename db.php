<?php
function getDbConnection() 
{
    $host = 'localhost';
    $user = 'root';
    $password = '';
    $database = 'jpg_history';
    
    $conn = new mysqli($host, $user, $password, $database);
    if ($conn->connect_error) 
    {
        error_log("Database connection failed: " . $conn->connect_error);
        return null;
    }
    $conn->set_charset("utf8mb4");
    return $conn;
}