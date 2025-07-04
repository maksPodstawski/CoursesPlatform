export function getDifficultyLabel(
  difficulty: number | null | undefined
): "Beginner" | "Intermediate" | "Advanced" | undefined {
  switch (difficulty) {
    case 1:
      return "Beginner";
    case 2:
      return "Intermediate";
    case 3:
      return "Advanced";
    default:
      return undefined;
  }
}