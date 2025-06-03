import { useState } from "react";
import { useNavigate, useParams, useLocation } from "react-router-dom";
import { ArrowLeft, Star, Send } from "lucide-react";
import { config } from "../config";
import { useAuth } from "../context/AuthContext";
import "../styles/CourseReviewForm.css";

const CourseReviewForm: React.FC = () => {
    const { id: courseId } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const location = useLocation();
    const courseTitle = location.state?.courseTitle;
    const { isLoggedIn } = useAuth();

    const [rating, setRating] = useState(0);
    const [hoveredRating, setHoveredRating] = useState(0);
    const [comment, setComment] = useState("");
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [error, setError] = useState<string>("");

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (rating === 0) {
            setError("Please select a rating");
            return;
        }

        if (!comment.trim()) {
            setError("Please write a comment");
            return;
        }

        if (!isLoggedIn) {
            navigate(`/login?redirect=${window.location.pathname}`);
            return;
        }

        setIsSubmitting(true);
        setError("");

        try {
            const response = await fetch(`${config.apiBaseUrl}${config.apiEndpoints.addReview}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                credentials: "include",
                body: JSON.stringify({
                    courseId: courseId,
                    rating: rating,
                    comment: comment.trim()
                })
            });

            if (!response.ok) {
                const errorData = await response.json().catch(() => null);

                if (response.status === 400) {
                    setError(errorData?.message || "You have already reviewed this course");
                } else if (response.status === 404) {
                    setError("Course not found");
                } else if (response.status === 401) {
                    navigate(`/login?redirect=${window.location.pathname}`);
                    return;
                } else {
                    setError("Failed to submit review. Please try again.");
                }
                return;
            }

            navigate(`/course/${courseId}`, {
                state: { message: "Review submitted successfully!" }
            });

        } catch (error) {
            console.error('Error submitting review:', error);
            setError("Network error. Please check your connection and try again.");
        } finally {
            setIsSubmitting(false);
        }
    };

    const handleStarClick = (starValue: number) => {
        setRating(starValue);
        setError("");
    };

    const handleStarHover = (starValue: number) => {
        setHoveredRating(starValue);
    };

    const handleStarLeave = () => {
        setHoveredRating(0);
    };

    const handleClear = () => {
        setRating(0);
        setComment("");
        setError("");
    };

    const goBack = () => {
        navigate(`/course/${courseId}`);
    };

    const displayRating = hoveredRating || rating;

    return (
        <div className="review-container">
            <button onClick={goBack} className="back-button" type="button">
                <ArrowLeft size={16} />
                Back to Course
            </button>

            <div className="review-card">
                <div className="card-header">
                    <h1 className="card-title">Course Review</h1>
                    <p className="card-description">
                        {courseTitle ? `Share your experience with "${courseTitle}"` : "Share your experience with the course"}
                    </p>
                </div>

                {error && (
                    <div className="error-message">
                        {error}
                    </div>
                )}

                <form onSubmit={handleSubmit}>
                    <div className="card-content">
                        <div className="form-group">
                            <label htmlFor="rating-stars" className="form-label">Rating</label>
                            <div className="rating-container" id="rating-stars">
                                <div className="stars-container">
                                    {[1, 2, 3, 4, 5].map((star) => (
                                        <button
                                            key={star}
                                            type="button"
                                            className="star-button"
                                            onClick={() => handleStarClick(star)}
                                            onMouseEnter={() => handleStarHover(star)}
                                            onMouseLeave={handleStarLeave}
                                        >
                                            <Star
                                                className={`star-icon ${
                                                    star <= displayRating
                                                        ? "star-filled"
                                                        : "star-empty"
                                                }`}
                                            />
                                        </button>
                                    ))}
                                </div>
                                <span className="rating-text">
                                    {rating > 0 ? `${rating} out of 5 stars` : "No rating selected"}
                                </span>
                            </div>
                        </div>

                        <div className="form-group">
                            <label htmlFor="comment" className="form-label">Your Review</label>
                            <textarea
                                id="comment"
                                placeholder="Share your thoughts about the course..."
                                value={comment}
                                onChange={(e) => setComment(e.target.value)}
                                rows={4}
                                className="form-textarea"
                                maxLength={250}
                            />
                            <p className="char-count">{comment.length}/250 characters</p>
                        </div>
                    </div>

                    <div className="card-footer">
                        <button
                            type="button"
                            className="btn-danger"
                            onClick={handleClear}
                            disabled={isSubmitting}
                        >
                            Clear
                        </button>
                        <button
                            type="submit"
                            className="btn-primary"
                            disabled={rating === 0 || !comment.trim() || isSubmitting}
                        >
                            {isSubmitting ? (
                                <>
                                    <div className="spinner" />
                                    Submitting...
                                </>
                            ) : (
                                <>
                                    <Send size={16} />
                                    Submit Review
                                </>
                            )}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default CourseReviewForm;