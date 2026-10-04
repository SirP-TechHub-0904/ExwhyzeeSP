# PowerShell script to port Models from SchoolPortal.Web to ExwhyzeeSP.Core
$sourceBase = "c:\Users\JURAY\source\SchoolPortal\SchoolPortal.Web\Models"
$destBase = "c:\Users\JURAY\source\SchoolPortal\ExwhyzeeSP\src\ExwhyzeeSP.Core"

New-Item -ItemType Directory -Force -Path "$destBase\Entities" | Out-Null
New-Item -ItemType Directory -Force -Path "$destBase\Entities\ResultArchive" | Out-Null
New-Item -ItemType Directory -Force -Path "$destBase\Entities\UI" | Out-Null
New-Item -ItemType Directory -Force -Path "$destBase\Dtos" | Out-Null
New-Item -ItemType Directory -Force -Path "$destBase\Models" | Out-Null

Copy-Item -Path "$sourceBase\Entities\*" -Destination "$destBase\Entities" -Recurse -Force
Copy-Item -Path "$sourceBase\ResultArchive\*" -Destination "$destBase\Entities\ResultArchive" -Recurse -Force
Copy-Item -Path "$sourceBase\UI\*" -Destination "$destBase\Entities\UI" -Recurse -Force
if (Test-Path "$sourceBase\Dtos") {
    Copy-Item -Path "$sourceBase\Dtos\*" -Destination "$destBase\Dtos" -Recurse -Force
}

# Copy root model files (e.g. FAQ.cs, PageCategory.cs, etc.)
Get-ChildItem -Path "$sourceBase\*.cs" | ForEach-Object {
    if ($_.Name -ne "IdentityModels.cs" -and $_.Name -ne "AccountViewModels.cs" -and $_.Name -ne "ManageViewModels.cs") {
        Copy-Item -Path $_.FullName -Destination "$destBase\Models\$($_.Name)" -Force
    }
}

Get-ChildItem -Path "$destBase" -Recurse -Filter *.cs | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    $content = $content.Replace("namespace SchoolPortal.Web.Models.Entities", "namespace ExwhyzeeSP.Core.Entities")
    $content = $content.Replace("namespace SchoolPortal.Web.Models.ResultArchive", "namespace ExwhyzeeSP.Core.Entities.ResultArchive")
    $content = $content.Replace("namespace SchoolPortal.Web.Models.UI", "namespace ExwhyzeeSP.Core.Entities.UI")
    $content = $content.Replace("namespace SchoolPortal.Web.Models.Dtos", "namespace ExwhyzeeSP.Core.Dtos")
    $content = $content.Replace("namespace SchoolPortal.Web.Models", "namespace ExwhyzeeSP.Core.Models")
    $content = $content.Replace("using SchoolPortal.Web.Models.Entities;", "using ExwhyzeeSP.Core.Entities;")
    $content = $content.Replace("using SchoolPortal.Web.Models.ResultArchive;", "using ExwhyzeeSP.Core.Entities.ResultArchive;")
    $content = $content.Replace("using SchoolPortal.Web.Models.UI;", "using ExwhyzeeSP.Core.Entities.UI;")
    $content = $content.Replace("using SchoolPortal.Web.Models.Dtos;", "using ExwhyzeeSP.Core.Dtos;")
    $content = $content.Replace("using SchoolPortal.Web.Models;", "using ExwhyzeeSP.Core.Models;")
    $content = $content.Replace("using System.Web.Mvc;", "using Microsoft.AspNetCore.Mvc;")
    $content = $content.Replace("using System.Web;", "// using System.Web;")
    Set-Content -Path $_.FullName -Value $content
}

Write-Host "Model porting completed successfully."
