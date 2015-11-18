using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quobject.SocketIoClientDotNet.Client;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Schema;

namespace LiveCoding.Backend.SocketIO {
    class LiveEvents {
        #region Variables
        //Hashtable Channels = new Hashtable();
        List<string> Channels = new List<string>();
        Socket listener;
        bool enableReconnect = false;
        #endregion

        #region API
        public Socket getListener() {
            return this.listener;
        }

        public void showNotification(string message) {
            Console.WriteLine(message);
        }

        public void addChannel(string channel) {
            Channels.Add(channel);
        }

        public bool Contains(JObject data, string arg) {
            if (data[arg] == null) {
                return false;
            }
            return true;
        }
        #endregion

        #region Sockets
        Socket createSocket() {
            IO.Options opts = new IO.Options();
            opts.Path = "/live.eio/";
            return IO.Socket("https://www.livecoding.tv/", opts);
        }

        public bool eventIsHooked(string evt) {
            //return listener.Io().HasListeners(evt);
            return false;
        }
        #endregion

        public void startListener(bool forceCreate = false) {
            string status = "";

            if (forceCreate) {
                status = "(FORCED) ";

                if(listener != null)
                    if (listener.Connected)
                        listener.Disconnect();
                listener = null;
            }

            if ((this.listener == null) || !listener.Connected) {
                listener = createSocket();
                Console.WriteLine(status + "Created Listener Socket");

                // Socket Handled Events
                listener.On("open", (data) => { Console.WriteLine("CONNECTED, send args? : " + data); });
                listener.On(Socket.EVENT_CONNECT_ERROR, (data) => { Console.WriteLine("Could not connect " + data); });
                listener.On(Socket.EVENT_CONNECT_TIMEOUT, (data) => { Console.WriteLine("Connection timed out." + data); });

                // LiveCoding Events
                listener.On("open", SOCKET_Open);
                listener.On("message", (args) => { SOCKET_Message(args); });
                Console.WriteLine("Listener Created");

                listener.Connect();
                listener.Open();
                Console.WriteLine("Listener Started");
            } else {
                Console.WriteLine("Listener already created, please force the creation");
            }
        }

        #region Events
        int retries = 0;
        public void SOCKET_Timeout(object p1) {
            string reason = (string)(p1);

            if (retries >= 4) {
                Console.WriteLine("Timed out from server (4/4): \n\r" + reason);
            } else {
                Console.WriteLine("Timed out from server (" + retries + "/4)");
                retries++;
            }
        }

        public void SOCKET_ConnectERROR(object p1) {
            string reason = (string)(p1);

            Console.WriteLine("Error on Connection: " + reason);
        }

        public void SOCKET_Open() {
            enableReconnect = true;

            foreach (string channel in Channels) {
                JObject json = new JObject();
                json["join"] = channel;

                listener.Send(json.ToString());

                Console.WriteLine("Joined Channel: " + channel);
            }
        }

        public void SOCKET_Message(object MessageData) {
            string JSON = (string)(MessageData);
            JObject data = JObject.Parse(JSON);

            foreach (KeyValuePair<string, JToken> property in data)
                Console.WriteLine(property.Key + " - " + property.Value);
            if(Contains(data, "channel_msg")) { }
        }
        #endregion
    }
}
