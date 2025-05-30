import { fetchClient } from "./fetchClient";
import type { CreateChatResponseDTO, Message } from "../types/courses.ts";

export const chatService = {
	async getMyChats(): Promise<CreateChatResponseDTO[]> {
		const response = await fetchClient.fetch("/api/chat/my");
		return response.json();
	},

	async createChat(name: string): Promise<CreateChatResponseDTO> {
		const response = await fetchClient.fetch("/api/chat/create", {
			method: "POST",
			headers: { "Content-Type": "application/json" },
			body: JSON.stringify({ name })
		});
		return response.json();
	},

	async createCourseChat(name: string, courseId: string): Promise<CreateChatResponseDTO> {
		const response = await fetchClient.fetch(`/course/create?courseId=${courseId}`, {
			method: "POST",
			headers: { "Content-Type": "application/json" },
			body: JSON.stringify({ name })
		});
		return response.json();
	},

	async joinChat(chatId: string): Promise<void> {
		await fetchClient.fetch(`/api/chat/${chatId}/join`, {
			method: "POST"
		});
	},

	async getChatMessages(chatId: string, count: number = 50): Promise<Message[]> {
		const response = await fetchClient.fetch(`/api/chat/${chatId}/messages?count=${count}`);
		return response.json();
	},

	async sendMessage(chatId: string, content: string): Promise<Message> {
		const response = await fetchClient.fetch(`/api/chats/${chatId}/messages`, {
			method: "POST",
			headers: { "Content-Type": "application/json" },
			body: JSON.stringify({ content })
		});
		return response.json();
	},
};
