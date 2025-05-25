import type { ReactNode } from "react";
import { useEffect, useState } from "react";
import { motion } from "framer-motion";
import { config } from "../config";

interface CourseCardProps {
  course: {
    id: string;
    name: string;
    description: string;
    imageUrl: string;
  };
  actions?: ReactNode;
}

const CourseCard = ({ course, actions }: CourseCardProps) => {
  const [rating, setRating] = useState<string | null>(null);

  useEffect(() => {
    const fetchRating = async () => {
      try {
        const url = config.apiEndpoints.avgRating.replace("{courseId}", course.id);
        const res = await fetch(`${config.apiBaseUrl}${url}`);
        if (!res.ok) throw new Error();
        const avg = await res.json();
        setRating(avg !== null ? Number(avg).toFixed(2) : null);
      } catch {
        setRating(null);
      }
    };
    fetchRating();
  }, [course.id]);

  return (
      <motion.div
          className="course-card"
          whileHover={{ scale: 1.03 }}
          transition={{ duration: 0.2 }}
      >
        <img src={course.imageUrl} alt={course.name} className="course-image" />
        <h3>{course.name}</h3>
        <p>
          <strong>Rating:</strong>{" "}
          {rating !== null && rating !== undefined ? rating : "No ratings yet"}
        </p>
        {actions}
      </motion.div>
  );
};

export default CourseCard;