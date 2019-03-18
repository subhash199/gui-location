using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Location
{
    public class ClientInfo
    {
        public string _host;
        public int _port;
        public string _userName;
        public string _location;
        public string _http;

        public ClientInfo(string host, int port, string userName, string location, string http)
        {
            this._host = host;
            this._port = port;
            this._userName = userName;
            this._location = location;
            this._http = http;
        }

    }

  
}
