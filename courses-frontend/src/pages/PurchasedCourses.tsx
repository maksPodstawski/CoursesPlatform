import { useEffect, useState } from "react";
import { fetchClient } from "../services/fetchClient";
import { useAuth } from "../context/AuthContext";

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

const PurchasedCourses = () => {
	const [courses, setCourses] = useState<PurchasedCourseWithDetails[]>([]);
	const [loading, setLoading] = useState(true);
	const [error, setError] = useState<string | null>(null);
	const [showForm, setShowForm] = useState<string | null>(null);
	const [rating, setRating] = useState(5);
	const [comment, setComment] = useState("");
	const [submitError, setSubmitError] = useState("");
	const { logout } = useAuth();

	useEffect(() => {
		const fetchPurchasedCourses = async () => {
			try {
				const response = await fetchClient.fetch("/api/courses/purchases/user");
				const purchased = await response.json();

				const details = await Promise.all(
					purchased.map(async (item: PurchasedCourse) => {
						try {
							const res = await fetchClient.fetch(`/api/courses/${item.courseId}`);
							const course = await res.json();
							return { ...item, course };
						} catch {
							return { ...item, course: undefined };
						}
					}),
				);
				setCourses(details);
				setError(null);
			} catch (error) {
				console.error("Error fetching courses:", error);
				setError("Failed to fetch purchased courses");
				if (error instanceof Error && error.message.includes("401")) {
					await logout();
				}
			} finally {
				setLoading(false);
			}
		};

		fetchPurchasedCourses();
	}, [logout]);

	const handleAddRating = async (courseId: string) => {
		setSubmitError("");
		try {
			const response = await fetchClient.fetch("/api/reviews", {
				method: "POST",
				headers: { "Content-Type": "application/json" },
				body: JSON.stringify({ courseId, rating, comment }),
			});

			if (!response.ok) throw new Error("Failed to add review");
			setShowForm(null);
			setRating(5);
			setComment("");
			alert("Review added!");
		} catch (err) {
			setSubmitError("Error adding review");
			if (err instanceof Error && err.message.includes("401")) {
				await logout();
			}
		}
	};

	if (error) {
		return <div className="error">{error}</div>;
	}

	return (
		<div className="purchased-courses">
			<h1>My Purchased Courses</h1>
			{loading ? (
				<div className="purchased-courses-loading">Loading...</div>
			) : courses.length === 0 ? (
				<div className="purchased-courses-empty">You haven't purchased any courses yet.</div>
			) : (
				<div className="purchased-courses-grid">
					{courses.map((item) => (
						<div key={item.id} className="purchased-course-card">
							{item.course ? (
								<>
									<h3>{item.course.name}</h3>
									<img
										src={item.course.imageUrl}
										style={{ width: "200px", height: "200px", objectFit: "cover" }}
										alt={item.course.name}
									/>
									<p>{item.course.description}</p>
									<p>
										<strong>Stages:</strong> {item.course.stagesCount}
									</p>
									<p>
										<strong>Subcategories:</strong> {item.course.subcategories.join(", ")}
									</p>
									<div className="purchased-course-details">
										<span>Purchased: {new Date(item.purchasedAt).toLocaleDateString()}</span>
										<span>Price: ${item.purchasedPrice}</span>
									</div>
									{!showForm ? (
										<button type="button" className="btn btn-primary" onClick={() => setShowForm(item.courseId)}>
											Add Review
										</button>
									) : showForm === item.courseId ? (
										<div className="review-form">
											<div className="rating-input">
												<label>Rating:</label>
												<input
													type="number"
													min="1"
													max="5"
													value={rating}
													onChange={(e) => setRating(Number(e.target.value))}
												/>
											</div>
											<textarea
												value={comment}
												onChange={(e) => setComment(e.target.value)}
												placeholder="Write your review..."
											/>
											{submitError && <div className="error">{submitError}</div>}
											<div className="review-form-actions">
												<button
													type="button"
													className="btn btn-primary"
													onClick={() => handleAddRating(item.courseId)}
												>
													Submit Review
												</button>
												<button type="button" className="btn" onClick={() => setShowForm(null)}>
													Cancel
												</button>
											</div>
										</div>
									) : null}
								</>
							) : (
								<div className="purchased-course-error">Course details not available</div>
							)}
						</div>
					))}
				</div>
			)}
		</div>
	);
};

export default PurchasedCourses;
