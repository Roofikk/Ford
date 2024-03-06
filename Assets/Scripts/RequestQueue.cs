using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueRequest
{
    public class RequestQueue
    {
        private Queue<ICommand> _queueRequests = new();
        private System.Threading.Tasks.Task _runQueue;

        public int Count { get {  return _queueRequests.Count; } }
        public bool BreakOnFailedRequest = false;

        public RequestQueue(bool breakOnFailedRequest)
        {
            BreakOnFailedRequest = breakOnFailedRequest;
        }

        public int PushToRun<T>(Pair<T> systemRequest)
        {
            _queueRequests.Enqueue(systemRequest);

            if (_queueRequests.Count == 1)
            {
                WhileNotEmpty();
            }

            return _queueRequests.Count;
        }

        public void Stop()
        {
            _runQueue.Dispose();
            _queueRequests.Clear();
        }

        private void WhileNotEmpty()
        {
            _runQueue = System.Threading.Tasks.Task.Run(async () =>
            {
                while (_queueRequests.Count > 0)
                {
                    var command = _queueRequests.Peek();
                    System.Threading.Tasks.Task task = command.Execute();
                    await task;

                    if (!task.IsCompletedSuccessfully && BreakOnFailedRequest)
                    {
                        _queueRequests.Clear();
                        throw new Exception("The command failed with an error");
                    }

                    _queueRequests.Dequeue();
                } 
            });
        }
    }

    public class Pair<T> : ICommand
    {
        private readonly Task<T> _request;
        private readonly Action<T> _response;

        public Task<T> Request { get { return _request; } }
        public Action<T> Response { get { return _response; } }

        public Pair(Task<T> request, Action<T> response)
        {
            _request = request;
            _response = response;
        }

        public async System.Threading.Tasks.Task Execute()
        {
            await Request;
            Response?.Invoke(Request.Result);
        }
    }

    public interface ICommand
    {
        public System.Threading.Tasks.Task Execute();
    }
}