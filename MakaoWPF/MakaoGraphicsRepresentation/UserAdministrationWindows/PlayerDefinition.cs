using Realms;

namespace MakaoGraphicsRepresentation.UserAdministrationWindows
{
    public class PlayerDefinition : RealmObject
    {
        public int ID { get; set; }
        public string PlayerName { get; set; }
        public string PlayerPassword { get; set; }
        public string PlayerLogin { get; set; }
    }
}
