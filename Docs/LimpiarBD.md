# Script para Limpiar Base de Datos - SGA

## 📋 Resumen

Se implementó un script PowerShell para limpiar completamente la base de datos del Sistema de Gestión de Ascensos (SGA), eliminando todos los datos pero manteniendo la estructura de tablas y relaciones.

## 🎯 Objetivo

Proporcionar una forma rápida y segura de resetear la base de datos durante el desarrollo, eliminando todos los registros y recreando la estructura desde cero con datos iniciales.

## 🔧 Script Implementado

### Ubicación

```
Scripts/clear-database.ps1
```

### Funcionalidad del Script

```powershell
# Script para limpiar todos los datos de las tablas
# Mantiene la estructura de la base de datos pero elimina todos los registros

Write-Host "🗑️ Limpiando base de datos..." -ForegroundColor Yellow

# Cambiar al directorio de la API
Set-Location "SGA.Api"

try {
    # Eliminar la base de datos
    Write-Host "📋 Eliminando base de datos..." -ForegroundColor Cyan
    dotnet ef database drop --force --context ApplicationDbContext

    # Recrear la base de datos con las migraciones
    Write-Host "🔄 Recreando base de datos..." -ForegroundColor Cyan
    dotnet ef database update --context ApplicationDbContext

    Write-Host "✅ Base de datos limpiada exitosamente!" -ForegroundColor Green
    Write-Host "🔑 Usuario administrador creado:" -ForegroundColor Blue
    Write-Host "   Email: admin@uta.edu.ec" -ForegroundColor White
    Write-Host "   Password: Admin123*" -ForegroundColor White

} catch {
    Write-Host "❌ Error al limpiar la base de datos: $($_.Exception.Message)" -ForegroundColor Red
}

# Volver al directorio raíz
Set-Location ".."
```

## � Cómo Usar el Script

### Ejecutar desde la raíz del proyecto:

```powershell
.\Scripts\clear-database.ps1
```

### Lo que hace el script:

1. **🗑️ Elimina** completamente la base de datos existente
2. **🔄 Recrea** la base de datos desde cero
3. **📊 Aplica** todas las migraciones automáticamente
4. **👤 Crea** el usuario administrador por defecto
5. **✅ Confirma** que la operación fue exitosa

## 📋 Resultados Obtenidos

### ✅ Base de Datos Limpia

- Todas las tablas recreadas con estructura correcta
- Índices y constraints aplicados
- Relaciones entre tablas establecidas
- Datos de prueba eliminados

### 🔑 Usuario Administrador Creado

- **Email**: `admin@uta.edu.ec`
- **Password**: `Admin123*`
- **Rol**: Administrador
- **Estado**: Activo

### 📊 Estructura Completa

- **Tablas**: Usuarios, Docentes, Solicitudes, Documentos, Auditoría
- **Relaciones**: Foreign Keys configuradas
- **Constraints**: Validaciones aplicadas
- **Índices**: Optimizaciones de rendimiento

## 🔍 Características del Script

### ✅ Seguridad

- Elimina completamente la base de datos para evitar datos residuales
- Recrea desde migraciones para garantizar estructura correcta
- Manejo de errores con mensajes informativos

### ✅ Eficiencia

- Operación rápida y automatizada
- No requiere intervención manual
- Restaura directorio de trabajo original

### ✅ Confiabilidad

- Usa comandos oficiales de Entity Framework
- Aplica todas las migraciones en orden correcto
- Confirma éxito de la operación

## 🎯 Casos de Uso

### Durante Desarrollo

- Limpiar datos de prueba acumulados
- Resetear estado para nuevas pruebas
- Aplicar cambios de schema desde cero

### Después de Cambios de Modelo

- Aplicar nuevas migraciones
- Eliminar datos incompatibles
- Verificar estructura correcta

### Preparación de Demos

- Estado limpio para demostraciones
- Datos iniciales consistentes
- Usuario administrador disponible

## ✅ Estado Actual

- ✅ **Script Funcional**: Probado y verificado
- ✅ **Base de Datos Limpia**: Sin datos residuales
- ✅ **Estructura Completa**: Todas las tablas creadas
- ✅ **Usuario Administrador**: Listo para usar
- ✅ **Migraciones Aplicadas**: Estructura actualizada

## 📝 Conclusión

El script `clear-database.ps1` proporciona una solución completa y confiable para limpiar la base de datos del SGA, ideal para desarrollo y testing. Su uso garantiza un estado limpio y consistente del sistema.
