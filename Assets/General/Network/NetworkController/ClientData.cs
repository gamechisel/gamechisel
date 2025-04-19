using Unity.Netcode;

[System.Serializable]
public struct ClientData : INetworkSerializable, System.IEquatable<ClientData>
{
    public ulong ClientId;
    public int Ping;
    public bool IsHost;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            // The complex type handles its own de-serialization
            // serializer.SerializeValue(ref ApplyWeaponBooster);
            // Now de-serialize the non-complex type properties
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out ClientId);
            reader.ReadValueSafe(out Ping);
            reader.ReadValueSafe(out IsHost);
        }
        else
        {
            // The complex type handles its own serialization
            // serializer.SerializeValue(ref ApplyWeaponBooster);
            // Now serialize the non-complex type properties
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(ClientId);
            writer.WriteValueSafe(Ping);
            writer.WriteValueSafe(IsHost);
        }
    }

    public bool Equals(ClientData other)
    {
        return ClientId == other.ClientId;
    }
}