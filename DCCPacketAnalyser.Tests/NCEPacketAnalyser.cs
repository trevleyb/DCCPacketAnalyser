using System.Diagnostics;
using System.IO.Ports;
using DCCPacketAnalyser.Analyser;
using DCCPacketAnalyser.Analyser.Base;

namespace DCCPacketAnalyser.Tests;

public class NCEPacketAnalyser {

    public delegate void PacketAnalysedEvent(IPacketMessage packetMessage);
    public event PacketAnalyser.PacketAnalysedEvent? PacketAnalysed;

    private          string        _lastMessage     = string.Empty;
    private readonly Queue<string> _messageQueue    = new();
    private          string        _bufferRemainder = string.Empty;

    public void Run() {
        var cts = new CancellationTokenSource();
        var result = ReadSerialAndProcessMessages(cts);
        
    }

    
    private Task ReadSerialAndProcessMessages (CancellationTokenSource cts) {
        using var serialPort = new SerialPort();
        serialPort.PortName = "/dev/tty.usbserial-11420";
        serialPort.BaudRate = 38400;
        serialPort.Parity   = Parity.None;
        serialPort.DataBits = 8;
        serialPort.StopBits = StopBits.One;
        serialPort.NewLine  = "0x0D";
        try {
            serialPort.Open();
            
            // Only need to do send these the first time as with NCE it keeps these values
            // -----------------------------------------------------------------------------
            SendPacketAnalyzerCommand(serialPort, "H2"); // We need Verbose Mode or Hex Mode? 
            SendPacketAnalyzerCommand(serialPort, "A+"); // We want Accessory commands
            SendPacketAnalyzerCommand(serialPort, "I-"); // We do not want IDLE commands
            SendPacketAnalyzerCommand(serialPort, "L+"); // We want Loco commands
            SendPacketAnalyzerCommand(serialPort, "R+"); // We do not need RESET commands
            SendPacketAnalyzerCommand(serialPort, "S+"); // We want Signal commands
            
            var packetAnalyser = new PacketAnalyser();
            packetAnalyser.PacketAnalysed += message => PacketAnalysed?.Invoke(message);
            Debug.WriteLine("Reading Packets from Analyzer.");

            var cancellationToken = cts.Token;
            _ = Task.Run(() => {
                Debug.WriteLine("Press Q to quit.");
                while (!cancellationToken.IsCancellationRequested) {
                    if (Console.ReadKey().Key == ConsoleKey.Q) {
                        Debug.WriteLine("Q pressed: Quiting");
                        cts.Cancel();
                    }
                }
            }, cancellationToken);
            
            // Start the main loop
            while (!cancellationToken.IsCancellationRequested) {
                if (serialPort.BytesToRead > 0) {
                    AddToQueue(serialPort.ReadExisting());
                }
                ProcessQueue(packetAnalyser);
            }
        } catch (Exception ex) {
            Debug.WriteLine(ex.Message);
        } finally {
            serialPort.Close();
        }
        Debug.WriteLine("Finished.");
        return Task.CompletedTask;
    }

    private void ProcessQueue(PacketAnalyser packetAnalyser) {
        foreach (var message in GetQueuedMessages()) {
            if (_lastMessage != message) {
                var decodedMessage = packetAnalyser.Decode(message);
            }
            _lastMessage = message;
        }
    }

    IEnumerable<string> GetQueuedMessages() {
        while (_messageQueue.Any()) {
            yield return _messageQueue.Dequeue();
        }
    }

    
    private void AddToQueue(string buffer) {
        var delimiters = new[] { "\r", "\n", "\r\n", "\n\r" };
        buffer = _bufferRemainder + buffer;
        var parts = buffer.Split(delimiters, StringSplitOptions.None);

        // If the last part does not end with a newline, store it for the next call
        if (!buffer.EndsWith("\r") && !buffer.EndsWith("\n")) {
            _bufferRemainder = parts[^1];
            parts            = parts.Take(parts.Length - 1).ToArray();
        } else {
            _bufferRemainder = string.Empty;
        }

        foreach (var part in parts) {
            if (!string.IsNullOrEmpty(part)) {
                _messageQueue.Enqueue(part);
            }
        }
    }
    
    private static void SendPacketAnalyzerCommand(SerialPort serialPort, string command) {

        //A[+/-] Accessory packets on/off
        //H[0-7] Hex        mode 0-7 0=ICC mode 
        //I[+/-] idle       packets on/off     
        //L[+/-] Locomotive pkts on/off  
        //R[+/-] Resets  
        //S[+/-] Signal packets on/off   
        //V             Verbose mode 

        // We need to send multiple time, like 10 or 20, to get it to take the command
        // so loop and send. Testing shows 50ms between sends and about 50 sends. 
        Debug.WriteLine($"Sending Setting: {command} to Adapter.");
        for (var i = 0; i < 20; i++) {
            serialPort.WriteLine(command);
            Thread.Sleep(50); // Wait 100ms and send again 
        }
    }
}