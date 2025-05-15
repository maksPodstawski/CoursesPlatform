export const config = {
	apiBaseUrl: import.meta.env.API_BASE_URL || "https://api.czester.ovh",
	apiEndpoints: {
		login: "/api/account/login",
		register: "/api/account/register",
		refreshToken: "/api/account/refresh",
	},
} as const;
