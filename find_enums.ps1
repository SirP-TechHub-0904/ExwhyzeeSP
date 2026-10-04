$src = "C:\Users\JURAY\source\repos\JurayUniversal"
$items = Get-ChildItem -Path $src -Recurse -Filter "*Enum*.cs"
foreach ($item in $items) {
    Write-Host "ENUM FILE: $($item.FullName)"
    Get-Content -Path $item.FullName -Raw | Write-Host
}
