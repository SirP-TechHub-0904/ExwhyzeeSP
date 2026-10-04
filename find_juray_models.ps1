$src = "C:\Users\JURAY\source\repos\JurayUniversal"
$items = Get-ChildItem -Path $src -Directory -Recurse | Where-Object { $_.Name -in @("Models", "Entities", "Dtos") }
foreach ($item in $items) {
    Write-Host "FOLDER: $($item.FullName)"
    Get-ChildItem -Path $item.FullName -Filter "*.cs" | ForEach-Object {
        Write-Host "   - $($_.Name)"
    }
}
