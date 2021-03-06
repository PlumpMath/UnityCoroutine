using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class MyMonoBehaviour : MonoBehaviour {
    private enum CoroutineState
    {
        Running,
        Paused,
        Finished
    }
    private class ManagedCoroutine
    {
        private IEnumerator it { get; set; }
        private int pausedSince { get; set; }
        private int delay { get; set; }

        private bool isPaused
        {
            get
            {
                return (Environment.TickCount - pausedSince) < delay;
            }
        }

        public ManagedCoroutine(IEnumerator it)
        {
            this.it = it;
        }

        public void Pause(float delay)
        {
            this.pausedSince = Environment.TickCount;
            this.delay = (int)(delay * 1000);
        }
        public CoroutineState Step(out object data)
        {
            data = null;

            if (isPaused)
                return CoroutineState.Paused;

            var isResumable = it.MoveNext();
            if (isResumable)
                data = it.Current;
            
            return isResumable ?
                CoroutineState.Running :
                CoroutineState.Finished;
        }
    }

    private List<ManagedCoroutine> coroutines { get; set; }

    public MyMonoBehaviour()
    {
        coroutines = new List<ManagedCoroutine>();
    }

    public void MyStartCoroutine(IEnumerator it)
    {
        coroutines.Add(new ManagedCoroutine(it));
    }
    public void MyStopAllCoroutines()
    {
        coroutines.Clear();
    }

    void Update () {
        // 순회 중 추가/제거 문제를 방지하기 위해 복사
        // 복사는 linq를 이용해 대충
        var copy = coroutines.ToList();

        foreach (var coro in copy)
        {
            object data;
            var state = coro.Step(out data);

            switch (state)
            {
                case CoroutineState.Running:
                    var waitForSeconds = data as MyWaitForSeconds;
                    if (waitForSeconds != null)
                        coro.Pause(waitForSeconds.delay);
                    break;
                case CoroutineState.Finished:
                    coroutines.Remove(coro);
                    break;
            }
        }
    }
}
