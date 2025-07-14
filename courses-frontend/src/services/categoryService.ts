import { config } from '../config';
import {fetchClient} from "./fetchClient.ts";
import { Category, Subcategory } from "../types/courses";

export async function getCategories(): Promise<Category[]> {
    const res = await fetch(`${config.apiBaseUrl}/api/categories`);
    if (!res.ok) throw new Error("Failed to fetch categories");
    return res.json();
}

export async function getSubcategories(categoryId: string): Promise<Subcategory[]> {
    const res = await fetch(`${config.apiBaseUrl}/api/categories/${categoryId}/subcategories`);
    if (!res.ok) throw new Error("Failed to fetch subcategories");
    return res.json();
}

export async function getSubcategoriesByCategoryId(categoryId: string) {
    const response = await fetchClient.fetch(`${config.apiBaseUrl}/api/categories/${categoryId}/subcategories`, {
        credentials: 'include',
    });

    if (!response.ok) {
        throw new Error('Error fetching subcategories');
    }

    return await response.json();
}
