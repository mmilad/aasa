"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";

import { Alert } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

import type { CreateServerRequest } from "@/lib/worker-types";

export default function NewServerPage() {
  const router = useRouter();

  const [name, setName] = useState("ASA Server");
  const [mapName, setMapName] = useState("TheIsland_WP");
  const [sessionName, setSessionName] = useState("My Session");
  const [gamePort, setGamePort] = useState<string>("7777");
  const [queryPort, setQueryPort] = useState<string>("27015");
  const [rconPort, setRconPort] = useState<string>("27020");
  const [rconPassword, setRconPassword] = useState<string>("secret");

  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  function parseOptionalInt(value: string): number | null {
    const trimmed = value.trim();
    if (!trimmed) return null;
    const parsed = Number(trimmed);
    if (!Number.isFinite(parsed)) return null;
    return parsed;
  }

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    setSubmitting(true);
    setError(null);

    try {
      const payload: CreateServerRequest = {
        name,
        mapName,
        sessionName,
        gamePort: parseOptionalInt(gamePort),
        queryPort: parseOptionalInt(queryPort),
        rconPort: parseOptionalInt(rconPort),
        rconPassword: rconPassword.trim() ? rconPassword : null,
      };

      const res = await fetch("/api/worker/servers", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });

      if (!res.ok) {
        const text = await res.text();
        throw new Error(text || `Create server failed with ${res.status}`);
      }

      const created = await res.json();
      router.push(`/servers/${created.id}`);
    } catch (err) {
      setError(err instanceof Error ? err.message : String(err));
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="min-h-screen bg-zinc-50 p-6">
      <div className="mx-auto max-w-2xl">
        <div className="mb-6">
          <div className="text-xl font-semibold">Create server</div>
          <div className="text-sm text-zinc-600">Creates a server entry in Worker and prepares the folder.</div>
        </div>

        <Card>
          <CardHeader>
            <CardTitle>Server details</CardTitle>
            <CardDescription>These values are sent to `POST /api/v1/servers`.</CardDescription>
          </CardHeader>
          <CardContent>
            {error ? (
              <Alert variant="destructive" className="mb-4">
                {error}
              </Alert>
            ) : null}

            <form onSubmit={onSubmit} className="grid grid-cols-1 gap-4">
              <div className="grid gap-2">
                <Label htmlFor="name">Name</Label>
                <Input id="name" value={name} onChange={(e) => setName(e.target.value)} required />
              </div>

              <div className="grid gap-2">
                <Label htmlFor="mapName">Map</Label>
                <Input id="mapName" value={mapName} onChange={(e) => setMapName(e.target.value)} required />
              </div>

              <div className="grid gap-2">
                <Label htmlFor="sessionName">Session name</Label>
                <Input
                  id="sessionName"
                  value={sessionName}
                  onChange={(e) => setSessionName(e.target.value)}
                  required
                />
              </div>

              <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
                <div className="grid gap-2">
                  <Label htmlFor="gamePort">Game port</Label>
                  <Input id="gamePort" value={gamePort} onChange={(e) => setGamePort(e.target.value)} />
                </div>
                <div className="grid gap-2">
                  <Label htmlFor="queryPort">Query port</Label>
                  <Input id="queryPort" value={queryPort} onChange={(e) => setQueryPort(e.target.value)} />
                </div>
                <div className="grid gap-2">
                  <Label htmlFor="rconPort">RCON port</Label>
                  <Input id="rconPort" value={rconPort} onChange={(e) => setRconPort(e.target.value)} />
                </div>
              </div>

              <div className="grid gap-2">
                <Label htmlFor="rconPassword">RCON password (optional)</Label>
                <Input
                  id="rconPassword"
                  value={rconPassword}
                  onChange={(e) => setRconPassword(e.target.value)}
                  placeholder="(leave blank for none)"
                />
              </div>

              <div className="flex items-center justify-end gap-3 pt-2">
                <Button variant="secondary" type="button" onClick={() => router.push("/")}>
                  Cancel
                </Button>
                <Button type="submit" disabled={submitting}>
                  {submitting ? "Creating..." : "Create server"}
                </Button>
              </div>
            </form>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}

