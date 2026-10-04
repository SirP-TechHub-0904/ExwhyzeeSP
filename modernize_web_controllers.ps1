$webDir = "c:\Users\JURAY\source\SchoolPortal\ExwhyzeeSP\src\ExwhyzeeSP.Web"

Get-ChildItem -Path $webDir -Recurse -Include *.cs, *.cshtml | ForEach-Object {
    $content = Get-Content -Raw -Path $_.FullName
    $changed = $false

    # Fix Bind(Include = "...") -> Bind("...")
    if ($content -match "\[Bind\(Include\s*=\s*") {
        $content = $content -replace "\[Bind\(Include\s*=\s*", "[Bind("
        $changed = $true
    }

    # Fix Bind(Exclude = "...") -> Bind("...")
    if ($content -match "\[Bind\(Exclude\s*=\s*") {
        $content = $content -replace "\[Bind\(Exclude\s*=\s*", "[Bind("
        $changed = $true
    }

    if ($changed) {
        Set-Content -Path $_.FullName -Value $content -NoNewline
        Write-Host "Updated Bind attributes in $($_.Name)"
    }
}
