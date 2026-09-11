import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "react-hot-toast";
import {
    getWishlist,
    removeFromWishlist
} from "../services/wishlistService";
import { addToCart } from "../services/cartService";
import "./Orders.css";
import {
    getImageUrl,
    IMAGE_FALLBACK
} from "../utils/imageUrl";

function Wishlist()
{
    const navigate = useNavigate();
    const [items, setItems] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() =>
    {
        const loadWishlist = async () =>
        {
            try
            {
                setItems(await getWishlist());
            }
            catch (err)
            {
                toast.error(
                    err.response?.data?.message ||
                    "Unable to load your wishlist."
                );
            }
            finally
            {
                setLoading(false);
            }
        };

        loadWishlist();
    }, []);

    const handleRemove = async (productId) =>
    {
        try
        {
            await removeFromWishlist(productId);
            setItems(previous =>
                previous.filter(item => item.productId !== productId)
            );
            toast.success("Removed from wishlist.");
        }
        catch (err)
        {
            toast.error(
                err.response?.data?.message ||
                "Unable to remove item."
            );
        }
    };

    const handleAddToCart = async (item) =>
    {
        if (item.stockQuantity <= 0)
        {
            toast.error("This product is out of stock.");
            return;
        }

        try
        {
            await addToCart(item.productId, 1);
            toast.success("Added to cart.");
        }
        catch (err)
        {
            toast.error(
                err.response?.data?.message ||
                "Unable to add product to cart."
            );
        }
    };

    return (
        <div className="orders-page">
            <div className="orders-container">
                <div className="orders-header">
                    <div>
                        <p className="orders-eyebrow">ACCOUNT</p>
                        <h1>My Wishlist</h1>
                        <p>Save products you want to buy later.</p>
                    </div>
                    <button
                        type="button"
                        className="orders-back-button"
                        onClick={() => navigate("/products")}
                    >
                        Continue Shopping
                    </button>
                </div>

                {loading && (
                    <div className="orders-message">
                        Loading your wishlist...
                    </div>
                )}

                {!loading && items.length === 0 && (
                    <div className="orders-empty">
                        <div className="orders-empty-icon">♡</div>
                        <h2>Your wishlist is empty</h2>
                        <p>Tap the heart on a product to save it here.</p>
                        <button
                            type="button"
                            onClick={() => navigate("/products")}
                        >
                            Browse Products
                        </button>
                    </div>
                )}

                {!loading && items.length > 0 && (
                    <div className="wishlist-grid">
                        {items.map(item => (
                            <div className="wishlist-card" key={item.productId}>
                                <div className="wishlist-image">
                                    {getImageUrl(item.imageUrl)
                                        ? <img
                                            src={getImageUrl(item.imageUrl)}
                                            alt={item.productName}
                                            onError={(event) =>
                                            {
                                                event.currentTarget.onerror = null;
                                                event.currentTarget.src = IMAGE_FALLBACK;
                                            }}
                                        />
                                        : "No Image"}
                                </div>
                                <div className="wishlist-content">
                                    <h2>{item.productName}</h2>
                                    <p>{item.description || "No description available."}</p>
                                    <strong>
                                        ₹{Number(item.price).toLocaleString("en-IN")}
                                    </strong>
                                    <span className={item.stockQuantity > 0 ? "wishlist-stock" : "wishlist-stock out"}>
                                        {item.stockQuantity > 0
                                            ? `${item.stockQuantity} in stock`
                                            : "Out of stock"}
                                    </span>
                                    <div className="wishlist-actions">
                                        <button
                                            type="button"
                                            onClick={() => handleAddToCart(item)}
                                            disabled={item.stockQuantity <= 0}
                                        >
                                            Add to Cart
                                        </button>
                                        <button
                                            type="button"
                                            className="wishlist-remove"
                                            onClick={() => handleRemove(item.productId)}
                                        >
                                            Remove
                                        </button>
                                    </div>
                                </div>
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
}

export default Wishlist;
