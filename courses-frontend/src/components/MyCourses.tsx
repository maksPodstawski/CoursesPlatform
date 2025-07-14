import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { getPurchasedCourses } from "../services/courseService";
import type { Course } from "../types/courses";
import { Grid, List, BookOpen } from "lucide-react";
import { CourseCard, CourseCardSkeleton } from "../components/CourseCard";
import { getCourseInstructor } from "../services/courseService";
import { getRatingSummary } from "../services/reviewService";

import "../styles/MyCourses.css";

export function MyCourses() {
	const [courses, setCourses] = useState<Course[]>([]);
	const [filteredCourses, setFilteredCourses] = useState<Course[]>([]);
	const [filter, setFilter] = useState<"all" | "in-progress" | "completed">("all");
	const [loading, setLoading] = useState(true);
	const [searchTerm, setSearchTerm] = useState("");
	const [viewMode, setViewMode] = useState<"grid" | "list">("grid");
	const [ratings, setRatings] = useState<Record<string, string>>({});
	const navigate = useNavigate();

	useEffect(() => {
	const fetchCourses = async () => {
		try {
			const data = await getPurchasedCourses();

			const coursesWithInstructors = await Promise.all(
				data.map(async (course: Course) => {
					try {
						const instructor = await getCourseInstructor(course.id ?? course.courseId ?? "");
						return {
							...course,
							id: course.id ?? course.courseId ?? "",
							instructor: instructor || { name: "Unknown Instructor" },
						};
					} catch {
						return {
							...course,
							id: course.id ?? course.courseId ?? "",
							instructor: { name: "Unknown Instructor" },
						};
					}
				})
			);

			setCourses(coursesWithInstructors);
			setFilteredCourses(coursesWithInstructors);
			for (const course of coursesWithInstructors) {
				try {
					const summary = await getRatingSummary(course.id);
					if (summary && typeof summary.averageRating === "number") {
						setRatings((prev) => ({
							...prev,
							[course.id]: summary.averageRating.toFixed(1),
						}));
					}
				} catch (error) {
					console.error(`Failed to fetch rating for course ${course.id}`, error);
				}
			}
		} catch (error) {
			console.error("Failed to fetch courses", error);
		} finally {
			setLoading(false);
		}
	};

	fetchCourses();
}, []);

	// Apply filter and search
	useEffect(() => {
		let filtered = [...courses];

		// Search
		if (searchTerm) {
			filtered = filtered.filter(
				(course) =>
					course.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
					course.description.toLowerCase().includes(searchTerm.toLowerCase())
			);
		}

		// Filter progress
		if (filter === "in-progress") {
			filtered = filtered.filter((c) => c.isCompleted === false);
		} else if (filter === "completed") {
			filtered = filtered.filter((c) => c.isCompleted === true);
		}

		setFilteredCourses(filtered);
	}, [searchTerm, filter, courses]);

	const handleViewDetails = (courseId?: string) => {
		if (courseId) navigate(`/course/${courseId}`);
	};

	return (
		<div className="my-courses-page">
			{/* Header */}
			<div className="courses-header">
				<div className="courses-header-content">
					<h1 className="courses-title">My Courses</h1>
					<p className="courses-subtitle">Access and continue your learning journey</p>
				</div>
			</div>

			{/* Filters and Search */}
			<div className="courses-filters">
				<div className="filters-content">
					<div className="filters-controls">
						<div className="search-container">
							<input
								type="text"
								placeholder="Search my courses..."
								value={searchTerm}
								onChange={(e) => setSearchTerm(e.target.value)}
								className="search-input"
							/>
						</div>

						<div className="filter-buttons">
							<button
								type="button"
								className={filter === "all" ? "active" : ""}
								onClick={() => setFilter("all")}
							>
								All
							</button>
							<button
								type="button"
								className={filter === "in-progress" ? "active" : ""}
								onClick={() => setFilter("in-progress")}
							>
								In Progress
							</button>
							<button
								type="button"
								className={filter === "completed" ? "active" : ""}
								onClick={() => setFilter("completed")}
							>
								Completed
							</button>
						</div>

						<div className="view-toggle">
							<button
								className={`view-btn ${viewMode === "grid" ? "active" : ""}`}
								onClick={() => setViewMode("grid")}
							>
								<Grid size={20} />
							</button>
							<button
								className={`view-btn ${viewMode === "list" ? "active" : ""}`}
								onClick={() => setViewMode("list")}
							>
								<List size={20} />
							</button>
						</div>
					</div>
				</div>
			</div>

			{/* Course Grid */}
			<div className="courses-container">
				{loading ? (
					<div className={`courses-grid ${viewMode === "list" ? "list-view" : ""}`}>
						{[...Array(6)].map((_, i) => (
							<CourseCardSkeleton key={i} />
						))}
					</div>
				) : filteredCourses.length === 0 ? (
					<div className="no-courses">
						<BookOpen className="no-courses-icon" size={80} />
						<h3 className="no-courses-title">No courses found</h3>
						<p className="no-courses-text">Try adjusting your search or filters</p>
					</div>
				) : (
					<>
						<div className="courses-info">
							<p className="courses-count">
								Showing {filteredCourses.length} of {courses.length} courses
							</p>
						</div>

						<div className={`courses-grid ${viewMode === "list" ? "list-view" : ""}`}>
							{filteredCourses.map((course) => {
							const courseId = course.id ?? course.courseId;

							if (!courseId) return null;

							return (
								<CourseCard
									key={courseId}
									course={{
										...course,
										id: courseId,
										instructor: course.instructor?.name ?? "Unknown Instructor",
									}}
									rating={ratings[courseId]} 
									actions={
										<button
											onClick={() => handleViewDetails(courseId)}
											className="btn btn-primary"
										>
											Continue
										</button>
									}
									variant="my-courses"
								/>
							);
						})}
						</div>
					</>
				)}
			</div>
		</div>
	);
}

export default MyCourses;
