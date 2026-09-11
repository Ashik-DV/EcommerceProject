import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { getMyOrders } from "../services/orderService";
import "./Orders.css";

const formatCurrency = (value) =>
    `₹${Number(value).toLocaleString("en-IN", {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    })}`;

const formatDate = (value) =>
    new Date(value).toLocaleDateString("en-IN", {
        day: "numeric",
        month: "short",
        year: "numeric"
    });

function Orders()
{
    const navigate = useNavigate();
    const [orders, setOrders] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() =>
    {
        const loadOrders = async () =>
        {
            try
            {
                setOrders(await getMyOrders());
            }
            catch (err)
            {
                setError(
                    err.response?.data?.message ||
                    "Unable to load your orders."
                );
            }
            finally
            {
                setLoading(false);
            }
        };

        loadOrders();
    }, []);

    return (
        <div className="orders-page">
            <div className="orders-container">
                <div className="orders-header">
                    <div>
                        <p className="orders-eyebrow">ACCOUNT</p>
                        <h1>My Orders</h1>
                        <p>Track your purchases and view order details.</p>
                    </div>
                    <button
                        type="button"
                        className="orders-back-button"
                        onClick={() => navigate("/user")}
                    >
                        Back to Home
                    </button>
                </div>

                {loading && <div className="orders-message">Loading your orders...</div>}
                {!loading && error && (
                    <div className="orders-message orders-error">{error}</div>
                )}
                {!loading && !error && orders.length === 0 && (
                    <div className="orders-empty">
                        <div className="orders-empty-icon">📦</div>
                        <h2>No orders yet</h2>
                        <p>Your completed purchases will appear here.</p>
                        <button
                            type="button"
                            onClick={() => navigate("/products")}
                        >
                            Start Shopping
                        </button>
                    </div>
                )}
                {!loading && !error && orders.length > 0 && (
                    <div className="orders-list">
                        {orders.map((order) => (
                            <button
                                type="button"
                                className="order-card"
                                key={order.id}
                                onClick={() => navigate(`/orders/${order.id}`)}
                            >
                                <div className="order-card-main">
                                    <div>
                                        <span className="order-label">ORDER #{order.id}</span>
                                        <h2>{formatCurrency(order.totalAmount)}</h2>
                                        <p>{formatDate(order.createdAt)} · {order.items.length} item(s)</p>
                                    </div>
                                    <span className={`order-status status-${order.status.toLowerCase()}`}>
                                        {order.status}
                                    </span>
                                </div>
                                <span className="order-card-link">View details →</span>
                            </button>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
}

export default Orders;
