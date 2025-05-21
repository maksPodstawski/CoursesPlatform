import { useEffect, useState } from "react";

export function useAnimatedCounter(value: number, duration: number = 2000) {
  const [count, setCount] = useState(0);

  useEffect(() => {
    let current = 0;
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
  }, [value, duration]);

  return count;
}
