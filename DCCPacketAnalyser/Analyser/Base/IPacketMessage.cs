namespace DCCPacketAnalyser.Analyser.Base;

public interface IPacketMessage {
    int             Address     { get; }
    AddressTypeEnum AddressType { get; }
    PacketData      PacketData  { get; }
    string          Detailed    { get; }
    string          Summary     { get; }
}