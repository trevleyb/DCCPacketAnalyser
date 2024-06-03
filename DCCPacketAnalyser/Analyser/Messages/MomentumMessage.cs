using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class MomentumMessage(IPacketMessage packet) : PacketMessage(packet.PacketData, packet.AddressType, packet.Address) {
    
    public MomentumTypeEnum Type  { get; set; }
    public byte             Value { get; set; }       
    
    public MomentumMessage(IPacketMessage packet, MomentumTypeEnum type, byte value) : this(packet) {
        Type = type;
        Value = value;
    }
    public override string ToString() {
        return $"MOMENTUM: Address={Address} \t[{PacketData.ToBinary}]";
    }
}

public enum MomentumTypeEnum {
    Acceleration = 23, 
    Deceleration = 24,
    Lock = 0
}