using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LiveCoding.Backend {
    public class Engine {
        public SocketIO.LiveEvents Events;
        public SocketIO.Listener Listener { get; set; }

        public Engine() { }

        public void StartListener(string Username) {
            Events = new SocketIO.LiveEvents();
            Events.startListener();
        }
    }
}
