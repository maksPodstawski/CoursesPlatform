import { getAllReviews } from "./reviewService";

export async function countToxicReviews(): Promise<number> {
    const allReviews = await getAllReviews();
    let toxicCount = 0;

    for (const review of allReviews) {
        const score = await checkToxicity(review.comment);
        if (score > 0.2) {
            toxicCount++;
        }
    }

    return toxicCount;
}

export async function checkToxicity(text: string): Promise<number> {
    const response = await fetch(
        "https://commentanalyzer.googleapis.com/v1alpha1/comments:analyze?key=KLUCZYK_API",
        {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                comment: { text },
                languages: ["pl"],
                requestedAttributes: { TOXICITY: {} },
            }),
        }
    );

    if (!response.ok) throw new Error("API request failed");

    const data = await response.json();
    return data.attributeScores?.TOXICITY?.summaryScore?.value ?? 0;
}
