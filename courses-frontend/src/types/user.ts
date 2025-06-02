export type LoginRequest = {
	email: string;
	password: string;
};

export type RegisterRequest = {
	email: string;
	password: string;
	firstName: string;
	lastName: string;
};

export type UserInfo = {
	id: string;
	email: string;
	firstName: string;
	lastName: string;
};
