using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class ConfigCVMessage(IPacketMessage packet) : PacketMessage(packet.PacketData, packet.AddressType, packet.Address) {
    
    public ConfigCVTypeEnum Type { get; set; }
    public int CvNumber          { get; set; }
    public byte CvValue          { get; set; }
    
    public ConfigCVMessage(IPacketMessage packet, ConfigCVTypeEnum type, int cvNumber, byte cvValue) : this(packet) {
        Type = type;
        CvNumber = cvNumber;
        CvValue = cvValue;
    }
    
    public override string ToString() {
        return $"CONFIG(CV): Address={Address} \t[{PacketData.ToBinary}]";
    }
}

public enum ConfigCVTypeEnum {
    Verify, 
    Write,
    BitManipulate
}