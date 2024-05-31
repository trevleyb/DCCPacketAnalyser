using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Base;

public interface IPacketMessage {
    PacketData      PacketData  { get; set; } 
    int             Address     { get; set; }
    AddressTypeEnum AddressType { get; set; }

    void ProcessRemainingPacket();
}