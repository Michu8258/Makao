using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineHost.GameStateUpdatesSending
{
    public class ReturnData
    {
        public string PlayerID { get; set; }
        public int PlayerNumber { get; set; }
        public object Response { get; set; }
    }
}
