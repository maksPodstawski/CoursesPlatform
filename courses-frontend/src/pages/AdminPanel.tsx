import { useAuth } from "../context/AuthContext.tsx";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { adminService } from "../services/adminService";
import { getCategories } from "../services/categoryService.ts";
import { getCourses } from "../services/courseService.ts";
import { getReviewsByCourse, deleteReviews } from "../services/reviewService";
import { config } from "../config.ts";
import "../styles/Admin.css";

type Category = {
    id: string;
    name: string;
};

type Subcategory = {
    id: string;
    name: string;
    categoryId: string;
};

const AdminPanel = () => {
    const { isLoggedIn } = useAuth();
    const navigate = useNavigate();
    const [error, setError] = useState<string | null>(null);
    const [categoryName, setCategoryName] = useState("");
    const [addCategoryMsg, setAddCategoryMsg] = useState<string | null>(null);
    const [subcategoryName, setSubcategoryName] = useState("");
    const [addSubcategoryMsg, setAddSubcategoryMsg] = useState<string | null>(null);
    const [categories, setCategories] = useState<Category[]>([]);
    const [subcategories, setSubcategories] = useState<Subcategory[]>([]);
    const [selectedCategoryId, setSelectedCategoryId] = useState<string>("");
    const [selectedSubcategoryId, setSelectedSubcategoryId] = useState<string>("");
    const [selectedCourseId, setSelectedCourseId] = useState<string>("");
    const [courses, setCourses] = useState<{ id: string; name: string }[]>([]);
    const [deleteCourseMsg, setDeleteCourseMsg] = useState<string | null>(null);
    const [deleteReviewsMsg, setDeleteReviewsMsg] = useState<string | null>(null);
    const [deleteCategoryMsg, setDeleteCategoryMsg] = useState<string | null>(null);
    const [deleteSubcategoryMsg, setDeleteSubcategoryMsg] = useState<string | null>(null);
    const [reviews, setReviews] = useState<{ id: string; content: string }[]>([]);
    const [selectedReviewIds, setSelectedReviewIds] = useState<string[]>([]);

    useEffect(() => {
        const checkDashboard = async () => {
            try {
                await adminService.fetchDashboard();
            } catch (err) {
                setError("Brak dostępu. Przekierowanie na stronę główną.");
                navigate("/");
            }
        };

        if (localStorage.getItem("isLoggedIn")) {
            checkDashboard();
        } else {
            navigate("/login");
        }
    }, [isLoggedIn, navigate]);

    useEffect(() => {
        const fetchCategories = async () => {
            try {
                const data = await getCategories();
                setCategories(data);
            } catch (err) {
                setCategories([]);
            }
        };
        fetchCategories();
    }, []);

    useEffect(() => {
        if (!selectedCategoryId) {
            setSubcategories([]);
            return;
        }
        const fetchSubcategories = async () => {
            try {
                const response = await fetch(`${config.apiBaseUrl}/api/categories/${selectedCategoryId}/subcategories`);
                if (!response.ok) throw new Error();
                const data = await response.json();
                setSubcategories(data);
            } catch {
                setSubcategories([]);
            }
        };
        fetchSubcategories();
    }, [selectedCategoryId]);

    useEffect(() => {
        const fetchCoursesList = async () => {
            try {
                const data = await getCourses();
                setCourses(data);
            } catch {
                setCourses([]);
            }
        };
        fetchCoursesList();
    }, []);

    useEffect(() => {
        if (!selectedCourseId) {
            setReviews([]);
            setSelectedReviewIds([]);
            return;
        }
        const fetchReviews = async () => {
            try {
                const data = await getReviewsByCourse(selectedCourseId);
                setReviews(data);
            } catch {
                setReviews([]);
            }
        };
        fetchReviews();
    }, [selectedCourseId]);

    const handleAddCategory = async (e: React.FormEvent) => {
        e.preventDefault();
        setAddCategoryMsg(null);
        try {
            await adminService.addCategory(categoryName);
            setAddCategoryMsg("Category added successfully!");
            setCategoryName("");
            const data = await getCategories();
            setCategories(data);
            if (data.length > 0) setSelectedCategoryId(data[0].id);
        } catch (err) {
            setAddCategoryMsg("Error adding category. Please try again.");
        }
    };

    const handleAddSubcategory = async (e: React.FormEvent) => {
        e.preventDefault();
        setAddSubcategoryMsg(null);
        try {
            await adminService.addSubcategory(subcategoryName, selectedCategoryId);
            setAddSubcategoryMsg("Subcategory added successfully!");
            setSubcategoryName("");
            // odśwież podkategorie
            const response = await fetch(`${config.apiBaseUrl}/api/categories/${selectedCategoryId}/subcategories`);
            if (response.ok) {
                const data = await response.json();
                setSubcategories(data);
            }
        } catch (err) {
            setAddSubcategoryMsg("Error adding subcategory. Please try again.");
        }
    };

    const handleDeleteCourse = async (e: React.FormEvent) => {
        e.preventDefault();
        setDeleteCourseMsg(null);
        try {
            await adminService.deleteCourse(selectedCourseId);
            setDeleteCourseMsg("Course deleted successfully!");
            const data = await getCourses();
            setCourses(data);
            setSelectedCourseId("");
        } catch {
            setDeleteCourseMsg("Error deleting course. Please try again.");
        }
    };

    const handleDeleteCategory = async (e: React.FormEvent) => {
        e.preventDefault();
        setDeleteCategoryMsg(null);
        try {
            await adminService.deleteCategory(selectedCategoryId);
            setDeleteCategoryMsg("Category deleted successfully!");
            const data = await getCategories();
            setCategories(data);
            setSelectedCategoryId("");
            setSubcategories([]);
        } catch {
            setDeleteCategoryMsg("Error deleting category. Please try again.");
        }
    };

    const handleDeleteSubcategory = async (e: React.FormEvent) => {
        e.preventDefault();
        setDeleteSubcategoryMsg(null);
        try {
            await adminService.deleteSubcategory(selectedSubcategoryId);
            setDeleteSubcategoryMsg("Subcategory deleted successfully!");
            // odśwież podkategorie
            const response = await fetch(`${config.apiBaseUrl}/api/categories/${selectedCategoryId}/subcategories`);
            if (response.ok) {
                const data = await response.json();
                setSubcategories(data);
            }
            setSelectedSubcategoryId("");
        } catch {
            setDeleteSubcategoryMsg("Error deleting subcategory. Please try again.");
        }
    };

    const handleReviewCheckbox = (reviewId: string) => {
        setSelectedReviewIds(prev =>
            prev.includes(reviewId)
                ? prev.filter(id => id !== reviewId)
                : [...prev, reviewId]
        );
    };

    const handleDeleteSelectedReviews = async (e: React.FormEvent) => {
        e.preventDefault();
        setDeleteReviewsMsg(null);
        try {
            await deleteReviews(selectedReviewIds);
            setDeleteReviewsMsg("Selected reviews deleted!");
            setReviews(reviews.filter(r => !selectedReviewIds.includes(r.id)));
            setSelectedReviewIds([]);
        } catch {
            setDeleteReviewsMsg("Error deleting reviews. Please try again.");
        }
    };

    if (error) {
        return <p>{error}</p>;
    }

    return (
        <div className="admin-panel-row">
            <div className="admin-panel-section">
                <h2>➕ Add New Category</h2>
                <form className="admin-form" onSubmit={handleAddCategory}>
                    <input
                        type="text"
                        value={categoryName}
                        onChange={e => setCategoryName(e.target.value)}
                        placeholder="Category name"
                        required
                        className="admin-input"
                    />
                    <button type="submit" className="admin-btn">Add</button>
                </form>
                {addCategoryMsg && <p className="admin-msg">{addCategoryMsg}</p>}
            </div>

            <div className="admin-panel-section">
                <h2>➖ Delete Category</h2>
                <form className="admin-form" onSubmit={handleDeleteCategory}>
                    <select
                        className="admin-input"
                        value={selectedCategoryId}
                        onChange={e => setSelectedCategoryId(e.target.value)}
                        required
                    >
                        <option value="" disabled className="placeholder-option">
                            Select category
                        </option>
                        {categories.map(cat => (
                            <option key={cat.id} value={cat.id}>
                                {cat.name}
                            </option>
                        ))}
                    </select>
                    <button type="submit" className="admin-btn">Delete Category</button>
                </form>
                {deleteCategoryMsg && <p className="admin-msg">{deleteCategoryMsg}</p>}
            </div>

            <div className="admin-panel-section">
                <h2>➕ Add New SubCategory</h2>
                <form className="admin-form" onSubmit={handleAddSubcategory}>
                    <input
                        type="text"
                        value={subcategoryName}
                        onChange={e => setSubcategoryName(e.target.value)}
                        placeholder="Subcategory name"
                        required
                        className="admin-input"
                    />
                    <select
                        className="admin-input"
                        value={selectedCategoryId}
                        onChange={e => setSelectedCategoryId(e.target.value)}
                        required
                    >
                        <option value="" disabled className="placeholder-option">
                            Category name
                        </option>
                        {categories.map(cat => (
                            <option key={cat.id} value={cat.id}>
                                {cat.name}
                            </option>
                        ))}
                    </select>
                    <button type="submit" className="admin-btn">Add</button>
                </form>
                {addSubcategoryMsg && <p className="admin-msg">{addSubcategoryMsg}</p>}
            </div>

            <div className="admin-panel-section">
                <h2>➖ Delete Subcategory</h2>
                <form className="admin-form" onSubmit={handleDeleteSubcategory}>
                    <select
                        className="admin-input"
                        value={selectedCategoryId}
                        onChange={e => setSelectedCategoryId(e.target.value)}
                        required
                    >
                        <option value="" disabled className="placeholder-option">
                            Select category
                        </option>
                        {categories.map(cat => (
                            <option key={cat.id} value={cat.id}>
                                {cat.name}
                            </option>
                        ))}
                    </select>
                    <select
                        className="admin-input"
                        value={selectedSubcategoryId}
                        onChange={e => setSelectedSubcategoryId(e.target.value)}
                        required
                        disabled={!selectedCategoryId}
                    >
                        <option value="" disabled className="placeholder-option">
                            Select subcategory
                        </option>
                        {subcategories.map(sub => (
                            <option key={sub.id} value={sub.id}>
                                {sub.name}
                            </option>
                        ))}
                    </select>
                    <button type="submit" className="admin-btn" disabled={!selectedSubcategoryId}>
                        Delete Subcategory
                    </button>
                </form>
                {deleteSubcategoryMsg && <p className="admin-msg">{deleteSubcategoryMsg}</p>}
            </div>

            <div className="admin-panel-section">
                <h2>➖ Delete Course</h2>
                <form className="admin-form" onSubmit={handleDeleteCourse}>
                    <select
                        className="admin-input"
                        value={selectedCourseId}
                        onChange={e => setSelectedCourseId(e.target.value)}
                        required
                    >
                        <option value="" disabled className="placeholder-option">
                            Select course
                        </option>
                        {courses.map(course => (
                            <option key={course.id} value={course.id}>
                                {course.name}
                            </option>
                        ))}
                    </select>
                    <button type="submit" className="admin-btn">Delete Course</button>
                </form>
                {deleteCourseMsg && <p className="admin-msg">{deleteCourseMsg}</p>}
            </div>

            <div className="admin-panel-section">
                <h2>➖ Delete Selected Reviews</h2>
                <form className="admin-form" onSubmit={handleDeleteSelectedReviews}>
                    <select
                        className="admin-input"
                        value={selectedCourseId}
                        onChange={e => setSelectedCourseId(e.target.value)}
                        required
                    >
                        <option value="" disabled className="placeholder-option">
                            Select course
                        </option>
                        {courses.map(course => (
                            <option key={course.id} value={course.id}>
                                {course.name}
                            </option>
                        ))}
                    </select>
                    <div style={{ maxHeight: 200, overflowY: "auto", margin: "1em 0" }}>
                        {selectedCourseId && reviews.length === 0 ? (
                            <p style={{ color: "#aaa", textAlign: "center" }}>Brak recenzji dla wybranego kursu.</p>
                        ) : (
                            reviews.map(review => (
                                <label key={review.id} style={{ display: "block" }}>
                                    <input
                                        type="checkbox"
                                        checked={selectedReviewIds.includes(review.id)}
                                        onChange={() => handleReviewCheckbox(review.id)}
                                    />
                                    {review.content}
                                </label>
                            ))
                        )}
                    </div>
                    <button type="submit" className="admin-btn" disabled={selectedReviewIds.length === 0}>
                        Delete Selected Reviews
                    </button>
                </form>
                {deleteReviewsMsg && <p className="admin-msg">{deleteReviewsMsg}</p>}
            </div>
        </div>
    );
};

export default AdminPanel;