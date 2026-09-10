import { useEffect, useState } from "react";
import {toast} from "react-hot-toast"
import {
useNavigate,
useParams
} from "react-router-dom";

import {
getProductById,
updateProduct
} from "../services/productService";

function EditProduct() {

const navigate = useNavigate();

const { id } = useParams();


const [name, setName] = useState("");
const [description, setDescription] = useState("");
const [category, setCategory] = useState("");
const [brand, setBrand] = useState("");
const [price, setPrice] = useState("");
const [stockQuantity, setStockQuantity] = useState("");
const [imageUrl, setImageUrl] = useState("");

const [loading, setLoading] = useState(true);

const [saving, setSaving] = useState(false);

const [error, setError] = useState("");


// ==========================================
// LOAD PRODUCT
// ==========================================

useEffect(() => {

    const loadProduct = async () => {

        try {

            const product =
                await getProductById(id);


            setName(product.name || "");

            setDescription(
                product.description || ""
            );

            setCategory(
                product.category || ""
            );

            setBrand(
                product.brand || ""
            );

            setPrice(product.price ?? "");

            setStockQuantity(
                product.stockQuantity ?? ""
            );

            setImageUrl(
                product.imageUrl || ""
            );


        } catch (error) {

            console.error(error);

            setError(
                "Unable to load product."
            );

        } finally {

            setLoading(false);
        }
    };


    loadProduct();

}, [id]);


// ==========================================
// UPDATE
// ==========================================

const handleSubmit = async (event) => {

    event.preventDefault();

    setError("");


    if (!name.trim()) {

        setError(
            "Product name is required."
        );

        return;
    }


    if (!description.trim()) {

        setError(
            "Product description is required."
        );

        return;
    }


    if (!price || Number(price) <= 0) {

        setError(
            "Price must be greater than 0."
        );

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

        setSaving(true);


        const product = {

            name: name.trim(),

            description:
                description.trim(),

            category:
                category.trim(),

            brand:
                brand.trim(),

            price:
                Number(price),

            stockQuantity:
                Number(stockQuantity),

            imageUrl:
                imageUrl.trim()
        };


        await updateProduct(
            id,
            product
        );


        toast.success(
            "Product updated successfully."
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
                "You are not authenticated."
            );

        } else if (
            error.response?.status === 403
        ) {

            setError(
                "Only Admin can update products."
            );

        } else {

            setError(
                error.response?.data?.message ||
                "Failed to update product."
            );
        }

    } finally {

        setSaving(false);
    }
};


if (loading) {

    return (

        <div className="form-page">

            <div className="loading">
                Loading product...
            </div>

        </div>
    );
}


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
                    Edit Product
                </h1>

                <p>
                    Update product information.
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
                            min="0"
                            value={price}
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
                            min="0"
                            value={stockQuantity}
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
                    disabled={saving}
                >

                    {saving
                        ? "Updating..."
                        : "Update Product"}

                </button>

            </form>

        </div>

    </div>
);


}

export default EditProduct;
