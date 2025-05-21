import { motion } from "framer-motion";
import { useScrollVisibility } from "../utils/scroll";

const ScrollToTopButton = () => {
  const visible = useScrollVisibility(300);

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
};

export default ScrollToTopButton;
