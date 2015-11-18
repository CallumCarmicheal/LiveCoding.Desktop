﻿//using log4net;

using System.Threading;

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Quobject.EngineIoClientDotNet.Client;
using System;
using Quobject.EngineIoClientDotNet.Modules;


namespace Quobject.EngineIoClientDotNet_Tests.ClientTests
{
    [TestClass]
    public class UsageTest : Connection
    {


        [TestMethod]
        public void Usage1()
        {
            LogManager.SetupLogManager();
            var log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod());
            log.Info("Start");


            var options = CreateOptions();
            var socket = new Socket(options);

            //You can use `Socket` to connect:
            //var socket = new Socket("ws://localhost");
            socket.On(Socket.EVENT_OPEN, () =>
            {
                socket.Send("hi");
                socket.Close();
            });
            socket.Open();

            //System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
        }

        private ManualResetEvent _manualResetEvent = null;

        [TestMethod]
        public void Usage2()
        {
            LogManager.SetupLogManager();
            var log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod());
            log.Info("Start");
            _manualResetEvent = new ManualResetEvent(false);

            var options = CreateOptions();
            var socket = new Socket(options);

            //Receiving data
            //var socket = new Socket("ws://localhost:3000");
            socket.On(Socket.EVENT_OPEN, () =>
            {
                socket.On(Socket.EVENT_MESSAGE, (data) => Console.WriteLine((string)data));
                _manualResetEvent.Set();
            });
            socket.Open();
            _manualResetEvent.WaitOne();
            socket.Close();

            
        }


    }
}
