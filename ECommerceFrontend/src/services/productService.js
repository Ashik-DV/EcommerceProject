import axios from "axios";

const API_URL = "http://localhost:5208/api/Products";

const getAuthConfig = () =>
{
    const token = localStorage.getItem("token");

    return {
        headers: {
            Authorization: `Bearer ${token}`
        }
    };
};

export const getProducts = async (params = {}) =>
{
    const response = await axios.get(
        API_URL,
        {
            ...getAuthConfig(),
            params
        }
    );

    return response.data;
};

export const getProductFilterOptions = async () =>
{
    const response = await axios.get(
        `${API_URL}/filters`,
        getAuthConfig()
    );

    return response.data;
};

export const getProductById = async (id) =>
{
    const response = await axios.get(
        `${API_URL}/${id}`,
        getAuthConfig()
    );

    return response.data;
};

export const createProduct = async (product) =>
{
    const response = await axios.post(
        API_URL,
        product,
        getAuthConfig()
    );

    return response.data;
};

export const updateProduct = async (id, product) =>
{
    const response = await axios.put(
        `${API_URL}/${id}`,
        product,
        getAuthConfig()
    );

    return response.data;
};

export const deleteProduct = async (id) =>
{
    const response = await axios.delete(
        `${API_URL}/${id}`,
        getAuthConfig()
    );

    return response.data;
};

export const importProductsCsv = async (file) =>
{
    const formData = new FormData();
    formData.append("file", file);

    const token = localStorage.getItem("token");

    const response = await axios.post(
        `${API_URL}/import-csv`,
        formData,
        {
            headers: {
                Authorization: `Bearer ${token}`
            }
        }
    );

    return response.data;
};
