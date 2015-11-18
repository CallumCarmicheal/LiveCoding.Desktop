﻿

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Quobject.Collections.Immutable;
using Quobject.EngineIoClientDotNet.Client;
using Quobject.EngineIoClientDotNet.Client.Transports;
using Quobject.EngineIoClientDotNet.Modules;
using System.Collections.Generic;
using System.Threading;


namespace Quobject.EngineIoClientDotNet_Tests.ClientTests
{
    [TestClass]
    public class BinaryPollingTest : Connection
    {

         private ManualResetEvent _manualResetEvent = null;

        //[TestMethod]
        //public void PingTest()
        //{

        //    var log = LogManager.GetLogger(Global.CallerName());
        //    log.Info("Start");

        //    var binaryData = new byte[5];
        //    for (int i = 0; i < binaryData.Length; i++)
        //    {
        //        binaryData[i] = (byte)i;
        //    }

        //    var events = new ConcurrentQueue<object>();


        //    var options = CreateOptions();
        //    //options.Transports = ImmutableList.Create<string>(Polling.NAME);

        //    var socket = new Socket(options);

        //    socket.On(Socket.EVENT_OPEN, () =>
        //    {

        //        log.Info("EVENT_OPEN");

        //        socket.Send(binaryData);
        //        socket.Send("cash money €€€");
        //    });

        //    socket.On(Socket.EVENT_MESSAGE, (d) =>
        //    {

        //        var data = d as string;
        //        log.Info(string.Format("EVENT_MESSAGE data ={0} d = {1} ", data, d));

        //        if (data == "hi")
        //        {
        //            return;
        //        }
        //        events.Enqueue(d);
        //        //socket.Close();
        //    });

        //    socket.Open();
        //    Task.Delay(20000).Wait();
        //    socket.Close();
        //    log.Info("ReceiveBinaryData end");

        //    var binaryData2 = new byte[5];
        //    for (int i = 0; i < binaryData2.Length; i++)
        //    {
        //        binaryData2[i] = (byte)(i + 1);
        //    }

        //    object result;
        //    events.TryDequeue(out result);
        //    Assert.AreEqual("1", "1");
        //}



        [TestMethod]
        public void ReceiveBinaryData()
        {

            var log = LogManager.GetLogger(Global.CallerName());
            log.Info("Start");
            _manualResetEvent = new ManualResetEvent(false);

            var events = new Queue<object>();

            var binaryData = new byte[5];
            for (int i = 0; i < binaryData.Length; i++)
            {
                binaryData[i] = (byte)i;
            }


            var options = CreateOptions();
            options.Transports = ImmutableList.Create<string>(Polling.NAME);


            var socket = new Socket(options);

            socket.On(Socket.EVENT_OPEN, () =>
            {

                log.Info("EVENT_OPEN");            
                //socket.Send("cash money €€€");
                socket.Send(binaryData);
            });

            socket.On(Socket.EVENT_MESSAGE, (d) =>
            {
                var data = d as string;
                log.Info(string.Format("EVENT_MESSAGE data ={0} d = {1} ", data, d));

                if (data == "hi")
                {
                    return;
                }
                events.Enqueue(d);
                _manualResetEvent.Set();
            });

            socket.Open();
            _manualResetEvent.WaitOne();           
            socket.Close();
            log.Info("ReceiveBinaryData end");

            var binaryData2 = new byte[5];
            for (int i = 0; i < binaryData2.Length; i++)
            {
                binaryData2[i] = (byte)(i + 1);
            }

            object result;
            result = events.Dequeue();
            CollectionAssert.AreEqual(binaryData, (byte[])result);
        }


        [TestMethod]
        public void ReceiveBinaryDataAndMultibyteUTF8String()
        {
            var log = LogManager.GetLogger(Global.CallerName());
            log.Info("Start");
            _manualResetEvent = new ManualResetEvent(false);

            var events = new Queue<object>();

            var binaryData = new byte[5];
            for (int i = 0; i < binaryData.Length; i++)
            {
                binaryData[i] = (byte)i;
            }
            const string stringData = "cash money €€€";

            var options = CreateOptions();
            options.Transports = ImmutableList.Create<string>(Polling.NAME);


            var socket = new Socket(options);

            socket.On(Socket.EVENT_OPEN, () =>
            {

                log.Info("EVENT_OPEN");

                socket.Send(binaryData); 
                socket.Send(stringData);
            });

            socket.On(Socket.EVENT_MESSAGE, (d) =>
            {

                var data = d as string;
                log.Info(string.Format("EVENT_MESSAGE data ={0} d = {1} ", data, d));

                if (data == "hi")
                {
                    return;
                }
                events.Enqueue(d);
                if (events.Count > 1)
                {
                    _manualResetEvent.Set();
                }
            });

            socket.Open();
            _manualResetEvent.WaitOne();
            socket.Close();


            var binaryData2 = new byte[5];
            for (int i = 0; i < binaryData2.Length; i++)
            {
                binaryData2[i] = (byte)(i + 1);
            }

            object result;
            result = events.Dequeue();
            if (result is string)
            {
                Assert.AreEqual(stringData, (string)result);                
            }
            else
            {
                CollectionAssert.AreEqual(binaryData, (byte[])result);                
            }
            result = events.Dequeue();
            if (result is string)
            {
                Assert.AreEqual(stringData, (string)result);
            }
            else
            {
                CollectionAssert.AreEqual(binaryData, (byte[])result);
            }

        }


    }
}