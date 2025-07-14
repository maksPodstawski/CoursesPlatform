"use client"

import { useState, useEffect } from "react"
import { useNavigate } from "react-router-dom"
import AdminSidebar from "../../components/AdminSidebar"
import { adminService } from "../../services/adminService"
import { GraduationCap, Eye, EyeOff, Trash2, AlertTriangle, Settings, Filter } from "lucide-react"
import "../../styles/Admin.css"

type Course = { id: string; name: string; isHidden: boolean }

export default function AdminCoursesManagement() {
    const [courses, setCourses] = useState<Course[]>([])
    const [selectedCourseId, setSelectedCourseId] = useState<string>("")
    const [message, setMessage] = useState<{ type: string; text: string } | null>(null) // Unified message state
    const [loading, setLoading] = useState(true)
    const [showDeleteConfirm, setShowDeleteConfirm] = useState(false)
    const [filterType, setFilterType] = useState<"all" | "visible" | "hidden">("all")

    const navigate = useNavigate()

    useEffect(() => {
        const initialize = async () => {
            try {
                await adminService.fetchDashboard()
                await fetchCourses()
            } catch (err) {
                console.error("Unauthorized or error:", err)
                navigate("/")
            } finally {
                setLoading(false)
            }
        }

        initialize()
    }, [navigate])

    const fetchCourses = async () => {
        try {
            const data = await adminService.getAllCourses()
            setCourses(data)
        } catch (error) {
            console.error("Error fetching courses:", error)
            showMessage("danger", "Failed to load courses.")
        }
    }

    const showMessage = (type: string, text: string) => {
        setMessage({ type, text })
        setTimeout(() => {
            setMessage(null)
        }, 3000)
    }

    const handleDeleteCourse = async () => {
        if (!selectedCourseId) return

        try {
            await adminService.deleteCourse(selectedCourseId)
            showMessage("success", "✅ Course deleted successfully!")
            setSelectedCourseId("")
            setShowDeleteConfirm(false)
            fetchCourses()
        } catch (error: any) {
            showMessage("danger", "❌ " + (error.message || "Error deleting course."))
        }
    }

    const handleToggleVisibility = async (courseId: string) => {
        try {
            await adminService.toggleCourseVisibility(courseId)
            showMessage("success", "✅ Course visibility updated!")
            fetchCourses()
        } catch (error: any) {
            showMessage("danger", "❌ " + (error.message || "Error toggling visibility."))
        }
    }

    const handleBulkAction = async (action: "hideAll" | "showAll") => {
        const coursesToUpdate =
            action === "hideAll" ? courses.filter((c) => !c.isHidden) : courses.filter((c) => c.isHidden)

        if (coursesToUpdate.length === 0) {
            showMessage("info", `No courses to ${action === "hideAll" ? "hide" : "show"}.`)
            return
        }

        try {
            for (const course of coursesToUpdate) {
                await handleToggleVisibility(course.id)
            }
            showMessage("success", `✅ All selected courses have been ${action === "hideAll" ? "hidden" : "shown"}!`)
            fetchCourses()
        } catch (error: any) {
            showMessage("danger", `❌ Error performing bulk action: ${error.message || "Unknown error."}`)
        }
    }

    const visibleCourses = courses.filter((course) => !course.isHidden)
    const hiddenCourses = courses.filter((course) => course.isHidden)

    const filteredCourses = courses.filter((course) => {
        if (filterType === "visible") return !course.isHidden
        if (filterType === "hidden") return course.isHidden
        return true
    })

    if (loading) {
        return (
            <div className="admin-layout">
                <AdminSidebar />
                <div className="admin-content">
                    <div className="loading-container">
                        <div className="loading-spinner"></div>
                        <p>Loading courses...</p>
                    </div>
                </div>
            </div>
        )
    }

    return (
        <div className="admin-layout">
            <AdminSidebar />
            <div className="admin-content">
                {/* Header Section */}
                <div className="admin-header">
                    <div className="header-content">
                        <div className="header-text">
                            <h1>Course Management</h1>
                            <p>Manage course visibility and delete courses</p>
                        </div>
                        <div className="header-actions">
                            <div className="filter-group">
                                <Filter size={16} />
                                <select
                                    value={filterType}
                                    onChange={(e) => setFilterType(e.target.value as "all" | "visible" | "hidden")}
                                    className="filter-select"
                                >
                                    <option value="all">All Courses</option>
                                    <option value="visible">Visible Only</option>
                                    <option value="hidden">Hidden Only</option>
                                </select>
                            </div>
                        </div>
                    </div>
                </div>

                {/* Stats Section */}
                <div className="stats-section">
                    <div className="stats-grid">
                        <div className="stat-card">
                            <div className="stat-card-header">
                                <span className="stat-title">Total Courses</span>
                                <div className="stat-icon-wrapper">
                                    <GraduationCap size={20} className="stat-icon" />
                                </div>
                            </div>
                            <div className="stat-value">{courses.length}</div>
                            <div className="stat-subtitle">All courses in system</div>
                        </div>

                        <div className="stat-card">
                            <div className="stat-card-header">
                                <span className="stat-title">Visible Courses</span>
                                <div className="stat-icon-wrapper">
                                    <Eye size={20} className="stat-icon" />
                                </div>
                            </div>
                            <div className="stat-value">{visibleCourses.length}</div>
                            <div className="stat-subtitle">Active on platform</div>
                        </div>

                        <div className="stat-card">
                            <div className="stat-card-header">
                                <span className="stat-title">Hidden Courses</span>
                                <div className="stat-icon-wrapper">
                                    <EyeOff size={20} className="stat-icon danger" />
                                </div>
                            </div>
                            <div className="stat-value danger">{hiddenCourses.length}</div>
                            <div className="stat-subtitle">Not visible to users</div>
                        </div>
                    </div>
                </div>

                {/* Main Content Section */}
                <div className="main-section">
                    {message && <div className={`message ${message.type}`}>{message.text}</div>} {/* Unified message display */}
                    <div className="management-grid">
                        {/* Courses List */}
                        <div className="content-card courses-list-card">
                            <div className="card-header">
                                <div className="card-title">
                                    <GraduationCap size={20} />
                                    All Courses ({filteredCourses.length})
                                </div>
                                <div className="card-subtitle">Click the visibility button to toggle course visibility</div>
                            </div>
                            <div className="card-content">
                                {filteredCourses.length === 0 ? (
                                    <div className="empty-state">
                                        <GraduationCap size={48} className="empty-icon" />
                                        <h3 className="empty-title">No courses found</h3>
                                        <p className="empty-subtitle">
                                            {filterType === "all" ? "No courses have been created yet." : `No ${filterType} courses found.`}
                                        </p>
                                    </div>
                                ) : (
                                    <div className="courses-container">
                                        {filteredCourses.map((course) => (
                                            <div key={course.id} className="course-item">
                                                <div className="course-info">
                                                    <div className="course-details">
                                                        <h4 className="course-name">{course.name}</h4>
                                                        <p className="course-id">ID: {course.id}</p>
                                                    </div>
                                                </div>
                                                <div className="course-actions">
                          <span className={`status-badge ${course.isHidden ? "hidden" : "visible"}`}>
                            {course.isHidden ? "Hidden" : "Visible"}
                          </span>
                                                    <button
                                                        className={`icon-button ${course.isHidden ? "" : "active"}`}
                                                        onClick={() => handleToggleVisibility(course.id)}
                                                        title={course.isHidden ? "Show course" : "Hide course"}
                                                    >
                                                        {course.isHidden ? <Eye size={16} /> : <EyeOff size={16} />}
                                                    </button>
                                                </div>
                                            </div>
                                        ))}
                                    </div>
                                )}
                            </div>
                        </div>

                        {/* Management Actions */}
                        <div className="actions-column">
                            {" "}
                            <div className="content-card danger-card">
                                <div className="card-header">
                                    <div className="card-title danger">
                                        <Trash2 size={16} />
                                        Delete Course
                                    </div>
                                    <div className="card-subtitle">Permanently delete a course. This action cannot be undone.</div>
                                </div>
                                <div className="card-content">
                                    <div className="form">
                                        <div className="form-group">
                                            <label htmlFor="courseSelect">Select Course</label>
                                            <select
                                                id="courseSelect"
                                                value={selectedCourseId}
                                                onChange={(e) => setSelectedCourseId(e.target.value)}
                                                className="form-select"
                                            >
                                                <option value="">Choose course to delete</option>
                                                {courses.map((course) => (
                                                    <option key={course.id} value={course.id}>
                                                        {course.name}
                                                    </option>
                                                ))}
                                            </select>
                                        </div>

                                        <button
                                            className="btn btn-danger full-width"
                                            disabled={!selectedCourseId}
                                            onClick={() => setShowDeleteConfirm(true)}
                                        >
                                            <Trash2 size={16} />
                                            Delete Course
                                        </button>
                                    </div>
                                </div>
                            </div>
                            <div className="content-card">
                                <div className="card-header">
                                    <div className="card-title">
                                        <Settings size={16} />
                                        Quick Actions
                                    </div>
                                    <div className="card-subtitle">Bulk operations for course management</div>
                                </div>
                                <div className="card-content">
                                    <div className="quick-actions">
                                        <button
                                            className="btn btn-outline full-width"
                                            onClick={() => handleBulkAction("hideAll")}
                                            disabled={visibleCourses.length === 0}
                                        >
                                            <EyeOff size={16} />
                                            Hide All Visible ({visibleCourses.length})
                                        </button>
                                        <button
                                            className="btn btn-outline full-width"
                                            onClick={() => handleBulkAction("showAll")}
                                            disabled={hiddenCourses.length === 0}
                                        >
                                            <Eye size={16} />
                                            Show All Hidden ({hiddenCourses.length})
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                {showDeleteConfirm && (
                    <div className="modal-overlay" onClick={() => setShowDeleteConfirm(false)}>
                        <div className="modal" onClick={(e) => e.stopPropagation()}>
                            <div className="modal-header">
                                <div className="modal-title">
                                    <AlertTriangle size={20} className="danger" />
                                    Confirm Course Deletion
                                </div>
                            </div>
                            <div className="modal-content">
                                <p>
                                    Are you absolutely sure you want to delete this course? This action cannot be undone and will
                                    permanently remove all associated data from the servers.
                                </p>
                                <div className="warning-box">
                                    <AlertTriangle size={16} />
                                    <span>This will also remove all student progress, reviews, and course materials.</span>
                                </div>
                            </div>
                            <div className="modal-actions">
                                <button className="btn btn-outline" onClick={() => setShowDeleteConfirm(false)}>
                                    Cancel
                                </button>
                                <button className="btn btn-danger" onClick={handleDeleteCourse}>
                                    <Trash2 size={16} />
                                    Delete Course
                                </button>
                            </div>
                        </div>
                    </div>
                )}
            </div>
        </div>
    )
}
