using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveCoding.Backend.Types {
    public class Streamer {
        public string Username { get; private set; }

        public Streamer(string StreamerName) { // LOOKS LIKE ONE ARG TO ME....
            this.Username = StreamerName;
        }
    }
}
