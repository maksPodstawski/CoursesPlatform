import { useNavigate } from 'react-router-dom';
import { authService } from '../services/authService';
import { validateRegisterForm } from "../validation/registerValidation";
import { useForm } from '../hooks/useForm';
import { mapBackendErrors } from '../utils/errorHandling';
import type { RegisterRequest } from "../types/user";
import {registerFieldMapping, registerInitialValues} from "../constants/forms.ts";

export const Register = () => {
    const navigate = useNavigate();

    const {
        formData,
        fieldErrors,
        setFieldErrors,
        generalError,
        setGeneralError,
        handleChange,
        handleBlur,
        validateAll,
        isSubmitting,
        setIsSubmitting
    } = useForm<RegisterRequest>(registerInitialValues, validateRegisterForm);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setFieldErrors({});
        setGeneralError('');

        if (!validateAll()) return;

        setIsSubmitting(true);
        try {
            await authService.register(formData);
            navigate('/login');
        } catch (err: unknown) {
            const error = err as Error & {
                response?: {
                    status: number,
                    data: { errors?: Record<string, string[]> }
                }
            };

            const response = error.response;

            if (response?.status === 400 && response.data?.errors) {
                const mappedErrors = mapBackendErrors<RegisterRequest>(
                    response.data.errors,
                    registerFieldMapping
                );
                setFieldErrors(mappedErrors);
            } else {
                setGeneralError(error.message || 'Registration failed. Please try again.');
            }
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <div className="form">
            <h2>Create your account</h2>
            <form onSubmit={handleSubmit}>
                {generalError && <div className="error">{generalError}</div>}

                <div className="form-group">
                    <label htmlFor="firstName" className="form-label">First Name</label>
                    <input
                        id="firstName"
                        name="firstName"
                        type="text"
                        className="form-input"
                        value={formData.firstName}
                        onChange={handleChange}
                        onBlur={handleBlur}
                    />
                    {fieldErrors.firstName && <div className="field-error">{fieldErrors.firstName}</div>}
                </div>

                <div className="form-group">
                    <label htmlFor="lastName" className="form-label">Last Name</label>
                    <input
                        id="lastName"
                        name="lastName"
                        type="text"
                        className="form-input"
                        value={formData.lastName}
                        onChange={handleChange}
                        onBlur={handleBlur}
                    />
                    {fieldErrors.lastName && <div className="field-error">{fieldErrors.lastName}</div>}
                </div>

                <div className="form-group">
                    <label htmlFor="email" className="form-label">Email address</label>
                    <input
                        id="email"
                        name="email"
                        type="email"
                        className="form-input"
                        value={formData.email}
                        onChange={handleChange}
                        onBlur={handleBlur}
                    />
                    {fieldErrors.email && <div className="field-error">{fieldErrors.email}</div>}
                </div>

                <div className="form-group">
                    <label htmlFor="password" className="form-label">Password</label>
                    <input
                        id="password"
                        name="password"
                        type="password"
                        className="form-input"
                        value={formData.password}
                        onChange={handleChange}
                        onBlur={handleBlur}
                    />
                    {fieldErrors.password && <div className="field-error">{fieldErrors.password}</div>}
                </div>

                <div className="form-group">
                    <label htmlFor="confirmPassword" className="form-label">Confirm Password</label>
                    <input
                        id="confirmPassword"
                        name="confirmPassword"
                        type="password"
                        className="form-input"
                        value={formData.confirmPassword}
                        onChange={handleChange}
                        onBlur={handleBlur}
                    />
                    {fieldErrors.confirmPassword && <div className="field-error">{fieldErrors.confirmPassword}</div>}
                </div>

                <button
                    type="submit"
                    className="btn btn-primary"
                    disabled={isSubmitting}
                >
                    {isSubmitting ? 'Creating account...' : 'Create Account'}
                </button>
            </form>
        </div>
    );
};