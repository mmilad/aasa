export enum ServerState {
  Stopped = 0,
  Starting = 1,
  Running = 2,
  Stopping = 3,
  Updating = 4,
  Crashed = 5,
  Error = 6,
}

export enum JobType {
  InstallOrUpdate = 0,
  Start = 1,
  Stop = 2,
  Restart = 3,
  Backup = 4,
}

export enum JobStatus {
  Pending = 0,
  Running = 1,
  Done = 2,
  Failed = 3,
}

export type ApiErrorEnvelope = {
  error: {
    code: string;
    message: string;
    detail?: unknown;
  };
};

export type ErrorEnvelope = ApiErrorEnvelope;

export type ServerSummaryResponse = {
  id: string;
  name: string;
  state: ServerState;
  gamePort: number;
  queryPort: number;
  rconPort: number;
};

export type ServerListResponse = {
  items: ServerSummaryResponse[];
};

export type CreateServerRequest = {
  name: string;
  mapName: string;
  sessionName: string;
  gamePort?: number | null;
  queryPort?: number | null;
  rconPort?: number | null;
  rconPassword?: string | null;
};

export type ServerResponse = {
  id: string;
  name: string;
  createdAtUtc: string;
  updatedAtUtc: string;
  state: ServerState;
  installRoot: string;
  gamePort: number;
  queryPort: number;
  rconPort: number;
  mapName: string;
  sessionName: string;
  pid?: number | null;
  lastJobId?: string | null;
};

export type JobAcceptedResponse = {
  jobId: string;
};

export type JobResponse = {
  id: string;
  serverId: string;
  type: JobType;
  status: JobStatus;
  createdAtUtc: string;
  startedAtUtc?: string | null;
  finishedAtUtc?: string | null;
  exitCode?: number | null;
  logBlob?: string | null;
};

