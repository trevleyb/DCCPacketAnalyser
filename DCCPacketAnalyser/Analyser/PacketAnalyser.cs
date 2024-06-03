using System.Text;
using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;
using DCCPacketAnalyser.Analyser.Messages;

namespace DCCPacketAnalyser.Analyser;

public static class PacketAnalyser {

    public delegate void PacketAnalysedEventHandler(IPacketMessage packetMessage);
    public static event PacketAnalysedEventHandler? PacketAnalysed;
    
    private static PacketData? _previousData;
    
    public static IPacketMessage Decode(byte[] packet) {
        return Decode(new PacketData(packet));
    }

    public static IPacketMessage Decode(PacketData packet) {
        // Start by working out what type of Address the packet is addressed to
        // and return an instance/object that represents the type of thing we are 
        // sending messages to. Then, decode the rest of the message in that object. 
        // ----------------------------------------------------------------------------
        if (_previousData is not null && _previousData.Equals(packet)) return new DuplicateMessage(packet);
        try {
            var decodedPacket = ProcessRemainingPacket(DeterminePacketType(packet));
            _previousData = packet;
            PacketAnalysed?.Invoke(decodedPacket);
            return decodedPacket;
        } catch (Exception ex) {
            return new ErrorMessage(packet, "Could not determine the packet type: " + ex.Message);
        }
    }

    // Look at the first and second bytes and work out the type of address 
    // and what the actual address is. The first bits of the first byte of the 
    // packet determines the type and that also determines if we need the 2nd 
    // byte of the packet to calculate the address. 
    // This also increments the 'offset' so that we can proceed to use byte 2 or 3
    // in future calculations. 
    internal static IPacketMessage DeterminePacketType(PacketData packetData) {
        // Validate that we can access the Packet Array without an Index Out of Range
        // --------------------------------------------------------------------------
        if (!packetData.IsAtLeastLength(2)) throw new IndexOutOfRangeException("Packet size is < 2 so is an invalid packet.");

        var typeByte = packetData.First(); // Get Byte #1
        var dataByte = packetData.Peek();  // Get Byte #2 but leave it ready for further processing.

        if (typeByte == 0b11111111) return new IdleMessage(packetData);
        if (typeByte == 0b00000000) return new PacketMessage(packetData, AddressTypeEnum.Broadcast, 0);

        // Short Address Decoder is represented as 00xxxxx0 where xxxxx0 is the address
        // -------------------------------------------------------------------------------------
        if (!typeByte.GetBit(7) && !typeByte.GetBit(6) && typeByte.GetBit(0)) {
            var address = typeByte & 0b00111111;
            return new PacketMessage(packetData, AddressTypeEnum.Short, address);
        }

        // Long Address Decoder is represented by 10xxxxxx - xxxxxx is high range of the address
        // Note: 2nd byte is used up, so increment packet byte pointer. 
        // -------------------------------------------------------------------------------------
        if (typeByte.GetBit(7) && typeByte.GetBit(6)) {
            var address = 256 * (typeByte & 0b00111111) + packetData.Next();
            return new PacketMessage(packetData, AddressTypeEnum.Long, address);
        }

        // Accessory is represented by 10xxxxxx 1xxxxxxx 
        // Note: 2nd byte is reused in the subsequent processing so don't increment next packet pointer
        // -------------------------------------------------------------------------------------
        if (typeByte.GetBit(7) && !typeByte.GetBit(6) && dataByte.GetBit(7)) {
            var address = ((~dataByte & 0b01110000) << 2) | (typeByte & 0b00111111);
            return new PacketMessage(packetData, AddressTypeEnum.Accessory, address);
        }

        // Signal is represented by 10xxxxxx 0xxx0xx1
        // -------------------------------------------------------------------------------------
        if (typeByte.GetBit(7) && !typeByte.GetBit(6) && !dataByte.GetBit(7)) {
            // The extended address is calculated as a normal address (as per Accessory)
            // and then shifted 2 bits up and bits 1 & 2 of the dataByte added.
            // This works, but I don't think it is the right solution.
            var address  = ((~dataByte & 0b01110000) << 2) | (typeByte & 0b00111111);
            var extraByte = (dataByte & 0b00000110) >> 1;
            address = (((address - 1) << 2) | extraByte) + 1;
            packetData.Next(); // Move pointer along
            return new PacketMessage(packetData, AddressTypeEnum.Signal, address);
        }

        return new ErrorMessage(packetData, "Unable to determine the type of packet.");
    }
    
