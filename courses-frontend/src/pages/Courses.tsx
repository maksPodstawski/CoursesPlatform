import { useEffect, useState } from 'react';
import { getCourses } from '../services/courseService';

const Courses = () => {
	const [courses, setCourses] = useState([]);
	const [loading, setLoading] = useState(true);

	useEffect(() => {
		const fetchCourses = async () => {
			try {
				const data = await getCourses();
				setCourses(data);
			} catch (error) {
				console.error('Błąd:', error);
			} finally {
				setLoading(false);
			}
		};

		fetchCourses();
	}, []);

	return (
		<div>
			<h1>Kursy</h1>
			{loading ? <p>Ładowanie...</p> : (
				<ul>
					{courses.map((course: any) => (
						<li key={course.id}>
							<h2>{course.name}</h2>
							<p>{course.description}</p>
							<img src={course.imageUrl} style={{ width: "200px", height: "200px", objectFit: "cover" }} />
						</li>
					))}
				</ul>
			)}
		</div>
	);
};

export default Courses;
