import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import "./Cart.css";
import {
getCart,
updateCartItem,
removeCartItem,
clearCart
} from "../services/cartService";
import {
getImageUrl,
IMAGE_FALLBACK
} from "../utils/imageUrl";

function Cart()
{
const navigate = useNavigate();

const [cart, setCart] = useState(null);

const [loading, setLoading] = useState(true);

const [error, setError] = useState("");

const [updatingItem, setUpdatingItem] = useState(null);


// ======================================================
// LOAD CART
// ======================================================

const loadCart = async () =>
{
    try
    {
        setLoading(true);
        setError("");

        const data = await getCart();

        setCart(data);
    }
    catch (err)
    {
        console.error(err);

        if (err.response?.status === 401)
        {
            setError(
                "Your session has expired. Please login again."
            );
        }
        else
        {
            setError(
                err.response?.data?.message ||
                "Unable to load cart."
            );
        }
    }
    finally
    {
        setLoading(false);
    }
};


// ======================================================
// LOAD CART
// ======================================================

useEffect(() =>
{
    loadCart();
}, []);


// ======================================================
// INCREASE / DECREASE QUANTITY
// ======================================================

const handleQuantityChange = async (
    cartItemId,
    newQuantity
) =>
{
    if (newQuantity < 1)
    {
        return;
    }

    try
    {
        setUpdatingItem(cartItemId);
        setError("");

        const updatedCart =
            await updateCartItem(
                cartItemId,
                newQuantity
            );

        setCart(updatedCart);
    }
    catch (err)
    {
        console.error(err);

        setError(
            err.response?.data?.message ||
            "Unable to update quantity."
        );
    }
    finally
    {
        setUpdatingItem(null);
    }
};


// ======================================================
// REMOVE ITEM
// ======================================================

const handleRemove = async (
    cartItemId
) =>
{
    const confirmed =
        window.confirm(
            "Remove this product from your cart?"
        );

    if (!confirmed)
    {
        return;
    }

    try
    {
        setUpdatingItem(cartItemId);
        setError("");

        await removeCartItem(cartItemId);

        await loadCart();
    }
    catch (err)
    {
        console.error(err);

        setError(
            err.response?.data?.message ||
            "Unable to remove item."
        );
    }
    finally
    {
        setUpdatingItem(null);
    }
};


// ======================================================
// CLEAR CART
// ======================================================

const handleClearCart = async () =>
{
    const confirmed =
        window.confirm(
            "Are you sure you want to clear your entire cart?"
        );

    if (!confirmed)
    {
        return;
    }

    try
    {
        setLoading(true);
        setError("");

        await clearCart();

        await loadCart();
    }
    catch (err)
    {
        console.error(err);

        setError(
            err.response?.data?.message ||
            "Unable to clear cart."
        );

        setLoading(false);
    }
};


// ======================================================
// CHECKOUT
// ======================================================

const handleCheckout = () =>
{
    navigate("/checkout");
};


// ======================================================
// LOADING
// ======================================================

if (loading)
{
    return (
        <div className="cart-page">

            <div className="cart-container">

                <div className="cart-loading">

                    <div className="cart-loading-icon">
                        🛒
                    </div>

                    <p>
                        Loading your cart...
                    </p>

                </div>

            </div>

        </div>
    );
}


// ======================================================
// ERROR
// ======================================================

if (error && !cart)
{
    return (
        <div className="cart-page">

            <div className="cart-container">

                <div className="cart-error">

                    <div className="cart-error-icon">
                        ⚠️
                    </div>

                    <h2>
                        Something went wrong
                    </h2>

                    <p>
                        {error}
                    </p>

                    <button
                        type="button"
                        className="cart-retry-button"
                        onClick={loadCart}
                    >
                        Try Again
                    </button>

                </div>

            </div>

        </div>
    );
}


const items =
    cart?.items || [];


// ======================================================
// EMPTY CART
// ======================================================

if (items.length === 0)
{
    return (
        <div className="cart-page">

            <div className="cart-container">

                <div className="cart-header">

                    <div>

                        <h1>
                            Shopping Cart
                        </h1>

                        <p>
                            Your cart is currently empty
                        </p>

                    </div>

                </div>


                <div className="empty-cart">

                    <div className="empty-cart-icon">
                        🛒
                    </div>

                    <h2>
                        Your cart is empty
                    </h2>

                    <p>
                        Looks like you haven't
                        added anything to your
                        cart yet.
                    </p>

                    <Link
                        to="/products"
                        className="empty-cart-button"
                    >
                        Start Shopping
                    </Link>

                </div>

            </div>

        </div>
    );
}


// ======================================================
// CART PAGE
// ======================================================

return (
    <div className="cart-page">

        <div className="cart-container">


            {/* HEADER */}

            <div className="cart-header">

                <div>

                    <h1>
                        Shopping Cart
                    </h1>

                    <p>
                        {cart.totalItems}{" "}
                        {cart.totalItems === 1
                            ? "item"
                            : "items"}{" "}
                        in your cart
                    </p>

                </div>


                <button
                    type="button"
                    className="clear-cart-button"
                    onClick={handleClearCart}
                >
                    Clear Cart
                </button>

            </div>


            {/* ERROR MESSAGE */}

            {error && (

                <div className="cart-message">
                    {error}
                </div>

            )}


            {/* CART LAYOUT */}

            <div className="cart-layout">


                {/* CART ITEMS */}

                <div className="cart-items">

                    {
                        items.map(function(item)
                        {
                            return (
                                <div
                                    className="cart-item"
                                    key={item.cartItemId}
                                >


                                    {/* IMAGE */}

                                    <div className="cart-product-image">

                                        {
                                            getImageUrl(item.imageUrl)
                                                ? (
                                                    <img
                                                        src={
                                                            getImageUrl(item.imageUrl)
                                                        }
                                                        alt={
                                                            item.productName
                                                        }
                                                        onError={(event) =>
                                                        {
                                                            event.currentTarget.onerror = null;
                                                            event.currentTarget.src = IMAGE_FALLBACK;
                                                        }}
                                                    />
                                                )
                                                : (
                                                    <div className="no-image">
                                                        No Image
                                                    </div>
                                                )
                                        }

                                    </div>


                                    {/* PRODUCT INFO */}

                                    <div className="cart-product-info">

                                        <h2>
                                            {
                                                item.productName
                                            }
                                        </h2>


                                        {
                                            item.description && (
                                                <p className="cart-description">
                                                    {
                                                        item.description
                                                    }
                                                </p>
                                            )
                                        }


                                        <p className="cart-price">

                                            ₹
                                            {
                                                Number(
                                                    item.price
                                                ).toLocaleString(
                                                    "en-IN"
                                                )
                                            }

                                            <span>
                                                {" "}each
                                            </span>

                                        </p>

                                    </div>


                                    {/* ACTIONS */}

                                    <div className="cart-product-actions">


                                        {/* QUANTITY */}

                                        <div className="quantity-section">

                                            <span className="quantity-label">
                                                Quantity
                                            </span>


                                            <div className="quantity-control">

                                                <button
                                                    type="button"
                                                    disabled={
                                                        updatingItem ===
                                                            item.cartItemId ||
                                                        item.quantity <= 1
                                                    }
                                                    onClick={
                                                        function()
                                                        {
                                                            handleQuantityChange(
                                                                item.cartItemId,
                                                                item.quantity - 1
                                                            );
                                                        }
                                                    }
                                                >
                                                    −
                                                </button>


                                                <span>
                                                    {
                                                        item.quantity
                                                    }
                                                </span>


                                                <button
                                                    type="button"
                                                    disabled={
                                                        updatingItem ===
                                                        item.cartItemId
                                                    }
                                                    onClick={
                                                        function()
                                                        {
                                                            handleQuantityChange(
                                                                item.cartItemId,
                                                                item.quantity + 1
                                                            );
                                                        }
                                                    }
                                                >
                                                    +
                                                </button>

                                            </div>

                                        </div>


                                        {/* SUBTOTAL */}

                                        <div className="cart-subtotal-section">

                                            <span className="subtotal-label">
                                                Total
                                            </span>

                                            <strong className="cart-subtotal">

                                                ₹
                                                {
                                                    Number(
                                                        item.subTotal
                                                    ).toLocaleString(
                                                        "en-IN"
                                                    )
                                                }

                                            </strong>

                                        </div>


                                        {/* REMOVE */}

                                        <button
                                            type="button"
                                            className="remove-item-button"
                                            disabled={
                                                updatingItem ===
                                                item.cartItemId
                                            }
                                            onClick={
                                                function()
                                                {
                                                    handleRemove(
                                                        item.cartItemId
                                                    );
                                                }
                                            }
                                        >
                                            Remove
                                        </button>

                                    </div>

                                </div>
                            );
                        })
                    }

                </div>


                {/* ORDER SUMMARY */}

                <aside className="cart-summary">

                    <h2>
                        Order Summary
                    </h2>


                    <div className="summary-row">

                        <span>
                            Items
                        </span>

                        <span>
                            {cart.totalItems}
                        </span>

                    </div>


                    <div className="summary-row">

                        <span>
                            Subtotal
                        </span>

                        <span>

                            ₹
                            {
                                Number(
                                    cart.totalAmount
                                ).toLocaleString(
                                    "en-IN"
                                )
                            }

                        </span>

                    </div>


                    <div className="summary-row">

                        <span>
                            Delivery
                        </span>

                        <span className="free-delivery">
                            Free
                        </span>

                    </div>


                    <div className="summary-divider">
                    </div>


                    <div className="summary-total">

                        <span>
                            Total
                        </span>

                        <strong>

                            ₹
                            {
                                Number(
                                    cart.totalAmount
                                ).toLocaleString(
                                    "en-IN"
                                )
                            }

                        </strong>

                    </div>


                    {/* CHECKOUT */}

                    <button
                        type="button"
                        className="checkout-button"
                        onClick={handleCheckout}
                    >
                        Proceed to Checkout
                    </button>


                    {/* CONTINUE SHOPPING */}

                    <Link
                        to="/products"
                        className="continue-shopping-link"
                    >
                        ← Continue Shopping
                    </Link>

                </aside>

            </div>

        </div>

    </div>
);

}

export default Cart;