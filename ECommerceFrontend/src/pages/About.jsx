import { useNavigate } from "react-router-dom";
import "./About.css";

function About() {
    const navigate = useNavigate();
    const isLoggedIn = Boolean(localStorage.getItem("token"));

    return (
        <div className="about-page">
          
            <main className="about-container">
                <section className="about-hero">
                    <span className="about-eyebrow">ABOUT OUR STORE</span>
                    <h1>Simple shopping, reliable service.</h1>
                    <p>
                        ECommerce is designed to make discovering products,
                        managing your cart, and completing purchases simple and convenient.
                    </p>
                    <div className="about-hero-actions">
                        <button type="button" className="about-primary-button" onClick={() => navigate(isLoggedIn ? "/products" : "/login")}>
                            Explore Products
                        </button>
                        <button type="button" className="about-secondary-button" onClick={() => navigate(isLoggedIn ? "/user" : "/register")}>
                            {isLoggedIn ? "Back to Home" : "Create an Account"}
                        </button>
                    </div>
                </section>

                <section className="about-values">
                    <article className="about-card">
                        <div className="about-card-icon">✓</div>
                        <h2>Quality First</h2>
                        <p>We keep the shopping experience clear and focused so you can make confident product decisions.</p>
                    </article>

                    <article className="about-card">
                        <div className="about-card-icon">⚡</div>
                        <h2>Fast Experience</h2>
                        <p>From browsing products to checkout, every part of the experience is designed to stay simple and responsive.</p>
                    </article>

                    <article className="about-card">
                        <div className="about-card-icon">🔒</div>
                        <h2>Secure Shopping</h2>
                        <p>Your account and shopping flow are protected with authenticated access and secure application practices.</p>
                    </article>
                </section>

                <section className="about-story">
                    <div>
                        <span className="about-section-label">OUR PLATFORM</span>
                        <h2>Everything you need in one place.</h2>
                    </div>
                    <p>
                        Browse products, use filters to find what you need, add items to your cart,
                        and move through checkout with a clean and straightforward interface.
                        Administrators also have dedicated tools for managing products and inventory.
                    </p>
                </section>
            </main>
        </div>
    );
}

export default About;
