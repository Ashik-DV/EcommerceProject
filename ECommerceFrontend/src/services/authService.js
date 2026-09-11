const API_URL = "http://localhost:5208/api";

export const registerUser = async (userData) => {
    const response = await fetch(`${API_URL}/Auth/register`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(userData)
    });

    const data = await response.json();

    if (!response.ok) {
        throw new Error(data.message || "Registration failed");
    }

    return data;
};


export const loginUser = async (loginData) => {
    const response = await fetch(`${API_URL}/Auth/login`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(loginData)
    });

    const data = await response.json();

    if (!response.ok) {
        throw new Error(data.message || "Login failed");
    }

    return data;
};

export const loginWithGoogle = async (credential) => {
    const response = await fetch(`${API_URL}/Auth/google`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ credential })
    });

    const data = await response.json();

    if (!response.ok) {
        throw new Error(data.message || "Google login failed");
    }

    return data;
};