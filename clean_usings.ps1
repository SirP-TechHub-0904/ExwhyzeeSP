$modelsDir = "c:\Users\JURAY\source\SchoolPortal\ExwhyzeeSP\src\ExwhyzeeSP.Core\Models"
Get-ChildItem -Path $modelsDir -Filter "*.cs" | ForEach-Object {
    $content = Get-Content -Path $_.FullName -Raw
    $newContent = $content -replace 'using\s+JurayUniversal[^\r\n;]*;', 'using ExwhyzeeSP.Core.Entities;'
    if ($newContent -ne $content) {
        Set-Content -Path $_.FullName -Value $newContent -Encoding utf8
        Write-Host "CLEANED USINGS IN: $($_.Name)"
    }
}
