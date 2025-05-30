import { createContext, useContext, useEffect, useState } from "react";
import { authService } from "../services/authService";

interface AuthContextType {
	isLoggedIn: boolean;
	login: () => void;
	logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
	const [isLoggedIn, setIsLoggedIn] = useState(false);

	useEffect(() => {
		const storedLogin = localStorage.getItem("isLoggedIn");
		if (storedLogin === "true") {
			setIsLoggedIn(true);
		}
	}, []);

	const login = () => {
		setIsLoggedIn(true);
		localStorage.setItem("isLoggedIn", "true");
	};

	const logout = async () => {
		await authService.logout();
		setIsLoggedIn(false);
		localStorage.removeItem("isLoggedIn");
		localStorage.removeItem("userId");
		localStorage.removeItem("userName");
	};

	return <AuthContext.Provider value={{ isLoggedIn, login, logout }}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
	const context = useContext(AuthContext);
	if (!context) throw new Error("useAuth must be used within AuthProvider");
	return context;
};
