import { useEffect, useState } from "react";
import { useParams, Link, useNavigate } from "react-router-dom";
import { ArrowLeft, Play, BookOpen, Clock, Star, Video, Check, Lock } from "lucide-react";
import "../styles/CourseView.css";
import { getCourseById, getCourseInstructor } from "../services/courseService";
import { getCourseStagesWithProgress } from "../services/progressService";
import type { StageWithProgress as ApiStageWithProgress } from "../types/courses";

type Course = {
	id?: string;
	courseId?: string;
	name: string;
	description: string;
	imageUrl: string;
};

type CourseDetails = {
	id: string;
	title: string;
	description: string;
	thumbnail: string;
	instructor: {
		name: string;
		avatar: string;
		title: string;
	};
	totalStages: number;
	completedStages: number;
	totalDuration: number;
	progress: number;
	rating: number;
	reviews: number;
	currentStage: string;
	stages: {
		id: string;
		courseId: string;
		name: string;
		description: string;
		duration: number;
		videoPath: string;
		completed: boolean;
		locked: boolean;
		current: boolean;
		createdAt: string;
	}[];
};

const mapToCourseDetails = (
	course: Course,
	stages: ApiStageWithProgress[],
	instructor: { name: string; avatar: string; title: string },
): CourseDetails => {
	const completedStages = stages.filter((s) => s.isCompleted).length;
	const totalDuration = stages.reduce((sum, s) => sum + s.duration, 0);
	const progress = Math.round((completedStages / stages.length) * 100);
	const currentStageIndex = stages.findIndex((s) => !s.isCompleted);
	const currentStage = currentStageIndex >= 0 ? stages[currentStageIndex]?.id : stages[0]?.id;

	return {
		id: course.id || "",
		title: course.name,
		description: course.description,
		thumbnail: course.imageUrl,
		instructor,
		totalStages: stages.length,
		completedStages,
		totalDuration,
		progress,
		rating: 4.8,
		reviews: 324,
		currentStage,
		stages: stages.map((s, index) => ({
			id: s.id,
			courseId: course.id || "",
			name: s.name,
			description: s.description,
			duration: s.duration,
			videoPath: s.videoPath,
			completed: s.isCompleted,
			locked: !s.startedAt && index > 0 && !stages[index - 1]?.isCompleted,
			current: s.id === currentStage,
			createdAt: s.startedAt || new Date().toISOString(),
		})),
	};
};

const formatDuration = (minutes: number): string => {
	if (minutes < 60) {
		return `${minutes}m`;
	}
	const hours = Math.floor(minutes / 60);
	const remainingMinutes = minutes % 60;
	return remainingMinutes > 0 ? `${hours}h ${remainingMinutes}m` : `${hours}h`;
};

export default function CourseView() {
	const { id } = useParams<{ id: string }>();
	const [course, setCourse] = useState<CourseDetails | null>(null);
	const [loading, setLoading] = useState(true);

	const navigate = useNavigate();

	const goToStage = (stageId: string) => {
		navigate(`/course/${course?.id}/stage/${stageId}`);
	};

	useEffect(() => {
		const load = async () => {
			try {
				setLoading(true);
				const [courseData, stages, instructor] = await Promise.all([
					getCourseById(id!),
					getCourseStagesWithProgress(id!),
					getCourseInstructor(id!),
				]);
				const details = mapToCourseDetails(courseData, stages, instructor);
				setCourse(details);
			} catch (error) {
				console.error("Error loading course:", error);
			} finally {
				setLoading(false);
			}
		};

		if (id) load();
	}, [id]);

	if (loading || !course) return <div className="skeleton">Loading...</div>;

	return (
		<div className="course-container">
			<Link to="/my-courses" className="back-button">
				<ArrowLeft size={16} />
				Back to My Courses
			</Link>

			<div className="course-header">
				<div className="course-content">
					<h1>{course.title}</h1>
					<p className="description">{course.description}</p>

					<div className="course-meta">
						<span>
							<BookOpen size={16} /> {course.completedStages}/{course.totalStages} stages
						</span>
						<span>
							<Clock size={16} /> {formatDuration(course.totalDuration)}
						</span>
						<span>
							<Star size={16} /> {course.rating} ({course.reviews} reviews)
						</span>
					</div>

					<div className="instructor">
						<img src={course.instructor.avatar} alt={course.instructor.name} className="avatar" />
						<div className="instructor-info">
							<div className="name">{course.instructor.name}</div>
							<div className="title">{course.instructor.title}</div>
						</div>
					</div>

					<div className="progress-section">
						<div className="progress-header">
							<span className="progress-label">Course Progress</span>
							<span className="progress-percentage">{course.progress}%</span>
						</div>
						<div className="progress-bar">
							<div className="progress-fill" style={{ width: `${course.progress}%` }} />
						</div>
					</div>

					<div className="action-buttons">
						<button type="button" className="btn-primary" onClick={() => goToStage(course.currentStage)}>
							<Play size={16} />
							Continue Learning
						</button>
					</div>
				</div>

				<div className="thumbnail-container">
					{course.thumbnail ? (
						<img src={course.thumbnail} alt="Course thumbnail" className="thumbnail-image" />
					) : (
						<div className="thumbnail-placeholder">
							<Video size={48} />
							<span>Course Preview</span>
						</div>
					)}
				</div>
			</div>

			<div className="stages">
				<h2>
					<BookOpen size={24} />
					Course Stages
				</h2>
				<ul>
					{course.stages.map((stage, index) => (
						<li
							key={stage.id}
							className={`stage ${stage.completed ? "completed" : ""} ${stage.current ? "current" : ""} ${stage.locked ? "locked" : ""}`}
						>
							<div className="stage-icon">
								{stage.completed ? <Check size={20} /> : stage.locked ? <Lock size={20} /> : index + 1}
							</div>

							<div className="stage-content">
								<h3>
									Stage {index + 1}: {stage.name}
								</h3>
								<p>{stage.description}</p>
								<div className="stage-meta">
									<span>
										<Clock size={14} /> {formatDuration(stage.duration)}
									</span>
									<span>
										<Video size={14} /> Video content
									</span>
								</div>
							</div>

							<div className="stage-actions">
								<div className="stage-duration">{formatDuration(stage.duration)}</div>
								{stage.completed && (
									<button type="button" className="btn-review" onClick={() => goToStage(stage.id)}>
										<Play size={14} />
										Review
									</button>
								)}
								{!stage.completed && !stage.locked && (
									<button type="button" className="btn-review" onClick={() => goToStage(stage.id)}>
										<Play size={14} />
										Start
									</button>
								)}
							</div>
						</li>
					))}
				</ul>
			</div>
		</div>
	);
}
