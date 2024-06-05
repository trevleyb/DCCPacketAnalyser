using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class ConfigCvMessage(IPacketMessage packet) : PacketMessage(packet.PacketData, packet.AddressType, packet.Address), IEquatable<ConfigCvMessage> {
    public ConfigCvTypeEnum Type     { get; }
    public int              CvNumber { get; }
    public byte             CvValue  { get; }

    public ConfigCvMessage(IPacketMessage packet, ConfigCvTypeEnum type, int cvNumber, byte cvValue) : this(packet) {
        Type     = type;
        CvNumber = cvNumber;
        CvValue  = cvValue;
    }

    public override string Summary => $"{AddressAsString} CV{CvNumber}={CvValue}";

    public override string ToString() {
        return FormatHelper.FormatMessage("CONFIG(CV)", base.ToString(), PacketData, ("Operation", Type.ToString()), ("CvNumber", CvNumber), ("CvValue", CvValue));
    }

    public bool Equals(ConfigCvMessage? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Address != other.Address) return false;
        return Type == other.Type && CvNumber == other.CvNumber && CvValue == other.CvValue;
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((ConfigCvMessage)obj);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Address, (int)Type, CvNumber, CvValue);
    }
}

public enum ConfigCvTypeEnum {
    Verify,
    Write,
    BitManipulate
}