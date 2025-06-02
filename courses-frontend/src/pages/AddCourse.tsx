import '../styles/AddCourse.css';
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { config } from "../config";
import Sidebar from "../components/Sidebar";

type CourseForm = {
  name: string;
  description: string;
  imageUrl: string;
  duration: number;
  price: number;
};

export const AddCourse = () => {
  const [form, setForm] = useState<CourseForm>({
    name: "",
    description: "",
    imageUrl: "",
    duration: 1,
    price: 1,
  });

  const navigate = useNavigate();

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    const { name, value } = e.target;
    setForm((prev) => ({
      ...prev,
      [name]: name === "duration" || name === "price" ? parseFloat(value.replace(',', '.')) : value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      const res = await fetch(`${config.apiBaseUrl}${config.apiEndpoints.addCourse}`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(form),
      });

      if (res.ok) {
        navigate("/courses");
      }
    } catch (err) {
      console.error("Error submitting course:", err);
    }
  };

  const renderField = (
    label: string,
    name: keyof CourseForm,
    type: "text" | "number" | "textarea" = "text",
    placeholder?: string
  ) => (
    <div className="form-group">
      <label htmlFor={name}>{label}</label>
      {type === "textarea" ? (
        <textarea
          id={name}
          name={name}
          value={form[name]}
          onChange={handleChange}
          className="form-input"
          placeholder={placeholder}
        />
      ) : (
        <input
          id={name}
          name={name}
          type={type}
          value={form[name]}
          onChange={handleChange}
          className="form-input"
          placeholder={placeholder}
          min={type === "number" ? 1 : undefined}
          step={type === "number" ? "0.01" : undefined}
        />
      )}
    </div>
  );

  return (
    <div className="page-layout">
      <Sidebar />
      <main className="main-content">
        <form className="form" onSubmit={handleSubmit}>
          <div className="form-title-container">
            <h2 className="form-title">Add New Course</h2>
          </div>

          {renderField("Course Name", "name", "text", "Enter course name")}
          {renderField("Description", "description", "textarea", "Enter course description")}
          {renderField("Image URL", "imageUrl", "text", "https://example.com/image.jpg")}
          {renderField("Duration", "duration", "number", "Duration in weeks")}
          {renderField("Price", "price", "number", "Course price")}

          <button type="submit" className="btn">
            Submit Course
          </button>
        </form>
      </main>
    </div>
  );
};
