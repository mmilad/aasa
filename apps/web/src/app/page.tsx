"use client";

import Link from "next/link";
import { useEffect, useMemo, useState } from "react";

import { Alert } from "@/components/ui/alert";
import { Badge } from "@/components/ui/badge";
import { Card, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import type { ServerListResponse, ServerSummaryResponse } from "@/lib/worker-types";
import { ServerState } from "@/lib/worker-types";

function stateLabel(state: ServerState) {
  switch (state) {
    case ServerState.Stopped:
      return "Stopped";
    case ServerState.Starting:
      return "Starting";
    case ServerState.Running:
      return "Running";
    case ServerState.Stopping:
      return "Stopping";
    case ServerState.Updating:
      return "Updating";
    case ServerState.Crashed:
      return "Crashed";
    case ServerState.Error:
      return "Error";
    default:
      return `Unknown (${state})`;
  }
}

export default function Home() {
  const [servers, setServers] = useState<ServerSummaryResponse[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let cancelled = false;

    async function load() {
      try {
        setLoading(true);
        setError(null);
        const res = await fetch("/api/worker/servers");
        if (!res.ok) {
          const text = await res.text();
          throw new Error(text || `Worker list servers failed with ${res.status}`);
        }

        const body = (await res.json()) as ServerListResponse;
        if (!cancelled) setServers(body.items ?? []);
      } catch (e) {
        if (!cancelled) setError(e instanceof Error ? e.message : String(e));
      } finally {
        if (!cancelled) setLoading(false);
      }
    }

    load();
    return () => {
      cancelled = true;
    };
  }, []);

  const content = useMemo(() => {
    if (loading) return <div className="text-sm text-zinc-600">Loading servers...</div>;
    if (error) {
      return (
        <Alert variant="destructive" className="mb-4">
          {error}
        </Alert>
      );
    }

    if (servers.length === 0) return <div className="text-sm text-zinc-600">No servers registered yet.</div>;

    return (
      <div className="space-y-3">
        {servers.map((s) => (
          <Link key={s.id} href={`/servers/${s.id}`} className="block">
            <Card className="hover:bg-zinc-50 transition-colors">
              <CardHeader className="p-4">
                <div className="flex items-start justify-between gap-3">
                  <div>
                    <CardTitle className="text-base">{s.name}</CardTitle>
                    <CardDescription className="mt-1">
                      Ports: game {s.gamePort} / query {s.queryPort} / rcon {s.rconPort}
                    </CardDescription>
                  </div>
                  <Badge variant={s.state === ServerState.Running ? "success" : s.state === ServerState.Stopped ? "secondary" : "warning"}>
                    {stateLabel(s.state)}
                  </Badge>
                </div>
              </CardHeader>
            </Card>
          </Link>
        ))}
      </div>
    );
  }, [error, loading, servers]);

  return (
    <div className="min-h-screen bg-zinc-50 p-6">
      <div className="mx-auto max-w-4xl">
        <div className="mb-6 flex items-center justify-between">
          <div>
            <div className="text-xl font-semibold">ArkServerManager Admin</div>
            <div className="text-sm text-zinc-600">Local operator panel (POC)</div>
          </div>
          <Link
            href="/servers/new"
            className="inline-flex items-center justify-center whitespace-nowrap rounded-md bg-zinc-900 px-4 py-2 text-sm font-medium text-zinc-50 hover:bg-zinc-900/90"
          >
            Create server
          </Link>
        </div>

        {content}
      </div>
    </div>
  );
}
