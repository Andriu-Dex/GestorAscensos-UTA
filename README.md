# Scripts de Configuración - SGA

Esta carpeta contiene scripts de utilidad para configurar y gestionar el entorno de desarrollo del Sistema de Gestión de Ascensos.

## 📋 Scripts Disponibles

### 🔧 `setup-env.ps1`

**Propósito**: Configura variables de entorno del sistema Windows  
**Uso**: `.\setup-env.ps1`  
**Descripción**: Establece todas las variables de entorno necesarias en el sistema Windows para el usuario actual.

### ✅ `verify-env.ps1`

**Propósito**: Verifica que el archivo .env esté configurado correctamente  
**Uso**: `.\verify-env.ps1`  
**Descripción**: Lee y valida el contenido del archivo .env, mostrando las configuraciones (ocultando datos sensibles).

### 📄 `create-env.ps1`

**Propósito**: Crea archivo .env desde .env.example  
**Uso**: `.\create-env.ps1`  
**Descripción**: Copia el archivo .env.example a .env y proporciona instrucciones detalladas para configurarlo.

## 🚀 Flujo de Trabajo Recomendado

Para nuevos desarrolladores:

```powershell
# 1. Crear archivo .env desde ejemplo
.\scripts\create-env.ps1

# 2. Editar .env con datos reales
code .env

# 3. Verificar configuración
.\scripts\verify-env.ps1

# 4. Ejecutar aplicación
cd SGA.Api
dotnet run
```

## ⚠️ Notas Importantes

- **Ejecutar desde la raíz** del proyecto (donde está .env)
- **PowerShell requerido** para todos los scripts
- **Permisos**: Algunos scripts pueden requerir permisos de administrador
- **SQL Server**: Asegúrate de que esté instalado y ejecutándose

## 🔒 Seguridad

- Los scripts **no exponen** información sensible
- **Protegen** archivos .env existentes
- **Validan** configuraciones antes de proceder
