import { NextResponse } from "next/server";

import { enqueueStop } from "@/lib/worker-client";

export async function POST(
  _req: Request,
  context: { params: Promise<{ serverId: string }> },
) {
  const { serverId } = await context.params;
  const result = await enqueueStop(serverId);
  return NextResponse.json(result, { status: 202 });
}

