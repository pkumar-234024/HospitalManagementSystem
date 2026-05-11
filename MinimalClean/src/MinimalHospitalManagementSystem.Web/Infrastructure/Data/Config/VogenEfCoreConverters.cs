using MinimalHospitalManagementSystem.Web.Domain.CartAggregate;
using MinimalHospitalManagementSystem.Web.Domain.GuestUserAggregate;
using MinimalHospitalManagementSystem.Web.Domain.OrderAggregate;
using MinimalHospitalManagementSystem.Web.Domain.ProductAggregate;
using Vogen;

namespace MinimalHospitalManagementSystem.Web.Infrastructure.Data.Config;

[EfCoreConverter<ProductId>]
[EfCoreConverter<CartId>]
[EfCoreConverter<CartItemId>]
[EfCoreConverter<GuestUserId>]
[EfCoreConverter<OrderId>]
[EfCoreConverter<OrderItemId>]
[EfCoreConverter<Quantity>]
[EfCoreConverter<Price>]
internal partial class VogenEfCoreConverters;
