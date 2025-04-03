import { useState } from "react";
import { Link, Route, BrowserRouter as Router, Routes } from "react-router-dom";
import "./App.css";

function Login() {
	const [email, setEmail] = useState("");
	const [password, setPassword] = useState("");
	const [message, setMessage] = useState("");

	const handleSubmit = async (e) => {
		e.preventDefault();
		setMessage("");

		try {
			const response = await fetch("https://localhost:7080/api/account/login", {
				method: "POST",
				headers: {
					"Content-Type": "application/json",
				},
				body: JSON.stringify({ email, password }),
				credentials: "include",
			});

			if (response.ok) {
				setMessage("Login successful! Cookies stored.");
				console.log("Login successful! Tokens are stored in cookies.");
			} else {
				throw new Error("Login failed");
			}
		} catch (error) {
			setMessage(error.message);
		}
	};

	return (
		<div className="auth-container">
			<h2>Login</h2>
			<form onSubmit={handleSubmit}>
				<input type="email" placeholder="Email" value={email} onChange={(e) => setEmail(e.target.value)} required />
				<input
					type="password"
					placeholder="Password"
					value={password}
					onChange={(e) => setPassword(e.target.value)}
					required
				/>
				<button type="submit">Login</button>
			</form>
			{message && <p>{message}</p>}
			<p>
				Don't have an account? <Link to="/register">Register</Link>
			</p>
		</div>
	);
}

function Register() {
	const [email, setEmail] = useState("");
	const [password, setPassword] = useState("");
	const [firstName, setFirstName] = useState("");
	const [lastName, setLastName] = useState("");
	const [message, setMessage] = useState("");

	const handleSubmit = async (e) => {
		e.preventDefault();
		setMessage("");

		try {
			const response = await fetch("https://localhost:7080/api/account/register", {
				method: "POST",
				headers: {
					"Content-Type": "application/json",
				},
				body: JSON.stringify({ email, password, firstName, lastName }),
			});

			if (response.ok) {
				setMessage("Registration successful! You can now login.");
			} else {
				throw new Error("Registration failed");
			}
		} catch (error) {
			setMessage(error.message);
		}
	};

	return (
		<div className="auth-container">
			<h2>Register</h2>
			<form onSubmit={handleSubmit}>
				<input
					type="text"
					placeholder="First Name"
					value={firstName}
					onChange={(e) => setFirstName(e.target.value)}
					required
				/>
				<input
					type="text"
					placeholder="Last Name"
					value={lastName}
					onChange={(e) => setLastName(e.target.value)}
					required
				/>
				<input type="email" placeholder="Email" value={email} onChange={(e) => setEmail(e.target.value)} required />
				<input
					type="password"
					placeholder="Password"
					value={password}
					onChange={(e) => setPassword(e.target.value)}
					required
				/>
				<button type="submit">Register</button>
			</form>
			{message && <p>{message}</p>}
			<p>
				Already have an account? <Link to="/login">Login</Link>
			</p>
		</div>
	);
}

function App() {
	return (
		<Router>
			<Routes>
				<Route path="/" element={<Login />} />
				<Route path="/login" element={<Login />} />
				<Route path="/register" element={<Register />} />
			</Routes>
		</Router>
	);
}

export default App;
