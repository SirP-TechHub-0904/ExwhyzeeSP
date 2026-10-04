$servicesDir = "c:\Users\JURAY\source\SchoolPortal\ExwhyzeeSP\src\ExwhyzeeSP.Web\Areas\Data\Services"

Get-ChildItem -Path $servicesDir -Filter *.cs | ForEach-Object {
    $content = Get-Content -Raw -Path $_.FullName
    $name = [System.IO.Path]::GetFileNameWithoutExtension($_.FullName)

    if ($content -match "ApplicationUserManager" -or $content -match "ApplicationRoleManager" -or $content -match "GetOwinContext") {
        # Regex replacement for the legacy constructor & property block
        $pattern = @"
(?s)public\s+$name\s*\(\s*\)\s*\{.*?\}\s*public\s+$name\s*\(\s*ApplicationUserManager\s+userManager.*?private\s+ApplicationRoleManager\s+_roleManager;.*?set\s*\{\s*_roleManager\s*=\s*value;\s*\}\s*\}
"@
        
        $modernBoth = @"
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public $name()
        {
        }

        public $name(
            ApplicationDbContext context = null,
            UserManager<ApplicationUser> userManager = null,
            RoleManager<IdentityRole> roleManager = null)
        {
            if (context != null) db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public UserManager<ApplicationUser> UserManager => _userManager;
        public RoleManager<IdentityRole> RoleManager => _roleManager;
"@

        if ($content -match $pattern) {
            $content = $content -replace $pattern, $modernBoth
        } else {
            # Single UserManager pattern
            $patternSingle = @"
(?s)public\s+$name\s*\(\s*\)\s*\{.*?\}\s*public\s+$name\s*\(\s*ApplicationUserManager\s+userManager.*?private\s+ApplicationUserManager\s+_userManager;.*?set\s*\{\s*_userManager\s*=\s*value;\s*\}\s*\}
"@
            $modernSingle = @"
        private readonly UserManager<ApplicationUser> _userManager;

        public $name()
        {
        }

        public $name(
            ApplicationDbContext context = null,
            UserManager<ApplicationUser> userManager = null)
        {
            if (context != null) db = context;
            _userManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager => _userManager;
"@
            if ($content -match $patternSingle) {
                $content = $content -replace $patternSingle, $modernSingle
            }
        }

        # Also clean up any lingering System.Web / Owin usings
        $content = $content -replace "using Microsoft\.AspNet\.Identity\.Owin;", "using Microsoft.AspNetCore.Identity;"
        $content = $content -replace "using Microsoft\.AspNet\.Identity;", "using Microsoft.AspNetCore.Identity;"
        $content = $content -replace "using System\.Web;", "// using System.Web;"
        
        Set-Content -Path $_.FullName -Value $content -NoNewline
        Write-Host "Modernized Auth in $($_.Name)"
    }
}
