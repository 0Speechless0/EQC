using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EQC.Detection
{
    public class DownloadTaskDetection
    {
        public static HashSet<int> downloadTaskTag = new HashSet<int>();
        public static Dictionary<int, Task> userNextAction = new Dictionary<int, Task>();
        public static Queue<Task> taskQueue = new Queue<Task>();
        public static Task AddTaskQueneToRun(Action action, int taskTagId )
        {

            Task TaskRuningProcess =
                CreateTaskRuning(action, taskTagId);

            taskQueue.Enqueue(TaskRuningProcess);

            if (taskQueue.Count == 1)
            {
                TaskRuningProcess.Start();
            }
            if (!userNextAction.ContainsKey(taskTagId))
                userNextAction[taskTagId] = TaskRuningProcess;
            return TaskRuningProcess;


        }
        public static decimal getUserQueueWaitingPercent(int taskTagId)
        {
            decimal i = 0;
            if (!userNextAction.ContainsKey(taskTagId))
                return -1;
            foreach (var task in taskQueue)
            {
                if (task == userNextAction[taskTagId])
                    return decimal.Round( (1 -  i / ((decimal)taskQueue.Count() ) )* 150, 2);
                i++;
            }
            return -1;
        }
        public static Task CreateTaskRuning(Action action, int taskTagId)
        {
            return new Task(() => {
                Task nextTask = null;
                try
                {
                    downloadTaskTag.Add(taskTagId);
                    action.Invoke();
     
                }
                catch(Exception e)
                {

                }
                finally
                {
                    taskQueue.Dequeue();
                    if (taskQueue.Count > 0)
                        nextTask = taskQueue.Peek();

                    downloadTaskTag.Remove(taskTagId);
                    userNextAction.Remove(taskTagId);
                }

                if(nextTask != null)
                {
                    nextTask.Start();
                }
            });
        }
    }
}