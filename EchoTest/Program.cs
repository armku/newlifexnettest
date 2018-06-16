﻿using System;
using System.Threading;
using NewLife.Log;
using NewLife.Net;
using NewLife.Threading;

namespace EchoTest
{
    class Program
    {
        static void Main(String[] args)
        {
            XTrace.UseConsole();

            try
            {                               
                    TestClient();
            }
            catch (Exception ex)
            {
                XTrace.WriteException(ex);
            }

            Console.WriteLine("OK!");
            Console.ReadKey();
        }

        static TimerX _timer;
        static NetServer _server;
        
        static void TestClient()
        {
            var uri = new NetUri("tcp://127.0.0.1:1234");
            //var uri = new NetUri("tcp://net.newlifex.com:1234");
            var client = uri.CreateRemote();
            client.Log = XTrace.Log;
            client.Received += (s, e) =>
            {
                XTrace.WriteLine("收到：{0}", e.Packet.ToStr());
            };
            client.Open();

            // 定时显示性能数据
            _timer = new TimerX(ShowStat, client, 100, 1000);

            // 循环发送数据
            for (var i = 0; i < 5; i++)
            {
                Thread.Sleep(1000);

                var str = "你好" + (i + 1);
                client.Send(str);
            }

            client.Dispose();
        }

        static void ShowStat(Object state)
        {
            var msg = "";
            if (state is NetServer ns)
                msg = ns.GetStat();
            else if (state is ISocketRemote ss)
                msg = ss.GetStat();

            Console.Title = msg;
        }
    }
}