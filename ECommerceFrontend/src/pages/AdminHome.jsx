import { useNavigate } from "react-router-dom";
import LogoutButton from "../components/LogoutButton";

function AdminHome() {


const navigate = useNavigate();

const userData = localStorage.getItem("user");

const user = userData
    ? JSON.parse(userData)
    : null;

return (
    <div className="dashboard">

        <header className="dashboard-header">

            <div>
                <h1>
                    Welcome, {user?.name}
                </h1>

                <p>
                    Admin Dashboard
                </p>
            </div>

            <LogoutButton />

        </header>


        <main className="dashboard-main">

            <div className="admin-card">

                <div className="dashboard-icon">
                    ⚙️
                </div>

                <h2>
                    Admin Panel
                </h2>

                <p>
                    Manage your e-commerce products.
                </p>


                <div className="admin-actions">

                    <button
                        className="primary-button"
                        onClick={() =>
                            navigate("/products")
                        }
                    >
                        Manage Products
                    </button>


                    <button
                        className="secondary-button"
                        onClick={() =>
                            navigate("/products/add")
                        }
                    >
                        + Add Product
                    </button>

                </div>

            </div>

        </main>

    </div>
);

}

export default AdminHome;
