import { fetchClient } from "./fetchClient";
import type { Progress, StageWithProgress } from "../types/courses.ts";

export const getProgressByStage = async (stageId: string): Promise<Progress | null> => {
	try {
		const response = await fetchClient.fetch(`/api/progress/stage/${stageId}`);
		return await response.json() as Progress;
	} catch (error) {
		console.error("Error fetching stage progress:", error);
		return null;
	}
};

export const getUserProgress = async (): Promise<Progress[]> => {
	try {
		const response = await fetchClient.fetch("/api/progress/user");
		return await response.json() as Progress[];
	} catch (error) {
		console.error("Error fetching user progress:", error);
		return [];
	}
};

export const getCourseStagesWithProgress = async (courseId: string): Promise<StageWithProgress[]> => {
	try {
		const response = await fetchClient.fetch(`/api/progress/course/${courseId}`);
		return await response.json() as StageWithProgress[];
	} catch (error) {
		console.error("Error fetching course stages with progress:", error);
		return [];
	}
};

export const markStageAsCompleted = async (stageId: string): Promise<boolean> => {
	try {
		await fetchClient.fetch(`/api/progress/stage/${stageId}/complete`, {
			method: "POST"
		});
		return true;
	} catch (error) {
		console.error("Error marking stage as completed:", error);
		return false;
	}
};

export const startStage = async (stageId: string): Promise<Progress | null> => {
	try {
		const response = await fetchClient.fetch(`/api/progress/stage/${stageId}/start`, {
			method: "POST"
		});
		return await response.json() as Progress;
	} catch (error) {
		console.error("Error starting stage:", error);
		return null;
	}
};
