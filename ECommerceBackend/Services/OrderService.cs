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

public OrderCreateResponseDto CreateOrder(
    int userId,
    OrderCreateDto dto)
{
    var cart =
        _cartRepository.GetCartByUserId(
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
        _orderRepository.Create(
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

    _orderRepository.Update(
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

public bool VerifyPayment(
    int userId,
    FakePaymentDto dto)
{
    // ==================================================
    // Get order belonging to current user
    // ==================================================

    var order =
        _orderRepository.GetById(
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
        _cartRepository.GetCartByUserId(
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

    _orderRepository.Update(
        order
    );


    // ==================================================
    // Clear Cart
    // ==================================================

    _cartRepository.ClearCart(
        cart
    );

    return true;
}


// ======================================================
// GET MY ORDERS
// ======================================================

public List<OrderResponseDto> GetMyOrders(
    int userId)
{
    var orders =
        _orderRepository.GetByUserId(
            userId
        );

    return orders
        .Select(MapToDto)
        .ToList();
}


// ======================================================
// GET MY ORDER BY ID
// ======================================================

public OrderResponseDto? GetMyOrderById(
    int userId,
    int orderId)
{
    var order =
        _orderRepository.GetById(
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

public List<OrderResponseDto> GetAllOrders()
{
    var orders =
        _orderRepository.GetAll();

    return orders
        .Select(MapToDto)
        .ToList();
}


// ======================================================
// GET ORDER BY ID - ADMIN
// ======================================================

public OrderResponseDto? GetOrderByIdForAdmin(
    int orderId)
{
    var order =
        _orderRepository.GetByIdForAdmin(
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