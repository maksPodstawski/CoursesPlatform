import { Link, useLocation } from "react-router-dom";
import { FolderCog, LayoutDashboard, User, GraduationCap, MessageSquareMore } from "lucide-react";
import "../styles/Sidebar.css";

const adminNavItems = [
    { label: "Dashboard", icon: <LayoutDashboard size={22} />, path: "/admin" },
    { label: "Category", icon: <FolderCog size={22} />, path: "/admin/categories" },
    { label: "Courses", icon: <GraduationCap size={22} />, path: "/admin/courses" },
    { label: "Reviews", icon: <MessageSquareMore size={22} />, path: "/admin/reviews" },
];

export default function AdminSidebar() {
    const location = useLocation();

    return (
        <aside className="sidemenu">
            <div className="sidemenu__top">
                <nav className="sidemenu__nav">
                    {adminNavItems.map(({ label, icon, path }) => (
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
                    Profil
                </Link>
            </div>
        </aside>
    );
}