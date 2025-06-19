import { motion } from "framer-motion";
import { useInView } from "react-intersection-observer";
import React from "react";

export function PageWrapper({ children }: { children: React.ReactNode }) {
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

export function AnimatedSection({
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
      transition: { duration: 0.6, ease: "easeOut" as const },
    },
  };

  return (
    <motion.div ref={ref} variants={variants} initial="hidden" animate={inView ? "visible" : "hidden"}>
      {children}
    </motion.div>
  );
}
