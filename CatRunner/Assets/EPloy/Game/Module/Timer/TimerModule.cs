using EPloy.Timer;
using System;
using System.Collections.Generic;
using System.Timers;


//高效定时器
public partial class TimerModule : IGameModule
{
    private Timer serTime;
    private static readonly string lockId = "lockId";
    private static readonly string lockTime = "lockTime";
    private static readonly string lockFrame = "lockFrame";
    private static readonly string lockDelID = "lockDelID";
    //private static readonly string lockDelteId = "lockDelteId";

    private int id;
    private Dictionary<int, TaskFlag> idDic = new Dictionary<int, TaskFlag>();
    private List<int> delIds = new List<int>();

    private DateTime startDateTime = new DateTime(1970, 1, 1, 0, 0, 0);
    private double nowTime;

    private List<TimeTask> tempTimeTaskList = new List<TimeTask>();
    private List<TimeTask> timeTaskList = new List<TimeTask>();
    private List<int> tempDelTimeList = new List<int>();


    private List<FrameTask> tempFrameTaskList = new List<FrameTask>();
    private List<FrameTask> frameTaskList = new List<FrameTask>();
    private List<int> tempDelFrameList = new List<int>();


    public void Awake()
    {
        OnDestroy();
    }

    public void Update()
    {
        TimeTaskTick();
        FrameTaskTick();

        RecDelTimeTask();
        RecDelFrameTask();
        RecDelId();
    }

    public void OnDestroy()
    {
        if (serTime != null) serTime.Stop();
        idDic.Clear();
        tempTimeTaskList.Clear();
        timeTaskList.Clear();
        tempFrameTaskList.Clear();
        frameTaskList.Clear();
        id = 0;
    }

    public void ResetTimer()
    {
        OnDestroy();
    }

    #region Tool
    public double GetMillisecondsTime()
    {
        return nowTime;
    }

    public DateTime GetLocalDateTime()
    {
        TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
        DateTime dt = TimeZoneInfo.ConvertTimeFromUtc(startDateTime.AddMilliseconds(nowTime), cstZone);
        return dt;
    }

    public int GetYear()
    {
        return GetLocalDateTime().Year;
    }
    public int GetMonth()
    {
        return GetLocalDateTime().Month;
    }
    public int GetDay()
    {
        return GetLocalDateTime().Day;
    }
    public int GetWeek()
    {
        return (int)GetLocalDateTime().DayOfWeek;
    }

    public string GetLocalTimeStr()
    {
        DateTime dt = GetLocalDateTime();
        string str = GetTimeStr(dt.Hour) + ":" + GetTimeStr(dt.Minute) + ":" + GetTimeStr(dt.Second);
        return str;
    }
    #endregion

    #region TimeTask
    private void TimeTaskTick()
    {
        if (tempTimeTaskList.Count > 0)
        {
            lock (lockTime)
            {
                for (int i = 0; i < tempTimeTaskList.Count; i++)
                {
                    AddTimeListItem(tempTimeTaskList[i]);
                }
                tempTimeTaskList.Clear();
            }
        }

        nowTime = GetUTCMilliseconds();
        for (int i = 0; i < timeTaskList.Count; i++)
        {
            TimeTask task = timeTaskList[i];
            if (nowTime.CompareTo(task.destTime) < 0) continue;
            else
            {
                try
                {
                    task.callBack?.Invoke(task.id);
                }
                catch (Exception e)
                {
                    Log.Error(string.Format("{0} {1}", e, task.callBack));
                }
            }

            if (task.count == 1)
            {
                DelTimer(task.id);
                ///*lock (lockTime) */deleteIds.Add(task.id);
            }
            else
            {
                if (task.count > 0)
                {
                    --task.count;
                }
                task.destTime += task.delay;
                timeTaskList[i] = task;
            }
        }
    }
    /// <summary>
    /// 以时间延迟
    /// </summary>
    public IDPack InTimer(double delay, Action<int> callBack, int count = 1, TimeUnit unit = TimeUnit.Secound)
    {
        switch (unit)
        {
            case TimeUnit.Millisecound:
                break;
            case TimeUnit.Secound:
                delay *= 1000;
                break;
            case TimeUnit.Minute:
                delay *= 1000 * 60;
                break;
            case TimeUnit.Hour:
                delay *= 1000 * 60 * 60;
                break;
            case TimeUnit.Day:
                delay *= 1000 * 60 * 60 * 24; // 最大支持 24天
                break;
        }
        nowTime = GetUTCMilliseconds();
        double destTime = nowTime + delay;

        int id = GetId();
        if (id == -1) return new IDPack(id, TaskType.TimeTask);

        idDic[id] = new TaskFlag(idDic[id], TaskType.TimeTask);
        TimeTask task = new TimeTask
        {
            id = id,
            delay = delay,
            destTime = destTime,
            callBack = callBack,
            count = count
        };
        lock (lockTime)
        {
            tempTimeTaskList.Add(task);
        }

        return new IDPack(id, TaskType.TimeTask);
    }
    /// <summary>
    /// 删除时间延迟回调
    /// </summary>
    public void DelTimer(int id)
    {
        lock (lockTime) tempDelTimeList.Add(id);
    }
    /// <summary>
    /// 覆盖时间延迟回调
    /// </summary>
    public bool ReplaceTimer(int id, double delay, Action<int> callBack, int count = 1, TimeUnit unit = TimeUnit.Secound)
    {
        switch (unit)
        {
            case TimeUnit.Millisecound:
                break;
            case TimeUnit.Secound:
                delay *= 1000;
                break;
            case TimeUnit.Minute:
                delay *= 1000 * 60;
                break;
            case TimeUnit.Hour:
                delay *= 1000 * 60 * 60;
                break;
            case TimeUnit.Day:
                delay *= 1000 * 60 * 60 * 24; // 最大支持 24天
                break;
        }
        nowTime = GetUTCMilliseconds();
        double destTime = nowTime + delay;
        TimeTask task = new TimeTask
        {
            id = id,
            delay = delay,
            destTime = destTime,
            callBack = callBack,
            count = count,
        };

        if (idDic.ContainsKey(id) && idDic[id].active)
        {
            timeTaskList[idDic[id].index] = task;
            return true;
        }
        else
        {
            for (int i = 0; i < tempTimeTaskList.Count; i++)
            {
                if (tempTimeTaskList[i].id == id)
                {
                    tempTimeTaskList[i] = task;
                    return true;
                }
            }
        }

        return false;
    }

