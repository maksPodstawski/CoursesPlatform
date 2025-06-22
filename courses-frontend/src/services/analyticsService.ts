import { fetchClient } from "./fetchClient";

export interface CreatorAnalytics {
	creatorId: string;
	creatorName: string;
	totalCourses: number;
	totalStudents: number;
	totalRevenue: number;
	averageRating: number;
	totalReviews: number;
	totalStages: number;
	completedStages: number;
	courses: CourseAnalytics[];
	monthlyRevenue: MonthlyRevenue[];
	topPerformingCourses: CoursePerformance[];
}

export interface CourseAnalytics {
	courseId: string;
	courseName: string;
	studentsCount: number;
	revenue: number;
	averageRating: number;
	reviewsCount: number;
	stagesCount: number;
	completedStagesCount: number;
	createdAt: string;
	stages: StageAnalytics[];
}

export interface StageAnalytics {
	stageId: string;
	stageName: string;
	studentsStarted: number;
	studentsCompleted: number;
	completionRate: number;
	averageTimeToComplete: number;
}

export interface MonthlyRevenue {
	year: number;
	month: number;
	revenue: number;
	salesCount: number;
}

export interface CoursePerformance {
	courseId: string;
	courseName: string;
	studentsCount: number;
	revenue: number;
	averageRating: number;
	completionRate: number;
}

export const analyticsService = {
	async getMyAnalytics(year: number): Promise<CreatorAnalytics> {
		const response = await fetchClient.fetch(`/api/analytics/my-analytics?year=${year}`);
		return response.json();
	},

	async getCreatorAnalytics(creatorId: string, year: number): Promise<CreatorAnalytics> {
		const response = await fetchClient.fetch(`/api/analytics/creator/${creatorId}?year=${year}`);
		return response.json();
	},

	async getMyCourseAnalytics(): Promise<CourseAnalytics[]> {
		const response = await fetchClient.fetch("/api/analytics/courses");
		return response.json();
	},

	async getMonthlyRevenue(year: number): Promise<MonthlyRevenue[]> {
		const response = await fetchClient.fetch(`/api/analytics/monthly-revenue/${year}`);
		return response.json();
	},

	async getTopPerformingCourses(limit: number = 5): Promise<CoursePerformance[]> {
		const response = await fetchClient.fetch(`/api/analytics/top-performing-courses?limit=${limit}`);
		return response.json();
	},
};
