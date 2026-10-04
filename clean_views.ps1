$files = Get-ChildItem -Path "src\ExwhyzeeSP.Web\Areas", "src\ExwhyzeeSP.Web\Views" -Recurse -Filter *.cshtml
foreach ($f in $files) {
    $c = Get-Content $f.FullName -Raw
    $c = $c.Replace("X.X.PagedList", "X.PagedList")
    $c = $c.Replace("X.X.PagedList.Mvc.Core", "X.PagedList.Mvc.Core")
    $c = $c.Replace("PagedList.IPagedList", "IPagedList")
    $c = $c.Replace("X.PagedList.IPagedList", "IPagedList")
    $c = $c.Replace("PagedList.Mvc.Core.PagedListRenderOptions", "PagedListRenderOptions")
    $c = $c.Replace("using PagedList.Mvc;", "using X.PagedList.Mvc.Core;")
    $c = $c.Replace("using PagedList;", "using X.PagedList;`nusing X.PagedList.Mvc.Core;")
    Set-Content -Path $f.FullName -Value $c
}
Write-Host "Razor views cleaned successfully."
