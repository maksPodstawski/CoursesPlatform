import { BrowserRouter as Router } from "react-router-dom";
import { useState } from "react";
import { GoogleReCaptchaProvider } from "react-google-recaptcha-v3";
import NavBar from "./components/NavBar";
import Footer from "./components/Footer";
import ScrollToTopButton from "./components/ScrollToTopButton";
import AnimatedRoutes from "./routes/AnimatedRoutes";
import { AuthProvider } from "./context/AuthContext.tsx";
import "./App.css";

function App() {
	const [sidebarOpen, setSidebarOpen] = useState(false);
	const toggleSidebar = () => setSidebarOpen((prev) => !prev);

	return (
		<GoogleReCaptchaProvider
			reCaptchaKey={"6Le_kFsrAAAAALpPsvN5ogmajyec6H3_jjf7QRL1"}
			scriptProps={{
				async: false,
				defer: false,
				appendTo: "head",
				nonce: undefined,
			}}
		>
			<Router>
				<AuthProvider>
					<div className="app">
						<NavBar sidebarOpen={sidebarOpen} toggleSidebar={toggleSidebar} />
						<main>
							<AnimatedRoutes />
							<ScrollToTopButton />
						</main>
						<Footer />
					</div>
				</AuthProvider>
			</Router>
		</GoogleReCaptchaProvider>
	);
}

export default App;
