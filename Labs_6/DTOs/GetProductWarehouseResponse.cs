namespace Labs_6.DTOs;

public record GetProductWarehouseResponse(int IdProductWarehouse, int IdWarehouse, int IdProduct, int IdOrder, int Amount, decimal Price, DateTime CreatedAt);