export interface Stage {
	id: string;
	courseId: string;
	name: string;
	description: string;
	duration: number;
	videoPath?: string;
	createdAt: string;
	isCompleted: boolean;
	locked: boolean;
}

export interface Progress {
	id: string;
	userId: string;
	stageId: string;
	completed: boolean;
	startedAt: string;
	completedAt?: string;
}

export interface StageWithProgress extends Stage {
	progress?: Progress;
}

export interface Message {
	id: string;
	chatId: string;
	authorId: string;
	authorName: string;
	content: string;
	createdAt: string;
}

export interface CreateChatResponseDTO {
	id: string;
	name: string;
	createdAt: string;
}

export type Course = {
	courseId?: string;
	id?: string;
	name: string;
	description: string;
	imageUrl: string;
	price?: number;
};

export type Chat = {
	id: string;
	name: string;
	createdAt: string;
	updatedAt: string;
	participants: string[];
};

export type CreateChatDTO = {
	name: string;
};
export type CreateReviewRequest = {
	courseId: string;
	rating: number;
	comment: string;
};
export interface UpdateReviewRequest {
	rating: number;
	comment: string;
}

export interface ReviewResponseDTO {
	id: string;
	rating: number;
	comment: string;
	createdAt: string;
	userName: string;
	courseId: string;
	courseName: string;
}