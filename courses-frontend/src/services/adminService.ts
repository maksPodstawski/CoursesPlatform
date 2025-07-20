import { config } from "../config";
import { fetchClient } from "./fetchClient.ts";

async function handleResponse<T = any>(response: Response, defaultMsg: string): Promise<T> {
    if (response.ok) {
        try { return await response.json(); } catch { return undefined as any; }
    }
    let msg = defaultMsg;
    try {
        const data = await response.json();
        if (data.message) {
            msg = data.message;
        } else if (data.Name && Array.isArray(data.Name)) {
            msg = data.Name[0];
        } else {
            msg = JSON.stringify(data);
        }
    } catch {
        msg = await response.text() || msg;
    }
    throw new Error(msg);
}

export const adminService = {
    async fetchDashboard(): Promise<void> {
        const response = await fetchClient.fetch(`${config.apiBaseUrl}/api/admin/dashboard`);
        await handleResponse(response, "Error while fetching dashboard data.");
    },

    async addCategory(name: string): Promise<void> {
        const response = await fetchClient.fetch(`${config.apiBaseUrl}/api/admin/categories`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ name }),
        });
        await handleResponse(response, "Error while adding category.");
    },

    async addSubcategory(name: string, categoryId: string): Promise<void> {
        const response = await fetchClient.fetch(`${config.apiBaseUrl}/api/admin/subcategories`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ name, categoryId }),
        });
        await handleResponse(response, "Error while adding subcategory.");
    },

    async updateCategory(categoryId: string, name: string): Promise<void> {
        const response = await fetchClient.fetch(`${config.apiBaseUrl}/api/admin/category/${categoryId}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ name }),
        });
        await handleResponse(response, "Error while updating category.");
    },

    async updateSubcategory(subcategoryId: string, name: string): Promise<void> {
        const response = await fetchClient.fetch(`${config.apiBaseUrl}/api/admin/subcategory/${subcategoryId}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ name }),
        });
        await handleResponse(response, "Error while updating subcategory.");
    },

    async deleteCourse(courseId: string): Promise<any> {
        const response = await fetchClient.fetch(`${config.apiBaseUrl}/api/admin/course/${courseId}`, {
            method: "DELETE",
            credentials: "include"
        });
        return await handleResponse(response, "Failed to delete course");
    },

    async deleteCategory(categoryId: string): Promise<any> {
        const response = await fetchClient.fetch(`${config.apiBaseUrl}/api/admin/category/${categoryId}`, {
            method: "DELETE",
            credentials: "include"
        });
        return await handleResponse(response, "Failed to delete category");
    },

    async deleteSubcategory(subcategoryId: string): Promise<any> {
        const response = await fetchClient.fetch(`${config.apiBaseUrl}/api/admin/subcategory/${subcategoryId}`, {
            method: "DELETE",
            credentials: "include"
        });
        return await handleResponse(response, "Failed to delete subcategory");
    },

    async toggleCourseVisibility(courseId: string): Promise<void> {
        const response = await fetchClient.fetch(`${config.apiBaseUrl}/api/admin/courses/${courseId}/toggle-visibility`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            credentials: "include"
        });
        await handleResponse(response, "Failed to toggle course visibility");
    },

    async getAllCourses(): Promise<{ id: string; name: string; isHidden: boolean }[]> {
        const response = await fetchClient.fetch(`${config.apiBaseUrl}/api/courses/all`, {
            method: "GET",
            credentials: "include"
        });
        return await handleResponse(response, "Failed to fetch all courses");
    }
};
