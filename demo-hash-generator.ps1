# Script para demostrar el generador de hashes
Write-Host "=== GENERADOR DE HASHES - SISTEMA SGA ===" -ForegroundColor Green
Write-Host ""

# Ruta del ejecutable
$hashGeneratorPath = "c:\Users\andri\Documents\D-Proyectos\Git\ProyectoAgiles\SistemaGestionAscensos\HashGenerator\bin\Debug\net9.0\HashGenerator.exe"

Write-Host "Generando hashes de ejemplo usando el mismo sistema que SGA..." -ForegroundColor Yellow
Write-Host ""

# Ejemplos de contraseñas comunes
$passwords = @("123456", "admin", "password", "Docente123!", "SGA2025")

Write-Host "=== HASHES DE CONTRASEÑAS (SHA256 + Base64) ===" -ForegroundColor Cyan
Write-Host "Formato usado en tabla Docentes - campo PasswordHash" -ForegroundColor Gray
Write-Host ""

foreach ($password in $passwords) {
    # Generar hash usando .NET directamente
    $sha256 = [System.Security.Cryptography.SHA256]::Create()
    $bytes = [System.Text.Encoding]::UTF8.GetBytes($password)
    $hash = $sha256.ComputeHash($bytes)
    $base64Hash = [Convert]::ToBase64String($hash)
    
    Write-Host "Contraseña: '$password'" -ForegroundColor White
    Write-Host "Hash:       $base64Hash" -ForegroundColor Green
    Write-Host "SQL:        UPDATE Docentes SET PasswordHash = '$base64Hash' WHERE NombreUsuario = 'usuario';" -ForegroundColor DarkGray
    Write-Host ""
}

Write-Host "=== HASHES DE TEXTO (SHA256 + Hexadecimal) ===" -ForegroundColor Cyan
Write-Host "Formato usado en tabla Documentos - campo HashSHA256" -ForegroundColor Gray
Write-Host ""

$texts = @("123456", "Documento de prueba", "Contenido del archivo PDF")

foreach ($text in $texts) {
    # Generar hash hexadecimal
    $sha256 = [System.Security.Cryptography.SHA256]::Create()
    $bytes = [System.Text.Encoding]::UTF8.GetBytes($text)
    $hash = $sha256.ComputeHash($bytes)
    $hexHash = [System.BitConverter]::ToString($hash) -replace '-', ''
    
    Write-Host "Texto: '$text'" -ForegroundColor White
    Write-Host "Hash:  $($hexHash.ToLower())" -ForegroundColor Green
    Write-Host ""
}

Write-Host "=== EJEMPLOS DE USO EN BASE DE DATOS ===" -ForegroundColor Cyan
Write-Host ""

# Ejemplo 1: Crear docente con contraseña
$examplePassword = "Docente123!"
$sha256 = [System.Security.Cryptography.SHA256]::Create()
$bytes = [System.Text.Encoding]::UTF8.GetBytes($examplePassword)
$hash = $sha256.ComputeHash($bytes)
$passwordHash = [Convert]::ToBase64String($hash)

Write-Host "1. INSERTAR DOCENTE CON CONTRASEÑA:" -ForegroundColor Yellow
Write-Host @"
INSERT INTO Docentes (
    Cedula, Nombres, Apellidos, Email, TelefonoContacto, 
    FacultadId, NivelActual, NombreUsuario, PasswordHash
) VALUES (
    '1756789012', 'Juan Carlos', 'Pérez', 'jperez@uta.edu.ec', '0987654321',
    1, 1, 'jperez', '$passwordHash'
);
"@ -ForegroundColor Green

Write-Host ""
Write-Host "2. VERIFICAR CONTRASEÑA EN LOGIN:" -ForegroundColor Yellow
Write-Host @"
-- Para verificar login, el sistema genera el hash de la contraseña ingresada
-- y lo compara con el almacenado en PasswordHash
SELECT * FROM Docentes 
WHERE NombreUsuario = 'jperez' 
AND PasswordHash = '$passwordHash';
"@ -ForegroundColor Green

Write-Host ""
Write-Host "=== HERRAMIENTA COMPILADA ===" -ForegroundColor Cyan
Write-Host "Se ha creado el ejecutable HashGenerator.exe en:" -ForegroundColor White
Write-Host $hashGeneratorPath -ForegroundColor Green
Write-Host ""
Write-Host "Para usarlo interactivamente, ejecuta:" -ForegroundColor White
Write-Host "dotnet run --project HashGenerator" -ForegroundColor Green
Write-Host ""
Write-Host "O directamente:" -ForegroundColor White
Write-Host $hashGeneratorPath -ForegroundColor Green
