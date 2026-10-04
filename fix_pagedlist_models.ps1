$files = Get-ChildItem -Path "src\ExwhyzeeSP.Web\Areas", "src\ExwhyzeeSP.Web\Views" -Recurse -Filter *.cshtml
foreach ($f in $files) {
    $c = Get-Content $f.FullName -Raw
    $c = $c.Replace("X.PagedList.IX.PagedList", "IPagedList")
    $c = $c.Replace("X.PagedList.IPagedList", "IPagedList")
    $c = $c.Replace("PagedList.IPagedList", "IPagedList")
    $c = $c.Replace("X.PagedList.PagedList", "IPagedList")
    $c = $c.Replace("PagedList.PagedList", "IPagedList")
    $c = [System.Text.RegularExpressions.Regex]::Replace($c, '@model\s+X\.PagedList\.[A-Za-z0-9_.]*<', '@model IPagedList<')
    $c = [System.Text.RegularExpressions.Regex]::Replace($c, '@model\s+PagedList\.[A-Za-z0-9_.]*<', '@model IPagedList<')
    Set-Content -Path $f.FullName -Value $c
}
Write-Host "PagedList models cleaned up successfully."
