import { useState } from "react";

import {
useNavigate
} from "react-router-dom";

import {
importProductsCsv
} from "../services/productService";

function ImportProducts()
{
const navigate =
useNavigate();


const [file, setFile] =
    useState(null);


const [loading, setLoading] =
    useState(false);


const [message, setMessage] =
    useState("");


const [error, setError] =
    useState("");


const handleFileChange =
    (event) =>
    {
        const selectedFile =
            event.target.files[0];


        setMessage("");
        setError("");


        if (!selectedFile)
        {
            setFile(null);
            return;
        }


        if (
            !selectedFile.name
                .toLowerCase()
                .endsWith(".csv")
        )
        {
            setError(
                "Please select a CSV file."
            );

            setFile(null);
            return;
        }


        setFile(selectedFile);
    };


const handleUpload =
    async (event) =>
    {
        event.preventDefault();


        setMessage("");
        setError("");


        if (!file)
        {
            setError(
                "Please select a CSV file."
            );

            return;
        }


        try
        {
            setLoading(true);


            const response =
                await importProductsCsv(
                    file
                );


            setMessage(
                response.message
            );


            setFile(null);


            event.target.reset();
        }
        catch (error)
        {
            console.error(error);


            if (
                error.response?.status === 401
            )
            {
                setError(
                    "Your login session has expired. Please login again."
                );
            }
            else if (
                error.response?.status === 403
            )
            {
                setError(
                    "Only Admin can import products."
                );
            }
            else
            {
                setError(
                    error.response?.data?.message ||
                    "Failed to import products."
                );
            }
        }
        finally
        {
            setLoading(false);
        }
    };


return (
    <div className="form-page">

        <div className="form-card">


            <button
                className="back-button"
                onClick={() =>
                    navigate(
                        "/products"
                    )
                }
            >
                ← Back
            </button>


            <div className="form-page-header">

                <h1>
                    Import Products
                </h1>


                <p>
                    Upload a CSV file to add
                    multiple products.
                </p>

            </div>


            {error && (
                <div className="form-error">
                    {error}
                </div>
            )}


            {message && (
                <div className="form-success">
                    {message}
                </div>
            )}


            <form
                onSubmit={handleUpload}
                className="product-form"
            >

                <div className="csv-upload-area">

                    <div className="csv-icon">
                        📄
                    </div>


                    <h3>
                        Select CSV File
                    </h3>


                    <p>
                        Product Name, Description, Category, Brand,
                        Price, Stock Quantity, Image URL
                    </p>


                    <input
                        type="file"
                        accept=".csv"
                        onChange={
                            handleFileChange
                        }
                    />

                </div>


                {file && (
                    <div className="selected-file">

                        Selected file:

                        {" "}

                        {file.name}

                    </div>
                )}


                <button
                    type="submit"
                    className="primary-button form-submit"
                    disabled={
                        loading ||
                        !file
                    }
                >

                    {loading
                        ? "Importing..."
                        : "Import Products"}

                </button>

            </form>

        </div>

    </div>
);


}

export default ImportProducts;
