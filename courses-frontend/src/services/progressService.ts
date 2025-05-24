import axios from 'axios';
import { API_URL } from '../config';

export interface Progress {
    id: string;
    userId: string;
    stageId: string;
    lastAccessedAt: string;
    isCompleted: boolean;
    startedAt: string;
    completedAt: string | null;
}

export interface StageWithProgress {
    id: string;
    name: string;
    description: string;
    videoPath: string;
    duration: number;
    order: number;
    isCompleted: boolean;
    startedAt: string | null;
    completedAt: string | null;
    lastAccessedAt: string | null;
}

export interface Course {
	courseId?: string;
	id?: string;
	name: string;
	description: string;
	imageUrl: string;
}

export const getProgressByStage = async (stageId: string): Promise<Progress | null> => {
    try {
        const response = await axios.get(`${API_URL}/api/progress/stage/${stageId}`, { withCredentials: true });
        return response.data as Progress;
    } catch (error) {
        console.error('Error fetching stage progress:', error);
        return null;
    }
};

export const getUserProgress = async (): Promise<Progress[]> => {
    try {
        const response = await axios.get(`${API_URL}/api/progress/user`, { withCredentials: true });
        return response.data as Progress[];
    } catch (error) {
        console.error('Error fetching user progress:', error);
        return [];
    }
};

export const getCourseStagesWithProgress = async (courseId: string): Promise<StageWithProgress[]> => {
    try {
        const response = await axios.get(`${API_URL}/api/progress/course/${courseId}`, { withCredentials: true });
        return response.data as StageWithProgress[];
    } catch (error) {
        console.error('Error fetching course stages with progress:', error);
        return [];
    }
};

export const markStageAsCompleted = async (stageId: string): Promise<boolean> => {
    try {
        await axios.post(`${API_URL}/api/progress/stage/${stageId}/complete`, {}, { withCredentials: true });
        return true;
    } catch (error) {
        console.error('Error marking stage as completed:', error);
        return false;
    }
};

export const startStage = async (stageId: string): Promise<Progress | null> => {
    try {
        const response = await axios.post(`${API_URL}/api/progress/stage/${stageId}/start`, {}, { withCredentials: true });
        return response.data as Progress;
    } catch (error) {
        console.error('Error starting stage:', error);
        return null;
    }
}; 