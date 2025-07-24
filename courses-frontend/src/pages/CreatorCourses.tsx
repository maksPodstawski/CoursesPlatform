import { useEffect, useState } from 'react';
import { config } from "../config.ts";
import { getCourseImageUrl } from "../utils/getCourseImageUrl";
import '../styles/AddCourse.css';
import TextEditor from '../components/TextEditor';
import { getCategories, getSubcategories } from '../services/categoryService';
import { Category, Subcategory } from '../types/courses';
import Sidebar from '../components/Sidebar';
import { validateCourseForm, CourseFormData, CourseFieldErrors } from '../validation/courseValidation';
import { validateStageForm, StageFormData } from '../validation/stageValidation';

interface CreatorCourse {
    id: string;
    name: string;
    description: string | null;
    imageUrl: string;
    duration: number;
    price: number;
    createdAt: string;
    updatedAt: string | null;
    averageRating: number | null;
    reviewsCount: number;
    stagesCount: number;
    subcategories: string[];
    creators: string[];
    difficulty?: number;
    isHidden?: boolean;
}

interface Stage {
    id: string;
    name: string;
    description: string;
    duration: number;
    videoPath?: string | null;
    videoFile?: File | null; 
}

type Tab = 'details' | 'stages' | 'summary';

const CreatorCourses = () => {
    const [courses, setCourses] = useState<CreatorCourse[]>([]);
    const [loading, setLoading] = useState(true);
    const [courseStagesCountMap, setCourseStagesCountMap] = useState<{ [courseId: string]: number }>({});
    const [selectedCourse, setSelectedCourse] = useState<CreatorCourse | null>(null);
    const [activeTab, setActiveTab] = useState<Tab>('details');
    const [editForm, setEditForm] = useState<any>(null);
    const [imageFile, setImageFile] = useState<File | null>(null);
    const [localStages, setLocalStages] = useState<Stage[]>([]);
    const [stageEdit, setStageEdit] = useState<Stage | null>(null);
    const [statusMsg, setStatusMsg] = useState<string | null>(null);
    const [progress, setProgress] = useState<number>(0);
    const [errorMsg, setErrorMsg] = useState<string | null>(null);
    const [categories, setCategories] = useState<Category[]>([]);
    const [subcategories, setSubcategories] = useState<Subcategory[]>([]);
    const [selectedCategory, setSelectedCategory] = useState<string>("");
    const [selectedSubcategory, setSelectedSubcategory] = useState<string>("");
    const [newStage, setNewStage] = useState<{ name: string; description: string; duration: number | ''; price: number | ''; videoFile: File | null }>({ name: '', description: '', duration: '', price: '', videoFile: null });
    const [coursesListOpen, setCoursesListOpen] = useState(true);
    const [fieldErrors, setFieldErrors] = useState<CourseFieldErrors>({});
    const [touched, setTouched] = useState<{ [key: string]: boolean }>({});
    const [isFormValid, setIsFormValid] = useState(true);
    const [stageFieldErrors, setStageFieldErrors] = useState<Partial<Record<string, string>>>({});
    const [stageTouched, setStageTouched] = useState<{ [key: string]: boolean }>({});

    useEffect(() => {
        const fetchCreatorCourses = async () => {
            try {
                const response = await fetch(`${config.apiBaseUrl}${config.apiEndpoints.getCreatorCourses}`, {
                    credentials: 'include'
                });
                const data = await response.json();
                setCourses(data);
                const counts: { [courseId: string]: number } = {};
                await Promise.all(
                    data.map(async (course: CreatorCourse) => {
                        try {
                            const res = await fetch(`${config.apiBaseUrl}/api/stages/course/${course.id}`, { credentials: 'include' });
                            if (res.ok) {
                                const stages = await res.json();
                                counts[course.id] = Array.isArray(stages) ? stages.length : 0;
                            }
                        } catch {}
                    })
                );
                setCourseStagesCountMap(counts);
            } catch (error) {
                console.error('Error:', error);
            } finally {
                setLoading(false);
            }
        };
        fetchCreatorCourses();
    }, []);

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

    useEffect(() => {
        if (selectedCourse) {
            setEditForm({
                ...selectedCourse,
                description: selectedCourse.description || '',
                duration: selectedCourse.duration ? String(selectedCourse.duration) : '',
                price: selectedCourse.price ? String(selectedCourse.price) : '',
            });
            setImageFile(null);
            if (selectedCourse.subcategories && selectedCourse.subcategories.length > 0) {
                setSelectedSubcategory(selectedCourse.subcategories[0]);
            } else {
                setSelectedSubcategory("");
            }
            fetch(`${config.apiBaseUrl}/api/stages/course/${selectedCourse.id}`, { credentials: 'include' })
                .then(res => res.json())
                .then(data => setLocalStages(data))
                .catch(() => setLocalStages([]));
        }
    }, [selectedCourse]);

    useEffect(() => {
        if (selectedCourse) {
            setCourseStagesCountMap(prev => ({
                ...prev,
                [selectedCourse.id]: localStages.length
            }));
        }
    }, [localStages, selectedCourse]);

    useEffect(() => {
        if (!editForm) return;
        const formData: CourseFormData = {
            name: editForm.name,
            description: editForm.description,
            duration: Number(editForm.duration),
            price: Number(editForm.price),
            difficulty: Number(editForm.difficulty),
            selectedCategory,
            selectedSubcategory,
            imageFile: imageFile,
        };
        const errors = validateCourseForm(formData);
        setFieldErrors(errors);
        setIsFormValid(Object.keys(errors).length === 0);
    }, [editForm, selectedCategory, selectedSubcategory, localStages, imageFile]);


    const handleCourseFieldChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        const { name, value, type } = e.target;
        if (type === 'file' && name === 'imageFile') {
            const fileInput = e.target as HTMLInputElement;
            setImageFile(fileInput.files && fileInput.files.length > 0 ? fileInput.files[0] : null);
        } else {
            setEditForm((prev: any) => ({ ...prev, [name]: type === 'number' ? (value === '' ? '' : Number(value)) : value }));
        }
    };

    const handleDifficultyChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        setEditForm((prev: any) => ({ ...prev, difficulty: Number(e.target.value) }));
    };

    const handleDescriptionChange = (val: string) => {
        setEditForm((prev: any) => ({ ...prev, description: val }));
    };

    const handleCategoryChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        setSelectedCategory(e.target.value);
        setSelectedSubcategory("");
    };
    const handleSubcategoryChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        setSelectedSubcategory(e.target.value);
    };

    const handleSaveCourse = async () => {
        setErrorMsg(null);
        setStatusMsg('Saving...');
        setProgress(10);
        setFieldErrors({});
        if (!selectedCourse) return;

        // --- WALIDACJA FORMULARZA ---
        const formData: CourseFormData = {
            name: editForm.name,
            description: editForm.description,
            duration: Number(editForm.duration),
            price: Number(editForm.price),
            difficulty: Number(editForm.difficulty),
            selectedCategory,
            selectedSubcategory,
            imageFile: imageFile,
        };
        const errors = validateCourseForm(formData);
        if (Object.keys(errors).length > 0) {
            setFieldErrors(errors);
            setStatusMsg(null);
            setProgress(0);
            return;
        }
        // --- KONIEC WALIDACJI ---

        try {
            let imageUrl = editForm.imageUrl;
            if (imageFile) {
                const formData = new FormData();
                formData.append('file', imageFile);
                const resImg = await fetch(`${config.apiBaseUrl}/api/courses/${selectedCourse.id}/image`, {
                    method: 'POST',
                    credentials: 'include',
                    body: formData
                });
                if (!resImg.ok) throw new Error('Image upload failed');
                const data = await resImg.json();
                imageUrl = data.imageUrl;
            }

            setStatusMsg('Saving course details...');
            setProgress(30);
            const formData = new FormData();
            formData.append('Name', editForm.name);
            formData.append('Description', editForm.description);
            formData.append('Duration', editForm.duration.toString());
            formData.append('Price', editForm.price.toString());
            formData.append('IsHidden', editForm.isHidden ?? false);
            formData.append('Difficulty', editForm.difficulty ?? 1);
            if (imageFile) {
                formData.append('Image', imageFile);
            }
            if (selectedSubcategory) {
                formData.append('SubcategoryIds', selectedSubcategory);
            }
            for (let [key, value] of formData.entries()) {
                console.log(key, value);
            }
            const res = await fetch(`${config.apiBaseUrl}/api/Courses/${selectedCourse.id}`, {
                method: 'PUT',
                credentials: 'include',
                body: formData
            });
            if (!res.ok) {
                let errorMsg = 'Failed to update course';
                try {
                    const errorData = await res.json();
                    errorMsg = errorData?.Name ? errorData.Name[0] : (errorData?.error || errorMsg);
                } catch {
                    const text = await res.text();
                    errorMsg = text || errorMsg;
                }
                setErrorMsg(errorMsg);
                setProgress(0);
                return;
            }
            setStatusMsg('Synchronizing stages...');
            setProgress(60);
            const backendStagesRes = await fetch(`${config.apiBaseUrl}/api/stages/course/${selectedCourse.id}`, { credentials: 'include' });
            const backendStages: Stage[] = await backendStagesRes.json();
            const newStages = localStages.filter(s => !backendStages.some(bs => bs.id === s.id));
            for (const stage of newStages) {
                const res = await fetch(`${config.apiBaseUrl}/api/stages`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    credentials: 'include',
                    body: JSON.stringify({
                        name: stage.name,
                        description: stage.description,
                        duration: stage.duration,
                        courseId: selectedCourse.id
                    })
                });
                if (!res.ok) {
                    let errorMsg = 'Stage creation failed.';
                    try {
                        const errorData = await res.json();
                        errorMsg = errorData?.Name ? errorData.Name[0] : (errorData?.error || errorMsg);
                    } catch {
                        const text = await res.text();
                        errorMsg = text || errorMsg;
                    }
                    throw new Error(errorMsg);
                }
                const createdStage = await res.json();
                if (stage.videoFile) {
                    const formData = new FormData();
                    formData.append('file', stage.videoFile);
                    const videoRes = await fetch(`${config.apiBaseUrl}/api/stages/${createdStage.id}/video`, {
                        method: 'POST',
                        credentials: 'include',
                        body: formData
                    });
                    if (!videoRes.ok) throw new Error('Video upload failed');
                }
            }
            for (const stage of localStages.filter(s => backendStages.some(bs => bs.id === s.id))) {
                const res = await fetch(`${config.apiBaseUrl}/api/stages/${stage.id}`, {
                    method: 'PUT',
                    headers: { 'Content-Type': 'application/json' },
                    credentials: 'include',
                    body: JSON.stringify({
                        id: stage.id,
                        name: stage.name,
                        description: stage.description,
                        duration: stage.duration
                    })
                });
                if (!res.ok) throw new Error('Stage update failed.');
                if (stage.videoFile) {
                    const formData = new FormData();
                    formData.append('file', stage.videoFile);
                    const videoRes = await fetch(`${config.apiBaseUrl}/api/stages/${stage.id}/video`, {
                        method: 'POST',
                        credentials: 'include',
                        body: formData
                    });
                    if (!videoRes.ok) throw new Error('Video upload failed');
                }
            }
            for (const stage of backendStages.filter(bs => !localStages.some(s => s.id === bs.id))) {
                await fetch(`${config.apiBaseUrl}/api/stages/${stage.id}`, {
                    method: 'DELETE',
                    credentials: 'include'
                });
            }
            setStatusMsg('Course and stages updated!');
            setProgress(100);
            setCourses(prev => prev.map(c => c.id === selectedCourse.id ? {
                ...c,
                name: editForm.name,
                description: editForm.description,
                imageUrl: imageUrl,
                duration: editForm.duration,
                price: editForm.price,
                isHidden: editForm.isHidden ?? false,
                difficulty: editForm.difficulty ?? 1,
                subcategories: selectedSubcategory ? [selectedSubcategory] : c.subcategories
            } : c));
            setCourseStagesCountMap(prev => ({
                ...prev,
                [selectedCourse.id]: localStages.length
            }));
            setTimeout(() => {
                setStatusMsg(null);
                setProgress(0);
            }, 1500);
        } catch (e: any) {
            setErrorMsg(e.message);
            setProgress(0);
        }
    };

    const handleStageFieldChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value, type } = e.target;
        setNewStage(prev => ({ ...prev, [name]: type === 'number' ? (value === '' ? '' : Number(value)) : value }));
    };

    const handleStageDescriptionChange = (val: string) => {
        setNewStage(prev => ({ ...prev, description: val }));
    };

    const handleStageFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.files && e.target.files.length > 0) {
            setNewStage(prev => ({ ...prev, videoFile: e.target.files![0] }));
        }
    };

    const handleAddStage = () => {
        const stageData: StageFormData = {
            name: newStage.name,
            description: newStage.description,
            duration: Number(newStage.duration),
        };
        const errors = validateStageForm(stageData);
        setStageFieldErrors(errors);
        setStageTouched({ name: true, description: true, duration: true });
        if (Object.keys(errors).length > 0) return;
        setLocalStages(prev => ([...prev, {
            id: Math.random().toString(36).substr(2, 9),
            name: newStage.name,
            description: newStage.description,
            duration: Number(newStage.duration),
            videoFile: newStage.videoFile,
            videoPath: newStage.videoFile ? URL.createObjectURL(newStage.videoFile) : undefined,
        }]));
        setNewStage({ name: '', description: '', duration: '', price: '', videoFile: null });
        setStageFieldErrors({});
        setStageTouched({});
    };
    const handleRemoveStage = (id: string) => {
        setLocalStages(prev => prev.filter(s => s.id !== id));
    };
    const handleEditStage = (stage: Stage) => {
        setStageEdit(stage);
    };
    const handleSaveStageEdit = () => {
        if (!stageEdit) return;
        setLocalStages(prev => prev.map(s => s.id === stageEdit.id ? stageEdit : s));
        setStageEdit(null);
    };

    const renderTabs = () => (
        <div className="tabs">
            <button type="button" className={`tab-btn ${activeTab === 'details' ? 'active' : ''}`} onClick={() => setActiveTab('details')}>Course Details</button>
            <button type="button" className={`tab-btn ${activeTab === 'stages' ? 'active' : ''}`} onClick={() => setActiveTab('stages')}>Course Stages</button>
            <button type="button" className={`tab-btn ${activeTab === 'summary' ? 'active' : ''}`} onClick={() => setActiveTab('summary')}>Summary</button>
        </div>
    );

    const renderDetailsTab = () => (
        <div className="form-content">
            <div className="course-section">
                <h3>Course Details</h3>
                <div className="form-group">
                    <label>Course Name</label>
                    <input className="form-input" name="name" value={editForm?.name || ''} onChange={handleCourseFieldChange} onBlur={() => setTouched(t => ({ ...t, name: true }))} />
                    {touched.name && fieldErrors.name && (
                        <div className="field-error">
                            <svg
                                xmlns="http://www.w3.org/2000/svg"
                                fill="none"
                                viewBox="0 0 24 24"
                                strokeWidth={2}
                                stroke="currentColor"
                                className="icon"
                                width="16"
                                height="16"
                            >
                                <path
                                    strokeLinecap="round"
                                    strokeLinejoin="round"
                                    d="M12 9v2m0 4h.01M12 2a10 10 0 100 20 10 10 0 000-20z"
                                />
                            </svg>
                            <span>{fieldErrors.name}</span>
                        </div>
                    )}
                </div>
                <div className="form-group">
                    <label>Description</label>
                    <div tabIndex={-1} onBlur={() => setTouched(t => ({ ...t, description: true }))} style={{ outline: 'none' }}>
                        <TextEditor key={selectedCourse?.id || 'no-course'} value={editForm?.description || ''} onChange={handleDescriptionChange} placeholder="Enter course description" />
                    </div>
                    {touched.description && fieldErrors.description && (
                        <div className="field-error">
                            <svg
                                xmlns="http://www.w3.org/2000/svg"
                                fill="none"
                                viewBox="0 0 24 24"
                                strokeWidth={2}
                                stroke="currentColor"
                                className="icon"
                                width="16"
                                height="16"
                            >
                                <path
                                    strokeLinecap="round"
                                    strokeLinejoin="round"
                                    d="M12 9v2m0 4h.01M12 2a10 10 0 100 20 10 10 0 000-20z"
                                />
                            </svg>
                            <span>{fieldErrors.description}</span>
                        </div>
                    )}
                </div>
                <div className="form-group">
                    <label>Course Image</label>
                    <input className="form-input" name="imageFile" type="file" accept="image/*" onChange={handleCourseFieldChange} onBlur={() => setTouched(t => ({ ...t, imageFile: true }))} />
                    {touched.imageFile && fieldErrors.imageFile && (
                        <div className="field-error">
                            <svg
                                xmlns="http://www.w3.org/2000/svg"
                                fill="none"
                                viewBox="0 0 24 24"
                                strokeWidth={2}
                                stroke="currentColor"
                                className="icon"
                                width="16"
                                height="16"
                                style={{ marginRight: 4, verticalAlign: 'middle' }}
                            >
                                <path
                                    strokeLinecap="round"
                                    strokeLinejoin="round"
                                    d="M12 9v2m0 4h.01M12 2a10 10 0 100 20 10 10 0 000-20z"
                                />
                            </svg>
                            <span>{fieldErrors.imageFile}</span>
                        </div>
                    )}
                </div>
                <div className="duration-price-group">
                    <div className="form-group">
                        <label>Duration</label>
                        <input className="form-input" name="duration" type="number" value={editForm?.duration || ''} onChange={handleCourseFieldChange} onBlur={() => setTouched(t => ({ ...t, duration: true }))} placeholder="Enter duration" />
                        {touched.duration && fieldErrors.duration && (
                            <div className="field-error">
                                <svg
                                    xmlns="http://www.w3.org/2000/svg"
                                    fill="none"
                                    viewBox="0 0 24 24"
                                    strokeWidth={2}
                                    stroke="currentColor"
                                    className="icon"
                                    width="16"
                                    height="16"
                                >
                                    <path
                                        strokeLinecap="round"
                                        strokeLinejoin="round"
                                        d="M12 9v2m0 4h.01M12 2a10 10 0 100 20 10 10 0 000-20z"
                                    />
                                </svg>
                                <span>{fieldErrors.duration}</span>
                            </div>
                        )}
                    </div>
                    <div className="form-group">
                        <label>Price</label>
                        <input className="form-input" name="price" type="number" value={editForm?.price || ''} onChange={handleCourseFieldChange} onBlur={() => setTouched(t => ({ ...t, price: true }))} placeholder="Enter price" />
                        {touched.price && fieldErrors.price && (
                            <div className="field-error">
                                <svg
                                    xmlns="http://www.w3.org/2000/svg"
                                    fill="none"
                                    viewBox="0 0 24 24"
                                    strokeWidth={2}
                                    stroke="currentColor"
                                    className="icon"
                                    width="16"
                                    height="16"
                                >
                                    <path
                                        strokeLinecap="round"
                                        strokeLinejoin="round"
                                        d="M12 9v2m0 4h.01M12 2a10 10 0 100 20 10 10 0 000-20z"
                                    />
                                </svg>
                                <span>{fieldErrors.price}</span>
                            </div>
                        )}
                    </div>
                </div>
                <div className="category-row">
                    <div className="form-group">
                        <label className="category-label">Category</label>
                        <select className="category-select" value={selectedCategory} onChange={handleCategoryChange} onBlur={() => setTouched(t => ({ ...t, selectedCategory: true }))} required>
                            <option value="">Select a category</option>
                            {categories.map(cat => (
                                <option key={cat.id} value={cat.id}>{cat.name}</option>
                            ))}
                        </select>
                        {touched.selectedCategory && fieldErrors.selectedCategory && (
                            <div className="field-error">
                                <svg
                                    xmlns="http://www.w3.org/2000/svg"
                                    fill="none"
                                    viewBox="0 0 24 24"
                                    strokeWidth={2}
                                    stroke="currentColor"
                                    className="icon"
                                    width="16"
                                    height="16"
                                >
                                    <path
                                        strokeLinecap="round"
                                        strokeLinejoin="round"
                                        d="M12 9v2m0 4h.01M12 2a10 10 0 100 20 10 10 0 000-20z"
                                    />
                                </svg>
                                <span>{fieldErrors.selectedCategory}</span>
                            </div>
                        )}
                    </div>
                    <div className="form-group">
                        <label className="subcategory-label">Subcategory</label>
                        <select className="subcategory-select" value={selectedSubcategory} onChange={handleSubcategoryChange} onBlur={() => setTouched(t => ({ ...t, selectedSubcategory: true }))} required disabled={!selectedCategory}>
                            <option value="">Select a subcategory</option>
                            {subcategories.map(sub => (
                                <option key={sub.id} value={sub.id}>{sub.name}</option>
                            ))}
                        </select>
                        {touched.selectedSubcategory && fieldErrors.selectedSubcategory && (
                            <div className="field-error">
                                <svg
                                    xmlns="http://www.w3.org/2000/svg"
                                    fill="none"
                                    viewBox="0 0 24 24"
                                    strokeWidth={2}
                                    stroke="currentColor"
                                    className="icon"
                                    width="16"
                                    height="16"
                                >
                                    <path
                                        strokeLinecap="round"
                                        strokeLinejoin="round"
                                        d="M12 9v2m0 4h.01M12 2a10 10 0 100 20 10 10 0 000-20z"
                                    />
                                </svg>
                                <span>{fieldErrors.selectedSubcategory}</span>
                            </div>
                        )}
                    </div>
                    <div className="form-group">
                        <label className="difficulty-label">Difficulty Level</label>
                        <select className="difficulty-select" name="difficulty" value={editForm?.difficulty || 1} onChange={handleDifficultyChange} onBlur={() => setTouched(t => ({ ...t, difficulty: true }))} required>
                            <option value={1}>Beginner</option>
                            <option value={2}>Intermediate</option>
                            <option value={3}>Advanced</option>
                        </select>
                        {touched.difficulty && fieldErrors.difficulty && (
                            <div className="field-error">
                                <svg
                                    xmlns="http://www.w3.org/2000/svg"
                                    fill="none"
                                    viewBox="0 0 24 24"
                                    strokeWidth={2}
                                    stroke="currentColor"
                                    className="icon"
                                    width="16"
                                    height="16"
                                >
                                    <path
                                        strokeLinecap="round"
                                        strokeLinejoin="round"
                                        d="M12 9v2m0 4h.01M12 2a10 10 0 100 20 10 10 0 000-20z"
                                    />
                                </svg>
                                <span>{fieldErrors.difficulty}</span>
                            </div>
                        )}
                    </div>
                </div>
                <div className="form-group" style={{ display: 'flex', alignItems: 'center', gap: 12, marginTop: 16 }}>
                    <label style={{ margin: 0, fontWeight: 500, color: '#e0e0e0' }}>Hidden</label>
                    <label style={{ display: 'flex', alignItems: 'center', cursor: 'pointer', marginLeft: 8 }}>
                        <input type="checkbox" name="isHidden" checked={!!editForm?.isHidden} onChange={e => setEditForm((prev: any) => ({ ...prev, isHidden: e.target.checked }))} style={{ width: 0, height: 0, opacity: 0, position: 'absolute' }} />
                        <span style={{
                            width: 40,
                            height: 22,
                            background: !!editForm?.isHidden ? '#3dc55f' : '#2a2a2a',
                            borderRadius: 12,
                            position: 'relative',
                            display: 'inline-block',
                            transition: 'background 0.2s',
                            marginRight: 8
                        }}>
                            <span style={{
                                position: 'absolute',
                                left: !!editForm?.isHidden ? 20 : 2,
                                top: 2,
                                width: 18,
                                height: 18,
                                background: '#fff',
                                borderRadius: '50%',
                                boxShadow: '0 1px 4px rgba(0,0,0,0.15)',
                                transition: 'left 0.2s'
                            }} />
                        </span>
                        <span style={{ color: '#b0b0b0', fontSize: 14 }}>{!!editForm?.isHidden ? 'Yes' : 'No'}</span>
                    </label>
                </div>
            </div>
            <div className="course-preview">
                <h3>Course Preview</h3>
                <div className="preview-content">
                    <div className="preview-image">
                        {imageFile ? (
                            <img src={URL.createObjectURL(imageFile)} alt="Course preview" />
                        ) : (
                            editForm?.imageUrl ? (
                                <img src={getCourseImageUrl(editForm.imageUrl)} alt="Course preview" />
                            ) : (
                                <div className="preview-image-placeholder">
                                    <span>No image selected</span>
                                </div>
                            )
                        )}
                    </div>
                    <div className="preview-details">
                        <h4>{editForm?.name || 'Untitled Course'}</h4>
                        <div className="preview-description" dangerouslySetInnerHTML={{ __html: editForm?.description || 'No description provided' }} />
                        <div className="preview-stats">
                            <div className="stat">
                                <span>Duration:</span>
                                <span>{editForm?.duration} minutes</span>
                            </div>
                            <div className="stat">
                                <span>Price:</span>
                                <span>${editForm?.price}</span>
                            </div>
                            <div className="stat">
                                <span>Difficulty:</span>
                                <span>{editForm?.difficulty === 1 ? 'Beginner' : editForm?.difficulty === 2 ? 'Intermediate' : 'Advanced'}</span>
                            </div>
                            <div className="stat">
                                <span>Stages:</span>
                                <span>{localStages.length}</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );

    
    const renderStagesTab = () => (
        <div className="form-content">
            {stageEdit ? (
                <div className="stages-section" style={{ width: '100%' }}>
                    <h3>Edit Stage</h3>
                    <div className="form-group">
                        <label>Stage Name</label>
                        <input className="form-input" name="name" value={stageEdit.name} onChange={e => setStageEdit({ ...stageEdit, name: e.target.value })} onBlur={() => setStageTouched(t => ({ ...t, name: true }))} />
                        {stageTouched.name && stageFieldErrors.name && (
                            <div className="field-error">
                                <svg
                                    xmlns="http://www.w3.org/2000/svg"
                                    fill="none"
                                    viewBox="0 0 24 24"
                                    strokeWidth={2}
                                    stroke="currentColor"
                                    className="icon"
                                    width="16"
                                    height="16"
                                >
                                    <path
                                        strokeLinecap="round"
                                        strokeLinejoin="round"
                                        d="M12 9v2m0 4h.01M12 2a10 10 0 100 20 10 10 0 000-20z"
                                    />
                                </svg>
                                <span>{stageFieldErrors.name}</span>
                            </div>
                        )}
                    </div>
                    <div className="form-group">
                        <label>Description</label>
                        <div tabIndex={-1} onBlur={() => setStageTouched(t => ({ ...t, description: true }))} style={{ outline: 'none' }}>
                            <TextEditor value={stageEdit.description} onChange={val => setStageEdit({ ...stageEdit, description: val })} placeholder="Enter stage description" />
                        </div>
                        {stageTouched.description && stageFieldErrors.description && (
                            <div className="field-error">
                                <svg
                                    xmlns="http://www.w3.org/2000/svg"
                                    fill="none"
                                    viewBox="0 0 24 24"
                                    strokeWidth={2}
                                    stroke="currentColor"
                                    className="icon"
                                    width="16"
                                    height="16"
                                >
                                    <path
                                        strokeLinecap="round"
                                        strokeLinejoin="round"
                                        d="M12 9v2m0 4h.01M12 2a10 10 0 100 20 10 10 0 000-20z"
                                    />
                                </svg>
                                <span>{stageFieldErrors.description}</span>
                            </div>
                        )}
                    </div>
                    <div className="form-group">
                        <label>Duration (min)</label>
                        <input className="form-input" name="duration" type="number" value={stageEdit.duration} onChange={e => setStageEdit({ ...stageEdit, duration: Number(e.target.value) })} onBlur={() => setStageTouched(t => ({ ...t, duration: true }))} />
                        {stageTouched.duration && stageFieldErrors.duration && (
                            <div className="field-error">
                                <svg
                                    xmlns="http://www.w3.org/2000/svg"
                                    fill="none"
                                    viewBox="0 0 24 24"
                                    strokeWidth={2}
                                    stroke="currentColor"
                                    className="icon"
                                    width="16"
                                    height="16"
                                >
                                    <path
                                        strokeLinecap="round"
                                        strokeLinejoin="round"
                                        d="M12 9v2m0 4h.01M12 2a10 10 0 100 20 10 10 0 000-20z"
                                    />
                                </svg>
                                <span>{stageFieldErrors.duration}</span>
                            </div>
                        )}
                    </div>
                    <div className="form-group">
                        <label>Upload Video (optional)</label>
                        <input className="form-input" name="video" type="file" accept="video/*" onChange={e => setStageEdit({ ...stageEdit, videoFile: e.target.files && e.target.files[0] })} />
                    </div>
                    <button className="btn btn-primary" type="button" onClick={handleSaveStageEdit}>Save Stage</button>
                    <button className="btn btn-secondary" type="button" onClick={() => setStageEdit(null)}>Cancel</button>
                </div>
            ) : (
                <div className="stages-section">
                    <h3>Add Stage</h3>
                    <div className="form-group">
                        <label>Stage Name</label>
                        <input className="form-input" name="name" value={newStage.name} onChange={handleStageFieldChange} onBlur={() => setStageTouched(t => ({ ...t, name: true }))} />
                        {stageTouched.name && stageFieldErrors.name && (
                            <div className="field-error">
                                <svg
                                    xmlns="http://www.w3.org/2000/svg"
                                    fill="none"
                                    viewBox="0 0 24 24"
                                    strokeWidth={2}
                                    stroke="currentColor"
                                    className="icon"
                                    width="16"
                                    height="16"
                                >
                                    <path
                                        strokeLinecap="round"
                                        strokeLinejoin="round"
                                        d="M12 9v2m0 4h.01M12 2a10 10 0 100 20 10 10 0 000-20z"
                                    />
                                </svg>
                                <span>{stageFieldErrors.name}</span>
                            </div>
                        )}
                    </div>
                    <div className="form-group">
                        <label>Description</label>
                        <div tabIndex={-1} onBlur={() => setStageTouched(t => ({ ...t, description: true }))} style={{ outline: 'none' }}>
                            <TextEditor value={newStage.description} onChange={handleStageDescriptionChange} placeholder="Enter stage description" />
                        </div>
                        {stageTouched.description && stageFieldErrors.description && (
                            <div className="field-error">
                                <svg
                                    xmlns="http://www.w3.org/2000/svg"
                                    fill="none"
                                    viewBox="0 0 24 24"
                                    strokeWidth={2}
                                    stroke="currentColor"
                                    className="icon"
                                    width="16"
                                    height="16"
                                >
                                    <path
                                        strokeLinecap="round"
                                        strokeLinejoin="round"
                                        d="M12 9v2m0 4h.01M12 2a10 10 0 100 20 10 10 0 000-20z"
                                    />
                                </svg>
                                <span>{stageFieldErrors.description}</span>
                            </div>
                        )}
                    </div>
                    <div className="form-group">
                        <label>Duration (min)</label>
                        <input className="form-input" name="duration" type="number" value={newStage.duration || ''} onChange={handleStageFieldChange} onBlur={() => setStageTouched(t => ({ ...t, duration: true }))} placeholder="Enter duration" />
                        {stageTouched.duration && stageFieldErrors.duration && (
                            <div className="field-error">
                                <svg
                                    xmlns="http://www.w3.org/2000/svg"
                                    fill="none"
                                    viewBox="0 0 24 24"
                                    strokeWidth={2}
                                    stroke="currentColor"
                                    className="icon"
                                    width="16"
                                    height="16"
                                >
                                    <path
                                        strokeLinecap="round"
                                        strokeLinejoin="round"
                                        d="M12 9v2m0 4h.01M12 2a10 10 0 100 20 10 10 0 000-20z"
                                    />
                                </svg>
                                <span>{stageFieldErrors.duration}</span>
                            </div>
                        )}
                    </div>
                    <div className="form-group">
                        <label>Upload Video (optional)</label>
                        <input className="form-input" name="video" type="file" accept="video/*" onChange={handleStageFileChange} />
                    </div>
                    <button className="btn btn-secondary" type="button" onClick={handleAddStage}>
                        Add Stage
                    </button>
                </div>
            )}
            <div className="stages-overview">
                <h3>Stages Overview</h3>
                <div className="overview-stats">
                    <div className="stat-item">
                        <span className="stat-value">{localStages.length}</span>
                        <span className="stat-label">Total Stages</span>
                    </div>
                    <div className="stat-item">
                        <span className="stat-value">{localStages.reduce((acc, s) => acc + (s.duration || 0), 0)}</span>
                        <span className="stat-label">Total Duration</span>
                    </div>
                    <div className="stat-item">
                        <span className="stat-value">{localStages.filter(s => s.videoFile || s.videoPath).length}</span>
                        <span className="stat-label">Videos Uploaded</span>
                    </div>
                </div>
                <div className="stages-timeline">
                    {localStages.length > 0 ? localStages.map((stage, idx) => (
                        <div key={stage.id} className="timeline-item" style={{ cursor: 'pointer', position: 'relative' }} onClick={() => handleEditStage(stage)}>
                            <div className="timeline-number">{idx + 1}</div>
                            <div className="timeline-content">
                                <div className="timeline-header">
                                    <h4>{stage.name}</h4>
                                    <button
                                        className="btn btn-danger btn-sm"
                                        style={{ marginLeft: 8, position: 'absolute', top: 16, right: 16, zIndex: 2 }}
                                        onClick={e => { e.stopPropagation(); handleRemoveStage(stage.id); }}
                                        title="Remove stage"
                                    >
                                        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                            <path d="M3 6h18" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
                                            <path d="M8 6V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
                                            <path d="M19 6l-1 14a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2L5 6" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
                                            <path d="M10 11v6" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
                                            <path d="M14 11v6" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
                                        </svg>
                                    </button>
                                </div>
                                <div className="timeline-details">
                                    <span className="timeline-duration">{stage.duration} min</span>
                                    <span className="timeline-videos">{stage.videoFile || stage.videoPath ? 1 : 0} videos</span>
                                    {stage.description && <p className="timeline-description" dangerouslySetInnerHTML={{ __html: stage.description }} />}
                                </div>
                            </div>
                        </div>
                    )) : (
                        <div className="empty-stages">
                            <span>No stages added yet</span>
                            <p>Add your first stage to start building your course</p>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );

    const renderSummaryTab = () => (
        <div className="summary-section">
            <h3>Course Summary</h3>
            <div className="summary-content">
                <div className="summary-group">
                    <h4>Basic Information</h4>
                    <div className="summary-item"><span>Course Name:</span><span>{editForm?.name || 'Not set'}</span></div>
                    <div className="summary-item"><span>Description:</span><span dangerouslySetInnerHTML={{ __html: editForm?.description || 'Not set' }} /></div>
                    <div className="summary-item"><span>Duration:</span><span>{editForm?.duration} minutes</span></div>
                    <div className="summary-item"><span>Price:</span><span>${editForm?.price}</span></div>
                    <div className="summary-item"><span>Difficulty:</span><span>{editForm?.difficulty === 1 ? 'Beginner' : editForm?.difficulty === 2 ? 'Intermediate' : 'Advanced'}</span></div>
                </div>
                <div className="summary-group">
                    <h4>Stages Overview</h4>
                    <div className="summary-item"><span>Total Stages:</span><span>{localStages.length}</span></div>
                    <div className="summary-item"><span>Total Duration:</span><span>{localStages.reduce((acc, s) => acc + (s.duration || 0), 0)} minutes</span></div>
                </div>
                <div className="summary-group">
                    <h4>Stages List</h4>
                    {localStages.map((stage, idx) => (
                        <div key={stage.id} className="summary-stage-item">
                            <div className="summary-item"><span>Stage {idx + 1}:</span><span>{stage.name}</span></div>
                            <div className="summary-item"><span>Duration:</span><span>{stage.duration} minutes</span></div>
                            {stage.description && (
                                <div className="summary-item"><span>Description:</span><span dangerouslySetInnerHTML={{ __html: stage.description }} /></div>
                            )}
                            {stage.videoPath && (
                                <div className="summary-item">
                                    <span>Video:</span>
                                    <span>{stage.videoPath.split('/').pop()}</span>
                                </div>
                            )}
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );

    const ArrowIcon = ({ open }: { open: boolean }) => (
        <svg width="20" height="20" viewBox="0 0 20 20" style={{ display: 'inline-block', verticalAlign: 'middle', transition: 'transform 0.2s', transform: open ? 'rotate(0deg)' : 'rotate(-90deg)' }}>
            <polyline points="6 8 10 12 14 8" fill="none" stroke="#fff" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
        </svg>
    );

    return (
        <div className="page-layout">
            <Sidebar />
            <div style={{ flex: 1, display: 'flex', minHeight: '100vh' }}>
                <div style={{ width: coursesListOpen ? 320 : 48, background: '#181818', borderRight: '1px solid #222', padding: 24, transition: 'width 0.2s' }}>
                    <div style={{ display: 'flex', alignItems: 'center', marginBottom: 24, height: 32 }}>
                        <button
                            onClick={() => setCoursesListOpen(open => !open)}
                            style={{
                                background: 'none',
                                border: 'none',
                                color: '#fff',
                                cursor: 'pointer',
                                marginRight: 12,
                                padding: 0,
                                display: 'flex',
                                alignItems: 'center',
                            }}
                            title={coursesListOpen ? 'Hide courses list' : 'Show courses list'}
                        >
                            <ArrowIcon open={coursesListOpen} />
                        </button>
                        <h2
                            style={{
                                color: '#fff',
                                margin: 0,
                                opacity: coursesListOpen ? 1 : 0,
                                width: coursesListOpen ? 'auto' : 0,
                                transition: 'opacity 0.2s, width 0.2s',
                                overflow: 'hidden',
                                whiteSpace: 'nowrap',
                                pointerEvents: coursesListOpen ? 'auto' : 'none',
                            }}
                        >
                            Your Courses
                        </h2>
                    </div>
                    {coursesListOpen && (loading ? <p style={{ color: '#aaa' }}>Loading...</p> : (
                        <ul style={{ listStyle: 'none', padding: 0 }}>
                            {courses.map(course => (
                                <li key={course.id} style={{ marginBottom: 16 }}>
                                    <button
                                        style={{
                                            width: '100%',
                                            background: selectedCourse?.id === course.id ? '#232323' : '#222',
                                            color: '#fff',
                                            border: 'none',
                                            borderRadius: 8,
                                            padding: 12,
                                            textAlign: 'left',
                                            cursor: 'pointer',
                                            fontWeight: selectedCourse?.id === course.id ? 'bold' : 'normal',
                                            boxShadow: selectedCourse?.id === course.id ? '0 0 8px #3dc55f' : 'none',
                                            transition: 'background 0.2s, box-shadow 0.2s',
                                        }}
                                        onClick={() => setSelectedCourse(course)}
                                    >
                                        <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
                                            <img src={getCourseImageUrl(course.imageUrl || "")} alt={course.name} style={{ width: 48, height: 48, objectFit: 'cover', borderRadius: 8, border: '1px solid #333' }} />
                                            <div>
                                                <div style={{ fontSize: 16 }}>{course.name}</div>
                                                <div style={{ fontSize: 12, color: '#aaa' }}>
                                                    {selectedCourse?.id === course.id ? localStages.length : (courseStagesCountMap[course.id] ?? course.stagesCount)} stages
                                                </div>
                                            </div>
                                        </div>
                                    </button>
                                </li>
                            ))}
                        </ul>
                    ))}
                </div>
                <main className="main-content">
                    {!selectedCourse ? (
                        <div style={{ color: '#aaa', textAlign: 'center', marginTop: 80, fontSize: 24 }}>
                            Select a course to edit
                        </div>
                    ) : (
                        <div className="course-form">
                            <div className="form-title-container">
                                <h2 className="form-title">Edit Course</h2>
                            </div>
                            {renderTabs()}
                            {activeTab === 'details' && renderDetailsTab()}
                            {activeTab === 'stages' && renderStagesTab()}
                            {activeTab === 'summary' && renderSummaryTab()}
                            {errorMsg ? (
                                <div className="error-message">{errorMsg}</div>
                            ) : (
                                statusMsg && (
                                    <div className="submission-status" style={{ marginTop: 16, marginBottom: 0 }}>
                                        <div className="status-content">
                                            <div className="status-message">{statusMsg}</div>
                                            <div className="progress-bar">
                                                <div className="progress-fill" style={{ width: `${progress}%` }} />
                                            </div>
                                        </div>
                                    </div>
                                )
                            )}
                            <button className="btn btn-primary" style={{ marginTop: 32 }} type="button" onClick={handleSaveCourse} disabled={!isFormValid}>
                                Save Course
                            </button>
                        </div>
                    )}
                </main>
            </div>
        </div>
    );
};

export default CreatorCourses;