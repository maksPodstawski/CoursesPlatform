import { CreateReviewRequest } from "../types/courses";
import { fetchClient } from "./fetchClient";



export async function createReview(data: CreateReviewRequest): Promise<void> {
	await fetchClient.fetch(`/api/reviews`, {
		method: "POST",
		headers: {
			"Content-Type": "application/json"
		},
		body: JSON.stringify(data)
	});
}

export async function getReviewsByCourse(courseId: string) {
	const response = await fetchClient.fetch(`/api/reviews?courseId=${courseId}`);
	return response.json(); 
}