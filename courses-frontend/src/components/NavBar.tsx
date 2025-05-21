import React from "react";
import { Link } from "react-router-dom";
import { Menu } from "lucide-react";
import {useAuth} from "../context/AuthContext.tsx";

interface NavBarProps {
  sidebarOpen: boolean;
  toggleSidebar: () => void;
}

const NavBar: React.FC<NavBarProps> = ({ sidebarOpen, toggleSidebar }) => {
  const { isLoggedIn, logout } = useAuth();

    const handleLogout = async () => {
        try {
            await logout();
            window.location.href = "/";
        } catch (error) {
            console.error("Logout failed", error);
        }
    };

    return (
        <nav className="nav">
            <div className="nav-content">
                <Link to="/" className="logo">Courses Platform</Link>
                <div className={`nav-links ${sidebarOpen ? "hide-on-mobile" : ""}`}>
                    <Link to="/courses">Courses</Link>
                    {isLoggedIn ? (
                        <button onClick={handleLogout} className="btn logout">Logout</button>
                    ) : (
                        <>
                            <Link to="/login">Login</Link>
                            <Link to="/register">Register</Link>
                        </>
                    )}
                </div>
                <button className="menu-toggle" onClick={toggleSidebar}>
                    <Menu size={28} color="#e0e0e0" />
                </button>
            </div>

            <div className={`sidebar ${sidebarOpen ? "open" : ""}`}>
                <button className="close-btn" onClick={toggleSidebar}>×</button>
                <Link to="/" onClick={toggleSidebar}>Home</Link>
                <Link to="/courses" onClick={toggleSidebar}>Courses</Link>
                {isLoggedIn ? (
                    <button onClick={() => { handleLogout(); toggleSidebar(); }} className="btn logout">Logout</button>
                ) : (
                    <>
                        <Link to="/login" onClick={toggleSidebar}>Login</Link>
                        <Link to="/register" onClick={toggleSidebar}>Register</Link>
                    </>
                )}
            </div>
        </nav>
    );
};

export default NavBar;
