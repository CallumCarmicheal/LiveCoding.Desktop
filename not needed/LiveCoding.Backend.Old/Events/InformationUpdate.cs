using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveCoding.Backend.Events {
    public class ReportEvent {
        public Information.Report Report { get; private set; }

        public ReportEvent (Information.Report data) {
            Report = data;
        } 
    }
}
