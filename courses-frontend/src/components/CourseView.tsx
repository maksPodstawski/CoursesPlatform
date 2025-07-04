import { useEffect, useState } from "react";
import { useParams, Link, useNavigate } from "react-router-dom";
import { ArrowLeft, Play, BookOpen, Clock, Star, Check, Lock, Pencil, User } from "lucide-react";
import "../styles/CourseView.css";
import { getCourseById, getCourseInstructor } from "../services/courseService";
import { getCourseStagesWithProgress } from "../services/progressService";
import { createReview, getRatingSummary, getUserReviewForCourse, updateReview, deleteReview } from "../services/reviewService";
import type { StageWithProgress as ApiStageWithProgress } from "../types/courses";
import { getCourseImageUrl } from "../utils/getCourseImageUrl";

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
      createdAt: s.progress?.startedAt || new Date().toISOString(),
    })),
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
      const summary = await getRatingSummary(courseId);
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
          getCourseInstructor(id!),
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
    <div className="course-view">
      <div className="course-view-container">
        {/* Back Button */}
        <Link to="/my-courses" className="back-button">
          <ArrowLeft size={16} />
          Back to My Courses
        </Link>

        {/* Course Header with Thumbnail */}
        <div className="course-header">
          {/* Course Thumbnail */}
          <div className="thumbnail-card">
            <div className="thumbnail-wrapper">
              <img src={getCourseImageUrl(course.thumbnail || "")} alt="Course thumbnail" className="thumbnail-image" />
            </div>
          </div>

          {/* Course Info */}
          <div className="course-info">
            <h1 className="course-title">{course.title}</h1>
            <p className="course-description">{course.description}</p>
          </div>

          {/* Course Meta */}
          <div className="course-meta">
            <div className="meta-item">
              <BookOpen size={16} />
              <span>
                {course.completedStages}/{course.totalStages} stages
              </span>
            </div>
            <div className="meta-item">
              <Clock size={16} />
              <span>{formatDuration(course.totalDuration)}</span>
            </div>
            <div className="meta-item">
              <Star size={16} className="star-filled" />
              <span>
                {ratingSummary ? `${ratingSummary.averageRating.toFixed(1)} (${ratingSummary.reviewCount} reviews)` : "No reviews yet"}
              </span>
            </div>
          </div>

          {/* Instructor and Progress Row */}
          <div className="instructor-progress-row">
            {/* Instructor */}
            <div className="instructor-card">
              <div className="instructor-content">
                <div className="instructor-avatar">
                  <img src={course.instructor.avatar || "/placeholder.svg"} alt={course.instructor.name} />
                  <div className="avatar-fallback">
                    <User size={32} />
                  </div>
                </div>
                <div className="instructor-details">
                  <h3 className="instructor-name">{course.instructor.name}</h3>
                  <p className="instructor-title">{course.instructor.title}</p>
                </div>
              </div>
            </div>

            {/* Progress Section */}
            <div className="progress-card">
              <div className="progress-content">
                <div className="progress-header">
                  <h3>Course Progress</h3>
                  <span className="progress-badge">{course.progress}%</span>
                </div>
                <div className="progress-bar">
                  <div className="progress-fill" style={{ width: `${course.progress}%` }} />
                </div>
                <div className="progress-details">
                  <span>{course.completedStages} completed</span>
                  <span>{course.totalStages - course.completedStages} remaining</span>
                </div>
              </div>
            </div>
          </div>

          {/* Continue Learning Button */}
          <button className="continue-button" onClick={() => goToStage(course.currentStage)}>
            <Play size={20} />
            Continue Learning
          </button>
        </div>

        {/* Course Content */}
        <div className="content-card">
          <div className="content-header">
            <h2>Course Content</h2>
            <p>
              {course.totalStages} stages • {formatDuration(course.totalDuration)} total length
            </p>
          </div>
          <div className="stage-list">
            {course.stages.map((stage, index) => (
              <div key={stage.id} className="stage-item-wrapper">
                <div
                  className={`stage-item ${stage.current ? "current" : ""} ${stage.locked ? "locked" : ""}`}
                  onClick={() => !stage.locked && goToStage(stage.id)}
                >
                  <div className="stage-icon">
                    {stage.completed ? (
                      <div className="completed-icon">
                        <Check size={16} />
                      </div>
                    ) : stage.locked ? (
                      <Lock size={20} />
                    ) : (
                      <div className="play-icon">
                        <Play size={12} />
                      </div>
                    )}
                  </div>
                  <div className="stage-info">
                    <div className="stage-title-wrapper">
                      <h4 className="stage-title">{stage.name}</h4>
                      {stage.current && <span className="current-badge">Current</span>}
                    </div>
                    <p className="stage-description">{stage.description}</p>
                  </div>
                  <div className="stage-duration">
                    <Clock size={12} />
                    <span>{formatDuration(stage.duration)}</span>
                  </div>
                </div>
                {index < course.stages.length - 1 && <div className="stage-separator" />}
              </div>
            ))}
          </div>
        </div>

        {/* Reviews Section */}
        <div className="reviews-card">
          <div className="reviews-header">
            <div>
              <h2>Course Reviews & Rating</h2>
              <p>Share your experience with this course</p>
            </div>
            {userReview && (
              <button className="edit-review-button" onClick={() => setShowReviewForm(!showReviewForm)}>
                <Pencil size={16} />
              </button>
            )}
          </div>
          <div className="reviews-content">
            {!showReviewForm ? (
              <div className="review-display-row">
                <div className="rating-summary">
                  <div className="rating-value">{ratingSummary?.averageRating.toFixed(1) || course.rating.toFixed(1)}</div>
                  <div className="rating-stars">
                    {[1, 2, 3, 4, 5].map((star) => (
                      <Star
                        key={star}
                        size={24}
                        className={star <= Math.round(ratingSummary?.averageRating || course.rating) ? "star-filled" : "star-empty"}
                      />
                    ))}
                  </div>
                  <p className="total-reviews">{ratingSummary?.reviewCount || course.reviews} total ratings</p>
                </div>

                <div className="user-review-section">
                  {userReview ? (
                    <div className="user-review">
                      <h4>Your Review</h4>
                      <div className="user-rating">
                        {[1, 2, 3, 4, 5].map((star) => (
                          <Star key={star} size={16} className={star <= userReview.rating ? "star-filled" : "star-empty"} />
                        ))}
                      </div>
                      <p>{userReview.comment}</p>
                    </div>
                  ) : (
                    <div className="no-review">
                      <button className="add-review-button" onClick={() => setShowReviewForm(true)}>
                        <Star size={20} />
                        Leave a Review
                      </button>
                    </div>
                  )}
                </div>
              </div>
            ) : (
              <div className="review-form">
                <h4>{userReview ? "Edit Your Review" : "Write a Review"}</h4>
                <div className="rating-input">
                  <label>Rating</label>
                  <div className="stars">
                    {[1, 2, 3, 4, 5].map((star) => (
                      <Star
                        key={star}
                        size={32}
                        className={star <= (hoveredRating ?? rating) ? "star-filled" : "star-empty"}
                        onClick={() => setRating(star)}
                        onMouseEnter={() => setHoveredRating(star)}
                        onMouseLeave={() => setHoveredRating(null)}
                      />
                    ))}
                  </div>
                </div>
                <div className="comment-input">
                  <label>Your Experience</label>
                  <textarea
                    value={comment}
                    onChange={(e) => setComment(e.target.value)}
                    placeholder="Tell us about your experience with this course..."
                    rows={5}
                  />
                </div>
                <div className="review-actions">
                  <button className="submit-button" onClick={handleSubmitReview}>
                    {userReview ? "Update Review" : "Submit Review"}
                  </button>
                  <button className="cancel-button" onClick={() => setShowReviewForm(false)}>
                    Cancel
                  </button>
                  {userReview && (
                    <button className="delete-button" onClick={handleDeleteReview}>
                      Delete Review
                    </button>
                  )}
                </div>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}