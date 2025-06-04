import { useEffect, useState } from "react";
import { useParams, Link, useNavigate } from "react-router-dom";
import {
	ArrowLeft,
	Play,
	BookOpen,
	Clock,
	Star,
	Video,
	Check,
	Lock,
	Pencil
} from "lucide-react";
import "../styles/CourseView.css";
import { getCourseById, getCourseInstructor } from "../services/courseService";
import { getCourseStagesWithProgress } from "../services/progressService";
import type { StageWithProgress as ApiStageWithProgress } from "../types/courses";
import { createReview, getRatingSummary, getUserReviewForCourse, updateReview, deleteReview } from "../services/reviewService";
type Course = {
	id?: string;
	courseId?: string;
	name: string;
	description: string;
	imageUrl: string;
	rating?: number;
	reviewsCount?: number;
};

type CourseDetails = {
	id: string;
	title: string;
	description: string;
	thumbnail: string;
	instructor: {
		name: string;
		avatar: string;
		title: string;
	};
	totalStages: number;
	completedStages: number;
	totalDuration: number;
	progress: number;
	rating: number;
	reviews: number;
	currentStage: string;
	stages: {
		id: string;
		courseId: string;
		name: string;
		description: string;
		duration: number;
		videoPath: string;
		completed: boolean;
		locked: boolean;
		current: boolean;
		createdAt: string;
	}[];
};

const mapToCourseDetails = (
	course: Course,
	stages: ApiStageWithProgress[],
	instructor: { name: string; avatar: string; title: string }
): CourseDetails => {
	const completedStages = stages.filter((s) => s.isCompleted).length;
	const totalDuration = stages.reduce((sum, s) => sum + s.duration, 0);
	const progress = Math.round((completedStages / stages.length) * 100);
	const currentStageIndex = stages.findIndex((s) => !s.isCompleted);
	const currentStage = currentStageIndex >= 0 ? stages[currentStageIndex]?.id : stages[0]?.id;

	return {
		id: course.id || "",
		title: course.name,
		description: course.description,
		thumbnail: course.imageUrl,
		instructor,
		totalStages: stages.length,
		completedStages,
		totalDuration,
		progress,
		rating: course.rating ?? 0,
		reviews: course.reviewsCount ?? 0, 
		currentStage,
		stages: stages.map((s, index) => ({
			id: s.id,
			courseId: course.id || "",
			name: s.name,
			description: s.description,
			duration: s.duration,
			videoPath: s.videoPath || "",
			completed: s.isCompleted,
			locked: !s.progress?.startedAt && index > 0 && !stages[index - 1]?.isCompleted,
			current: s.id === currentStage,
			createdAt: s.progress?.startedAt || new Date().toISOString()
		}))
	};
};

const formatDuration = (minutes: number): string => {
	if (minutes < 60) {
		return `${minutes}m`;
	}
	const hours = Math.floor(minutes / 60);
	const remainingMinutes = minutes % 60;
	return remainingMinutes > 0 ? `${hours}h ${remainingMinutes}m` : `${hours}h`;
};

