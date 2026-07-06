import { NextResponse } from "next/server";

import { enqueueInstallOrUpdate } from "@/lib/worker-client";

export async function POST(
  _req: Request,
  context: { params: Promise<{ serverId: string }> },
) {
  const { serverId } = await context.params;
  const result = await enqueueInstallOrUpdate(serverId);
  return NextResponse.json(result, { status: 202 });
}

