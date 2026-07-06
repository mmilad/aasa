import { NextResponse } from "next/server";

import { enqueueRestart } from "@/lib/worker-client";

export async function POST(
  _req: Request,
  context: { params: Promise<{ serverId: string }> },
) {
  const { serverId } = await context.params;
  const result = await enqueueRestart(serverId);
  return NextResponse.json(result, { status: 202 });
}

