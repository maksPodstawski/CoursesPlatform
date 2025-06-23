import {
  Clock, Users, Star, User, BookOpen, Video, FileText, Code, Database, Rocket
} from "lucide-react";
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import {
  getCourseById,
  getCourseInstructor,
  getCourseParticipantsCount,
  getStagesByCourse
} from "../services/courseService";
import {
  getReviewsByCourse,
  getRatingSummary
} from "../services/reviewService";
import type { Course, Stage, ReviewResponseDTO } from "../types/courses";
import "../styles/CourseDetails.css";

const ICONS = [BookOpen, Code, FileText, Video, Database, Rocket];

function getInitials(name: string) {
  return name
    .split(" ")
    .map((n) => n[0])
    .join("")
    .toUpperCase();
}

const CourseDetails = () => {
  const { id } = useParams<{ id: string }>();
  const [course, setCourse] = useState<Course | null>(null);
  const [instructor, setInstructor] = useState<string>("");
  const [participantsCount, setParticipantsCount] = useState<number | null>(null);
  const [stages, setStages] = useState<Stage[]>([]);
  const [reviews, setReviews] = useState<ReviewResponseDTO[]>([]);
  const [ratingSummary, setRatingSummary] = useState<{ averageRating: number; reviewCount: number } | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) return;
    setLoading(true);
    setError(null);
    Promise.all([
      getCourseById(id),
      getCourseInstructor(id),
      getCourseParticipantsCount(id),
      getStagesByCourse(id),
      getReviewsByCourse(id),
      getRatingSummary(id)
    ])
      .then(([
        courseData,
        instructorData,
        participantsCountData,
        stagesData,
        reviewsData,
        ratingSummaryData
      ]) => {
        setCourse(courseData);
        setInstructor(instructorData?.name || "Unknown");
        setParticipantsCount(typeof participantsCountData === "number" ? participantsCountData : null);
        setStages(Array.isArray(stagesData) ? stagesData : []);
        setReviews(Array.isArray(reviewsData) ? reviewsData : []);
        setRatingSummary(ratingSummaryData);
      })
      .catch((err) => {
        setError("Failed to load course details.");
      })
      .finally(() => setLoading(false));
  }, [id]);

  if (loading) {
    return <div className="course-details-root"><div className="course-details-container"><p>Loading...</p></div></div>;
  }
  if (error || !course) {
    return <div className="course-details-root"><div className="course-details-container"><p>{error || "Course not found."}</p></div></div>;
  }

  return (
    <div className="course-details-root">
      <div className="course-details-container">
        {/* Main Info Grid */}
        <div className="course-details-main-grid">
          {/* Left: Photo + Buy */}
          <div className="course-details-photo-purchase">
            <div className="course-details-cover-card">
              <img
                src={course.imageUrl || "/placeholder.svg?height=200&width=300"}
                alt="Course cover"
                className="course-details-cover-img"
              />
            </div>
            <button className="course-details-buy-btn">
              Buy for {course.price ? `$${course.price}` : "Free"}
            </button>
          </div>

          {/* Right: Info */}
          <div className="course-details-info">
            <div>
              <h1 className="course-details-title">{course.name}</h1>
              <p className="course-details-desc">{course.description}</p>
            </div>
            <div>
              <div className="course-details-stats">
                <div className="course-details-stat">
                  <User size={16} />
                  <span>{instructor}</span>
                </div>
                <div className="course-details-stat">
                  <Clock size={16} />
                  <span>
                    {stages.length > 0
                      ? `${stages.reduce((acc, s) => acc + (s.duration || 0), 0)} min total`
                      : "-"}
                  </span>
                </div>
                <div className="course-details-stat">
                  <Users size={16} />
                  <span>{participantsCount !== null ? participantsCount.toLocaleString() : "-"} participants</span>
                </div>
              </div>
              <div className="course-details-rating-row">
                <div className="course-details-stars">
                  {[1, 2, 3, 4, 5].map((star) => (
                    <Star
                      key={star}
                      className={`course-details-star${ratingSummary && star <= Math.round(ratingSummary.averageRating) ? " filled" : ""}`}
                    />
                  ))}
                </div>
                <span className="course-details-rating-value">
                  {ratingSummary ? ratingSummary.averageRating.toFixed(2) : "-"}
                </span>
                <span className="course-details-rating-count">
                  ({ratingSummary ? ratingSummary.reviewCount.toLocaleString() : 0} reviews)
                </span>
              </div>
            </div>
          </div>
        </div>

        {/* Course Content */}
        <div className="course-details-card">
          <div className="course-details-card-header">
            <div className="course-details-card-title">Course Content</div>
            <div className="course-details-card-subtitle">
              {stages.length} module{stages.length !== 1 ? "s" : ""} • {stages.reduce((acc, s) => acc + (s.duration || 0), 0)} min
            </div>
          </div>
          <div className="course-details-card-content">
            <ul className="course-details-stages-list">
              {stages.length === 0 ? (
                <li className="course-details-stage-row">No modules yet.</li>
              ) : (
                stages.map((stage, idx) => {
                  const Icon = ICONS[idx % ICONS.length];
                  return (
                    <li className="course-details-stage-row" key={stage.id}>
                      <span className="course-details-stage-icon">
                        <Icon size={18} />
                      </span>
                      <div className="course-details-stage-info">
                        <div className="course-details-stage-title">{stage.name}</div>
                        <div className="course-details-stage-desc">{stage.description}</div>
                      </div>
                      <div className="course-details-stage-duration">
                        <Clock size={13} />
                        <span>{stage.duration} min</span>
                      </div>
                    </li>
                  );
                })
              )}
            </ul>
          </div>
        </div>

        {/* Reviews Section */}
        <div className="course-details-card">
          <div className="course-details-card-header">
            <div className="course-details-card-title">Student Reviews</div>
            <div className="course-details-card-subtitle">What our students are saying</div>
          </div>
          <div className="course-details-card-content">
            <div className="course-details-reviews-summary">
              <div className="course-details-reviews-summary-value">
                {ratingSummary ? ratingSummary.averageRating.toFixed(2) : "-"}
              </div>
              <div className="course-details-reviews-summary-stars">
                {[1, 2, 3, 4, 5].map((star) => (
                  <Star
                    key={star}
                    className={`course-details-star${ratingSummary && star <= Math.round(ratingSummary.averageRating) ? " filled" : ""}`}
                  />
                ))}
              </div>
              <div className="course-details-reviews-summary-count">
                {ratingSummary ? ratingSummary.reviewCount.toLocaleString() : 0} reviews
              </div>
            </div>
            <div className="course-details-reviews-list">
              {(reviews.filter(r => r.courseId === id)).length === 0 ? (
                <div className="course-details-review-item">No reviews yet.</div>
              ) : (
                reviews.filter(r => r.courseId === id).map((review) => (
                  <div className="course-details-review-item" key={review.id}>
                    <div className="course-details-review-header">
                      <span className="course-details-avatar">
                        {getInitials(review.userName)}
                      </span>
                      <div className="course-details-review-info">
                        <div className="course-details-review-name">{review.userName}</div>
                        <div className="course-details-review-date">
                          {new Date(review.createdAt).toLocaleDateString()}
                        </div>
                      </div>
                      <div className="course-details-review-stars">
                        {[1, 2, 3, 4, 5].map((star) => (
                          <Star
                            key={star}
                            className={`course-details-star${star <= review.rating ? " filled" : ""}`}
                            size={15}
                          />
                        ))}
                      </div>
                    </div>
                    <div className="course-details-review-comment">{review.comment}</div>
                  </div>
                ))
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default CourseDetails;
