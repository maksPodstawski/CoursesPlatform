import React from "react";
import { Link } from "react-router-dom";

const Footer: React.FC = () => {
  return (
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
  );
};

export default Footer;
