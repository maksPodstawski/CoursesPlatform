import { config } from "../config";

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
}