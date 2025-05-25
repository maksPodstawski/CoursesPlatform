import { useEffect, useState } from 'react';
import {config} from "../config.ts";

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
}

const CreatorCourses = () => {
    const [courses, setCourses] = useState<CreatorCourse[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchCreatorCourses = async () => {
            try {
                const response = await fetch(`${config.apiBaseUrl}${config.apiEndpoints.getCreatorCourses}`, {
                    credentials: 'include'
                });
                const data = await response.json();
                setCourses(data);
            } catch (error) {
                console.error('Error:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchCreatorCourses();
    }, []);

    return (
        <div>
            <h1>Creating Courses</h1>
            {loading ? <p>Loading...</p> : (
                <ul>
                    {courses.map(course => (
                        <li key={course.id}>
                            <h2>{course.name}</h2>
                            <img src={course.imageUrl} style={{width: "200px", height: "200px", objectFit: "cover"}}
                                 alt={course.name}/>
                            <p>{course.description}</p>
                            <p><strong>Price:</strong> {course.price} $</p>
                            <p><strong>Stages:</strong> {course.stagesCount}</p>
                            <p><strong>Reviews:</strong> {course.reviewsCount}</p>
                            <p><strong>Subcategories:</strong> {course.subcategories.join(', ')}</p>
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
};

export default CreatorCourses;