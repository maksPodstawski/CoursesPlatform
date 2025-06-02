import { useState } from "react";
import { authService } from "../services/authService";
import { useNavigate } from "react-router-dom";
import {useAuth} from "../context/AuthContext.tsx";

export const Login = () => {
	const [email, setEmail] = useState("");
	const [password, setPassword] = useState("");
	const [error, setError] = useState("");

	const navigate = useNavigate();
	const { login } = useAuth();

	

	const handleSubmit = async (e: React.FormEvent) => {
		e.preventDefault();
		setError("");

		try {
			await authService.login({ email, password });
			const userInfo = await authService.getMe();
			localStorage.setItem("userId", userInfo.id);
			localStorage.setItem("userName", userInfo.firstName);
			login();
			navigate("/");
		} catch (err) {
			setError("Invalid email or password");
		}
	};

	return (
		<div className="form">
			<h2 style={{ textAlign: "center", marginBottom: "1.5rem" }}>Sign in</h2>
			<form onSubmit={handleSubmit}>
				{error && <div className="error">{error}</div>}
				<div className="form-group">
					<label htmlFor="email">Email</label>
					<input id="email" type="email" required value={email} onChange={(e) => setEmail(e.target.value)} />
				</div>
				<div className="form-group">
					<label htmlFor="password">Password</label>
					<input id="password" type="password" required value={password} onChange={(e) => setPassword(e.target.value)} />
				</div>
				<button type="submit" className="btn btn-primary">Sign in</button>
			</form>
		</div>
	);
};
