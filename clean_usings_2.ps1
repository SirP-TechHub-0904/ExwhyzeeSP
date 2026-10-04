$modelsDir = "c:\Users\JURAY\source\SchoolPortal\ExwhyzeeSP\src\ExwhyzeeSP.Core\Models"
Get-ChildItem -Path $modelsDir -Filter "*.cs" | ForEach-Object {
    $content = Get-Content -Path $_.FullName -Raw
    $newContent = $content -replace 'using\s+static\s+JurayUniversal\.Domain\.Enum\.Enum;', 'using static ExwhyzeeSP.Core.Entities.Enum;'
    $newContent = $newContent -replace 'JurayUniversal\.Domain\.Enum', 'ExwhyzeeSP.Core.Entities'
    $newContent = $newContent -replace 'JurayUniversal\.Domain\.Models', 'ExwhyzeeSP.Core.Models'
    if ($newContent -ne $content) {
        Set-Content -Path $_.FullName -Value $newContent -Encoding utf8
        Write-Host "CLEANED IN: $($_.Name)"
    }
}
