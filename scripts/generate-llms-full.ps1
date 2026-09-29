<#
.SYNOPSIS
    Compiles all Tier 1 markdown documentation from docs/ into the consolidated Tier 3 llms-full.txt file.

.DESCRIPTION
    Scans the docs/ directory in a logical architectural reading order (Cross-cutting Architecture ->
    Development Workflows -> Modules), injects a Master Table of Contents, generates clear section
    delimiters, and outputs a complete, single-pass LLM ingestion document.
#>

[CmdletBinding()]
param(
    [string]$DocsRoot = "$PSScriptRoot/../docs",
    [string]$OutputFile = "$PSScriptRoot/../llms-full.txt"
)

$ErrorActionPreference = "Stop"

Write-Host "Compiling Tier 3 documentation into $OutputFile..." -ForegroundColor Cyan

$DocsRoot = (Resolve-Path $DocsRoot).Path
$OutputFile = [System.IO.Path]::GetFullPath($OutputFile)

# Defined canonical reading order
$orderedRelativePaths = @(
    "architecture/overview.md",
    "development/getting-started.md",
    "development/testing-strategy.md",
    "modules/directory-service/overview.md",
    "modules/directory-service/domain-model.md",
    "modules/directory-service/cqrs-application.md",
    "modules/directory-service/persistence.md",
    "modules/directory-service/api-presentation.md"
)

# Discover any additional documents in docs/ not explicitly listed above (excluding index.md)
$allDocs = Get-ChildItem -Path $DocsRoot -Recurse -Filter "*.md" | Where-Object { $_.Name -ne "index.md" }
$discoveredRelativePaths = @()
foreach ($doc in $allDocs) {
    $rel = [System.IO.Path]::GetRelativePath($DocsRoot, $doc.FullName).Replace("\", "/")
    if ($orderedRelativePaths -notcontains $rel) {
        $discoveredRelativePaths += $rel
    }
}

$finalPaths = $orderedRelativePaths + ($discoveredRelativePaths | Sort-Object)

# Build Header and Table of Contents
$sb = [System.Text.StringBuilder]::new()
[void]$sb.AppendLine("# dotnet-fullstack — Consolidated Complete Documentation (llms-full.txt)")
[void]$sb.AppendLine()
[void]$sb.AppendLine("> Comprehensive consolidated documentation for the dotnet-fullstack .NET 10 Modular Monolith.")
[void]$sb.AppendLine("> Generated automatically by scripts/generate-llms-full.ps1. Do not edit directly; update docs/ instead.")
[void]$sb.AppendLine()
[void]$sb.AppendLine("---")
[void]$sb.AppendLine()
[void]$sb.AppendLine("## Master Table of Contents")
[void]$sb.AppendLine()

$sectionIndex = 1
foreach ($relPath in $finalPaths) {
    $fullPath = Join-Path $DocsRoot $relPath
    if (Test-Path $fullPath) {
        $firstLine = Get-Content $fullPath -TotalCount 1
        $title = if ($firstLine -match "^#\s+(.+)$") { $matches[1] } else { [System.IO.Path]::GetFileNameWithoutExtension($relPath) }
        [void]$sb.AppendLine("$sectionIndex. [$title](#section-$sectionIndex) ($relPath)")
        $sectionIndex++
    }
}

[void]$sb.AppendLine()
[void]$sb.AppendLine("---")
[void]$sb.AppendLine()

# Append each file content with demarcations
$sectionIndex = 1
foreach ($relPath in $finalPaths) {
    $fullPath = Join-Path $DocsRoot $relPath
    if (-not (Test-Path $fullPath)) {
        Write-Warning "File not found: $relPath"
        continue
    }

    $content = Get-Content -Path $fullPath -Raw -Encoding utf8
    
    [void]$sb.AppendLine("<a name=`"section-$sectionIndex`"></a>")
    [void]$sb.AppendLine("<!-- ===================================================================== -->")
    [void]$sb.AppendLine("<!-- START DOCUMENT $sectionIndex of $($finalPaths.Count): $relPath -->")
    [void]$sb.AppendLine("<!-- ===================================================================== -->")
    [void]$sb.AppendLine()
    [void]$sb.AppendLine($content.Trim())
    [void]$sb.AppendLine()
    [void]$sb.AppendLine("<!-- END DOCUMENT: $relPath -->")
    [void]$sb.AppendLine()
    [void]$sb.AppendLine("---")
    [void]$sb.AppendLine()

    $sectionIndex++
}

$compiledText = $sb.ToString()
[System.IO.File]::WriteAllText($OutputFile, $compiledText, [System.Text.Encoding]::UTF8)

$charCount = $compiledText.Length
$estTokens = [Math]::Round($charCount / 4)

Write-Host "Success! Compiled $($finalPaths.Count) documents into $OutputFile" -ForegroundColor Green
Write-Host "Total Characters: $charCount (~$estTokens tokens)" -ForegroundColor Yellow
