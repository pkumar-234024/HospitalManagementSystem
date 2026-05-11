using MinimalHospitalManagementSystem.Web.Domain.ProductAggregate;

namespace MinimalHospitalManagementSystem.Web.ProductFeatures;
public record ProductDto(ProductId Id, string Name, decimal UnitPrice);
