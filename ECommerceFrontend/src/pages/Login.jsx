import { useEffect, useRef, useState } from "react";
import {toast} from "react-hot-toast"
import {
    useNavigate
} from "react-router-dom";

import axios from "axios";
import { loginWithGoogle } from "../services/authService";

function Login()
{
    const navigate =
        useNavigate();

    const [email, setEmail] =
        useState("");

    const [password, setPassword] =
        useState("");

    const [error, setError] =
        useState("");

    const [loading, setLoading] =
        useState(false);

    const [showPassword, setShowPassword] =
        useState(false);

    const googleButtonRef = useRef(null);
    const googleClientId = import.meta.env.VITE_GOOGLE_CLIENT_ID;

    useEffect(() => {
        if (!googleClientId || !googleButtonRef.current) {
            return undefined;
        }

        const renderGoogleButton = () => {
            if (!window.google?.accounts?.id || !googleButtonRef.current) {
                return;
            }

            window.google.accounts.id.initialize({
                client_id: googleClientId,
                callback: async (response) => {
                    setError("");
                    setLoading(true);

                    try {
                        const data = await loginWithGoogle(response.credential);

                        localStorage.setItem("token", data.token);
                        localStorage.setItem("user", JSON.stringify(data));

                        const role = String(data.role || "").toLowerCase();
                        navigate(role === "admin" ? "/admin" : "/user", {
                            replace: true
                        });
                    } catch (googleError) {
                        console.error("Google login error:", googleError);
                        setError(googleError.message || "Google login failed.");
                    } finally {
                        setLoading(false);
                    }
                }
            });

            googleButtonRef.current.innerHTML = "";
            window.google.accounts.id.renderButton(googleButtonRef.current, {
                theme: "outline",
                size: "large",
                width: 350,
                text: "signin_with"
            });
        };

        if (window.google?.accounts?.id) {
            renderGoogleButton();
            return undefined;
        }

        const script = document.querySelector(
            'script[src="https://accounts.google.com/gsi/client"]'
        );
        script?.addEventListener("load", renderGoogleButton);

        return () => {
            script?.removeEventListener("load", renderGoogleButton);
        };
    }, [googleClientId, navigate]);


    // ======================================================
    // LOGIN FUNCTIONALITY
    // ======================================================

    const handleSubmit =
        async (event) =>
        {
            event.preventDefault();

            setError("");
            setLoading(true);

            try
            {
                const response =
                    await axios.post(
                        "http://localhost:5208/api/Auth/login",
                        {
                            email: email,
                            password: password
                        }
                    );

                const data =
                    response.data;


                // ==========================================
                // SAVE JWT TOKEN
                // ==========================================

                localStorage.setItem(
                    "token",
                    data.token
                );


                // ==========================================
                // SAVE COMPLETE USER INFORMATION
                // ==========================================

                localStorage.setItem(
                    "user",
                    JSON.stringify(data)
                );


                // ==========================================
                // REDIRECT BASED ON ROLE
                // ==========================================

                const role =
                    String(
                        data.role || ""
                    ).toLowerCase();


                if (role === "admin")
                {
                    navigate(
                        "/admin",
                        {
                            replace: true
                        }
                    );
                }
                else
                {
                    navigate(
                        "/user",
                        {
                            replace: true
                        }
                    );
                }
            }
            catch (error)
            {
                console.error(
                    "Login error:",
                    error
                );

                setError(
                    error.response?.data?.message ||
                    "Invalid email or password."
                );
            }
            finally
            {
                setLoading(false);
            }
        };


    return (
        <div className="modern-login-page">

            {/* ==================================================
                BACKGROUND DECORATIONS
                ================================================== */}

            <div className="login-bg-circle login-bg-circle-one"></div>

            <div className="login-bg-circle login-bg-circle-two"></div>

            <div className="login-bg-circle login-bg-circle-three"></div>


            <div className="login-dot-pattern login-dot-left">
                {Array.from({ length: 15 }).map(
                    (_, index) => (
                        <span key={index}></span>
                    )
                )}
            </div>


            <div className="login-dot-pattern login-dot-right">
                {Array.from({ length: 15 }).map(
                    (_, index) => (
                        <span key={index}></span>
                    )
                )}
            </div>


            {/* ==================================================
                MAIN CONTAINER
                ================================================== */}

            <div className="modern-login-container">


                {/* ==================================================
                    LEFT SECTION
                    ================================================== */}

                <div className="login-intro">

                    <div className="login-welcome">
                        Welcome Back 👋
                    </div>


                    <h1>
                        Glad to see you
                        <br />
                        again!
                    </h1>


                    <div className="login-title-line"></div>


                    <p className="login-intro-text">
                        Login to continue shopping
                        your favorite products and
                        get the best deals.
                    </p>


                    {/* ==================================================
                        FEATURES
                        ================================================== */}

                    <div className="login-benefits">


                        <div className="login-benefit">

                            <div className="benefit-icon">
                                🏷️
                            </div>

                            <div>
                                <strong>
                                    Best Prices
                                </strong>

                                <span>
                                    Get the best deals
                                </span>
                            </div>

                        </div>


                        <div className="login-benefit">

                            <div className="benefit-icon">
                                🛡️
                            </div>

                            <div>
                                <strong>
                                    Secure Shopping
                                </strong>

                                <span>
                                    100% secure & safe
                                </span>
                            </div>

                        </div>


                        <div className="login-benefit">

                            <div className="benefit-icon">
                                🚚
                            </div>

                            <div>
                                <strong>
                                    Fast Delivery
                                </strong>

                                <span>
                                    On-time at your doorstep
                                </span>
                            </div>

                        </div>

                    </div>


                    {/* ==================================================
                        SHOPPING BAG DECORATION
                        ================================================== */}

                    <div className="login-bag-decoration">

                        <div className="bag-handle"></div>

                        <div className="bag-shape">
                            🛍️
                        </div>

                    </div>

                </div>


                {/* ==================================================
                    LOGIN CARD
                    ================================================== */}

                <div className="modern-login-card">


                    {/* ==================================================
                        ICON
                        ================================================== */}

                    <div className="login-main-icon">
                        🛍️
                    </div>


                    <div className="login-card-sparkle sparkle-left">
                        ✦
                    </div>

                    <div className="login-card-sparkle sparkle-right">
                        ✦
                    </div>


                    {/* ==================================================
                        TITLE
                        ================================================== */}

                    <h2>
                        Login
                    </h2>

                    <p className="login-card-subtitle">
                        Sign in to{" "}
                        <span>
                            your account
                        </span>
                    </p>


                    {/* ==================================================
                        ERROR
                        ================================================== */}

                    {error && (
                        <div className="modern-login-error">
                            {error}
                        </div>
                    )}


                    {/* ==================================================
                        FORM
                        ================================================== */}

                    <form
                        onSubmit={handleSubmit}
                    >


                        {/* EMAIL */}

                        <div className="modern-form-group">

                            <label>
                                Email
                            </label>

                            <div className="modern-input-container">

                                <span className="modern-input-icon">
                                    ✉
                                </span>

                                <input
                                    type="email"
                                    value={email}
                                    onChange={
                                        event =>
                                            setEmail(
                                                event.target.value
                                            )
                                    }
                                    placeholder="Enter your email"
                                    required
                                />

                            </div>

                        </div>


                        {/* PASSWORD */}

                        <div className="modern-form-group">

                            <label>
                                Password
                            </label>

                            <div className="modern-input-container">

                                <span className="modern-input-icon">
                                    🔒
                                </span>

                                <input
                                    type={
                                        showPassword
                                            ? "text"
                                            : "password"
                                    }
                                    value={password}
                                    onChange={
                                        event =>
                                            setPassword(
                                                event.target.value
                                            )
                                    }
                                    placeholder="Enter your password"
                                    required
                                />

                                <button
                                    type="button"
                                    className="password-eye-button"
                                    onClick={() =>
                                        setShowPassword(
                                            previous =>
                                                !previous
                                        )
                                    }
                                >
                                    {showPassword
                                        ? "🙈"
                                        : "👁️"}
                                </button>

                            </div>

                        </div>


                        {/* ==================================================
                            OPTIONS
                            ================================================== */}

                        <div className="login-options-row">

                            <label className="remember-me">

                                <input
                                    type="checkbox"
                                />

                                <span>
                                    Remember me
                                </span>

                            </label>


                            <button
                                type="button"
                                className="forgot-password-button"
                                onClick={() =>
                                    toast.success(
                                        "Password reset functionality will be added soon."
                                    )
                                }
                            >
                                Forgot password?
                            </button>

                        </div>


                        {/* ==================================================
                            LOGIN BUTTON
                            ================================================== */}

                        <button
                            type="submit"
                            className="modern-login-submit"
                            disabled={loading}
                        >

                            {loading
                                ? "Logging in..."
                                : (
                                    <>
                                        Login
                                        <span>
                                            →
                                        </span>
                                    </>
                                )}

                        </button>

                    </form>

                    {googleClientId && (
                        <>
                            <div className="login-divider">
                                <span>or continue with</span>
                            </div>

                            <div
                                ref={googleButtonRef}
                                className="google-login-button"
                            ></div>
                        </>
                    )}


                    {/* ==================================================
                        REGISTER
                        ================================================== */}

                    <div className="modern-login-footer">

                        <span>
                            Don't have an account?
                        </span>

                        <button
                            type="button"
                            onClick={() =>
                                navigate(
                                    "/register"
                                )
                            }
                        >
                            Register
                        </button>

                    </div>

                </div>

            </div>


            {/* ==================================================
                PLANT DECORATION
                ================================================== */}

            <div className="login-plant-decoration">

                <div className="plant-stem"></div>

                <div className="plant-leaf plant-leaf-one"></div>

                <div className="plant-leaf plant-leaf-two"></div>

                <div className="plant-leaf plant-leaf-three"></div>

                <div className="plant-leaf plant-leaf-four"></div>

                <div className="plant-pot"></div>

            </div>

        </div>
    );
}

export default Login;