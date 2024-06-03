using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class SpeedStepsMessage (IPacketMessage packet) : PacketMessage(packet.PacketData, packet.AddressType, packet.Address) {
    private byte           _speedStepsData;
    public  SpeedStepsEnum SpeedSteps      { get; set; }
    public  bool           RestrictedSpeed { get; set; }

    public byte SpeedStepsData {
        get => _speedStepsData;
        set {
            _speedStepsData = value; 
            SpeedSteps      = (SpeedStepsEnum) (value & 0b00000111);
        }
    }

    
    public override string ToString() {
        return $"SPEEDSTEPS: Address={Address} \t[{PacketData.ToBinary}]";
    }
}