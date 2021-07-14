using System.Collections.Generic;
using System.Threading;

namespace EPloy
{
    public struct Timer
    {
        public long Id { get; set; }
        public long Time { get; set; }
        public ETTaskCompletionSource tcs;
    }

    public class TimerModule : EPloyModule
    {
        private Dictionary<long, Timer> timers;
        /// <summary>
        /// key: time, value: timer id
        /// </summary>
        private MultiMap<long, long> timeId;
        private Queue<long> timeOutTime;
        private Queue<long> timeOutTimerIds;

        // 记录最小时间，不用每次都去MultiMap取第一个值
        private long minTime;
        private int recordId;

        public override void Awake()
        {
            recordId = 0;
            timers = new Dictionary<long, Timer>();
            timeId = new MultiMap<long, long>();
            timeOutTime = new Queue<long>();
            timeOutTimerIds = new Queue<long>();
        }

        public override void Update()
        {
            if (this.timeId.Count == 0)
            {
                return;
            }

            long timeNow = TimeHelper.Now();

            if (timeNow < this.minTime)
            {
                return;
            }

            foreach (KeyValuePair<long, List<long>> kv in this.timeId.GetDictionary())
            {
                long k = kv.Key;
                if (k > timeNow)
                {
                    minTime = k;
                    break;
                }
                this.timeOutTime.Enqueue(k);
            }

            while (this.timeOutTime.Count > 0)
            {
                long time = this.timeOutTime.Dequeue();
                foreach (long timerId in this.timeId[time])
                {
                    this.timeOutTimerIds.Enqueue(timerId);
                }
                this.timeId.Remove(time);
            }

            while (this.timeOutTimerIds.Count > 0)
            {
                long timerId = this.timeOutTimerIds.Dequeue();
                Timer timer;
                if (!this.timers.TryGetValue(timerId, out timer))
                {
                    continue;
                }
                this.timers.Remove(timerId);
                timer.tcs.SetResult();
            }
        }

        public override void OnDestroy()
        {
            timers.Clear();
            timeId.Clear();
            timeOutTime.Clear();
            timeOutTimerIds.Clear();
        }

        private void Remove(long id)
        {
            this.timers.Remove(id);
        }

        public ETTask WaitTillAsync(long tillTime, CancellationToken cancellationToken)
        {
            ETTaskCompletionSource tcs = new ETTaskCompletionSource();
            Timer timer = new Timer { Id = GenerateId(), Time = tillTime, tcs = tcs };
            this.timers[timer.Id] = timer;
            this.timeId.Add(timer.Time, timer.Id);
            if (timer.Time < this.minTime)
            {
                this.minTime = timer.Time;
            }
            cancellationToken.Register(() => { this.Remove(timer.Id); });
            return tcs.Task;
        }

        public ETTask WaitTillAsync(long tillTime)
        {
            ETTaskCompletionSource tcs = new ETTaskCompletionSource();
            Timer timer = new Timer { Id = GenerateId(), Time = tillTime, tcs = tcs };
            this.timers[timer.Id] = timer;
            this.timeId.Add(timer.Time, timer.Id);
            if (timer.Time < this.minTime)
            {
                this.minTime = timer.Time;
            }
            return tcs.Task;
        }

        public ETTask WaitAsync(long time, CancellationToken cancellationToken)
        {
            ETTaskCompletionSource tcs = new ETTaskCompletionSource();
            Timer timer = new Timer { Id = GenerateId(), Time = TimeHelper.Now() + time, tcs = tcs };
            this.timers[timer.Id] = timer;
            this.timeId.Add(timer.Time, timer.Id);
            if (timer.Time < this.minTime)
            {
                this.minTime = timer.Time;
            }
            cancellationToken.Register(() => { this.Remove(timer.Id); });
            return tcs.Task;
        }

        public ETTask WaitAsync(long time)
        {
            ETTaskCompletionSource tcs = new ETTaskCompletionSource();
            Timer timer = new Timer { Id = GenerateId(), Time = TimeHelper.Now() + time, tcs = tcs };
            this.timers[timer.Id] = timer;
            this.timeId.Add(timer.Time, timer.Id);
            if (timer.Time < this.minTime)
            {
                this.minTime = timer.Time;
            }
            return tcs.Task;
        }

        private int GenerateId()
        {
            while (true)
            {
                if (this.timeId.ContainsKey(recordId))
                {
                    recordId += 1;
                    continue;
                }
                return recordId;
            }
        }
    }
}