export const config = {
	apiBaseUrl: import.meta.env.API_BASE_URL || "https://api.czester.ovh",
	apiEndpoints: {
		login: "/api/account/login",
		register: "/api/account/register",
		logout: "/api/account/logout",
		refreshToken: "/api/account/refresh",

		getCourses: "/api/courses",
		getCoursesByTitle: "/api/courses",
		getCoursesByPriceRange: "/api/courses/price-range",
		getCreatorCourses: "/api/Creator/courses",
		getPurchasedCourses: "/api/courses/purchases/user",
		getPurchasedCoursesUser: "/api/courses/purchases/user",
		getStagesByCourse: "/api/stages/course/",
		getReviewsCourse: "/api/Reviews/course/{courseId}",

		streamStageVideo: "/api/stages/",
		addCourse: "/api/courses",
		buyCourse: "/api/courses/purchases",
		addReview: "/api/Reviews",
		avgRating: "/api/Reviews/course/{courseId}/average-rating",
		getUserProfile: "/api/user/me",

		addStage: "/api/stages"
	},
} as const;
