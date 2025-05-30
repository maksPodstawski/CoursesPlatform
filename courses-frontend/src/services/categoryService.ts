import { config } from '../config';

export async function getCategories() {
    const response = await fetch(`${config.apiBaseUrl}/api/categories`, {
        credentials: 'include',
    });
    if (!response.ok) {
        throw new Error('Error fetching categories');
    }
    return await response.json();
}