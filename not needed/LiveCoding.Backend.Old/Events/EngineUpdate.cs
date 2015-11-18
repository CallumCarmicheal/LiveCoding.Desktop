using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveCoding.Backend.Events {
    public class EngineUpdate {
        public EngineUpdateState Mode { get; private set; }
        public object ReportData { get; private set; }

        public EngineUpdate(EngineUpdateState throwCause, object data) {
            Mode = throwCause;
            ReportData = data;
        } 
    }

    public enum EngineUpdateState {
        StatusUpdate
    }
}
