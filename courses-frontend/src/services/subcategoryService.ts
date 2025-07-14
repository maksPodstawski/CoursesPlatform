import { fetchClient } from './fetchClient';

export interface Subcategory {
	id: string;
	name: string;
	categoryId: string;
}

export async function getSubcategoriesByCategoryId(categoryId: string): Promise<Subcategory[]> {
	const response = await fetchClient.fetch(`/api/categories/${categoryId}/subcategories`);
	return await response.json();
} 