import { useLocation, useNavigate } from "react-router-dom";
import "./OrderSuccess.css";
function OrderSuccess()
{
const location = useLocation();
const navigate = useNavigate();

const order =
    location.state?.order;

const orderId =
    location.state?.orderId ||
    order?.orderId;

const amount =
    location.state?.amount ||
    order?.amount ||
    0;

const paymentId =
    location.state?.paymentId ||
    order?.paymentId ||
    "N/A";

const paymentMethod =
    location.state?.paymentMethod ||
    "UPI";


const formattedAmount =
    Number(amount).toLocaleString(
        "en-IN",
        {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        }
    );


return (
    <div className="order-success-page">

        <div className="order-success-card">

            {/* ==================================================
                SUCCESS ICON
                ================================================== */}

            <div className="success-icon">
                ✓
            </div>


            {/* ==================================================
                TITLE
                ================================================== */}

            <h1>
                Payment Successful!
            </h1>

            <p className="success-message">
                Your order has been placed successfully.
            </p>


            {/* ==================================================
                ORDER DETAILS
                ================================================== */}

            <div className="success-details">

                <div className="success-row">

                    <span>
                        Order ID
                    </span>

                    <strong>
                        #{orderId || "N/A"}
                    </strong>

                </div>


                <div className="success-row">

                    <span>
                        Payment ID
                    </span>

                    <strong>
                        {paymentId}
                    </strong>

                </div>


                <div className="success-row">

                    <span>
                        Payment Method
                    </span>

                    <strong>
                        {paymentMethod}
                    </strong>

                </div>


                <div className="success-row total-row">

                    <span>
                        Amount Paid
                    </span>

                    <strong>
                        ₹{formattedAmount}
                    </strong>

                </div>

            </div>


            {/* ==================================================
                STATUS
                ================================================== */}

            <div className="paid-status">

                ✓ Payment Confirmed

            </div>


            {/* ==================================================
                ACTIONS
                ================================================== */}

            <div className="success-actions">

                <button
                    type="button"
                    className="continue-shopping-button"
                    onClick={() =>
                        navigate("/products")
                    }
                >
                    Continue Shopping
                </button>


                <button
                    type="button"
                    className="view-orders-button"
                    onClick={() =>
                        navigate("/orders")
                    }
                >
                    View My Orders
                </button>

            </div>

        </div>

    </div>
);

}

export default OrderSuccess;
