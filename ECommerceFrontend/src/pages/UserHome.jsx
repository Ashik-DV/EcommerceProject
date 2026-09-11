import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import "./UserHome.css";
function UserHome()
{
const navigate = useNavigate();

const [currentAd, setCurrentAd] = useState(0);

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

const ads = [
    {
        icon: "🔥",
        title: "Mega Sale",
        subtitle: "Up to 50% OFF",
        description: "Grab amazing deals on selected products.",
        button: "Shop Now"
    },
    {
        icon: "⚡",
        title: "Electronics",
        subtitle: "Best Deals Today",
        description: "Discover phones, gadgets and accessories.",
        button: "Explore Deals"
    },
    {
        icon: "👟",
        title: "Fashion Deals",
        subtitle: "Starting From ₹499",
        description: "Upgrade your style with our latest collection.",
        button: "Shop Fashion"
    }
];

useEffect(() =>
{
    const timer = setInterval(() =>
    {
        setCurrentAd((previous) =>
            (previous + 1) % ads.length
        );
    }, 4000);

    return () =>
    {
        clearInterval(timer);
    };
}, []);

const nextAd = () =>
{
    setCurrentAd(
        (currentAd + 1) % ads.length
    );
};

const previousAd = () =>
{
    setCurrentAd(
        (currentAd - 1 + ads.length) % ads.length
    );
};

const ad = ads[currentAd];

const handleLogout = () =>
{
    localStorage.removeItem("token");
    localStorage.removeItem("user");

    navigate("/login");
};

return (
    <div className="user-home-page">

        {/* HEADER */}
        <header className="user-home-header">

            <div>
                <h1>
                    Welcome, {user?.name || "User"}
                </h1>

               
            </div>
            <div className="btns">

            <button
                type="button"
                className="logout-button"
                onClick={() => navigate("/about")}
            >
                ABOUT
            </button>

            <button
                type="button"
                className="logout-button"
                onClick={() => navigate("/orders")}
            >
                MY ORDERS
            </button>

            <button
                type="button"
                className="logout-button"
                onClick={() => navigate("/wishlist")}
            >
                WISHLIST
            </button>
                
            <button
                type="button"
                className="logout-button"
                onClick={handleLogout}
            >
                Logout
            </button>
            
            </div>



        </header>


        {/* MAIN CONTENT */}
        <main className="user-home-container">

            {/* LEFT SIDE */}
            <section className="welcome-card">

                <div className="welcome-icon">
                    🛍️
                </div>

                <h2>
                    Welcome to our Store
                </h2>

                <p>
                    Browse our products and discover
                    something you like.
                </p>

                <button
                    type="button"
                    className="view-products-button"
                    onClick={() =>
                        navigate("/products")
                    }
                >
                    View Products
                </button>

            </section>


            {/* RIGHT SIDE - ADVERTISEMENT */}
            <section className="advertisement-card">

                <div className="ad-content">

                    <div className="ad-icon">
                        {ad.icon}
                    </div>

                    <span className="ad-label">
                        SPECIAL OFFER
                    </span>

                    <h2>
                        {ad.title}
                    </h2>

                    <h3>
                        {ad.subtitle}
                    </h3>

                    <p>
                        {ad.description}
                    </p>

                    <button
                        type="button"
                        className="ad-button"
                        onClick={() =>
                            navigate("/products")
                        }
                    >
                        {ad.button}
                    </button>

                </div>


                {/* PREVIOUS BUTTON */}
                <button
                    type="button"
                    className="ad-arrow ad-arrow-left"
                    onClick={previousAd}
                    aria-label="Previous advertisement"
                >
                    ‹
                </button>


                {/* NEXT BUTTON */}
                <button
                    type="button"
                    className="ad-arrow ad-arrow-right"
                    onClick={nextAd}
                    aria-label="Next advertisement"
                >
                    ›
                </button>


                {/* DOTS */}
                <div className="ad-dots">

                    {ads.map((_, index) => (

                        <button
                            type="button"
                            key={index}
                            className={
                                index === currentAd
                                    ? "ad-dot active"
                                    : "ad-dot"
                            }
                            onClick={() =>
                                setCurrentAd(index)
                            }
                            aria-label={
                                `Show advertisement ${index + 1}`
                            }
                        />

                    ))}

                </div>

            </section>

        </main>


        {/* QUICK BENEFITS */}
        <section className="benefits-section">

            <div className="benefit-card">

                <div className="benefit-icon">
                    🚚
                </div>

                <div>
                    <h3>
                        Fast Delivery
                    </h3>

                    <p>
                        Get your products delivered quickly.
                    </p>
                </div>

            </div>


            <div className="benefit-card">

                <div className="benefit-icon">
                    🔒
                </div>

                <div>
                    <h3>
                        Secure Shopping
                    </h3>

                    <p>
                        Your shopping experience is secure.
                    </p>
                </div>

            </div>


            <div className="benefit-card">

                <div className="benefit-icon">
                    💳
                </div>

                <div>
                    <h3>
                        Easy Payments
                    </h3>

                    <p>
                        Simple and convenient checkout.
                    </p>
                </div>

            </div>

        </section>

    </div>
);

}

export default UserHome;