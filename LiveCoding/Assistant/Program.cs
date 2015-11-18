using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveCoding.Assistant {
    static class Program {
        static LiveCoding.Backend.Engine engine = new Backend.Engine();


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            /*Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());*/

            doMethod(2);
        }

        static void doMethod(int method) {
            if (method == 1) {
                Backend.SocketIO.Listener list = new Backend.SocketIO.Listener();
                list.setupListener("callumc");

                do { } while (true);
            } else if (method == 2) {
                Console.WriteLine("Program Started");
                engine.CreateListener();
                engine.getListenerEngine().addChannel("callumc");
                engine.getListenerEngine().startListener(true);

                Console.WriteLine("Waiting 5 Seconds");
                System.Threading.Thread.Sleep(5000);
                do {
                    if (engine.getListenerEngine() != null) {
                        if (engine.getListenerEngine().getListener() != null) {
                            /*bool isConnected = engine.getListenerEngine().getListenerEngine().Connected;
                            Console.WriteLine("CONNECTION STATUS: " + isConnected);

                            if (!isConnected) {
                                Console.WriteLine("RECREATING SOCKET");
                                recreateSocket();

                                Console.WriteLine("Waiting 5 Seconds");
                                System.Threading.Thread.Sleep(5000);
                            }*/
                        } else {
                            Console.WriteLine("Socket is null, recreating it.");
                            recreateSocket();
                        }
                    } else {
                        Console.WriteLine("Listener is null, recreating it & socket.");
                        recreateListenerAndSocket();
                    }
                    System.Threading.Thread.Sleep(1000);
                } while (true);
                Console.WriteLine("Program Ended"); // Should not be reached but if it does, i want to know about it
            }
        }

        static void recreateSocket() {
            engine.getListenerEngine().addChannel("callumc");
            engine.getListenerEngine().startListener(true);
        }

        static void recreateListenerAndSocket() {
            engine.CreateListener();
            engine.getListenerEngine().addChannel("callumc");
            engine.getListenerEngine().startListener(true);
        }
    }
}
