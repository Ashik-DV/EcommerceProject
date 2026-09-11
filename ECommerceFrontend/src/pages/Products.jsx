import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "react-hot-toast";
import {
getProducts,
getProductFilterOptions,
deleteProduct
} from "../services/productService";
import { addToCart } from "../services/cartService";
import {
addToWishlist,
getWishlist,
removeFromWishlist
} from "../services/wishlistService";
import {
    getImageUrl,
    IMAGE_FALLBACK
} from "../utils/imageUrl";

function Products()
{
const navigate = useNavigate();

const [products, setProducts] = useState([]);
const [loading, setLoading] = useState(true);
const [error, setError] = useState("");
const [cartLoading, setCartLoading] = useState(null);
const [buyLoading, setBuyLoading] = useState(null);
const [wishlistIds, setWishlistIds] = useState([]);
const [wishlistLoading, setWishlistLoading] = useState(null);
const [quantities, setQuantities] = useState({});

const [searchText, setSearchText] = useState("");
const [searchTerm, setSearchTerm] = useState("");
const [minPrice, setMinPrice] = useState("");
const [maxPrice, setMaxPrice] = useState("");
const [stockFilter, setStockFilter] = useState("all");
const [category, setCategory] = useState("all");
const [brand, setBrand] = useState("all");
const [sortBy, setSortBy] = useState("default");
const [showFilters, setShowFilters] = useState(false);

const [page, setPage] = useState(1);
const [pageSize] = useState(12);

const [totalCount, setTotalCount] = useState(0);
const [totalPages, setTotalPages] = useState(0);

const [categories, setCategories] = useState([]);
const [brands, setBrands] = useState([]);

const userData = localStorage.getItem("user");
let user = null;

if (userData)
{
    try
    {
        user = JSON.parse(userData);
    }
    catch
    {
        user = null;
    }
}

const isAdmin =
    user &&
    user.role &&
    user.role.toLowerCase() === "admin";

const loadProducts = async (requestedPage = page) =>
{
    try
    {
        setLoading(true);
        setError("");

        const data = await getProducts({
            search: searchTerm || undefined,
            category: category !== "all" ? category : undefined,
            brand: brand !== "all" ? brand : undefined,
            minPrice: minPrice !== "" ? minPrice : undefined,
            maxPrice: maxPrice !== "" ? maxPrice : undefined,
            stock: stockFilter !== "all" ? stockFilter : undefined,
            sortBy: sortBy !== "default" ? sortBy : undefined,
            page: requestedPage,
            pageSize
        });

        setProducts(data.products || []);
        setTotalCount(data.totalCount || 0);
        setTotalPages(data.totalPages || 0);
        setPage(data.page || requestedPage);

        const defaultQuantities = {};

        (data.products || []).forEach(product =>
        {
            defaultQuantities[product.id] =
                quantities[product.id] || 1;
        });

        setQuantities(previous => ({
            ...previous,
            ...defaultQuantities
        }));
    }
    catch (err)
    {
        console.error(err);

        setError(
            err.response?.status === 401
                ? "You are not authorized. Please login again."
                : "Unable to load products."
        );
    }
    finally
    {
        setLoading(false);
    }
};

const loadFilterOptions = async () =>
{
    try
    {
        const data = await getProductFilterOptions();

        setCategories(data.categories || []);
        setBrands(data.brands || []);
    }
    catch (err)
    {
        console.error(
            "Unable to load filter options",
            err
        );
    }
};

useEffect(() =>
{
    loadFilterOptions();
}, []);

useEffect(() =>
{
    if (isAdmin)
    {
        return;
    }

    const loadWishlist = async () =>
    {
        try
        {
            const items = await getWishlist();
            setWishlistIds(items.map(item => item.productId));
        }
        catch (err)
        {
            console.error("Unable to load wishlist", err);
        }
    };

    loadWishlist();
}, [isAdmin]);

useEffect(() =>
{
    const timer = setTimeout(() =>
    {
        setPage(1);
        loadProducts(1);
    }, 500);

    return () => clearTimeout(timer);
}, [
    searchTerm,
    category,
    brand,
    minPrice,
    maxPrice,
    stockFilter,
    sortBy
]);

useEffect(() =>
{
    const timer = setTimeout(() =>
    {
        const trimmed = searchText.trim();

        setSearchTerm(trimmed);
    }, 500);

    return () => clearTimeout(timer);
}, [searchText]);

const handleSearch = () =>
{
    setSearchTerm(searchText.trim());
};

const handleClearFilters = () =>
{
    setSearchText("");
    setSearchTerm("");
    setMinPrice("");
    setMaxPrice("");
    setStockFilter("all");
    setCategory("all");
    setBrand("all");
    setSortBy("default");
    setPage(1);
};

const handlePageChange = (nextPage) =>
{
    if (
        nextPage < 1 ||
        nextPage > totalPages ||
        nextPage === page
    )
    {
        return;
    }

    loadProducts(nextPage);

    window.scrollTo({
        top: 0,
        behavior: "smooth"
    });
};

const handleDelete = async (id) =>
{
    if (
        !window.confirm(
            "Are you sure you want to delete this product?"
        )
    )
    {
        return;
    }

    try
    {
        await deleteProduct(id);

        toast.success(
            "Product deleted successfully."
        );

        await loadProducts(page);
        await loadFilterOptions();
    }
    catch (err)
    {
        console.error(err);

        toast.error(
            err.response?.status === 403
                ? "Only Admin can delete products."
                : "Unable to delete product."
        );
    }
};

const handleQuantityChange = (
    productId,
    quantity,
    stockQuantity
) =>
{
    let newQuantity = Number(quantity);

    if (Number.isNaN(newQuantity))
    {
        newQuantity = 1;
    }

    if (newQuantity < 1)
    {
        newQuantity = 1;
    }

    if (newQuantity > stockQuantity)
    {
        newQuantity = stockQuantity;
    }

    setQuantities(previous => ({
        ...previous,
        [productId]: newQuantity
    }));
};

const handleAddToCart = async (product) =>
{
    const quantity =
        quantities[product.id] || 1;

    if (product.stockQuantity <= 0)
    {
        toast(
            "This product is currently out of stock."
        );

        return;
    }

    if (quantity > product.stockQuantity)
    {
        toast(
            `Only ${product.stockQuantity} items are available in stock.`
        );

        return;
    }

    try
    {
        setCartLoading(product.id);

        await addToCart(
            product.id,
            quantity
        );

        toast.success(
            `${product.name} added to cart successfully.`
        );

        setQuantities(previous => ({
            ...previous,
            [product.id]: 1
        }));
    }
    catch (err)
    {
        console.error(err);

        if (err.response?.status === 401)
        {
            toast(
                "Please login to add products to your cart."
            );

            navigate("/login");
        }
        else
        {
            toast.error(
                err.response?.data?.message ||
                "Unable to add product to cart."
            );
        }
    }
    finally
    {
        setCartLoading(null);
    }
};

const handleBuyNow = async (product) =>
{
    const quantity =
        quantities[product.id] || 1;

    if (product.stockQuantity <= 0)
    {
        toast(
            "This product is currently out of stock."
        );

        return;
    }

    if (quantity > product.stockQuantity)
    {
        toast(
            `Only ${product.stockQuantity} items are available in stock.`
        );

        return;
    }

    try
    {
        setBuyLoading(product.id);

        await addToCart(
            product.id,
            quantity
        );

        navigate("/checkout");
    }
    catch (err)
    {
        console.error(err);

        if (err.response?.status === 401)
        {
            toast(
                "Please login to buy this product."
            );

            navigate("/login");
        }
        else
        {
            toast.error(
                err.response?.data?.message ||
                "Unable to proceed with purchase."
            );
        }
    }
    finally
    {
        setBuyLoading(null);
    }
};

const handleWishlistToggle = async (product) =>
{
    try
    {
        setWishlistLoading(product.id);

        if (wishlistIds.includes(product.id))
        {
            await removeFromWishlist(product.id);
            setWishlistIds(previous =>
                previous.filter(id => id !== product.id)
            );
            toast.success("Removed from wishlist.");
        }
        else
        {
            await addToWishlist(product.id);
            setWishlistIds(previous => [...previous, product.id]);
            toast.success("Added to wishlist.");
        }
    }
    catch (err)
    {
        toast.error(
            err.response?.data?.message ||
            "Unable to update wishlist."
        );
    }
    finally
    {
        setWishlistLoading(null);
    }
};

/*
 * Creates compact pagination.
 *
 * Example:
 * Previous 1 2 3 4 5 ... 86 Next
 *
 * Middle pages:
 * Previous 1 ... 8 9 10 11 12 ... 86 Next
 *
 * Last pages:
 * Previous 1 ... 82 83 84 85 86 Next
 */
const renderPagination = () =>
{
    if (totalPages <= 1)
    {
        return null;
    }

    const pageNumbers = [];

    if (totalPages <= 7)
    {
        for (let i = 1; i <= totalPages; i++)
        {
            pageNumbers.push(i);
        }
    }
    else if (page <= 3)
    {
        pageNumbers.push(1);
        pageNumbers.push(2);
        pageNumbers.push(3);
        pageNumbers.push(4);
        pageNumbers.push(5);
        pageNumbers.push("...");
        pageNumbers.push(totalPages);
    }
    else if (page >= totalPages - 2)
    {
        pageNumbers.push(1);
        pageNumbers.push("...");
        pageNumbers.push(totalPages - 4);
        pageNumbers.push(totalPages - 3);
        pageNumbers.push(totalPages - 2);
        pageNumbers.push(totalPages - 1);
        pageNumbers.push(totalPages);
    }
    else
    {
        pageNumbers.push(1);
        pageNumbers.push("...");
        pageNumbers.push(page - 2);
        pageNumbers.push(page - 1);
        pageNumbers.push(page);
        pageNumbers.push(page + 1);
        pageNumbers.push(page + 2);
        pageNumbers.push("...");
        pageNumbers.push(totalPages);
    }

    return (
        <div className="pagination">

            <button
                type="button"
                onClick={() =>
                    handlePageChange(page - 1)
                }
                disabled={page === 1}
            >
                Previous
            </button>

            {pageNumbers.map(
                (pageNumber, index) =>
                {
                    if (pageNumber === "...")
                    {
                        return (
                            <span
                                key={`dots-${index}`}
                                className="pagination-dots"
                            >
                                ...
                            </span>
                        );
                    }

                    return (
                        <button
                            type="button"
                            key={pageNumber}
                            onClick={() =>
                                handlePageChange(
                                    pageNumber
                                )
                            }
                            className={
                                pageNumber === page
                                    ? "active"
                                    : ""
                            }
                        >
                            {pageNumber}
                        </button>
                    );
                }
            )}

            <button
                type="button"
                onClick={() =>
                    handlePageChange(page + 1)
                }
                disabled={
                    page === totalPages
                }
            >
                Next
            </button>

        </div>
    );
};

if (loading && products.length === 0)
{
    return (
        <div className="products-page">
            <div className="loading">
                Loading products...
            </div>
        </div>
    );
}

return (
    <div className="products-page">

        <div className="products-container">

            <div className="products-header">

                <div>
                    <h1>Products</h1>

                    <p>
                        {isAdmin
                            ? "Manage your products"
                            : "Explore our products"}
                    </p>
                </div>

                <div className="products-header-actions">

                    <button
                        type="button"
                        className="cart-header-button"
                        onClick={() =>
                            navigate("/admin")
                        }
                    >
                        Home
                    </button>

                    <button
                        type="button"
                        className="cart-header-button"
                        onClick={() =>
                            navigate("/cart")
                        }
                    >
                        🛒 View Cart
                    </button>

                    {!isAdmin && (
                        <button
                            type="button"
                            className="cart-header-button"
                            onClick={() => navigate("/wishlist")}
                        >
                            ♡ Wishlist
                        </button>
                    )}

                    {isAdmin && (
                        <>
                            <button
                                type="button"
                                className="add-product-button"
                                onClick={() =>
                                    navigate(
                                        "/products/add"
                                    )
                                }
                            >
                                + Add Product
                            </button>

                            <button
                                type="button"
                                className="csv-button"
                                onClick={() =>
                                    navigate(
                                        "/products/import"
                                    )
                                }
                            >
                                Upload CSV
                            </button>
                        </>
                    )}

                </div>

            </div>

            <div className="search-filter-section">

                <div className="search-row">

                    <input
                        type="text"
                        className="search-input"
                        placeholder="Search name, description, category or brand..."
                        value={searchText}
                        onChange={(event) =>
                            setSearchText(
                                event.target.value
                            )
                        }
                        onKeyDown={(event) =>
                        {
                            if (
                                event.key ===
                                "Enter"
                            )
                            {
                                handleSearch();
                            }
                        }}
                    />

                    <button
                        type="button"
                        className="filter-button"
                        onClick={() =>
                            setShowFilters(
                                previous =>
                                    !previous
                            )
                        }
                    >
                        ⚙ Filter
                    </button>

                    <button
                        type="button"
                        className="clear-filter-button"
                        onClick={
                            handleClearFilters
                        }
                    >
                        ✕ Clear
                    </button>

                </div>

                {showFilters && (
                    <div className="filter-panel">

                        <div className="filter-group">

                            <label>
                                Category
                            </label>

                            <select
                                value={category}
                                onChange={(event) =>
                                {
                                    setCategory(
                                        event.target.value
                                    );
                                    setPage(1);
                                }}
                            >
                                <option value="all">
                                    All Categories
                                </option>

                                {categories.map(item => (
                                    <option
                                        key={item}
                                        value={item}
                                    >
                                        {item}
                                    </option>
                                ))}
                            </select>

                        </div>

                        <div className="filter-group">

                            <label>
                                Brand
                            </label>

                            <select
                                value={brand}
                                onChange={(event) =>
                                {
                                    setBrand(
                                        event.target.value
                                    );
                                    setPage(1);
                                }}
                            >
                                <option value="all">
                                    All Brands
                                </option>

                                {brands.map(item => (
                                    <option
                                        key={item}
                                        value={item}
                                    >
                                        {item}
                                    </option>
                                ))}
                            </select>

                        </div>

                        <div className="filter-group">

                            <label>
                                Minimum Price
                            </label>

                            <input
                                type="number"
                                min="0"
                                placeholder="₹ Min"
                                value={minPrice}
                                onChange={(event) =>
                                {
                                    setMinPrice(
                                        event.target.value
                                    );
                                    setPage(1);
                                }}
                            />

                        </div>

                        <div className="filter-group">

                            <label>
                                Maximum Price
                            </label>

                            <input
                                type="number"
                                min="0"
                                placeholder="₹ Max"
                                value={maxPrice}
                                onChange={(event) =>
                                {
                                    setMaxPrice(
                                        event.target.value
                                    );
                                    setPage(1);
                                }}
                            />

                        </div>

                        <div className="filter-group">

                            <label>
                                Stock
                            </label>

                            <select
                                value={stockFilter}
                                onChange={(event) =>
                                {
                                    setStockFilter(
                                        event.target.value
                                    );
                                    setPage(1);
                                }}
                            >
                                <option value="all">
                                    All Products
                                </option>

                                <option value="in-stock">
                                    In Stock
                                </option>

                                <option value="out-of-stock">
                                    Out of Stock
                                </option>
                            </select>

                        </div>

                        <div className="filter-group">

                            <label>
                                Sort By
                            </label>

                            <select
                                value={sortBy}
                                onChange={(event) =>
                                {
                                    setSortBy(
                                        event.target.value
                                    );
                                    setPage(1);
                                }}
                            >
                                <option value="default">
                                    Default
                                </option>

                                <option value="price-low">
                                    Price: Low to High
                                </option>

                                <option value="price-high">
                                    Price: High to Low
                                </option>

                                <option value="name-az">
                                    Name: A to Z
                                </option>

                                <option value="name-za">
                                    Name: Z to A
                                </option>
                            </select>

                        </div>

                    </div>
                )}

            </div>

            {!error && (
                <div className="product-result-info">

                    {totalCount === 0
                        ? "No products match the selected criteria."
                        : (
                            <>
                                Showing{" "}
                                <strong>
                                    {products.length}
                                </strong>{" "}
                                of{" "}
                                <strong>
                                    {totalCount}
                                </strong>{" "}
                                matching products
                            </>
                        )}

                </div>
            )}

            {error && (
                <div className="error-message">

                    {error}

                    <button
                        type="button"
                        onClick={() =>
                            loadProducts(page)
                        }
                    >
                        Retry
                    </button>

                </div>
            )}

            {!error &&
                products.length === 0 && (
                    <div className="empty-products">

                        <h2>
                            No Matching Products
                        </h2>

                        <p>
                            Try changing your search
                            or filter criteria.
                        </p>

                        <button
                            type="button"
                            className="primary-button"
                            onClick={
                                handleClearFilters
                            }
                        >
                            Clear Search & Filters
                        </button>

                    </div>
                )}

            {!error &&
                products.length > 0 && (
                    <>
                        <div className="product-grid">

                            {products.map(product => (
                                <div
                                    className="product-card"
                                    key={product.id}
                                >

                                    <div className="product-image">

                                        {getImageUrl(product.imageUrl) ? (
                                            <img
                                                src={
                                                    getImageUrl(product.imageUrl)
                                                }
                                                alt={
                                                    product.name
                                                }
                                                onError={(event) =>
                                                {
                                                    event.currentTarget.onerror = null;
                                                    event.currentTarget.src = IMAGE_FALLBACK;
                                                }}
                                            />
                                        ) : (
                                            <span>
                                                No Image
                                            </span>
                                        )}

                                    </div>

                                    <div className="product-content">

                                        {!isAdmin && (
                                            <button
                                                type="button"
                                                className={`wishlist-toggle ${wishlistIds.includes(product.id) ? "active" : ""}`}
                                                onClick={() => handleWishlistToggle(product)}
                                                disabled={wishlistLoading === product.id}
                                                aria-label={wishlistIds.includes(product.id) ? "Remove from wishlist" : "Add to wishlist"}
                                            >
                                                {wishlistIds.includes(product.id) ? "♥" : "♡"}
                                            </button>
                                        )}

                                        <h2>
                                            {product.name}
                                        </h2>

                                        <p className="product-description">
                                            {product.description ||
                                                "No description available."}
                                        </p>

                                        <div className="product-price">
                                            ₹
                                            {Number(
                                                product.price
                                            ).toLocaleString(
                                                "en-IN"
                                            )}
                                        </div>

                                        {(product.category ||
                                            product.brand) && (
                                            <div className="stock">

                                                {product.category && (
                                                    <span>
                                                        Category:{" "}
                                                        {
                                                            product.category
                                                        }
                                                    </span>
                                                )}

                                                {product.brand && (
                                                    <span>
                                                        {
                                                            product.category
                                                                ? " • "
                                                                : ""
                                                        }
                                                        Brand:{" "}
                                                        {
                                                            product.brand
                                                        }
                                                    </span>
                                                )}

                                            </div>
                                        )}

                                        <div className="stock">
                                            Stock:{" "}
                                            {
                                                product.stockQuantity
                                            }
                                        </div>

                                        {!isAdmin && (
                                            <>
                                                {product.stockQuantity >
                                                0 ? (
                                                    <div className="shopping-controls">

                                                        <div className="quantity-selector">

                                                            <label>
                                                                Quantity
                                                            </label>

                                                            <div className="quantity-input-wrapper">

                                                                <button
                                                                    type="button"
                                                                    onClick={() =>
                                                                        handleQuantityChange(
                                                                            product.id,
                                                                            (quantities[
                                                                                product
                                                                                    .id
                                                                            ] ||
                                                                                1) -
                                                                                1,
                                                                            product.stockQuantity
                                                                        )
                                                                    }
                                                                    disabled={
                                                                        (quantities[
                                                                            product
                                                                                .id
                                                                        ] ||
                                                                            1) <=
                                                                        1
                                                                    }
                                                                >
                                                                    −
                                                                </button>

                                                                <input
                                                                    type="number"
                                                                    min="1"
                                                                    max={
                                                                        product.stockQuantity
                                                                    }
                                                                    value={
                                                                        quantities[
                                                                            product
                                                                                .id
                                                                        ] ||
                                                                        1
                                                                    }
                                                                    onChange={(
                                                                        event
                                                                    ) =>
                                                                        handleQuantityChange(
                                                                            product.id,
                                                                            event
                                                                                .target
                                                                                .value,
                                                                            product.stockQuantity
                                                                        )
                                                                    }
                                                                />

                                                                <button
                                                                    type="button"
                                                                    onClick={() =>
                                                                        handleQuantityChange(
                                                                            product.id,
                                                                            (quantities[
                                                                                product
                                                                                    .id
                                                                            ] ||
                                                                                1) +
                                                                                1,
                                                                            product.stockQuantity
                                                                        )
                                                                    }
                                                                    disabled={
                                                                        (quantities[
                                                                            product
                                                                                .id
                                                                        ] ||
                                                                            1) >=
                                                                        product.stockQuantity
                                                                    }
                                                                >
                                                                    +
                                                                </button>

                                                            </div>

                                                        </div>

                                                        <div className="product-buy-buttons">

                                                            <button
                                                                type="button"
                                                                className="add-to-cart-button"
                                                                onClick={() =>
                                                                    handleAddToCart(
                                                                        product
                                                                    )
                                                                }
                                                                disabled={
                                                                    cartLoading ===
                                                                        product.id ||
                                                                    buyLoading ===
                                                                        product.id
                                                                }
                                                            >
                                                                {cartLoading ===
                                                                product.id
                                                                    ? "Adding..."
                                                                    : "🛒 Add to Cart"}
                                                            </button>

                                                            <button
                                                                type="button"
                                                                className="buy-now-button"
                                                                onClick={() =>
                                                                    handleBuyNow(
                                                                        product
                                                                    )
                                                                }
                                                                disabled={
                                                                    cartLoading ===
                                                                        product.id ||
                                                                    buyLoading ===
                                                                        product.id
                                                                }
                                                            >
                                                                {buyLoading ===
                                                                product.id
                                                                    ? "Processing..."
                                                                    : "⚡ Buy Now"}
                                                            </button>

                                                        </div>

                                                    </div>
                                                ) : (
                                                    <div className="out-of-stock">
                                                        Out of Stock
                                                    </div>
                                                )}
                                            </>
                                        )}

                                        {isAdmin && (
                                            <div className="product-actions">

                                                <button
                                                    type="button"
                                                    className="edit-button"
                                                    onClick={() =>
                                                        navigate(
                                                            `/products/edit/${product.id}`
                                                        )
                                                    }
                                                >
                                                    Edit
                                                </button>

                                                <button
                                                    type="button"
                                                    className="delete-button"
                                                    onClick={() =>
                                                        handleDelete(
                                                            product.id
                                                        )
                                                    }
                                                >
                                                    Delete
                                                </button>

                                            </div>
                                        )}

                                    </div>

                                </div>
                            ))}

                        </div>

                        {renderPagination()}

                    </>
                )}

        </div>

    </div>
);

}

export default Products;