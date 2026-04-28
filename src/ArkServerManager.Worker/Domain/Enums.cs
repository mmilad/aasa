namespace ArkServerManager.Worker.Domain;

public enum ServerState
{
    Stopped = 0,
    Starting = 1,
    Running = 2,
    Stopping = 3,
    Updating = 4,
    Crashed = 5,
    Error = 6,
}

public enum JobType
{
    InstallOrUpdate = 0,
    Start = 1,
    Stop = 2,
    Restart = 3,
    Backup = 4,
}

public enum JobStatus
{
    Pending = 0,
    Running = 1,
    Done = 2,
    Failed = 3,
}
