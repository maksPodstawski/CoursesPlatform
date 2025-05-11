import { useState } from 'react';
import { authService } from '../services/authService';
import { useNavigate } from 'react-router-dom';

export const Register = () => {
    const [formData, setFormData] = useState({
        email: '',
        password: '',
        firstName: '',
        lastName: ''
    });
    const [error, setError] = useState('');
    const navigate = useNavigate();

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');

        try {
            await authService.register(formData);
            navigate('/login');
        } catch (err) {
            setError('Registration failed. Please try again.');
        }
    };

    return (
        <div className="form">
            <h2>Create your account</h2>
            <form onSubmit={handleSubmit}>
                {error && <div className="error">{error}</div>}
                
                <div className="form-group">
                    <label htmlFor="firstName" className="form-label">
                        First Name
                    </label>
                    <input
                        id="firstName"
                        name="firstName"
                        type="text"
                        required
                        className="form-input"
                        value={formData.firstName}
                        onChange={handleChange}
                    />
                </div>

                <div className="form-group">
                    <label htmlFor="lastName" className="form-label">
                        Last Name
                    </label>
                    <input
                        id="lastName"
                        name="lastName"
                        type="text"
                        required
                        className="form-input"
                        value={formData.lastName}
                        onChange={handleChange}
                    />
                </div>

                <div className="form-group">
                    <label htmlFor="email" className="form-label">
                        Email address
                    </label>
                    <input
                        id="email"
                        name="email"
                        type="email"
                        required
                        className="form-input"
                        value={formData.email}
                        onChange={handleChange}
                    />
                </div>

                <div className="form-group">
                    <label htmlFor="password" className="form-label">
                        Password
                    </label>
                    <input
                        id="password"
                        name="password"
                        type="password"
                        required
                        className="form-input"
                        value={formData.password}
                        onChange={handleChange}
                    />
                </div>

                <button type="submit" className="btn btn-primary">
                    Create Account
                </button>
            </form>
        </div>
    );
}; 