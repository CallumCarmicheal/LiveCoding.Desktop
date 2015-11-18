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
        //Hashtable Channels = new Hashtable();
        List<string> Channels = new List<string>();
        Socket listener;
        bool enableReconnect = false;


        public void showNotification(string message) {
            Console.WriteLine(message);
        }

        Socket createSocket() {
            IO.Options opts = new IO.Options();
            opts.Path = "/live.eio/";
            return IO.Socket("https://www.livecoding.tv/", opts);
        }

        public void addChannel(string channel) {
            Channels.Add(channel);
        }

        public bool hooked(string evt) {
            return listener.HasListeners(evt);
        }

        public void startListener() {
            if (!listener.Connected) {
                listener = createSocket();
            }

            if (!hooked("open")) listener.On("open", SOCKET_Open);
            if (!hooked("message")) listener.On("message", (args) => { SOCKET_Message(args); });
        }

        public Socket getListener() {
            return this.listener;
        }


        public void SOCKET_Open() {
            enableReconnect = true;

            foreach (string channel in Channels) {
                JObject json = new JObject();
                json["join"] = channel;

                listener.Send(json.ToString());
            }
        }

        public void SOCKET_Message(object MessageData) {
            string JSON = (string)(MessageData);
            JObject data = JObject.Parse(JSON);

            foreach (KeyValuePair<string, JToken> property in data)
                Console.WriteLine(property.Key + " - " + property.Value);
            if(Contains(data, "channel_msg")) { }
        }

        public bool Contains(JObject data, string arg) {
            if (data[arg] == null) {
                return false;
            }
            return true;
        }
    }
}
