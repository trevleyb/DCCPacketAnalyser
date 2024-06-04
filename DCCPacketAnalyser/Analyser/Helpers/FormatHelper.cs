using System.Text;
using DCCPacketAnalyser.Analyser.Base;

namespace DCCPacketAnalyser.Analyser.Helpers;

public static class FormatHelper {
    public static string FormatMessage(string msgType, string address, PacketData packetData, params (string name, object value)[] properties) {
        var sb = new StringBuilder();
        sb.Append($"{msgType,12}: ");
        sb.Append($"Address={address} ");
        foreach (var property in properties) sb.Append($"{property.name}={property.value ?? "?"} ");
        sb.Append($" [{packetData.ToBinary}]");
        return sb.ToString();
    }
}