    private static IPacketMessage ProcessRemainingPacket(IPacketMessage packetMessage) {
        return packetMessage.AddressType switch {
            AddressTypeEnum.Accessory => ProcessAccessoryPacket(packetMessage),
            AddressTypeEnum.Signal    => ProcessSignalPacket(packetMessage),
            AddressTypeEnum.Short     => ProcessesDecoderPacket((PacketMessage)packetMessage),
            AddressTypeEnum.Long      =>ProcessesDecoderPacket((PacketMessage)packetMessage),
            AddressTypeEnum.Broadcast => ProcessesDecoderPacket((PacketMessage)packetMessage),
            _                         => packetMessage
        };
    }

    private static SignalMessage ProcessSignalPacket(IPacketMessage packetMessage) {
        var signalMessage = new SignalMessage(packetMessage);
        var dataByte = signalMessage.PacketData.Next();
        if (!dataByte.GetBit(7)) { // Basic Accessory Decoder
            signalMessage.Aspect = (SignalAspectEnums)(byte)(dataByte & 0b00011111);
        } else {
            throw new Exception("We really should not be here. This should be have been an Accessory not Signal.");
        }
        return signalMessage;
    }

    private static AccessoryMessage ProcessAccessoryPacket(IPacketMessage packetMessage) {
        var accessoryMessage = new AccessoryMessage(packetMessage);
        var dataByte = accessoryMessage.PacketData.Next();
        if (dataByte.GetBit(7)) { // Basic Accessory Decoder
            var activated  = dataByte.GetBit(3);
            var newAddress = (((dataByte >> 4) & 0b00000111) << 6) | accessoryMessage.Address;
            var output     = dataByte.GetBit(0);
            var outputPair = (dataByte & 0b00000110) >> 1;
            accessoryMessage.State = dataByte.GetBit(0) ? AccessoryStateEnum.Normal : AccessoryStateEnum.Reversed;
        } else {
            throw new Exception("We really should not be here. This should be have been an Signal not Accessory.");
        }
        return accessoryMessage;
    }

    /// <summary>
    /// Get the multifunction data from the packet and update the decoded data object.
    /// </summary>
    private static IPacketMessage ProcessesDecoderPacket(IPacketMessage packetMessage) {
        if (!packetMessage.PacketData.IsAtLeastLength(3)) throw new Exception("Invalid Data to process. Must have at least 3 elements: " + packetMessage.PacketData.ToBinary);

        // Decoder Control is in the form CCCDDDDD where 
        // CCC = command and DDDDD = dfata to apply
        // ------------------------------------------------------------
        var instByte = packetMessage.PacketData.Next();
        var cmdByte  = (byte)(instByte & 0b11100000) >> 5;
        var dataByte = (byte)(instByte & 0b00011111);

        return cmdByte switch {
            0b00000000 => DecoderAndConsistControl(packetMessage, dataByte),
            0b00000001 => AdvancedOperationInstructions(packetMessage, dataByte),
            0b00000010 => SpeedAndDirectionForReverse(packetMessage, dataByte),
            0b00000011 => SpeedAndDirectionForForward(packetMessage, dataByte),
            0b00000100 => FunctionGroupOneInstructions(packetMessage, dataByte),
            0b00000101 => FunctionGroupTwoInstructions(packetMessage, dataByte),
            0b00000110 => ExtendedFunctions(packetMessage, dataByte),
            0b00000111 => ConfigurationVariableAccess(packetMessage, dataByte),
            _          => throw new ArgumentOutOfRangeException(null, $"Invalid Instruction byte for MultiFunction Decoder.")
        };
    }

