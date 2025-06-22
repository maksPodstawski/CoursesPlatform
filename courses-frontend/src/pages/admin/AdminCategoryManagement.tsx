import { useState, useEffect } from "react";
import { adminService } from "../../services/adminService";
import { getCategories } from "../../services/categoryService";
import "../../styles/Admin.css";
import AdminSidebar from "../../components/AdminSidebar";

type Category = { id: string; name: string };

export default function AdminCategoriesSection() {
    const [categoryName, setCategoryName] = useState("");
    const [addCategoryMsg, setAddCategoryMsg] = useState<string | null>(null);
    const [categories, setCategories] = useState<Category[]>([]);
    const [selectedCategoryId, setSelectedCategoryId] = useState<string>("");
    const [deleteCategoryMsg, setDeleteCategoryMsg] = useState<string | null>(null);

    useEffect(() => {
        getCategories().then(setCategories).catch(() => setCategories([]));
    }, []);

    const handleAddCategory = async (e: React.FormEvent) => {
        e.preventDefault();
        setAddCategoryMsg(null);
        try {
            await adminService.addCategory(categoryName);
            setAddCategoryMsg("✅ Category added successfully!");
            setCategoryName("");
            const data = await getCategories();
            setCategories(data);
            if (data.length > 0) setSelectedCategoryId(data[0].id);
        } catch {
            setAddCategoryMsg("❌ Error adding category. Please try again.");
        }
    };

    const handleDeleteCategory = async (e: React.FormEvent) => {
        e.preventDefault();
        setDeleteCategoryMsg(null);
        try {
            await adminService.deleteCategory(selectedCategoryId);
            setDeleteCategoryMsg("✅ Category deleted successfully!");
            const data = await getCategories();
            setCategories(data);
            setSelectedCategoryId("");
        } catch {
            setDeleteCategoryMsg("❌ Error deleting category. Please try again.");
        }
    };

    return (
        <div style={{ display: "flex" }}>
            <AdminSidebar />

            <div className="admin-content-wrapper">
                <div className="panel-column">
                    <div className="panel-card wide">
                        <h3 className="panel-title yellow">📂 Existing Categories</h3>
                        <ul className="category-list">
                            {categories.length === 0 ? (
                                <li className="empty">No categories yet.</li>
                            ) : (
                                categories.map(cat => (
                                    <li key={cat.id} className="category-item">{cat.name}</li>
                                ))
                            )}
                        </ul>
                    </div>
                </div>

                <div className="panel-column">
                    <div className="panel-card">
                        <h3 className="panel-title purple">➕ Add New Category</h3>
                        <form className="panel-form" onSubmit={handleAddCategory}>
                            <input
                                type="text"
                                value={categoryName}
                                onChange={(e) => setCategoryName(e.target.value)}
                                placeholder="Category name"
                                required
                                className="panel-input"
                            />
                            <button type="submit" className="panel-button green">Add</button>
                        </form>
                        {addCategoryMsg && <p className="panel-msg">{addCategoryMsg}</p>}
                    </div>

                    <div className="panel-card">
                        <h3 className="panel-title violet">➖ Delete Category</h3>
                        <form className="panel-form" onSubmit={handleDeleteCategory}>
                            <select
                                className="panel-input"
                                value={selectedCategoryId}
                                onChange={(e) => setSelectedCategoryId(e.target.value)}
                                required
                            >
                                <option value="" disabled>Select category</option>
                                {categories.map((cat) => (
                                    <option key={cat.id} value={cat.id}>{cat.name}</option>
                                ))}
                            </select>
                            <button type="submit" className="panel-button green">Delete Category</button>
                        </form>
                        {deleteCategoryMsg && <p className="panel-msg">{deleteCategoryMsg}</p>}
                    </div>
                </div>
            </div>
        </div>
    );
}

