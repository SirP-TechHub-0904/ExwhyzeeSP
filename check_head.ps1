$content = Get-Content -Path "C:\Users\JURAY\Documents\divne sql front.sql"
Write-Host "--- FIRST 15 LINES OF divne sql front.sql ---"
for ($i = 0; $i -lt [Math]::Min(15, $content.Length); $i++) {
    Write-Host "Line $($i+1): $($content[$i])"
}
