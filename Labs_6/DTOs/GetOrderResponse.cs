namespace Labs_6.DTOs;

public record GetOrderResponse(int IdOrder, int IdProduct, int Amount, DateTime CreatedAt, DateTime? FulfilledAt);