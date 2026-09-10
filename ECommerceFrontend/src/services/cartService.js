import axios from "axios";

const API_URL = "http://localhost:5208/api/Cart";

// ======================================================
// AUTHORIZATION HEADER
// ======================================================

const getAuthHeaders = () =>
{
const token =
localStorage.getItem("token");

return {
    headers: {
        Authorization: "Bearer " + token
    }
};

};

// ======================================================
// GET CART
// GET: /api/Cart
// ======================================================

export const getCart = async () =>
{
const response =
await axios.get(
API_URL,
getAuthHeaders()
);

return response.data;

};

// ======================================================
// ADD TO CART
// POST: /api/Cart
// ======================================================

export const addToCart = async (
productId,
quantity = 1
) =>
{
const response =
await axios.post(
API_URL,
{
productId: productId,
quantity: quantity
},
getAuthHeaders()
);

return response.data;

};

// ======================================================
// UPDATE CART ITEM
// PUT: /api/Cart/{cartItemId}
// ======================================================

export const updateCartItem = async (
cartItemId,
quantity
) =>
{
const response =
await axios.put(
API_URL + "/" + cartItemId,
{
quantity: quantity
},
getAuthHeaders()
);

return response.data;

};

// ======================================================
// REMOVE CART ITEM
// DELETE: /api/Cart/{cartItemId}
// ======================================================

export const removeCartItem = async (
cartItemId
) =>
{
const response =
await axios.delete(
API_URL + "/" + cartItemId,
getAuthHeaders()
);

return response.data;

};

// ======================================================
// CLEAR CART
// DELETE: /api/Cart
// ======================================================

export const clearCart = async () =>
{
const response =
await axios.delete(
API_URL,
getAuthHeaders()
);

return response.data;

};