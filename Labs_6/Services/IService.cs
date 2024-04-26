using Labs_6.DTOs;

namespace Labs_6.Services;

public interface IService
{
    Task<GetProductResponse?> GetProduct(int id);
    Task<GetWarehouseResponse?> GetWarehouse(int id);
    Task<GetOrderResponse?> GetOrder(int idProduct, int amount, DateTime createdAt);
    Task<GetProductWarehouseResponse?> GetProductWarehouse(int idOrder);
    Task<int> CreateProductWarehouse(int idOrder, CreateProductWarehouseRequest createProductWarehouseRequest, decimal price);
}