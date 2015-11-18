using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveCoding.Backend.Types {
    public class UserAccount {

        public string Username { get; private set; }
        public string Password { get; private set; }

        public UserAccount(string Username, string Password) {
            this.Username = Username;
            this.Password = Password;
        }
    }
}
