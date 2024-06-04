using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

/// <summary>
/// Represents if we got an error working out the packet data. The message indicates
/// why we had an error and the original data is included. 
/// </summary>
/// <param name="message">A message representing the error</param>
/// <param name="packet">The original packet data</param>
public class ErrorMessage : PacketMessage, IPacketMessage {
    public string Message { get; set; }

    public ErrorMessage(IPacketMessage packetMessage, string message) : base(packetMessage.PacketData, packetMessage.AddressType, packetMessage.Address) {
        Message     = message;
    }

    public ErrorMessage(PacketData packetData, string message) : base(packetData, AddressTypeEnum.Error, 0) {
        Message     = message;
    }

    public override string ToString() {
        return FormatHelper.FormatMessage("ERROR", base.ToString(), PacketData, ("Message",Message) );
    }

    public void ProcessRemainingPacket() { }
}