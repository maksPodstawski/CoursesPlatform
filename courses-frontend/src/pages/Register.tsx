import "../styles/Register.css";
import { useNavigate } from "react-router-dom";
import { authService } from "../services/authService";
import { validateRegisterForm } from "../validation/registerValidation";
import { useForm } from "../hooks/useForm";
import { mapBackendErrors } from "../utils/errorHandling";
import type { RegisterRequest } from "../types/user";
import { registerFieldMapping, registerInitialValues } from "../constants/forms.ts";
import { useGoogleReCaptcha } from "react-google-recaptcha-v3";

export const Register = () => {
	const navigate = useNavigate();
	const { executeRecaptcha } = useGoogleReCaptcha();

	const {
		formData,
		fieldErrors,
		setFieldErrors,
		generalError,
		setGeneralError,
		handleChange,
		handleBlur,
		validateAll,
		isSubmitting,
		setIsSubmitting,
	} = useForm<RegisterRequest>(registerInitialValues, validateRegisterForm);

	const handleSubmit = async (e: React.FormEvent) => {
		e.preventDefault();
		setFieldErrors({});
		setGeneralError("");

		if (!validateAll()) return;

		if (!executeRecaptcha) {
			setGeneralError("reCAPTCHA is not ready. Please try again.");
			return;
		}

		setIsSubmitting(true);
		try {
			const recaptchaToken = await executeRecaptcha("register");

			const registerData = {
				...formData,
				recaptchaToken,
			};

			await authService.register(registerData);
			navigate("/login");
		} catch (err: unknown) {
			const error = err as Error & {
				response?: {
					status: number;
					data: { errors?: Record<string, string[]> };
				};
			};

			const response = error.response;

			if (response?.status === 400 && response.data?.errors) {
				const mappedErrors = mapBackendErrors<RegisterRequest>(response.data.errors, registerFieldMapping);
				setFieldErrors(mappedErrors);
			} else {
				setGeneralError(error.message || "Registration failed. Please try again.");
			}
		} finally {
			setIsSubmitting(false);
		}
	};

	const renderField = (name: keyof RegisterRequest, type: string, placeholder: string) => (
		<div className="form-group">
			<input
				id={name}
				name={name}
				type={type}
				className={`form-input ${fieldErrors[name] ? "error" : ""}`}
				value={formData[name]}
				onChange={handleChange}
				onBlur={handleBlur}
				placeholder={placeholder}
			/>
			{fieldErrors[name] && (
				<div className="field-error">
					<svg
						xmlns="http://www.w3.org/2000/svg"
						fill="none"
						viewBox="0 0 24 24"
						strokeWidth={2}
						stroke="currentColor"
						className="icon"
						width="16"
						height="16"
					>
						<path
							strokeLinecap="round"
							strokeLinejoin="round"
							d="M12 9v2m0 4h.01M12 2a10 10 0 100 20 10 10 0 000-20z"
						/>
					</svg>
					<span>{fieldErrors[name]}</span>
				</div>
			)}
		</div>
	);

	return (
		<div className="form">
			<h2>Create your account</h2>
			<form onSubmit={handleSubmit}>
				{generalError && <div className="error">{generalError}</div>}

				{renderField("firstName", "text", "First Name")}
				{renderField("lastName", "text", "Last Name")}
				{renderField("email", "email", "Email address")}
				{renderField("password", "password", "Password")}
				{renderField("confirmPassword", "password", "Confirm Password")}

				<button type="submit" className="btn btn-primary" disabled={isSubmitting}>
					{isSubmitting ? "Creating account..." : "Create Account"}
				</button>
			</form>
		</div>
	);
};
