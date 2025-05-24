import React, { useState } from "react";
import { useChat } from "../services/useChat";

const ChatTestPage = () => {
	const { sendMessage, joinChat, messages } = useChat();
	const [chatId, setChatId] = useState("");
	const [message, setMessage] = useState("");

	const handleJoin = () => {
		joinChat(chatId);
	};

	const handleSend = () => {
		if (chatId && message) {
			sendMessage(chatId, message);
			setMessage("");
		}
	};

	return (
		<div className="container">
			<h1>Test Chat Page (SignalR)</h1>

			<div className="chat-controls">
				<input type="text" value={chatId} onChange={(e) => setChatId(e.target.value)} placeholder="Chat ID" />
				<button type="button" onClick={handleJoin}>
					Join Chat
				</button>
			</div>

			<div className="chat-controls">
				<input type="text" value={message} onChange={(e) => setMessage(e.target.value)} placeholder="Your message" />
				<button type="button" onClick={handleSend}>
					Send
				</button>
			</div>

			<div className="chat-messages">
				<h3>Messages:</h3>
				<ul>
					{messages.map((msg, i) => (
						<li key={i}>
							<strong>Author {msg.authorName}:</strong> {msg.content}
							<small> ({new Date(msg.createdAt).toLocaleString()})</small>
						</li>
					))}
				</ul>
			</div>
		</div>
	);
};

export default ChatTestPage;
