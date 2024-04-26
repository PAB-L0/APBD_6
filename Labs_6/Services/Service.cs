using System.Data.SqlClient;
using Labs_6.DTOs;

namespace Labs_6.Services;

public class Service(IConfiguration configuration) : IService
{
    private async Task<SqlConnection> GetConnection()
    {
        var connection = new SqlConnection(configuration.GetConnectionString("Default"));
        await connection.OpenAsync();
        return connection;
    }

    public async Task<GetProductResponse?> GetProduct(int id)
    {
        await using var connection = await GetConnection();
        
        var command = new SqlCommand("SELECT * FROM Product WHERE IdProduct = @idProduct", connection);
        command.Parameters.AddWithValue("@idProduct", id);

        var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows)
        {
            return null;
        }

        await reader.ReadAsync();
        return new GetProductResponse(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetDecimal(3)
        );
    }

    public async Task<GetWarehouseResponse?> GetWarehouse(int id)
    {
        await using var connection = await GetConnection();

        var command = new SqlCommand("SELECT * FROM Warehouse WHERE IdWarehouse = @idWarehouse", connection);
        command.Parameters.AddWithValue("@idWarehouse", id);

        var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows)
        {
            return null;
        }

        await reader.ReadAsync();
        return new GetWarehouseResponse(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetString(2)
        );
    }

    public async Task<GetOrderResponse?> GetOrder(int idProduct, int amount, DateTime createdAt)
    {
        await using var connection = await GetConnection();

        var command = new SqlCommand("SELECT * FROM [Order] WHERE IdProduct = @idProduct AND Amount = @amount AND CreatedAt < @createdAt", connection);
        command.Parameters.AddWithValue("@idProduct", idProduct);
        command.Parameters.AddWithValue("@amount", amount);
        command.Parameters.AddWithValue("@createdAt", createdAt);

        var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows)
        {
            return null;
        }

        await reader.ReadAsync();
        return new GetOrderResponse(
            reader.GetInt32(0),
            reader.GetInt32(1),
            reader.GetInt32(2),
            reader.GetDateTime(3),
            await reader.IsDBNullAsync(4) ? null : reader.GetDateTime(4)
        );
    }

    public async Task<GetProductWarehouseResponse?> GetProductWarehouse(int idOrder)
    {
        await using var connection = await GetConnection();

        var command = new SqlCommand("SELECT * FROM Product_Warehouse WHERE IdOrder = @idOrder", connection);
        command.Parameters.AddWithValue("@idOrder", idOrder);

        var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows)
        {
            return null;
        }

        await reader.ReadAsync();
        return new GetProductWarehouseResponse(
            reader.GetInt32(0),
            reader.GetInt32(1),
            reader.GetInt32(2),
            reader.GetInt32(3),
            reader.GetInt32(4),
            reader.GetDecimal(5),
            reader.GetDateTime(6)
        );
    }

    public async Task<int> CreateProductWarehouse(int idOrder, CreateProductWarehouseRequest createProductWarehouseRequest, decimal price)
    {
        await using var connection = await GetConnection();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var command1 = new SqlCommand("UPDATE [Order] SET FulfilledAt = @now WHERE IdOrder = @idOrder", connection, (SqlTransaction)transaction);
            command1.Parameters.AddWithValue("@now", DateTime.Now);
            command1.Parameters.AddWithValue("@idOrder", idOrder);

            await command1.ExecuteNonQueryAsync();

            var command2 = new SqlCommand("INSERT INTO Product_Warehouse VALUES (@idWarehouse, @idProduct, @idOrder, @amount, @price, @createdAt); SELECT CAST(SCOPE_IDENTITY() AS INT)", connection, (SqlTransaction)transaction);
            command2.Parameters.AddWithValue("@idWarehouse", createProductWarehouseRequest.IdWarehouse);
            command2.Parameters.AddWithValue("@idProduct", createProductWarehouseRequest.IdProduct);
            command2.Parameters.AddWithValue("@idOrder", idOrder);
            command2.Parameters.AddWithValue("@amount", createProductWarehouseRequest.Amount);
            command2.Parameters.AddWithValue("@price", createProductWarehouseRequest.Amount * price);
            command2.Parameters.AddWithValue("@createdAt", DateTime.Now);

            var result = (int)(await command2.ExecuteScalarAsync())!;

            await transaction.CommitAsync();

            return result;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}