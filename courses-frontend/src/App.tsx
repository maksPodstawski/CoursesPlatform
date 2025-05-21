import { BrowserRouter as Router } from "react-router-dom";
import { useState } from "react";

import NavBar from "./components/NavBar";
import Footer from "./components/Footer"; 
import ScrollToTopButton from "./components/ScrollToTopButton";
import AnimatedRoutes from "./routes/AnimatedRoutes";

import "./App.css";

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
        <NavBar
          isLoggedIn={isLoggedIn}
          onLogout={handleLogout}
          sidebarOpen={sidebarOpen}
          toggleSidebar={toggleSidebar}
        />  

        <main>
          <AnimatedRoutes />
          <ScrollToTopButton />
        </main>

        
        <Footer />
      </div>
    </Router>
  );
}

export default App;