    private void AddTimeListItem(TimeTask task)
    {
        TaskFlag flag = new TaskFlag
        {
            id = task.id,
            index = timeTaskList.Count,
            active = true
        };
        idDic[task.id] = flag;
        timeTaskList.Add(task);
    }
    private void RecDelTimeTask()
    {
        if (tempDelTimeList.Count > 0)
        {
            lock (lockTime)
            {
                for (int i = 0; i < tempDelTimeList.Count; i++)
                {
                    DealDeleteTimeTask(tempDelTimeList[i]);
                }

                tempDelTimeList.Clear();
            }
        }
    }

    private bool DealDeleteTimeTask(int id)
    {
        bool exit = false;

        if (idDic.ContainsKey(id) && idDic[id].active)
        {
            exit = true;
            RemoveTimeListItem(idDic[id].index);
        }

        if (!exit)
        {
            for (int i = 0; i < tempTimeTaskList.Count; i++)
            {
                if (tempTimeTaskList[i].id == id)
                {
                    exit = true;
                    RemoveListItem_TimeTask(tempTimeTaskList, i);
                    lock (lockDelID) delIds.Add(id);
                    //lock(lockTime) idDic.Remove(id);
                    break;
                }
            }
        }

        return exit;
    }
    private void RemoveTimeListItem(int index)
    {
        if (timeTaskList.Count == 0 && tempTimeTaskList.Count == 0) return;

        TimeTask task = timeTaskList[index];

        RemoveListItem_TimeTask(timeTaskList, index);

        // 更新因为删除操作，交换到 index 位置的元素 下标
        if (index < timeTaskList.Count)
        {
            TimeTask indexTask = timeTaskList[index];
            TaskFlag flag = new TaskFlag
            {
                id = indexTask.id,
                index = index,
                active = true
            };
            idDic[indexTask.id] = flag;
        }

        lock (lockDelID) delIds.Add(task.id);
        //lock(lockId) idDic.Remove(task.id);
    }
    private void RemoveListItem_TimeTask(List<TimeTask> list, int index)
    {
        int last = list.Count - 1;
        TimeTask temp = list[index];
        list[index] = list[last];
        list[last] = temp;
        list.RemoveAt(last);
    }
    #endregion

    #region FrameTask
    private void FrameTaskTick()
    {
        if (tempFrameTaskList.Count > 0)
        {
            lock (lockFrame)
            {
                for (int i = 0; i < tempFrameTaskList.Count; i++)
                {
                    AddFrameListItem(tempFrameTaskList[i]);
                }
                tempFrameTaskList.Clear();
            }
        }


        for (int i = 0; i < frameTaskList.Count; i++)
        {
            FrameTask task = frameTaskList[i];
            if (task.curFrame < task.delay)
            {
                task.curFrame += 1;
                frameTaskList[i] = task;
                continue;
            }
            else
            {
                try
                {
                    task.callBack?.Invoke(task.id);
                }
                catch (Exception e)
                {
                    Log.Error(string.Format("{0} task.id:  {1}", e, task.id));
                }
            }

            if (task.count == 1)
            {
                DelFrame(task.id);
                //deleteIds.Add(task.id);
            }
            else
            {
                task.curFrame = 0;
                if (task.count > 0)
                {
                    --task.count;
                }
                frameTaskList[i] = task;
            }
        }
    }

