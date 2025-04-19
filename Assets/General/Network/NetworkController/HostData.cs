using Unity.Collections;
using Unity.Netcode;

[System.Serializable]
public struct HostData : INetworkSerializable
{
    public ushort Port;
    public FixedString32Bytes LocalIP;
    public FixedString32Bytes PublicIP;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out Port);
            reader.ReadValueSafe(out LocalIP);
            reader.ReadValueSafe(out PublicIP);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(Port);
            writer.WriteValueSafe(LocalIP);
            writer.WriteValueSafe(PublicIP);
        }
    }
}
