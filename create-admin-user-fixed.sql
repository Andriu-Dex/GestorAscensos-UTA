USE [SGA_Main]
GO

-- Crear usuario administrador
INSERT INTO [dbo].[Usuarios]
           ([Id]
           ,[Email]
           ,[PasswordHash]
           ,[Rol]
           ,[EstaActivo]
           ,[IntentosLogin]
           ,[UltimoBloqueado]
           ,[UltimoLogin]
           ,[FechaCreacion]
           ,[FechaModificacion])
     VALUES
           (NEWID()
           ,'admin@uta.edu.ec'
           ,'$2a$11$Vhn38Yy/d9yUZAqJB1jmMesFL4J4HJ0e/pkF5NvXPrBogB/X2Vrmq'
           ,2  -- Administrador = 2
           ,1
           ,0
           ,NULL
           ,GETDATE()  -- UltimoLogin debe ser una fecha, no NULL
           ,GETDATE()
           ,GETDATE())
GO

-- Crear docente asociado al usuario administrador
INSERT INTO [dbo].[Docentes]
           ([Id]
           ,[UsuarioId]
           ,[Cedula]
           ,[Nombres]
           ,[Apellidos]
           ,[Email]
           ,[NivelActual]
           ,[FechaInicioNivelActual]
           ,[EstaActivo]
           ,[FechaCreacion]
           ,[FechaModificacion])
     VALUES
           (NEWID()
           ,(SELECT Id FROM [dbo].[Usuarios] WHERE Email = 'admin@uta.edu.ec')
           ,'999999999'
           ,'Admin'
           ,'Global'
           ,'admin@uta.edu.ec'
           ,5  -- Titular5 = 5
           ,GETDATE()
           ,1
           ,GETDATE()
           ,GETDATE())
GO

PRINT 'Usuario administrador creado exitosamente!'
PRINT 'Email: admin@uta.edu.ec'
PRINT 'Contraseña: Admin12345'
PRINT 'Cédula: 999999999'
