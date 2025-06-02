import { config } from "../config";

export const adminService = {
    async getDashboard(): Promise<string> {
        const response = await fetch(`${config.apiBaseUrl}/api/admin/dashboard`, {
            credentials: "include",
        });
        if (!response.ok) {
            throw new Error("Error fetching admin dashboard.");
        }
        const data = await response.json();
        return data.message;
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

    addSubcategory: async (name: string, categoryId: string): Promise<void> => {
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

};