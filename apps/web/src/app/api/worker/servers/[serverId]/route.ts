import { NextResponse } from "next/server";

import { getServer } from "@/lib/worker-client";

export async function GET(
  _req: Request,
  context: { params: Promise<{ serverId: string }> },
) {
  const { serverId } = await context.params;
  const server = await getServer(serverId);
  return NextResponse.json(server, { status: 200 });
}

