using System.Text;
using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class DecoderPacket : PacketMessage, IPacketMessage {

    public SpeedStepsEnum            SpeedSteps            { get; set; }
    public ControlMessageTypeEnum    ControlMessageType    { get; set; }
    public DecoderMessageTypeEnum    DecoderMessageType    { get; set; }
    public DecoderMessageSubTypeEnum DecoderMessageSubType { get; set; }
    public DirectionEnum             Direction             { get; set; }
    public int                       ConsistAddress        { get; set; }
    public byte                      Speed                 { get; set; }
    public bool                      RestrictedSpeed       { get; set; }
    public byte                      SpeedStepsData        { get; set; }
    public bool[]                    Functions             { get; set; } = new bool[69];

    public DecoderPacket(PacketData packetData, AddressTypeEnum addressType, int address) : base(packetData) {
        Address = address;
        AddressType = addressType;
        SpeedSteps = SpeedStepsEnum.Unknown;
        ControlMessageType = ControlMessageTypeEnum.None;
        Direction = DirectionEnum.Unknown;
        ConsistAddress = 0;
        Speed = 0;
        RestrictedSpeed = false;
        SpeedStepsData = 0;
        for (var i = 0; i < 69; i++) Functions[i] = false;
    }

    public override string ToString() => $"DECODER: Address={Address}{AddressType}";

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
            ControlMessageType = instraByte switch {
                0b000 => instraData == 0 ? ControlMessageTypeEnum.Reset : ControlMessageTypeEnum.HardReset,
                0b001 => ControlMessageTypeEnum.FactoryTest,
                0b010 => ControlMessageTypeEnum.Reserved,
                0b011 => ControlMessageTypeEnum.DecoderFlags,
                0b100 => ControlMessageTypeEnum.Reserved,
                0b101 => ControlMessageTypeEnum.AdvancedAddressing,
                0b110 => ControlMessageTypeEnum.Reserved,
                0b111 => instraData == 0 ? ControlMessageTypeEnum.None : ControlMessageTypeEnum.DecoderAck,
                _     => ControlMessageTypeEnum.None,
            };
        } else {
            switch (dataByte & 0b00001111) {
            case 0b0010:
                ControlMessageType = ControlMessageTypeEnum.Consist;
                ConsistAddress = PacketData.GetNext() & 0b01111111;
                Direction = DirectionEnum.Forward;
                break;
            case 0b0011:
                ControlMessageType = ControlMessageTypeEnum.Consist;
                ConsistAddress = PacketData.GetNext() & 0b01111111;
                Direction = DirectionEnum.Forward;
                break;
            default:
                ControlMessageType = ControlMessageTypeEnum.Error;
                break;
            }
        }
        return DecoderMessageTypeEnum.DecoderAndConsist;
    }

    internal DecoderMessageTypeEnum AdvancedOperationInstructions(byte dataByte) {
        switch (dataByte) {
        case 0b00111111: // 128 Speed Step Control
            DecoderMessageSubType = DecoderMessageSubTypeEnum.SpeedStepControl;
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
            break;
        case 0b00111110: // Restricted Speed Step
            DecoderMessageSubType = DecoderMessageSubTypeEnum.RestrictedSpeedStep;
            SpeedStepsData = (byte)(PacketData.GetNext() & 0b01111111);
            RestrictedSpeed = PacketData.GetCurrent().GetBit(7);
            break;
        case 0b00111101: // Analog Function Group
            DecoderMessageSubType = DecoderMessageSubTypeEnum.AnalogFunctionGroup;
            break;
        default:
            DecoderMessageSubType = DecoderMessageSubTypeEnum.Reserved;
            break;
        }

        return DecoderMessageTypeEnum.AdvancedOperation;
    }

    internal DecoderMessageTypeEnum SpeedAndDirectionForReverse(byte dataByte) {
        Speed = (byte)((((dataByte & 0B00001111) << 1) - 3) + (dataByte.GetBit(4) ? 1 : 0));
        Direction = Speed switch {
            253 or 254 => DirectionEnum.Stop,
            255        => DirectionEnum.EStop,
            _          => DirectionEnum.Reverse
        };

        return DecoderMessageTypeEnum.SpeedAndDirectionForReverse;
    }

    internal DecoderMessageTypeEnum SpeedAndDirectionForForward(byte dataByte) {
        Speed = (byte)((((dataByte & 0B00001111) << 1) - 3) + (dataByte.GetBit(4) ? 1 : 0));
        Direction = Speed switch {
            253 or 254 => DirectionEnum.Stop,
            255        => DirectionEnum.EStop,
            _          => DirectionEnum.Forward
        };

        return DecoderMessageTypeEnum.SpeedAndDirectionForForward;
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
        DecoderMessageSubType = DecoderMessageSubTypeEnum.FunctionsF0_F4;
        return DecoderMessageTypeEnum.FunctionGroupOne;
    }

    internal DecoderMessageTypeEnum FunctionGroupTwoInstructions(byte dataByte) {
        if (dataByte.GetBit(4)) { // F5-F8
            DecoderMessageSubType = DecoderMessageSubTypeEnum.FunctionsF5_F8;
            SetFunction(dataByte, 5, 8);
        } else { // F9-F12
            DecoderMessageSubType = DecoderMessageSubTypeEnum.FunctionsF9_F12;
            SetFunction(dataByte, 9, 12);
        }

        return DecoderMessageTypeEnum.FunctionGroupTwo;
    }

    internal DecoderMessageTypeEnum ExtendedFunctions(byte instByte) {
        var dataByte = PacketData.GetNext();
        switch (instByte & 0b00011111) {
        case 0b00000: // Binary State Control Long Form
            DecoderMessageSubType = DecoderMessageSubTypeEnum.BinaryStateLong;
            break;
        case 0b11101: // Binary State Control Short Form
            DecoderMessageSubType = DecoderMessageSubTypeEnum.BinaryStateShort;
            break;
        case 0b11110: // F13-F20
            SetFunction(dataByte, 13, 20);
            DecoderMessageSubType = DecoderMessageSubTypeEnum.FunctionsF13_F20;
            break;
        case 0b11111: // F21-F28
            SetFunction(dataByte, 21, 28);
            DecoderMessageSubType = DecoderMessageSubTypeEnum.FunctionsF21_F28;
            break;
        case 0b11000: // F29-F36
            SetFunction(dataByte, 29, 36);
            DecoderMessageSubType = DecoderMessageSubTypeEnum.FunctionsF29_F36;
            break;
        case 0b11001: // F37-F44
            SetFunction(dataByte, 37, 44);
            DecoderMessageSubType = DecoderMessageSubTypeEnum.FunctionsF37_F44;
            break;
        case 0b11010: // F45-F52 
            SetFunction(dataByte, 45, 52);
            DecoderMessageSubType = DecoderMessageSubTypeEnum.FunctionsF45_F52;
            break;
        case 0b11011: // F53-F60 
            SetFunction(dataByte, 53, 60);
            DecoderMessageSubType = DecoderMessageSubTypeEnum.FunctionsF53_F60;
            break;
        case 0b11100: // F61-F68 
            SetFunction(dataByte, 61, 68);
            DecoderMessageSubType = DecoderMessageSubTypeEnum.FunctionsF61_F68;
            break;
        }

        return DecoderMessageTypeEnum.ExtendedFunctions;
    }

    internal DecoderMessageTypeEnum ConfigurationVariableAccess(byte dataByte) {

        return DecoderMessageTypeEnum.ConfigurationVariables;
    }
}