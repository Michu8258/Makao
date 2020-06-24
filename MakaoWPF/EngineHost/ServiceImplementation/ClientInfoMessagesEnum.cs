namespace EngineHost.ServiceImplementation
{
    public enum ClientInfoType
    {
        PlayersDataChanged,
        ClosedByHost,
        PlayersReadinessChanged,
        JoiningTimeout,
        ReadinessTimeout,
        CheckAliveness,
        LostConnectionToClient,
        PlayerLeftGame,
    }
}
