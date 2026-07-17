using Fusion;

[System.Serializable]
public struct UserData : INetworkStruct
{
    public NetworkString<_16> nickName;
    public int mainCharacterIndex;
    public int mainCharacterSkinIndex;
}