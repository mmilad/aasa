import path from "path";
import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  // Monorepo: avoid inferring workspace root from a parent folder (e.g. sibling package-lock.json).
  turbopack: {
    root: path.join(__dirname, "..", ".."),
  },
};

export default nextConfig;
