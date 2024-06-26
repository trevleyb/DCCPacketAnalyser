namespace DCCPacketAnalyser.Analyser.Base;

public class PacketMessage(PacketData packetData, AddressTypeEnum addressType, int address) : IPacketMessage {
    // Only used for Testing
    internal PacketMessage(PacketData packetData) : this(packetData, AddressTypeEnum.Unknown, 0) {
        PacketData = packetData;
    }

    public PacketData      PacketData  { get; } = packetData;
    public int             Address     { get; } = address;
    public AddressTypeEnum AddressType { get; } = addressType;

    public virtual string Detailed => ToString();
    public virtual string Summary  => AddressAsString;

    public override string ToString() {
        return $"{AddressAsString}";
    }

    protected internal string AddressAsString =>
        AddressType switch {
            AddressTypeEnum.Short     => $"S{Address:D4}",
            AddressTypeEnum.Long      => $"L{Address:D4}",
            AddressTypeEnum.Broadcast => $"B{Address:D4}",
            AddressTypeEnum.Accessory => $"A{Address:D4}",
            AddressTypeEnum.Signal    => $"G{Address:D4}",
            _                         => $"X{Address:D4}"
        };
}

public enum AddressTypeEnum {
    Idle      = 'I',
    Broadcast = 'B',
    Short     = 'S',
    Long      = 'L',
    Accessory = 'A',
    Signal    = 'G',
    Error     = '?',
    Duplicate = '!',
    Unknown   = 'U'
}