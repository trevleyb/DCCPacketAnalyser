using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Base;

public class PacketMessage(PacketData packetData, AddressTypeEnum addressType, int address) : IPacketMessage {
    public PacketData      PacketData  { get; } = packetData;
    public int             Address     { get; protected init; } = address;
    public AddressTypeEnum AddressType { get; protected init; } = addressType;
    
    public override string ToString() {
        return $"{address:D4}{(char)AddressType}";
    }

}


public enum AddressTypeEnum {
    Idle        = 'I',
    Broadcast   = 'B',
    Short       = 'S',
    Long        = 'L',
    Accessory   = 'A',
    Signal      = 'G',
    Error       = '?',
    Duplicate   = '!'
}
