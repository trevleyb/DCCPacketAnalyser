using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class SignalPacket : PacketMessage, IPacketMessage {
    public SignalAspectEnums Aspect { get; set; }
    public SignalPacket(PacketData packetData, int address) : base(packetData) {
        Address     = address;
        AddressType = AddressTypeEnum.Signal;
        Aspect      = SignalAspectEnums.AllOff;
    }
    
    public override string ToString() => $"SIGNAL: Address={Address} Aspect={Aspect}";

    public void ProcessRemainingPacket() {
        var dataByte = PacketData.GetNext();
        if (!dataByte.GetBit(7)) { // Basic Accessory Decoder
            Aspect = (SignalAspectEnums)(byte)(dataByte & 0b00011111);
        } else {
            throw new Exception("We really should not be here. This should be have been an Accessory not Signal.");
        }
    }
}