import { Routes, Route, useLocation } from "react-router-dom";
import { AnimatePresence } from "framer-motion";
import { PageWrapper } from "../utils/animations";

import { Login } from "../pages/Login";
import { Register } from "../pages/Register";
import HomeContent from "../pages/HomeContent";
import Courses from "../pages/Courses";
import Chats from "../pages/Chats";
import MyCourses from "../pages/MyCourses.tsx";
import {AddCourse} from "../pages/AddCourse.tsx";
import CreatorCourses from "../pages/CreatorCourses.tsx";
import PurchasedCourses from "../pages/PurchasedCourses.tsx";
import CourseDetails from "../pages/CourseDetails.tsx";

const AnimatedRoutes = () => {
	const location = useLocation();

	return (
		<AnimatePresence mode="wait">
			<Routes location={location} key={location.pathname}>
				<Route path="/login" element={<PageWrapper> <Login /></PageWrapper> }/>
				<Route path="/register" element={<PageWrapper><Register /></PageWrapper>}/>
				<Route path="/courses" element={<PageWrapper><Courses /></PageWrapper>}/>
				<Route path="/add-course" element={<PageWrapper><AddCourse /></PageWrapper>} />
				<Route path="/creator-courses" element={<PageWrapper><CreatorCourses /></PageWrapper>} />
				<Route path="/purchased-courses" element={<PageWrapper><PurchasedCourses /></PageWrapper>} />
				<Route path="/courses/:id" element={<PageWrapper><CourseDetails /></PageWrapper>} />
				<Route path="/" element={<PageWrapper><HomeContent /></PageWrapper>}/>
				<Route path="/my-courses" element={<PageWrapper><MyCourses /></PageWrapper>}/>
				<Route path="/chats" element={<PageWrapper><Chats /></PageWrapper>}/>
			</Routes>
		</AnimatePresence>
	);
};

export default AnimatedRoutes;
