$srcDir = "C:\Users\JURAY\source\repos\JurayUniversal\src\JurayUniversal.Domain\Models"
$destDir = "c:\Users\JURAY\source\SchoolPortal\ExwhyzeeSP\src\ExwhyzeeSP.Core\Models"

$modelsToCopy = @(
    "WebPage.cs",
    "PageCategory.cs",
    "PageSection.cs",
    "PageSectionList.cs",
    "DataConfig.cs",
    "SuperSetting.cs",
    "Slider.cs",
    "Blog.cs",
    "BlogCategory.cs",
    "Testimony.cs",
    "FAQ.cs",
    "PostModal.cs",
    "TemplateType.cs",
    "TemplateCategory.cs",
    "ClientRequest.cs",
    "ContactUs.cs"
)

foreach ($file in $modelsToCopy) {
    $srcFile = Join-Path $srcDir $file
    if (Test-Path $srcFile) {
        $content = Get-Content -Path $srcFile -Raw
        # Replace namespace
        $content = $content -replace 'namespace\s+JurayUniversal\.Domain\.Models', 'namespace ExwhyzeeSP.Core.Models'
        $content = $content -replace 'using\s+JurayUniversal\.Domain\.Models;', 'using ExwhyzeeSP.Core.Models;'
        $content = $content -replace 'using\s+JurayUniversal\.Domain\.Enum;', 'using ExwhyzeeSP.Core.Entities;'
        $content = $content -replace 'using\s+JurayUniversal\.Domain\.Entities;', 'using ExwhyzeeSP.Core.Entities;'
        
        $destFile = Join-Path $destDir $file
        Set-Content -Path $destFile -Value $content -Encoding utf8
        Write-Host "COPIED & SYNCHRONIZED: $file"
    } else {
        Write-Host "NOT FOUND: $srcFile"
    }
}
