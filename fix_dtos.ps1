$coreDtos = "c:\Users\JURAY\source\SchoolPortal\ExwhyzeeSP\src\ExwhyzeeSP.Core\Dtos"

# Fix PrintResultArchiveDto.cs
$file1 = Join-Path $coreDtos "PrintResultArchiveDto.cs"
if (Test-Path $file1) {
    $c = Get-Content -Raw $file1
    $c = $c -replace "using SchoolPortal\.Web\.Models\.ResultArchive;", "using ExwhyzeeSP.Core.Entities.ResultArchive;"
    Set-Content -Path $file1 -Value $c -NoNewline
}

# Fix PrintThirdTermArchiveDto.cs
$file2 = Join-Path $coreDtos "PrintThirdTermArchiveDto.cs"
if (Test-Path $file2) {
    $c = Get-Content -Raw $file2
    $c = $c -replace "using SchoolPortal\.Web\.Models\.ResultArchive;", "using ExwhyzeeSP.Core.Entities.ResultArchive;"
    Set-Content -Path $file2 -Value $c -NoNewline
}

# Fix SubjectListPrintoutArchiveDto.cs
$file3 = Join-Path $coreDtos "SubjectListPrintoutArchiveDto.cs"
if (Test-Path $file3) {
    $c = Get-Content -Raw $file3
    $c = $c -replace "using SchoolPortal\.Web\.Models\.ResultArchive;", "using ExwhyzeeSP.Core.Entities.ResultArchive;"
    $c = $c -replace "private ApplicationDbContext db = new ApplicationDbContext\(\);", "// In-memory grading calculation via GradingEvaluator"
    $c = $c -replace "using ExwhyzeeSP\.Core\.Entities;", "using ExwhyzeeSP.Core.Entities;`nusing ExwhyzeeSP.Core.Services;"
    Set-Content -Path $file3 -Value $c -NoNewline
}

# Fix SubjectListPrintoutDto.cs
$file4 = Join-Path $coreDtos "SubjectListPrintoutDto.cs"
if (Test-Path $file4) {
    $c = Get-Content -Raw $file4
    $c = $c -replace "private ApplicationDbContext db = new ApplicationDbContext\(\);", "// In-memory grading calculation via GradingEvaluator"
    $c = $c -replace "using ExwhyzeeSP\.Core\.Entities;", "using ExwhyzeeSP.Core.Entities;`nusing ExwhyzeeSP.Core.Services;"
    Set-Content -Path $file4 -Value $c -NoNewline
}

# Fix EnrolledSubjectDto.cs
$file5 = Join-Path $coreDtos "EnrolledSubjectDto.cs"
if (Test-Path $file5) {
    $c = Get-Content -Raw $file5
    $c = $c -replace "private ApplicationDbContext db = new ApplicationDbContext\(\);", "// In-memory grading calculation via GradingEvaluator"
    $c = $c -replace "using ExwhyzeeSP\.Core\.Entities;", "using ExwhyzeeSP.Core.Entities;`nusing ExwhyzeeSP.Core.Services;"
    Set-Content -Path $file5 -Value $c -NoNewline
}
