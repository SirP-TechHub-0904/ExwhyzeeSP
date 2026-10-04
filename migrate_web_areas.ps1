# PowerShell script to port Areas and Controllers from SchoolPortal.Web to ExwhyzeeSP.Web
$sourceBase = "c:\Users\JURAY\source\SchoolPortal\SchoolPortal.Web"
$destBase = "c:\Users\JURAY\source\SchoolPortal\ExwhyzeeSP\src\ExwhyzeeSP.Web"

# 1. Copy Areas
New-Item -ItemType Directory -Force -Path "$destBase\Areas" | Out-Null
Copy-Item -Path "$sourceBase\Areas\*" -Destination "$destBase\Areas" -Recurse -Force

# 2. Copy Root Controllers
New-Item -ItemType Directory -Force -Path "$destBase\Controllers" | Out-Null
Copy-Item -Path "$sourceBase\Controllers\*" -Destination "$destBase\Controllers" -Recurse -Force

# 3. Transform C# files
Get-ChildItem -Path "$destBase\Areas", "$destBase\Controllers" -Recurse -Filter *.cs | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    $content = $content.Replace("namespace SchoolPortal.Web.Areas", "namespace ExwhyzeeSP.Web.Areas")
    $content = $content.Replace("namespace SchoolPortal.Web.Controllers", "namespace ExwhyzeeSP.Web.Controllers")
    $content = $content.Replace("namespace SchoolPortal.Web", "namespace ExwhyzeeSP.Web")
    $content = $content.Replace("using SchoolPortal.Web.Models.Entities;", "using ExwhyzeeSP.Core.Entities;")
    $content = $content.Replace("using SchoolPortal.Web.Models.ResultArchive;", "using ExwhyzeeSP.Core.Entities.ResultArchive;")
    $content = $content.Replace("using SchoolPortal.Web.Models.UI;", "using ExwhyzeeSP.Core.Entities.UI;")
    $content = $content.Replace("using SchoolPortal.Web.Models.Dtos;", "using ExwhyzeeSP.Core.Dtos;")
    $content = $content.Replace("using SchoolPortal.Web.Models;", "using ExwhyzeeSP.Core.Models;`nusing ExwhyzeeSP.Data.Contexts;")
    $content = $content.Replace("using SchoolPortal.Web;", "using ExwhyzeeSP.Web;")
    $content = $content.Replace("using System.Web.Mvc;", "using Microsoft.AspNetCore.Mvc;`nusing Microsoft.AspNetCore.Authorization;")
    $content = $content.Replace("using System.Web;", "// using System.Web;")
    $content = $content.Replace("using System.Data.Entity;", "using Microsoft.EntityFrameworkCore;")
    $content = $content.Replace("using Microsoft.AspNet.Identity;", "using Microsoft.AspNetCore.Identity;`nusing System.Security.Claims;")
    $content = $content.Replace("using PagedList;", "using X.PagedList.Extensions;`nusing X.PagedList;")
    $content = $content.Replace("HttpPostedFileBase", "IFormFile")
    $content = $content.Replace("User.Identity.GetUserId()", "User.FindFirstValue(ClaimTypes.NameIdentifier)")
    Set-Content -Path $_.FullName -Value $content
}

# 4. Transform Razor .cshtml files
Get-ChildItem -Path "$destBase\Areas", "$destBase\Views" -Recurse -Filter *.cshtml | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    $content = $content.Replace("SchoolPortal.Web.Models.Entities", "ExwhyzeeSP.Core.Entities")
    $content = $content.Replace("SchoolPortal.Web.Models.ResultArchive", "ExwhyzeeSP.Core.Entities.ResultArchive")
    $content = $content.Replace("SchoolPortal.Web.Models.UI", "ExwhyzeeSP.Core.Entities.UI")
    $content = $content.Replace("SchoolPortal.Web.Models.Dtos", "ExwhyzeeSP.Core.Dtos")
    $content = $content.Replace("SchoolPortal.Web.Models", "ExwhyzeeSP.Core.Models")
    $content = $content.Replace("SchoolPortal.Web", "ExwhyzeeSP.Web")
    $content = $content.Replace("PagedList.Mvc", "X.PagedList.Mvc.Core")
    $content = $content.Replace("PagedList.IPagedList", "X.PagedList.IPagedList")
    $content = $content.Replace("PagedList", "X.PagedList")
    Set-Content -Path $_.FullName -Value $content
}

Write-Host "Areas, controllers, and views successfully ported and modernized."
