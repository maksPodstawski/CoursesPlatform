import { config } from "../config";

export const adminService = {
    async fetchDashboard(): Promise<void> {
        const response = await fetch(`${config.apiBaseUrl}/api/admin/dashboard`, {
            method: "GET",
            credentials: "include"
        });
        if (!response.ok) {
            throw new Error("Brak dostępu");
        }
    },

    async addCategory(name: string): Promise<void> {
        const response = await fetch(`${config.apiBaseUrl}/api/admin/categories`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify({ name }),
        });
        if (!response.ok) {
            throw new Error("Error while adding category.");
        }
    },

    async addSubcategory(name: string, categoryId: string): Promise<void> {
        const response = await fetch(`${config.apiBaseUrl}/api/admin/subcategories`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify({ name, categoryId }),
        });
        if (!response.ok) {
            throw new Error("Error while adding subcategory.");
        }
    },

    async deleteCourse(courseId: string): Promise<any> {
        const response = await fetch(`${config.apiBaseUrl}/api/admin/course/${courseId}`, {
            method: "DELETE",
            credentials: "include"
        });
        if (!response.ok) {
            throw new Error("Failed to delete course");
        }
        return await response.json();
    },

    async deleteComments(commentIds: string[]): Promise<void> {
        const response = await fetch(`${config.apiBaseUrl}/api/admin/comments/delete-many`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify({ commentIds }),
        });
        if (!response.ok) {
            throw new Error("Failed to delete comments");
        }
    },

    async deleteCategory(categoryId: string): Promise<any> {
        const response = await fetch(`${config.apiBaseUrl}/api/admin/category/${categoryId}`, {
            method: "DELETE",
            credentials: "include"
        });
        if (!response.ok) {
            throw new Error("Failed to delete category");
        }
        return await response.json();
    },

    async deleteSubcategory(subcategoryId: string): Promise<any> {
        const response = await fetch(`${config.apiBaseUrl}/api/admin/subcategory/${subcategoryId}`, {
            method: "DELETE",
            credentials: "include"
        });
        if (!response.ok) {
            throw new Error("Failed to delete subcategory");
        }
        return await response.json();
    }
};