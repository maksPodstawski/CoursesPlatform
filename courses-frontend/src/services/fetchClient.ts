import { config } from "../config";

interface RequestOptions extends RequestInit {
	retryCount?: number;
}

interface QueuedRequest {
	url: string;
	options: RequestOptions;
	resolve: (value: Response) => void;
	reject: (reason: any) => void;
}

class FetchClient {
	private static instance: FetchClient;
	private isRefreshing: boolean = false;
	private refreshSubscribers: QueuedRequest[] = [];
	private maxRetries: number = 3;
	private baseUrl: string = config.apiBaseUrl;

	private constructor() {}

	public static getInstance(): FetchClient {
		if (!FetchClient.instance) {
			FetchClient.instance = new FetchClient();
		}
		return FetchClient.instance;
	}

	private async refreshToken(): Promise<boolean> {
		try {
			const response = await fetch(`${this.baseUrl}/api/account/refresh`, {
				method: "POST",
				credentials: "include",
			});

			if (!response.ok) {
				await this.logout();
				throw new Error("Token refresh failed");
			}

			return true;
		} catch (error) {
			console.error("Token refresh error:", error);
			await this.logout();
			return false;
		}
	}

	private async handleRefreshToken(): Promise<boolean> {
		if (this.isRefreshing) {
			return new Promise((resolve) => {
				this.refreshSubscribers.push({
					url: "",
					options: {},
					resolve: () => resolve(true),
					reject: () => resolve(false),
				});
			});
		}

		this.isRefreshing = true;
		const success = await this.refreshToken();
		this.isRefreshing = false;

		this.refreshSubscribers.forEach(({ resolve, reject }) => {
			if (success) {
				resolve(new Response());
			} else {
				reject(new Error("Token refresh failed"));
			}
		});
		this.refreshSubscribers = [];

		return success;
	}

	private async retryWithBackoff(url: string, options: RequestOptions, retryCount: number = 0): Promise<Response> {
		try {
			const response = await fetch(url, {
				...options,
				credentials: "include",
			});

			if (response.status === 401 && retryCount < this.maxRetries) {
				const refreshSuccess = await this.handleRefreshToken();
				if (refreshSuccess) {
					return this.retryWithBackoff(url, options, retryCount + 1);
				}
			}

			return response;
		} catch (error) {
			if (retryCount < this.maxRetries) {
				const delay = 2 ** retryCount * 1000;
				await new Promise((resolve) => setTimeout(resolve, delay));
				return this.retryWithBackoff(url, options, retryCount + 1);
			}
			throw error;
		}
	}

	public async fetch(url: string, options: RequestOptions = {}): Promise<Response> {
		const fullUrl = url.startsWith("http") ? url : `${this.baseUrl}${url}`;

		try {
			const response = await this.retryWithBackoff(fullUrl, options);

			return response;
		} catch (error) {
			console.error("Fetch error:", error);
			throw error;
		}
	}

	public async logout(): Promise<void> {
		try {
			await fetch(`${this.baseUrl}/api/account/logout`, {
				method: "POST",
				credentials: "include",
			});
			localStorage.removeItem("isLoggedIn");
			localStorage.removeItem("userId");
			localStorage.removeItem("userName");
		} catch (error) {
			console.error("Logout error:", error);
			localStorage.removeItem("isLoggedIn");
			localStorage.removeItem("userId");
			localStorage.removeItem("userName");
		}
	}
}

export const fetchClient = FetchClient.getInstance();
