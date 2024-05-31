using System.Text;
using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class DecoderPacket : PacketMessage, IPacketMessage {

    public SpeedStepsEnum         SpeedSteps         { get; private set; }
    public DecoderMessageTypeEnum DecoderMessageType { get; private set; }
    public DirectionEnum          Direction          { get; private set; }
    public int                    ConsistAddress     { get; private set; }
    public byte                   Speed              { get; private set; }
    public bool                   RestrictedSpeed    { get; private set; }
    public byte                   SpeedStepsData     { get; private set; }
    public bool[]                 Functions          { get; private set; } = new bool[69];
    public int                    CvNumber           { get; private set; }
    public byte                   CvValue            { get; private set; }
    public bool                   DecoderLock        { get; private set; }
    
    public DecoderPacket(PacketData packetData, AddressTypeEnum addressType, int address) : base(packetData) {
        Address            = address;
        AddressType        = addressType;
        SpeedSteps         = SpeedStepsEnum.Unknown;
        DecoderMessageType = DecoderMessageTypeEnum.None;
        Direction          = DirectionEnum.Unknown;
        ConsistAddress     = 0;
        Speed              = 0;
        RestrictedSpeed    = false;
        SpeedStepsData     = 0;
        for (var i = 0; i < 69; i++) Functions[i] = false;
    }

    public override string ToString() {
        var message = new StringBuilder();
        message.Append("DECODER: Address='");
        if (AddressType == AddressTypeEnum.Broadcast) message.Append("Broadcast(0)");
        if (AddressType == AddressTypeEnum.Long) message.Append($"{Address}L");
        if (AddressType == AddressTypeEnum.Short) message.Append($"{Address}S");
        message.Append("' ");

        message.Append("MESSAGE:");
        message.Append(DecoderMessageType switch {
            DecoderMessageTypeEnum.Error               => $"ERROR - Invalid data in packet.",
            DecoderMessageTypeEnum.None                => $"None",
            DecoderMessageTypeEnum.Reserved            => $"Reserved",
            DecoderMessageTypeEnum.DecoderAck          => $"Decoder Acknowledge",
            DecoderMessageTypeEnum.DecoderFlags        => $"Decoder Flags",
            DecoderMessageTypeEnum.AdvancedAddressing  => $"Advanced Addressing",
            DecoderMessageTypeEnum.Reset               => $"Reset",
            DecoderMessageTypeEnum.HardReset           => $"Hard Reset",
            DecoderMessageTypeEnum.FactoryTest         => $"Factory Test",
            DecoderMessageTypeEnum.ConsistControl      => $"Consist Address='{ConsistAddress}'",
            DecoderMessageTypeEnum.SpeedAndDirection   => $"Speed='{Speed}' in '{Direction}'",
            DecoderMessageTypeEnum.SpeedStepControl    => $"SpeedSteps='{SpeedSteps}'",
            DecoderMessageTypeEnum.RestrictedSpeedStep => $"Restricted Speed is '{(RestrictedSpeed ? "ON" : "OFF")}'",
            DecoderMessageTypeEnum.BinaryStateLong     => $"Binary State (L)",
            DecoderMessageTypeEnum.BinaryStateShort    => $"Binary State (L)",
            DecoderMessageTypeEnum.FunctionsF0F4       => $"Function F00-F04 : {OutputFunctions(0,4)}",
            DecoderMessageTypeEnum.FunctionsF5F8       => $"Function F05-F08 : {OutputFunctions(5,8)}",
            DecoderMessageTypeEnum.FunctionsF9F12      => $"Function F09-F12 : {OutputFunctions(9,12)}",
            DecoderMessageTypeEnum.FunctionsF13F20     => $"Function F13-F20 : {OutputFunctions(13,20)}",
            DecoderMessageTypeEnum.FunctionsF21F28     => $"Function F21-F28 : {OutputFunctions(21,28)}",
            DecoderMessageTypeEnum.FunctionsF29F36     => $"Function F29-F36 : {OutputFunctions(29,36)}",
            DecoderMessageTypeEnum.FunctionsF37F44     => $"Function F37-F44 : {OutputFunctions(37,44)}",
            DecoderMessageTypeEnum.FunctionsF45F52     => $"Function F45-F52 : {OutputFunctions(45,52)}",
            DecoderMessageTypeEnum.FunctionsF53F60     => $"Function F53-F60 : {OutputFunctions(53,60)}",
            DecoderMessageTypeEnum.FunctionsF61F68     => $"Function F61-F68 : {OutputFunctions(61,68)}",
            DecoderMessageTypeEnum.Acceleration        => $"Acceleration set to '{CvValue}",
            DecoderMessageTypeEnum.Deceleration        => $"Deceleration set to '{CvValue}",
            DecoderMessageTypeEnum.Lock                => $"Lock CV   '{CvNumber}' is '{CvValue}'",
            DecoderMessageTypeEnum.VerifyCv            => $"Verify CV '{CvNumber}' is '{CvValue}'",
            DecoderMessageTypeEnum.WriteCv             => $"Write  CV '{CvNumber}' is '{CvValue}'",
            DecoderMessageTypeEnum.BitManipulate       => $"Bit Manipulation ",
            _                                          => "Unknown"
        });
        return message.ToString();
    }

    /// <summary>
    /// Function to reformat the Functions so they can be displayed. Format as X-X-X-X- where X is On, - is OFF
    /// </summary>
    /// <param name="fromF">First Function Number (0)</param>
    /// <param name="toF">Last Function Number (68)</param>
    /// <returns>A string representing the state of the functions. </returns>
    private string OutputFunctions(int fromF, int toF) {
        if (fromF <= 0 || fromF > Functions.Length - 1 || toF < 0 || toF > Functions.Length - 1 || toF < fromF) return "--------";
        var functionBlock = "";
        for (var i = fromF; i <= toF; i++) {
            functionBlock += Functions[i] ? "X" : "-";
        }
        return functionBlock;
    }

    /// <summary>
    /// Get the multifunction data from the packet and update the decoded data object.
    /// </summary>
    /// <param name="packet">The packet data</param>
    /// <returns>True if the multifunction data was successfully retrieved and updated, otherwise false</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the offset is larger than the size of the packet</exception>
    public void ProcessRemainingPacket() {

        if (!PacketData.IsAtLeastLength(3)) throw new Exception("Invalid Data to process. Must have at least 3 elements: " + PacketData.ToBinary);

        // Decoder Control is in the form CCCDDDDD where 
        // CCC = command and DDDDD = dfata to apply
        // ------------------------------------------------------------
        var instrByte = (byte)(PacketData.GetCurrent() & 0b11100000) >> 5;
        var dataByte = (byte)(PacketData.GetCurrent() & 0b00011111);

        DecoderMessageType = instrByte switch {
            0b00000000 => DecoderAndConsistControl(dataByte),
            0b00000001 => AdvancedOperationInstructions(dataByte),
            0b00000010 => SpeedAndDirectionForReverse(dataByte),
            0b00000011 => SpeedAndDirectionForForward(dataByte),
            0b00000100 => FunctionGroupOneInstructions(dataByte),
            0b00000101 => FunctionGroupTwoInstructions(dataByte),
            0b00000110 => ExtendedFunctions(dataByte),
            0b00000111 => ConfigurationVariableAccess(dataByte),
            _          => throw new ArgumentOutOfRangeException(null, $"Invalid Instruction byte for MultiFunction Decoder.")
        };
    }

    internal DecoderMessageTypeEnum DecoderAndConsistControl(byte dataByte) {
        // Format: 000CCCF
        // If the first bit of C is 0 it is a decoder control message
        // If the first bit of C is 1 it is a Consist control message
        if (!dataByte.GetBit(4)) {
            var instraByte = (byte)((dataByte & 0b00001110) >> 1);
            var instraData = (byte)((dataByte & 0b00000001));
            DecoderMessageType = instraByte switch {
                0b000 => instraData == 0 ? DecoderMessageTypeEnum.Reset : DecoderMessageTypeEnum.HardReset,
                0b001 => DecoderMessageTypeEnum.FactoryTest,
                0b010 => DecoderMessageTypeEnum.Reserved,
                0b011 => DecoderMessageTypeEnum.DecoderFlags,
                0b100 => DecoderMessageTypeEnum.Reserved,
                0b101 => DecoderMessageTypeEnum.AdvancedAddressing,
                0b110 => DecoderMessageTypeEnum.Reserved,
                0b111 => instraData == 0 ? DecoderMessageTypeEnum.None : DecoderMessageTypeEnum.DecoderAck,
                _     => DecoderMessageTypeEnum.None,
            };
        } else {
            switch (dataByte & 0b00001111) {
            case 0b0010:
                ConsistAddress     = PacketData.GetNext() & 0b01111111;
                Direction          = DirectionEnum.Forward;
                return DecoderMessageTypeEnum.ConsistControl;
            case 0b0011:
                ConsistAddress     = PacketData.GetNext() & 0b01111111;
                Direction          = DirectionEnum.Forward;
                return DecoderMessageTypeEnum.ConsistControl;
            default:
                return DecoderMessageTypeEnum.Error;
            }
        }
        return DecoderMessageTypeEnum.DecoderAndConsist;
    }

    internal DecoderMessageTypeEnum AdvancedOperationInstructions(byte dataByte) {
        switch (dataByte) {
        case 0b00111111: // 128 Speed Step Control
            Speed = (byte)(PacketData.GetNext() & 0b01111111);
            switch (Speed) {
            case 0:
                Direction = DirectionEnum.Stop;
                break;
            case 1:
                Speed -= 1;
                Direction = DirectionEnum.EStop;
                break;
            default:
                Speed -= 1;
                Direction = PacketData.GetNext().GetBit(7) ? DirectionEnum.Forward : DirectionEnum.Reverse;
                break;
            }
            return DecoderMessageTypeEnum.SpeedAndDirection;
        case 0b00111110: // Restricted Speed Step
            SpeedStepsData = (byte)(PacketData.GetNext() & 0b01111111);
            RestrictedSpeed = PacketData.GetCurrent().GetBit(7);
            break;
        case 0b00111101: // Analog Function Group
            break;
        }
        return DecoderMessageTypeEnum.SpeedAndDirection;
    }

    internal DecoderMessageTypeEnum SpeedAndDirectionForReverse(byte dataByte) {
        Speed = (byte)((((dataByte & 0B00001111) << 1) - 3) + (dataByte.GetBit(4) ? 1 : 0));
        Direction = Speed switch {
            253 or 254 => DirectionEnum.Stop,
            255        => DirectionEnum.EStop,
            _          => DirectionEnum.Reverse
        };
        return DecoderMessageTypeEnum.SpeedAndDirection;
    }

    internal DecoderMessageTypeEnum SpeedAndDirectionForForward(byte dataByte) {
        Speed = (byte)((((dataByte & 0B00001111) << 1) - 3) + (dataByte.GetBit(4) ? 1 : 0));
        Direction = Speed switch {
            253 or 254 => DirectionEnum.Stop,
            255        => DirectionEnum.EStop,
            _          => DirectionEnum.Forward
        };
        return DecoderMessageTypeEnum.SpeedAndDirection;
    }

    private void SetFunction(byte dataByte, byte start, byte end) {
        for (byte func = start; func <= end; func++) {
            Functions[func] = dataByte.GetBit(func - start);
        }
    }

    internal DecoderMessageTypeEnum FunctionGroupOneInstructions(byte dataByte) {
        // Loc Function L-4-3-2-1
        SetFunction(dataByte, 1, 4);
        Functions[0] = dataByte.GetBit(4);
        return DecoderMessageTypeEnum.FunctionsF0F4;
    }

    internal DecoderMessageTypeEnum FunctionGroupTwoInstructions(byte dataByte) {
        if (dataByte.GetBit(4)) { // F5-F8
            SetFunction(dataByte, 5, 8);
            return DecoderMessageTypeEnum.FunctionsF5F8;
        } else { // F9-F12
            SetFunction(dataByte, 9, 12);
            return DecoderMessageTypeEnum.FunctionsF9F12;
        }
    }

    internal DecoderMessageTypeEnum ExtendedFunctions(byte instByte) {
        var dataByte = PacketData.GetNext();
        switch (instByte & 0b00011111) {
        case 0b00000: // Binary State Control Long Form
            return DecoderMessageTypeEnum.BinaryStateLong;
        case 0b11101: // Binary State Control Short Form
            return DecoderMessageTypeEnum.BinaryStateShort;
        case 0b11110: // F13-F20
            SetFunction(dataByte, 13, 20);
            return DecoderMessageTypeEnum.FunctionsF13F20;
        case 0b11111: // F21-F28
            SetFunction(dataByte, 21, 28);
            return DecoderMessageTypeEnum.FunctionsF21F28;
        case 0b11000: // F29-F36
            SetFunction(dataByte, 29, 36);
            return DecoderMessageTypeEnum.FunctionsF29F36;
        case 0b11001: // F37-F44
            SetFunction(dataByte, 37, 44);
            return DecoderMessageTypeEnum.FunctionsF37F44;
        case 0b11010: // F45-F52 
            SetFunction(dataByte, 45, 52);
            return DecoderMessageTypeEnum.FunctionsF45F52;
        case 0b11011: // F53-F60 
            SetFunction(dataByte, 53, 60);
            return DecoderMessageTypeEnum.FunctionsF53F60;
        case 0b11100: // F61-F68 
            SetFunction(dataByte, 61, 68);
            return DecoderMessageTypeEnum.FunctionsF61F68;
        }
        return DecoderMessageTypeEnum.Error;
    }

    internal DecoderMessageTypeEnum ConfigurationVariableAccess(byte dataByte) {
        // Format for short form is 1111CCCC DDDDDDDD
        // Format for long form is 1110CCVV VVVVVVVV DDDDDDDD
        switch (dataByte & 11110000) {
        case 0b11110000:
            switch (dataByte & 0b00001111) {
            case 0b0000:
                return DecoderMessageTypeEnum.Error;
            case 0b0010:
                CvNumber              = 23;
                CvValue               = PacketData.GetNext();
                return DecoderMessageTypeEnum.Acceleration;
            case 0b0011:
                CvNumber              = 24;
                CvValue               = PacketData.GetNext();
                return DecoderMessageTypeEnum.Deceleration;
            case 0b1001:
                CvNumber              = 0;
                CvValue               = PacketData.GetNext();
                return DecoderMessageTypeEnum.Lock;
            default:
                return DecoderMessageTypeEnum.Error;
            }
        case 0b11100000:
            switch (dataByte & 0b00001100) {
            case 0b00:
                return DecoderMessageTypeEnum.Error;
            case 0b01:
                CvNumber              = ((dataByte & 00000011) << 8) | PacketData.GetNext();
                CvValue               = PacketData.GetNext();
                return DecoderMessageTypeEnum.VerifyCv;
            case 0b11:
                CvNumber              = ((dataByte & 00000011) << 8) | PacketData.GetNext();
                CvValue               = PacketData.GetNext();
                return DecoderMessageTypeEnum.WriteCv;
            case 0b10:
                CvNumber              = ((dataByte & 00000011) << 8) | PacketData.GetNext();
                CvValue               = PacketData.GetNext();
                return DecoderMessageTypeEnum.BitManipulate;
            default:
                return DecoderMessageTypeEnum.Error;
            }
        default:
            return DecoderMessageTypeEnum.Error;
        }
    }
}