export const config = {
	apiBaseUrl: "https://localhost:7207",
	recaptchaSiteKey: "6Lc03GgrAAAAAKmdJ_q1SP1l54nmz9GaIfhU17U2",
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

		addStage: "/api/stages",
	},
} as const;
