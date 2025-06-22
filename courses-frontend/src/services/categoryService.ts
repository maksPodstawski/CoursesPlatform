import { config } from '../config';
import {fetchClient} from "./fetchClient.ts";

export async function getCategories() {
    const response = await fetchClient.fetch(`${config.apiBaseUrl}/api/categories`, {
        credentials: 'include',
    });
    if (!response.ok) {
        throw new Error('Error fetching categories');
    }
    return await response.json();
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
