using MinimalHospitalManagementSystem.Web.Domain.CartAggregate;

namespace MinimalHospitalManagementSystem.Web.CartFeatures;

public record CartDto(CartId Id, IReadOnlyList<CartItemDto> Items, decimal Total);

public record CartItemDto(int ProductId, int Quantity, decimal UnitPrice, decimal TotalPrice);
