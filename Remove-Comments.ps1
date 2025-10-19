# Remove-Comments.ps1
# This script removes all comments from C# files in the solution
# Including: XML documentation (///), single-line (//), and multi-line (/* */)

param(
    [string]$Path = ".",
    [switch]$WhatIf = $false
)

$ErrorActionPreference = "Continue"
$filesProcessed = 0
$totalFiles = 0

Write-Host "Starting comment removal process..." -ForegroundColor Cyan
Write-Host "Target directory: $(Resolve-Path $Path)" -ForegroundColor Yellow
if ($WhatIf) {
    Write-Host "RUNNING IN WHATIF MODE - NO CHANGES WILL BE MADE" -ForegroundColor Magenta
}
Write-Host ""

# Find all .cs files
$csFiles = Get-ChildItem -Path $Path -Filter "*.cs" -Recurse | Where-Object {
    $_.FullName -notmatch '\\obj\\' -and
    $_.FullName -notmatch '\\bin\\' -and
    $_.FullName -notmatch '\\node_modules\\'
}

$totalFiles = $csFiles.Count
Write-Host "Found $totalFiles C# files to process" -ForegroundColor Green
Write-Host ""

foreach ($file in $csFiles) {
    try {
        Write-Host "Processing: $($file.Name)" -ForegroundColor Gray

        # Read file content
        $content = Get-Content -Path $file.FullName -Raw -Encoding UTF8
        $originalContent = $content

        # Step 1: Remove XML documentation comments (/// ...)
        $content = $content -replace '(?m)^\s*///.*$', ''

        # Step 2: Remove multi-line comments (/* ... */)
        # Use non-greedy matching to handle multiple comments in one file
        $content = $content -replace '(?s)/\*.*?\*/', ''

        # Step 3: Remove single-line comments (//)
        # But preserve URLs like http:// and https://
        # This regex looks for // that is NOT preceded by : (to keep URLs)
        $content = $content -replace '(?m)(?<!:)//(?!/)[^\r\n]*', ''

        # Step 4: Clean up empty lines (keep maximum of 2 consecutive empty lines)
        $content = $content -replace '(?m)^\s*$(\r?\n^\s*$)+', "`r`n"

        # Step 5: Remove trailing whitespace from each line
        $content = $content -replace '(?m)[ \t]+$', ''

        # Only write if content changed
        if ($content -ne $originalContent) {
            if (-not $WhatIf) {
                # Save the file with UTF-8 encoding (without BOM)
                $utf8NoBom = New-Object System.Text.UTF8Encoding $false
                [System.IO.File]::WriteAllText($file.FullName, $content, $utf8NoBom)

                Write-Host "  [OK] Comments removed" -ForegroundColor Green
                $filesProcessed++
            } else {
                Write-Host "  [WHATIF] Would remove comments" -ForegroundColor Yellow
                $filesProcessed++
            }
        } else {
            Write-Host "  [SKIP] No comments found" -ForegroundColor DarkGray
        }
    }
    catch {
        Write-Host "  [ERROR] Failed to process file: $_" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Summary:" -ForegroundColor Cyan
Write-Host "  Total files scanned: $totalFiles" -ForegroundColor White
Write-Host "  Files modified: $filesProcessed" -ForegroundColor Green
Write-Host "  Files unchanged: $($totalFiles - $filesProcessed)" -ForegroundColor Gray
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

if ($WhatIf) {
    Write-Host "This was a dry run. To actually remove comments, run without -WhatIf" -ForegroundColor Magenta
} else {
    Write-Host "Done! All comments have been removed." -ForegroundColor Green
    Write-Host "Remember to commit your changes to git!" -ForegroundColor Yellow
}
