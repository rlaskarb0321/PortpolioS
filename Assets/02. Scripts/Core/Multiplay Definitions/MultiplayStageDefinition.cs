using Fusion;

[System.Serializable]
public struct MultiplayStageDefinition : INetworkStruct
{
    public NetworkString<_16> sessionName;
    public int stageIndex;
    public EMultiplayType multiplayMapType;
    public int multiplayMapTypeIndex;
    public NetworkString<_16> bgmAddress;
}