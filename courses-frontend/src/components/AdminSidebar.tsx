import { Link, useLocation } from "react-router-dom"
import { LayoutDashboard, FolderCog, GraduationCap, MessageSquareMore, User, Settings } from "lucide-react"

const adminNavItems = [
    {
        label: "Dashboard",
        icon: LayoutDashboard,
        path: "/admin",
    },
    {
        label: "Categories",
        icon: FolderCog,
        path: "/admin/categories",
    },
    {
        label: "Courses",
        icon: GraduationCap,
        path: "/admin/courses",
    },
    {
        label: "Reviews",
        icon: MessageSquareMore,
        path: "/admin/reviews",
    },
]

export default function AdminSidebar() {
    const location = useLocation()

    return (
        <aside className="admin-sidebar">
            <div className="admin-sidebar-header">
                <div className="admin-sidebar-logo">
                    <Settings size={24} />
                    <h2>Admin Panel</h2>
                </div>
            </div>

            <nav className="admin-sidebar-nav">
                <div className="admin-nav-section">
                    <h3 className="admin-nav-title">Management</h3>
                    <ul className="admin-nav-list">
                        {adminNavItems.map((item) => {
                            const Icon = item.icon
                            const isActive = location.pathname === item.path

                            return (
                                <li key={item.label}>
                                    <Link to={item.path} className={`admin-nav-link ${isActive ? "active" : ""}`}>
                                        <Icon size={18} />
                                        <span>{item.label}</span>
                                    </Link>
                                </li>
                            )
                        })}
                    </ul>
                </div>
            </nav>

            <div className="admin-sidebar-footer">
                <Link to="/my-profile" className="admin-profile-link">
                    <User size={18} />
                    <span>Profile</span>
                </Link>
            </div>
        </aside>
    )
}
