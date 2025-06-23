import { useEffect, useState } from 'react';
import { getCourses, getCourseInstructor, getCourseParticipantsCount } from '../services/courseService';
import { getRatingSummary } from '../services/reviewService';
import { useNavigate } from "react-router-dom";
import BuyButton from "../components/BuyButton.tsx";
import { Search, Grid, List, BookOpen } from 'lucide-react';
import '../styles/Courses.css';
import { CourseCard, CourseCardSkeleton } from '../components/CourseCard';

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

const Courses = () => {
	const [courses, setCourses] = useState<Course[]>([]);
	const [filteredCourses, setFilteredCourses] = useState<Course[]>([]);
	const [loading, setLoading] = useState(true);
	const [searchTerm, setSearchTerm] = useState("");
	const [selectedCategory, setSelectedCategory] = useState("all");
	const [selectedLevel, setSelectedLevel] = useState("all");
	const [viewMode, setViewMode] = useState<"grid" | "list">("grid");
	const [ratings, setRatings] = useState<Record<string, string>>({});
	const navigate = useNavigate();

	const fetchCourses = async () => {
		try {
			const data = await getCourses();
			const enhancedData = await Promise.all(
				data.map(async (course: any) => {
					const instructor = await getCourseInstructor(course.id);
					const studentsCount = await getCourseParticipantsCount(course.id);
					return {
						...course,
						instructor: instructor.name || "N/A",
						duration: course.duration || "8 hours",
						studentsCount: studentsCount,
						level: course.level || ["Beginner", "Intermediate", "Advanced"][Math.floor(Math.random() * 3)],
						category: course.category || "Programming"
					};
				})
			);
			setCourses(enhancedData);
			setFilteredCourses(enhancedData);
		} catch (error) {
			console.error('Error:', error);
		} finally {
			setLoading(false);
		}
	};

	const fetchRating = async (courseId: string) => {
		try {
			const summary = await getRatingSummary(courseId);
			if (summary && typeof summary.averageRating === 'number') {
				setRatings((prev) => ({ ...prev, [courseId]: summary.averageRating.toFixed(1) }));
			}
		} catch (error) {
			console.error(`Error fetching rating for course ${courseId}:`, error);
		}
	};

	useEffect(() => {
		fetchCourses();
	}, []);

	useEffect(() => {
		courses.forEach((course) => {
			fetchRating(course.id);
		});
	}, [courses]);

	useEffect(() => {
		let filtered = courses;

		// Filter by search term
		if (searchTerm) {
			filtered = filtered.filter(
				(course) =>
					course.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
					course.description.toLowerCase().includes(searchTerm.toLowerCase()) ||
					course.instructor?.toLowerCase().includes(searchTerm.toLowerCase()),
			);
		}

		// Filter by category
		if (selectedCategory !== "all") {
			filtered = filtered.filter((course) => course.category === selectedCategory);
		}

		// Filter by level
		if (selectedLevel !== "all") {
			filtered = filtered.filter((course) => course.level === selectedLevel);
		}

		setFilteredCourses(filtered);
	}, [courses, searchTerm, selectedCategory, selectedLevel]);

	const categories = Array.from(new Set(courses.map((course) => course.category).filter(Boolean)));
	const levels = ["Beginner", "Intermediate", "Advanced"];

	const handleViewDetails = (courseId: string) => {
		navigate(`/courses/${courseId}`);
	};

	return (
		<div className="courses-page">
			{/* Header */}
			<div className="courses-header">
				<div className="courses-header-content">
					<h1 className="courses-title">Our Courses</h1>
					<p className="courses-subtitle">Discover and learn from our comprehensive course library</p>
				</div>
			</div>

			{/* Filters and Search */}
			<div className="courses-filters">
				<div className="filters-content">
					<div className="filters-controls">
						<div className="search-container">
							<Search className="search-icon" size={20} />
							<input
								type="text"
								placeholder="Search courses..."
								value={searchTerm}
								onChange={(e) => setSearchTerm(e.target.value)}
								className="search-input"
							/>
						</div>

						<select
							value={selectedCategory}
							onChange={(e) => setSelectedCategory(e.target.value)}
							className="filter-select"
						>
							<option value="all">All Categories</option>
							{categories.map((category) => (
								<option key={category} value={category}>
									{category}
								</option>
							))}
						</select>

						<select
							value={selectedLevel}
							onChange={(e) => setSelectedLevel(e.target.value)}
							className="filter-select"
						>
							<option value="all">All Levels</option>
							{levels.map((level) => (
								<option key={level} value={level}>
									{level}
								</option>
							))}
						</select>

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
						<p className="no-courses-text">Try adjusting your search or filter criteria</p>
					</div>
				) : (
					<>
						<div className="courses-info">
							<p className="courses-count">
								Showing {filteredCourses.length} of {courses.length} courses
							</p>
						</div>

						<div className={`courses-grid ${viewMode === "list" ? "list-view" : ""}`}>
							{filteredCourses.map((course) => (
								<CourseCard
									key={course.id}
									course={course}
									rating={ratings[course.id]}
									actions={
										<>
											<BuyButton courseId={course.id} price={course.price} redirectAfterLogin="/courses" />
											<button
												onClick={() => handleViewDetails(course.id)}
												className="btn btn-primary"
											>
												Details
											</button>
										</>
									}
								/>
							))}
						</div>
					</>
				)}
			</div>
		</div>
	);
};

export default Courses;