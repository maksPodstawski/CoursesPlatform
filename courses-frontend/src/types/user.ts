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
	recaptchaToken: string;
};

export type UserInfo = {
	id: string;
	email: string;
	firstName: string;
	lastName: string;
};
export type UserProfile = {
	firstName: string;
	lastName: string;
	email: string;
	userName: string;
	phoneNumber: string;
	profilePictureBase64: string | null;
};
