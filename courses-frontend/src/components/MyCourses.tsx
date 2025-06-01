import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { getPurchasedCourses } from "../services/courseService.ts";
import type { Course } from "../types/courses.ts";
import "../styles/MyCourses.css";

export function MyCourses() {
	const [courses, setCourses] = useState<Course[]>([]);
	const [filter, setFilter] = useState<"all" | "in-progress" | "completed">("all");
	const navigate = useNavigate();

	useEffect(() => {
		const fetchCourses = async () => {
			try {
				const data = await getPurchasedCourses();
				setCourses(data);
			} catch (error) {
				console.error("Failed to fetch courses", error);
			}
		};

		fetchCourses();
	}, []);

	const filteredCourses = courses.filter((course) => {
		return true;
	});

	return (
		<div className="courses-container">
			<h1>My Courses</h1>

			<div className="filter-buttons">
				<button className={filter === "all" ? "active" : ""} onClick={() => setFilter("all")}>
					All
				</button>
				<button className={filter === "in-progress" ? "active" : ""} onClick={() => setFilter("in-progress")}>
					In Progress
				</button>
				<button className={filter === "completed" ? "active" : ""} onClick={() => setFilter("completed")}>
					Completed
				</button>
			</div>

			<div className="courses-grid">
				{filteredCourses.map((course) => (
					<div
						key={course.id ?? course.courseId}
						className="course-card"
						onClick={() => navigate(`/course/${course.id ?? course.courseId}`)}
					>
						<img src={course.imageUrl} alt={course.name} />
						<h2>{course.name}</h2>
						<p>{course.description}</p>
					</div>
				))}
			</div>
		</div>
	);
}
