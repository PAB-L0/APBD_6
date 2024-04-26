namespace Labs_6.DTOs;

public record CreateProductWarehouseRequest(int IdProduct, int IdWarehouse, int Amount, DateTime CreatedAt);