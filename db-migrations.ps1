#!/usr/bin/env pwsh
param (
    [Parameter(Mandatory=$false)]
    [string]$Action = "help",
    
    [Parameter(Mandatory=$false)]
    [string]$Name = ""
)

$rootDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$infrastructureDir = Join-Path $rootDir "SGA.Infrastructure"
$startupProjectPath = Join-Path $rootDir "SGA.Api\SGA.Api.csproj"

function Show-Help {
    Write-Host "Database Migration Script for SGA"
    Write-Host "=================================="
    Write-Host ""
    Write-Host "Usage:"
    Write-Host "  .\db-migrations.ps1 <action> [parameters]"
    Write-Host ""
    Write-Host "Actions:"
    Write-Host "  add <name>     - Create a new migration with the specified name"
    Write-Host "  apply          - Apply pending migrations to the database"
    Write-Host "  remove         - Remove the last migration (if not applied)"
    Write-Host "  list           - List all migrations"
    Write-Host "  help           - Show this help message"
    Write-Host ""
    Write-Host "Examples:"
    Write-Host "  .\db-migrations.ps1 add InitialMigration"
    Write-Host "  .\db-migrations.ps1 apply"
    Write-Host "  .\db-migrations.ps1 list"
    Write-Host ""
}

function Add-Migration {
    param (
        [string]$MigrationName
    )
    
    if ([string]::IsNullOrEmpty($MigrationName)) {
        Write-Host "Error: Migration name is required." -ForegroundColor Red
        Show-Help
        return
    }
    
    Write-Host "Creating migration '$MigrationName'..." -ForegroundColor Cyan
    
    try {
        # Change directory to infrastructure project
        Push-Location $infrastructureDir
        
        # Run EF Core command to add migration
        dotnet ef migrations add $MigrationName --startup-project $startupProjectPath --verbose
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Migration '$MigrationName' created successfully!" -ForegroundColor Green
        } else {
            Write-Host "Failed to create migration. Check the error message above." -ForegroundColor Red
        }
    } catch {
        Write-Host "An error occurred: $_" -ForegroundColor Red
    } finally {
        # Return to original directory
        Pop-Location
    }
}

function Apply-Migrations {
    Write-Host "Applying migrations to the database..." -ForegroundColor Cyan
    
    try {
        # Change directory to infrastructure project
        Push-Location $infrastructureDir
        
        # Run EF Core command to update database
        dotnet ef database update --startup-project $startupProjectPath --verbose
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Migrations applied successfully!" -ForegroundColor Green
        } else {
            Write-Host "Failed to apply migrations. Check the error message above." -ForegroundColor Red
        }
    } catch {
        Write-Host "An error occurred: $_" -ForegroundColor Red
    } finally {
        # Return to original directory
        Pop-Location
    }
}

function Remove-LastMigration {
    Write-Host "Removing the last migration..." -ForegroundColor Cyan
    
    try {
        # Change directory to infrastructure project
        Push-Location $infrastructureDir
        
        # Run EF Core command to remove migration
        dotnet ef migrations remove --startup-project $startupProjectPath --verbose --force
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Last migration removed successfully!" -ForegroundColor Green
        } else {
            Write-Host "Failed to remove migration. Check the error message above." -ForegroundColor Red
        }
    } catch {
        Write-Host "An error occurred: $_" -ForegroundColor Red
    } finally {
        # Return to original directory
        Pop-Location
    }
}

function List-Migrations {
    Write-Host "Listing all migrations..." -ForegroundColor Cyan
    
    try {
        # Change directory to infrastructure project
        Push-Location $infrastructureDir
        
        # Run EF Core command to list migrations
        dotnet ef migrations list --startup-project $startupProjectPath
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Failed to list migrations. Check the error message above." -ForegroundColor Red
        }
    } catch {
        Write-Host "An error occurred: $_" -ForegroundColor Red
    } finally {
        # Return to original directory
        Pop-Location
    }
}

# Main script execution
switch ($Action.ToLower()) {
    "add" { Add-Migration -MigrationName $Name }
    "apply" { Apply-Migrations }
    "remove" { Remove-LastMigration }
    "list" { List-Migrations }
    default { Show-Help }
}
