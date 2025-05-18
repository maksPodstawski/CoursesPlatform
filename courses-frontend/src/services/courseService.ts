import { config } from '../config';

export async function getCourses() {
	const response = await fetch(`${config.apiBaseUrl}${config.apiEndpoints.getCourses}`);
	if (!response.ok) {
		throw new Error('Failed to download courses');
	}
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