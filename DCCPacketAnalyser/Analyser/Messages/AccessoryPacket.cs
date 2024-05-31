using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class AccessoryPacket : PacketMessage, IPacketMessage {
    public AccessoryStateEnum State { get; set; }

    public AccessoryPacket(PacketData packetData, int address) : base(packetData) {
        Address = address;
        AddressType = AddressTypeEnum.Accessory;
    }

    public override string ToString() => $"ACCESSORY: Address={Address} State={State}";

    public void ProcessRemainingPacket() {
        var dataByte = PacketData.Next();
        if (dataByte.GetBit(7)) { // Basic Accessory Decoder
            var activated = dataByte.GetBit(3);
            var newAddress = (((dataByte >> 4) & 0b00000111) << 6) | Address;
            var output = dataByte.GetBit(0);
            var outputPair = ((dataByte & 0b00000110) >> 1);
            State = dataByte.GetBit(0) ? AccessoryStateEnum.Normal : AccessoryStateEnum.Reversed;
        } else {
            throw new Exception("We really should not be here. This should be have been an Signal not Accessory.");
        }
    }
}