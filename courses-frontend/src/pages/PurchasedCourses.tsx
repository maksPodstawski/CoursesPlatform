import { useEffect, useState } from "react";
import ReactPlayer from "react-player";
import { getPurchasedCourses, getStageVideoStreamUrl } from "../services/courseService";
import {
	type Course,
	getCourseStagesWithProgress,
	markStageAsCompleted,
	type StageWithProgress,
	startStage,
} from "../services/progressService";
import "./PurchasedCourses.css";

const PurchasedCourses = () => {
	const [courses, setCourses] = useState<Course[]>([]);
	const [loading, setLoading] = useState(true);
	const [selectedCourse, setSelectedCourse] = useState<Course | null>(null);
	const [stages, setStages] = useState<StageWithProgress[]>([]);
	const [selectedStage, setSelectedStage] = useState<StageWithProgress | null>(null);

	useEffect(() => {
		const fetchPurchasedCourses = async () => {
			try {
				const data = await getPurchasedCourses();
				setCourses(data);
			} catch (error) {
				console.error("Error fetching courses:", error);
			} finally {
				setLoading(false);
			}
		};
		fetchPurchasedCourses();
	}, []);

	useEffect(() => {
		const fetchStagesWithProgress = async () => {
			if (selectedCourse) {
				setStages([]);
				setSelectedStage(null);
				try {
					const courseId = selectedCourse.courseId || selectedCourse.id;
					if (!courseId) throw new Error("No course ID found");
					const stagesData = await getCourseStagesWithProgress(courseId);
					setStages(stagesData);
				} catch (error) {
					console.error("Error loading stages:", error);
				}
			}
		};
		fetchStagesWithProgress();
	}, [selectedCourse]);

	const handleStageSelect = (stage: StageWithProgress) => {
		setSelectedStage(stage);
	};

	const handleVideoStart = async () => {
		if (!selectedStage) return;
		try {
			await startStage(selectedStage.id);
		} catch (error) {
			console.error("Error starting stage:", error);
		}
	};

	const handlePrevStage = () => {
		if (!selectedStage || stages.length === 0) return;
		const idx = stages.findIndex((s) => s.id === selectedStage.id);
		if (idx > 0) setSelectedStage(stages[idx - 1]);
	};

	const handleNextStage = () => {
		if (!selectedStage || stages.length === 0) return;
		const idx = stages.findIndex((s) => s.id === selectedStage.id);
		if (idx < stages.length - 1) setSelectedStage(stages[idx + 1]);
	};

	const handleVideoEnded = async () => {
		if (selectedStage && !selectedStage.isCompleted) {
			try {
				await markStageAsCompleted(selectedStage.id);
				setStages(
					stages.map((stage) =>
						stage.id === selectedStage.id
							? { ...stage, isCompleted: true, completedAt: new Date().toISOString() }
							: stage,
					),
				);
				setSelectedStage({ ...selectedStage, isCompleted: true, completedAt: new Date().toISOString() });
			} catch (error) {
				console.error("Error marking stage as completed:", error);
			}
		}
	};

	const completedCount = stages.filter((stage) => stage.isCompleted).length;
	const progress = stages.length > 0 ? (completedCount / stages.length) * 100 : 0;

	useEffect(() => {
		if (stages.length > 0) {
			setSelectedStage(stages[0]);
		} else {
			setSelectedStage(null);
		}
	}, [stages]);

	return (
		<div className="purchased-courses-root">
			{!selectedCourse && !loading && (
				<div className="purchased-courses-picker">
					<h2 className="purchased-courses-picker-title">Select a course to continue</h2>
					<div className="purchased-courses-picker-list">
						{courses.map((course: Course) => (
							<div
								key={course.courseId || course.id}
								className="purchased-courses-picker-card"
								onClick={() => setSelectedCourse(course)}
							>
								<img src={course.imageUrl} alt={course.name} />
								<h3>{course.name}</h3>
								<div className="purchased-courses-picker-card-desc">{course.description}</div>
							</div>
						))}
					</div>
				</div>
			)}
			{selectedCourse && (
				<>
					<aside className="purchased-courses-sidebar">
						<h2 className="purchased-courses-sidebar-title">{selectedCourse ? selectedCourse.name : "Course"}</h2>
						<div style={{ marginBottom: 24 }}>
							<div className="purchased-courses-sidebar-content-title">Course Content</div>
							<ul className="purchased-courses-sidebar-list">
								{stages.map((stage) => (
									<li
										key={stage.id}
										className={`purchased-courses-sidebar-list-item${selectedStage && selectedStage.id === stage.id ? " selected" : ""}${stage.isCompleted ? " completed" : ""}`}
										onClick={() => handleStageSelect(stage)}
									>
										<span className="purchased-courses-sidebar-list-item-dot" />
										<span style={{ flex: 1 }}>{stage.name}</span>
										<span className="purchased-courses-sidebar-list-item-duration">
											{stage.duration ? `${stage.duration.toFixed(2)} min` : ""}
										</span>
									</li>
								))}
							</ul>
						</div>
						<div className="purchased-courses-sidebar-stage-index">
							{selectedStage && stages.length > 0 && (
								<>
									Stage {stages.findIndex((s) => s.id === selectedStage.id) + 1} of {stages.length}
								</>
							)}
						</div>
						<button
							type="button"
							className="purchased-courses-sidebar-change-btn"
							onClick={() => setSelectedCourse(null)}
						>
							← Change Course
						</button>
					</aside>
					<main className="purchased-courses-main">
						{selectedStage && (
							<>
								<div className="purchased-courses-main-header">
									<div>
										<h1 className="purchased-courses-main-title">{selectedStage.name}</h1>
										<div className="purchased-courses-main-desc">{selectedStage.description}</div>
									</div>
									<div className="purchased-courses-main-progress-label">
										{completedCount}/{stages.length} completed
									</div>
								</div>
								<div className="purchased-courses-main-progress-bar">
									<div className="purchased-courses-main-progress-bar-inner" style={{ width: `${progress}%` }} />
								</div>
								<div className="purchased-courses-main-video-wrap">
									<div className="custom-video-player-wrapper">
										<ReactPlayer
											url={getStageVideoStreamUrl(selectedStage.id)}
											controls={true}
											width="90%"
											height="80%"
											config={{ file: { attributes: { controlsList: "nodownload" } } }}
											onEnded={handleVideoEnded}
											onPlay={handleVideoStart}
										/>
									</div>
								</div>
								<div className="purchased-courses-main-nav">
									<button
										type="button"
										className="purchased-courses-main-nav-btn"
										style={{ opacity: stages.findIndex((s) => s.id === selectedStage.id) === 0 ? 0.5 : 1 }}
										onClick={handlePrevStage}
										disabled={stages.findIndex((s) => s.id === selectedStage.id) === 0}
									>
										&#8592; Previous Stage
									</button>
									<div className="purchased-courses-main-stage-index">
										Stage {stages.findIndex((s) => s.id === selectedStage.id) + 1} of {stages.length}
									</div>
									<button
										className="purchased-courses-main-nav-btn"
										type="button"
										style={{
											opacity: stages.findIndex((s) => s.id === selectedStage.id) === stages.length - 1 ? 0.5 : 1,
										}}
										onClick={handleNextStage}
										disabled={stages.findIndex((s) => s.id === selectedStage.id) === stages.length - 1}
									>
										Next Stage &#8594;
									</button>
								</div>
							</>
						)}
						{!selectedStage && (
							<div className="purchased-courses-main-video-placeholder">Select a stage to start learning.</div>
						)}
					</main>
				</>
			)}
			{loading && <div className="purchased-courses-loading">Loading...</div>}
		</div>
	);
};

export default PurchasedCourses;
