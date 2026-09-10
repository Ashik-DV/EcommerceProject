import { useState } from "react";
import { useNavigate } from "react-router-dom";
import axios from "axios";

function Checkout()
{
const navigate = useNavigate();

const [shippingAddress, setShippingAddress] =
    useState("");

const [loading, setLoading] =
    useState(false);

const [error, setError] =
    useState("");


// ======================================================
// PLACE ORDER
// ======================================================

const handleCheckout = async (event) =>
{
    event.preventDefault();


    if (!shippingAddress.trim())
    {
        setError(
            "Please enter your shipping address."
        );

        return;
    }


    try
    {
        setLoading(true);

        setError("");


        const token =
            localStorage.getItem("token");


        if (!token)
        {
            navigate("/login");

            return;
        }


        // ==================================================
        // CREATE PENDING ORDER
        // ==================================================

        const response =
            await axios.post(
                "http://localhost:5208/api/Order/checkout",

                {
                    shippingAddress:
                        shippingAddress.trim()
                },

                {
                    headers:
                    {
                        Authorization:
                            `Bearer ${token}`
                    }
                }
            );


        const order =
            response.data;


        console.log(
            "Order created:",
            order
        );


        // ==================================================
        // GO TO FAKE PAYMENT PAGE
        // ==================================================

        navigate(
            "/payment",
            {
                state:
                {
                    order:
                        order,

                    shippingAddress:
                        shippingAddress.trim()
                }
            }
        );
    }
    catch (err)
    {
        console.error(
            "Checkout error:",
            err
        );


        if (
            err.response?.status === 401
        )
        {
            localStorage.removeItem(
                "token"
            );

            navigate("/login");

            return;
        }


        setError(
            err.response
                ?.data
                ?.message ||
            "Unable to create order."
        );
    }
    finally
    {
        setLoading(false);
    }
};


return (
    <div className="checkout-page">

        <div className="checkout-container">

            {/* ==================================================
                HEADER
                ================================================== */}

            <div className="checkout-header">

                <h1>
                    Checkout
                </h1>

                <p>
                    Enter your delivery address
                </p>

            </div>


            {/* ==================================================
                ERROR
                ================================================== */}

            {error && (
                <div className="checkout-error">
                    {error}
                </div>
            )}


            {/* ==================================================
                FORM
                ================================================== */}

            <form
                className="checkout-form"
                onSubmit={
                    handleCheckout
                }
            >

                <div className="form-group">

                    <label>
                        Shipping Address
                    </label>


                    <textarea
                        value={
                            shippingAddress
                        }
                        onChange={
                            (event) =>
                                setShippingAddress(
                                    event.target.value
                                )
                        }
                        placeholder="House No, Street, Area, City, State, PIN Code"
                        rows="6"
                        required
                    />

                </div>


                {/* ==================================================
                    ACTIONS
                    ================================================== */}

                <div className="checkout-actions">

                    <button
                        type="button"
                        className="back-button"
                        onClick={() =>
                            navigate("/cart")
                        }
                        disabled={loading}
                    >
                        Back to Cart
                    </button>

                    
                  


                    <button
                        type="submit"
                        className="checkout-button"
                        disabled={loading}
                    >
                        {loading
                            ? "Processing..."
                            : "Place Order"}
                    </button>

                </div>

            </form>

        </div>

    </div>
);

}export default Checkout;