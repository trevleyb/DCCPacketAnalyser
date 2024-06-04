using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class ConfigCVMessage(IPacketMessage packet) : PacketMessage(packet.PacketData, packet.AddressType, packet.Address), IEquatable<ConfigCVMessage> {
    
    public ConfigCVTypeEnum Type { get; init; }
    public int CvNumber          { get; init; }
    public byte CvValue          { get; init; }
    
    public ConfigCVMessage(IPacketMessage packet, ConfigCVTypeEnum type, int cvNumber, byte cvValue) : this(packet) {
        Type = type;
        CvNumber = cvNumber;
        CvValue = cvValue;
    }
    
    public override string Summary => $"{AddressAsString} CV{CvNumber}={CvValue}"; 
    public override string ToString() {
        return FormatHelper.FormatMessage("CONFIG(CV)", base.ToString(), PacketData, ("Operation",Type.ToString()),("CvNumber", CvNumber), ("CvValue", CvValue) );
    }

    public bool Equals(ConfigCVMessage? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Address != other.Address) return false;
        return Type == other.Type && CvNumber == other.CvNumber && CvValue == other.CvValue;
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ConfigCVMessage)obj);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Address, (int)Type, CvNumber, CvValue);
    }
}

public enum ConfigCVTypeEnum {
    Verify, 
    Write,
    BitManipulate
}