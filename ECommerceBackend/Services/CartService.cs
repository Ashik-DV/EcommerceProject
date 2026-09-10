using ECommerceBackend.DTOs.Cart;
using ECommerceBackend.Models;
using ECommerceBackend.Repositories.Interfaces;
using ECommerceBackend.Services.Interfaces;

namespace ECommerceBackend.Services;

public class CartService : ICartService
{
private readonly ICartRepository _cartRepository;
private readonly IProductRepository _productRepository;

public CartService(
    ICartRepository cartRepository,
    IProductRepository productRepository)
{
    _cartRepository = cartRepository;
    _productRepository = productRepository;
}

// ======================================================
// GET CART
// ======================================================

public CartResponseDto GetCart(int userId)
{
    var cart =
        _cartRepository.GetCartByUserId(userId);

    if (cart == null)
    {
        return new CartResponseDto
        {
            CartId = 0,
            UserId = userId,
            Items = new List<CartItemResponseDto>(),
            TotalAmount = 0,
            TotalItems = 0
        };
    }

    return MapCartToDto(cart);
}

// ======================================================
// ADD TO CART
// ======================================================

public CartResponseDto AddToCart(
    int userId,
    AddToCartDto dto)
{
    var product =
        _productRepository.GetById(dto.ProductId);

    if (product == null)
    {
        throw new Exception("Product not found.");
    }

    if (product.StockQuantity <= 0)
    {
        throw new Exception("Product is out of stock.");
    }

    if (dto.Quantity > product.StockQuantity)
    {
        throw new Exception(
            $"Only {product.StockQuantity} items are available in stock."
        );
    }

    var cart =
        _cartRepository.GetCartByUserId(userId);

    if (cart == null)
    {
        cart = new Cart
        {
            UserId = userId
        };

        cart =
            _cartRepository.CreateCart(cart);
    }

    var existingCartItem =
        _cartRepository.GetCartItemByProductId(
            userId,
            dto.ProductId
        );

    if (existingCartItem != null)
    {
        var newQuantity =
            existingCartItem.Quantity + dto.Quantity;

        if (newQuantity > product.StockQuantity)
        {
            throw new Exception(
                $"Only {product.StockQuantity} items are available in stock."
            );
        }

        existingCartItem.Quantity =
            newQuantity;

        _cartRepository.UpdateCartItem(
            existingCartItem
        );
    }
    else
    {
        var cartItem = new CartItem
        {
            CartId = cart.Id,
            ProductId = product.Id,
            Quantity = dto.Quantity
        };

        _cartRepository.AddCartItem(
            cartItem
        );
    }

    var updatedCart =
        _cartRepository.GetCartByUserId(userId);

    return MapCartToDto(updatedCart!);
}

// ======================================================
// UPDATE CART ITEM
// ======================================================

public CartResponseDto UpdateCartItem(
    int userId,
    int cartItemId,
    UpdateCartItemDto dto)
{
    var cartItem =
        _cartRepository.GetCartItem(
            cartItemId,
            userId
        );

    if (cartItem == null)
    {
        throw new Exception(
            "Cart item not found."
        );
    }

    if (cartItem.Product == null)
    {
        throw new Exception(
            "Product not found."
        );
    }

    if (dto.Quantity > cartItem.Product.StockQuantity)
    {
        throw new Exception(
            $"Only {cartItem.Product.StockQuantity} items are available in stock."
        );
    }

    cartItem.Quantity =
        dto.Quantity;

    _cartRepository.UpdateCartItem(
        cartItem
    );

    var updatedCart =
        _cartRepository.GetCartByUserId(userId);

    return MapCartToDto(updatedCart!);
}

// ======================================================
// REMOVE CART ITEM
// ======================================================

public bool RemoveCartItem(
    int userId,
    int cartItemId)
{
    var cartItem =
        _cartRepository.GetCartItem(
            cartItemId,
            userId
        );

    if (cartItem == null)
    {
        return false;
    }

    var removed =
        _cartRepository.RemoveCartItem(
            cartItem
        );

    return removed;
}

// ======================================================
// CLEAR CART
// ======================================================

public bool ClearCart(int userId)
{
    var cart =
        _cartRepository.GetCartByUserId(userId);

    if (cart == null)
    {
        return true;
    }

    var cleared =
        _cartRepository.ClearCart(cart);

    return cleared;
}

// ======================================================
// MAP CART TO DTO
// ======================================================

private CartResponseDto MapCartToDto(
    Cart cart)
{
    var items = cart.CartItems
        .Where(ci => ci.Product != null)
        .Select(ci => new CartItemResponseDto
        {
            CartItemId = ci.Id,

            ProductId = ci.ProductId,

            ProductName = ci.Product!.Name,

            Description = ci.Product.Description,

            Price = ci.Product.Price,

            ImageUrl = ci.Product.ImageUrl,

            Quantity = ci.Quantity,

            SubTotal =
                ci.Product.Price * ci.Quantity
        })
        .ToList();

    var response = new CartResponseDto
    {
        CartId = cart.Id,

        UserId = cart.UserId,

        Items = items,

        TotalAmount =
            items.Sum(item => item.SubTotal),

        TotalItems =
            items.Sum(item => item.Quantity)
    };

    return response;
}

}