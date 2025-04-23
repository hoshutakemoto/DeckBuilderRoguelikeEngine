using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

public sealed class CommandManager : ICommandManager
{
    private readonly ICommandDispatcher _dispatcher;
    private readonly ICommandRecorder<ICommand> _history;
    private readonly ConcurrentQueue<ICommand> _commandQueue = new();
    private readonly SemaphoreSlim _queueLock = new(1, 1);

    public int HistoryCount => _history.Count;

    public CommandManager(ICommandDispatcher dispatcher,
        ICommandRecorder<ICommand> history)
    {
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _history = history ?? throw new ArgumentNullException(nameof(history));
    }

    public async Task ExecuteAsync(ICommand command)
    {
        var isValid = await command.ValidateWithAsync(_dispatcher);
        if (!isValid)
        {
            throw new InvalidOperationException($"Command validation failed: {command.CommandName}");
        }

        try
        {
            await command.ExecuteWithAsync(_dispatcher);
            _history.Push(command);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Command execution failed: {command.CommandName}", ex);
        }
    }

    public void QueueCommand(ICommand command)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));
        _commandQueue.Enqueue(command);
        ProcessQueueAsync().ConfigureAwait(false);
    }

    private async Task ProcessQueueAsync()
    {
        if (!await _queueLock.WaitAsync(0)) return;

        try
        {
            while (_commandQueue.TryDequeue(out var command))
            {
                await ExecuteAsync(command);
            }
        }
        finally
        {
            _queueLock.Release();
        }
    }

    public string SerializeHistory(ICommandSerializer serializer)
    {
        return serializer.Serialize(_history.GetHistoryInOrder());
    }
}