    internal static IPacketMessage DecoderAndConsistControl(IPacketMessage packetMessage, byte dataByte) {
        // Format: 000CCCF
        // If the first bit of C is 0 it is a decoder control message
        // If the first bit of C is 1 it is a Consist control message
        if (!dataByte.GetBit(4)) {
            var instructionByte = (byte)((dataByte & 0b00001110) >> 1);
            var instructionData = (byte)(dataByte & 0b00000001);
            return new ControlMessage(packetMessage) {
                MessageType = instructionByte switch {
                    0b000 => instructionData == 0 ? DecoderMessageTypeEnum.Reset : DecoderMessageTypeEnum.HardReset,
                    0b001 => DecoderMessageTypeEnum.FactoryTest,
                    0b010 => DecoderMessageTypeEnum.Reserved,
                    0b011 => DecoderMessageTypeEnum.DecoderFlags,
                    0b100 => DecoderMessageTypeEnum.Reserved,
                    0b101 => DecoderMessageTypeEnum.AdvancedAddressing,
                    0b110 => DecoderMessageTypeEnum.Reserved,
                    0b111 => instructionData == 0 ? DecoderMessageTypeEnum.None : DecoderMessageTypeEnum.DecoderAck,
                    _     => DecoderMessageTypeEnum.None
                }
            };
        }

        var consistMessage = new ConsistMessage(packetMessage);
        switch (dataByte & 0b00001111) {
        case 0b0010:
            consistMessage.ConsistAddress = packetMessage.PacketData.Next() & 0b01111111;
            consistMessage.Direction      = DirectionEnum.Reverse;
            return consistMessage;
        case 0b0011:
            consistMessage.ConsistAddress = packetMessage.PacketData.Next() & 0b01111111;
            consistMessage.Direction      = DirectionEnum.Forward;
            return consistMessage;
        default:
            return new ErrorMessage(packetMessage, "Invalid Consist Control Message.");
        }
    }

    internal static IPacketMessage AdvancedOperationInstructions(IPacketMessage packetMessage, byte dataByte) {
        var speedAndDir = new SpeedAndDirMessage(packetMessage);
        switch (dataByte) {
        case 0b00111111: // 128 Speed Step Control
            speedAndDir.Speed = (byte)(packetMessage.PacketData.Next() & 0b01111111);
            switch (speedAndDir.Speed) {
            case 0:
                speedAndDir.Direction = DirectionEnum.Stop;
                break;
            case 1:
                speedAndDir.Speed     -= 1;
                speedAndDir.Direction =  DirectionEnum.EStop;
                break;
            default:
                speedAndDir.Speed     -= 1;
                speedAndDir.Direction =  packetMessage.PacketData.Next().GetBit(7) ? DirectionEnum.Forward : DirectionEnum.Reverse;
                break;
            }
            return speedAndDir;
        
        case 0b00111110: // Restricted Speed Step
            var speedStepsMessage = new SpeedStepsMessage(packetMessage) {
                SpeedStepsData  = (byte)(packetMessage.PacketData.Next() & 0b01111111),
                RestrictedSpeed = packetMessage.PacketData.Current().GetBit(7)
            };
            return speedStepsMessage;
        case 0b00111101: // Analog Function Group
            break;
        }
        return new ErrorMessage(packetMessage, "Invalid Advanced Operation Instruction.");
    }

    internal static IPacketMessage SpeedAndDirectionForReverse(IPacketMessage packetMessage, byte dataByte) {
        var speedAndDir = new SpeedAndDirMessage(packetMessage) {
            Speed = (byte)(((dataByte & 0B00001111) << 1) - 3 + (dataByte.GetBit(4) ? 1 : 0))
        };

        speedAndDir.Direction = speedAndDir.Speed switch {
            253 or 254 => DirectionEnum.Stop,
            255        => DirectionEnum.EStop,
            _          => DirectionEnum.Reverse
        };
        return speedAndDir;
    }

    internal static IPacketMessage SpeedAndDirectionForForward(IPacketMessage packetMessage, byte dataByte) {
        var speedAndDir = new SpeedAndDirMessage(packetMessage) {
            Speed = (byte)(((dataByte & 0B00001111) << 1) - 3 + (dataByte.GetBit(4) ? 1 : 0))
        };
        speedAndDir.Direction = speedAndDir.Speed switch {
            253 or 254 => DirectionEnum.Stop,
            255        => DirectionEnum.EStop,
            _          => DirectionEnum.Forward
        };
        return speedAndDir;
    }

