using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LiveCoding.Backend {
    class Engine {
        SocketIO.LiveEvents Listener;

        public Engine() { }

        public SocketIO.LiveEvents getListenerEngine() {
            return Listener;
        }

        public void CreateListener() {
            Listener = new SocketIO.LiveEvents();
        }
    }
}
