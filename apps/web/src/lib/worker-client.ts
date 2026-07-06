import {
  CreateServerRequest,
  JobAcceptedResponse,
  JobResponse,
  ServerListResponse,
  ServerResponse,
} from "./worker-types";

const WORKER_BASE_URL = process.env.WORKER_BASE_URL ?? "http://127.0.0.1:5080/";
function getWorkerApiKey() {
  const key = process.env.WORKER_API_KEY;
  if (!key) throw new Error("Missing env var WORKER_API_KEY for Worker API access.");
  return key;
}

function urlFor(relativePath: string) {
  const base = WORKER_BASE_URL.endsWith("/") ? WORKER_BASE_URL : `${WORKER_BASE_URL}/`;
  const path = relativePath.startsWith("/") ? relativePath.slice(1) : relativePath;
  return `${base}${path}`;
}

async function workerFetch<T>(path: string, init: RequestInit): Promise<T> {
  const workerApiKey = getWorkerApiKey();
  const res = await fetch(urlFor(path), {
    ...init,
    headers: {
      "X-Api-Key": workerApiKey,
      ...(init.headers ?? {}),
    },
  });

  if (!res.ok) {
    const body = await res.text();
    throw new Error(`Worker API error ${res.status}: ${body}`);
  }

  return (await res.json()) as T;
}

export async function listServers() {
  return workerFetch<ServerListResponse>("api/v1/servers", { method: "GET" });
}

export async function createServer(request: CreateServerRequest) {
  return workerFetch<ServerResponse>("api/v1/servers", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(request),
  });
}

export async function getServer(serverId: string) {
  return workerFetch<ServerResponse>(`api/v1/servers/${serverId}`, { method: "GET" });
}

export async function enqueueInstallOrUpdate(serverId: string) {
  return workerFetch<JobAcceptedResponse>(`api/v1/servers/${serverId}/jobs/install`, {
    method: "POST",
  });
}

export async function enqueueStart(serverId: string) {
  return workerFetch<JobAcceptedResponse>(`api/v1/servers/${serverId}/actions/start`, {
    method: "POST",
  });
}

export async function enqueueStop(serverId: string) {
  return workerFetch<JobAcceptedResponse>(`api/v1/servers/${serverId}/actions/stop`, {
    method: "POST",
  });
}

export async function enqueueRestart(serverId: string) {
  return workerFetch<JobAcceptedResponse>(`api/v1/servers/${serverId}/actions/restart`, {
    method: "POST",
  });
}

export async function enqueueBackup(serverId: string) {
  return workerFetch<JobAcceptedResponse>(`api/v1/servers/${serverId}/jobs/backup`, {
    method: "POST",
  });
}

export async function getJob(jobId: string) {
  return workerFetch<JobResponse>(`api/v1/jobs/${jobId}`, { method: "GET" });
}

