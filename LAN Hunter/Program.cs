using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace LAN_Hunter
{
    internal class Program
    {
        static int thread_num = Environment.ProcessorCount;
        static List<string> hosts = new List<string> { 
            "www.baidu.com", 
            "www.so.com", 
            "www.hao123.com",
            "www.iqiyi.com",
            "www.cnblogs.com",
            "cn.bing.com",
            "www.bilibili.com",
            "www.msn.cn",
            "www.sohu.com",
            "www.163.com",
            "www.gushiwen.cn",
            "www.qq.com",
            "weixin.qq.com",
            "www.onlinedown.net"
        };

        static int[] ports = { 80, 443 };

        static int process_num = 4;
        static int i = 0;

        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                if (args[0].Equals("--process"))
                {
                    Console.Title = "0";

                    for (int i = 0; i < thread_num; i++)
                    {
                        // 启动一个新任务
                        Task.Run(() => Hunter(3977 / thread_num));
                    }

                    Console.WriteLine($"[ INFO ] {i} threads started: deal {3977 / thread_num}");

                    Hunter(3977 % thread_num);
                }
                else
                {
                    process_num = int.Parse(args[0]);
                }
            }

            Console.Title = "0";

            string appPath = Process.GetCurrentProcess().MainModule.FileName;

            int currentProcessId = Process.GetCurrentProcess().Id;

            // Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(appPath));
            // foreach (Process process in processes)
            // {
            //     if (process.Id == currentProcessId)
            //     {
            //         continue;
            //     }
            //     process.Kill();
            //     Console.WriteLine("[ INFO ] killed one process.");
            // }

            List<Process> processlist = new List<Process>();

            for (int i = 0; i < process_num; i++)
            {
                // 启动进程并将其放在后台运行
                Process process = new Process();

                process.StartInfo.FileName = appPath;
                process.StartInfo.Arguments = "--process";
                // process.StartInfo.CreateNoWindow = true;
                // process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                process.StartInfo.UseShellExecute = false;

                process.Start();

                processlist.Add(process);

            }

            Console.WriteLine("[ INFO ] started done.");

            Console.ReadLine();

            foreach (Process process in processlist)
            {
                process.Kill();
            }

            Console.WriteLine("Killed.");

        }

        static TcpClient GetTcpClient()
        {
            while (true)
            {
                if (i >= (hosts.Count - 1))
                {
                    i = 0;
                }
                else
                {
                    i++;
                }

                string host = hosts[i];

                TcpClient client = new TcpClient();

                Random rnd = new Random();
                int rand = rnd.Next(ports.Length);
                int port = ports[rand];

                try
                {
                    client.Connect(host, port);
                    Console.Title = (int.Parse(Console.Title) + 1).ToString();
                    return client;
                }
                catch (Exception e)
                {
                    client.Close();
                    Console.WriteLine($"[ Error ] There was an error when connecting: {host}");
                    Console.WriteLine(e.ToString());
                }

                Thread.Sleep(1);
            }
        }

        static void Hunter(int num)
        {
            List<TcpClient> clients = new List<TcpClient>();

            for (int i = 0; i < num; i++)
            {
                clients.Add(GetTcpClient());
            }

            Console.WriteLine("done");

            while(true)
            {
                foreach (TcpClient client in clients)
                {
                    if (!client.Connected)
                    {
                        clients.Remove(client);
                        clients.Add(GetTcpClient());
                        Console.WriteLine("[ INFO ] ReCononnected A Connect.");
                    }
                    Thread.Sleep(1);
                }
            }
        }
    }
}
