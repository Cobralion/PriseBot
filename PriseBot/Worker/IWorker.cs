using PriseBot.Records;
using System;
using System.Threading.Tasks;

namespace PriseBot.Worker
{
    public interface IWorker : IDisposable
    {
        void Start();
        void Stop();
        void Configure(WorkerInformation workerInformation);

        bool IsRunning();
    }
}