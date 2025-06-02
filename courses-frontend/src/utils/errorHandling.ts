export type BackendValidationError = Record<string, string[]>;

export const mapBackendErrors = <T>(
    backendErrors: BackendValidationError,
    fieldMapping: Record<string, keyof T>
): Partial<Record<keyof T, string>> => {
    const frontendErrors: Partial<Record<keyof T, string>> = {};

    for (const key in backendErrors) {
        const mappedKey = fieldMapping[key];
        if (mappedKey) {
            frontendErrors[mappedKey] = backendErrors[key][0];
        }
    }

    return frontendErrors;
};