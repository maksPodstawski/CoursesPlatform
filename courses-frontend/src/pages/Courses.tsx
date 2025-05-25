import { useEffect, useState } from 'react';
import { getCourses } from '../services/courseService';
import { useNavigate } from "react-router-dom";
import CourseCard from "../components/CourseCard.tsx";
import BuyButton from "../components/BuyButton.tsx";

interface Course {
	id: string;
	name: string;
	description: string;
	price: number;
	imageUrl: string;
}

const Courses = () => {
	const [courses, setCourses] = useState<Course[]>([]);
	const [loading, setLoading] = useState(true);
	const navigate = useNavigate();

	useEffect(() => {
		const fetchCourses = async () => {
			try {
				const data = await getCourses();
				setCourses(data);
			} catch (error) {
				console.error('Error:', error);
			} finally {
				setLoading(false);
			}
		};
		fetchCourses();
	}, []);


	return (
		<div>
			<h1>Courses</h1>
			{loading ? <p>Loading...</p> : (
				<div className="courses-grid">
					{courses.map((course: Course) => (
						<CourseCard
							key={course.id}
							course={course}
							actions={
								<div style={{ display: "flex", gap: "8px", marginTop: "8px" }}>
									<BuyButton courseId={course.id} price={course.price} redirectAfterLogin="/courses" />
									<button type="button" className="btn btn-primary"
											onClick={() => navigate(`/courses/${course.id}`)}>
										Details
									</button>
								</div>
							}
						/>
					))}
				</div>
			)}
		</div>
	);
};

export default Courses;