using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class SignalMessage(IPacketMessage packet, SignalAspectEnums aspect) : PacketMessage(packet.PacketData, AddressTypeEnum.Signal, packet.Address), IEquatable<SignalMessage> {
    public SignalAspectEnums Aspect { get; init; } = aspect;

    public override string Summary => $"{AddressAsString} {Aspect}"; 
    public override string ToString() {
        return FormatHelper.FormatMessage("SIGNAL", base.ToString(), PacketData, ("Aspect",Aspect.ToString()) );
    }

    public bool Equals(SignalMessage? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Address != other.Address) return false;
        return Aspect == other.Aspect;
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((SignalMessage)obj);
    }

    public override int GetHashCode() {
        return HashCode.Combine((int)Address, (int) Aspect);
    }
}

public enum SignalAspectEnums {
    Red              = 0,
    Yellow           = 1,
    Green            = 2,
    FlashRed         = 3,
    FlashYellow      = 4,
    FlashGreen       = 5,
    RedYellow        = 6,
    FlashRedYellow   = 7,
    RedFlashYellow   = 8,
    RedGreen         = 9,
    FlashRedGreen    = 10,
    RedFlashGreen    = 11,
    YellowGreen      = 12,
    FlashYellowGreen = 13,
    YellowFlashGreen = 14,
    AllOn            = 15,
    AllFlash         = 30,
    AllOff           = 31
}