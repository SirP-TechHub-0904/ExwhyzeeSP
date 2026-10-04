$sourceEntities = "c:\Users\JURAY\source\SchoolPortal\SchoolPortal.Web\Models\Entities"
$destEntities = "c:\Users\JURAY\source\SchoolPortal\ExwhyzeeSP\src\ExwhyzeeSP.Core\Entities"

$sourceDtos = "c:\Users\JURAY\source\SchoolPortal\SchoolPortal.Web\Models\Dtos"
$destDtos = "c:\Users\JURAY\source\SchoolPortal\ExwhyzeeSP\src\ExwhyzeeSP.Core\Dtos"

# Copy all DTOs recursively
Get-ChildItem -Path $sourceDtos -Recurse -File | ForEach-Object {
    $relPath = $_.FullName.Substring($sourceDtos.Length)
    $targetFile = Join-Path $destDtos $relPath
    $targetDir = Split-Path $targetFile -Parent
    if (!(Test-Path $targetDir)) {
        New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
    }

    $content = Get-Content -Raw -Path $_.FullName
    $content = $content -replace "namespace SchoolPortal\.Web\.Models\.Dtos", "namespace ExwhyzeeSP.Core.Dtos"
    $content = $content -replace "namespace SchoolPortal\.Web\.Models\.Entities", "namespace ExwhyzeeSP.Core.Entities"
    $content = $content -replace "using SchoolPortal\.Web\.Models\.Entities;", "using ExwhyzeeSP.Core.Entities;"
    $content = $content -replace "using SchoolPortal\.Web\.Models\.Dtos;", "using ExwhyzeeSP.Core.Dtos;"
    $content = $content -replace "using SchoolPortal\.Web\.Models;", "using ExwhyzeeSP.Core.Models; using ExwhyzeeSP.Core.Entities;"
    $content = $content -replace "using System\.Web\.Mvc;", "using ExwhyzeeSP.Core.Attributes;"
    $content = $content -replace "using System\.Web;", "// using System.Web;"
    
    Set-Content -Path $targetFile -Value $content -NoNewline
    Write-Host "Synced DTO: $($_.Name)"
}
