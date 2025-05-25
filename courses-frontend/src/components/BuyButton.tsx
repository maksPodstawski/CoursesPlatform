import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { config } from "../config";

interface BuyButtonProps {
    courseId: string;
    price: number;
    redirectAfterLogin?: string;
    children?: React.ReactNode;
}

const BuyButton = ({ courseId, price, redirectAfterLogin, children }: BuyButtonProps) => {
    const { isLoggedIn } = useAuth();
    const navigate = useNavigate();

    const handleBuy = async () => {
        if (!isLoggedIn) {
            navigate(`/login?redirect=${redirectAfterLogin || window.location.pathname}`);
            return;
        }
        try {
            const response = await fetch(
                `${config.apiBaseUrl}${config.apiEndpoints.buyCourse}`,
                {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    credentials: "include",
                    body: JSON.stringify({
                        courseId,
                        price,
                        expirationDate: null,
                    }),
                }
            );
            if (!response.ok) throw new Error("Purchase failed!");
            alert("The course has been purchased!");
        } catch {
            alert("Error purchasing the course!");
        }
    };

    return (
        <button
            type="button"
            className="btn btn-primary"
            onClick={handleBuy}
        >
            {children || "Buy"}
        </button>
    );
};

export default BuyButton;