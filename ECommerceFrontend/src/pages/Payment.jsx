import { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import axios from "axios";
import "./Payment.css";
function Payment()
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

const [bank, setBank] =
    useState("");

const [loading, setLoading] =
    useState(false);

const [error, setError] =
    useState("");


// ======================================================
// CHECK PAYMENT SESSION
// ======================================================

if (!order)
{
    return (
        <div className="payment-page">

            <div className="payment-session-error">

                <h2>
                    Payment Session Expired
                </h2>

                <p>
                    Please return to your cart
                    and start checkout again.
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
// AMOUNT
// ======================================================

const amount =
    Number(order.amount || 0);


const formattedAmount =
    amount.toLocaleString(
        "en-IN",
        {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
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


    // ==================================================
    // VALIDATE UPI
    // ==================================================

    if (
        paymentMethod === "UPI" &&
        !upiId.trim()
    )
    {
        setError(
            "Please enter your UPI ID."
        );

        return;
    }


    // ==================================================
    // VALIDATE CARD
    // ==================================================

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


        if (
            cardNumber.replace(
                /\s/g,
                ""
            ).length !== 16
        )
        {
            setError(
                "Please enter a valid 16-digit card number."
            );

            return;
        }


        if (
            cvv.length !== 3
        )
        {
            setError(
                "Please enter a valid 3-digit CVV."
            );

            return;
        }
    }


    // ==================================================
    // VALIDATE NET BANKING
    // ==================================================

    if (
        paymentMethod === "NETBANKING" &&
        !bank
    )
    {
        setError(
            "Please select your bank."
        );

        return;
    }


    try
    {
        setLoading(true);


        const token =
            localStorage.getItem(
                "token"
            );


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
                    1800
                )
        );


        // ==================================================
        // VERIFY PAYMENT
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
            "Payment successful:",
            response.data
        );


        // ==================================================
        // SUCCESS
        // ==================================================

        navigate(
            "/order-success",
            {
                state:
                {
                    orderId:
                        order.orderId,

                    amount:
                        order.amount,

                    paymentId:
                        order.paymentOrderId,

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
            "Payment failed. Please try again."
        );
    }
    finally
    {
        setLoading(false);
    }
};


return (
    <div className="payment-page">

        <div className="payment-container">


            {/* ==================================================
                TOP HEADER
                ================================================== */}

            <div className="payment-topbar">

                <div className="payment-logo">

                    <div className="payment-logo-symbol">
                        R
                    </div>

                    <div>

                        <div className="payment-logo-title">
                            Razorpay
                        </div>

                        <div className="payment-logo-subtitle">
                            Secure Checkout
                        </div>

                    </div>

                </div>


                <div className="payment-secure">

                    <span>
                        🔒
                    </span>

                    Secure Payment

                </div>

            </div>


            {/* ==================================================
                ORDER INFORMATION
                ================================================== */}

            <div className="payment-order-info">

                <div>

                    <span className="payment-label">
                        Amount Payable
                    </span>

                    <div className="payment-amount">
                        ₹{formattedAmount}
                    </div>

                </div>


                <div className="payment-order-id">

                    Order ID

                    <strong>
                        #{order.orderId}
                    </strong>

                </div>

            </div>


            {/* ==================================================
                MAIN CONTENT
                ================================================== */}

            <div className="payment-body">


                {/* ==================================================
                    LEFT SIDE
                    ================================================== */}

                <div className="payment-method-list">

                    <h3>
                        Payment Methods
                    </h3>


                    <button
                        type="button"
                        className={
                            paymentMethod === "UPI"
                                ? "payment-method active"
                                : "payment-method"
                        }
                        onClick={() =>
                            setPaymentMethod(
                                "UPI"
                            )
                        }
                    >

                        <span className="method-icon">
                            U
                        </span>

                        <span className="method-text">

                            <strong>
                                UPI
                            </strong>

                            <small>
                                Google Pay, PhonePe, Paytm
                            </small>

                        </span>

                    </button>


                    <button
                        type="button"
                        className={
                            paymentMethod === "CARD"
                                ? "payment-method active"
                                : "payment-method"
                        }
                        onClick={() =>
                            setPaymentMethod(
                                "CARD"
                            )
                        }
                    >

                        <span className="method-icon">
                            💳
                        </span>

                        <span className="method-text">

                            <strong>
                                Cards
                            </strong>

                            <small>
                                Credit / Debit Card
                            </small>

                        </span>

                    </button>


                    <button
                        type="button"
                        className={
                            paymentMethod === "NETBANKING"
                                ? "payment-method active"
                                : "payment-method"
                        }
                        onClick={() =>
                            setPaymentMethod(
                                "NETBANKING"
                            )
                        }
                    >

                        <span className="method-icon">
                            🏦
                        </span>

                        <span className="method-text">

                            <strong>
                                Net Banking
                            </strong>

                            <small>
                                All major banks
                            </small>

                        </span>

                    </button>

                </div>


                {/* ==================================================
                    RIGHT SIDE
                    ================================================== */}

                <div className="payment-form-container">

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

                                <h2>
                                    Pay using UPI
                                </h2>

                                <p>
                                    Enter your UPI ID to continue
                                </p>


                                <label>
                                    UPI ID
                                </label>


                                <input
                                    type="text"
                                    value={upiId}
                                    onChange={
                                        event =>
                                            setUpiId(
                                                event.target.value
                                            )
                                    }
                                    placeholder="example@upi"
                                />


                                <div className="upi-options">

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

                                <h2>
                                    Enter card details
                                </h2>

                                <p>
                                    Your card information is securely processed.
                                </p>


                                <label>
                                    Card Number
                                </label>

                                <input
                                    type="text"
                                    value={cardNumber}
                                    onChange={
                                        event =>
                                            setCardNumber(
                                                event.target.value
                                            )
                                    }
                                    placeholder="1234 5678 9012 3456"
                                    maxLength="16"
                                />


                                <label>
                                    Card Holder Name
                                </label>

                                <input
                                    type="text"
                                    value={cardName}
                                    onChange={
                                        event =>
                                            setCardName(
                                                event.target.value
                                            )
                                    }
                                    placeholder="Name on card"
                                />


                                <div className="card-input-row">

                                    <div>

                                        <label>
                                            Expiry
                                        </label>

                                        <input
                                            type="text"
                                            value={expiry}
                                            onChange={
                                                event =>
                                                    setExpiry(
                                                        event.target.value
                                                    )
                                            }
                                            placeholder="MM/YY"
                                            maxLength="5"
                                        />

                                    </div>


                                    <div>

                                        <label>
                                            CVV
                                        </label>

                                        <input
                                            type="password"
                                            value={cvv}
                                            onChange={
                                                event =>
                                                    setCvv(
                                                        event.target.value
                                                    )
                                            }
                                            placeholder="•••"
                                            maxLength="3"
                                        />

                                    </div>

                                </div>

                            </div>

                        )}


                        {/* ==================================================
                            NET BANKING
                            ================================================== */}

                        {paymentMethod === "NETBANKING" && (

                            <div className="payment-form">

                                <h2>
                                    Net Banking
                                </h2>

                                <p>
                                    Select your bank to continue
                                </p>


                                <label>
                                    Select Bank
                                </label>


                                <select
                                    value={bank}
                                    onChange={
                                        event =>
                                            setBank(
                                                event.target.value
                                            )
                                    }
                                >

                                    <option value="">
                                        Select your bank
                                    </option>

                                    <option value="SBI">
                                        State Bank of India
                                    </option>

                                    <option value="HDFC">
                                        HDFC Bank
                                    </option>

                                    <option value="ICICI">
                                        ICICI Bank
                                    </option>

                                    <option value="AXIS">
                                        Axis Bank
                                    </option>

                                    <option value="KOTAK">
                                        Kotak Mahindra Bank
                                    </option>

                                </select>

                            </div>

                        )}


                        {/* ==================================================
                            PAY BUTTON
                            ================================================== */}

                        <button
                            type="submit"
                            className="fake-pay-button"
                            disabled={loading}
                        >

                            {loading
                                ? "Processing Payment..."
                                : `Pay ₹${formattedAmount}`
                            }

                        </button>


                        <div className="payment-footer">

                            🔒

                            <span>
                                This is a simulated payment for
                                development/testing.
                            </span>

                        </div>

                    </form>

                </div>

            </div>

        </div>

    </div>
);

}

export default Payment;