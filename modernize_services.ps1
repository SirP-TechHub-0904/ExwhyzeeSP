# Modernize Service files in ExwhyzeeSP.Web
$services = Get-ChildItem -Path "src\ExwhyzeeSP.Web\Areas", "src\ExwhyzeeSP.Web\Controllers" -Recurse -Filter *.cs
foreach ($f in $services) {
    $c = Get-Content $f.FullName -Raw
    $c = $c.Replace("HttpContext.Current.User.Identity.GetUserName()", "_httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? ""Admin""")
    $c = $c.Replace("HttpContext.Current.User.Identity.GetUserId()", "_httpContextAccessor?.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value")
    $c = $c.Replace("User.Identity.GetUserName()", "User.Identity?.Name ?? ""Admin""")
    $c = $c.Replace("User.Identity.GetUserId()", "User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value")
    $c = $c.Replace("System.Web.HttpContext.Current.Request", "HttpContext.Request")
    $c = $c.Replace("HttpContext.Current.Request", "HttpContext.Request")
    $c = $c.Replace("HttpContext.Current.Server.MapPath", "Path.Combine(Directory.GetCurrentDirectory(), ""wwwroot"") + ")
    $c = $c.Replace("Server.MapPath", "Path.Combine(Directory.GetCurrentDirectory(), ""wwwroot"") + ")
    $c = $c.Replace("HttpPostedFileBase", "IFormFile")
    $c = $c.Replace("using System.Web.Mvc;", "using Microsoft.AspNetCore.Mvc;`nusing Microsoft.AspNetCore.Authorization;")
    $c = $c.Replace("using System.Web;", "// using System.Web;")
    $c = $c.Replace("using System.Data.Entity;", "using Microsoft.EntityFrameworkCore;")
    $c = $c.Replace("EntityState.Modified", "EntityState.Modified")
    Set-Content -Path $f.FullName -Value $c
}
Write-Host "Services and controllers modernized."
