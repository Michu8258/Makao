using System.Runtime.Serialization;

namespace MakaoGameClientService.Messages
{
    //return data for start game request from host
    [DataContract]
    public class UpdatingGameStatusResponse
    {
        [DataMember]
        public string PlayerID { get; set; }

        [DataMember]
        public int PlayerNumber { get; set; }

        [DataMember]
        public bool Done { get; set; }
    }
}
