import { useParams, Navigate } from "react-router-dom";
import { useState, useEffect } from "react";
import { StagePlayer } from "../components/StagePlayer";
import { getCourseById } from "../services/courseService";
import { getCourseStagesWithProgress, markStageAsCompleted } from "../services/progressService";
import type { Stage } from "../types/courses";

const StagePlayerPage = () => {
	const { id: courseId, stageId } = useParams();
	const [stage, setStage] = useState<Stage | null>(null);
	const [courseName, setCourseName] = useState("");
	const [allStages, setAllStages] = useState<Stage[]>([]);
	const [loading, setLoading] = useState(true);
	const [error, setError] = useState<string | null>(null);

	useEffect(() => {
		const fetchData = async () => {
			if (!courseId || !stageId) return;

			try {
				setLoading(true);
				setError(null);

				const courseData = await getCourseById(courseId);
				setCourseName(courseData.name);

				const stagesWithProgress = await getCourseStagesWithProgress(courseId);
				console.log("Stages with progress:", stagesWithProgress);
				setAllStages(stagesWithProgress);

				const currentStage = stagesWithProgress.find((s) => s.id === stageId);
				console.log("Current stage:", currentStage);
				if (!currentStage) {
					throw new Error("Stage not found");
				}
				setStage(currentStage);

				setLoading(false);
			} catch (error) {
				console.error("Error fetching stage data:", error);
				setError(error instanceof Error ? error.message : "Failed to load stage data");
				setLoading(false);
			}
		};

		fetchData();
	}, [courseId, stageId]);

	const handleStageComplete = async () => {
		if (!stageId) return;

		try {
			console.log("Marking stage as completed:", stageId);
			const success = await markStageAsCompleted(stageId);
			if (success) {
				setStage((prev: Stage | null) => (prev ? { ...prev, isCompleted: true } : null));
				setAllStages((prev: Stage[]) => prev.map((s) => (s.id === stageId ? { ...s, isCompleted: true } : s)));
			}
		} catch (error) {
			console.error("Error marking stage as completed:", error);
		}
	};

	if (!courseId || !stageId) {
		return <Navigate to="/courses" replace />;
	}

	if (loading) {
		return <div>Loading...</div>;
	}

	if (error) {
		return <div>Error: {error}</div>;
	}

	if (!stage) {
		return <Navigate to="/courses" replace />;
	}

	const currentStageIndex = allStages.findIndex((s) => s.id === stageId);
	const nextStageId = currentStageIndex < allStages.length - 1 ? allStages[currentStageIndex + 1].id : undefined;
	const prevStageId = currentStageIndex > 0 ? allStages[currentStageIndex - 1].id : undefined;

	return (
		<main>
			<StagePlayer
				stage={stage}
				courseId={courseId}
				courseName={courseName}
				allStages={allStages}
				nextStageId={nextStageId}
				prevStageId={prevStageId}
				onComplete={handleStageComplete}
			/>
		</main>
	);
};

export default StagePlayerPage;
