import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";
import { config } from "../config.ts";

interface ChatMessage {
	chatId: string;
	authorId: string;
	authorName: string;
	content: string;
	createdAt: string;
}

export function useChat() {
	const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
	const [messages, setMessages] = useState<ChatMessage[]>([]);
	const [connected, setConnected] = useState(false);

	useEffect(() => {
		const conn = new signalR.HubConnectionBuilder()
			.withUrl(`${config.apiBaseUrl}/hubs/chat`, {
				withCredentials: true,
			})
			.withAutomaticReconnect()
			.build();

		conn.on("ReceiveMessage", (message: ChatMessage) => {
			setMessages((prev) => [...prev, message]);
		});

		conn
			.start()
			.then(() => {
				console.log("Connected to Hub");
				setConnected(true);
			})
			.catch((err) => console.error("Connection error: ", err));

		setConnection(conn);

		return () => {
			conn.stop();
		};
	}, []);

	const sendMessage = async (chatId: string, content: string) => {
		if (connection && connected) {
			try {
				await connection.invoke("SendMessage", chatId, content);
			} catch (error) {
				console.error("Error sending message:", error);
			}
		}
	};

	const joinChat = async (chatId: string) => {
		if (connection && connected) {
			try {
				await connection.invoke("JoinChat", chatId);
			} catch (error) {
				console.error("Error joining chat:", error);
			}
		}
	};

	return { sendMessage, joinChat, messages };
}
