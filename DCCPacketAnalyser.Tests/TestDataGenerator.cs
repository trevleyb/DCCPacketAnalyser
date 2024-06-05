using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using DCCPacketAnalyser;
using DCCPacketAnalyser.Analyser;
using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;
using DCCPacketAnalyser.Analyser.Messages;
using NUnit.Framework;

namespace DCCPacketAnalyser.Tests;

[TestFixture]
public class TestDataGenerator {
    private Dictionary<Type, Dictionary<int, IPacketMessage>> _data = new();

    [Test]
    [Ignore("Skip as only for live testing")]
    public void Run() {
        var analyser = new NCEPacketAnalyser();
        analyser.PacketAnalysed += AnalyserOnPacketAnalysed;
        analyser.Run();
        DumpTestData();
    }

    private void DumpTestData() {
        foreach (var msgType in _data) {
            Debug.WriteLine("MSG=>" + msgType.Key.ToString());
            foreach (var packetMessage in msgType.Value) {
                var sb = new StringBuilder();
                AppendTestData(sb, packetMessage.Value.PacketData);

                switch (packetMessage.Value) {
                case ConfigCvMessage configCvMessage:
                    // [TestCase(new byte[] { 0x81, 0x71, 0x0A, 0xFA }, typeof(ConfigCVMessage), 1, 10, 10)]
                    sb.Append("typeof(ConfigCVMessage), ");
                    sb.Append(configCvMessage.Address + ", ");
                    sb.Append(configCvMessage.CvNumber + ", ");
                    sb.Append(configCvMessage.CvValue + ")]");
                    break;
                case AccessoryMessage accessoryMessage:
                    // [TestCase(new byte[] { 0x81, 0x71, 0x0A, 0xFA }, typeof(AccessoryMessage), 1, 10)]
                    sb.Append("typeof(AccessoryMessage), ");
                    sb.Append(accessoryMessage.Address + ", ");
                    sb.Append("AccessoryStateEnum." + accessoryMessage.State + ")]");
                    break;
                case SpeedAndDirectionMessage speedAndDirectionMessage:
                    // [TestCase(new byte[] { 0x81, 0x71, 0x0A, 0xFA }, typeof(SpeedAndDirectionMessage), 1, 1, DirectionEnum.Stop)]
                    sb.Append("typeof(SpeedAndDirectionMessage), ");
                    sb.Append(speedAndDirectionMessage.Address + ", ");
                    sb.Append(speedAndDirectionMessage.Speed + ", ");
                    sb.Append("DirectionEnum." + speedAndDirectionMessage.Direction + ")]");
                    break;
                case SignalMessage signalMessage:
                    // [TestCase(new byte[] { 0x81, 0x71, 0x0A, 0xFA }, typeof(SignalMessage), 1, SignalAspectEnums.Red)]
                    sb.Append("typeof(SignalMessage), ");
                    sb.Append(signalMessage.Address + ", ");
                    sb.Append("SignalAspectEnums." + signalMessage.Aspect + ")]");
                    break;
                case FunctionsMessage functionMessage:
                    // [TestCase(new byte[] { 0x81, 0x71, 0x0A, 0xFA }, typeof(FunctionsMessage),1, FunctionsGroupEnum.F0F4, 0)]
                    sb.Append("typeof(FunctionsMessage), ");
                    sb.Append(functionMessage.Address + ", ");
                    sb.Append("FunctionsGroupEnum." + functionMessage.Group + ", ");
                    sb.Append(functionMessage.BitValues + ")]");
                    break;
                }

                sb.AppendLine();
                Debug.WriteLine(sb.ToString());
            }
        }
    }

    private void AppendTestData(StringBuilder sb, PacketData data) {
        var first = true;
        sb.Append("[TestCase(new byte[] { ");
        for (var i = 0; i < data.Elements; i++) {
            if (!first) sb.Append(", ");
            sb.Append($" 0x{data.GetAt(i):X2}");
            first = false;
        }

        sb.Append(" }, ");
    }

    // Build up a set of UNIQUE messages that have been decomposed.
    // --------------------------------------------------------------------------
    private void AnalyserOnPacketAnalysed(IPacketMessage packetMessage) {
        Debug.WriteLine(packetMessage.ToString());
        if (!_data.ContainsKey(packetMessage.GetType())) {
            _data.Add(packetMessage.GetType(), new Dictionary<int, IPacketMessage>());
        }

        if (!_data[packetMessage.GetType()].ContainsKey(packetMessage.GetHashCode())) {
            _data[packetMessage.GetType()].Add(packetMessage.GetHashCode(), packetMessage);
        }
    }
}