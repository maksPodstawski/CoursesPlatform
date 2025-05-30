export type LoginRequest = {
	email: string;
	password: string;
};

export type RegisterRequest = {
	email: string;
	password: string;
	confirmPassword: string;
	firstName: string;
	lastName: string;
};

export type UserInfo = {
	id: string;
	email: string;
	firstName: string;
	lastName: string;
};