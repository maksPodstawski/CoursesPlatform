import { config } from "../config";
import { CreateReviewRequest } from "../types/courses";
import { fetchClient } from "./fetchClient";

export async function getReviewsByCourse(courseId: string) {
    const response = await fetch(`${config.apiBaseUrl}/api/reviews/course/${courseId}`, {
        credentials: "include"
    });
    if (!response.ok) {
        throw new Error("Nie udało się pobrać recenzji");
    }
    return await response.json();
}

export async function deleteReviews(reviewIds: string[]) {
    const response = await fetch(`${config.apiBaseUrl}/api/reviews/delete-many`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({ reviewIds }),
    });
    if (!response.ok) {
        throw new Error("Nie udało się usunąć recenzji");
    }

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
export async function getRatingSummary(courseId: string): Promise<{ averageRating: number; reviewCount: number }> {
	const response = await fetchClient.fetch(`/api/reviews/course/${courseId}/rating-summary`);

	if (!response.ok) {
		throw new Error("Nie udało się pobrać podsumowania ocen.");
	}

	return response.json();
}
export async function getUserReviewForCourse(courseId: string) {
	const response = await fetchClient.fetch(`/api/reviews/course/${courseId}/user`);
	if (response.status === 404) {
		return null; 
	}
	if (!response.ok) {
		throw new Error("Nie udało się pobrać recenzji użytkownika.");
	}
	return response.json();
}
export async function updateReview(id: string, data: CreateReviewRequest): Promise<void> {
  const response = await fetchClient.fetch(`/api/reviews/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify(data),
  });

  if (!response.ok) {
   
    const errorText = await response.text();
    throw new Error(`Failed to update review: ${response.status} - ${errorText}`);
  }
}

export async function deleteReview(id: string): Promise<void> {
	await fetchClient.fetch(`/api/reviews/${id}`, {
		method: "DELETE"
	});
}