using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

/// <summary>
/// Represents if we got an error working out the packet data. The message indicates
/// why we had an error and the original data is included. 
/// </summary>
/// <param name="message">A message representing the error</param>
/// <param name="packet">The original packet data</param>
public class ErrorPacket : PacketMessage, IPacketMessage {
    public string Message { get; set; } = string.Empty;

    public ErrorPacket(PacketData packetData, string message) : base(packetData) {
        Address     = 0;
        AddressType = AddressTypeEnum.Error;
        Message     = message;
    }

    public override string ToString() {
        return $"ERROR: Message='{Message}'";
    }

    public void ProcessRemainingPacket() { }
}