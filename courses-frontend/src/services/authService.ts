import { config } from "../config";
import type { LoginRequest, RegisterRequest, UserInfo } from "../types/user.ts";

export const authService = {
	async login(credentials: LoginRequest) {
		const response = await fetch(`${config.apiBaseUrl}${config.apiEndpoints.login}`, {
			method: "POST",
			headers: { "Content-Type": "application/json" },
			body: JSON.stringify(credentials),
			credentials: "include",
		});

		if (!response.ok) throw new Error("Login failed");
	},

	async getMe(): Promise<UserInfo> {
		const response = await fetch(`${config.apiBaseUrl}/api/account/me`, {
			credentials: "include",
		});

		if (!response.ok) throw new Error("Failed to get user info");
		return response.json();
	},

	async logout() {
		const response = await fetch(`${config.apiBaseUrl}${config.apiEndpoints.logout}`, {
			method: "POST",
			credentials: "include",
		});

		if (!response.ok) throw new Error("Logout failed");
	},

	async register(data: RegisterRequest) {
		const response = await fetch(`${config.apiBaseUrl}${config.apiEndpoints.register}`, {
			method: "POST",
			headers: { "Content-Type": "application/json" },
			body: JSON.stringify(data),
			credentials: "include",
		});

		if (!response.ok) throw new Error("Registration failed");
	},
};
