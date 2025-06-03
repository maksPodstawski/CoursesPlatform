import { Routes, Route, useLocation } from "react-router-dom";
import { AnimatePresence } from "framer-motion";
import { PageWrapper } from "../utils/animations";
import ProtectedRoute from "../components/ProtectedRoute";
import { Login } from "../pages/Login";
import { Register } from "../pages/Register";
import HomeContent from "../pages/HomeContent";
import Courses from "../pages/Courses";
import { AddCourse } from "../pages/AddCourse.tsx";
import CreatorCourses from "../pages/CreatorCourses.tsx";
import CourseDetails from "../pages/CourseDetails.tsx";
import AdminPanel from "../pages/AdminPanel";
import CreatorPanel from "../pages/CreatorPanel.tsx";
import CreatorPanelChats from "../pages/CreatorPanelChats.tsx";
import MyProfile from "../pages/MyProfile.tsx";
import MyCoursesPage from "../pages/MyCoursesPage.tsx";
import { CoursePage } from "../pages/CourseViewPage.tsx";
import StagePlayerPage from "../pages/StagePlayerPage.tsx";
import { AddStage } from "../pages/AddStage.tsx";
import AddReviewForm from "../components/CourseReviewForm.tsx";


const AnimatedRoutes = () => {
	const location = useLocation();

	return (
		<AnimatePresence mode="wait">
			<Routes location={location} key={location.pathname}>
				<Route path="/login" element={<PageWrapper><Login /></PageWrapper>} />
				<Route path="/register" element={<PageWrapper><Register /></PageWrapper>} />
				<Route path="/courses" element={<PageWrapper><Courses /></PageWrapper>} />
				<Route path="/add-course" element={<PageWrapper><ProtectedRoute><AddCourse /></ProtectedRoute></PageWrapper>} />
				<Route path="/creator-courses" element={<PageWrapper><ProtectedRoute><CreatorCourses /></ProtectedRoute></PageWrapper>} />
				<Route path="/courses/:id" element={<PageWrapper><CourseDetails /></PageWrapper>} />
				<Route path="/" element={<PageWrapper><HomeContent /></PageWrapper>} />
				<Route path="/my-courses" element={<PageWrapper><ProtectedRoute><MyCoursesPage /></ProtectedRoute></PageWrapper>} />
				<Route path="/my-profile" element={<PageWrapper><ProtectedRoute><MyProfile /></ProtectedRoute></PageWrapper>} />
				<Route path="/creator-panel" element={<ProtectedRoute><CreatorPanel /></ProtectedRoute>} />
				<Route path="/creatorpanel/chats" element={<ProtectedRoute><CreatorPanelChats /></ProtectedRoute>} />
				<Route path="/course/:id" element={<PageWrapper><ProtectedRoute><CoursePage /></ProtectedRoute></PageWrapper>} />
				<Route path="/course/:id/stage/:stageId" element={<PageWrapper><ProtectedRoute><StagePlayerPage /></ProtectedRoute></PageWrapper>} />
				<Route path="/admin" element={<PageWrapper><ProtectedRoute><AdminPanel /></ProtectedRoute></PageWrapper>} />
				<Route path="/add-stage" element={<PageWrapper><ProtectedRoute><AddStage /></ProtectedRoute></PageWrapper>} />
				<Route path="/course/:id/add-review" element={<AddReviewForm />} />
				<Route path="*" element={<PageWrapper><h1>404 - Page Not Found</h1></PageWrapper>} />
			</Routes>
		</AnimatePresence>
	);
};

export { AnimatedRoutes as default };
