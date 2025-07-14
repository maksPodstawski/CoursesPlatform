import { fetchClient } from "./fetchClient";

export async function getStatistics(): Promise<{ usersCount: number; coursesCount: number; reviewsCount: number }> {
  const response = await fetchClient.fetch("/api/statistics");
  if (!response.ok) {
    throw new Error("Nie udało się pobrać statystyk.");
  }
  return response.json();
}
