import { useEffect, useState } from "react";
import { config } from "../config";

interface PurchasedCourse {
    id: string;
    courseId: string;
    purchasedPrice: number;
    purchasedAt: string;
    isActive: boolean;
}

interface CourseDetails {
    id: string;
    name: string;
    description: string | null;
    imageUrl: string;
    stagesCount: number;
    subcategories: string[];
}

interface PurchasedCourseWithDetails extends PurchasedCourse {
    course?: CourseDetails;
}

interface PurchasedCourseWithDetails extends PurchasedCourse { course?: CourseDetails; }


const PurchasedCourses = () => {
    const [courses, setCourses] = useState<PurchasedCourseWithDetails[]>([]);
    const [loading, setLoading] = useState(true);
    const [showForm, setShowForm] = useState<string | null>(null);
    const [rating, setRating] = useState(5);
    const [comment, setComment] = useState("");
    const [submitError, setSubmitError] = useState("");

    useEffect(() => {
        const fetchPurchasedCourses = async () => {
            try {
                const response = await fetch(
                    `${config.apiBaseUrl}${config.apiEndpoints.getPurchasedCoursesUser}`,
                    { credentials: "include" }
                );
                const purchased = await response.json();

                const details = await Promise.all(
                    purchased.map(async (item: PurchasedCourse) => {
                        try {
                            const res = await fetch(
                                `${config.apiBaseUrl}${config.apiEndpoints.getCourses}/${item.courseId}`
                            );
                            if (!res.ok)
                                throw new Error();
                            const course = await res.json();

                            return { ...item, course };
                        } catch {
                            return { ...item, course: undefined };
                        }
                    })
                );
                setCourses(details);
            } catch (error) {
                console.error("Error:", error);
            } finally {
                setLoading(false);
            }
        };

        fetchPurchasedCourses();
    }, []);

    const handleAddRating = async (courseId: string) => {
        setSubmitError("");
        try {
            const response = await fetch(
                `${config.apiBaseUrl}${config.apiEndpoints.addReview}`,
                {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    credentials: "include",
                    body: JSON.stringify({ courseId, rating, comment }),
                }
            );
            if (!response.ok) throw new Error("Failed to add review");
            setShowForm(null);
            setRating(5);
            setComment("");
            alert("Review added!");
        } catch (err) {
            setSubmitError("Error adding review");
        }
    };


    return (
        <div>
            <h1>Your purchased courses</h1>
            {loading ? <p>Loading...</p> : (
                <ul>
                    {courses.map((item) => (
                        <li key={item.id}>
                            {item.course ? (
                                <>
                                    <h2>{item.course.name}</h2>
                                    <img src={item.course.imageUrl} style={{width: "200px", height: "200px", objectFit: "cover"}} alt={item.course.name} />
                                    <p>{item.course.description}</p>
                                    <p><strong>Stages:</strong> {item.course.stagesCount}</p>
                                    <p><strong>Subcategories:</strong> {item.course.subcategories.join(", ")}</p>
                                </>
                            ) : (
                                <p>Course data is unavailable.</p>
                            )}
                            <p><strong>Purchase price:</strong> {item.purchasedPrice} $</p>
                            <p><strong>Date of purchase:</strong> {new Date(item.purchasedAt).toLocaleString()}</p>
                            <p><strong>Active:</strong> {item.isActive ? "Yes" : "No"}</p>

                            <button
                                type="button"
                                className="btn btn-primary"
                                onClick={() => setShowForm(item.courseId)}
                            >
                                Add a rating
                            </button>
                            {showForm === item.courseId && (
                                <form
                                    onSubmit={e => {
                                        e.preventDefault();
                                        handleAddRating(item.courseId);
                                    }}
                                    style={{ marginTop: "1rem" }}
                                >
                                    <label>
                                        Rating:
                                        <select value={rating} onChange={e => setRating(Number(e.target.value))}>
                                            {[1,2,3,4,5].map(n => (
                                                <option key={n} value={n}>{n}</option>
                                            ))}
                                        </select>
                                    </label>
                                    <br />
                                    <label>
                                        Comment:
                                        <textarea
                                            value={comment}
                                            onChange={e => setComment(e.target.value)}
                                            required
                                        />
                                    </label>
                                    <br />
                                    <button type="submit" className="btn btn-primary">Submit</button>
                                    <button type="button" className="btn btn-primary" onClick={() => setShowForm(null)}>Cancel</button>
                                    {submitError && <div style={{color: "red"}}>{submitError}</div>}
                                </form>
                            )}

                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
};

export default PurchasedCourses;