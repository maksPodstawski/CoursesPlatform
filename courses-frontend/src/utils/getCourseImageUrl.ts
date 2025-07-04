import { config } from "../config";

export function getCourseImageUrl(imageUrl: string): string {
  const match = imageUrl.match(/\/UploadedImages\/(.*?)\/(.*)$/) || imageUrl.match(/\/UploadedImages\/(.*?)\/(.*)$/);
  if (!match) return "/placeholder.svg";
  const courseId = match[1];
  const fileName = match[2];
  return `${config.apiBaseUrl}/api/courses/${courseId}/image/${encodeURIComponent(fileName)}`;
} 