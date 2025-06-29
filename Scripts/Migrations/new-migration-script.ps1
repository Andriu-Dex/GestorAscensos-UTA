# Script para generar nuevos scripts de migración basados en plantillas
param(
    [ValidateSet("pre", "post", "rollback")]
    [string]$Type = "post",
    
    [Parameter(Mandatory=$true)]
    [string]$Name,
    
    [string]$Number = "",
    [string]$Author = $env:USERNAME,
    [string]$Database = "SGA_Main"
)

# Configuración
$TemplatesPath = "..\..\Templates"
$ScriptsPath = "Scripts\Migrations"

function Get-NextScriptNumber {
    param([string]$ScriptDir)
    
    if (-not (Test-Path $ScriptDir)) {
        return "01"
    }
    
    $existingScripts = Get-ChildItem -Path $ScriptDir -Filter "*.sql" | 
        Where-Object { $_.Name -match "^(\d+)-" } |
        ForEach-Object { [int]$Matches[1] } |
        Sort-Object
    
    if ($existingScripts.Count -eq 0) {
        return "01"
    }
    
    $nextNumber = ($existingScripts | Measure-Object -Maximum).Maximum + 1
    return $nextNumber.ToString("00")
}

function New-MigrationScript {
    param(
        [string]$ScriptType,
        [string]$ScriptName,
        [string]$ScriptNumber,
        [string]$AuthorName,
        [string]$DatabaseName
    )
    
    # Determinar paths y archivos
    $templateFile = switch ($ScriptType) {
        "pre" { "$TemplatesPath\pre-migration-template.sql" }
        "post" { "$TemplatesPath\post-migration-template.sql" }
        "rollback" { "$TemplatesPath\rollback-template.sql" }
    }
    
    $outputDir = switch ($ScriptType) {
        "pre" { "$ScriptsPath\Pre-Migration" }
        "post" { "$ScriptsPath\Post-Migration" }
        "rollback" { "$ScriptsPath\Rollback" }
    }
    
    # Crear directorio si no existe
    if (-not (Test-Path $outputDir)) {
        New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
    }
    
    # Obtener número automático si no se especificó
    if ([string]::IsNullOrEmpty($ScriptNumber)) {
        $ScriptNumber = Get-NextScriptNumber -ScriptDir $outputDir
    }
    
    # Generar nombre de archivo
    $fileName = "$ScriptNumber-$($ScriptName.ToLower().Replace(' ', '-').Replace('_', '-')).sql"
    $outputFile = "$outputDir\$fileName"
    
    # Verificar si el archivo ya existe
    if (Test-Path $outputFile) {
        Write-Host "❌ Error: El archivo ya existe: $outputFile" -ForegroundColor Red
        return $false
    }
    
    # Verificar si existe la plantilla
    if (-not (Test-Path $templateFile)) {
        Write-Host "❌ Error: Plantilla no encontrada: $templateFile" -ForegroundColor Red
        return $false
    }
    
    try {
        # Leer plantilla
        $template = Get-Content $templateFile -Raw
        
        # Reemplazar marcadores de posición
        $content = $template.
            Replace('[NUMERO]', $ScriptNumber).
            Replace('[DESCRIPCION]', $ScriptName).
            Replace('[NOMBRE]', $AuthorName).
            Replace('[FECHA]', (Get-Date -Format "yyyy-MM-dd")).
            Replace('[DATABASE_NAME]', $DatabaseName)
        
        # Escribir archivo
        $content | Set-Content -Path $outputFile -Encoding UTF8
        
        Write-Host "✅ Script creado exitosamente:" -ForegroundColor Green
        Write-Host "   📁 Archivo: $outputFile" -ForegroundColor Cyan
        Write-Host "   📝 Tipo: $ScriptType-migration" -ForegroundColor Cyan
        Write-Host "   🔢 Número: $ScriptNumber" -ForegroundColor Cyan
        Write-Host "   👤 Autor: $AuthorName" -ForegroundColor Cyan
        
        # Abrir archivo en editor si está disponible
        if (Get-Command "code" -ErrorAction SilentlyContinue) {
            Write-Host "   🚀 Abriendo en VS Code..." -ForegroundColor Yellow
            Start-Process "code" -ArgumentList $outputFile
        }
        
        return $true
        
    } catch {
        Write-Host "❌ Error creando script: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Función principal
function Main {
    Write-Host "🚀 Generador de Scripts de Migración" -ForegroundColor Cyan
    Write-Host "===================================" -ForegroundColor Cyan
    
    # Validar nombre
    if ([string]::IsNullOrEmpty($Name)) {
        Write-Host "❌ Error: Se requiere especificar -Name" -ForegroundColor Red
        return
    }
    
    # Sanitizar nombre
    $sanitizedName = $Name.Trim()
    if ([string]::IsNullOrEmpty($sanitizedName)) {
        Write-Host "❌ Error: El nombre no puede estar vacío" -ForegroundColor Red
        return
    }
    
    Write-Host "📋 Configuración:" -ForegroundColor White
    Write-Host "   Tipo: $Type" -ForegroundColor Gray
    Write-Host "   Nombre: $sanitizedName" -ForegroundColor Gray
    Write-Host "   Número: $(if ($Number) { $Number } else { 'Auto' })" -ForegroundColor Gray
    Write-Host "   Autor: $Author" -ForegroundColor Gray
    Write-Host "   Base de datos: $Database" -ForegroundColor Gray
    Write-Host ""
    
    # Crear script
    $success = New-MigrationScript -ScriptType $Type -ScriptName $sanitizedName -ScriptNumber $Number -AuthorName $Author -DatabaseName $Database
    
    if ($success) {
        Write-Host ""
        Write-Host "🎉 ¡Script generado correctamente!" -ForegroundColor Green
        Write-Host ""
        Write-Host "📚 Próximos pasos:" -ForegroundColor Yellow
        Write-Host "   1. Editar el script generado" -ForegroundColor Gray
        Write-Host "   2. Reemplazar los comentarios [DESCRIBIR...] con código real" -ForegroundColor Gray
        Write-Host "   3. Probar el script en un entorno de desarrollo" -ForegroundColor Gray
        Write-Host "   4. Ejecutar con el framework: .\migration-framework-v2.ps1 -Action apply-scripts" -ForegroundColor Gray
    } else {
        Write-Host ""
        Write-Host "❌ Error generando script" -ForegroundColor Red
        exit 1
    }
}

# Mostrar ayuda si se solicita
if ($args -contains "-Help" -or $args -contains "-h") {
    Write-Host @"
🚀 Generador de Scripts de Migración

DESCRIPCIÓN:
  Genera nuevos scripts SQL basados en plantillas predefinidas para 
  ser usados con el framework de migraciones.

PARÁMETROS:
  -Type       Tipo de script: pre, post, rollback (default: post)
  -Name       Nombre descriptivo del script (REQUERIDO)
  -Number     Número del script (opcional, se genera automáticamente)
  -Author     Nombre del autor (default: usuario actual)
  -Database   Nombre de la base de datos (default: SGA_Main)

EJEMPLOS:
  # Crear script post-migración
  .\new-migration-script.ps1 -Type post -Name "Add User Roles"
  
  # Crear script pre-migración con número específico
  .\new-migration-script.ps1 -Type pre -Name "Backup Critical Data" -Number 05
  
  # Crear script de rollback
  .\new-migration-script.ps1 -Type rollback -Name "Remove User Roles"

TIPOS DE SCRIPTS:
  pre      - Scripts que se ejecutan ANTES de aplicar migraciones EF
  post     - Scripts que se ejecutan DESPUÉS de aplicar migraciones EF  
  rollback - Scripts para revertir cambios (se ejecutan en orden inverso)

ESTRUCTURA GENERADA:
  Scripts/
    ├── Pre-Migration/
    │   └── 01-backup-critical-data.sql
    ├── Post-Migration/
    │   └── 01-add-user-roles.sql
    └── Rollback/
        └── 01-rollback-user-roles.sql
"@ -ForegroundColor Cyan
    exit 0
}

# Ejecutar función principal
Main
