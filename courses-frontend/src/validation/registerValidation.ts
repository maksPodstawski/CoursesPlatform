import type { RegisterRequest } from "../types/user";

export const validateRegisterForm = (formData: RegisterRequest): Partial<Record<keyof RegisterRequest, string>> => {
    const errors: Partial<Record<keyof RegisterRequest, string>> = {};

    if (!formData.firstName?.trim()) {
        errors.firstName = "The first name field is required!";
    } else if (formData.firstName.length > 50) {
        errors.firstName = "First name must not exceed 50 characters!";
    }

    if (!formData.lastName?.trim()) {
        errors.lastName = "The last name field is required!";
    } else if (formData.lastName.length > 50) {
        errors.lastName = "Name must not exceed 50 characters!";
    }

    if (!formData.email?.trim()) {
        errors.email = "The email address field is required!";
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
        errors.email = "Please enter a valid email address!";
    }

    if (!formData.password) {
        errors.password = "The password field is required!";
    } else if (formData.password.length < 8) {
        errors.password = "The password must have at least 8 characters!";
    } else if (!/^(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*(),.?"{}\|<>]).*$/.test(formData.password)) {
        errors.password = "The password must contain at least one uppercase letter, one lowercase letter, and one special character!";
    }

    if (!formData.confirmPassword) {
        errors.confirmPassword = "The password confirmation field is required!";
    } else if (formData.password !== formData.confirmPassword) {
        errors.confirmPassword = "Passwords must be identical!";
    }

    return errors;
};