import type { LoginRequest, RegisterRequest } from "../types/user";

export const registerInitialValues: RegisterRequest = {
    email: '',
    password: '',
    confirmPassword: '',
    firstName: '',
    lastName: '',
    recaptchaToken: ''
};

export const registerFieldMapping: Record<string, keyof RegisterRequest> = {
    'Email': 'email',
    'Password': 'password',
    'ConfirmPassword': 'confirmPassword',
    'FirstName': 'firstName',
    'LastName': 'lastName'
};

export const loginInitialValues: LoginRequest = {
    email: '',
    password: ''
};

export const loginFieldMapping: Record<string, keyof LoginRequest> = {
    'Email': 'email',
    'Password': 'password'
};