import { useEffect, useState } from "react"
import { useNavigate } from "react-router-dom"
import { getCategories, getSubcategoriesByCategoryId } from "../services/categoryService"
import AdminSidebar from "../components/AdminSidebar"
import { adminService } from "../services/adminService"
import { countToxicReviews } from "../services/toxicService"
import {
    Folder,
    GraduationCap,
    MessageCircleWarning,
    BookOpen,
    AlertTriangle,
    ChevronDown,
    ChevronRight,
} from "lucide-react"
import '../styles/Admin.css';

interface Category {
    id: string
    name: string
}

interface Subcategory {
    id: string
    name: string
    categoryId: string
}

interface Course {
    id: string
    name: string
    isHidden: boolean
}

export default function AdminPanel() {
    const [categories, setCategories] = useState<Category[]>([])
    const [subcategories, setSubcategories] = useState<Subcategory[]>([])
    const [courses, setCourses] = useState<Course[]>([])
    const [toxicCount, setToxicCount] = useState<number>(0)
    const [loading, setLoading] = useState(true)
    const [expandedCategories, setExpandedCategories] = useState<Set<string>>(new Set())
    const navigate = useNavigate()

    useEffect(() => {
        const initialize = async () => {
            try {
                await adminService.fetchDashboard()
                await fetchData()
                const toxic = await countToxicReviews()
                setToxicCount(toxic)
            } catch (err) {
                console.error("Unauthorized or fetch error:", err)
                navigate("/")
            } finally {
                setLoading(false)
            }
        }
        initialize()
    }, [navigate])

    const fetchData = async () => {
        const catData = await getCategories()
        setCategories(catData)

        const subMap: Subcategory[] = []
        for (const cat of catData) {
            const subs = await getSubcategoriesByCategoryId(cat.id)
            subMap.push(...subs)
        }
        setSubcategories(subMap)

        const courseData = await adminService.getAllCourses()
        setCourses(courseData)
    }

    const toggleCategory = (categoryId: string) => {
        setExpandedCategories((prev) => {
            const newSet = new Set(prev)
            newSet.has(categoryId) ? newSet.delete(categoryId) : newSet.add(categoryId)
            return newSet
        })
    }

    const visibleCourses = courses.filter((course) => !course.isHidden)
    const hiddenCourses = courses.filter((course) => course.isHidden)

    if (loading) {
        return (
            <div className="admin-layout">
                <AdminSidebar />
                <div className="admin-content">
                    <div className="loading-container">
                        <div className="loading-spinner"></div>
                        <p>Loading dashboard...</p>
                    </div>
                </div>
            </div>
        )
    }

    return (
        <div className="admin-layout">
            <AdminSidebar />
            <div className="admin-content">
                <div className="admin-header">
                    <h1>Admin Dashboard</h1>
                    <p>Overview of your platform's content and activity</p>
                </div>

                {/* Stats Cards */}
                <div className="stats-grid">
                    <div className="stat-card">
                        <div className="stat-card-header">
                            <span className="stat-title">Total Categories</span>
                            <Folder size={20} className="stat-icon" />
                        </div>
                        <div className="stat-value">{categories.length}</div>
                        <div className="stat-subtitle">{subcategories.length} subcategories</div>
                    </div>

                    <div className="stat-card">
                        <div className="stat-card-header">
                            <span className="stat-title">Total Courses</span>
                            <GraduationCap size={20} className="stat-icon" />
                        </div>
                        <div className="stat-value">{courses.length}</div>
                        <div className="stat-subtitle">
                            {visibleCourses.length} visible, {hiddenCourses.length} hidden
                        </div>
                    </div>

                    <div className="stat-card">
                        <div className="stat-card-header">
                            <span className="stat-title">Visible Courses</span>
                            <BookOpen size={20} className="stat-icon" />
                        </div>
                        <div className="stat-value success">{visibleCourses.length}</div>
                        <div className="stat-subtitle">Active on platform</div>
                    </div>

                    <div className="stat-card">
                        <div className="stat-card-header">
                            <span className="stat-title">Toxic Comments</span>
                            <MessageCircleWarning size={20} className="stat-icon" />
                        </div>
                        <div className={`stat-value ${toxicCount > 0 ? "danger" : "success"}`}>{toxicCount}</div>
                        <div className="stat-subtitle">Requiring review</div>
                    </div>
                </div>

                {/* Alert for toxic comments */}
                {toxicCount > 0 && (
                    <div className="alert alert-warning">
                        <AlertTriangle size={16} />
                        <div>
                            <strong>Attention needed:</strong> You have {toxicCount} toxic comment{toxicCount !== 1 ? "s" : ""} that
                            need{toxicCount === 1 ? "s" : ""} review.
                            <a href="/admin/reviews" className="alert-link">
                                Review now →
                            </a>
                        </div>
                    </div>
                )}

                <div className="content-grid">
                    {/* Categories Overview */}
                    <div className="content-card">
                        <div className="card-header">
                            <div className="card-title">
                                <Folder size={20} />
                                Categories Overview
                            </div>
                            <div className="card-subtitle">Manage your course categories and subcategories</div>
                        </div>
                        <div className="card-content">
                            <div className="scrollable-content">
                                {categories.map((category) => {
                                    const subs = subcategories.filter((sub) => sub.categoryId === category.id)
                                    const isExpanded = expandedCategories.has(category.id)

                                    return (
                                        <div key={category.id} className="category-item">
                                            <div className="category-header" onClick={() => toggleCategory(category.id)}>
                                                <div className="category-info">
                                                    <h4>{category.name}</h4>
                                                    <span className="badge">
                            {subs.length} sub{subs.length !== 1 ? "s" : ""}
                          </span>
                                                </div>
                                                {isExpanded ? <ChevronDown size={16} /> : <ChevronRight size={16} />}
                                            </div>
                                            {isExpanded && subs.length > 0 && (
                                                <div className="subcategory-list">
                                                    {subs.slice(0, 3).map((sub) => (
                                                        <span key={sub.id} className="subcategory-tag">
                              {sub.name}
                            </span>
                                                    ))}
                                                    {subs.length > 3 && <span className="subcategory-tag more">+{subs.length - 3} more</span>}
                                                </div>
                                            )}
                                        </div>
                                    )
                                })}
                            </div>
                        </div>
                    </div>

                    {/* Courses Overview */}
                    <div className="content-card">
                        <div className="card-header">
                            <div className="card-title">
                                <GraduationCap size={20} />
                                Courses Overview
                            </div>
                            <div className="card-subtitle">Monitor your course visibility and status</div>
                        </div>
                        <div className="card-content">
                            <div className="scrollable-content">
                                {courses.map((course) => (
                                    <div key={course.id} className="course-item">
                                        <div className="course-info">
                                            <GraduationCap size={16} />
                                            <span>{course.name}</span>
                                        </div>
                                        <span className={`status-badge ${course.isHidden ? "hidden" : "visible"}`}>
                      {course.isHidden ? "Hidden" : "Visible"}
                    </span>
                                    </div>
                                ))}
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    )
}
