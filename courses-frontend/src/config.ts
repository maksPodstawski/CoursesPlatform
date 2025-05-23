export const config = {
	apiBaseUrl: import.meta.env.API_BASE_URL || "https://localhost:7207",
	apiEndpoints: {
		login: "/api/account/login",
		register: "/api/account/register",
		logout: "/api/account/logout",
		refreshToken: "/api/account/refresh",

		getCourses: "/api/courses",
		getCoursesByTitle: "/api/courses", 
		getCoursesByPriceRange: "/api/courses/price-range", 
	},
} as const;
