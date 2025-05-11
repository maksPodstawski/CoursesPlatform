import { BrowserRouter as Router, Routes, Route, Link } from "react-router-dom";
import { useState, useEffect } from "react";
import { Login } from "./pages/Login";
import { Register } from "./pages/Register";
import { weatherService } from "./services/weatherService";
import "./App.css";

function App() {
	const [weatherData, setWeatherData] = useState<any>(null);
	const [error, setError] = useState<string>("");
	const [isLoggedIn, setIsLoggedIn] = useState<boolean>(false);

	useEffect(() => {
		// Check if user is logged in by trying to fetch weather data
		const checkAuth = async () => {
			try {
				await weatherService.getForecast();
				setIsLoggedIn(true);
			} catch {
				setIsLoggedIn(false);
			}
		};
		checkAuth();
	}, []);

	const handleWeatherRequest = async () => {
		try {
			const data = await weatherService.getForecast();
			setWeatherData(data);
			setError("");
		} catch (err) {
			setError("Failed to fetch weather data. Please make sure you are logged in.");
			setWeatherData(null);
			setIsLoggedIn(false);
		}
	};

	const handleLogout = () => {
		setWeatherData(null);
		setIsLoggedIn(false);
		window.location.href = "/login";
	};

	return (
		<Router>
			<div className="app">
				<nav className="nav">
					<div className="nav-content">
						<Link to="/" className="nav-link">
							Courses Platform
						</Link>
						<div>
							{isLoggedIn ? (
								<button type="button" onClick={handleLogout} className="btn btn-danger">
									Logout
								</button>
							) : (
								<>
									<Link to="/login" className="nav-link">
										Login
									</Link>
									<Link to="/register" className="nav-link">
										Register
									</Link>
								</>
							)}
						</div>
					</div>
				</nav>

				<main className="main">
					<Routes>
						<Route path="/login" element={<Login onLoginSuccess={() => setIsLoggedIn(true)} />} />
						<Route path="/register" element={<Register />} />
						<Route
							path="/"
							element={
								<div>
									<h1>Welcome to Courses Platform</h1>
									{isLoggedIn ? (
										<div className="card">
											<h2>Test Authorization</h2>
											<p>Click the button below to test if your authorization is working.</p>
											<button type="button" onClick={handleWeatherRequest} className="btn btn-primary">
												Get Weather Forecast
											</button>
											{error && <div className="error">{error}</div>}
											{weatherData && (
												<div className="success">
													<h3>Weather Data:</h3>
													<pre>{JSON.stringify(weatherData, null, 2)}</pre>
												</div>
											)}
										</div>
									) : (
										<p>Please login or register to continue.</p>
									)}
								</div>
							}
						/>
					</Routes>
				</main>
			</div>
		</Router>
	);
}

export default App;
