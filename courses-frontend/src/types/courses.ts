export type Progress = {
	id: string;
	userId: string;
	stageId: string;
	lastAccessedAt: string;
	isCompleted: boolean;
	startedAt: string;
	completedAt: string | null;
};

export type StageWithProgress = {
	id: string;
	name: string;
	description: string;
	videoPath: string;
	duration: number;
	order: number;
	isCompleted: boolean;
	startedAt: string | null;
	completedAt: string | null;
	lastAccessedAt: string | null;
};

export type Course = {
	courseId?: string;
	id?: string;
	name: string;
	description: string;
	imageUrl: string;
};

export type Message = {
	id: string;
	content: string;
	authorId: string;
	authorName: string;
	createdAt: string;
	chatId: string;
};

export type Chat = {
	id: string;
	name: string;
	createdAt: string;
	updatedAt: string;
	participants: string[];
};

export type CreateChatResponseDTO = {
	id: string;
	name: string;
};

export type CreateChatDTO = {
	name: string;
};
