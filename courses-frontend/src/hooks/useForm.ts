import { useState, useEffect } from 'react';

type ValidationFunction<T> = (data: T) => Partial<Record<keyof T, string>>;

export function useForm<T extends Record<string, unknown>>(
    initialValues: T,
    validate: ValidationFunction<T>
) {
    const [formData, setFormData] = useState<T>(initialValues);
    const [fieldErrors, setFieldErrors] = useState<Partial<Record<keyof T, string>>>({});
    const [touched, setTouched] = useState<Record<keyof T, boolean>>(
        Object.keys(initialValues).reduce((acc, key) => {
            acc[key as keyof T] = false;
            return acc;
        }, {} as Record<keyof T, boolean>)
    );
    const [generalError, setGeneralError] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);

    useEffect(() => {
        const errors = validate(formData);
        const touchedErrors: Partial<Record<keyof T, string>> = {};

        Object.keys(errors).forEach(key => {
            const field = key as keyof T;
            if (touched[field]) {
                touchedErrors[field] = errors[field];
            }
        });

        setFieldErrors(touchedErrors);
    }, [formData, touched, validate]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
    };

    const handleBlur = (e: React.FocusEvent<HTMLInputElement>) => {
        const { name } = e.target;
        setTouched(prev => ({
            ...prev,
            [name]: true
        }));
    };

    const validateAll = (): boolean => {
        const errors = validate(formData);

        setTouched(
            Object.keys(formData).reduce((acc, key) => {
                acc[key as keyof T] = true;
                return acc;
            }, {} as Record<keyof T, boolean>)
        );
        setFieldErrors(errors);

        return Object.keys(errors).length === 0;
    };

    const resetForm = () => {
        setFormData(initialValues);
        setFieldErrors({});
        setGeneralError('');
        setTouched(
            Object.keys(initialValues).reduce((acc, key) => {
                acc[key as keyof T] = false;
                return acc;
            }, {} as Record<keyof T, boolean>)
        );
    };

    return {
        formData,
        setFormData,
        fieldErrors,
        setFieldErrors,
        touched,
        generalError,
        setGeneralError,
        isSubmitting,
        setIsSubmitting,
        handleChange,
        handleBlur,
        validateAll,
        resetForm
    };
}