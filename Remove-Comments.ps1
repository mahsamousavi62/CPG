# Remove-Comments.ps1
# This script removes all comments from C# files in the solution
# Including: XML documentation (///), single-line (//), and multi-line (/* */)

param(
    [string]$Path = ".",
    [switch]$WhatIf = $false
)

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

        # Remove XML documentation comments (/// <summary>, /// <param>, etc.)
        # This removes entire XML doc comment blocks
        $content = $content -replace '(?m)^\s*///.*$\r?\n?', ''

        # Remove multi-line comments (/* ... */)
        # This handles multi-line and single-line /* */ comments
        $content = $content -replace '/\*[\s\S]*?\*/', ''

        # Remove single-line comments (//)
        # But preserve URLs like http:// and https://
        $content = $content -replace '(?<!:)//(?!/)[^\r\n]*', ''

        # Remove empty lines that were left after comment removal
        # Keep maximum of 2 consecutive empty lines
        $content = $content -replace '(\r?\n){4,}', "`r`n`r`n`r`n"

        # Only write if content changed
        if ($content -ne $originalContent) {
            if (-not $WhatIf) {
                # Save the file with UTF-8 encoding (without BOM)
                $utf8NoBom = New-Object System.Text.UTF8Encoding $false
                [System.IO.File]::WriteAllText($file.FullName, $content, $utf8NoBom)

                Write-Host "  ✓ Comments removed" -ForegroundColor Green
                $filesProcessed++
            } else {
                Write-Host "  ➜ Would remove comments (WhatIf mode)" -ForegroundColor Yellow
                $filesProcessed++
            }
        } else {
            Write-Host "  - No comments found" -ForegroundColor DarkGray
        }
    }
    catch {
        Write-Host "  ✗ Error processing file: $_" -ForegroundColor Red
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
    Write-Host "Do not forget to commit your changes to git!" -ForegroundColor Yellow
}
