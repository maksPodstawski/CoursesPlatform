import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { Users, GraduationCap, Star, Check } from "lucide-react";

import { getCourses, getCourseInstructor, getCourseParticipantsCount } from "../services/courseService";
import { getRatingSummary } from "../services/reviewService";
import { getStatistics } from "../services/statisticsService";
import { AnimatedSection } from "../utils/animations";
import StatCard from "../components/StatCard";
import { CourseCard } from "../components/CourseCard";
import { getDifficultyLabel } from "../utils/difficulty";


const HomeContent = () => {
  const [featuredCourses, setFeaturedCourses] = useState<any[]>([]);
  const [ratings, setRatings] = useState<Record<string, string>>({});
  const { isLoggedIn } = useAuth();
  const [stats, setStats] = useState<{ usersCount: number; coursesCount: number; reviewsCount: number } | null>(null);
  const [statsLoading, setStatsLoading] = useState(true);
  const [statsError, setStatsError] = useState<string | null>(null);

  useEffect(() => {
    const fetchCourses = async () => {
      try {
        const data = await getCourses();
        const topCourses = data.slice(0, 3);
        
        const enhancedData = await Promise.all(
          topCourses.map(async (course: any) => {
            const instructor = await getCourseInstructor(course.id);
            const studentsCount = await getCourseParticipantsCount(course.id);
            const level = await getDifficultyLabel(course.difficulty);
            return {
              ...course,
              instructor: instructor.name || "N/A",
              duration: course.duration || "8 hours",
              studentsCount: studentsCount,
              level: level,
              category: course.category || "Programming"
            };
          })
        );
        
        setFeaturedCourses(enhancedData);
      } catch (error) {
        console.error("Error while downloading", error);
      }
    };
    fetchCourses();
  }, []);

  useEffect(() => {
    const fetchAllRatings = async () => {
      const newRatings: Record<string, string> = {};
      for (const course of featuredCourses) {
        try {
          const summary = await getRatingSummary(course.id);
          if (summary && typeof summary.averageRating === 'number') {
            newRatings[course.id] = summary.averageRating.toFixed(1);
          }
        } catch (error) {
          console.error(`Error fetching rating for course ${course.id}:`, error);
        }
      }
      setRatings(newRatings);
    };

    if (featuredCourses.length > 0) {
      fetchAllRatings();
    }
  }, [featuredCourses]);

  useEffect(() => {
    getStatistics()
      .then(setStats)
      .catch((err) => {
        setStatsError("Failed to load statistics");
        console.error("Statistics fetch error:", err);
      })
      .finally(() => setStatsLoading(false));
  }, []);

  return (
      <>
        <header className="hero">
          <div className="hero-overlay">
            <h1>Empower Your Future with our WebSide</h1>
            <p>
              Discover a world of learning designed for real-world success. From coding to creative arts,
              our expert-led courses help you build the skills that matter — at your pace, on your terms.
            </p>
            {isLoggedIn ? (
              <Link to="/courses" className="btn primary">Go to courses</Link>
            ) : (
              <Link to="/register" className="btn primary">Get Started</Link>
            )}
          </div>
        </header>

        <AnimatedSection direction="up">
          <section className="section featured">
            <h2>Featured Courses</h2>
            <div className="courses-grid">
              {featuredCourses.length === 0 ? (
                  <p>No courses found.</p>
              ) : (
                  featuredCourses.map(course => (
                    <CourseCard 
                    key={course.id} 
                    course={course} 
                    rating={ratings[course.id]}
                    actions={
                        <Link to={`/courses/${course.id}`} className="btn btn-primary">
                          View Course
                        </Link>
                      }

                      />
                  ))
              )}
            </div>
          </section>
        </AnimatedSection>

        <AnimatedSection direction="right">
          <section className="about-section-split">
            <div className="about-left">
              <h2>Why Courses Platform?</h2>
              <p className="about-description">
                Whether you're a beginner or a pro, Courses Platform offers a learning experience that's personalized,
                accessible, and deeply practical. We blend theory with real-world application so you can learn effectively.
              </p>
              <ul className="about-features">
                <li><Check size={18} style={{marginRight: 8}} />Modern platform with intuitive interface</li>
                <li><Check size={18} style={{marginRight: 8}} />Track your progress</li>
                <li><Check size={18} style={{marginRight: 8}} />Certified instructors</li>
                <li><Check size={18} style={{marginRight: 8}} />24/7 support</li>
              </ul>
            </div>

            <div className="about-right">
              <h2>Join Our Community</h2>
              <p>Stay updated with new courses, tips, and exclusive content. Be part of a growing learning network!</p>
              <form className="subscribe-form">
                <input type="email" placeholder="Enter your email" required />
                <button type="submit">Join Now</button>
              </form>
            </div>
          </section>
        </AnimatedSection>

        <AnimatedSection direction="left">
          <section className="stats-section">
            <h2 className="stats-heading">Our Achievements</h2>
            <div className="stats-grid">
              {statsLoading && <div>Loading...</div>}
              {statsError && <div style={{ color: 'red' }}>{statsError}</div>}
              {stats && (
                <>
                  <StatCard label="Users" value={stats.usersCount} icon={<Users className="icon-users" />} />
                  <StatCard label="Courses" value={stats.coursesCount} icon={<GraduationCap className="icon-courses" />} />
                  <StatCard label="Reviews" value={stats.reviewsCount} icon={<Star className="icon-reviews" />} />
                </>
              )}
            </div>
          </section>
        </AnimatedSection>
      </>
  );
};

export default HomeContent;