//import '../styles/AddStage.css';
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { config } from "../config";
//import Sidebar from "../components/Sidebar";

type StageForm = {
  name: string;
  description: string;
  duration: number;
  courseId: string;
};

export const AddStage = () => {
  const [form, setForm] = useState<StageForm>({
    name: "",
    description: "",
    duration: 1,
    courseId: "",
  });

  const [videoFile, setVideoFile] = useState<File | null>(null);
  const navigate = useNavigate();

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    const { name, value } = e.target;
    setForm((prev) => ({
      ...prev,
      [name]: name === "duration" ? parseFloat(value.replace(',', '.')) : value,
    }));
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      setVideoFile(e.target.files[0]);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      const res = await fetch(`${config.apiBaseUrl}${config.apiEndpoints.addStage}`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(form),
      });

      if (!res.ok) throw new Error("Stage creation failed.");

      const data = await res.json();
      const stageId = data.id;

      if (videoFile) {
        const formData = new FormData();
        formData.append("file", videoFile);

        const videoRes = await fetch(`${config.apiBaseUrl}/api/stages/${stageId}/video`, {
          method: "POST",
          credentials: "include",
          body: formData,
        });

        if (!videoRes.ok) throw new Error("Video upload failed.");
      }

      navigate("/stages");
    } catch (err) {
      console.error("Error submitting stage:", err);
    }
  };

  const renderField = (
    label: string,
    name: keyof StageForm,
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
      <main className="main-content">
        <form className="form" onSubmit={handleSubmit}>
          <h2>Add New Stage</h2>

          {renderField("Stage Name", "name", "text", "Enter stage name")}
          {renderField("Description", "description", "textarea", "Enter stage description")}
          {renderField("Duration", "duration", "number", "Duration in minutes")}
          {renderField("Course ID", "courseId", "text", "Paste course ID")}

          <div className="form-group">
            <label htmlFor="video">Upload Video (optional)</label>
            <input
              id="video"
              name="video"
              type="file"
              accept="video/*"
              onChange={handleFileChange}
              className="form-input"
            />
          </div>

          <button type="submit" className="btn">
            Submit Stage
          </button>
        </form>
      </main>
    </div>
  );
};
