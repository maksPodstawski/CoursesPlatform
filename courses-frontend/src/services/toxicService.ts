import { config } from "../config";
import {fetchClient} from "./fetchClient.ts";

export async function checkToxicity(comment: string): Promise<number> {
    const response = await fetchClient.fetch(`${config.apiBaseUrl}/api/admin/analyze-toxicity`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ comment }),
    });

    if (!response.ok) return 0;

    const data = await response.json();
    return data.score ?? 0;
}

export async function countToxicReviews(): Promise<number> {
    const response = await fetchClient.fetch(`${config.apiBaseUrl}/api/admin/toxic-review-count`);

    if (!response.ok) return 0;

    const data = await response.json();
    return data.count ?? 0;
}


