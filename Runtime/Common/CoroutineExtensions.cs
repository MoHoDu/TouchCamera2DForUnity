using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class CoroutineExtensions
{
    public static Task RunCoroutine(this MonoBehaviour mono, IEnumerator routine, CancellationToken token = default, Action<Coroutine> callback = null)
    {
        var tcs = new TaskCompletionSource<bool>();

        Coroutine co = mono.StartCoroutine(WrapCoroutine(routine, tcs, token));
        callback?.Invoke(co);

        return tcs.Task;
    }

    private static IEnumerator WrapCoroutine(IEnumerator routine, TaskCompletionSource<bool> tcs, CancellationToken token)
    {
        while (true)
        {
            if (token.IsCancellationRequested)
            {
                tcs.TrySetCanceled(token);
                yield break;
            }

            if (!routine.MoveNext())
                break;

            yield return routine.Current;
        }

        tcs.TrySetResult(true);
    }
}
