import {
BrowserRouter,
Routes,
Route,
Navigate
} from "react-router-dom";
import About from "./pages/About";
import Login
from "./pages/Login";

import Register
from "./pages/Register";

import UserHome
from "./pages/UserHome";

import AdminHome
from "./pages/AdminHome";

import Products
from "./pages/Products";

import AddProduct
from "./pages/AddProduct";

import EditProduct
from "./pages/EditProduct";

import ImportProducts
from "./pages/ImportProducts";

import Cart
from "./pages/Cart";

import Checkout
from "./pages/Checkout";

import Payment
from "./pages/Payment";

import OrderSuccess
from "./pages/OrderSuccess";

import Orders
from "./pages/Orders";

import OrderDetails
from "./pages/OrderDetails";

import Wishlist
from "./pages/Wishlist";

import ProtectedRoute
from "./components/ProtectedRoute";

function App()
{
return (
<BrowserRouter>

        <Routes>

            <Route
                path="/"
                element={
                    <Navigate
                        to="/login"
                        replace
                    />
                }
            />


            <Route
                path="/login"
                element={
                    <Login />
                }
            />


            <Route
                path="/register"
                element={
                    <Register />
                }
            />


            <Route
                path="/user"
                element={
                    <ProtectedRoute
                        allowedRole="User"
                    >
                        <UserHome />
                    </ProtectedRoute>
                }
            />


            <Route
                path="/admin"
                element={
                    <ProtectedRoute
                        allowedRole="Admin"
                    >
                        <AdminHome />
                    </ProtectedRoute>
                }
            />


            <Route
                path="/products"
                element={
                    <ProtectedRoute>
                        <Products />
                    </ProtectedRoute>
                }
            />
                <Route
                path="/about"
                element={
                    <ProtectedRoute>
                        <About/>
                    </ProtectedRoute>
                }
            />

            

            <Route
                path="/products/add"
                element={
                    <ProtectedRoute
                        allowedRole="Admin"
                    >
                        <AddProduct />
                    </ProtectedRoute>
                }
            />


            <Route
                path="/products/edit/:id"
                element={
                    <ProtectedRoute
                        allowedRole="Admin"
                    >
                        <EditProduct />
                    </ProtectedRoute>
                }
            />


            <Route
                path="/products/import"
                element={
                    <ProtectedRoute
                        allowedRole="Admin"
                    >
                        <ImportProducts />
                    </ProtectedRoute>
                }
            />


            {/* ==================================================
                USER CART
                ================================================== */}

            <Route
                path="/cart"
                element={
                    <ProtectedRoute>
                        <Cart />
                    </ProtectedRoute>
                }
            />


            {/* ==================================================
                CHECKOUT
                ================================================== */}

            <Route
                path="/checkout"
                element={
                    <ProtectedRoute>
                        <Checkout />
                    </ProtectedRoute>
                }
            />


            {/* ==================================================
                FAKE RAZORPAY PAYMENT
                ================================================== */}

            <Route
                path="/payment"
                element={
                    <ProtectedRoute>
                        <Payment />
                    </ProtectedRoute>
                }
            />


            {/* ==================================================
                ORDER SUCCESS
                ================================================== */}

            <Route
                path="/order-success"
                element={
                    <ProtectedRoute>
                        <OrderSuccess />
                    </ProtectedRoute>
                }
            />

            <Route
                path="/orders"
                element={
                    <ProtectedRoute>
                        <Orders />
                    </ProtectedRoute>
                }
            />

            <Route
                path="/orders/:id"
                element={
                    <ProtectedRoute>
                        <OrderDetails />
                    </ProtectedRoute>
                }
            />

            <Route
                path="/wishlist"
                element={
                    <ProtectedRoute>
                        <Wishlist />
                    </ProtectedRoute>
                }
            />


            {/* ==================================================
                INVALID ROUTE
                ================================================== */}

            <Route
                path="*"
                element={
                    <Navigate
                        to="/login"
                        replace
                    />
                }
            />

        </Routes>

    </BrowserRouter>
);

}

export default App;