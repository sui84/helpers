using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Concurrent;
using Common.Utils;
using Common.Utils.Log;

namespace Common.Tests
{
    // 待整理
    public class TestThreads
    {
        public List<string> users = new List<string>();
        public TestThreads()
        {
            int numberOfUsers = 10;
            for (int i = 0; i < numberOfUsers; i++)
            {
                this.users.Add(string.Format("LoadTest{0}", i + 1));
            }
        }

        private  void WriteLog(string userName)
        {
            Console.WriteLine(string.Format("Begin Time {0}:ProcessId {2} ThreadId {3} Handle {1}", DateTime.Now, userName, System.Diagnostics.Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId));
            Thread.Sleep(5000);
            if (userName == "LoadTest0")
            {
                Thread.Sleep(5000);
                throw new Exception("Lazy!");
            }
            Console.WriteLine(string.Format("End Time {0}:ProcessId {2} ThreadId {3} Handle {1}", DateTime.Now, userName, System.Diagnostics.Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId));
        }

        #region 2个线程并行执行
        private void DiffTheadForTask()
        {
            Console.WriteLine(string.Format("{0} Start Time {1}", MethodBase.GetCurrentMethod().Name, DateTime.Now));
            Task t = Task.Factory.StartNew(() => WriteLog(users[0]));
            Thread.Sleep(5000);
            Console.WriteLine("Is first one start?");
            Task t1 = Task.Factory.StartNew(() => WriteLog(users[1]));
            Thread.Sleep(5000);
            Console.WriteLine("Are they start?");
            Thread.Sleep(5000);
            t1.Wait();
            Console.WriteLine("Which one end?");
            t.Wait();
            Console.WriteLine(string.Format("{0} End Time {1}", MethodBase.GetCurrentMethod().Name, DateTime.Now));
        }
        #endregion

        #region 10个线程并行执行10个user task
        private void TheadForEveryTask()
        {
            Console.WriteLine(string.Format("{0} Start Time {1}",MethodBase.GetCurrentMethod().Name, DateTime.Now));
            var userThreads = new List<Thread>();
            for (int i = 0; i < users.Count(); i++)
            {
                int index = i;
                Thread t = new Thread(() => WriteLog(users[index]));
                t.Name = "Thread" + i;
                userThreads.Add(t);
            }

            Parallel.ForEach(userThreads, thread => thread.Start());
            foreach (var userThread in userThreads)
            {
                userThread.Join();
            }
            Console.WriteLine(string.Format("{0} End Time {1}", MethodBase.GetCurrentMethod().Name, DateTime.Now));
        }
         #endregion

        #region 不等到完成就结束的错误写法
        private void TheadForEveryTask2()
        {
            Console.WriteLine(string.Format("{0} Start Time {1}", MethodBase.GetCurrentMethod().Name, DateTime.Now));
            foreach (string user in users)
            {
                ThreadPool.QueueUserWorkItem(arg =>
                {
                    WriteLog(user);
                });
            }
            Console.WriteLine(string.Format("{0} End Time {1}", MethodBase.GetCurrentMethod().Name, DateTime.Now));
        }
        #endregion

        #region 每次最多做3个user task
        private void LimitTheadForAllTask()
        {
            int maxWorkUsers = 3;
            Console.WriteLine(string.Format("{0} Start Time {1}", MethodBase.GetCurrentMethod().Name, DateTime.Now));
            Parallel.For(0, users.Count(), (new ParallelOptions { MaxDegreeOfParallelism = maxWorkUsers }), (int i) =>
            {
                try
                {
                    WriteLog(users[i]);
                }
                catch (Exception ex)
                {
                    string fileName = string.Format("{0}\\Temp\\{1}.txt", AppDomain.CurrentDomain.BaseDirectory, Thread.CurrentThread.ManagedThreadId);
                    LogHelper logHelper = new LogHelper();
                    logHelper.LogInfo(string.Format("Exception in {0}:{1}\r\n{2}", AppDomain.CurrentDomain.FriendlyName, ex.Message, ex.StackTrace), fileName);
                }
            });
            Console.WriteLine(string.Format("{0} End Time {1}", MethodBase.GetCurrentMethod().Name, DateTime.Now));
        }
        #endregion

        #region 每次最多做3个user task,不等到完成就结束的错误写法
        private void LimitTheadForAllTask2()
        {
            int maxWorkUsers = 3;
            Console.WriteLine(string.Format("{0} Start Time {1}", MethodBase.GetCurrentMethod().Name, DateTime.Now));
            var options = new ParallelOptions();
            options.MaxDegreeOfParallelism = maxWorkUsers;
            Parallel.ForEach(users, options,
              (user) =>
              {
                  WriteLog(user);
              });
            Console.WriteLine(string.Format("{0} End Time {1}", MethodBase.GetCurrentMethod().Name, DateTime.Now));
        }
        #endregion

