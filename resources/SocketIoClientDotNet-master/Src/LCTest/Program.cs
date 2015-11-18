using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quobject.SocketIoClientDotNet.Client;

namespace LCTest {
    class Program {
        static void Main(string[] args) {
            IO.Options opts = new IO.Options();
            


            Socket socket = IO.Socket("", opts);


            // register for 'connect' event with io server
            socket.On("connect", (fn) => {
                Console.WriteLine("\r\nConnected event...\r\n");
                Console.WriteLine("Emit Part object");

                // emit Json Serializable object, anonymous types, or strings
                Part newPart = new Part() { PartNumber = "K4P2G324EC", Code = "DDR2", Level = 1 };
                socket.Emit("partInfo", newPart);

            });


            // register for 'update' events - message is a json 'Part' object
            socket.On("update", (data) => {
                Console.WriteLine("recv [socket].[update] event");
                //Console.WriteLine("  raw message:      {0}", data.RawMessage);
                //Console.WriteLine("  string message:   {0}", data.MessageText);
                //Console.WriteLine("  json data string: {0}", data.Json.ToJsonString());
                //Console.WriteLine("  json raw:         {0}", data.Json.Args[0]);

                // cast message as Part - use type cast helper
                Part part = data.Json.GetFirstArgAs<Part>();
                Console.WriteLine(" Part Level:   {0}\r\n", part.Level);

            });
        }
    }
}
