import { useState, useEffect } from "react";
import AdminSidebar from "../../components/AdminSidebar";
import { adminService } from "../../services/adminService";
import { getCourses } from "../../services/courseService";
import { GraduationCap } from "lucide-react";
import "../../styles/Admin.css";

type Course = { id: string; name: string };

export default function AdminCoursesManagement() {
    const [courses, setCourses] = useState<Course[]>([]);
    const [selectedCourseId, setSelectedCourseId] = useState<string>("");
    const [deleteMsg, setDeleteMsg] = useState<string | null>(null);

    useEffect(() => {
        fetchCourses();
    }, []);

    const fetchCourses = async () => {
        const data = await getCourses();
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

    return (
        <div style={{ display: "flex" }}>
            <AdminSidebar />
            <div className="admin-content-wrapper">
                {/* Lewa kolumna: lista kursów */}
                <div className="panel-column">
                    <div className="panel-card wide">
                        <h3 className="panel-title yellow">📚 Courses</h3>
                        <ul className="item-list">
                            {courses.map(course => (
                                <li key={course.id} className="item-row">
                                    <GraduationCap size={16} className="icon-inline" />
                                    {course.name}
                                </li>
                            ))}
                        </ul>
                    </div>
                </div>

                {/* Prawa kolumna: usuwanie */}
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
            </div>
        </div>
    );
}
