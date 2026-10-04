$content = Get-Content -Path "C:\Users\JURAY\Documents\divne sql front.sql"
Write-Host "--- ALL PageCategories INSERTS IN FILE ---"
for ($i = 0; $i -lt $content.Length; $i++) {
    if ($content[$i] -match 'PageCategories') {
        Write-Host "Line $($i+1): $($content[$i])"
    }
}
