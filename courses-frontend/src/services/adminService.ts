import { config } from "../config";
import { fetchClient } from "./fetchClient.ts";

export const adminService = {
    async fetchDashboard(): Promise<void> {
        const response = await fetchClient.fetch(`${config.apiBaseUrl}/api/admin/dashboard`);
        if (!response.ok) {
            throw new Error("Error while fetching dashboard data.");
        }
    },

    async addCategory(name: string): Promise<void> {
        const response = await fetchClient.fetch(`${config.apiBaseUrl}/api/admin/categories`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ name }),
        });
        if (!response.ok) {
            throw new Error("Error while adding category.");
        }
    },

    async addSubcategory(name: string, categoryId: string): Promise<void> {
        const response = await fetchClient.fetch(`${config.apiBaseUrl}/api/admin/subcategories`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ name, categoryId }),
        });
        if (!response.ok) {
            throw new Error("Error while adding subcategory.");
        }
    },

    async deleteCourse(courseId: string): Promise<any> {
        const response = await fetchClient.fetch(`${config.apiBaseUrl}/api/admin/course/${courseId}`, {
            method: "DELETE",
            credentials: "include"
        });
        if (!response.ok) {
            throw new Error("Failed to delete course");
        }
        return await response.json();
    },

    async deleteCategory(categoryId: string): Promise<any> {
        const response = await fetchClient.fetch(`${config.apiBaseUrl}/api/admin/category/${categoryId}`, {
            method: "DELETE",
            credentials: "include"
        });
        if (!response.ok) {
            throw new Error("Failed to delete category");
        }
        return await response.json();
    },

    async deleteSubcategory(subcategoryId: string): Promise<any> {
        const response = await fetchClient.fetch(`${config.apiBaseUrl}/api/admin/subcategory/${subcategoryId}`, {
            method: "DELETE",
            credentials: "include"
        });
        if (!response.ok) {
            throw new Error("Failed to delete subcategory");
        }
        return await response.json();
    },

    async toggleCourseVisibility(courseId: string): Promise<void> {
        const response = await fetchClient.fetch(`${config.apiBaseUrl}/api/admin/courses/${courseId}/toggle-visibility`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            credentials: "include"
        });
        if (!response.ok) {
            throw new Error("Failed to toggle course visibility");
        }
    },
    async getAllCourses(): Promise<{ id: string; name: string; isHidden: boolean }[]> {
        const response = await fetchClient.fetch(`${config.apiBaseUrl}/api/courses/all`, {
            method: "GET",
            credentials: "include"
        });

        if (!response.ok) {
            throw new Error("Failed to fetch all courses");
        }

        return await response.json();
    }

};
