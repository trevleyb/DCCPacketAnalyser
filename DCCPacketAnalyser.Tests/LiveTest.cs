using System.Diagnostics;

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