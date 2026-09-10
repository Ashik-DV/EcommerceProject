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

public async Task<CartResponseDto> GetCartAsync(int userId)
{
    var cart =
        await _cartRepository.GetCartByUserIdAsync(userId);

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

public async Task<CartResponseDto> AddToCartAsync(
    int userId,
    AddToCartDto dto)
{
    var product =
        await _productRepository.GetByIdAsync(dto.ProductId);

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
        await _cartRepository.GetCartByUserIdAsync(userId);

    if (cart == null)
    {
        cart = new Cart
        {
            UserId = userId
        };

        cart =
            await _cartRepository.CreateCartAsync(cart);
    }

    var existingCartItem =
        await _cartRepository.GetCartItemByProductIdAsync(
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

        await _cartRepository.UpdateCartItemAsync(
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

        await _cartRepository.AddCartItemAsync(
            cartItem
        );
    }

    var updatedCart =
        await _cartRepository.GetCartByUserIdAsync(userId);

    return MapCartToDto(updatedCart!);
}

// ======================================================
// UPDATE CART ITEM
// ======================================================

public async Task<CartResponseDto> UpdateCartItemAsync(
    int userId,
    int cartItemId,
    UpdateCartItemDto dto)
{
    var cartItem =
        await _cartRepository.GetCartItemAsync(
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

    await _cartRepository.UpdateCartItemAsync(
        cartItem
    );

    var updatedCart =
        await _cartRepository.GetCartByUserIdAsync(userId);

    return MapCartToDto(updatedCart!);
}

// ======================================================
// REMOVE CART ITEM
// ======================================================

public async Task<bool> RemoveCartItemAsync(
    int userId,
    int cartItemId)
{
    var cartItem =
        await _cartRepository.GetCartItemAsync(
            cartItemId,
            userId
        );

    if (cartItem == null)
    {
        return false;
    }

    var removed =
        await _cartRepository.RemoveCartItemAsync(
            cartItem
        );

    return removed;
}

// ======================================================
// CLEAR CART
// ======================================================

public async Task<bool> ClearCartAsync(int userId)
{
    var cart =
        await _cartRepository.GetCartByUserIdAsync(userId);

    if (cart == null)
    {
        return true;
    }

    var cleared =
        await _cartRepository.ClearCartAsync(cart);

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