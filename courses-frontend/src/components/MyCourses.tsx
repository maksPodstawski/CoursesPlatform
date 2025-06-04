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
		switch (filter) {
			case "in-progress":
				if (course) return true;
				return true; // TODO: Add proper filtering logic when progress tracking is implemented
			case "completed":
				return true; // TODO: Add proper filtering logic when progress tracking is implemented
			default:
				return true;
		}
	});

	return (
		<div className="courses-container">
			<h1>My Courses</h1>

			<div className="filter-buttons">
				<button type="button" className={filter === "all" ? "active" : ""} onClick={() => setFilter("all")}>
					All
				</button>
				<button type="button" className={filter === "in-progress" ? "active" : ""} onClick={() => setFilter("in-progress")}>
					In Progress
				</button>
				<button type="button" className={filter === "completed" ? "active" : ""} onClick={() => setFilter("completed")}>
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
