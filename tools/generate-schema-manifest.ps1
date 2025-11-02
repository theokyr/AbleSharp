param(
    [string]$SchemaDirectory = "resources/schemas",
    [string]$OutputPath = "resources/schemas/schema-descriptors.json"
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $SchemaDirectory)) {
    throw "Schema directory '$SchemaDirectory' was not found."
}

$files = Get-ChildItem -Path $SchemaDirectory -Filter "*.txt" -File

if ($files.Count -eq 0) {
    throw "No schema files were discovered under '$SchemaDirectory'."
}

$manifest = @()

foreach ($file in $files) {
    $name = [System.IO.Path]::GetFileNameWithoutExtension($file.Name)

    if ($name -notmatch '^(?<major>\d+)\.(?<minor>\d+)_(?<change>\d+)$') {
        Write-Warning "Skipping unrecognized schema file name: $($file.Name)"
        continue
    }

    $hash = Get-FileHash -Path $file.FullName -Algorithm SHA256

    $manifest += [PSCustomObject]@{
        versionId = $name
        majorVersion = [int]$matches.major
        minorVersion = $matches.minor
        schemaChangeCount = [int]$matches.change
        fileName = $file.Name
        sizeBytes = $file.Length
        sha256 = $hash.Hash.ToLower()
        lastModified = $file.LastWriteTimeUtc.ToString("o")
    }
}

if ($manifest.Count -eq 0) {
    throw "No schema descriptors could be generated."
}

$orderedManifest = $manifest | Sort-Object versionId

$json = $orderedManifest | ConvertTo-Json -Depth 5

Set-Content -Path $OutputPath -Value $json -Encoding UTF8

Write-Host "Generated $(($orderedManifest | Measure-Object).Count) schema descriptors into '$OutputPath'."

