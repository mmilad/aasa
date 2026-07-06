"use client";

import * as React from "react";

import { cn } from "@/lib/utils";

export type AlertVariant = "default" | "destructive";

export function Alert({
  className,
  variant = "default",
  ...props
}: React.HTMLAttributes<HTMLDivElement> & { variant?: AlertVariant }) {
  return (
    <div
      className={cn(
        "rounded-lg border border-zinc-200 bg-white p-4 text-sm text-zinc-900 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-50",
        variant === "destructive" &&
          "border-red-600/60 bg-red-50 text-red-900 dark:bg-red-950/40 dark:text-red-200",
        className,
      )}
      {...props}
    />
  );
}

