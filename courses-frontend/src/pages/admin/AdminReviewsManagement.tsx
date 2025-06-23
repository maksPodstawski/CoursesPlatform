import { useEffect, useState } from "react";
import AdminSidebar from "../../components/AdminSidebar";
import { getAllReviews, deleteReview } from "../../services/reviewService";
import { adminService } from "../../services/adminService";
import { checkToxicity } from "../../services/toxicService";
import "../../styles/Admin.css";

type Review = {
    id: string;
    comment: string;
    userName: string;
    score?: number;
};

export default function AdminReviewsManagement() {
    const [toxicReviews, setToxicReviews] = useState<Review[]>([]);
    const [loading, setLoading] = useState<boolean>(true);

    useEffect(() => {
        const init = async () => {
            try {
                await adminService.fetchDashboard(); // tylko do sprawdzenia autoryzacji
                await fetchAndFilter();
            } catch (error) {
                console.error("Błąd autoryzacji administratora:", error);
            }
        };

        init();
    }, []);

    const fetchAndFilter = async () => {
        setLoading(true);
        try {
            const allReviews = await getAllReviews();
            const filtered: Review[] = [];

            for (const review of allReviews) {
                const score = await checkToxicity(review.comment);
                if (score > 0.2) {
                    filtered.push({ ...review, score });
                }
            }

            const sorted = filtered.sort((a, b) => (b.score ?? 0) - (a.score ?? 0));
            setToxicReviews(sorted);
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id: string) => {
        await deleteReview(id);
        setToxicReviews(prev => prev.filter(r => r.id !== id));
    };

    const handleKeep = (id: string) => {
        setToxicReviews(prev => prev.filter(r => r.id !== id));
    };

    return (
        <div style={{ display: "flex" }}>
            <AdminSidebar />
            <div className="admin-content-wrapper">
                <div className="panel-column">
                    <div className="panel-card wide">
                        <h3 className="panel-title violet">💬 Toxic Reviews</h3>

                        {loading ? (
                            <p>Loading...</p>
                        ) : toxicReviews.length === 0 ? (
                            <p className="empty">No toxic comments found.</p>
                        ) : (
                            <ul className="item-list">
                                {toxicReviews.map((review, index) => (
                                    <li key={index} className="item-row">
                                        <p><strong>User:</strong> {review.userName}</p>
                                        <p><strong>Comment:</strong> {review.comment}</p>
                                        <p><strong>Toxicity Score:</strong> {(review.score! * 100).toFixed(1)}%</p>
                                        <div className="actions">
                                            <button onClick={() => handleKeep(review.id)}>Keep</button>
                                            <button onClick={() => handleDelete(review.id)}>Delete</button>
                                        </div>
                                    </li>
                                ))}
                            </ul>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
}
