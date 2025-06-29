# Guía Rápida - Variables de Entorno SGA

## 🚀 Para Nuevos Desarrolladores

### 1. Crear tu archivo .env

```bash
.\scripts\create-env.ps1
```

### 2. Editar con tus datos

```bash
code .env
# Reemplaza:
# - TU_SERVIDOR → .\SQLEXPRESS (o tu servidor)
# - TU_CLAVE_SECRETA_JWT... → Una clave segura de 64+ caracteres
```

### 3. Verificar configuración

```bash
.\scripts\verify-env.ps1
```

### 4. Ejecutar aplicación

```bash
cd SGA.Api
dotnet run
```

## ⚠️ IMPORTANTE

- ❌ **NUNCA** commitees el archivo `.env`
- ✅ **SOLO** commitea `.env.example`
- 🔐 **USA** claves JWT únicas y seguras

## 📋 Variables Necesarias

```bash
SGA_DB_CONNECTION=Server=.\SQLEXPRESS;Database=SGA_Main;...
SGA_JWT_SECRET_KEY=tu_clave_secreta_muy_larga...
SGA_CORS_ORIGINS=https://localhost:7149,http://localhost:5039
# + otras 4 conexiones de BD
```

## 🆘 Problemas Comunes

- **Error JWT**: Verifica que `SGA_JWT_SECRET_KEY` tenga 64+ caracteres
- **Error BD**: Verifica que SQL Server esté ejecutándose
- **Variables no detectadas**: Reinicia VS Code/IDE

## 📚 Documentación Completa

Ver: `README_VARIABLES_ENTORNO.md`
