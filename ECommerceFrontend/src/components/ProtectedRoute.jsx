import { Navigate } from "react-router-dom";

function ProtectedRoute({
children,
allowedRole
})
{
const token =
localStorage.getItem("token");

const userData =
    localStorage.getItem("user");


if (!token)
{
    return (
        <Navigate
            to="/login"
            replace
        />
    );
}


let user = null;


try
{
    user =
        userData
            ? JSON.parse(userData)
            : null;
}
catch
{
    localStorage.removeItem("token");
    localStorage.removeItem("user");

    return (
        <Navigate
            to="/login"
            replace
        />
    );
}


if (!user)
{
    localStorage.removeItem("token");

    return (
        <Navigate
            to="/login"
            replace
        />
    );
}


if (
    allowedRole &&
    user.role?.toLowerCase() !==
        allowedRole.toLowerCase()
)
{
    if (
        user.role?.toLowerCase() ===
        "admin"
    )
    {
        return (
            <Navigate
                to="/admin"
                replace
            />
        );
    }


    return (
        <Navigate
            to="/user"
            replace
        />
    );
}


return children;

}

export default ProtectedRoute;