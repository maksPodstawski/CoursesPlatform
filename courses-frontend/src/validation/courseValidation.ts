export interface CourseFormData {
    name: string;
    description: string;
    duration: number;
    price: number;
    difficulty: number;
    selectedCategory: string;
    selectedSubcategory: string;
    imageFile: File | null;
}

export type CourseFieldErrors = Partial<Record<
    keyof CourseFormData,
    string
>>;

export const validateCourseForm = (formData: CourseFormData): CourseFieldErrors => {
    const errors: CourseFieldErrors = {};

    if (!formData.name?.trim()) {
        errors.name = "The course name field is required!";
    } else if (formData.name.length > 100) {
        errors.name = "Course name must not exceed 100 characters!";
    }

    const isDescriptionEmpty = !formData.description || !formData.description.replace(/<(.|\n)*?>/g, '').trim();
    if (isDescriptionEmpty) {
        errors.description = "The description field is required!";
    }

    if (
        !formData.duration ||
        isNaN(formData.duration) ||
        formData.duration < 1 ||
        !/^[0-9]+$/.test(String(formData.duration))
    ) {
        errors.duration = "Duration must be a positive integer!";
    }

    if (
        !formData.price ||
        isNaN(formData.price) ||
        formData.price < 1 ||
        !/^[0-9]+$/.test(String(formData.price))
    ) {
        errors.price = "Price must be a positive integer!";
    }

    if (!formData.selectedCategory) {
        errors.selectedCategory = "Category is required!";
    }

    if (!formData.selectedSubcategory) {
        errors.selectedSubcategory = "Subcategory is required!";
    }

    if (!formData.difficulty || ![1,2,3].includes(formData.difficulty)) {
        errors.difficulty = "Difficulty is required!";
    }

    if (!formData.imageFile) {
        errors.imageFile = "Course image is required!";
    }

    return errors;
};
