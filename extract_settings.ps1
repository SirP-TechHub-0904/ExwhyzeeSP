$content = Get-Content -Path "C:\Users\JURAY\Documents\divne sql front.sql" -Raw
if ($content -match '(?s)(INSERT\s+\[dbo\]\.\[Settings\].*?\)\s*SET IDENTITY_INSERT)') {
    Set-Content -Path "c:\Users\JURAY\source\SchoolPortal\ExwhyzeeSP\settings_extracted.sql" -Value $matches[1]
    Write-Host "EXTRACTED SETTINGS INSERT BLOCK SUCCESSFULLY"
} else {
    Write-Host "COULD NOT MATCH SETTINGS INSERT"
}
