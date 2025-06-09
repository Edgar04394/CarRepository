
using Microsoft.Data.SqlClient;
using CarApi.Models;

namespace CarApi.Data
{
    public class CarroData
    {
        private readonly string _connectionString;

        public CarroData(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<List<Carro>> GetAll()
        {
            var carros = new List<Carro>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SELECT * FROM Carros", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        carros.Add(new Carro
                        {
                            Id = reader.GetInt32(0),
                            Marca = reader.GetString(1),
                            Modelo = reader.GetString(2),
                            Anio = reader.GetInt32(3),
                            Color = reader.GetString(4),
                            Precio = reader.GetDecimal(5)
                        });
                    }
                }
            }

            return carros;
        }

        public async Task<Carro?> GetById(int id)  // Nota el ? que indica puede retornar null
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand("SELECT * FROM Carros WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Carro
                {
                    Id = reader.GetInt32(0),
                    Marca = reader.GetString(1),
                    Modelo = reader.GetString(2),
                    Anio = reader.GetInt32(3),
                    Color = reader.GetString(4),
                    Precio = reader.GetDecimal(5)
                };
            }

            return null; // Ahora es válido porque el método declara que puede retornar null
        }

        public async Task<int> Create(Carro carro)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "INSERT INTO Carros (Marca, Modelo, Anio, Color, Precio) VALUES (@Marca, @Modelo, @Anio, @Color, @Precio); SELECT SCOPE_IDENTITY();";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Marca", carro.Marca);
                    command.Parameters.AddWithValue("@Modelo", carro.Modelo);
                    command.Parameters.AddWithValue("@Anio", carro.Anio);
                    command.Parameters.AddWithValue("@Color", carro.Color);
                    command.Parameters.AddWithValue("@Precio", carro.Precio);

                    var id = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(id);
                }
            }
        }

        public async Task<bool> Update(Carro carro)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE Carros SET Marca = @Marca, Modelo = @Modelo, Anio = @Anio, Color = @Color, Precio = @Precio WHERE Id = @Id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", carro.Id);
                    command.Parameters.AddWithValue("@Marca", carro.Marca);
                    command.Parameters.AddWithValue("@Modelo", carro.Modelo);
                    command.Parameters.AddWithValue("@Anio", carro.Anio);
                    command.Parameters.AddWithValue("@Color", carro.Color);
                    command.Parameters.AddWithValue("@Precio", carro.Precio);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<bool> Delete(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("DELETE FROM Carros WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
}