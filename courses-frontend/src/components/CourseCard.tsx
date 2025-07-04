import { motion } from "framer-motion";
import type { ReactNode, MouseEvent } from "react";
import { useNavigate } from "react-router-dom";
import { Star, Clock, Users } from 'lucide-react';
import '../styles/CourseCard.css';
import { getCourseImageUrl } from "../utils/getCourseImageUrl";

interface Course {
	id: string;
	name: string;
	description: string;
	price: number;
	imageUrl: string;
	instructor?: string;
	duration?: string;
	studentsCount?: number;
	level?: "Beginner" | "Intermediate" | "Advanced";
	category?: string;
}

interface CourseCardProps {
  course: Course;
  rating?: string;
  actions: ReactNode;
}

export const CourseCard = ({ course, rating, actions }: CourseCardProps) => {
  const navigate = useNavigate();

  // Zapobiegaj nawigacji jeśli kliknięto w przycisk w actions
  const handleCardClick = (e: MouseEvent<HTMLDivElement>) => {
    // Jeśli kliknięto w element z .course-actions lub jego potomka, nie nawiguj
    if ((e.target as HTMLElement).closest('.course-actions')) return;
    navigate(`/courses/${course.id}`);
  };

  return (
    <motion.div
      className="course-card"
      whileHover={{ y: -5, transition: { duration: 0.2 } }}
      onClick={handleCardClick}
      style={{ cursor: 'pointer' }}
    >
      <div className="course-card-header">
        <div className="course-image-container">
          <img
            src={getCourseImageUrl(course?.imageUrl || "")}
            alt={course?.name}
            className="course-image"
          />
          {course?.level && (
            <span className={`level-badge ${course.level.toLowerCase()}`}>
              {course.level}
            </span>
          )}
        </div>
      </div>

      <div className="course-card-content">
        <h3 className="course-title">{course?.name}</h3>
        <span className="course-price">${course?.price}</span>
        <div className="course-meta">
          {course?.instructor && (
            <div className="meta-item">
              <Users className="meta-icon" size={16} />
              {course.instructor}
            </div>
          )}
          {course?.duration && (
            <div className="meta-item">
              <Clock className="meta-icon" size={16} />
              {course.duration}
            </div>
          )}
          <div className="meta-item">
            <Star className="meta-icon star" size={16} strokeWidth={1.5} />
            {rating ? `${rating} rating` : "No ratings yet"}
          </div>
          {course?.studentsCount && (
            <div className="students-count">
              {course.studentsCount.toLocaleString()} participants
            </div>
          )}
        </div>
      </div>
      
      <div className="course-card-footer">
        <div className="course-actions">
          {actions}
        </div>
      </div>
    </motion.div>
  );
};

export const CourseCardSkeleton = () => (
    <div className="course-card skeleton">
        <div className="skeleton-image" />
        <div className="course-card-content">
            <div className="skeleton-title" />
            <div className="skeleton-text" />
            <div className="skeleton-text" />
        </div>
        <div className="course-card-footer">
            <div className="skeleton-button" />
            <div className="skeleton-button" />
        </div>
    </div>
);