export default function CourseView() {
	const { id } = useParams<{ id: string }>();
	const [course, setCourse] = useState<CourseDetails | null>(null);
	const [loading, setLoading] = useState(true);
	const [showReviewForm, setShowReviewForm] = useState(false);
	const [rating, setRating] = useState(5);
	const [comment, setComment] = useState("");
	const [hoveredRating, setHoveredRating] = useState<number | null>(null);
	const [ratingSummary, setRatingSummary] = useState<{ averageRating: number; reviewCount: number } | null>(null);
	const [userReview, setUserReview] = useState<{ id: string; rating: number; comment: string } | null>(null);
	const navigate = useNavigate();

	const goToStage = (stageId: string) => {
		navigate(`/course/${course?.id}/stage/${stageId}`);
	};
	const loadUserReview = async (courseId: string) => {
	try {
		const review = await getUserReviewForCourse(courseId);
		setUserReview(review);
		if (review) {
			setRating(review.rating);
			setComment(review.comment);
		}
	} catch (error) {
		console.error("Error loading user review:", error);
	}
};
	const loadRatingSummary = async (courseId: string) => {
		try {
			console.log("Loading rating summary for course:", courseId);
			const summary = await getRatingSummary(courseId);
			console.log("Loaded rating summary:", summary);
			setRatingSummary(summary);
		} catch (error) {
			console.error("Error loading rating summary:", error);
		}
	};
	const handleSubmitReview = async () => {
				if (!id) return;

				try {
					if (userReview && userReview.id) {
					
					await updateReview(userReview.id, {
						rating,
						comment,
						courseId: id, 
					});
					alert("Review updated successfully!");
					} else {
					
					await createReview({
						courseId: id,
						rating,
						comment,
					});
					alert("Review submitted successfully!");
					}
					await loadRatingSummary(id);
					await loadUserReview(id);

					setShowReviewForm(false);
				} catch (error) {
					alert("Error submitting review.");
					console.error(error);
				}
				};
	const handleDeleteReview = async () => {
		if (!userReview || !id) return;

		const confirmed = window.confirm("Are you sure you want to delete your review?");
		if (!confirmed) return;

		try {
			await deleteReview(userReview.id);
			alert("Review deleted successfully!");
			setUserReview(null);
			setRating(5);
			setComment("");
			await loadRatingSummary(id);
		} catch (error) {
			alert("Error deleting review.");
			console.error(error);
		}
	};
	useEffect(() => {
		const load = async () => {
			try {
				setLoading(true);
				const [courseData, stages, instructor] = await Promise.all([
					getCourseById(id!),
					getCourseStagesWithProgress(id!),
					getCourseInstructor(id!)
				]);
				const details = mapToCourseDetails(courseData, stages, instructor);
				setCourse(details);
				await loadRatingSummary(courseData.id);
				await loadUserReview(courseData.id);
			} catch (error) {
				console.error("Error loading course:", error);
			} finally {
				setLoading(false);
			}
		};

		if (id) load();
	}, [id]);

	if (loading || !course) return <div className="skeleton">Loading...</div>;

	return (
		<div className="course-container">
			<Link to="/my-courses" className="back-button">
				<ArrowLeft size={16} />
				Back to My Courses
			</Link>

			<div className="course-header">
				<div className="course-content">
					<h1>{course.title}</h1>
					<p className="description">{course.description}</p>

					<div className="course-meta">
						<span>
							<BookOpen size={16}/> {course.completedStages}/{course.totalStages} stages
						</span>
						<span>
							<Clock size={16}/> {formatDuration(course.totalDuration)}
						</span>
						<span>
							<Star size={16} />
							{ratingSummary ? (
								`${ratingSummary.averageRating} (${ratingSummary.reviewCount} reviews)`
							) : (
								"No reviews yet"
							)}
						</span>
					</div>

					<div className="instructor">
						<img src={course.instructor.avatar} alt={course.instructor.name} className="avatar"/>
						<div className="instructor-info">
							<div className="name">{course.instructor.name}</div>
							<div className="title">{course.instructor.title}</div>
						</div>
					</div>

					<div className="progress-section">
						<div className="progress-header">
							<span className="progress-label">Course Progress</span>
							<span className="progress-percentage">{course.progress}%</span>
						</div>
						<div className="progress-bar">
							<div className="progress-fill" style={{width: `${course.progress}%`}}/>
						</div>
					</div>
					<div className="action-buttons">
						<button type="button" className="btn-primary" onClick={() => goToStage(course.currentStage)}>
							<Play size={16}/>
							Continue Learning
						</button>
							{userReview ? (
							<>
								<button type="button" className="btn-secondary" onClick={() => setShowReviewForm(true)}>
									<Pencil size={16} />
									Edit Review
								</button>
								<button type="button" className="btn-danger" onClick={handleDeleteReview}>
									Delete Review
								</button>
							</>
								) : (
									<button type="button" className="btn-secondary" onClick={() => setShowReviewForm(true)}>
										<Pencil size={16} />
										{userReview ? "Edit Review" : "Add Review"}
									</button>
							)}
					</div>
					{showReviewForm && (
						<div className="review-form">
							<h3>Add Your Review</h3>
							<div className="rating-stars">
								<span className="rating-label">Rating:</span>
								{[1, 2, 3, 4, 5].map((star) => (
									<span
										key={star}
										onMouseEnter={() => setHoveredRating(star)}
										onMouseLeave={() => setHoveredRating(null)}
										onClick={() => setRating(star)}
										style={{
											cursor: "pointer",
											color: star <= (hoveredRating ?? rating) ? "#f5c518" : "#444",
											fontSize: "24px"
										}}
									>
										★
									</span>
								))}
							</div>
							<label htmlFor="comment">
								Comment:
								</label>
								<textarea
								id="comment"
								value={comment}
								onChange={(e) => setComment(e.target.value)}
								className="comment-textarea"
								/>
							<div className="review-actions">
								<button onClick={handleSubmitReview} className="btn-primary">
									Submit
								</button>
								<button onClick={() => setShowReviewForm(false)} className="btn-secondary">
									Cancel
								</button>
							</div>
						</div>
					)}
				</div>

				<div className="thumbnail-container">
					{course.thumbnail ? (
						<img src={course.thumbnail} alt="Course thumbnail" className="thumbnail-image"/>
					) : (
						<div className="thumbnail-placeholder">
							<Video size={48}/>
							<span>Course Preview</span>
						</div>
					)}
				</div>
			</div>

			<div className="stages">
				<h2>
					<BookOpen size={24}/>
					Course Stages
				</h2>
				<ul>
					{course.stages.map((stage, index) => (
						<li
							key={stage.id}
							className={`stage ${stage.completed ? "completed" : ""} ${stage.current ? "current" : ""} ${
								stage.locked ? "locked" : ""
							}`}
						>
							<div className="stage-icon">
								{stage.completed ? <Check size={20} /> : stage.locked ? <Lock size={20} /> : index + 1}
							</div>

							<div className="stage-content">
								<h3>
									Stage {index + 1}: {stage.name}
								</h3>
								<p>{stage.description}</p>
								<div className="stage-meta">
									<span>
										<Clock size={14} /> {formatDuration(stage.duration)}
									</span>
									<span>
										<Video size={14} /> Video content
									</span>
								</div>
							</div>

							<div className="stage-actions">
								<div className="stage-duration">{formatDuration(stage.duration)}</div>
								{stage.completed && (
									<button type="button" className="btn-review" onClick={() => goToStage(stage.id)}>
										<Play size={14} />
										Review
									</button>
								)}
								{!stage.completed && !stage.locked && (
									<button type="button" className="btn-review" onClick={() => goToStage(stage.id)}>
										<Play size={14} />
										Start
									</button>
								)}
							</div>
						</li>
					))}
				</ul>
			</div>
		</div>
	);
}
