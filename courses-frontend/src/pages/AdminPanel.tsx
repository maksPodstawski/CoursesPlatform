import { useAuth } from "../context/AuthContext.tsx";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { adminService } from "../services/adminService";
import { getCategories } from "../services/categoryService.ts";
import { config } from "../config.ts";
import "../styles/Admin.css";

type Category = {
    id: string;
    name: string;
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
    const [selectedCategoryId, setSelectedCategoryId] = useState<string>("");

    useEffect(() => {
        const fetchDashboard = async () => {
            try {
                const response = await fetch(`${config.apiBaseUrl}/api/admin/dashboard`, {
                    method: "GET",
                    credentials: "include"
                });
                if (!response.ok) {
                    throw new Error("Brak dostępu");
                }
            } catch (err) {
                setError("Brak dostępu. Przekierowanie na stronę główną.");
                navigate("/");
            }
        };

        if (localStorage.getItem("isLoggedIn")) {
            fetchDashboard();
        } else {
            navigate("/login");
        }
    }, [isLoggedIn, navigate]);

    useEffect(() => {
        const fetchCategories = async () => {
            try {
                const data = await getCategories();
                setCategories(data);
                if (data.length > 0) setSelectedCategoryId(data[0].id);
            } catch (err) {
                setCategories([]);
            }
        };
        fetchCategories();
    }, []);

    const handleAddCategory = async (e: React.FormEvent) => {
        e.preventDefault();
        setAddCategoryMsg(null);
        try {
            await adminService.addCategory(categoryName);
            setAddCategoryMsg("Category added successfully!");
            setCategoryName("");
            // odśwież listę kategorii po dodaniu
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
        } catch (err) {
            setAddSubcategoryMsg("Error adding subcategory. Please try again.");
        }
    };

    if (error) {
        return <p>{error}</p>;
    }

    return (
        <div className="admin-panel-row">
            <div className="admin-panel-section">
                <h2>Add New Category</h2>
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
                <h2>Add New SubCategory</h2>
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
                        {categories.map(cat => (
                            <option key={cat.id} value={cat.id}>{cat.name}</option>
                        ))}
                    </select>
                    <button type="submit" className="admin-btn">Add</button>
                </form>
                {addSubcategoryMsg && <p className="admin-msg">{addSubcategoryMsg}</p>}
            </div>
        </div>
    );
};

export default AdminPanel;