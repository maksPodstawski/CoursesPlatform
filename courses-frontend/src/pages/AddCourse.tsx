import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { config } from "../config";


export const AddCourse = () => {
    const [form, setForm] = useState({
        name: "",
        description: "",
        imageUrl: "",
        duration: 1,
        price: 1,
    });
    const [error, setError] = useState("");
    const navigate = useNavigate();

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        const { name, value } = e.target;
        setForm(prev => ({
            ...prev,
            [name]: name === "duration" || name === "price" ? Number(value) : value
        }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError("");
        try {
            const response = await fetch(`${config.apiBaseUrl}${config.apiEndpoints.addCourse}`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                credentials: "include",
                body: JSON.stringify(form),
            });
            if (!response.ok) throw new Error("Failed to add course");
            navigate("/courses");
        } catch (err: unknown) {
            setError(err instanceof Error ? err.message : "Unknown error");
        }
    };

    return (
        <div>
            <h2>Add Course</h2>
            <form onSubmit={handleSubmit}>
                {error && <div>{error}</div>}
                <div>
                    <label htmlFor="name">Name</label>
                    <input name="name" value={form.name} onChange={handleChange} required />
                </div>
                <div>
                    <label htmlFor="description">Description</label>
                    <textarea name="description" value={form.description} onChange={handleChange} />
                </div>
                <div>
                    <label htmlFor="imageUrl">Image URL</label>
                    <input name="imageUrl" value={form.imageUrl} onChange={handleChange} required />
                </div>
                <div>
                    <label htmlFor="duration">Duration</label>
                    <input name="duration" type="number" min="1" value={form.duration} onChange={handleChange} required />
                </div>
                <div>
                    <label htmlFor="price">Price</label>
                    <input name="price" type="number" min="1" value={form.price} onChange={handleChange} required />
                </div>
                <button type="submit" className="btn btn-primary">Add Course</button>
            </form>
        </div>
    );
};