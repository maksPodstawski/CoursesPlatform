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
			login();
			navigate("/");
		} catch (err) {
			setGeneralError("Invalid email or password");
		} finally {
			setIsSubmitting(false);
		}
	};

	return (
		<div className="form">
			<h2 style={{ textAlign: "center", marginBottom: "1.5rem" }}>Sign in</h2>
			<form onSubmit={handleSubmit}>
				{generalError && <div className="error">{generalError}</div>}

				<div className="form-group">
					<label htmlFor="email" className="form-label">Email</label>
					<input
						id="email"
						name="email"
						type="text"
						className="form-input"
						value={formData.email}
						onChange={handleChange}
						onBlur={handleBlur}
					/>
					{fieldErrors.email && <div className="field-error">{fieldErrors.email}</div>}
				</div>

				<div className="form-group">
					<label htmlFor="password" className="form-label">Password</label>
					<input
						id="password"
						name="password"
						type="password"
						className="form-input"
						value={formData.password}
						onChange={handleChange}
						onBlur={handleBlur}
					/>
					{fieldErrors.password && <div className="field-error">{fieldErrors.password}</div>}
				</div>

				<button
					type="submit"
					className="btn btn-primary"
					disabled={isSubmitting}
				>
					{isSubmitting ? "Signing in..." : "Sign in"}
				</button>
			</form>
			<div style={{ marginTop: "1rem", textAlign: "center" }}>
				<p>Don't have an account?</p>
				<Link to="/register" className="btn btn-primary">Sign up</Link>
			</div>
		</div>
	);
};
