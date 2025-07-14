import { ReactNode } from "react";
import { motion } from "framer-motion";
import { useInView } from "react-intersection-observer";
import { useAnimatedCounter } from "../utils/counters"; 

const StatCard = ({ label, value, icon }: { label: string; value: number; icon: ReactNode }) => {
  const { ref, inView } = useInView({ triggerOnce: true });
  const count = useAnimatedCounter(inView ? value : 0);

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
};

export default StatCard;
