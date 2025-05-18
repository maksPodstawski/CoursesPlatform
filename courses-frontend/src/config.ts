export const config = {
	apiBaseUrl: import.meta.env.API_BASE_URL || "https://api.czester.ovh",
	apiEndpoints: {
		login: "/api/account/login",
		register: "/api/account/register",
		refreshToken: "/api/account/refresh",

		getCourses: "/api/courses",
		getCoursesByTitle: "/api/courses", 
		getCoursesByPriceRange: "/api/courses/price-range", 
	},
} as const;
