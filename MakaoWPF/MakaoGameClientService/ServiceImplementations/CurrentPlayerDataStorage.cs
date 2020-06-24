using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakaoGameClientService.ServiceImplementations
{
    public static class CurrentPlayerDataStorage
    {
        public static string PlayerName { get; set; }
        public static int PlayerNumber { get; set; }
        public static string PlayerID { get; set; }
    }
}