    /// <summary>
    /// 以帧延迟
    /// </summary>
    public IDPack InFrame(int delay, Action<int> callBack, int count = 1)
    {
        int id = GetId();
        if (id == -1) return new IDPack(id, TaskType.FrameTask);

        idDic[id] = new TaskFlag(idDic[id], TaskType.FrameTask);
        FrameTask task = new FrameTask
        {
            id = id,
            curFrame = 0,
            delay = delay,
            callBack = callBack,
            count = count
        };
        lock (lockFrame)
        {
            tempFrameTaskList.Add(task);
        }

        return new IDPack(id, TaskType.FrameTask);
    }
    /// <summary>
    /// 删除帧延迟回调
    /// </summary>
    /// <param name="id"></param>
    public void DelFrame(int id)
    {
        lock (lockFrame) tempDelFrameList.Add(id);
    }
    /// <summary>
    /// 覆盖帧延迟回调
    /// </summary>
    /// <param name="id"></param>
    public bool ReplaceFrame(int id, int delay, Action<int> callBack, int count = 1)
    {
        FrameTask task = new FrameTask
        {
            id = id,
            curFrame = 0,
            delay = delay,
            callBack = callBack,
            count = count
        };

        if (idDic.ContainsKey(id) && idDic[id].active)
        {
            frameTaskList[idDic[id].index] = task;
            return true;
        }
        else
        {
            for (int i = 0; i < tempFrameTaskList.Count; i++)
            {
                if (tempFrameTaskList[i].id == id)
                {
                    tempFrameTaskList[i] = task;
                    return true;
                }
            }
        }

        return false;
    }

    private void AddFrameListItem(FrameTask task)
    {
        TaskFlag flag = new TaskFlag
        {
            id = task.id,
            index = frameTaskList.Count,
            active = true
        };
        idDic[task.id] = flag;
        frameTaskList.Add(task);
    }
    private void RecDelFrameTask()
    {
        if (tempDelFrameList.Count > 0)
        {
            lock (lockFrame)
            {
                for (int i = 0; i < tempDelFrameList.Count; i++)
                {
                    DealDeleteFrameTask(tempDelFrameList[i]);
                }
                tempDelFrameList.Clear();
            }
        }
    }
    private bool DealDeleteFrameTask(int id)
    {
        bool exit = false;
        if (idDic.ContainsKey(id) && idDic[id].active)
        {
            exit = true;
            RemoveFrameListItem(idDic[id].index);
        }

        if (!exit)
        {
            for (int i = 0; i < tempFrameTaskList.Count; i++)
            {
                if (tempFrameTaskList[i].id == id)
                {
                    exit = true;
                    RemoveListItem_FrameTask(tempFrameTaskList, i);
                    lock (delIds) delIds.Add(id);
                    ///*lock (obj)*/ idDic.Remove(id);
                    break;
                }
            }
        }

        return exit;
    }
    private void RemoveFrameListItem(int index)
    {
        if (frameTaskList.Count == 0 && tempFrameTaskList.Count == 0) return;

        FrameTask task = frameTaskList[index];

        RemoveListItem_FrameTask(frameTaskList, index);
        if (index < frameTaskList.Count)
        {
            FrameTask indexTask = frameTaskList[index];
            TaskFlag flag = new TaskFlag
            {
                id = indexTask.id,
                index = index,
                active = true
            };
            idDic[indexTask.id] = flag;
        }

        lock (delIds) delIds.Add(task.id);
        //idDic.Remove(task.id);
    }
    private void RemoveListItem_FrameTask(List<FrameTask> list, int index)
    {
        int last = list.Count - 1;
        FrameTask temp = list[index];
        list[index] = list[last];
        list[last] = temp;
        list.RemoveAt(last);
    }
    #endregion

    #region Common
    private int GetId()
    {
        lock (lockId)
        {
            id += 1;

            int len = 0;
            while (true)
            {
                if (id == int.MaxValue) id = 0;

                if (idDic.ContainsKey(id)) id++;
                else break;

                len++;
                if (len == int.MaxValue)
                {
                    Log.Error("计时任务已满，无法添加任务");
                    return -1;
                }
            }

            TaskFlag flag = new TaskFlag
            {
                id = id,
                active = false
            };
            idDic.Add(id, flag);
        }

        return id;
    }
    private void RecDelId()
    {
        if (delIds.Count > 0)
        {
            lock (lockId)
            {
                for (int i = 0; i < delIds.Count; i++)
                {
                    idDic.Remove(delIds[i]);
                }
            }
            lock (lockDelID) delIds.Clear();
        }
    }

    private double GetUTCMilliseconds()
    {
        TimeSpan ts = DateTime.UtcNow - startDateTime;
        return ts.TotalMilliseconds;
    }

    private string GetTimeStr(int time)
    {
        if (time < 10) return "0" + time.ToString();
        else return time.ToString();
    }
    #endregion
}