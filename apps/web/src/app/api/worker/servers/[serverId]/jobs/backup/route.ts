import { NextResponse } from "next/server";

import { enqueueBackup } from "@/lib/worker-client";

export async function POST(
  _req: Request,
  context: { params: Promise<{ serverId: string }> },
) {
  const { serverId } = await context.params;
  const result = await enqueueBackup(serverId);
  return NextResponse.json(result, { status: 202 });
}

