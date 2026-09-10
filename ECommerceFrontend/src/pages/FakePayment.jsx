import { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import axios from "axios";

function FakePayment()
{
const location = useLocation();
const navigate = useNavigate();

const order =
    location.state?.order;


const [paymentMethod, setPaymentMethod] =
    useState("UPI");

const [upiId, setUpiId] =
    useState("");

const [cardNumber, setCardNumber] =
    useState("");

const [cardName, setCardName] =
    useState("");

const [expiry, setExpiry] =
    useState("");

const [cvv, setCvv] =
    useState("");

const [loading, setLoading] =
    useState(false);

const [error, setError] =
    useState("");


// ======================================================
// CHECK ORDER
// ======================================================

if (!order)
{
    return (
        <div className="payment-page">

            <div className="payment-error-box">

                <h2>
                    Payment Session Expired
                </h2>

                <p>
                    Please go back to checkout.
                </p>

                <button
                    onClick={() =>
                        navigate("/cart")
                    }
                >
                    Go to Cart
                </button>

            </div>

        </div>
    );
}


// ======================================================
// FORMAT AMOUNT
// ======================================================

const formattedAmount =
    Number(order.amount).toLocaleString(
        "en-IN",
        {
            minimumFractionDigits: 2
        }
    );


// ======================================================
// PAYMENT
// ======================================================

const handlePayment = async (
    event
) =>
{
    event.preventDefault();

    setError("");


    // --------------------------------------------------
    // Validate UPI
    // --------------------------------------------------

    if (
        paymentMethod === "UPI" &&
        !upiId.trim()
    )
    {
        setError(
            "Please enter a UPI ID."
        );

        return;
    }


    // --------------------------------------------------
    // Validate Card
    // --------------------------------------------------

    if (
        paymentMethod === "CARD"
    )
    {
        if (
            !cardNumber.trim() ||
            !cardName.trim() ||
            !expiry.trim() ||
            !cvv.trim()
        )
        {
            setError(
                "Please enter all card details."
            );

            return;
        }
    }


    try
    {
        setLoading(true);


        const token =
            localStorage.getItem("token");


        if (!token)
        {
            navigate("/login");

            return;
        }


        // ==================================================
        // FAKE PAYMENT PROCESSING
        // ==================================================

        await new Promise(
            resolve =>
                setTimeout(
                    resolve,
                    1500
                )
        );


        // ==================================================
        // VERIFY PAYMENT WITH BACKEND
        // ==================================================

        const response =
            await axios.post(
                "http://localhost:5208/api/Order/verify-payment",

                {
                    orderId:
                        order.orderId,

                    paymentOrderId:
                        order.paymentOrderId,

                    paymentMethod:
                        paymentMethod
                },

                {
                    headers:
                    {
                        Authorization:
                            `Bearer ${token}`
                    }
                }
            );


        console.log(
            "Payment result:",
            response.data
        );


        // ==================================================
        // PAYMENT SUCCESS
        // ==================================================

        navigate(
            "/order-success",
            {
                state:
                {
                    order:
                    {
                        ...order,

                        status:
                            "Paid"
                    },

                    paymentMethod:
                        paymentMethod
                }
            }
        );
    }
    catch (err)
    {
        console.error(
            "Payment error:",
            err
        );


        setError(
            err.response
                ?.data
                ?.message ||
            "Payment failed. Please try again."
        );
    }
    finally
    {
        setLoading(false);
    }
};


return (
    <div className="fake-payment-page">

        <div className="payment-wrapper">


            {/* ==================================================
                PAYMENT HEADER
                ================================================== */}

            <div className="payment-header">

                <div className="payment-brand">

                    <div className="payment-brand-icon">
                        R
                    </div>

                    <div>

                        <h2>
                            Razorpay
                        </h2>

                        <span>
                            Secure Checkout
                        </span>

                    </div>

                </div>


                <div className="secure-label">

                    🔒 Secure

                </div>

            </div>


            {/* ==================================================
                ORDER SUMMARY
                ================================================== */}

            <div className="payment-summary">

                <div>

                    <span>
                        Amount Payable
                    </span>

                    <strong>
                        ₹{formattedAmount}
                    </strong>

                </div>


                <div className="order-number">

                    Order #
                    {order.orderId}

                </div>

            </div>


            {/* ==================================================
                PAYMENT METHODS
                ================================================== */}

            <div className="payment-content">


                <div className="payment-methods">

                    <h3>
                        Payment Method
                    </h3>


                    <button
                        type="button"
                        className={
                            paymentMethod === "UPI"
                                ? "method active"
                                : "method"
                        }
                        onClick={() =>
                            setPaymentMethod(
                                "UPI"
                            )
                        }
                    >
                        <span>
                            UPI
                        </span>

                        <small>
                            Pay using UPI
                        </small>

                    </button>


                    <button
                        type="button"
                        className={
                            paymentMethod === "CARD"
                                ? "method active"
                                : "method"
                        }
                        onClick={() =>
                            setPaymentMethod(
                                "CARD"
                            )
                        }
                    >
                        <span>
                            💳 Card
                        </span>

                        <small>
                            Credit / Debit Card
                        </small>

                    </button>


                    <button
                        type="button"
                        className={
                            paymentMethod === "NETBANKING"
                                ? "method active"
                                : "method"
                        }
                        onClick={() =>
                            setPaymentMethod(
                                "NETBANKING"
                            )
                        }
                    >
                        <span>
                            🏦 Net Banking
                        </span>

                        <small>
                            Pay using your bank
                        </small>

                    </button>

                </div>


                {/* ==================================================
                    PAYMENT FORM
                    ================================================== */}

                <div className="payment-form-section">

                    {error && (
                        <div className="payment-error">
                            {error}
                        </div>
                    )}


                    <form
                        onSubmit={
                            handlePayment
                        }
                    >


                        {/* ==================================================
                            UPI
                            ================================================== */}

                        {paymentMethod === "UPI" && (

                            <div className="payment-form">

                                <h3>
                                    Pay using UPI
                                </h3>

                                <p>
                                    Enter your UPI ID
                                </p>


                                <input
                                    type="text"
                                    placeholder="example@upi"
                                    value={upiId}
                                    onChange={
                                        event =>
                                            setUpiId(
                                                event.target.value
                                            )
                                    }
                                />


                                <div className="upi-apps">

                                    <div>
                                        GPay
                                    </div>

                                    <div>
                                        PhonePe
                                    </div>

                                    <div>
                                        Paytm
                                    </div>

                                </div>

                            </div>

                        )}


                        {/* ==================================================
                            CARD
                            ================================================== */}

                        {paymentMethod === "CARD" && (

                            <div className="payment-form">

                                <h3>
                                    Card Details
                                </h3>


                                <input
                                    type="text"
                                    placeholder="Card Number"
                                    maxLength="16"
                                    value={cardNumber}
                                    onChange={
                                        event =>
                                            setCardNumber(
                                                event.target.value
                                            )
                                    }
                                />


                                <input
                                    type="text"
                                    placeholder="Card Holder Name"
                                    value={cardName}
                                    onChange={
                                        event =>
                                            setCardName(
                                                event.target.value
                                            )
                                    }
                                />


                                <div className="card-row">

                                    <input
                                        type="text"
                                        placeholder="MM/YY"
                                        maxLength="5"
                                        value={expiry}
                                        onChange={
                                            event =>
                                                setExpiry(
                                                    event.target.value
                                                )
                                        }
                                    />


                                    <input
                                        type="password"
                                        placeholder="CVV"
                                        maxLength="3"
                                        value={cvv}
                                        onChange={
                                            event =>
                                                setCvv(
                                                    event.target.value
                                                )
                                        }
                                    />

                                </div>

                            </div>

                        )}


                        {/* ==================================================
                            NET BANKING
                            ================================================== */}

                        {paymentMethod === "NETBANKING" && (

                            <div className="payment-form">

                                <h3>
                                    Net Banking
                                </h3>

                                <p>
                                    Select your bank
                                </p>


                                <select>

                                    <option>
                                        Select Bank
                                    </option>

                                    <option>
                                        State Bank of India
                                    </option>

                                    <option>
                                        HDFC Bank
                                    </option>

                                    <option>
                                        ICICI Bank
                                    </option>

                                    <option>
                                        Axis Bank
                                    </option>

                                </select>

                            </div>

                        )}


                        {/* ==================================================
                            PAY BUTTON
                            ================================================== */}

                        <button
                            type="submit"
                            className="pay-button"
                            disabled={loading}
                        >
                            {loading
                                ? "Processing Payment..."
                                : `Pay ₹${formattedAmount}`
                            }
                        </button>


                        <div className="payment-security">

                            🔒 Your payment information
                            is secure

                        </div>

                    </form>

                </div>

            </div>

        </div>

    </div>
);

}
export default FakePayment;