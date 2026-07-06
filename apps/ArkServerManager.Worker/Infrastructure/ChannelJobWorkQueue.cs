using System.Threading.Channels;

namespace ArkServerManager.Worker.Infrastructure;

public interface IJobWorkQueue
{
    ValueTask EnqueueAsync(Guid jobId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Hands job ids to <see cref="JobProcessorHostedService"/> for out-of-band execution after HTTP returns 202.
/// </summary>
public sealed class ChannelJobWorkQueue : IJobWorkQueue
{
    private readonly Channel<Guid> _channel = Channel.CreateUnbounded<Guid>(new UnboundedChannelOptions
    {
        SingleReader = true,
        SingleWriter = false,
    });

    internal ChannelReader<Guid> Reader => _channel.Reader;

    public ValueTask EnqueueAsync(Guid jobId, CancellationToken cancellationToken = default) =>
        _channel.Writer.WriteAsync(jobId, cancellationToken);
}
