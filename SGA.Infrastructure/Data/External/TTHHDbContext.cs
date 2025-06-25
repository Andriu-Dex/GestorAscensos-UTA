using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities.External;

namespace SGA.Infrastructure.Data.External;

public class TTHHDbContext : DbContext
{
    public TTHHDbContext(DbContextOptions<TTHHDbContext> options) : base(options)
    {
    }

    public DbSet<EmpleadoTTHH> Empleados { get; set; }
    public DbSet<AccionPersonalTTHH> AccionesPersonal { get; set; }
    public DbSet<CargoTTHH> Cargos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EmpleadoTTHH>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Cedula).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Nombres).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Apellidos).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CargoActual).IsRequired().HasMaxLength(255);
            entity.Property(e => e.NivelAcademico).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Cedula);
        });

        modelBuilder.Entity<AccionPersonalTTHH>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Cedula).IsRequired().HasMaxLength(10);
            entity.Property(e => e.TipoAccion).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CargoAnterior).HasMaxLength(255);
            entity.Property(e => e.CargoNuevo).HasMaxLength(255);
            entity.HasIndex(e => e.Cedula);
        });

        modelBuilder.Entity<CargoTTHH>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NombreCargo).IsRequired().HasMaxLength(255);
            entity.Property(e => e.NivelTitular).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
        });

        // Datos semilla para TTHH
        SeedTTHHData(modelBuilder);
    }

    private void SeedTTHHData(ModelBuilder modelBuilder)
    {
        var empleados = new List<EmpleadoTTHH>();
        var acciones = new List<AccionPersonalTTHH>();
        
        // Datos realistas de docentes
        var docentesData = new[]
        {
            new { Nombres = "María Elena", Apellidos = "Rodríguez", Cedula = "1800000001" },
            new { Nombres = "Carlos Alberto", Apellidos = "Morales", Cedula = "1800000002" },
            new { Nombres = "Ana Cristina", Apellidos = "Vásquez", Cedula = "1800000003" },
            new { Nombres = "Luis Fernando", Apellidos = "Herrera", Cedula = "1800000004" },
            new { Nombres = "Patricia Isabel", Apellidos = "Gómez", Cedula = "1800000005" },
            new { Nombres = "Jorge Eduardo", Apellidos = "Pérez", Cedula = "1800000006" },
            new { Nombres = "Rosa Angélica", Apellidos = "Torres", Cedula = "1800000007" },
            new { Nombres = "Miguel Ángel", Apellidos = "Castillo", Cedula = "1800000008" },
            new { Nombres = "Lucía Mercedes", Apellidos = "Jiménez", Cedula = "1800000009" },
            new { Nombres = "Francisco Javier", Apellidos = "Ramírez", Cedula = "1800000010" },
            new { Nombres = "Carmen Dolores", Apellidos = "Vargas", Cedula = "1800000011" },
            new { Nombres = "Roberto Carlos", Apellidos = "Mendoza", Cedula = "1800000012" },
            new { Nombres = "Gloria Patricia", Apellidos = "Sánchez", Cedula = "1800000013" },
            new { Nombres = "Andrés Felipe", Apellidos = "Guerrero", Cedula = "1800000014" },
            new { Nombres = "Beatriz Elena", Apellidos = "Ortega", Cedula = "1800000015" },
            new { Nombres = "Daniel Alejandro", Apellidos = "Silva", Cedula = "1800000016" },
            new { Nombres = "Mónica Teresa", Apellidos = "Ramos", Cedula = "1800000017" },
            new { Nombres = "Héctor Manuel", Apellidos = "Paredes", Cedula = "1800000018" },
            new { Nombres = "Esperanza del Carmen", Apellidos = "Lozano", Cedula = "1800000019" },
            new { Nombres = "Raúl Enrique", Apellidos = "Aguilar", Cedula = "1800000020" },
            new { Nombres = "Silvia Marcela", Apellidos = "Navarro", Cedula = "1800000021" },
            new { Nombres = "Pablo Antonio", Apellidos = "Cevallos", Cedula = "1800000022" },
            new { Nombres = "Verónica Alejandra", Apellidos = "Vallejo", Cedula = "1800000023" },
            new { Nombres = "Germán Patricio", Apellidos = "Salazar", Cedula = "1800000024" },
            new { Nombres = "Gabriela Fernanda", Apellidos = "Espinoza", Cedula = "1800000025" },
            new { Nombres = "Juan Carlos", Apellidos = "Maldonado", Cedula = "1800000026" },
            new { Nombres = "Sandra Milena", Apellidos = "Figueroa", Cedula = "1800000027" },
            new { Nombres = "Fernando José", Apellidos = "Cordero", Cedula = "1800000028" },
            new { Nombres = "Martha Lucía", Apellidos = "Villacís", Cedula = "1800000029" },
            new { Nombres = "Rodrigo Esteban", Apellidos = "Mantilla", Cedula = "1800000030" },
            new { Nombres = "Diana Carolina", Apellidos = "Zambrano", Cedula = "1800000031" },
            new { Nombres = "Iván Patricio", Apellidos = "Balarezo", Cedula = "1800000032" },
            new { Nombres = "Lilian Esperanza", Apellidos = "Freire", Cedula = "1800000033" },
            new { Nombres = "Nelson Rodrigo", Apellidos = "Santamaría", Cedula = "1800000034" },
            new { Nombres = "Alejandra Paola", Apellidos = "Montoya", Cedula = "1800000035" },
            new { Nombres = "Mauricio Xavier", Apellidos = "Villalba", Cedula = "1800000036" },
            new { Nombres = "Elena Guadalupe", Apellidos = "Ruiz", Cedula = "1800000037" },
            new { Nombres = "Jaime Ricardo", Apellidos = "Carrasco", Cedula = "1800000038" },
            new { Nombres = "Ximena del Rocío", Apellidos = "Albán", Cedula = "1800000039" },
            new { Nombres = "Oscar Vinicio", Apellidos = "Proaño", Cedula = "1800000040" },
            new { Nombres = "Pilar Soledad", Apellidos = "Tapia", Cedula = "1800000041" },
            new { Nombres = "Edison Fabián", Apellidos = "Velasco", Cedula = "1800000042" },
            new { Nombres = "Margarita Isabel", Apellidos = "Cáceres", Cedula = "1800000043" },
            new { Nombres = "Alberto Raúl", Apellidos = "Sandoval", Cedula = "1800000044" },
            new { Nombres = "Rocío del Pilar", Apellidos = "Carrión", Cedula = "1800000045" },
            new { Nombres = "Wilmer Patricio", Apellidos = "Mayorga", Cedula = "1800000046" },
            new { Nombres = "Nelly Esperanza", Apellidos = "Bermúdez", Cedula = "1800000047" },
            new { Nombres = "Víctor Hugo", Apellidos = "Solórzano", Cedula = "1800000048" },
            new { Nombres = "Blanca Estela", Apellidos = "Arévalo", Cedula = "1800000049" },
            new { Nombres = "Marco Vinicio", Apellidos = "Tixe", Cedula = "1800000050" }
        };
        
        var nivelesAcademicos = new[] { "PhD", "Magíster", "Licenciado", "Ingeniero" };
        var cargosTitulares = new[] { "Docente Titular 1", "Docente Titular 2", "Docente Titular 3", "Docente Titular 4", "Docente Titular 5" };
        
        for (int i = 0; i < docentesData.Length; i++)
        {
            var docente = docentesData[i];
            var fechaNombramiento = DateTime.UtcNow.AddYears(-Random.Shared.Next(2, 12));
            var primeraInicial = docente.Nombres.Split(' ')[0][0].ToString().ToLower();
            var apellidoSinEspacios = docente.Apellidos.Split(' ')[0].ToLower();
            var email = $"{primeraInicial}{apellidoSinEspacios}@uta.edu.ec";
            
            empleados.Add(new EmpleadoTTHH
            {
                Id = i + 1,
                Cedula = docente.Cedula,
                Nombres = docente.Nombres,
                Apellidos = docente.Apellidos,
                Email = email,
                FechaNombramiento = fechaNombramiento,
                CargoActual = cargosTitulares[Random.Shared.Next(cargosTitulares.Length)],
                NivelAcademico = nivelesAcademicos[Random.Shared.Next(nivelesAcademicos.Length)],
                EstaActivo = true
            });
        }

        modelBuilder.Entity<EmpleadoTTHH>().HasData(empleados);
        modelBuilder.Entity<AccionPersonalTTHH>().HasData(acciones);
        
        modelBuilder.Entity<CargoTTHH>().HasData(
            new CargoTTHH { Id = 1, NombreCargo = "Docente Titular 1", NivelTitular = "Titular1", Descripcion = "Nivel inicial", EstaActivo = true },
            new CargoTTHH { Id = 2, NombreCargo = "Docente Titular 2", NivelTitular = "Titular2", Descripcion = "Segundo nivel", EstaActivo = true },
            new CargoTTHH { Id = 3, NombreCargo = "Docente Titular 3", NivelTitular = "Titular3", Descripcion = "Tercer nivel", EstaActivo = true },
            new CargoTTHH { Id = 4, NombreCargo = "Docente Titular 4", NivelTitular = "Titular4", Descripcion = "Cuarto nivel", EstaActivo = true },
            new CargoTTHH { Id = 5, NombreCargo = "Docente Titular 5", NivelTitular = "Titular5", Descripcion = "Nivel máximo", EstaActivo = true }
        );
    }
}