    internal static IPacketMessage FunctionGroupOneInstructions(IPacketMessage packetMessage, byte dataByte) {
        return new FunctionsMessage(packetMessage);
    }

    internal static IPacketMessage FunctionGroupTwoInstructions(IPacketMessage packetMessage, byte dataByte) {
        return dataByte.GetBit(4) ? // F5-F8
            new FunctionsMessage(packetMessage,dataByte,FunctionsGroupEnum.FunctionsF5F8) :
            new FunctionsMessage(packetMessage,dataByte,FunctionsGroupEnum.FunctionsF9F12);
    }

    internal static IPacketMessage ExtendedFunctions(IPacketMessage packetMessage, byte instByte) {
        //var functions = new FunctionsMessage(packetMessage);
        var dataByte = packetMessage.PacketData.Next();
        return (instByte & 0b00011111) switch {
            0b00000 => new BinaryStateMessage(packetMessage, BinaryStateTypeEnum.Long),
            0b11101 => new BinaryStateMessage(packetMessage, BinaryStateTypeEnum.Short),
            0b11110 => new FunctionsMessage(packetMessage, dataByte, FunctionsGroupEnum.FunctionsF13F20),
            0b11111 => new FunctionsMessage(packetMessage, dataByte, FunctionsGroupEnum.FunctionsF21F28),
            0b11000 => new FunctionsMessage(packetMessage, dataByte, FunctionsGroupEnum.FunctionsF29F36),
            0b11001 => new FunctionsMessage(packetMessage, dataByte, FunctionsGroupEnum.FunctionsF37F44),
            0b11010 => new FunctionsMessage(packetMessage, dataByte, FunctionsGroupEnum.FunctionsF45F52),
            0b11011 => new FunctionsMessage(packetMessage, dataByte, FunctionsGroupEnum.FunctionsF53F60),
            0b11100 => new FunctionsMessage(packetMessage, dataByte, FunctionsGroupEnum.FunctionsF61F68),
                  _ => new ErrorMessage(packetMessage, "Invalid Extended Function Instruction.")
        };
    }

    internal static IPacketMessage ConfigurationVariableAccess(IPacketMessage packetMessage, byte dataByte) {
        // Format for short form is 1111CCCC DDDDDDDD
        // Format for long form is 1110CCVV VVVVVVVV DDDDDDDD
        switch (dataByte & 11110000) {
        case 0b11110000:
            return (dataByte & 0b00001111) switch {
                0b0000 => new ErrorMessage(packetMessage, "Invalid Short form CV Variable Access (00)"),
                0b0010 => new MomentumMessage(packetMessage, MomentumTypeEnum.Acceleration, packetMessage.PacketData.Next()),
                0b0011 => new MomentumMessage(packetMessage, MomentumTypeEnum.Deceleration, packetMessage.PacketData.Next()),
                0b1001 => new MomentumMessage(packetMessage, MomentumTypeEnum.Lock, packetMessage.PacketData.Next()),
                _      => new ErrorMessage(packetMessage, "Invalid Short form CV Variable Access")
            };

        case 0b11100000:
            return (dataByte & 0b00001100) switch {
                0b00 => new ErrorMessage(packetMessage, "Invalid Long form CV Variable Access (00)"),
                0b01 => new ConfigCVMessage(packetMessage, ConfigCVTypeEnum.Verify, ((dataByte & 00000011) << 8) | packetMessage.PacketData.Next(), packetMessage.PacketData.Next()),
                0b11 => new ConfigCVMessage(packetMessage, ConfigCVTypeEnum.Write, ((dataByte & 00000011) << 8) | packetMessage.PacketData.Next(), packetMessage.PacketData.Next()),
                0b10 => new ConfigCVMessage(packetMessage, ConfigCVTypeEnum.BitManipulate, ((dataByte & 00000011) << 8) | packetMessage.PacketData.Next(), packetMessage.PacketData.Next()),
                _    => new ErrorMessage(packetMessage, "Invalid Long form CV Variable Access")
            };
        default:
            return new ErrorMessage(packetMessage, "Invalid CV Variable Access");
        }
    }
    
}