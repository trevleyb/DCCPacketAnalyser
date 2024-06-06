using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class SpeedStepsMessage(IPacketMessage packet) : PacketMessage(packet.PacketData, packet.AddressType, packet.Address), IEquatable<SpeedStepsMessage> {
    private readonly byte           _speedStepsData;
    public           SpeedStepsEnum SpeedSteps      { get; init; }
    public           bool           RestrictedSpeed { get; init; }

    public byte SpeedStepsData {
        get => _speedStepsData;
        init {
            _speedStepsData = value;
            SpeedSteps      = (SpeedStepsEnum)(value & 0b00000111);
        }
    }

    public override string Summary => $"{AddressAsString} {SpeedSteps}";

    public override string ToString() {
        return FormatHelper.FormatMessage("SPEED STEPS", base.ToString(), PacketData, ("Steps", SpeedSteps.ToString()), ("as", _speedStepsData));
    }

    public bool Equals(SpeedStepsMessage? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Address != other.Address) return false;
        return _speedStepsData == other._speedStepsData && SpeedSteps == other.SpeedSteps && RestrictedSpeed == other.RestrictedSpeed;
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((SpeedStepsMessage)obj);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Address, _speedStepsData, (int)SpeedSteps, RestrictedSpeed);
    }
}

public enum SpeedStepsEnum {
    Steps14  = 14,
    Steps28  = 28,
    Steps128 = 128
}