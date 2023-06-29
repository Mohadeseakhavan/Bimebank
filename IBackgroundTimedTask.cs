using System;
using System.Threading;
using System.Threading.Tasks;

namespace bimeh_back.Components.Services.BackgroundTimedTask
{
    public interface IBackgroundTimedTask
    {
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);

        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}