using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAccesToOneObject
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var api = new ManagingObject();
            var invokers = new List<InvokingObject>();
            for(int i = 0;i<64;i++)
            {
                invokers.Add(new InvokingObject(i, api));
            }
            var tasks = new Task<string>[invokers.Count];
            while(api.Request <1000)
            {
                for(int i = 0;i<invokers.Count;i++)
                {
                    var task = tasks[i];
                    var invoker = invokers[i];
                    if (task == null)
                        tasks[i] = Task<string>.Run(() => invoker.DoStrategy());
                    else if (task.IsCompleted)
                    {
                        Console.WriteLine(task.Result);
                        task.Dispose();
                        tasks[i] = Task<string>.Run(() => invoker.DoStrategy());
                    }
                    else
                    {
                        continue;
                    }
                }
                if(api.Request == 100)
                    Console.WriteLine("100");
            }
            while(invokers.Count >0)
            {
                for (int i = 0; i < tasks.Length; i++)
                {
                    var task = tasks[i];
                    if (task != null && task.IsCompletedSuccessfully)
                    {
                        invokers.RemoveAt(0);
                        tasks[i] = null;
                    }
                    if (invokers.Count == 0)
                        break;
                }
            }

            Console.WriteLine("End of invoking");
            foreach (var task in tasks)
                if (task != null)
                    Console.WriteLine("Found working task!");

                
        }
    }

    public class ManagingObject
    {
        private Random random;
        public int Request { get; set; }
        public string TimeConsumingAction()
        {
            Request++;
            var number = Request;
            var timeToSleeep = random.Next() % 20 * 100 + 1000;
            Console.WriteLine("Request {0} goes to sleep.", number);
            Thread.Sleep(timeToSleeep);
            Console.WriteLine("Request {0} send.", number);
            return "Request handle ended. Number: " + number;
        }


        public ManagingObject()
        {
            random = new Random();
        }
    }

    public class InvokingObject
    {
        private ManagingObject apiObject;
        public int GUID { get; set; }
        public InvokingObject(int guid, ManagingObject api )
        {
            GUID = guid;
            apiObject = api;
        }

        public string DoStrategy()
        {
            Console.WriteLine("Agent {0} begins strategy.", GUID);
            var data = apiObject.TimeConsumingAction();
            Console.WriteLine("Agent {0} ends strategy.", GUID);
            return data;
        }
    }
}
