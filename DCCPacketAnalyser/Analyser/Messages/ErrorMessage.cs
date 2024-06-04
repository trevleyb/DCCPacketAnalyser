using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

/// <summary>
/// Represents if we got an error working out the packet data. The message indicates
/// why we had an error and the original data is included. 
/// </summary>
public class ErrorMessage : PacketMessage, IPacketMessage, IEquatable<ErrorMessage> {
    public string Message { get; init; }

    public ErrorMessage(IPacketMessage packetMessage, string message) : base(packetMessage.PacketData, packetMessage.AddressType, packetMessage.Address) {
        Message     = message;
    }

    public ErrorMessage(PacketData packetData, string message) : base(packetData, AddressTypeEnum.Error, 0) {
        Message     = message;
    }

    public override string ToString() {
        return FormatHelper.FormatMessage("ERROR", base.ToString(), PacketData, ("Message",Message) );
    }

    public bool Equals(ErrorMessage? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Message == other.Message;
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ErrorMessage)obj);
    }

    public override int GetHashCode() {
        return Message.GetHashCode();
    }
}