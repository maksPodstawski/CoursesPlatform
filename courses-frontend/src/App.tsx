import {
  BrowserRouter as Router,
  Routes,
  Route,
  Link,
  useLocation,
} from "react-router-dom";

import { useState, useEffect } from "react";
import { Login } from "./pages/Login";
import { Register } from "./pages/Register";
import Courses from "./pages/Courses";
import { getCourses } from "./services/courseService";
import { Menu } from "lucide-react";
import "./App.css";

import { AnimatePresence, motion } from "framer-motion";
import { useInView } from "react-intersection-observer";

// Page wrapper for transitions
function PageWrapper({ children }: { children: React.ReactNode }) {
  return (
    <motion.div
      initial={{ opacity: 0, x: -30 }}
      animate={{ opacity: 1, x: 0 }}
      exit={{ opacity: 0, x: 30 }}
      transition={{ duration: 0.4 }}
    >
      {children}
    </motion.div>
  );
}

function AnimatedSection({
  children,
  direction = "up",
}: {
  children: React.ReactNode;
  direction?: "up" | "down" | "left" | "right";
}) {
  const { ref, inView } = useInView({ triggerOnce: true, threshold: 0.3 });

  const variants = {
    hidden: {
      opacity: 0,
      y: direction === "up" ? 50 : direction === "down" ? -50 : 0,
      x: direction === "left" ? 50 : direction === "right" ? -50 : 0,
    },
    visible: {
      opacity: 1,
      y: 0,
      x: 0,
      transition: { duration: 0.6, ease: "easeOut" },
    },
  };

  return (
    <motion.div ref={ref} variants={variants} initial="hidden" animate={inView ? "visible" : "hidden"}>
      {children}
    </motion.div>
  );
}

function AnimatedRoutes() {
  const location = useLocation();

  return (
    <AnimatePresence mode="wait">
      <Routes location={location} key={location.pathname}>
        <Route
          path="/login"
          element={
            <PageWrapper>
              <Login onLoginSuccess={() => {}} />
            </PageWrapper>
          }
        />
        <Route
          path="/register"
          element={
            <PageWrapper>
              <Register />
            </PageWrapper>
          }
        />
        <Route
          path="/courses"
          element={
            <PageWrapper>
              <Courses />
            </PageWrapper>
          }
        />
        <Route
          path="/"
          element={
            <PageWrapper>
              <HomeContent />
            </PageWrapper>
          }
        />
      </Routes>
    </AnimatePresence>
  );
}

