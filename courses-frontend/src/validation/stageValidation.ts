export interface StageFormData {
    name: string;
    description: string;
    duration: number;
}

export type StageFieldErrors = Partial<Record<keyof StageFormData, string>>;

export const validateStageForm = (stage: StageFormData): StageFieldErrors => {
    const errors: StageFieldErrors = {};
    if (!stage.name?.trim()) {
        errors.name = "The stage name field is required!";
    } else if (stage.name.length > 100) {
        errors.name = "Stage name must not exceed 100 characters!";
    }
    const isDescriptionEmpty = !stage.description || !stage.description.replace(/<(.|\n)*?>/g, '').trim();
    if (isDescriptionEmpty) {
        errors.description = "The description field is required!";
    }
    if (!stage.duration || isNaN(stage.duration) || stage.duration < 1) {
        errors.duration = "Duration must be a positive integer!";
    }
    return errors;
};
