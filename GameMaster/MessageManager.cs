using GameArea;
using GameMasterMain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameMaster
{
    public class MessageManager
    {
        private static readonly object normalLock = new object();
        private static readonly object prioritisedLock = new object();
        Dictionary<string, List<IGMMessage>> Queue;

        List<IGMMessage> PrioritisedQueue;

        Dictionary<string, Task> QueueTasks;

        Task PrioritisedTask;

        GameMasterController gameMasterController;

        public MessageManager(GameMasterController gmController)
        {
            Queue = new Dictionary<string, List<IGMMessage>>();
            PrioritisedQueue = new List<IGMMessage>();
            QueueTasks = new Dictionary<string, Task>();
            gameMasterController = gmController;
        }

        public void ProcessMessage(IGMMessage message)
        {
            if (message.Prioritised || string.IsNullOrWhiteSpace(message.GUID))
                QueuePrioritisedMessage(message);
            else
                QueueNormalMessage(message);
        }

        private void QueuePrioritisedMessage(IGMMessage message)
        {
            Monitor.Enter(prioritisedLock);
            PrioritisedQueue.Add(message);
            Monitor.Exit(prioritisedLock);
            RunPrioritisedProcess();
        }

        private void RunPrioritisedProcess()
        {
            PrioritisedTask = Task.Run(() =>
            {
                while (gameMasterController.GameMaster.State != GameMasterState.GameOver)
                {
                    SpinWait.SpinUntil(() => PrioritisedQueue.Count > 0);
                    Monitor.Enter(prioritisedLock);
                    var message = PrioritisedQueue[0];
                    PrioritisedQueue.RemoveAt(0);
                    Monitor.Exit(prioritisedLock);
                    try
                    {
                        var responses = message?.Process(gameMasterController.GameMaster);
                        if (responses != null)
                            foreach (var msg in responses)
                                gameMasterController.BeginSend(msg);
                    }
                    catch (Exception e)
                    {
                        ConsoleWriter.Error("Error during processing prioritised message.\nError message: " + e.Message + "\nStackTrace:" + e.StackTrace);
                    }
                }
            });
        }

        private void QueueNormalMessage(IGMMessage message)
        {
            var guid = message.GUID;
            if(!Queue.ContainsKey(guid))
            {
                Queue.Add(guid, new List<IGMMessage>());
                RunNormalProcess(guid);
            }
            Monitor.Enter(normalLock);
            Queue[guid].Add(message);
            Monitor.Exit(normalLock);
        }

        private void RunNormalProcess(string guid)
        {
            if (QueueTasks.ContainsKey(guid))
            {
                if(QueueTasks[guid].Status == TaskStatus.Running)
                    return;
                else
                {
                    QueueTasks[guid].Dispose();
                    Queue.Remove(guid);
                }
            }
               
           
            QueueTasks.Add(guid, Task.Run(() =>
             {
                 var messagesList = Queue[guid];
                 while (gameMasterController.GameMaster.State != GameMasterState.GameResolved && gameMasterController.GameMaster.State != GameMasterState.GameOver)
                 {
                     SpinWait.SpinUntil(() => messagesList.Count > 0);
                     Monitor.Enter(normalLock);
                     var message = messagesList[0];
                     messagesList.RemoveAt(0);
                     Monitor.Exit(normalLock);
                     try
                     {
                         var responses = message?.Process(gameMasterController.GameMaster);
                         if (responses != null)
                             foreach (var msg in responses)
                                 gameMasterController.BeginSend(msg);
                     }
                     catch (Exception e)
                     {
                         ConsoleWriter.Error("Error during processing normal message for:  " + guid + ".\nError message: " + e.Message + "\nStackTrace:" + e.StackTrace);
                     }
                 }
                 messagesList.Clear(); //czyszczenie wiadomości przez zabiciem - nowa gra lub State == Dead
             }));
        }
    }
}
