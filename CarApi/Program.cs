using CarApi.Data;
using CarApi.Models;
using Microsoft.Data.SqlClient;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar CarroData como servicio
builder.Services.AddScoped<CarroData>(provider => 
    new CarroData(provider.GetRequiredService<IConfiguration>()));


// Habilitar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Crear la tabla si no existe
InitializeDatabase(app.Services);

app.Run();

void InitializeDatabase(IServiceProvider serviceProvider)
{
    try
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string not found");

        Console.WriteLine($"Attempting to connect to SQL Server...");

        // Primero probar conexión básica al servidor
        TestConnection(connectionString);

        // Luego verificar/crear la base de datos
        EnsureDatabaseExists(connectionString);

        // Finalmente verificar/crear la tabla
        EnsureTableExists(connectionString);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error initializing database: {ex.Message}");
        if (ex is SqlException sqlEx)
        {
            Console.WriteLine($"SQL Error Number: {sqlEx.Number}");
            Console.WriteLine($"SQL State: {sqlEx.State}");
            
            // Manejo específico para error de login
            if (sqlEx.Number == 18456)
            {
                Console.WriteLine("ERROR DE AUTENTICACIÓN:");
                Console.WriteLine("1. Verifica que el usuario y contraseña sean correctos");
                Console.WriteLine("2. Verifica que el usuario exista en SQL Server");
                Console.WriteLine("3. Verifica que la contraseña no tenga caracteres especiales problemáticos");
            }
        }
        throw;
    }
}

void TestConnection(string connectionString)
{
    using var connection = new SqlConnection(connectionString);
    try
    {
        connection.Open();
        Console.WriteLine("Conexión al servidor SQL exitosa!");
    }
    catch (SqlException ex) when (ex.Number == 18456)
    {
        Console.WriteLine($"Error de autenticación (18456). Verifica:");
        Console.WriteLine($"- Usuario y contraseña en appsettings.json");
        Console.WriteLine($"- Que el usuario tenga acceso al servidor");
        throw;
    }
}

void EnsureDatabaseExists(string connectionString)
{
    var builder = new SqlConnectionStringBuilder(connectionString);
    var databaseName = builder.InitialCatalog;
    
    // Conexión al servidor (master) sin especificar base de datos
    builder.InitialCatalog = "master";
    var masterConnectionString = builder.ToString();

    using var connection = new SqlConnection(masterConnectionString);
    connection.Open();

    // Verificar si la base de datos existe
    using (var command = new SqlCommand(
        $"SELECT database_id FROM sys.databases WHERE name = '{databaseName}'", 
        connection))
    {
        var exists = command.ExecuteScalar() != null;

        if (!exists)
        {
            Console.WriteLine($"Creando base de datos {databaseName}...");
            using (var createCommand = new SqlCommand(
                $"CREATE DATABASE {databaseName}", 
                connection))
            {
                createCommand.ExecuteNonQuery();
                Console.WriteLine($"Base de datos {databaseName} creada exitosamente");
            }
        }
        else
        {
            Console.WriteLine($"Base de datos {databaseName} ya existe");
        }
    }
}

void EnsureTableExists(string connectionString)
{
    using var connection = new SqlConnection(connectionString);
    connection.Open();

    Console.WriteLine("Verificando existencia de tabla Carros...");
    
    using var command = new SqlCommand(@"
        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Carros')
        BEGIN
            CREATE TABLE Carros (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                Marca NVARCHAR(100) NOT NULL,
                Modelo NVARCHAR(100) NOT NULL,
                Anio INT NOT NULL,
                Color NVARCHAR(50) NOT NULL,
                Precio DECIMAL(18,2) NOT NULL
            )
            PRINT 'Tabla Carros creada exitosamente'
        END", connection);
    
    command.ExecuteNonQuery();
    Console.WriteLine("Tabla Carros verificada/creada");
}