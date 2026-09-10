import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { createProduct } from "../services/productService";
import {toast} from "react-hot-toast"
function AddProduct() {


const navigate = useNavigate();

const [name, setName] = useState("");
const [description, setDescription] = useState("");
const [category, setCategory] = useState("");
const [brand, setBrand] = useState("");
const [price, setPrice] = useState("");
const [stockQuantity, setStockQuantity] = useState("");
const [imageUrl, setImageUrl] = useState("");

const [error, setError] = useState("");
const [loading, setLoading] = useState(false);


const handleSubmit = async (event) => {

    event.preventDefault();

    setError("");


    // ==========================================
    // VALIDATION
    // ==========================================

    if (!name.trim()) {

        setError("Product name is required.");

        return;
    }


    if (!description.trim()) {

        setError("Product description is required.");

        return;
    }


    if (!price || Number(price) <= 0) {

        setError("Price must be greater than 0.");

        return;
    }


    if (
        stockQuantity === "" ||
        Number(stockQuantity) < 0
    ) {

        setError(
            "Stock quantity cannot be negative."
        );

        return;
    }


    try {

        setLoading(true);


        const product = {

            name: name.trim(),

            description: description.trim(),

            category: category.trim(),

            brand: brand.trim(),

            price: Number(price),

            stockQuantity:
                Number(stockQuantity),

            imageUrl:
                imageUrl.trim()
        };


        await createProduct(product);


        toast.success(
            "Product created successfully."
        );


        navigate(
            "/products",
            {
                replace: true
            }
        );


    } catch (error) {

        console.error(error);


        if (
            error.response?.status === 401
        ) {

            setError(
                "You are not authenticated. Please login again."
            );

        } else if (
            error.response?.status === 403
        ) {

            setError(
                "Only Admin can create products."
            );

        } else {

            setError(
                error.response?.data?.message ||
                "Failed to create product."
            );
        }

    } finally {

        setLoading(false);
    }
};


return (

    <div className="form-page">

        <div className="form-card">

            <div className="form-page-header">

                <button
                    className="back-button"
                    onClick={() =>
                        navigate("/products")
                    }
                >
                    ← Back
                </button>


                <h1>
                    Add Product
                </h1>

                <p>
                    Add a new product to your store.
                </p>

            </div>


            {error && (

                <div className="form-error">
                    {error}
                </div>

            )}


            <form
                onSubmit={handleSubmit}
                className="product-form"
            >


                <div className="form-group">

                    <label>
                        Product Name
                    </label>

                    <input
                        type="text"
                        value={name}
                        placeholder="Enter product name"
                        onChange={(event) =>
                            setName(
                                event.target.value
                            )
                        }
                    />

                </div>


                <div className="form-group">

                    <label>
                        Description
                    </label>

                    <textarea
                        value={description}
                        placeholder="Enter product description"
                        rows="4"
                        onChange={(event) =>
                            setDescription(
                                event.target.value
                            )
                        }
                    />

                </div>


                <div className="form-row">

                    <div className="form-group">

                        <label>
                            Category
                        </label>

                        <input
                            type="text"
                            value={category}
                            placeholder="Electronics"
                            onChange={(event) =>
                                setCategory(event.target.value)
                            }
                        />

                    </div>

                    <div className="form-group">

                        <label>
                            Brand
                        </label>

                        <input
                            type="text"
                            value={brand}
                            placeholder="Samsung"
                            onChange={(event) =>
                                setBrand(event.target.value)
                            }
                        />

                    </div>

                </div>

                <div className="form-row">

                    <div className="form-group">

                        <label>
                            Price
                        </label>

                        <input
                            type="number"
                            value={price}
                            placeholder="79999"
                            min="0"
                            onChange={(event) =>
                                setPrice(
                                    event.target.value
                                )
                            }
                        />

                    </div>


                    <div className="form-group">

                        <label>
                            Stock Quantity
                        </label>

                        <input
                            type="number"
                            value={stockQuantity}
                            placeholder="20"
                            min="0"
                            onChange={(event) =>
                                setStockQuantity(
                                    event.target.value
                                )
                            }
                        />

                    </div>

                </div>


                <div className="form-group">

                    <label>
                        Image URL
                    </label>

                    <input
                        type="url"
                        value={imageUrl}
                        placeholder="https://example.com/product.jpg"
                        onChange={(event) =>
                            setImageUrl(
                                event.target.value
                            )
                        }
                    />

                </div>


                <button
                    type="submit"
                    className="primary-button form-submit"
                    disabled={loading}
                >

                    {loading
                        ? "Creating..."
                        : "Create Product"}

                </button>

            </form>

        </div>

    </div>
);


}

export default AddProduct;
