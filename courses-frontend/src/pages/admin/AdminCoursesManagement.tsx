import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import AdminSidebar from "../../components/AdminSidebar";
import { adminService } from "../../services/adminService";
import { GraduationCap } from "lucide-react";
import "../../styles/Admin.css";

type Course = { id: string; name: string; isHidden: boolean };

export default function AdminCoursesManagement() {
    const [courses, setCourses] = useState<Course[]>([]);
    const [selectedCourseId, setSelectedCourseId] = useState<string>("");
    const [deleteMsg, setDeleteMsg] = useState<string | null>(null);
    const [toggleMsg, setToggleMsg] = useState<string | null>(null);

    const navigate = useNavigate();

    useEffect(() => {
        const initialize = async () => {
            try {
                await adminService.fetchDashboard(); // Auth check
                await fetchCourses();
            } catch (err) {
                console.error("Unauthorized or error:", err);
                navigate("/"); // Redirect if no access
            }
        };

        initialize();
    }, []);

    const fetchCourses = async () => {
        const data = await adminService.getAllCourses(); // All courses, even hidden
        setCourses(data);
    };

    const handleDeleteCourse = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!selectedCourseId) return;

        const confirmDelete = window.confirm("Are you sure you want to delete this course?");
        if (!confirmDelete) return;

        setDeleteMsg(null);
        try {
            await adminService.deleteCourse(selectedCourseId);
            setDeleteMsg("✅ Course deleted!");
            setSelectedCourseId("");
            fetchCourses();
        } catch {
            setDeleteMsg("❌ Error deleting course.");
        }
    };

    const handleToggleVisibility = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!selectedCourseId) return;

        setToggleMsg(null);
        try {
            await adminService.toggleCourseVisibility(selectedCourseId);
            setToggleMsg("✅ Visibility updated!");
            fetchCourses();
        } catch {
            setToggleMsg("❌ Error toggling visibility.");
        }
    };

    return (
        <div style={{ display: "flex" }}>
            <AdminSidebar />
            <div className="admin-content-wrapper">
                {/* Lista kursów */}
                <div className="panel-column">
                    <div className="panel-card wide">
                        <h3 className="panel-title yellow">📚 Courses</h3>
                        <ul className="item-list">
                            {courses.map(course => (
                                <li key={course.id} className="item-row">
                                    <GraduationCap size={16} className="icon-inline" />
                                    {course.name}
                                    <span
                                        style={{
                                            marginLeft: "auto",
                                            fontSize: "0.8em",
                                            color: course.isHidden ? "#aaa" : "limegreen"
                                        }}
                                    >
                                        {course.isHidden ? "Hidden" : "Visible"}
                                    </span>
                                </li>
                            ))}
                        </ul>
                    </div>
                </div>

                {/* Usuwanie kursu */}
                <div className="panel-column">
                    <div className="panel-card">
                        <h3 className="panel-title violet">➖ Delete Course</h3>
                        <form className="panel-form" onSubmit={handleDeleteCourse}>
                            <select
                                value={selectedCourseId}
                                onChange={e => setSelectedCourseId(e.target.value)}
                                className="panel-input"
                                required
                            >
                                <option value="" disabled>Wybierz kurs</option>
                                {courses.map(course => (
                                    <option key={course.id} value={course.id}>
                                        {course.name}
                                    </option>
                                ))}
                            </select>
                            <button
                                type="submit"
                                className="panel-button green"
                                disabled={!selectedCourseId}
                            >
                                Delete
                            </button>
                        </form>
                        {deleteMsg && <p className="panel-msg">{deleteMsg}</p>}
                    </div>
                </div>

                {/* Przełączanie widoczności */}
                <div className="panel-column">
                    <div className="panel-card">
                        <h3 className="panel-title white">🫣 Toggle Visibility</h3>
                        <form className="panel-form" onSubmit={handleToggleVisibility}>
                            <select
                                value={selectedCourseId}
                                onChange={e => setSelectedCourseId(e.target.value)}
                                className="panel-input"
                                required
                            >
                                <option value="" disabled>Wybierz kurs</option>
                                {courses.map(course => (
                                    <option key={course.id} value={course.id}>
                                        {course.name}
                                    </option>
                                ))}
                            </select>
                            <button
                                type="submit"
                                className="panel-button white"
                                disabled={!selectedCourseId}
                            >
                                Toggle
                            </button>
                        </form>
                        {toggleMsg && <p className="panel-msg">{toggleMsg}</p>}
                    </div>
                </div>
            </div>
        </div>
    );
}
