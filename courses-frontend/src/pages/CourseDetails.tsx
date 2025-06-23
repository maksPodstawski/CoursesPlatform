import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { config } from "../config";
import BuyButton from "../components/BuyButton.tsx";

interface Course {
    id: string;
    name: string;
    description: string;
    price: number;
    imageUrl: string;
    creators: { name: string }[];
}

interface Review {
    id: string;
    rating: number;
    comment: string;
    createdAt: string;
    userName: string;
}


const CourseDetails = () => {
    const { id } = useParams<{ id: string }>();
    const [course, setCourse] = useState<Course | null>(null);
    const [loading, setLoading] = useState(true);
    const [averageRating, setAverageRating] = useState<string | null>(null);
    const [reviews, setReviews] = useState<Review[]>([]);

    useEffect(() => {
        const fetchCourse = async () => {
            try {
                const response = await fetch(`${config.apiBaseUrl}${config.apiEndpoints.getCourses}/${id}`);
                if (!response.ok) throw new Error("Course not found");
                const data = await response.json();
                setCourse(data);
            } catch {
                setCourse(null);
            } finally {
                setLoading(false);
            }
        };

        const fetchAverageRating = async () => {
            try {
                if (!id) return;
                const url = config.apiEndpoints.avgRating.replace("{courseId}", id);
                const res = await fetch(`${config.apiBaseUrl}${url}`);
                if (!res.ok) throw new Error();
                const data = await res.json();
                // The rating-summary endpoint returns { averageRating: number, reviewCount: number }
                setAverageRating(data.averageRating !== null ? Number(data.averageRating).toFixed(2) : null);
            } catch {
                setAverageRating(null);
            }
        };

        const fetchReviews = async () => {
            try {
                if (!id) return;
                const url = config.apiEndpoints.getReviewsCourse.replace("{courseId}", id);
                const res = await fetch(`${config.apiBaseUrl}${url}`);
                if (!res.ok) throw new Error();
                const data = await res.json();
                setReviews(data);
            } catch {
                setReviews([]);
            }
        };

        if (id) {
            fetchCourse();
            fetchAverageRating();
            fetchReviews();
        }
    }, [id]);

    if (loading) return <p>Loading...</p>;
    if (!course) return <p>Course not found.</p>;

    return (
        <div>
            <h1>{course.name}</h1>
            <img src={course.imageUrl} style={{width: "300px", height: "300px", objectFit: "cover"}} alt={course.name}/>
            <p>{course.description}</p>
            <p><strong>Price:</strong> {course.price} $</p>
            <p><strong>Author(s):</strong> {course.creators && course.creators.length > 0
                ? course.creators.map(c => c.name).join(", ")
                : "Unknown"}</p>
            <p><strong>Average rating:</strong> {averageRating !== null ? averageRating : "No ratings yet"}</p>
            <BuyButton courseId={course.id} price={course.price} redirectAfterLogin={`/courses/${course.id}`} />

            <h2>Reviews</h2>
            {reviews.length === 0 ? (
                <p>No reviews yet.</p>
            ) : (
                <ul>
                    {reviews.map(review => (
                        <li key={review.id} style={{marginBottom: "1em"}}>
                            <strong>{review.userName}</strong> ({new Date(review.createdAt).toLocaleDateString()}):<br/>
                            <span>Rating: {review.rating}/5</span>
                            <p>{review.comment}</p>
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
};

export default CourseDetails;