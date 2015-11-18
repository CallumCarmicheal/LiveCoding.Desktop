using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quobject.EngineIoClientDotNet.Client;

namespace LCTest {
    class Program {
        static void Main(string[] args) {
            Socket.Options opts = new Socket.Options();
            opts.Path = "/live.eio/";
            opts.Port = 80;
            opts.PolicyPort = 80;
            opts.Secure = false;
            opts.Hostname = "www.livecoding.tv";



            Socket socket = new Socket("http://www.livecoding.tv/", opts);
            socket.On("connect", (fn) => {
                Console.WriteLine("\r\nConnected event...\r\n");

            }); socket.On("update", (data) => {
                Console.WriteLine("recv [socket].[update] event");

                Console.WriteLine("recv [socket].[update] data: " + data);

            }); socket.On("message", (data) => {
                Console.WriteLine("recv [socket].[message] event");
                Console.WriteLine("recv [socket].[message] data: " + data);
            }); 
            
            socket.On("open", (data) => { Console.WriteLine("CONNECTED, send args? : " + data); });
            socket.On(Socket.EVENT_ERROR, (data) => {
                EngineIOException exception = (EngineIOException)(data);



                Console.WriteLine("Could not connect " + exception.StackTrace); 
            });
            socket.On(Socket.EVENT_HANDSHAKE, (data) => { Console.WriteLine("Connection Handshake : " + data); });

            socket.Open();
            Console.WriteLine("Socket Opened");
            do {

            } while (true);
        }
    }
}
