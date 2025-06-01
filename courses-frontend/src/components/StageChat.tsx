import { useState, useRef, useEffect } from "react";
import { Send, X, MessageSquare } from "lucide-react";
import { HubConnectionBuilder, type HubConnection } from "@microsoft/signalr";
import { chatService } from "../services/chatService";
import { config } from "../config";
import "../styles/StageChat.css";

interface ChatMessage {
	id: string;
	senderId: string;
	senderName: string;
	senderAvatar?: string;
	senderRole: "student" | "instructor";
	content: string;
	timestamp: string;
	isRead: boolean;
}

interface SignalRMessage {
	chatId: string;
	authorId: string;
	authorName: string;
	content: string;
	createdAt: string;
}

interface StageChatProps {
	stageId: string;
	stageName: string;
	courseId: string;
	courseName: string;
	onClose: () => void;
}

const handleReceiveMessage = (
	message: SignalRMessage,
	setMessages: React.Dispatch<React.SetStateAction<ChatMessage[]>>,
) => {
	if (!message?.authorId || !message.content) return;

	const messageWithId: ChatMessage = {
		id: `${message.chatId}-${message.authorId}-${message.createdAt}`,
		senderId: message.authorId,
		senderName: message.authorName,
		senderRole: message.authorId === localStorage.getItem("userId") ? "student" : "instructor",
		content: message.content,
		timestamp: message.createdAt,
		isRead: true,
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

export function StageChat({ stageId, stageName, courseId, courseName, onClose }: StageChatProps) {
	const [messages, setMessages] = useState<ChatMessage[]>([]);
	const [newMessage, setNewMessage] = useState("");
	const [isConnected, setIsConnected] = useState(false);
	const chatContainerRef = useRef<HTMLDivElement>(null);
	const connectionRef = useRef<HubConnection | null>(null);

	useEffect(() => {
		if (messages.length > 0 && chatContainerRef.current) {
			chatContainerRef.current.scrollTop = chatContainerRef.current.scrollHeight;
		}
	}, [messages.length]);

	useEffect(() => {
		const setupConnection = async () => {
			if (connectionRef.current) return;

			const connection = await initializeConnection(
				(message) => handleReceiveMessage(message, setMessages),
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
		const loadChatHistory = async () => {
			try {
				const chat = await chatService.getChatByCourse(courseId);
				await chatService.joinChat(chat.id);
				const chatMessages = await chatService.getChatMessages(chat.id, 15);
				const formattedMessages: ChatMessage[] = chatMessages.map((msg) => ({
					id: msg.id,
					senderId: msg.authorId,
					senderName: msg.authorName,
					senderRole: msg.authorId === localStorage.getItem("userId") ? "student" : "instructor",
					content: msg.content,
					timestamp: msg.createdAt,
					isRead: true,
				}));
				setMessages(formattedMessages);
			} catch (err) {
				console.error("Error loading chat history:", err);
			}
		};

		loadChatHistory();
	}, [stageId, courseId]);

	const handleSendMessage = async () => {
		if (!newMessage.trim() || !connectionRef.current || !isConnected) return;

		try {
			const chat = await chatService.getChatByCourse(courseId);
			await connectionRef.current.invoke("SendMessage", chat.id, newMessage);
			setNewMessage("");
		} catch (err) {
			console.error("Error sending message:", err);
		}
	};

	const handleKeyPress = (e: React.KeyboardEvent) => {
		if (e.key === "Enter" && !e.shiftKey) {
			e.preventDefault();
			handleSendMessage();
		}
	};

	const formatTime = (timestamp: string): string => {
		try {
			const date = new Date(timestamp);
			if (isNaN(date.getTime())) {
				console.warn('Invalid date received:', timestamp);
				return '--:--';
			}
			return date.toLocaleTimeString("en-GB", {
				hour: "2-digit",
				minute: "2-digit",
				hour12: false,
			});
		} catch (error) {
			console.error('Error formatting time:', error);
			return '--:--';
		}
	};

	return (
		<div className="stage-chat">
			<div className="stage-chat__header">
				<div className="stage-chat__header-content">
					<div>
						<h2 className="stage-chat__title">
							<MessageSquare className="stage-chat__icon" />
							Chat with Instructor
						</h2>
						<p className="stage-chat__subtitle">{stageName}</p>
					</div>
					<button className="stage-chat__close-btn" onClick={onClose}>
						<X className="stage-chat__icon" />
					</button>
				</div>
			</div>

			<div ref={chatContainerRef} className="stage-chat__messages" style={{ overflowY: "auto", height: "100%", padding: "16px" }}>
				{messages.map((message) => (
					<div
						key={message.id}
						className={`stage-chat__message ${
							message.senderRole === "student"
								? "stage-chat__message--student"
								: "stage-chat__message--instructor"
						}`}
						style={{ marginBottom: "8px" }}
					>
						<div className="stage-chat__message-content">
							<div className="stage-chat__message-info">
								<span>{formatTime(message.timestamp)}</span>
							</div>
							<div
								className={`stage-chat__message-bubble ${
									message.senderRole === "student"
										? "stage-chat__message-bubble--student"
										: "stage-chat__message-bubble--instructor"
								}`}
							>
								<p>{message.content}</p>
							</div>
						</div>
					</div>
				))}
			</div>

			<div className="stage-chat__input-container">
				<div className="stage-chat__input-wrapper">
					<input
						className="stage-chat__input"
						placeholder="Ask a question about this stage..."
						value={newMessage}
						onChange={(e) => setNewMessage(e.target.value)}
						onKeyPress={handleKeyPress}
					/>
					<button
						className="stage-chat__send-btn"
						onClick={handleSendMessage}
						disabled={!newMessage.trim() || !isConnected}
					>
						<Send className="stage-chat__icon" />
					</button>
				</div>
			</div>
		</div>
	);
}
