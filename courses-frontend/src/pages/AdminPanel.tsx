import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { getCategories, getSubcategoriesByCategoryId } from "../services/categoryService";
import AdminSidebar from "../components/AdminSidebar";
import { adminService } from "../services/adminService.ts";
import { countToxicReviews } from "../services/toxicService";
import { Folder, NotebookPen, GraduationCap, MessageCircleWarning } from "lucide-react";
import "../styles/Admin.css";

interface Category {
    id: string;
    name: string;
}

interface Subcategory {
    id: string;
    name: string;
    categoryId: string;
}

interface Course {
    id: string;
    name: string;
    isHidden: boolean;
}

export default function AdminDashboard() {
    const [categories, setCategories] = useState<Category[]>([]);
    const [subcategories, setSubcategories] = useState<Subcategory[]>([]);
    const [courses, setCourses] = useState<Course[]>([]);
    const [expandedCategories, setExpandedCategories] = useState<Set<string>>(new Set());
    const [toxicCount, setToxicCount] = useState<number>(0);
    const navigate = useNavigate();

    useEffect(() => {
        const initialize = async () => {
            try {
                await adminService.fetchDashboard();
                await fetchData();
                const toxic = await countToxicReviews();
                setToxicCount(toxic);
            } catch (err) {
                console.error("Unauthorized or fetch error:", err);
                navigate("/");
            }
        };
        initialize();
    }, []);

    const fetchData = async () => {
        const catData = await getCategories();
        setCategories(catData);

        const subMap: Subcategory[] = [];
        for (const cat of catData) {
            const subs = await getSubcategoriesByCategoryId(cat.id);
            subMap.push(...subs);
        }
        setSubcategories(subMap);

        const courseData = await adminService.getAllCourses();
        setCourses(courseData);
    };

    const toggleCategory = (categoryId: string) => {
        setExpandedCategories((prev) => {
            const newSet = new Set(prev);
            newSet.has(categoryId) ? newSet.delete(categoryId) : newSet.add(categoryId);
            return newSet;
        });
    };

    return (
        <div style={{ display: "flex" }}>
            <AdminSidebar />
            <div className="admin-content-wrapper">

                {/* LEWA KOLUMNA */}
                <div className="panel-column">
                    <div className="panel-card">
                        <h3 className="panel-title yellow">
                            <Folder size={20} /> Total Categories
                        </h3>
                        <p style={{ fontSize: "2rem", fontWeight: "bold", color: "#22c55e" }}>
                            {categories.length}
                        </p>
                    </div>

                    <div className="panel-card wide">
                        <h3 className="panel-title yellow">
                            <Folder size={20} /> Categories & Subcategories
                        </h3>
                        <ul className="category-list">
                            {categories.map((cat) => {
                                const subs = subcategories.filter((sub) => sub.categoryId === cat.id);
                                const isExpanded = expandedCategories.has(cat.id);
                                return (
                                    <li key={cat.id} className="category-item">
                                        <div className="category-header" onClick={() => toggleCategory(cat.id)}>
                                            <span className="category-name">{cat.name}</span>
                                            <span className="subcategory-info">
                                                <span className="subcategory-count">{subs.length}</span>
                                                <span className="subcategory-label">
                                                    {subs.length === 1 ? "subcategory" : "subcategories"}
                                                </span>
                                                <span className="arrow-icon">{isExpanded ? "▲" : "▼"}</span>
                                            </span>
                                        </div>
                                        {isExpanded && subs.length > 0 && (
                                            <ul className="subcategory-list">
                                                {subs.map((sub) => (
                                                    <li key={sub.id} className="subcategory-item">
                                                        🧩 {sub.name}
                                                    </li>
                                                ))}
                                            </ul>
                                        )}
                                    </li>
                                );
                            })}
                        </ul>
                    </div>
                </div>

                {/* PRAWA KOLUMNA */}
                <div className="panel-column">
                    <div className="panel-card">
                        <h3 className="panel-title yellow">
                            <NotebookPen size={20} /> Total Courses
                        </h3>
                        <p style={{ fontSize: "2rem", fontWeight: "bold", color: "#22c55e" }}>
                            {courses.length}
                        </p>
                    </div>

                    <div className="panel-card wide">
                        <h3 className="panel-title yellow">
                            <NotebookPen size={20} /> Courses
                        </h3>
                        <ul className="item-list">
                            {courses.map((course) => (
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

                {/* NOWA KOLUMNA – TOXIC COMMENTS */}
                <div className="panel-column">
                    <div className="panel-card">
                        <h3 className="panel-title yellow">
                            <MessageCircleWarning size={20} /> Toxic Comments
                        </h3>
                        <p
                            style={{
                                fontSize: "2rem",
                                fontWeight: "bold",
                                color: toxicCount === 0 ? "#22c55e" : "red"
                            }}
                        >
                            {toxicCount}
                        </p>
                    </div>
                </div>
            </div>
        </div>
    );
}
