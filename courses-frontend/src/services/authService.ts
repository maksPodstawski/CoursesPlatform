import { config } from '../config';

interface LoginRequest {
    email: string;
    password: string;
}

interface RegisterRequest {
    email: string;
    password: string;
    firstName: string;
    lastName: string;
}

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