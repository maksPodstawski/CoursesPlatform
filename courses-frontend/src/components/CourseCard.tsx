import { Link } from "react-router-dom";
import { motion } from "framer-motion";

const CourseCard = ({ course }: { course: any }) => (
  <motion.div
    className="course-card"
    whileHover={{ scale: 1.03 }}
    transition={{ duration: 0.2 }}
  >
    <img src={course.imageUrl} alt={course.name} className="course-image" />
    <h3>{course.name}</h3>
    <p>{course.description}</p>
    <Link to="/courses" className="btn btn-primary">Explore</Link>
  </motion.div>
);

export default CourseCard;
