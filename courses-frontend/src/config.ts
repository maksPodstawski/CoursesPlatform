export const config = {
	apiBaseUrl: import.meta.env.API_BASE_URL || "http://localhost:5000",
	apiEndpoints: {
		login: "/api/account/login",
		register: "/api/account/register",
		refreshToken: "/api/account/refresh",
	},
} as const;
