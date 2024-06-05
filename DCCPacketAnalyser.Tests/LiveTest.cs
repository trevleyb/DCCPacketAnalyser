using System.Diagnostics;
using System.IO.Ports;
using DCCPacketAnalyser;
using DCCPacketAnalyser.Analyser;
using DCCPacketAnalyser.Analyser.Helpers;
using NUnit.Framework;

namespace DCCPacketAnalyser.Tests;

[TestFixture]
public class LiveTest {
    [Test]
    [Ignore("Skip as only for live testing")]
    public void FullRunTest() {
        var analyser = new NCEPacketAnalyser();
        analyser.PacketAnalysed += message => Debug.WriteLine(message.ToString());
        analyser.Run();
    }
}