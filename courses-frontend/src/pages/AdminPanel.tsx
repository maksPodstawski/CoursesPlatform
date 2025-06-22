import { useEffect, useState } from "react";
import { getCategories, getSubcategoriesByCategoryId } from "../services/categoryService";
import { getCourses } from "../services/courseService";
import { Folder, NotebookPen } from "lucide-react";
import AdminSidebar from "../components/AdminSidebar";
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
}

export default function AdminDashboard() {
    const [categories, setCategories] = useState<Category[]>([]);
    const [subcategories, setSubcategories] = useState<Subcategory[]>([]);
    const [courses, setCourses] = useState<Course[]>([]);
    const [expandedCategories, setExpandedCategories] = useState<Set<string>>(new Set());

    useEffect(() => {
        fetchData();
    }, []);

    const fetchData = async () => {
        try {
            const catData = await getCategories();
            setCategories(catData);

            const subMap: Subcategory[] = [];
            for (const cat of catData) {
                const subs = await getSubcategoriesByCategoryId(cat.id);
                subMap.push(...subs);
            }
            setSubcategories(subMap);

            const courseData = await getCourses();
            setCourses(courseData);
        } catch (err) {
            console.error("Fetch error:", err);
        }
    };

    const toggleCategory = (categoryId: string) => {
        setExpandedCategories((prev) => {
            const newSet = new Set(prev);
            if (newSet.has(categoryId)) {
                newSet.delete(categoryId);
            } else {
                newSet.add(categoryId);
            }
            return newSet;
        });
    };

    return (
        <div style={{ display: "flex" }}>
            <AdminSidebar />
            <div className="admin-content-wrapper">

                {/* === LEWA KOLUMNA === */}
                <div className="panel-column">
                    <div className="panel-card">
                        <h3 className="panel-title yellow">
                            <Folder size={20} /> Total Categories
                        </h3>
                        <p style={{ fontSize: "2rem", fontWeight: "bold", color: "#22c55e" }}>{categories.length}</p>
                    </div>

                    <div className="panel-card wide">
                        <h3 className="panel-title yellow">
                            <Folder size={20} /> Categories & Subcategories
                        </h3>
                        <ul className="category-list">
                            {categories.map((cat) => {
                                const subs = subcategories.filter(sub => sub.categoryId === cat.id);
                                const isExpanded = expandedCategories.has(cat.id);
                                return (
                                    <li key={cat.id} className="category-item">
                                        <div
                                            className="category-header"
                                            onClick={() => toggleCategory(cat.id)}
                                        >
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

                {/* === PRAWA KOLUMNA === */}
                <div className="panel-column">
                    <div className="panel-card">
                        <h3 className="panel-title yellow">
                            <NotebookPen size={20} /> Total Courses
                        </h3>
                        <p style={{ fontSize: "2rem", fontWeight: "bold", color: "#22c55e" }}>{courses.length}</p>
                    </div>

                    <div className="panel-card wide">
                        <h3 className="panel-title yellow">
                            <NotebookPen size={20} /> Courses
                        </h3>
                        <ul className="category-list">
                            {courses.map((course) => (
                                <li key={course.id} className="category-item">
                                    📘 {course.name}
                                </li>
                            ))}
                        </ul>
                    </div>
                </div>

            </div>
        </div>
    );
}
