"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";

import { Alert } from "@/components/ui/alert";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Textarea } from "@/components/ui/textarea";

import type { JobAcceptedResponse, JobResponse, ServerResponse, ServerState } from "@/lib/worker-types";
import { JobStatus } from "@/lib/worker-types";

function stateLabel(state: ServerState) {
  switch (state) {
    case 0:
      return "Stopped";
    case 1:
      return "Starting";
    case 2:
      return "Running";
    case 3:
      return "Stopping";
    case 4:
      return "Updating";
    case 5:
      return "Crashed";
    case 6:
      return "Error";
    default:
      return `Unknown (${state})`;
  }
}

export default function ServerDetailPage() {
  const router = useRouter();
  const params = useParams<{ serverId: string }>();
  const serverId = params.serverId;

  const [server, setServer] = useState<ServerResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [jobId, setJobId] = useState<string | null>(null);
  const [job, setJob] = useState<JobResponse | null>(null);
  const [jobError, setJobError] = useState<string | null>(null);

  async function refresh() {
    if (!serverId) return;
    setLoading(true);
    setError(null);
    try {
      const res = await fetch(`/api/worker/servers/${serverId}`);
      if (!res.ok) throw new Error(await res.text());
      const body = (await res.json()) as ServerResponse;
      setServer(body);
    } catch (e) {
      setError(e instanceof Error ? e.message : String(e));
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    refresh();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [serverId]);

  useEffect(() => {
    if (!jobId) return;
    let cancelled = false;
    let timer: ReturnType<typeof setInterval> | null = null;

    async function tick() {
      try {
        const res = await fetch(`/api/worker/jobs/${jobId}`);
        if (!res.ok) throw new Error(await res.text());
        const body = (await res.json()) as JobResponse;
        if (cancelled) return;
        setJob(body);

        if (body.status === JobStatus.Done || body.status === JobStatus.Failed) {
          if (timer) clearInterval(timer);
          setJobId(null);
          refresh();
        }
      } catch (e) {
        if (cancelled) return;
        setJobError(e instanceof Error ? e.message : String(e));
      }
    }

    timer = setInterval(tick, 2000);
    tick();

    return () => {
      cancelled = true;
      if (timer) clearInterval(timer);
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [jobId]);

  async function runAction(action: "install" | "start" | "stop" | "restart" | "backup") {
    if (!serverId) return;
    setJobError(null);
    setJob(null);

    try {
      let accepted: JobAcceptedResponse;
      if (action === "install") {
        const res = await fetch(`/api/worker/servers/${serverId}/jobs/install`, { method: "POST" });
        accepted = (await res.json()) as JobAcceptedResponse;
      } else if (action === "backup") {
        const res = await fetch(`/api/worker/servers/${serverId}/jobs/backup`, { method: "POST" });
        accepted = (await res.json()) as JobAcceptedResponse;
      } else if (action === "start") {
        const res = await fetch(`/api/worker/servers/${serverId}/actions/start`, { method: "POST" });
        accepted = (await res.json()) as JobAcceptedResponse;
      } else if (action === "stop") {
        const res = await fetch(`/api/worker/servers/${serverId}/actions/stop`, { method: "POST" });
        accepted = (await res.json()) as JobAcceptedResponse;
      } else if (action === "restart") {
        const res = await fetch(`/api/worker/servers/${serverId}/actions/restart`, { method: "POST" });
        accepted = (await res.json()) as JobAcceptedResponse;
      } else {
        throw new Error(`Unknown action ${action}`);
      }

      if (!accepted.jobId) throw new Error("Worker did not return jobId.");
      setJobId(accepted.jobId);
    } catch (e) {
      setJobError(e instanceof Error ? e.message : String(e));
    }
  }

  if (loading) {
    return (
      <div className="min-h-screen bg-zinc-50 p-6">
        <div className="mx-auto max-w-4xl text-sm text-zinc-600">Loading server...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen bg-zinc-50 p-6">
        <div className="mx-auto max-w-4xl">
          <Alert variant="destructive">{error}</Alert>
          <div className="mt-4">
            <Button variant="secondary" onClick={() => router.push("/")}>Back</Button>
          </div>
        </div>
      </div>
    );
  }

  if (!server) {
    return (
      <div className="min-h-screen bg-zinc-50 p-6">
        <div className="mx-auto max-w-4xl text-sm text-zinc-600">Server not found.</div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-zinc-50 p-6">
      <div className="mx-auto max-w-4xl space-y-4">
        <div className="flex items-start justify-between gap-3">
          <div>
            <div className="text-xl font-semibold">{server.name}</div>
            <div className="text-sm text-zinc-600 break-all">{server.id}</div>
          </div>
          <Button variant="secondary" onClick={() => router.push("/")}>
            Back
          </Button>
        </div>

        <Card>
          <CardHeader className="p-4">
            <div className="flex items-start justify-between gap-3">
              <div>
                <CardTitle className="text-base">Status</CardTitle>
                <CardDescription className="mt-1">
                  {stateLabel(server.state)} (PID: {server.pid ?? "n/a"})
                </CardDescription>
              </div>
              <Badge
                variant={
                  server.state === 2 ? "success" : server.state === 0 ? "secondary" : "warning"
                }
              >
                {stateLabel(server.state)}
              </Badge>
            </div>
          </CardHeader>
          <CardContent className="space-y-3">
            <div className="text-sm">
              Ports: game {server.gamePort} / query {server.queryPort} / rcon {server.rconPort}
            </div>
            <div className="text-sm break-all">
              Install root: <span className="font-mono">{server.installRoot}</span>
            </div>
            <div className="text-sm">
              Map: {server.mapName} / Session: {server.sessionName}
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="p-4">
            <CardTitle className="text-base">Actions</CardTitle>
            <CardDescription className="mt-1">Enqueue a job in the Worker and poll until completion.</CardDescription>
          </CardHeader>
          <CardContent className="flex flex-wrap gap-2">
            <Button variant="secondary" onClick={() => runAction("install")}>Install/Update</Button>
            <Button variant="secondary" onClick={() => runAction("start")}>Start</Button>
            <Button variant="secondary" onClick={() => runAction("stop")}>Stop</Button>
            <Button variant="secondary" onClick={() => runAction("restart")}>Restart</Button>
            <Button variant="secondary" onClick={() => runAction("backup")}>Backup</Button>
          </CardContent>

          {jobError ? (
            <div className="px-6 pb-6">
              <Alert variant="destructive">{jobError}</Alert>
            </div>
          ) : null}

          {job ? (
            <div className="px-6 pb-6 space-y-3">
              <div className="text-sm">
                Job {job.id} status:{" "}
                <span className="font-medium">{JobStatus[job.status] ?? job.status}</span>{" "}
                (exit code: {job.exitCode ?? "n/a"})
              </div>
              <Textarea
                readOnly
                value={(job.logBlob ?? "").slice(0, 20000)}
                className="min-h-[120px]"
              />
            </div>
          ) : jobId ? (
            <div className="px-6 pb-6 text-sm text-zinc-600">Polling job {jobId}...</div>
          ) : null}
        </Card>
      </div>
    </div>
  );
}