function HomeContent() {
  const [featuredCourses, setFeaturedCourses] = useState<any[]>([]);

  useEffect(() => {
    const fetchCourses = async () => {
      try {
        const data = await getCourses();
        setFeaturedCourses(data.slice(0, 3));
      } catch (error) {
        console.error("Error while downloading", error);
      }
    };
    fetchCourses();
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
          <Link to="/register" className="btn primary">Get Started</Link>
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
                <motion.div
                  key={course.id}
                  className="course-card"
                  whileHover={{ scale: 1.03 }}
                  transition={{ duration: 0.2 }}
                >
                  <img src={course.imageUrl} alt={course.name} className="course-image" />
                  <h3>{course.name}</h3>
                  <p>{course.description}</p>
                  <Link to="/courses" className="btn btn-primary">Explore</Link>
                </motion.div>
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
              <li>Modern platform with intuitive interface</li>
              <li>Track your progress</li>
              <li>Certified instructors</li>
              <li>24/7 support</li>
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
            <StatCard label="Users" value={4521} icon="👥" />
            <StatCard label="Courses" value={128} icon="🎓" />
            <StatCard label="Reviews" value={312} icon="🌟" />
            <StatCard label="Avg. Session (min)" value={35} icon="⏳" />
          </div>
        </section> 
      </AnimatedSection>
    </>
  );
}

function StatCard({ label, value, icon }: { label: string; value: number; icon: string }) {
  const [count, setCount] = useState(0);
  const { ref, inView } = useInView({ triggerOnce: true });

  useEffect(() => {
    if (!inView) return;

    let current = 0;
    const duration = 2000;
    const step = Math.ceil(value / (duration / 16));

    const interval = setInterval(() => {
      current += step;
      if (current >= value) {
        current = value;
        clearInterval(interval);
      }
      setCount(current);
    }, 16);

    return () => clearInterval(interval);
  }, [inView, value]);

  return (
    <motion.div
      ref={ref}
      className="stat-card"
      whileHover={{ scale: 1.05 }}
      transition={{ duration: 0.3 }}
    >
      <div className="stat-icon">{icon}</div>
      <div className="stat-value">{count}</div>
      <div className="stat-label">{label}</div>
    </motion.div>
  );
}

function ScrollToTopButton() {
  const [visible, setVisible] = useState(false);

  useEffect(() => {
    const handleScroll = () => {
      setVisible(window.scrollY > 300);
    };
    window.addEventListener("scroll", handleScroll);
    return () => window.removeEventListener("scroll", handleScroll);
  }, []);

  const scrollToTop = () => {
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  return (
    <motion.button
      onClick={scrollToTop}
      initial={{ opacity: 0 }}
      animate={{ opacity: visible ? 1 : 0 }}
      transition={{ duration: 0.3 }}
      className="scroll-top-btn"
      aria-label="Scroll to top"
    >
      ↑
    </motion.button>
  );
}

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const toggleSidebar = () => setSidebarOpen(prev => !prev);

  const handleLogout = () => {
    setIsLoggedIn(false);
    window.location.href = "/login";
  };

  return (
    <Router>
      <div className="app">
        <nav className="nav">
          <div className="nav-content">
            <Link to="/" className="logo">Courses Platform</Link>
            <div className={`nav-links ${sidebarOpen ? "hide-on-mobile" : ""}`}>
              <Link to="/courses">Courses</Link>

              {isLoggedIn ? (
                <button onClick={handleLogout} className="btn logout">Logout</button>
              ) : (
                <>
                  <Link to="/login">Login</Link>
                  <Link to="/register">Register</Link>
                </>
              )}
            </div>
            <button className="menu-toggle" onClick={toggleSidebar}>
              <Menu size={28} color="#e0e0e0" />
            </button>
          </div>

          <div className={`sidebar ${sidebarOpen ? "open" : ""}`}>
            <button className="close-btn" onClick={toggleSidebar}>×</button>
            <Link to="/" onClick={toggleSidebar}>Home</Link>
            <Link to="/courses" onClick={toggleSidebar}>Courses</Link>
            {isLoggedIn ? (
              <button onClick={() => { handleLogout(); toggleSidebar(); }} className="btn logout">Logout</button>
            ) : (
              <>
                <Link to="/login" onClick={toggleSidebar}>Login</Link>
                <Link to="/register" onClick={toggleSidebar}>Register</Link>
              </>
            )}
          </div>
        </nav>

        <main>
          <AnimatedRoutes />
          <ScrollToTopButton />
        </main>

        
        
        <footer className="footer">
          <div className="footer-columns">
            <div className="footer-column">
              <h4>Courses Platform</h4>
              <p>Empowering learners through modern education. Start your journey with us today.</p>
            </div>
            <div className="footer-column">
              <h4>Contact</h4>
              <p>Email: <a href="mailto:support@soursesplatform.com">support@coursesplatform.com</a></p>
              <p>Phone: +48 123 456 789</p>
              <p>Location: Warsaw, Poland</p>
            </div>
            <div className="footer-column">
              <h4>Explore</h4>
              <ul>
                <li><Link to="/courses">Recommended Courses</Link></li>
                <li><a href="#">FAQs</a></li>
                <li><a href="#">Community Forum</a></li>
                <li><a href="https://facebook.com">Facebook</a></li>
                <li><a href="https://twitter.com">Twitter</a></li>
              </ul>
            </div>
          </div>
          <p className="footer-bottom">&copy; {new Date().getFullYear()} Courses Platform. All rights reserved.</p>
        </footer>
      </div>
    </Router>
  );
}

export default App;
