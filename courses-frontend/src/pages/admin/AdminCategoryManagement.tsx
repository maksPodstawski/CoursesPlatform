import { useState, useEffect } from "react";
import { adminService } from "../../services/adminService";
import {
    getCategories,
    getSubcategoriesByCategoryId
} from "../../services/categoryService";
import "../../styles/Admin.css";
import AdminSidebar from "../../components/AdminSidebar";

type Category = { id: string; name: string };
type Subcategory = { id: string; name: string; categoryId: string };

export default function AdminCategoryManagement() {
    const [categories, setCategories] = useState<Category[]>([]);
    const [subcategories, setSubcategories] = useState<Subcategory[]>([]);
    const [expandedCategories, setExpandedCategories] = useState<Set<string>>(new Set());

    const [selectedCategoryId, setSelectedCategoryId] = useState<string>("");
    const [selectedSubcategoryId, setSelectedSubcategoryId] = useState<string>("");

    const [categoryName, setCategoryName] = useState("");
    const [subcategoryName, setSubcategoryName] = useState("");

    const [addCategoryMsg, setAddCategoryMsg] = useState<string | null>(null);
    const [addSubcategoryMsg, setAddSubcategoryMsg] = useState<string | null>(null);
    const [deleteCategoryMsg, setDeleteCategoryMsg] = useState<string | null>(null);
    const [deleteSubcategoryMsg, setDeleteSubcategoryMsg] = useState<string | null>(null);

    useEffect(() => {
        fetchAll();
    }, []);

    useEffect(() => {
        if (!selectedCategoryId) return;

        getSubcategoriesByCategoryId(selectedCategoryId)
            .then(setSubcategories)
            .catch(() => setSubcategories([]));
    }, [selectedCategoryId]);

    const fetchAll = async () => {
        const cats = await getCategories();
        setCategories(cats);

        const allSub = await Promise.all(
            cats.map((cat: { id: string }) => getSubcategoriesByCategoryId(cat.id))
        );

        const mergedSub = allSub.flat();
        setSubcategories(mergedSub);
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

    const handleAddCategory = async (e: React.FormEvent) => {
        e.preventDefault();
        setAddCategoryMsg(null);
        try {
            await adminService.addCategory(categoryName);
            setAddCategoryMsg("✅ Category added!");
            setCategoryName("");
            fetchAll();
        } catch {
            setAddCategoryMsg("❌ Error adding category.");
        }
    };

    const handleAddSubcategory = async (e: React.FormEvent) => {
        e.preventDefault();
        setAddSubcategoryMsg(null);
        try {
            await adminService.addSubcategory(subcategoryName, selectedCategoryId);
            setSubcategoryName("");
            setAddSubcategoryMsg("✅ Subcategory added!");
            fetchAll();
        } catch {
            setAddSubcategoryMsg("❌ Error adding subcategory.");
        }
    };

    const handleDeleteCategory = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            await adminService.deleteCategory(selectedCategoryId);
            setDeleteCategoryMsg("✅ Category deleted!");
            setSelectedCategoryId("");
            fetchAll();
        } catch {
            setDeleteCategoryMsg("❌ Error deleting category.");
        }
    };

    const handleDeleteSubcategory = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            await adminService.deleteSubcategory(selectedSubcategoryId);
            setDeleteSubcategoryMsg("✅ Subcategory deleted!");
            setSelectedSubcategoryId("");
            fetchAll();
        } catch {
            setDeleteSubcategoryMsg("❌ Error deleting subcategory.");
        }
    };

    return (
        <div style={{ display: "flex" }}>
            <AdminSidebar />
            <div className="admin-content-wrapper">

                {/* LEWA KOLUMNA – rozwijalna lista */}
                <div className="panel-column">
                    <div className="panel-card wide">
                        <h3 className="panel-title yellow">📂 Categories & Subcategories</h3>
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
                                                {subs.map(sub => (
                                                    <li key={sub.id} className="subcategory-item">🧩 {sub.name}</li>
                                                ))}
                                            </ul>
                                        )}
                                    </li>
                                );
                            })}
                        </ul>
                    </div>
                </div>

                {/* ŚRODKOWA KOLUMNA – dodawanie */}
                <div className="panel-column">
                    <div className="panel-card">
                        <h3 className="panel-title purple">➕ Add New Category</h3>
                        <form className="panel-form" onSubmit={handleAddCategory}>
                            <input
                                type="text"
                                value={categoryName}
                                onChange={(e) => setCategoryName(e.target.value)}
                                placeholder="Category name"
                                className="panel-input"
                                required
                            />
                            <button type="submit" className="panel-button green">Add</button>
                        </form>
                        {addCategoryMsg && <p className="panel-msg">{addCategoryMsg}</p>}
                    </div>

                    <div className="panel-card">
                        <h3 className="panel-title purple">➕ Add New Subcategory</h3>
                        <form className="panel-form" onSubmit={handleAddSubcategory}>
                            <input
                                type="text"
                                value={subcategoryName}
                                onChange={(e) => setSubcategoryName(e.target.value)}
                                placeholder="Subcategory name"
                                className="panel-input"
                                required
                            />
                            <select
                                value={selectedCategoryId}
                                onChange={(e) => setSelectedCategoryId(e.target.value)}
                                className="panel-input"
                                required
                            >
                                <option value="" disabled>Select category</option>
                                {categories.map((cat) => (
                                    <option key={cat.id} value={cat.id}>{cat.name}</option>
                                ))}
                            </select>
                            <button type="submit" className="panel-button green">Add</button>
                        </form>
                        {addSubcategoryMsg && <p className="panel-msg">{addSubcategoryMsg}</p>}
                    </div>
                </div>

                {/* PRAWA KOLUMNA – usuwanie */}
                <div className="panel-column">
                    <div className="panel-card">
                        <h3 className="panel-title violet">➖ Delete Category</h3>
                        <form className="panel-form" onSubmit={handleDeleteCategory}>
                            <select
                                value={selectedCategoryId}
                                onChange={(e) => setSelectedCategoryId(e.target.value)}
                                className="panel-input"
                                required
                            >
                                <option value="" disabled>Select category</option>
                                {categories.map((cat) => (
                                    <option key={cat.id} value={cat.id}>{cat.name}</option>
                                ))}
                            </select>
                            <button type="submit" className="panel-button green">Delete</button>
                        </form>
                        {deleteCategoryMsg && <p className="panel-msg">{deleteCategoryMsg}</p>}
                    </div>

                    <div className="panel-card">
                        <h3 className="panel-title violet">➖ Delete Subcategory</h3>
                        <form className="panel-form" onSubmit={handleDeleteSubcategory}>
                            <select
                                value={selectedCategoryId}
                                onChange={(e) => {
                                    setSelectedCategoryId(e.target.value);
                                    setSelectedSubcategoryId("");
                                }}
                                className="panel-input"
                                required
                            >
                                <option value="" disabled>Select category</option>
                                {categories.map((cat) => (
                                    <option key={cat.id} value={cat.id}>{cat.name}</option>
                                ))}
                            </select>
                            <select
                                value={selectedSubcategoryId}
                                onChange={(e) => setSelectedSubcategoryId(e.target.value)}
                                className="panel-input"
                                required
                                disabled={!selectedCategoryId}
                            >
                                <option value="" disabled>Select subcategory</option>
                                {subcategories
                                    .filter(sub => sub.categoryId === selectedCategoryId)
                                    .map(sub => (
                                        <option key={sub.id} value={sub.id}>{sub.name}</option>
                                    ))}
                            </select>
                            <button
                                type="submit"
                                className="panel-button green"
                                disabled={!selectedCategoryId || !selectedSubcategoryId}
                            >
                                Delete
                            </button>
                        </form>
                        {deleteSubcategoryMsg && <p className="panel-msg">{deleteSubcategoryMsg}</p>}
                    </div>
                </div>
            </div>
        </div>
    );
}
