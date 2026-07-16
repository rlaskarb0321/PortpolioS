using Fusion;

[System.Serializable]
public struct PlayableCharacterInfo : INetworkStruct
{
    public NetworkString<_16> name;
    public int starCount;
}