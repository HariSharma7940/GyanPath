$saltBytes = [Convert]::FromBase64String('dXNlcjFzYWx0MTIzNDU2Nw==')
$pAdmin = New-Object System.Security.Cryptography.Rfc2898DeriveBytes('Admin@123', $saltBytes, 10000, [System.Security.Cryptography.HashAlgorithmName]::SHA256)
$hAdmin = [Convert]::ToBase64String($pAdmin.GetBytes(32))

$pStudent = New-Object System.Security.Cryptography.Rfc2898DeriveBytes('Student@123', $saltBytes, 10000, [System.Security.Cryptography.HashAlgorithmName]::SHA256)
$hStudent = [Convert]::ToBase64String($pStudent.GetBytes(32))

Write-Host "Salt: dXNlcjFzYWx0MTIzNDU2Nw=="
Write-Host "Admin@123 Hash: $hAdmin"
Write-Host "Student@123 Hash: $hStudent"
