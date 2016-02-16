using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quobject.EngineIoClientDotNet.Client;
using Quobject.Collections.Immutable;
using Newtonsoft.Json.Linq;

namespace LCTest {
    class Program {
        static string TimeStamp() {
            return DateTime.Now.ToString("hh:mm:ss") + " ";
        }

        static void Main(string[] args) {
            Socket.Options opts = new Socket.Options();
            opts.Path = "live.eio";
            opts.Port = 443;
            opts.PolicyPort = 843;
            opts.Secure = true;
            opts.Hostname = "ws.www.livecoding.tv";
            opts.Transports = ImmutableList.Create<string>("websocket");

            Socket socket = new Socket("www.livecoding.tv", opts);
            socket.On("connect", (fn) => {
                Console.WriteLine(TimeStamp() + "\r\nConnected event...\r\n");
            }); 
            
            socket.On("update", (data) => {
                Console.WriteLine(TimeStamp() + "recv [socket].[update] event");
                Console.WriteLine(TimeStamp() + "recv [socket].[update] data: " + data);
            }); 
            
            socket.On("message", (data) => {
                Console.WriteLine(TimeStamp() + "recv [socket].[message] event");
                string message = ""; //= Encoding.UTF8.GetString((byte[])data);
                if (data.GetType() == typeof(string)) {
                    message = (string)data;
                } else if (data.GetType() == typeof(byte[])) {
                    message = Encoding.UTF8.GetString((byte[])data);
                } else {
                    message = data.ToString();
                }
                Console.WriteLine(TimeStamp() + "recv [socket].[message] data: " + message);
            }); 
            
            socket.On("open", (data) => { 
                string json = @"['join','stream.callumc']";
                json = json.Replace("'", "\"");
                Console.WriteLine(TimeStamp() + "CONNECTED (" + data + "), sending channels : " + json);
                socket.Send(Encoding.UTF8.GetBytes(json));
            });

            socket.On(Socket.EVENT_ERROR, (data) => {
                EngineIOException exception = (EngineIOException)(data);
                Console.WriteLine(TimeStamp() + "Could not connect " + exception.StackTrace); 
            });


            socket.On("noop", (data) => {
                Console.WriteLine(TimeStamp() + "Noop: " + data);
            });

            socket.On(Socket.EVENT_HANDSHAKE, (data) => { Console.WriteLine(TimeStamp() + "Connection Handshake : " + data); });
            socket.On(Socket.EVENT_HEARTBEAT, () => { Console.WriteLine(TimeStamp() + "HeartBeat"); });
            socket.On(Socket.EVENT_OPEN, () => { Console.WriteLine(TimeStamp() + "SOCKET OPENED"); });
            socket.On(Socket.EVENT_CLOSE, () => { Console.WriteLine(TimeStamp() + "SOCKET CLOSED"); });
            socket.On(Socket.EVENT_HANDSHAKE, () => { Console.WriteLine(TimeStamp() + "SOCKET HANDSHAKE"); });
            socket.On(Socket.EVENT_PACKET, (data) => { 
                Quobject.EngineIoClientDotNet.Parser.Packet packet = ((Quobject.EngineIoClientDotNet.Parser.Packet)data);
                Console.WriteLine(TimeStamp() + "SOCKET PACKET  " + packet.Type); 
            });
            socket.On(Socket.EVENT_UPGRADE, () => { Console.WriteLine(TimeStamp() + "SOCKET UPGRADE"); });
            socket.On(Socket.EVENT_DRAIN, () => { Console.WriteLine(TimeStamp() + "SOCKET DRAINED"); });
            socket.On(Socket.EVENT_FLUSH, () => { Console.WriteLine(TimeStamp() + "SOCKET FLUSHED"); });
            socket.On(Socket.EVENT_PACKET_CREATE, () => { Console.WriteLine(TimeStamp() + "SOCKET PACKET CREATED"); });
            socket.On(Socket.EVENT_TRANSPORT, (dat) => { Console.WriteLine(TimeStamp() + "SOCKET TRANSPORT"); });
            socket.Open();

            do { } while (true);
        }
    }
}
