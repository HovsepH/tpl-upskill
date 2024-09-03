namespace Tpl;
#pragma warning disable
public static class StudentLogic
{
    public static Task TaskCreated()
    {
        return new Task(() => { });
    }

    public static Task WaitingForActivation()
    {
        return Foo(5);
    }

    public static Task WaitingToRun()
    {
        return Task.Factory.StartNew(() => { });
    }

    public static Task Running()
    {
        var tcs = new TaskCompletionSource<bool>();

        Task task = Task.Run(() =>
        {
            tcs.SetResult(true);

            Thread.Sleep(5000);
            Console.WriteLine("Task has completed work.");
        });

        tcs.Task.Wait();

        return task;
    }

    public static Task RanToCompletion()
    {
        var task = new Task(() => { });
        task.Start();
        task.Wait();
        return task;
    }

    public static Task WaitingForChildrenToComplete()
    {
        Task parent = Task.Factory.StartNew(() =>
        {
            Task child = Task.Factory.StartNew(() => Thread.Sleep(6000),
                TaskCreationOptions.AttachedToParent);
        });
        Thread.Sleep(2000);
        return parent;
    }

    public static Task IsCompleted()
    {
        var task = Task.Factory.StartNew(() => { });
        task.Wait();
        return task;
    }

    public static Task IsCancelled()
    {
        var tokenSource2 = new CancellationTokenSource();
        CancellationToken ct = tokenSource2.Token;

        var task = Task.Run(() =>
        {
            // Were we already canceled?
            ct.ThrowIfCancellationRequested();

            bool moreToDo = true;
            while (moreToDo)
            {
                // Poll on this property if you have to do
                // other cleanup before throwing.
                if (ct.IsCancellationRequested)
                {
                    // Clean up here, then...
                    ct.ThrowIfCancellationRequested();
                }

            }
        }, tokenSource2.Token); // Pass same token to Task.Run.

        tokenSource2.Cancel();
        Thread.Sleep(1000);
        return task;
    }

    public static Task IsFaulted()
    {
        var t = Task.Factory.StartNew(() => throw new InvalidCastException());
        Thread.Sleep(500);
        return t;
    }

    public static List<int> ForceParallelismPlinq()
    {
        var testList = Enumerable.Range(1, 300).ToList();
        var result = testList
             .AsParallel()
             .WithDegreeOfParallelism(5)
             .Select(n => n * n)
             .ToList();
        return result;
    }

    private static async Task<string> Foo(int seconds)
    {
        return await Task.Run(() =>
        {
            for (int i = 0; i < seconds; i++)
            {
                Task.Delay(TimeSpan.FromSeconds(1)).Wait();
            }

            return "Foo Completed";
        });
    }
}
