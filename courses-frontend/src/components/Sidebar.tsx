import { Link, useLocation } from "react-router-dom";
import { Book, MessageCircle, Plus, BarChart, User , Mail} from "lucide-react";
import "../styles/Sidebar.css";

const navItems = [
	{ label: "My Profile", icon: <User size={22} />, path: "/my-profile" },
	{ label: "Create Course", icon: <Plus size={22} />, path: "/add-course" },
	{ label: "Created Courses", icon: <Book size={22} />, path: "/creator-courses" },
	{ label: "Student Chats", icon: <MessageCircle size={22} />, path: "/creatorpanel/chats" },
	{ label: "Invitations", icon: <Mail size={22} />, path: "/invitations" },
	{ label: "Analytics", icon: <BarChart size={22} />, path: "/analytics" },
];

export default function Sidebar() {
	const location = useLocation();

	return (
		<aside className="sidemenu">
			<div className="sidemenu__top">
				<nav className="sidemenu__nav">
					{navItems.map(({ label, icon, path }) => (
						<Link key={label} to={path} className={`sidemenu__link ${location.pathname === path ? "active" : ""}`}>
							<span className="sidemenu__icon">{icon}</span>
							{label}
						</Link>
					))}
				</nav>
			</div>
		</aside>
	);
}
