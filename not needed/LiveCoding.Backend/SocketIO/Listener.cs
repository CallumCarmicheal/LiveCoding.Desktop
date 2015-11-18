using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quobject.SocketIoClientDotNet.Client;

namespace LiveCoding.Backend.SocketIO {
    class Listener {
        Socket listener;


        public void setupListener(string Username) {
            // Listener options
            IO.Options opts = new IO.Options();
            opts.Path = "/live.eio/";

            // Create our listener
            listener = IO.Socket("https://www.livecoding.tv/", opts);

            // Add our events
            listener.On("open", (data) => { Console.WriteLine("CONNECTED, send args? : " + data); });
            listener.On(Socket.EVENT_CONNECT_ERROR, (data) => { Console.WriteLine("Could not connect " + data); });
            listener.On(Socket.EVENT_CONNECT_TIMEOUT, (data) => { Console.WriteLine("Connection timed out." + data); });

            // We created our listener (DONT OPEN LISTENER LET OTHER METHODS DO IT)
            Console.WriteLine("CREATED LISTENER");
        }
    }
}
