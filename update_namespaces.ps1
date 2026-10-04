$webDir = "c:\Users\JURAY\source\SchoolPortal\ExwhyzeeSP\src\ExwhyzeeSP.Web"

Get-ChildItem -Path $webDir -Recurse -Include *.cs, *.cshtml | ForEach-Object {
    $content = Get-Content -Raw -Path $_.FullName
    $changed = $false

    # Namespaces
    if ($content -match "SchoolPortal\.Web") {
        $content = $content -replace "SchoolPortal\.Web\.Models\.Entities", "ExwhyzeeSP.Core.Entities"
        $content = $content -replace "SchoolPortal\.Web\.Models\.Dtos\.Zoom", "ExwhyzeeSP.Core.Dtos.Zoom"
        $content = $content -replace "SchoolPortal\.Web\.Models\.Dtos\.Api", "ExwhyzeeSP.Core.Dtos.Api"
        $content = $content -replace "SchoolPortal\.Web\.Models\.Dtos", "ExwhyzeeSP.Core.Dtos"
        $content = $content -replace "SchoolPortal\.Web\.Models", "ExwhyzeeSP.Core.Models"
        $content = $content -replace "SchoolPortal\.Web", "ExwhyzeeSP.Web"
        $changed = $true
    }

    if ($content -match "System\.Data\.Entity") {
        $content = $content -replace "using System\.Data\.Entity;", "using Microsoft.EntityFrameworkCore;"
        $changed = $true
    }

    if ($content -match "Microsoft\.AspNet\.Identity\.EntityFramework") {
        $content = $content -replace "Microsoft\.AspNet\.Identity\.EntityFramework", "Microsoft.AspNetCore.Identity"
        $changed = $true
    }

    if ($content -match "Microsoft\.AspNet\.Identity") {
        $content = $content -replace "Microsoft\.AspNet\.Identity", "Microsoft.AspNetCore.Identity"
        $changed = $true
    }

    if ($content -match "System\.Web\.Mvc") {
        $content = $content -replace "System\.Web\.Mvc", "Microsoft.AspNetCore.Mvc"
        $changed = $true
    }

    if ($content -match "System\.Web\.Routing") {
        $content = $content -replace "System\.Web\.Routing", "Microsoft.AspNetCore.Routing"
        $changed = $true
    }

    if ($changed) {
        Set-Content -Path $_.FullName -Value $content -NoNewline
        Write-Host "Updated $($_.Name)"
    }
}
