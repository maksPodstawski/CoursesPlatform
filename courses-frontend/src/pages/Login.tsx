import { useState } from "react";
import { authService } from "../services/authService";
import { useNavigate } from "react-router-dom";

interface LoginProps {
	onLoginSuccess: () => void;
}

export const Login = ({ onLoginSuccess }: LoginProps) => {
	const [email, setEmail] = useState("");
	const [password, setPassword] = useState("");
	const [error, setError] = useState("");
	const navigate = useNavigate();

	const handleSubmit = async (e: React.FormEvent) => {
		e.preventDefault();
		setError("");

		try {
			await authService.login({ email, password });
			onLoginSuccess();
			navigate("/");
		} catch (err) {
			setError("Invalid email or password");
		}
	};

	return (
		<div className="form">
			<h2 style={{ textAlign: "center", marginBottom: "1.5rem" }}>Sign in to your account</h2>
			<form onSubmit={handleSubmit}>
				{error && <div className="error">{error}</div>}
				<div className="form-group">
					<label htmlFor="email" className="form-label">
						Email address
					</label>
					<input
						id="email"
						name="email"
						type="email"
						required
						className="form-input"
						value={email}
						onChange={(e) => setEmail(e.target.value)}
					/>
				</div>
				<div className="form-group">
					<label htmlFor="password" className="form-label">
						Password
					</label>
					<input
						id="password"
						name="password"
						type="password"
						required
						className="form-input"
						value={password}
						onChange={(e) => setPassword(e.target.value)}
					/>
				</div>
				<button type="submit" className="btn btn-primary" style={{ width: "100%" }}>
					Sign in
				</button>
			</form>
		</div>
	);
};
