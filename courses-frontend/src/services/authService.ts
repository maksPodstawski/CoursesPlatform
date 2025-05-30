import { config } from "../config";
import type { LoginRequest, RegisterRequest, UserInfo } from "../types/user.ts";
import { fetchClient } from "./fetchClient";

export const authService = {
	async login(credentials: LoginRequest) {
		const response = await fetchClient.fetch(config.apiEndpoints.login, {
			method: "POST",
			headers: { "Content-Type": "application/json" },
			body: JSON.stringify(credentials),
		});

		if (!response.ok) throw new Error("Login failed");
	},

	async getMe(): Promise<UserInfo> {
		const response = await fetchClient.fetch("/api/account/me");
		if (!response.ok) throw new Error("Failed to get user info");
		return response.json();
	},

	async logout() {
		await fetchClient.logout();
	},

	async register(data: RegisterRequest) {
		const response = await fetchClient.fetch(config.apiEndpoints.register, {
			method: "POST",
			headers: { "Content-Type": "application/json" },
			body: JSON.stringify(data),
		});

		if (!response.ok) throw new Error("Registration failed");
	},
};
