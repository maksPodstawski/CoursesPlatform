import type { LoginRequest } from "../types/user";

export const validateLoginForm = (data: LoginRequest): Partial<Record<keyof LoginRequest, string>> => {
    const errors: Partial<Record<keyof LoginRequest, string>> = {};

    if (!data.email) {
        errors.email = "The email address field is required.";
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(data.email)) {
        errors.email = "Please enter a valid email address.";
    }

    if (!data.password) {
        errors.password = "The password field is required.";
    }

    return errors;
};