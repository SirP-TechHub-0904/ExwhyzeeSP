$connStr = "Server=JSS\SQLEXPRESS;Database=db_a42ea7_portaldivine;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
$sql = Get-Content -Path "c:\Users\JURAY\source\SchoolPortal\ExwhyzeeSP\ApplyWebsiteCMS.sql" -Raw

$conn = New-Object System.Data.SqlClient.SqlConnection($connStr)
$conn.Open()

$batches = $sql -split '(?m)^\s*GO\s*$'
foreach ($b in $batches) {
    $trimmed = $b.Trim()
    if ($trimmed.Length -gt 0) {
        $cmd = $conn.CreateCommand()
        $cmd.CommandText = $trimmed
        $cmd.ExecuteNonQuery() | Out-Null
    }
}

$conn.Close()
Write-Host "DATABASE UPDATE COMPLETE: All CMS tables and Settings columns are now in place!"
