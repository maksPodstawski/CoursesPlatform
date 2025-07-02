import CourseView from "../components/CourseView.tsx";
import "../styles/CourseViewPage.css";

export const CoursePage = () => {
	return (
		<main className="flex-1 overflow-hidden bg-[#1a1a1a] course-view-main">
			<CourseView />
		</main>
	);
};
