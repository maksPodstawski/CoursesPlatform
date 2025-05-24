import { apiClient } from "./apiClient";
import type { CreateChatResponseDTO, Message } from "../types/courses.ts";

export const chatService = {
	async getMyChats(): Promise<CreateChatResponseDTO[]> {
		const response = await apiClient.get("/api/chat/my");
		return response.data as CreateChatResponseDTO[];
	},

	async createChat(name: string): Promise<CreateChatResponseDTO> {
		const response = await apiClient.post("/api/chat/create", { name });
		return response.data as CreateChatResponseDTO;
	},

	async createCourseChat(name: string, courseId: string): Promise<CreateChatResponseDTO> {
		const response = await apiClient.post(`/course/create?courseId=${courseId}`, { name });
		return response.data as CreateChatResponseDTO;
	},

	async joinChat(chatId: string): Promise<void> {
		await apiClient.post(`/api/chat/${chatId}/join`);
	},

	async getChatMessages(chatId: string, count: number = 50): Promise<Message[]> {
		const response = await apiClient.get(`/api/chat/${chatId}/messages?count=${count}`);
		return response.data as Message[];
	},

	async sendMessage(chatId: string, content: string): Promise<Message> {
		const response = await apiClient.post(`/api/chats/${chatId}/messages`, { content });
		return response.data as Message;
	},
};
