import axios from "axios";

const API_URL = "http://localhost:5208/api/Wishlist";

const getAuthConfig = () => ({
    headers: {
        Authorization:
            "Bearer " + localStorage.getItem("token")
    }
});

export const getWishlist = async () =>
{
    const response = await axios.get(API_URL, getAuthConfig());
    return response.data;
};

export const addToWishlist = async (productId) =>
{
    const response = await axios.post(
        `${API_URL}/${productId}`,
        {},
        getAuthConfig()
    );

    return response.data;
};

export const removeFromWishlist = async (productId) =>
{
    await axios.delete(
        `${API_URL}/${productId}`,
        getAuthConfig()
    );
};
