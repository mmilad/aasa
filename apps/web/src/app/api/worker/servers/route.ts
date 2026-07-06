import { NextResponse } from "next/server";

import { createServer, listServers } from "@/lib/worker-client";
import type { CreateServerRequest } from "@/lib/worker-types";

export async function GET() {
  const result = await listServers();
  return NextResponse.json(result, { status: 200 });
}

export async function POST(req: Request) {
  const body = (await req.json()) as CreateServerRequest;
  const created = await createServer(body);
  return NextResponse.json(created, { status: 201 });
}

