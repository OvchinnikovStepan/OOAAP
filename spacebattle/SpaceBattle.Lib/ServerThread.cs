namespace SpaceBattle.Lib;

using System.Collections.Concurrent;
using Hwdtech;

public class ServerThread
{
    private Action _behaviour;
    private readonly BlockingCollection<Hwdtech.ICommand> _queue;
    private readonly Thread _thread;
    private bool _stop = false;

    public ServerThread(BlockingCollection<Hwdtech.ICommand> queue)
    {
        _queue = queue;

        _behaviour = () =>
        {
            var cmd = _queue.Take();
            try
            {
                cmd.Execute();
            }
            catch (Exception e)
            {
                IoC.Resolve<Hwdtech.ICommand>("ExceptionHandler.Handle", cmd, e).Execute();
            }
        };

        _thread = new Thread(Loop);
    }

    private void Loop()
    {
        while (!_stop)
        {
            _behaviour();
        }
    }

    internal void Stop()
    {
        _stop = !_stop;
    }

    internal Action GetBehaviour()
    {
        return _behaviour;
    }

    internal void SetBehaviour(Action newBehaviour)
    {
        _behaviour = newBehaviour;
    }

    public void Start()
    {
        _thread.Start();
    }
    public bool IsNotEmpty()
    {
        return Convert.ToBoolean(_queue.Count());
    }
    public override bool Equals(object? obj)
    {
        return obj != null && obj is Thread thread && _thread == thread;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
