# **Sistema de Gestión y Control de Ascensos para Profesores**

## **1. Objetivo General**

Desarrollar un sistema web para la gestión y control de ascensos de los docentes de la Universidad Técnica de Ambato, permitiendo validar requisitos, administrar documentación, procesar solicitudes y generar reportes de manera segura, eficiente y centralizada. El sistema estará disponible únicamente para dos tipos de usuarios: **docentes** y **administradores**.

---

## **3. Arquitectura y Tecnologías**

- **Arquitectura:** Onion Architecture (Frontend, Aplicación, Dominio, Infraestructura)
- **Frontend:** Blazor WebAssembly
- **Backend:** ASP.NET Core 9 Web API
- **Base de Datos:** SQL Server Express
- **Formato de Comunicación:** JSON vía API RESTful
- **Control de versiones:** Git con estrategia Gitflow
- **CI/CD y despliegue:** Se elegirá la opción gratuita y eficiente más adecuada
- **Seguridad:**

  - Autenticación con tokens JWT
  - Validación de inputs
  - Protección contra ataques (XSS, CSRF)
  - Cifrado en tránsito (TLS/SSL)
  - Registro de logs y auditoría
  - Bloqueo temporal tras 3 intentos fallidos

---

## **4. Reglas de Ascenso Docente**

Los ascensos son secuenciales entre niveles **Titular 1** a **Titular 5**, cumpliendo los siguientes requisitos:

| Ascenso       | Tiempo en rol actual | Obras mínimas | Puntaje evaluación docente | Horas de capacitación | Tiempo en investigación |
| ------------- | -------------------- | ------------- | -------------------------- | --------------------- | ----------------------- |
| Titular 1 → 2 | 4 años               | 1             | 75%                        | 96 horas              | No requerido            |
| Titular 2 → 3 | 4 años               | 2             | 75%                        | 96 horas              | 12 meses                |
| Titular 3 → 4 | 4 años               | 3             | 75%                        | 128 horas             | 24 meses                |
| Titular 4 → 5 | 4 años               | 5             | 75%                        | 160 horas             | 24 meses                |

**Importante:** al ascender, los contadores de obras, horas y proyectos se reinician.

---

## **5. Funcionalidades del Sistema**

### **5.1 Acceso y Autenticación**

- Registro e inicio de sesión para docentes
- Validación de credenciales
- Bloqueo temporal tras 3 intentos fallidos

### **5.2 Gestión del Perfil**

- Visualización de datos personales y hoja de vida
- Edición de información relevante
- Acceso únicamente al propio perfil en el caso del docente

### **5.3 Importación de Indicadores**

- Sección con indicadores clave:

  - Tiempo en rol actual
  - Número de obras
  - Puntaje evaluación
  - Horas capacitación
  - Tiempo en proyectos

- Botones “Importar” conectan con servicios externos mediante la cédula
- Las conexiones serán con:

  - DITIC (cursos)
  - DAC (evaluaciones)
  - TTHH (acción de personal)
  - Dirección de Investigación

- El sistema es **cliente REST**, espera recibir JSON. No valida la veracidad del dato recibido.

### **5.4 Validación de Requisitos**

- Botón “Solicitar Ascenso”
- Validación automática de requisitos según nivel actual
- Indicadores mostrarán estado (cumple / no cumple)

### **5.5 Gestión Documental**

- Subida de archivos PDF relacionados a obras, cursos, proyectos, etc.
- Posibilidad de agregar y editar documentos
- Almacenamiento comprimido en base de datos (sin límite, pero con validaciones de tamaño y formato)
- Validación de integridad y estructura del archivo

### **5.6 Solicitudes de Ascenso**

- Flujo para que el docente solicite el ascenso
- El administrador podrá:

  - Aprobar
  - Rechazar (con motivo)
  - Marcar como "En proceso"

- Las solicitudes rechazadas se **archivarán con estado “rechazada”**

### **5.7 Reportes**

- Reporte en PDF con el estado actual del docente y su proceso
- Acceso:

  - Docente: solo su propio reporte
  - Administrador: reportes de todos

---

## **6. Consideraciones Técnicas**

- Manejo robusto de errores
- Transacciones atómicas con TransactionScope para evitar inconsistencias
- Logs centralizados para errores y auditorías
- Pruebas unitarias obligatorias
- Seguridad de los archivos PDF (acceso restringido y cifrado)
- No habrá accesibilidad extendida ni API pública por el momento
- El sistema debe ser escalable a futuro (otros roles, otros tipos de personal)

---
