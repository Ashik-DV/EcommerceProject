import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { getMyOrderById } from "../services/orderService";
import "./Orders.css";

const formatCurrency = (value) =>
    `₹${Number(value).toLocaleString("en-IN", {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    })}`;

const steps = ["Pending", "Paid", "Shipped", "Delivered"];

function OrderDetails()
{
    const { id } = useParams();
    const navigate = useNavigate();
    const [order, setOrder] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() =>
    {
        const loadOrder = async () =>
        {
            try
            {
                setOrder(await getMyOrderById(id));
            }
            catch (err)
            {
                setError(
                    err.response?.data?.message ||
                    "Unable to load this order."
                );
            }
            finally
            {
                setLoading(false);
            }
        };

        loadOrder();
    }, [id]);

    if (loading)
    {
        return <div className="orders-page"><div className="orders-message">Loading order details...</div></div>;
    }

    if (error || !order)
    {
        return (
            <div className="orders-page">
                <div className="orders-message orders-error">{error || "Order not found."}</div>
                <button type="button" className="orders-back-button" onClick={() => navigate("/orders")}>
                    Back to Orders
                </button>
            </div>
        );
    }

    const currentStep = Math.max(
        steps.indexOf(order.status),
        order.status === "Paid" ? 1 : 0
    );

    return (
        <div className="orders-page">
            <div className="order-details-container">
                <button type="button" className="text-back-button" onClick={() => navigate("/orders")}>
                    ← Back to Orders
                </button>

                <div className="order-details-header">
                    <div>
                        <p className="orders-eyebrow">ORDER #{order.id}</p>
                        <h1>Order details</h1>
                    </div>
                    <span className={`order-status status-${order.status.toLowerCase()}`}>
                        {order.status}
                    </span>
                </div>

                <div className="tracking-card">
                    <h2>Order tracking</h2>
                    <div className="tracking-steps">
                        {steps.map((step, index) => (
                            <div className={`tracking-step ${index <= currentStep ? "complete" : ""}`} key={step}>
                                <span>{index <= currentStep ? "✓" : index + 1}</span>
                                <strong>{step}</strong>
                            </div>
                        ))}
                    </div>
                </div>

                <div className="order-details-grid">
                    <section className="details-card">
                        <h2>Items</h2>
                        {order.items.map((item) => (
                            <div className="detail-item" key={item.id}>
                                <div>
                                    <strong>{item.productName}</strong>
                                    <p>{item.quantity} × {formatCurrency(item.price)}</p>
                                </div>
                                <strong>{formatCurrency(item.subTotal)}</strong>
                            </div>
                        ))}
                        <div className="details-total">
                            <span>Total</span>
                            <strong>{formatCurrency(order.totalAmount)}</strong>
                        </div>
                    </section>

                    <section className="details-card">
                        <h2>Delivery address</h2>
                        <p className="address-text">{order.shippingAddress}</p>
                    </section>
                </div>
            </div>
        </div>
    );
}

export default OrderDetails;
