using Fusion;

[System.Serializable]
public struct PlayableCharacterInfo : INetworkStruct
{
    public int index;
    public NetworkString<_16> name;
    public int starCount;
}