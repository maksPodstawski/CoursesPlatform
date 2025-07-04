import { config } from '../config';
import { fetchClient } from './fetchClient';

export async function getCourses() {
	const response = await fetchClient.fetch(config.apiEndpoints.getCourses);
	return await response.json();
}
/*
export async function getCoursesByTitle(title: string) {
	const url = new URL(`${config.apiBaseUrl}${config.apiEndpoints.getCourses}`);
	url.searchParams.append('title', title);

	const response = await fetch(url.toString());
	if (!response.ok) {
		throw new Error('Failed to download courses by title');
	}
	return await response.json();
}

export async function getCoursesByPriceRange(min: number, max: number) {
	const url = new URL(`${config.apiBaseUrl}${config.apiEndpoints.getCoursesByPriceRange}`);
	url.searchParams.append('min', min.toString());
	url.searchParams.append('max', max.toString());

	const response = await fetch(url.toString());
	if (!response.ok) {
		throw new Error('Failed to download courses in the specified price range');
	}
	return await response.json();
}*/

export async function getPurchasedCourses() {
	const response = await fetchClient.fetch('/api/courses/purchases/user');
	return await response.json();
}

export async function getStagesByCourse(courseId: string) {
	const response = await fetchClient.fetch(`/api/stages/course/${courseId}`);
	return await response.json();
}

export function getStageVideoStreamUrl(stageId: string) {
	return `${config.apiBaseUrl}/api/stages/${stageId}/video/stream`;
}

export async function getCourseById(courseId: string) {
	const response = await fetchClient.fetch(`/api/courses/${courseId}`);
	return await response.json();
}

export async function getCourseInstructor(courseId: string) {
	const response = await fetchClient.fetch(`/api/courses/${courseId}/instructor`);
	return await response.json();
}

export async function getCourseParticipantsCount(courseId: string) {
	const response = await fetchClient.fetch(`/api/courses/purchases/course/${courseId}/count`);
	if (!response.ok) {
		throw new Error('Failed to fetch course participants count');
	}
	return await response.json();
}

export interface UpdateCourseData {
	name: string;
	description?: string;
	imageUrl: string;
	duration: number;
	price: number;
	isHidden: boolean;
	difficulty: number;
}

export async function updateCourse(courseId: string, courseData: UpdateCourseData) {
	const response = await fetchClient.fetch(`/api/courses/${courseId}`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json',
		},
		body: JSON.stringify(courseData),
	});
	
	if (!response.ok) {
		throw new Error('Failed to update course');
	}
	
	return await response.json();
}
