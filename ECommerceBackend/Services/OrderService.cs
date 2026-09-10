using ECommerceBackend.DTOs.Order;
using ECommerceBackend.Models;
using ECommerceBackend.Repositories.Interfaces;
using ECommerceBackend.Services.Interfaces;

namespace ECommerceBackend.Services;

public class OrderService : IOrderService
{
private readonly IOrderRepository _orderRepository;
private readonly ICartRepository _cartRepository;
private readonly IFakePaymentService _fakePaymentService;

public OrderService(
    IOrderRepository orderRepository,
    ICartRepository cartRepository,
    IFakePaymentService fakePaymentService)
{
    _orderRepository = orderRepository;
    _cartRepository = cartRepository;
    _fakePaymentService = fakePaymentService;
}


// ======================================================
// CREATE ORDER
// ======================================================

public async Task<OrderCreateResponseDto> CreateOrderAsync(
    int userId,
    OrderCreateDto dto)
{
    var cart =
        await _cartRepository.GetCartByUserIdAsync(
            userId
        );

    if (cart == null ||
        cart.CartItems == null ||
        cart.CartItems.Count == 0)
    {
        throw new InvalidOperationException(
            "Cart is empty."
        );
    }

    if (string.IsNullOrWhiteSpace(
            dto.ShippingAddress))
    {
        throw new InvalidOperationException(
            "Shipping address is required."
        );
    }


    // ==================================================
    // Validate cart and calculate total
    // ==================================================

    decimal totalAmount = 0;

    foreach (var cartItem in cart.CartItems)
    {
        var product =
            cartItem.Product;

        if (product == null)
        {
            throw new InvalidOperationException(
                "Product not found."
            );
        }

        if (cartItem.Quantity <= 0)
        {
            throw new InvalidOperationException(
                "Invalid product quantity."
            );
        }

        if (product.StockQuantity <
            cartItem.Quantity)
        {
            throw new InvalidOperationException(
                $"Not enough stock for product: {product.Name}"
            );
        }

        totalAmount +=
            product.Price *
            cartItem.Quantity;
    }


    // ==================================================
    // Create Pending Order
    // ==================================================

    var order = new Order
    {
        UserId =
            userId,

        TotalAmount =
            totalAmount,

        Status =
            "Pending",

        ShippingAddress =
            dto.ShippingAddress.Trim(),

        CreatedAt =
            DateTime.UtcNow
    };


    // ==================================================
    // Create Order Items
    // ==================================================

    foreach (var cartItem in cart.CartItems)
    {
        var product =
            cartItem.Product!;

        var orderItem = new OrderItem
        {
            ProductId =
                product.Id,

            Quantity =
                cartItem.Quantity,

            Price =
                product.Price
        };

        order.OrderItems.Add(
            orderItem
        );
    }


    // ==================================================
    // Save Order
    // ==================================================

    var createdOrder =
        await _orderRepository.CreateAsync(
            order
        );


    // ==================================================
    // Create Fake Payment Order
    // ==================================================

    var paymentOrderId =
        _fakePaymentService.CreatePaymentOrder(
            totalAmount,
            createdOrder.Id
        );


    // ==================================================
    // Save Payment Order ID
    // ==================================================

    createdOrder.PaymentOrderId =
        paymentOrderId;

    await _orderRepository.UpdateAsync(
        createdOrder
    );


    // ==================================================
    // Return Payment Information
    // ==================================================

    return new OrderCreateResponseDto
    {
        OrderId =
            createdOrder.Id,

        Amount =
            totalAmount,

        Currency =
            "INR",

        PaymentOrderId =
            paymentOrderId
    };
}


// ======================================================
// VERIFY PAYMENT
// ======================================================

public async Task<bool> VerifyPaymentAsync(
    int userId,
    FakePaymentDto dto)
{
    // ==================================================
    // Get order belonging to current user
    // ==================================================

    var order =
        await _orderRepository.GetByIdAsync(
            dto.OrderId,
            userId
        );

    if (order == null)
    {
        throw new InvalidOperationException(
            "Order not found."
        );
    }


    // ==================================================
    // Already Paid
    // ==================================================

    if (order.Status == "Paid")
    {
        return true;
    }


    // ==================================================
    // Validate Payment Order ID
    // ==================================================

    if (string.IsNullOrWhiteSpace(
            order.PaymentOrderId))
    {
        throw new InvalidOperationException(
            "Payment order ID is missing."
        );
    }


    if (!string.Equals(
            order.PaymentOrderId,
            dto.PaymentOrderId,
            StringComparison.Ordinal))
    {
        throw new InvalidOperationException(
            "Payment order ID does not match."
        );
    }


    // ==================================================
    // Verify Fake Payment
    // ==================================================

    var paymentValid =
        _fakePaymentService.VerifyPayment(
            dto.PaymentOrderId,
            dto.OrderId
        );

    if (!paymentValid)
    {
        throw new InvalidOperationException(
            "Payment verification failed."
        );
    }


    // ==================================================
    // Get Current Cart
    // ==================================================

    var cart =
        await _cartRepository.GetCartByUserIdAsync(
            userId
        );

    if (cart == null ||
        cart.CartItems == null ||
        cart.CartItems.Count == 0)
    {
        throw new InvalidOperationException(
            "Cart is empty."
        );
    }


    // ==================================================
    // Validate Stock Again
    // ==================================================

    foreach (var cartItem in cart.CartItems)
    {
        var product =
            cartItem.Product;

        if (product == null)
        {
            throw new InvalidOperationException(
                "Product not found."
            );
        }

        if (product.StockQuantity <
            cartItem.Quantity)
        {
            throw new InvalidOperationException(
                $"Not enough stock for product: {product.Name}"
            );
        }
    }


    // ==================================================
    // Reduce Stock
    // ==================================================

    foreach (var cartItem in cart.CartItems)
    {
        var product =
            cartItem.Product!;

        product.StockQuantity -=
            cartItem.Quantity;
    }


    // ==================================================
    // Save Payment ID
    // ==================================================

    order.PaymentId =
        dto.PaymentOrderId;


    // ==================================================
    // Mark Order Paid
    // ==================================================

    order.Status =
        "Paid";


    // ==================================================
    // Update Order
    // ==================================================

    await _orderRepository.UpdateAsync(
        order
    );


    // ==================================================
    // Clear Cart
    // ==================================================

    await _cartRepository.ClearCartAsync(
        cart
    );

    return true;
}


// ======================================================
// GET MY ORDERS
// ======================================================

public async Task<List<OrderResponseDto>> GetMyOrdersAsync(
    int userId)
{
    var orders =
        await _orderRepository.GetByUserIdAsync(
            userId
        );

    return orders
        .Select(MapToDto)
        .ToList();
}


// ======================================================
// GET MY ORDER BY ID
// ======================================================

public async Task<OrderResponseDto?> GetMyOrderByIdAsync(
    int userId,
    int orderId)
{
    var order =
        await _orderRepository.GetByIdAsync(
            orderId,
            userId
        );

    if (order == null)
    {
        return null;
    }

    return MapToDto(
        order
    );
}


// ======================================================
// GET ALL ORDERS - ADMIN
// ======================================================

public async Task<List<OrderResponseDto>> GetAllOrdersAsync()
{
    var orders =
        await _orderRepository.GetAllAsync();

    return orders
        .Select(MapToDto)
        .ToList();
}


// ======================================================
// GET ORDER BY ID - ADMIN
// ======================================================

public async Task<OrderResponseDto?> GetOrderByIdForAdminAsync(
    int orderId)
{
    var order =
        await _orderRepository.GetByIdForAdminAsync(
            orderId
        );

    if (order == null)
    {
        return null;
    }

    return MapToDto(
        order
    );
}


// ======================================================
// MAP ORDER → DTO
// ======================================================

private OrderResponseDto MapToDto(
    Order order)
{
    return new OrderResponseDto
    {
        Id =
            order.Id,

        UserId =
            order.UserId,

        TotalAmount =
            order.TotalAmount,

        Status =
            order.Status,

        ShippingAddress =
            order.ShippingAddress,

        CreatedAt =
            order.CreatedAt,

        Items =
            order.OrderItems
                .Select(item =>
                    new OrderItemResponseDto
                    {
                        Id =
                            item.Id,

                        ProductId =
                            item.ProductId,

                        ProductName =
                            item.Product?.Name
                            ?? string.Empty,

                        Quantity =
                            item.Quantity,

                        Price =
                            item.Price,

                        SubTotal =
                            item.Price *
                            item.Quantity
                    })
                .ToList()
    };
}

}