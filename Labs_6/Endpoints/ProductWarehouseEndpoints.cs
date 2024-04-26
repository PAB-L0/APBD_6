using FluentValidation;
using Labs_6.DTOs;
using Labs_6.Services;

namespace Labs_6.Endpoints;

public static class ProductWarehouseEndpoints
{
    public static void RegisterProductWarehouseEndpoints(this WebApplication webApplication)
    {
        webApplication.MapPost("productWarehouse", AddProductWarehouse);
    }

    private static async Task<IResult> AddProductWarehouse(
        IConfiguration configuration, 
        CreateProductWarehouseRequest createProductWarehouseRequest, 
        IValidator<CreateProductWarehouseRequest> validator, 
        IService service
    )
    {
        var validation = await validator.ValidateAsync(createProductWarehouseRequest);

        if (!validation.IsValid)
        {
            return Results.ValidationProblem(validation.ToDictionary());
        }

        var product = await service.GetProduct(createProductWarehouseRequest.IdProduct);

        if (product == null)
        {
            return Results.NotFound($"Product with id: {createProductWarehouseRequest.IdProduct} does not exist");
        }

        var warehouse = await service.GetWarehouse(createProductWarehouseRequest.IdWarehouse);

        if (warehouse == null)
        {
            return Results.NotFound($"Warehouse with id: {createProductWarehouseRequest.IdWarehouse} does not exist");
        }

        var order = await service.GetOrder(product.IdProduct, createProductWarehouseRequest.Amount, createProductWarehouseRequest.CreatedAt);

        if (order == null)
        {
            return Results.NotFound($"Order with product id: {product.IdProduct}, amount: {createProductWarehouseRequest.Amount} does not exist");
        }

        var productWarehouse = await service.GetProductWarehouse(order.IdOrder);

        if (productWarehouse != null)
        {
            return Results.Ok($"Order with id: {order.IdOrder} has been already realised");
        }

        var result = await service.CreateProductWarehouse(order.IdOrder, createProductWarehouseRequest, product.Price);
        return Results.Created($"productWarehouse/{result}", result);
    }
}