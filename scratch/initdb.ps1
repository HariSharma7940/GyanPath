$masterConnStr = "Server=(localdb)\mssqllocaldb;Database=master;Trusted_Connection=True;TrustServerCertificate=True;"
$dbConnStr = "Server=(localdb)\mssqllocaldb;Database=GyanPathDB;Trusted_Connection=True;TrustServerCertificate=True;"

# 1. Ensure GyanPathDB exists
$connMaster = New-Object Microsoft.Data.SqlClient.SqlConnection($masterConnStr)
$connMaster.Open()
$cmdMaster = $connMaster.CreateCommand()
$cmdMaster.CommandText = "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'GyanPathDB') CREATE DATABASE [GyanPathDB];"
$cmdMaster.ExecuteNonQuery()
$connMaster.Close()

# 2. Execute Tables.sql, SeedData.sql, StoredProcedures.sql
$connDb = New-Object Microsoft.Data.SqlClient.SqlConnection($dbConnStr)
$connDb.Open()

function Execute-SqlScriptFile($path) {
    Write-Host "Executing $path..."
    $text = Get-Content $path -Raw
    $blocks = [regex]::Split($text, '^\s*GO\s*$', [System.Text.RegularExpressions.RegexOptions]::IgnoreCase -bor [System.Text.RegularExpressions.RegexOptions]::Multiline)
    foreach ($block in $blocks) {
        $trimmed = $block.Trim()
        if (-not [string]::IsNullOrWhiteSpace($trimmed)) {
            $cmd = $connDb.CreateCommand()
            $cmd.CommandText = $trimmed
            $cmd.ExecuteNonQuery() | Out-Null
        }
    }
}

Execute-SqlScriptFile "SQL\Tables.sql"
Execute-SqlScriptFile "SQL\SeedData.sql"
Execute-SqlScriptFile "SQL\StoredProcedures.sql"

Write-Host "Database successfully created and seeded!"
$connDb.Close()
