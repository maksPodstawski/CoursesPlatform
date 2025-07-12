"use client"

import type React from "react"
import { useState, useEffect } from "react"
import { useNavigate } from "react-router-dom"
import { adminService } from "../../services/adminService"
import { getCategories, getSubcategoriesByCategoryId } from "../../services/categoryService"
import AdminSidebar from "../../components/AdminSidebar"
import {
    Plus,
    Trash2,
    Edit2,
    Save,
    X,
    ChevronDown,
    ChevronRight,
    Folder,
    FolderOpen,
    AlertTriangle,
} from "lucide-react"
import "../../styles/Admin.css"

type Category = { id: string; name: string }
type Subcategory = { id: string; name: string; categoryId: string }

export default function AdminCategoryManagement() {
    const [categories, setCategories] = useState<Category[]>([])
    const [subcategories, setSubcategories] = useState<Subcategory[]>([])
    const [expandedCategories, setExpandedCategories] = useState<Set<string>>(new Set())

    const [selectedCategoryId, setSelectedCategoryId] = useState<string>("")
    const [selectedSubcategoryId, setSelectedSubcategoryId] = useState<string>("")

    const [categoryName, setCategoryName] = useState("")
    const [subcategoryName, setSubcategoryName] = useState("")

    const [message, setMessage] = useState<{ type: string; text: string } | null>(null) // Unified message state

    // Edit states
    const [editingCategoryId, setEditingCategoryId] = useState<string | null>(null)
    const [editingCategoryName, setEditingCategoryName] = useState<string>("")
    const [editingSubcategoryId, setEditingSubcategoryId] = useState<string | null>(null)
    const [editingSubcategoryName, setEditingSubcategoryName] = useState<string>("")

    // Modals
    const [showDeleteCategoryConfirm, setShowDeleteCategoryConfirm] = useState(false)
    const [showDeleteSubcategoryConfirm, setShowDeleteSubcategoryConfirm] = useState(false)

    const navigate = useNavigate()

    useEffect(() => {
        const initialize = async () => {
            try {
                await adminService.fetchDashboard()
                await fetchAll()
            } catch (err) {
                console.error("Unauthorized or error:", err)
                navigate("/")
            }
        }
        initialize()
    }, [navigate])

    useEffect(() => {
        if (!selectedCategoryId) return

        getSubcategoriesByCategoryId(selectedCategoryId)
            .then(setSubcategories)
            .catch(() => setSubcategories([]))
    }, [selectedCategoryId])

    const fetchAll = async () => {
        try {
            const cats = await getCategories()
            setCategories(cats)

            const allSub = await Promise.all(cats.map((cat: { id: string }) => getSubcategoriesByCategoryId(cat.id)))
            const mergedSub = allSub.flat()
            setSubcategories(mergedSub)
        } catch (error) {
            console.error("Error fetching categories/subcategories:", error)
            showMessage("danger", "Failed to load categories and subcategories.")
        }
    }

    const showMessage = (type: string, text: string) => {
        setMessage({ type, text })
        setTimeout(() => {
            setMessage(null)
        }, 3000)
    }

    const toggleCategory = (categoryId: string) => {
        setExpandedCategories((prev) => {
            const newSet = new Set(prev)
            newSet.has(categoryId) ? newSet.delete(categoryId) : newSet.add(categoryId)
            return newSet
        })
    }

    const handleAddCategory = async (e: React.FormEvent) => {
        e.preventDefault()
        try {
            await adminService.addCategory(categoryName)
            showMessage("success", "✅ Category added successfully!")
            setCategoryName("")
            fetchAll()
        } catch (error: any) {
            showMessage("danger", "❌ " + (error.message || "Error adding category."))
        }
    }

    const handleAddSubcategory = async (e: React.FormEvent) => {
        e.preventDefault()
        try {
            await adminService.addSubcategory(subcategoryName, selectedCategoryId)
            setSubcategoryName("")
            showMessage("success", "✅ Subcategory added successfully!")
            fetchAll()
        } catch (error: any) {
            showMessage("danger", "❌ " + (error.message || "Error adding subcategory."))
        }
    }

    const confirmDeleteCategory = () => {
        setShowDeleteCategoryConfirm(true)
    }

    const confirmDeleteSubcategory = () => {
        setShowDeleteSubcategoryConfirm(true)
    }

    const handleDeleteCategory = async () => {
        if (!selectedCategoryId) return

        try {
            await adminService.deleteCategory(selectedCategoryId)
            showMessage("success", "✅ Category deleted successfully!")
            setSelectedCategoryId("")
            setShowDeleteCategoryConfirm(false)
            fetchAll()
        } catch (error: any) {
            showMessage("danger", "❌ " + (error.message || "Error deleting category."))
        }
    }

    const handleDeleteSubcategory = async () => {
        if (!selectedSubcategoryId) return

        try {
            await adminService.deleteSubcategory(selectedSubcategoryId)
            showMessage("success", "✅ Subcategory deleted successfully!")
            setSelectedSubcategoryId("")
            setShowDeleteSubcategoryConfirm(false)
            fetchAll()
        } catch (error: any) {
            showMessage("danger", "❌ " + (error.message || "Error deleting subcategory."))
        }
    }

    const handleStartEditCategory = (cat: Category) => {
        setEditingCategoryId(cat.id)
        setEditingCategoryName(cat.name)
    }

    const handleUpdateCategory = async (e: React.FormEvent) => {
        e.preventDefault()
        try {
            await adminService.updateCategory(editingCategoryId!, editingCategoryName)
            showMessage("success", "✅ Category updated successfully!")
            setEditingCategoryId(null)
            setEditingCategoryName("")
            fetchAll()
        } catch (error: any) {
            showMessage("danger", "❌ " + (error.message || "Error updating category."))
        }
    }

    const handleStartEditSubcategory = (sub: Subcategory) => {
        setEditingSubcategoryId(sub.id)
        setEditingSubcategoryName(sub.name)
    }

    const handleUpdateSubcategory = async (e: React.FormEvent) => {
        e.preventDefault()
        try {
            await adminService.updateSubcategory(editingSubcategoryId!, editingSubcategoryName)
            showMessage("success", "✅ Subcategory updated successfully!")
            setEditingSubcategoryId(null)
            setEditingSubcategoryName("")
            fetchAll()
        } catch (error: any) {
            showMessage("danger", "❌ " + (error.message || "Error updating subcategory."))
        }
    }

    return (
        <div className="admin-layout">
            <AdminSidebar />
            <div className="admin-content">
                <div className="admin-header">
                    <h1>Category Management</h1>
                    <p>Manage categories and subcategories for your courses</p>
                </div>

                <div className="management-grid">
                    {/* Categories Overview */}
                    <div className="content-card large">
                        <div className="card-header">
                            <div className="card-title">
                                <Folder size={20} />
                                Categories & Subcategories
                            </div>
                            <div className="card-subtitle">Click on categories to expand and see subcategories</div>
                        </div>
                        <div className="card-content">
                            <div className="scrollable-content tall">
                                {categories.map((cat) => {
                                    const subs = subcategories.filter((sub) => sub.categoryId === cat.id)
                                    const isExpanded = expandedCategories.has(cat.id)

                                    return (
                                        <div key={cat.id} className="collapsible-item">
                                            <div className="collapsible-header" onClick={() => toggleCategory(cat.id)}>
                                                <div className="collapsible-content">
                                                    {isExpanded ? (
                                                        <FolderOpen size={16} className="folder-icon open" />
                                                    ) : (
                                                        <Folder size={16} className="folder-icon" />
                                                    )}
                                                    {editingCategoryId === cat.id ? (
                                                        <form
                                                            onSubmit={handleUpdateCategory}
                                                            className="inline-edit-form"
                                                            onClick={(e) => e.stopPropagation()}
                                                        >
                                                            <input
                                                                type="text"
                                                                value={editingCategoryName}
                                                                onChange={(e) => setEditingCategoryName(e.target.value)}
                                                                className="inline-input"
                                                                required
                                                                autoFocus
                                                            />
                                                            <button type="submit" className="icon-button">
                                                                <Save size={14} />
                                                            </button>
                                                            <button
                                                                type="button"
                                                                className="icon-button"
                                                                onClick={(e) => {
                                                                    e.stopPropagation()
                                                                    setEditingCategoryId(null)
                                                                }}
                                                            >
                                                                <X size={14} />
                                                            </button>
                                                        </form>
                                                    ) : (
                                                        <>
                                                            <span className="category-name">{cat.name}</span>
                                                            <button
                                                                className="icon-button"
                                                                onClick={(e) => {
                                                                    e.stopPropagation()
                                                                    handleStartEditCategory(cat)
                                                                }}
                                                            >
                                                                <Edit2 size={14} />
                                                            </button>
                                                        </>
                                                    )}
                                                </div>
                                                <div className="collapsible-info">
                                                    <span className="badge">{subs.length} subcategories</span>
                                                    {isExpanded ? <ChevronDown size={16} /> : <ChevronRight size={16} />}
                                                </div>
                                            </div>

                                            {isExpanded && (
                                                <div className="collapsible-body">
                                                    {subs.length > 0 ? (
                                                        <div className="subcategory-grid">
                                                            {subs.map((sub) => (
                                                                <div key={sub.id} className="subcategory-item">
                                                                    {editingSubcategoryId === sub.id ? (
                                                                        <form onSubmit={handleUpdateSubcategory} className="inline-edit-form">
                                                                            <input
                                                                                type="text"
                                                                                value={editingSubcategoryName}
                                                                                onChange={(e) => setEditingSubcategoryName(e.target.value)}
                                                                                className="inline-input"
                                                                                required
                                                                                autoFocus
                                                                            />
                                                                            <button
                                                                                type="button"
                                                                                className="icon-button"
                                                                                onClick={() => setEditingSubcategoryId(null)}
                                                                            >
                                                                                <X size={12} />
                                                                            </button>
                                                                        </form>
                                                                    ) : (
                                                                        <>
                                                                            <span>{sub.name}</span>
                                                                            <button className="icon-button" onClick={() => handleStartEditSubcategory(sub)}>
                                                                                <Edit2 size={12} />
                                                                            </button>
                                                                        </>
                                                                    )}
                                                                </div>
                                                            ))}
                                                        </div>
                                                    ) : (
                                                        <p className="empty-state">No subcategories</p>
                                                    )}
                                                </div>
                                            )}
                                        </div>
                                    )
                                })}
                            </div>
                            {message && (message.type === "success" || message.type === "danger") && (
                                <div className={`message ${message.type}`}>{message.text}</div>
                            )}
                        </div>
                    </div>

                    {/* Management Actions */}
                    <div className="actions-column">
                        {/* Add Category */}
                        <div className="content-card">
                            <div className="card-header">
                                <div className="card-title">
                                    <Plus size={16} />
                                    Add Category
                                </div>
                            </div>
                            <div className="card-content">
                                <form onSubmit={handleAddCategory} className="form">
                                    <div className="form-group">
                                        <label htmlFor="categoryName">Category Name</label>
                                        <input
                                            id="categoryName"
                                            type="text"
                                            value={categoryName}
                                            onChange={(e) => setCategoryName(e.target.value)}
                                            placeholder="Enter category name"
                                            className="form-input"
                                            required
                                        />
                                    </div>
                                    <button type="submit" className="btn btn-primary">
                                        <Plus size={16} />
                                        Add Category
                                    </button>
                                </form>
                                {message && message.type === "addCategory" && (
                                    <div className={`message ${message.type}`}>{message.text}</div>
                                )}
                            </div>
                        </div>

                        {/* Add Subcategory */}
                        <div className="content-card">
                            <div className="card-header">
                                <div className="card-title">
                                    <Plus size={16} />
                                    Add Subcategory
                                </div>
                            </div>
                            <div className="card-content">
                                <form onSubmit={handleAddSubcategory} className="form">
                                    <div className="form-group">
                                        <label htmlFor="subcategoryName">Subcategory Name</label>
                                        <input
                                            id="subcategoryName"
                                            type="text"
                                            value={subcategoryName}
                                            onChange={(e) => setSubcategoryName(e.target.value)}
                                            placeholder="Enter subcategory name"
                                            className="form-input"
                                            required
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label htmlFor="categorySelect">Parent Category</label>
                                        <select
                                            id="categorySelect"
                                            value={selectedCategoryId}
                                            onChange={(e) => setSelectedCategoryId(e.target.value)}
                                            className="form-select"
                                            required
                                        >
                                            <option value="">Select category</option>
                                            {categories.map((cat) => (
                                                <option key={cat.id} value={cat.id}>
                                                    {cat.name}
                                                </option>
                                            ))}
                                        </select>
                                    </div>
                                    <button type="submit" className="btn btn-primary" disabled={!selectedCategoryId}>
                                        <Plus size={16} />
                                        Add Subcategory
                                    </button>
                                </form>
                                {message && message.type === "addSubcategory" && (
                                    <div className={`message ${message.type}`}>{message.text}</div>
                                )}
                            </div>
                        </div>

                        {/* Delete Category */}
                        <div className="content-card danger-card">
                            <div className="card-header">
                                <div className="card-title danger">
                                    <Trash2 size={16} />
                                    Delete Category
                                </div>
                            </div>
                            <div className="card-content">
                                <div className="form">
                                    <div className="form-group">
                                        <label htmlFor="deleteCategorySelect">Select Category</label>
                                        <select
                                            id="deleteCategorySelect"
                                            value={selectedCategoryId}
                                            onChange={(e) => setSelectedCategoryId(e.target.value)}
                                            className="form-select"
                                            required
                                        >
                                            <option value="">Select category to delete</option>
                                            {categories.map((cat) => (
                                                <option key={cat.id} value={cat.id}>
                                                    {cat.name}
                                                </option>
                                            ))}
                                        </select>
                                    </div>
                                    <button
                                        type="button"
                                        className="btn btn-danger"
                                        disabled={!selectedCategoryId}
                                        onClick={confirmDeleteCategory}
                                    >
                                        <Trash2 size={16} />
                                        Delete Category
                                    </button>
                                </div>
                                {message && message.type === "deleteCategory" && (
                                    <div className={`message ${message.type}`}>{message.text}</div>
                                )}
                            </div>
                        </div>

                        {/* Delete Subcategory */}
                        <div className="content-card danger-card">
                            <div className="card-header">
                                <div className="card-title danger">
                                    <Trash2 size={16} />
                                    Delete Subcategory
                                </div>
                            </div>
                            <div className="card-content">
                                <div className="form">
                                    <div className="form-group">
                                        <label htmlFor="categorySelectForSub">Category</label>
                                        <select
                                            id="categorySelectForSub"
                                            value={selectedCategoryId}
                                            onChange={(e) => {
                                                setSelectedCategoryId(e.target.value)
                                                setSelectedSubcategoryId("")
                                            }}
                                            className="form-select"
                                            required
                                        >
                                            <option value="">Select category</option>
                                            {categories.map((cat) => (
                                                <option key={cat.id} value={cat.id}>
                                                    {cat.name}
                                                </option>
                                            ))}
                                        </select>
                                    </div>
                                    <div className="form-group">
                                        <label htmlFor="subcategorySelectDelete">Subcategory</label>
                                        <select
                                            id="subcategorySelectDelete"
                                            value={selectedSubcategoryId}
                                            onChange={(e) => setSelectedSubcategoryId(e.target.value)}
                                            className="form-select"
                                            required
                                            disabled={!selectedCategoryId}
                                        >
                                            <option value="">Select subcategory</option>
                                            {subcategories
                                                .filter((sub) => sub.categoryId === selectedCategoryId)
                                                .map((sub) => (
                                                    <option key={sub.id} value={sub.id}>
                                                        {sub.name}
                                                    </option>
                                                ))}
                                        </select>
                                    </div>
                                    <button
                                        type="button"
                                        className="btn btn-danger"
                                        disabled={!selectedCategoryId || !selectedSubcategoryId}
                                        onClick={confirmDeleteSubcategory}
                                    >
                                        <Trash2 size={16} />
                                        Delete Subcategory
                                    </button>
                                </div>
                                {message && message.type === "deleteSubcategory" && (
                                    <div className={`message ${message.type}`}>{message.text}</div>
                                )}
                            </div>
                        </div>
                    </div>
                </div>

                {/* Delete Category Confirmation Modal */}
                {showDeleteCategoryConfirm && (
                    <div className="modal-overlay" onClick={() => setShowDeleteCategoryConfirm(false)}>
                        <div className="modal" onClick={(e) => e.stopPropagation()}>
                            <div className="modal-header">
                                <div className="modal-title">
                                    <AlertTriangle size={20} className="danger" />
                                    Confirm Category Deletion
                                </div>
                            </div>
                            <div className="modal-content">
                                <p>
                                    Are you absolutely sure you want to delete the category "
                                    {categories.find((cat) => cat.id === selectedCategoryId)?.name}"? This action cannot be undone and
                                    will permanently remove all associated subcategories and courses.
                                </p>
                                <div className="warning-box">
                                    <AlertTriangle size={16} />
                                    <span>This will also remove all associated subcategories and courses.</span>
                                </div>
                            </div>
                            <div className="modal-actions">
                                <button className="btn btn-outline" onClick={() => setShowDeleteCategoryConfirm(false)}>
                                    Cancel
                                </button>
                                <button className="btn btn-danger" onClick={handleDeleteCategory}>
                                    <Trash2 size={16} />
                                    Delete Category
                                </button>
                            </div>
                        </div>
                    </div>
                )}

                {/* Delete Subcategory Confirmation Modal */}
                {showDeleteSubcategoryConfirm && (
                    <div className="modal-overlay" onClick={() => setShowDeleteSubcategoryConfirm(false)}>
                        <div className="modal" onClick={(e) => e.stopPropagation()}>
                            <div className="modal-header">
                                <div className="modal-title">
                                    <AlertTriangle size={20} className="danger" />
                                    Confirm Subcategory Deletion
                                </div>
                            </div>
                            <div className="modal-content">
                                <p>
                                    Are you absolutely sure you want to delete the subcategory "
                                    {subcategories.find((sub) => sub.id === selectedSubcategoryId)?.name}"? This action cannot be undone.
                                </p>
                                <div className="warning-box">
                                    <AlertTriangle size={16} />
                                    <span>This will permanently remove the subcategory.</span>
                                </div>
                            </div>
                            <div className="modal-actions">
                                <button className="btn btn-outline" onClick={() => setShowDeleteSubcategoryConfirm(false)}>
                                    Cancel
                                </button>
                                <button className="btn btn-danger" onClick={handleDeleteSubcategory}>
                                    <Trash2 size={16} />
                                    Delete Subcategory
                                </button>
                            </div>
                        </div>
                    </div>
                )}
            </div>
        </div>
    )
}
