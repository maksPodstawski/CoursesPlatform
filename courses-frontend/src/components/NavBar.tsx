import type React from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { Menu, BookOpen, GraduationCap, User, Settings, LogOut, LogIn, UserPlus } from "lucide-react";
import { useAuth } from "../context/AuthContext.tsx";

interface NavBarProps {
	sidebarOpen: boolean;
	toggleSidebar: () => void;
}

const NavBar: React.FC<NavBarProps> = ({ sidebarOpen, toggleSidebar }) => {
	const { isLoggedIn, logout } = useAuth();
	const location = useLocation();
	const isAdminPanel = location.pathname.startsWith("/admin");
	const navigate = useNavigate();

	const handleLogout = async () => {
		try {
			await logout();
			navigate("/");
		} catch (error) {
			console.error("Logout failed", error);
		}
	};

	return (
		<nav className="nav">
			<div className="nav-content">
				<Link to="/" className="logo">
					<span className="logo-text">
						{isAdminPanel ? "Administrator Panel" : "Courses Platform"}
					</span>
				</Link>
				<div className={`nav-links ${sidebarOpen ? "hide-on-mobile" : ""}`}>
					<Link to="/courses" className="nav-link">
						<BookOpen size={18} />
						<span>Courses</span>
					</Link>
					{isLoggedIn && (
						<>
							<Link to="/my-courses" className="nav-link">
								<GraduationCap size={18} />
								<span>My Courses</span>
							</Link>
							<Link to="/creator-panel" className="nav-link">
								<Settings size={18} />
								<span>Creator Panel</span>
							</Link>
							<Link to="/my-profile" className="nav-link">
								<User size={18} />
								<span>My Profile</span>
							</Link>
						</>
					)}
					{isLoggedIn ? (
						<button type="button" onClick={handleLogout} className="btn logout">
							<LogOut size={18} />
							<span>Logout</span>
						</button>
					) : (
						<>
							<Link to="/login" className="nav-link">
								<LogIn size={18} />
								<span>Login</span>
							</Link>
							<Link to="/register" className="nav-link">
								<UserPlus size={18} />
								<span>Register</span>
							</Link>
						</>
					)}
				</div>
				<button type="button" className="menu-toggle" onClick={toggleSidebar}>
					<Menu size={28} color="#e0e0e0" />
				</button>
			</div>

			<div className={`sidebar ${sidebarOpen ? "open" : ""}`}>
				<button type="button" className="close-btn" onClick={toggleSidebar}>
					×
				</button>
				<Link to="/" onClick={toggleSidebar} className="sidebar-link">
					<span>{isAdminPanel ? "Administrator Panel" : "Home"}</span>
				</Link>
				<Link to="/courses" onClick={toggleSidebar} className="sidebar-link">
					<BookOpen size={20} />
					<span>Courses</span>
				</Link>
				{isLoggedIn && (
					<>
						<Link to={"/purchased-courses"} className="sidebar-link">
							<GraduationCap size={20} />
							<span>Purchased Courses</span>
						</Link>
						<Link to="/my-courses" onClick={toggleSidebar} className="sidebar-link">
							<GraduationCap size={20} />
							<span>My Courses</span>
						</Link>
						<Link to="/chats" onClick={toggleSidebar} className="sidebar-link">
							<User size={20} />
							<span>Chats</span>
						</Link>
					</>
				)}
				{isLoggedIn ? (
					<button
						type="button"
						onClick={() => {
							handleLogout();
							toggleSidebar();
						}}
						className="btn logout sidebar-logout"
					>
						<LogOut size={20} />
						<span>Logout</span>
					</button>
				) : (
					<>
						<Link to="/login" onClick={toggleSidebar} className="sidebar-link">
							<LogIn size={20} />
							<span>Login</span>
						</Link>
						<Link to="/register" onClick={toggleSidebar} className="sidebar-link">
							<UserPlus size={20} />
							<span>Register</span>
						</Link>
					</>
				)}
			</div>
		</nav>
	);
};

export default NavBar;
