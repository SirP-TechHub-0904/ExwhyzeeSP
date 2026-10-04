$viewImports = Get-Content "src\ExwhyzeeSP.Web\Views\_ViewImports.cshtml" -Raw
$areaDirs = Get-ChildItem -Path "src\ExwhyzeeSP.Web\Areas" -Directory -Filter Views -Recurse
foreach ($dir in $areaDirs) {
    Set-Content -Path (Join-Path $dir.FullName "_ViewImports.cshtml") -Value $viewImports
}
Write-Host "ViewImports distributed to $($areaDirs.Count) Area Views folders."
