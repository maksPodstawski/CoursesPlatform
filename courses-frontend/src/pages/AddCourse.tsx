import '../styles/AddCourse.css';
import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { config } from "../config";
import Sidebar from "../components/Sidebar";
import { getCategories, getSubcategories } from "../services/categoryService";
import { Category, Subcategory } from "../types/courses";

type Stage = {
  name: string;
  description: string;
  duration: number;
  videoFile?: File | null;
};

type CourseForm = {
  name: string;
  description: string;
  imageUrl: string;
  duration: number;
  price: number;
  stages: Stage[];
};

type Tab = 'details' | 'stages' | 'summary';

type SubmissionStatus = {
  isSubmitting: boolean;
  currentStep: string;
  progress: number;
  error: string | null;
};

export const AddCourse = () => {
  const [form, setForm] = useState<CourseForm>({
    name: "",
    description: "",
    imageUrl: "",
    duration: 1,
    price: 1,
    stages: [],
  });

  const [currentStage, setCurrentStage] = useState<Stage>({
    name: "",
    description: "",
    duration: 1,
    videoFile: null,
  });

  const [activeTab, setActiveTab] = useState<Tab>('details');
  const [submissionStatus, setSubmissionStatus] = useState<SubmissionStatus>({
    isSubmitting: false,
    currentStep: '',
    progress: 0,
    error: null,
  });

  const [categories, setCategories] = useState<Category[]>([]);
  const [subcategories, setSubcategories] = useState<Subcategory[]>([]);
  const [selectedCategory, setSelectedCategory] = useState<string>("");
  const [selectedSubcategory, setSelectedSubcategory] = useState<string>("");

  const navigate = useNavigate();

  useEffect(() => {
    getCategories().then(setCategories).catch(() => setCategories([]));
  }, []);

  useEffect(() => {
    if (selectedCategory) {
      getSubcategories(selectedCategory).then(setSubcategories).catch(() => setSubcategories([]));
    } else {
      setSubcategories([]);
      setSelectedSubcategory("");
    }
  }, [selectedCategory]);

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    const { name, value } = e.target;
    setForm((prev) => ({
      ...prev,
      [name]: name === "duration" || name === "price" ? parseFloat(value.replace(',', '.')) : value,
    }));
  };

  const handleStageChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    const { name, value } = e.target;
    setCurrentStage((prev) => ({
      ...prev,
      [name]: name === "duration" ? parseFloat(value.replace(',', '.')) : value,
    }));
  };

  const handleStageFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      setCurrentStage((prev) => ({
        ...prev,
        videoFile: e.target.files![0],
      }));
    }
  };

  const addStage = () => {
    if (currentStage.name && currentStage.description) {
      setForm((prev) => ({
        ...prev,
        stages: [...prev.stages, { ...currentStage }],
      }));
      setCurrentStage({
        name: "",
        description: "",
        duration: 1,
        videoFile: null,
      });
    }
  };

  const removeStage = (index: number) => {
    setForm((prev) => ({
      ...prev,
      stages: prev.stages.filter((_, i) => i !== index),
    }));
  };

  const handleCategoryChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setSelectedCategory(e.target.value);
    setSelectedSubcategory("");
  };

  const handleSubcategoryChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setSelectedSubcategory(e.target.value);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmissionStatus({
      isSubmitting: true,
      currentStep: 'Creating course...',
      progress: 0,
      error: null,
    });

    try {
      // Przygotuj payload
      const payload: any = {
        name: form.name,
        description: form.description,
        imageUrl: form.imageUrl,
        duration: form.duration,
        price: form.price,
      };
      if (selectedSubcategory && selectedSubcategory.trim() !== "") {
        payload.subcategoryIds = [selectedSubcategory];
      }

      // First create the course
      const courseRes = await fetch(`${config.apiBaseUrl}${config.apiEndpoints.addCourse}`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(payload),
      });

      if (!courseRes.ok) throw new Error("Course creation failed");
      
      const courseData = await courseRes.json();
      const courseId = courseData.id;

      setSubmissionStatus(prev => ({
        ...prev,
        currentStep: 'Course created, adding stages...',
        progress: 20,
      }));

      // Then create all stages
      const totalStages = form.stages.length;
      for (let i = 0; i < form.stages.length; i++) {
        const stage = form.stages[i];
        setSubmissionStatus(prev => ({
          ...prev,
          currentStep: `Adding stage ${i + 1} of ${totalStages}...`,
          progress: 20 + (i / totalStages) * 40,
        }));

        const stageRes = await fetch(`${config.apiBaseUrl}${config.apiEndpoints.addStage}`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          credentials: "include",
          body: JSON.stringify({
            name: stage.name,
            description: stage.description,
            duration: stage.duration,
            courseId: courseId,
          }),
        });

        if (!stageRes.ok) throw new Error("Stage creation failed");

        const stageData = await stageRes.json();
        const stageId = stageData.id;

        // Upload video if exists
        if (stage.videoFile) {
          setSubmissionStatus(prev => ({
            ...prev,
            currentStep: `Uploading video for stage ${i + 1}...`,
            progress: 60 + (i / totalStages) * 30,
          }));

          const formData = new FormData();
          formData.append("file", stage.videoFile);

          const videoRes = await fetch(`${config.apiBaseUrl}/api/stages/${stageId}/video`, {
            method: "POST",
            credentials: "include",
            body: formData,
          });

          if (!videoRes.ok) throw new Error("Video upload failed");
        }
      }

      setSubmissionStatus(prev => ({
        ...prev,
        currentStep: 'Course created successfully!',
        progress: 100,
      }));

      // Wait a moment to show the success message
      setTimeout(() => {
        navigate("/courses");
      }, 1500);

    } catch (err) {
      console.error("Error submitting course:", err);
      setSubmissionStatus(prev => ({
        ...prev,
        isSubmitting: false,
        error: err instanceof Error ? err.message : "An error occurred",
      }));
    }
  };

  const renderField = (
    label: string,
    name: keyof CourseForm | keyof Stage,
    type: "text" | "number" | "textarea" = "text",
    placeholder?: string,
    value?: string | number,
    onChange?: (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => void
  ) => (
    <div className="form-group">
      <label htmlFor={name}>{label}</label>
      {type === "textarea" ? (
        <textarea
          id={name}
          name={name}
          value={value}
          onChange={onChange}
          className="form-input"
          placeholder={placeholder}
        />
      ) : (
        <input
          id={name}
          name={name}
          type={type}
          value={value}
          onChange={onChange}
          className="form-input"
          placeholder={placeholder}
          min={type === "number" ? 1 : undefined}
          step={type === "number" ? "0.01" : undefined}
        />
      )}
    </div>
  );

  const calculateTotalDuration = () => {
    return form.stages.reduce((total, stage) => total + stage.duration, 0).toFixed(2);
  };

  const truncateText = (text: string, maxLength: number) => {
    if (text.length > maxLength) {
      return text.substring(0, maxLength) + '...';
    }
    return text;
  };

  const renderTabContent = () => {
    switch (activeTab) {
      case 'details':
        return (
          <div className="form-content">
            <div className="course-section">
              <h3>Course Details</h3>
              {renderField("Course Name", "name", "text", "Enter course name", form.name, handleChange)}
              {renderField("Description", "description", "textarea", "Enter course description", form.description, handleChange)}
              {renderField("Image URL", "imageUrl", "text", "https://example.com/image.jpg", form.imageUrl, handleChange)}
              <div className="duration-price-group">
                {renderField("Duration", "duration", "number", "Duration in minutes", form.duration, handleChange)}
                {renderField("Price", "price", "number", "Course price", form.price, handleChange)}
              </div>
              <div className="category-row">
                <div className="form-group">
                  <label className="category-label">Category</label>
                  <select className="category-select" value={selectedCategory} onChange={handleCategoryChange} required>
                    <option value="">Select a category</option>
                    {categories.map((cat) => (
                      <option key={cat.id} value={cat.id}>{cat.name}</option>
                    ))}
                  </select>
                </div>
                <div className="form-group">
                  <label className="subcategory-label">Subcategory</label>
                  <select className="subcategory-select" value={selectedSubcategory} onChange={handleSubcategoryChange} required disabled={!selectedCategory}>
                    <option value="">Select a subcategory</option>
                    {subcategories.map((sub) => (
                      <option key={sub.id} value={sub.id}>{sub.name}</option>
                    ))}
                  </select>
                </div>
              </div>
            </div>
            <div className="course-preview">
              <h3>Course Preview</h3>
              <div className="preview-content">
                <div className="preview-image">
                  {form.imageUrl ? (
                    <img src={form.imageUrl} alt="Course preview" />
                  ) : (
                    <div className="preview-image-placeholder">
                      <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M19 3H5C3.89543 3 3 3.89543 3 5V19C3 20.1046 3.89543 21 5 21H19C20.1046 21 21 20.1046 21 19V5C21 3.89543 20.1046 3 19 3Z" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                        <path d="M8.5 10C9.32843 10 10 9.32843 10 8.5C10 7.67157 9.32843 7 8.5 7C7.67157 7 7 7.67157 7 8.5C7 9.32843 7.67157 10 8.5 10Z" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                        <path d="M21 15L16 10L5 21" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                      </svg>
                      <span>No image selected</span>
                    </div>
                  )}
                </div>
                <div className="preview-details">
                  <h4>{form.name || 'Untitled Course'}</h4>
                  <p className="preview-description">{truncateText(form.description || 'No description provided', 150)}</p>
                  <div className="preview-stats">
                    <div className="stat">
                      <span>Duration:</span>
                      <span>{form.duration} minutes</span>
                    </div>
                    <div className="stat">
                      <span>Price:</span>
                      <span>${form.price}</span>
                    </div>
                    <div className="stat">
                      <span>Stages:</span>
                      <span>{form.stages.length}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        );
      case 'stages':
        return (
          <div className="form-content">
            <div className="stages-section">
              <h3>Course Stages</h3>
              <div className="stage-form">
                {renderField("Stage Name", "name", "text", "Enter stage name", currentStage.name, handleStageChange)}
                {renderField("Description", "description", "textarea", "Enter stage description", currentStage.description, handleStageChange)}
                {renderField("Duration", "duration", "number", "Duration in minutes", currentStage.duration, handleStageChange)}
                
                <div className="form-group">
                  <label htmlFor="video">Upload Video (optional)</label>
                  <input
                    id="video"
                    name="video"
                    type="file"
                    accept="video/*"
                    onChange={handleStageFileChange}
                    className="form-input"
                  />
                </div>

                <div style={{ textAlign: 'center' }}>
                  <button type="button" className="btn btn-secondary" onClick={addStage}>
                    Add Stage
                  </button>
                </div>
              </div>
            </div>
            <div className="stages-overview">
              <h3>Stages Overview</h3>
              <div className="overview-stats">
                <div className="stat-item">
                  <span className="stat-value">{form.stages.length}</span>
                  <span className="stat-label">Total Stages</span>
                </div>
                <div className="stat-item">
                  <span className="stat-value">{calculateTotalDuration()}</span>
                  <span className="stat-label">Total Duration</span>
                </div>
                <div className="stat-item">
                  <span className="stat-value">{form.stages.filter(s => s.videoFile).length}</span>
                  <span className="stat-label">Videos Uploaded</span>
                </div>
              </div>
              <div className="stages-timeline">
                {form.stages.length > 0 ? (
                  form.stages.map((stage, index) => (
                    <div key={index} className="timeline-item">
                      <div className="timeline-number">{index + 1}</div>
                      <div className="timeline-content">
                        <div className="timeline-header">
                          <h4>{stage.name}</h4>
                          <button 
                            className="btn btn-danger btn-sm"
                            onClick={() => removeStage(index)}
                            title="Remove stage"
                          >
                            <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                              <path d="M19 7L18.1327 19.1425C18.0579 20.1891 17.187 21 16.1378 21H7.86224C6.81296 21 5.94208 20.1891 5.86732 19.1425L5 7M10 11V17M14 11V17M15 7V4C15 3.44772 14.5523 3 14 3H10C9.44772 3 9 3.44772 9 4V7M4 7H20" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                            </svg>
                          </button>
                        </div>
                        <div className="timeline-details">
                          <span className="timeline-duration">{stage.duration} min</span>
                          <span className="timeline-videos">{stage.videoFile ? 1 : 0} videos</span>
                          {stage.description && (
                            <p className="timeline-description">{stage.description}</p>
                          )}
                        </div>
                      </div>
                    </div>
                  ))
                ) : (
                  <div className="empty-stages">
                    <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                      <path d="M12 22C17.5228 22 22 17.5228 22 12C22 6.47715 17.5228 2 12 2C6.47715 2 2 6.47715 2 12C2 17.5228 6.47715 22 12 22Z" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                      <path d="M12 8V12" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                      <path d="M12 16H12.01" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                    </svg>
                    <span>No stages added yet</span>
                    <p>Add your first stage to start building your course</p>
                  </div>
                )}
              </div>
            </div>
          </div>
        );
      case 'summary':
        return (
          <div className="summary-section">
            <h3>Course Summary</h3>
            <div className="summary-content">
              <div className="summary-group">
                <h4>Basic Information</h4>
                <div className="summary-item">
                  <span>Course Name:</span>
                  <span>{form.name || 'Not set'}</span>
                </div>
                <div className="summary-item">
                  <span>Description:</span>
                  <span>{form.description || 'Not set'}</span>
                </div>
                <div className="summary-item">
                  <span>Duration:</span>
                  <span>{form.duration} minutes</span>
                </div>
                <div className="summary-item">
                  <span>Price:</span>
                  <span>${form.price}</span>
                </div>
                <div className="summary-item">
                  <span>Category:</span>
                  <span>{categories.find(c => c.id === selectedCategory)?.name || 'Not set'}</span>
                </div>
                <div className="summary-item">
                  <span>Subcategory:</span>
                  <span>{subcategories.find(s => s.id === selectedSubcategory)?.name || 'Not set'}</span>
                </div>
              </div>

              <div className="summary-group">
                <h4>Stages Overview</h4>
                <div className="summary-item">
                  <span>Total Stages:</span>
                  <span>{form.stages.length}</span>
                </div>
                <div className="summary-item">
                  <span>Total Duration:</span>
                  <span>{calculateTotalDuration()} minutes</span>
                </div>
                <div className="summary-item">
                  <span>Videos:</span>
                  <span>{form.stages.filter(s => s.videoFile).length} uploaded</span>
                </div>
              </div>

              <div className="summary-group">
                <h4>Stages List</h4>
                {form.stages.map((stage, index) => (
                  <div key={index} className="summary-stage-item">
                    <div className="summary-item">
                      <span>Stage {index + 1}:</span>
                      <span>{stage.name}</span>
                    </div>
                    <div className="summary-item">
                      <span>Duration:</span>
                      <span>{stage.duration} minutes</span>
                    </div>
                    {stage.videoFile && (
                      <div className="summary-item">
                        <span>Video:</span>
                        <span>{stage.videoFile.name}</span>
                      </div>
                    )}
                  </div>
                ))}
              </div>
            </div>
          </div>
        );
    }
  };

  return (
    <div className="page-layout">
      <Sidebar />
      <main className="main-content">
        <form className="course-form" onSubmit={handleSubmit}>
          <div className="form-title-container">
            <h2 className="form-title">Add New Course</h2>
          </div>

          <div className="tabs">
            <button
              type="button"
              className={`tab-btn ${activeTab === 'details' ? 'active' : ''}`}
              onClick={() => setActiveTab('details')}
            >
              Course Details
            </button>
            <button
              type="button"
              className={`tab-btn ${activeTab === 'stages' ? 'active' : ''}`}
              onClick={() => setActiveTab('stages')}
            >
              Course Stages
            </button>
            <button
              type="button"
              className={`tab-btn ${activeTab === 'summary' ? 'active' : ''}`}
              onClick={() => setActiveTab('summary')}
            >
              Summary
            </button>
          </div>

          {renderTabContent()}

          {submissionStatus.isSubmitting && (
            <div className="submission-status">
              <div className="status-content">
                <div className="status-message">{submissionStatus.currentStep}</div>
                <div className="progress-bar">
                  <div 
                    className="progress-fill"
                    style={{ width: `${submissionStatus.progress}%` }}
                  />
                </div>
              </div>
            </div>
          )}

          {submissionStatus.error && (
            <div className="error-message">
              {submissionStatus.error}
            </div>
          )}

          <button 
            type="submit" 
            className="btn btn-primary"
            disabled={submissionStatus.isSubmitting}
          >
            {submissionStatus.isSubmitting ? 'Creating Course...' : 'Submit Course'}
          </button>
        </form>
      </main>
    </div>
  );
};
