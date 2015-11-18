using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace LiveCoding.Backend {

    public class Engine {
        public Html.Engine htmlEngine;
        public int engineWaitTime = 1000;

        public event EventHandler<Events.EngineUpdate> UpdateEvent;
        public event EventHandler<Events.ReportEvent> ReportEvent;

        public EngineMode engineState { get; private set; }
        public string User { get; set; }

        public Engine() {
            htmlEngine = new Html.Engine(this, engineWaitTime);
        }

        /// <summary>
        /// Create and tell the engine we want to monitor the users account
        /// </summary>
        /// <param name="userAccount">This will be the creditentials for the account</param>
        /// /// <returns>A created engine used for handling the requests on the account</returns>
        public static Engine CreateEngine(Types.UserAccount userAccount, int WaitTime = 1000) {
            Engine eng = new Engine();
            eng.engineState = EngineMode.Account;
            eng.User = userAccount.Username;
            eng.engineWaitTime = WaitTime;

            System.Diagnostics.Debug.WriteLine("Created Engine to handle streamer account");
            return eng;
        }

        /// <summary>
        /// Create and tell the engine we want to monitor someones stream
        /// </summary>
        /// <param name="streamer">The streamers username to monitor chat and concur-viewers</param>
        /// <returns>A created engine used for handling the requests on the streamer</returns>
        public static Engine CreateEngine(Types.Streamer streamer, int WaitTime = 1000) {
            Engine eng = new Engine();
            eng.engineState = EngineMode.Viewer;
            eng.User = streamer.Username;
            eng.engineWaitTime = WaitTime;

            System.Diagnostics.Debug.WriteLine("Created Engine to view streamer information");
            return eng;
        }

        public void ThrowUpdateEvent(Events.EngineUpdate evt) {
            if (UpdateEvent != null)
                UpdateEvent(this, evt);
        }

        public void ThrowReportEvent(Events.ReportEvent evt) {
            if (ReportEvent != null)
                ReportEvent(this, evt);
        }
    }

    public enum EngineMode {
        Viewer,
        Account
    }

}
