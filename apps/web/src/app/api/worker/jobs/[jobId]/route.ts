import { NextResponse } from "next/server";

import { getJob } from "@/lib/worker-client";

export async function GET(
  _req: Request,
  context: { params: Promise<{ jobId: string }> },
) {
  const { jobId } = await context.params;
  const job = await getJob(jobId);
  return NextResponse.json(job, { status: 200 });
}

