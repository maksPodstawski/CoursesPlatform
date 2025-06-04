import { Link, useLocation } from "react-router-dom";
import { Home, Book, MessageCircle, Plus, BarChart, Settings, User } from "lucide-react";
import "../styles/Sidebar.css";

const navItems = [
	{ label: "Dashboard", icon: <Home size={22} />, path: "/creatorpanel" },
	{ label: "My Courses", icon: <Book size={22} />, path: "/my-courses" },
	{ label: "Student Chats", icon: <MessageCircle size={22} />, path: "/creatorpanel/chats" },
	{ label: "Create Course", icon: <Plus size={22} />, path: "/add-course" },
	{ label: "Analytics", icon: <BarChart size={22} />, path: "/analytics" },
	{ label: "Settings", icon: <Settings size={22} />, path: "/settings" },
];

export default function Sidebar() {
	const location = useLocation();

	return (
		<aside className="sidemenu">
			<div className="sidemenu__top">
				{/*<div className="sidemenu__profile-section">
					<User className="sidemenu__avatar" size={48} />
					<div>
						<div className="sidemenu__name">Polska Sigma</div>
						<div className="sidemenu__role">Course Creator</div>
					</div>
				</div>*/}
				<nav className="sidemenu__nav">
					{navItems.map(({ label, icon, path }) => (
						<Link key={label} to={path} className={`sidemenu__link ${location.pathname === path ? "active" : ""}`}>
							<span className="sidemenu__icon">{icon}</span>
							{label}
						</Link>
					))}
				</nav>
			</div>
			<div className="sidemenu__bottom">
				<Link to="/my-profile" className="sidemenu__profile-link">
					<User className="sidemenu__icon" size={22} />
					Profile
				</Link>
			</div>
		</aside>
	);
}
