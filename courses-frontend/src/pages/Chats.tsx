import { useEffect, useState, useRef, useCallback } from "react";
import { useSearchParams } from "react-router-dom";
import { HubConnectionBuilder, type HubConnection } from "@microsoft/signalr";
import { chatService } from "../services/chatService";
import type { Message, CreateChatResponseDTO } from "../types/courses";
import { config } from "../config";
import "../styles/Chats.css";

interface SignalRMessage {
	chatId: string;
	authorId: string;
	authorName: string;
	content: string;
	createdAt: string;
}

const handleReceiveMessage = (
	message: SignalRMessage,
	selectedChatId: string | null,
	setMessages: React.Dispatch<React.SetStateAction<Message[]>>,
) => {
	if (!message?.authorId || !message.content) return;
	if (message.chatId !== selectedChatId) return;

	const messageWithId: Message = {
		...message,
		id: `${message.chatId}-${message.authorId}-${message.createdAt}`,
	};

	setMessages((prev) => {
		if (prev.some((m) => m.id === messageWithId.id)) return prev;
		return [...prev, messageWithId];
	});
};

const initializeConnection = async (
	onMessage: (message: SignalRMessage) => void,
	onClose: () => void,
): Promise<HubConnection | null> => {
	try {
		const connection = new HubConnectionBuilder()
			.withUrl(`${config.apiBaseUrl}/hubs/chat`, {
				withCredentials: true,
				skipNegotiation: true,
				transport: 1,
			})
			.withAutomaticReconnect()
			.build();

		connection.on("ReceiveMessage", onMessage);
		connection.onclose(onClose);

		await connection.start();
		console.log("SignalR Connected");
		return connection;
	} catch (err) {
		console.error("Error initializing connection:", err);
		return null;
	}
};

const Chats = () => {
	const [searchParams] = useSearchParams();
	const [chats, setChats] = useState<CreateChatResponseDTO[]>([]);
	const [selectedChat, setSelectedChat] = useState<CreateChatResponseDTO | null>(null);
	const selectedChatRef = useRef<CreateChatResponseDTO | null>(null);
	const [messages, setMessages] = useState<Message[]>([]);
	const [newMessage, setNewMessage] = useState("");
	const [loading, setLoading] = useState(true);
	const [isConnected, setIsConnected] = useState(false);
	const connectionRef = useRef<HubConnection | null>(null);
	const isInitializedRef = useRef<boolean>(false);
	const initialLoadDoneRef = useRef<boolean>(false);

	useEffect(() => {
		selectedChatRef.current = selectedChat;
	}, [selectedChat]);

	const joinChat = useCallback(
		async (chatId: string) => {
			if (!connectionRef.current || !isConnected) {
				console.log("Cannot join chat - no connection");
				return false;
			}
			try {
				await connectionRef.current.invoke("JoinChat", chatId);
				console.log("Joined chat:", chatId);
				return true;
			} catch (err) {
				console.error("Error joining chat:", err);
				return false;
			}
		},
		[isConnected],
	);

	useEffect(() => {
		const setupConnection = async () => {
			if (connectionRef.current) return;

			const connection = await initializeConnection(
				(message) => handleReceiveMessage(message, selectedChatRef.current?.id ?? null, setMessages),
				() => setIsConnected(false),
			);

			if (connection) {
				connectionRef.current = connection;
				setIsConnected(true);
			}
		};

		setupConnection();

		return () => {
			connectionRef.current?.stop();
			setIsConnected(false);
		};
	}, []);

	useEffect(() => {
		const initializeChat = async () => {
			if (isInitializedRef.current) return;

			try {
				const existingChats = await chatService.getMyChats();
				setChats(existingChats);

				const chatId = searchParams.get("chatId");
				if (chatId) {
					const chatToSelect = existingChats.find((chat) => chat.id === chatId);
					if (chatToSelect) {
						setSelectedChat(chatToSelect);
						await joinChat(chatToSelect.id);
					}
				}
			} catch (err) {
				console.error("Error initializing chat:", err);
			} finally {
				isInitializedRef.current = true;
				setLoading(false);
			}
		};

		initializeChat();
	}, [searchParams, joinChat]);

	useEffect(() => {
		const loadMessages = async () => {
			if (!selectedChat) return;

			try {
				const chatMessages = await chatService.getChatMessages(selectedChat.id);
				setMessages(chatMessages);
				await joinChat(selectedChat.id);
				initialLoadDoneRef.current = true;
			} catch (err) {
				console.error("Error loading messages:", err);
			}
		};

		loadMessages();
	}, [selectedChat, joinChat]);

	useEffect(() => {
		const container = document.querySelector(".messages-container");
		if (container) {
			container.scrollTop = container.scrollHeight;
		}
	}, [messages.length]);

	const handleSendMessage = async () => {
		if (!newMessage.trim() || !selectedChat || !connectionRef.current || !isConnected) {
			console.log("Cannot send message - missing requirements");
			return;
		}

		try {
			await connectionRef.current.invoke("SendMessage", selectedChat.id, newMessage);
			setNewMessage("");
		} catch (err) {
			console.error("Error sending message:", err);
		}
	};

	if (loading) {
		return <div className="chats-loading">Loading...</div>;
	}

	const uniqueChats = Array.from(new Map(chats.map((chat) => [chat.id, chat])).values());

	return (
		<div className="chats-container">
			<div className="chats-sidebar">
				<h2>Your Chats</h2>
				<div className="chats-list">
					{uniqueChats.map((chat) => (
						<div
							key={chat.id}
							className={`chat-item ${selectedChat?.id === chat.id ? "selected" : ""}`}
							onClick={() => setSelectedChat(chat)}
						>
							<h3>{chat.name}</h3>
						</div>
					))}
				</div>
			</div>
			<div className="chats-main">
				{selectedChat ? (
					<>
						<div className="chat-header">
							<h2>{selectedChat.name}</h2>
						</div>
						<div className="messages-container">
							{messages.map((message) => {
								const isCurrentUser = message.authorId === localStorage.getItem("userId");
								return (
									<div key={message.id} className={`message ${isCurrentUser ? "sent" : "received"}`}>
										<div className="message-header">
											<span className="sender-name">{message.authorName}</span>
											<span className="message-time">{new Date(message.createdAt).toLocaleTimeString()}</span>
										</div>
										<div className="message-content">{message.content}</div>
									</div>
								);
							})}
						</div>
						<div className="message-input">
							<input
								type="text"
								value={newMessage}
								onChange={(e) => setNewMessage(e.target.value)}
								onKeyPress={(e) => e.key === "Enter" && handleSendMessage()}
								placeholder="Type your message..."
							/>
							<button type="button" onClick={handleSendMessage}>
								Send
							</button>
						</div>
					</>
				) : (
					<div className="no-chat-selected">Select a chat to start messaging</div>
				)}
			</div>
		</div>
	);
};

export default Chats;
