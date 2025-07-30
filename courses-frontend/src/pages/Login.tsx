import '../styles/Login.css';
import { authService } from "../services/authService";
import {Link, useNavigate} from "react-router-dom";
import {useAuth} from "../context/AuthContext.tsx";
import type { LoginRequest } from "../types/user.ts";
import {useForm} from "../hooks/useForm.ts";
import {loginInitialValues} from "../constants/forms.ts";
import {validateLoginForm} from "../validation/loginValidation.ts";

export const Login = () => {
	const navigate = useNavigate();
	const { login } = useAuth();

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
		setIsSubmitting
	} = useForm<LoginRequest>(loginInitialValues, validateLoginForm);

	const handleSubmit = async (e: React.FormEvent) => {
		e.preventDefault();
		setFieldErrors({});
		setGeneralError("");

		if (!validateAll()) return;

		setIsSubmitting(true);
		try {
			await authService.login(formData);
			const userInfo = await authService.getMe();
			localStorage.setItem("userId", userInfo.id);
			localStorage.setItem("userName", userInfo.firstName);
			localStorage.setItem("userEmail", userInfo.email);
			login();
			navigate("/");
		} catch (err) {
			setGeneralError("Invalid email or password");
		} finally {
			setIsSubmitting(false);
		}
	};

	const renderField = ( name: keyof LoginRequest, type: string, placeholder: string ) => 
	(
		<div className="form-group">
			<input
				id={name}
				name={name}
				type={type}
				className={`form-input ${fieldErrors[name] ? 'error' : ''}`}
				value={formData[name]}
				onChange={handleChange}
				onBlur={handleBlur}
				placeholder={placeholder}
			/>
			{fieldErrors[name] && (
				<div className="field-error">
					<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"
					     strokeWidth={2} stroke="currentColor" className="icon" width="16" height="16">
						<path strokeLinecap="round" strokeLinejoin="round"
						      d="M12 9v2m0 4h.01M12 2a10 10 0 100 20 10 10 0 000-20z"/>
					</svg>
					<span>{fieldErrors[name]}</span>
				</div>
			)}
		</div>
	);

	return (
		<div className="form">
			<h2>Sign in</h2>
			<form onSubmit={handleSubmit}>
				{generalError && <div className="error">{generalError}</div>}

				{renderField("email", "text", "Email")}
				{renderField("password", "password", "Password")}

				<button
					type="submit"
					className="btn btn-primary"
					disabled={isSubmitting}
				>
					{isSubmitting ? "Signing in..." : "Sign in"}
				</button>
			</form>

			<div className="form-footer">
				<p>Don't have an account?</p>
				<Link to="/register" className="btn btn-secondary">Sign up</Link>
			</div>
		</div>
	);
};
