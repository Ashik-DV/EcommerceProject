import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { registerUser } from "../services/authService";
import "./Register.css";

function Register() {

    const navigate = useNavigate();

    const [formData, setFormData] = useState({
        name: "",
        email: "",
        password: "",
        confirmPassword: ""
    });

    const [errors, setErrors] = useState({});
    const [loading, setLoading] = useState(false);
    const [success, setSuccess] = useState("");


    const handleChange = (e) => {

        const { name, value } = e.target;

        setFormData({
            ...formData,
            [name]: value
        });

        
        setErrors({
            ...errors,
            [name]: ""
        });

        setSuccess("");
    };


    const validateForm = () => {

        const newErrors = {};

        if (!formData.name.trim()) {

            newErrors.name = "Name is required";

        } else if (formData.name.trim().length < 2) {

            newErrors.name =
                "Name must be at least 2 characters";

        }


        // Email validation
        if (!formData.email.trim()) {

            newErrors.email = "Email is required";

        } else if (
            !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(
                formData.email
            )
        ) {

            newErrors.email =
                "Enter a valid email address";

        }


        // Password validation
        if (!formData.password) {

            newErrors.password =
                "Password is required";

        } else if (formData.password.length < 8) {

            newErrors.password =
                "Password must be at least 8 characters";

        } else if (!/[A-Z]/.test(formData.password)) {

            newErrors.password =
                "Password must contain an uppercase letter";

        } else if (!/[a-z]/.test(formData.password)) {

            newErrors.password =
                "Password must contain a lowercase letter";

        } else if (!/[0-9]/.test(formData.password)) {

            newErrors.password =
                "Password must contain a number";

        } else if (
            !/[!@#$%^&*(),.?":{}|<>]/.test(
                formData.password
            )
        ) {

            newErrors.password =
                "Password must contain a special character";

        }


        // Confirm password
        if (!formData.confirmPassword) {

            newErrors.confirmPassword =
                "Please confirm your password";

        } else if (
            formData.password !==
            formData.confirmPassword
        ) {

            newErrors.confirmPassword =
                "Passwords do not match";

        }


        setErrors(newErrors);

        return Object.keys(newErrors).length === 0;
    };


    const handleRegister = async (e) => {

        e.preventDefault();

        setSuccess("");

        // Stop API request if validation fails
        if (!validateForm()) {
            return;
        }

        setLoading(true);

        try {

            await registerUser({
                name: formData.name.trim(),
                email: formData.email.trim(),
                password: formData.password
            });

            setSuccess(
                "Registration successful! Redirecting to login..."
            );

            setTimeout(() => {
                navigate("/login");
            }, 1000);

        } catch (error) {

            setErrors({
                server: error.message
            });

        } finally {

            setLoading(false);
        }
    };


    return (
        <div className="register-page">

            <div className="register-card">

                <div className="register-header">

                    <div className="register-icon">🛍️</div>
                    <h1>Create Account</h1>

                    <p>
                        Join our e-commerce platform
                    </p>

                </div>


                {errors.server && (
                    <div className="error-message">
                        {errors.server}
                    </div>
                )}


                {success && (
                    <div className="success-message">
                        {success}
                    </div>
                )}


                <form className="register-form" onSubmit={handleRegister}>

                    {/* NAME */}

                    <div className="form-group">

                        <label htmlFor="register-name">Name</label>

                        <input
                            id="register-name"
                            type="text"
                            name="name"
                            placeholder="Enter your name"
                            value={formData.name}
                            onChange={handleChange}
                        />

                        {errors.name && (
                            <span className="field-error">
                                {errors.name}
                            </span>
                        )}

                    </div>


                    {/* EMAIL */}

                    <div className="form-group">

                        <label htmlFor="register-email">Email</label>

                        <input
                            id="register-email"
                            type="email"
                            name="email"
                            placeholder="Enter your email"
                            value={formData.email}
                            onChange={handleChange}
                        />

                        {errors.email && (
                            <span className="field-error">
                                {errors.email}
                            </span>
                        )}

                    </div>


                    {/* PASSWORD */}

                    <div className="form-group">

                        <label htmlFor="register-password">Password</label>

                        <input
                            id="register-password"
                            type="password"
                            name="password"
                            placeholder="Create a password"
                            value={formData.password}
                            onChange={handleChange}
                        />

                        {errors.password && (
                            <span className="field-error">
                                {errors.password}
                            </span>
                        )}

                    </div>


                    {/* CONFIRM PASSWORD */}

                    <div className="form-group">

                        <label htmlFor="register-confirm-password">Confirm Password</label>

                        <input
                            id="register-confirm-password"
                            type="password"
                            name="confirmPassword"
                            placeholder="Confirm your password"
                            value={formData.confirmPassword}
                            onChange={handleChange}
                        />

                        {errors.confirmPassword && (
                            <span className="field-error">
                                {errors.confirmPassword}
                            </span>
                        )}

                    </div>


                    <button
                        type="submit"
                        className="register-submit-button"
                        disabled={loading}
                    >

                        {loading
                            ? "Creating Account..."
                            : "Register"}

                    </button>

                </form>


                <div className="register-footer">

                    <p>
                        Already have an account?
                    </p>

                    <button
                        type="button"
                        className="register-login-button"
                        onClick={() =>
                            navigate("/login")
                        }
                    >
                        Login
                    </button>

                </div>

            </div>

        </div>
    );
}

export default Register;
