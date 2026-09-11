const BACKEND_ORIGIN = "http://localhost:5208";

export const getImageUrl = (value) =>
{
    if (typeof value !== "string")
    {
        return "";
    }

    const trimmedValue = value.trim();

    if (!trimmedValue)
    {
        return "";
    }

    if (
        trimmedValue.startsWith("data:")
        || trimmedValue.startsWith("blob:")
        || trimmedValue.startsWith("http://")
        || trimmedValue.startsWith("https://")
    )
    {
        return trimmedValue;
    }

    if (trimmedValue.startsWith("//"))
    {
        return `${window.location.protocol}${trimmedValue}`;
    }

    if (trimmedValue.startsWith("www."))
    {
        return `https://${trimmedValue}`;
    }

    if (trimmedValue.startsWith("/"))
    {
        return `${BACKEND_ORIGIN}${trimmedValue}`;
    }

    return `https://${trimmedValue}`;
};

export const IMAGE_FALLBACK =
    "data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='640' height='480' viewBox='0 0 640 480'%3E%3Crect width='640' height='480' fill='%23f3f4f6'/%3E%3Cpath d='M192 352l88-104 72 80 48-56 88 80H192z' fill='%23cbd5e1'/%3E%3Ccircle cx='408' cy='176' r='32' fill='%23cbd5e1'/%3E%3Ctext x='320' y='420' fill='%2364748b' font-family='Arial,sans-serif' font-size='24' text-anchor='middle'%3EImage unavailable%3C/text%3E%3C/svg%3E";