        #region 不知是否正确的写法
        private void TheadForAllTask3()
        {
            Console.WriteLine(string.Format("{0} Start Time {1}", MethodBase.GetCurrentMethod().Name, DateTime.Now));
            Task[] ts = new Task[10];
            for (int i = 0; i < users.Count; i++)
            {
                ts[i] = Task.Factory.StartNew((object obj) =>
                {
                    //不能直接用i
                    int j = (int)obj;
                    WriteLog(users[j]);
                }, i);
            }
            Task.WaitAll(ts); 
            Console.WriteLine(string.Format("{0} End Time {1}", MethodBase.GetCurrentMethod().Name, DateTime.Now));
        }

        private void TheadForAllTask4()
        {
            Console.WriteLine(string.Format("{0} Start Time {1}", MethodBase.GetCurrentMethod().Name, DateTime.Now));
            ConcurrentBag<Task> ts = new ConcurrentBag<Task>();
            for (int i = 0; i < users.Count; i++)
            {
                Task t = Task.Factory.StartNew((object obj) =>
                {
                    int j = (int)obj;
                    WriteLog(users[j]);
                }, i);
                ts.Add(t);
            }
            Task.WaitAll(ts.ToArray());
            Console.WriteLine(string.Format("{0} End Time {1}", MethodBase.GetCurrentMethod().Name, DateTime.Now));
        }

//        3.20个人，后10个人监控前10个人干活

//static void Main(string[] args)
//        {
//            try
//            {
//                int numberOfUsers = 10;
//                var users = new string[20];
//                for (int i = 0; i < 20; i++)
//                {
//                    users[i] = string.Format("LoadTest{0}", i + 1);
//                }

//                Task[] ts = new Task[numberOfUsers];
//                for (int i = 0; i < numberOfUsers; i++)
//                {
//                    ts[i] = Task.Factory.StartNew((object obj) =>
//                    {
//                        int j = (int)obj;
//                        WriteLog(users[j]);
//                    }, i).ContinueWith((prevTask) =>
//                    {
//                        string exp = string.Empty;
//                        if (prevTask.Exception!=null )  exp = "Exception:" + prevTask.Exception.Message;
//                        Console.WriteLine(string.Format("ID:{0} Status:{1} IsCanceled:{2} IsCompleted:{3} {4}", prevTask.Id, prevTask.Status, prevTask.IsCanceled, prevTask.IsCompleted, exp));
//                        WriteLog(users[9 + prevTask.Id]);
//                    });
                   
//                }
                
//                Task.WaitAll(ts);
//                Console.WriteLine("Will End soon!");
//                Thread.Sleep(500000000);

//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(string.Format("Time {0}:{1}",DateTime.Now, ex.Message));
//                Thread.Sleep(500000000);
//            }
//        }

//        private static void WriteLog(string userName)
//        {
//            Console.WriteLine(string.Format("Begin Time {0}:ProcessId {2} ThreadId {3} {1}", DateTime.Now, userName, System.Diagnostics.Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId));
//            Thread.Sleep(5000);
//            if (userName == "LoadTest2")
//            {
//                Thread.Sleep(5000);
//                throw new Exception("Lazy!");
//            }
//            Console.WriteLine(string.Format("End Time {0}:ProcessId {2} ThreadId {3} {1}", DateTime.Now, userName, System.Diagnostics.Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId));
//        }
        
//4.Cancel一个任务下面的所有子任务
//static void Main(string[] args)
//        {
//            try
//            {
//                int numberOfUsers = 10;
//                var users = new string[numberOfUsers];
//                for (int i = 0; i < numberOfUsers; i++)
//                {
//                    users[i] = string.Format("LoadTest{0}", i + 1);
//                }
//                CancellationTokenSource tokenSource = new CancellationTokenSource();
//                CancellationToken  token = tokenSource.Token;
//                ConcurrentBag<Task> ts = new ConcurrentBag<Task>();
//                Console.WriteLine("Press any key to begin tasks...");
//                Console.WriteLine("To terminate the example, press 'c' to cancel and exit...");
//                Console.ReadKey();
//                Console.WriteLine();


//                //Task t = Task.Factory.StartNew(() => WriteLog(users[0], token), token);
//                //Console.WriteLine("Task {0} executing", t.Id);
//                //ts.Add(t);

//                Task t = Task.Factory.StartNew(() =>
//                {
//                    Task tc;
//                    for (int i = 0; i < numberOfUsers; i++)
//                    {
//                        tc = Task.Factory.StartNew((object obj) =>
//                        {
//                            int j = (int)obj;
//                            WriteLog(users[j], token);
//                        }, i, token);
//                        ts.Add(tc);
//                    }
//                },token);
//                ts.Add(t);

//                if (Console.ReadKey().KeyChar == 'c')
//                {
//                    tokenSource.Cancel();
//                    Console.WriteLine("\nTask cancellation requested.");
//                }
//                try
//                {
//                    Task.WaitAll(ts.ToArray());
//                }
//                catch (AggregateException e)
//                {
//                    foreach (var v in e.InnerExceptions)
//                    {
//                        if (v is TaskCanceledException)
//                            Console.WriteLine("   TaskCanceledException: Task {0}",
//                                              ((TaskCanceledException)v).Task.Id);
//                        else
//                            Console.WriteLine("   Exception: {0}", v.GetType().Name);
//                    } 

//                }
//                Console.WriteLine("Will End soon!");
//                Thread.Sleep(500000000);

//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(string.Format("Time {0}:{1}",DateTime.Now, ex.Message));
//                Thread.Sleep(500000000);
//            }
//        }

//        private static void WriteLog(string userName, CancellationToken ct)
//        {
//            Console.WriteLine(string.Format("Begin Time {0}:ProcessId {2} ThreadId {3} {1}", DateTime.Now, userName, System.Diagnostics.Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId));
//            if (userName != "LoadTest1" && userName != "LoadTest2" && userName != "LoadTest3")
//            {
//                Thread.Sleep(5000);
//            }
//            if (ct.IsCancellationRequested)
//            {
//                Console.WriteLine(string.Format("Cancel :{0} ThreadId {1}", userName, Thread.CurrentThread.ManagedThreadId));
//                ct.ThrowIfCancellationRequested();
//            }
//            Console.WriteLine(string.Format("End Time {0}:ProcessId {2} ThreadId {3} {1}", DateTime.Now, userName, System.Diagnostics.Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId));
//        }
        
//5.每次最多3个user
//        static void Main(string[] args)
//        {
//            try
//            {
//                int numberOfUsers = 10;
//                var users = new string[numberOfUsers];
//                for (int i = 0; i < numberOfUsers; i++)
//                {
//                    users[i] = string.Format("LoadTest{0}", i + 1);
//                }

//                Action[] aArr = new Action[numberOfUsers];
//                int j = 0 ;
//                for (int i = 0; i < numberOfUsers; i++){
//                aArr[i] = new Action(() => WriteLog(users[j++]));
//               }

//                System.Threading.Tasks.ParallelOptions po = new ParallelOptions();
//                po.MaxDegreeOfParallelism = 3;
                
//                try
//                {
//                    System.Threading.Tasks.Parallel.Invoke(po, aArr);
//                }
//                catch (AggregateException e)
//                {
//                    foreach (var v in e.InnerExceptions)
//                    {
//                        if (v is TaskCanceledException)
//                            Console.WriteLine("   TaskCanceledException: Task {0}",
//                                              ((TaskCanceledException)v).Task.Id);
//                        else
//                            Console.WriteLine("   Exception: {0}", v.Message);
//                    } 

//                }
//                Console.WriteLine("Will End soon!");
//                Thread.Sleep(500000000);

//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(string.Format("Time {0}:{1}",DateTime.Now, ex.Message));
//                Thread.Sleep(500000000);
//            }
//        }

//        private static void WriteLog(string userName)
//        {
//            Console.WriteLine(string.Format("Begin Time {0}:ProcessId {2} ThreadId {3} {1}", DateTime.Now, userName, System.Diagnostics.Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId));
//            Thread.Sleep(5000);
//            if (userName == "LoadTest2")
//            {
//                Thread.Sleep(5000);
//                throw new Exception("Lazy!");
//            }
//            Console.WriteLine(string.Format("End Time {0}:ProcessId {2} ThreadId {3} {1}", DateTime.Now, userName, System.Diagnostics.Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId));
//        }
        
//6.总共10个user,每次最多3个user，后一批要等前面一批全部完成才继续

//static void Main(string[] args)
//        {
//            try
//            {
//                int numberOfUsers = 10;
//                int maxConcurrent = 3;
//                var users = new string[numberOfUsers];
//                for (int i = 0; i < numberOfUsers; i++)
//                {
//                    users[i] = string.Format("LoadTest{0}", i + 1);
//                }

//                int itemProcessed = 0;
//                int loopCnt = 0;
//                do
//                {
//                    List<Task> taskList = new List<Task>();
//                    for (int i = 0; i < maxConcurrent; i++)
//                    {
//                        taskList.Add(Task.Factory.StartNew((object obj) => 
//                            {
//                                int j = (int)obj;
//                                try
//                                {
//                                    if (loopCnt * maxConcurrent + j < numberOfUsers)
//                                    {
//                                        WriteLog(users[loopCnt * maxConcurrent + j]);
//                                    }
//                                }
//                                catch (Exception ex)
//                                {
//                                    Console.WriteLine(string.Format("{0} encounter error :{1}", users[loopCnt * maxConcurrent + j],ex.Message ));
//                                }
//                            }
//                            ,i));
//                        Interlocked.Increment(ref itemProcessed);
//                    }
//                    Task.WaitAll(taskList.ToArray());
//                    loopCnt++;
//                } while (itemProcessed < numberOfUsers);

//                Console.WriteLine("Will End soon!");
//                Thread.Sleep(500000000);

//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(string.Format("Time {0}:{1}",DateTime.Now, ex.Message));
//                Thread.Sleep(500000000);
//            }
//        }

//        private static void WriteLog(string userName)
//        {
//            Console.WriteLine(string.Format("Begin Time {0}:ProcessId {2} ThreadId {3} {1}", DateTime.Now, userName, System.Diagnostics.Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId));
//            Thread.Sleep(5000);
//            if (userName == "LoadTest2")
//            {
//                Thread.Sleep(5000);
//                throw new Exception("Lazy!");
//            }
//            Console.WriteLine(string.Format("End Time {0}:ProcessId {2} ThreadId {3} {1}", DateTime.Now, userName, System.Diagnostics.Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId));
//        }
        
//==================================
//7.多線程改變某值

//private static List<string> testList = new List<string>();
//        private static void AddList(string name)
//        {
//            testList.Add(name);
//        }

//        static void test2()
//        {
            
//            int threadcount = 10;
//            List<Thread> threads = new List<Thread>();
//            for (int i = 0; i < threadcount; i++)
//            {
//                Thread t = new Thread(delegate(object parameter)
//                {
//                    EmpServiceDataContext db = new EmpServiceDataContext();
//                    int _i = (int)parameter;
//                    int index = 0;
//                    for( int k = 0 ; k < 20000 ; k++)
//                    {
//                        if (index % threadcount == _i)
//                        {
//                            try
//                            {
//                                AddList("Thread" + k);
//                            }
//                            catch (Exception ex)
//                            {
//                            }
//                        }
//                        index++;
//                    }
//                });
//                t.Name = string.Format("SendEmailThread_{0}", i);
//                threads.Add(t);
//                t.Start(i);
//            }
//            foreach (Thread t in threads)
//            {
//                t.Join();
//            }
//        }
        
//Task t = Task.Factory.StartNew(() => test2());
//t.Wait();
                
                

//7.volatile  和 Interlocked
//http://baike.baidu.com/view/608706.htm?fr=aladdin

//1）. 并行设备的硬件寄存器（如：状态寄存器）
 
//2）. 一个中断服务子程序中会访问到的非自动变量（Non-automatic variables)
 
//3）. 多线程应用中被几个任务共享的变量

//Interlocked.Decrement(ref JOB_COUNT);

//8. ThreadStartStop
//如何：创建和终止线程（C# 编程指南）
//http://msdn.microsoft.com/zh-cn/library/7a2f3ay4(VS.80).aspx

//public class Worker
//{
//    // This method will be called when the thread is started.
//    public void DoWork()
//    {
//        while (!_shouldStop)
//        {
//            Console.WriteLine("worker thread: working...");
//        }
//        Console.WriteLine("worker thread: terminating gracefully.");
//    }
//    public void RequestStop()
//    {
//        _shouldStop = true;
//    }
//    // Volatile is used as hint to the compiler that this data
//    // member will be accessed by multiple threads.
//    private volatile bool _shouldStop;
//}

//public class WorkerThreadExample
//{
//    static void Main()
//    {
//        // Create the thread object. This does not start the thread.
//        Worker workerObject = new Worker();
//        Thread workerThread = new Thread(workerObject.DoWork);

//        // Start the worker thread.
//        workerThread.Start();
//        Console.WriteLine("main thread: Starting worker thread...");

//        // Loop until worker thread activates.
//        while (!workerThread.IsAlive);

//        // Put the main thread to sleep for 1 millisecond to
//        // allow the worker thread to do some work:
//        Thread.Sleep(1);

//        // Request that the worker thread stop itself:
//        workerObject.RequestStop();

//        // Use the Join method to block the current thread 
//        // until the object's thread terminates.
//        workerThread.Join();
//        Console.WriteLine("main thread: Worker thread has terminated.");
//    }
//}

//9.Thread类的Join()方法 
//多线程：Thread类的Join()方法
// http://blog.163.com/hc_ranxu/blog/static/3672318220095284513678/

        #endregion


    }
}
