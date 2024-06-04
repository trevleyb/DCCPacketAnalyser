using System.Text;
using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;
using DCCPacketAnalyser.Analyser.Messages;

namespace DCCPacketAnalyser.Analyser;

public class PacketAnalyser {

    public delegate void              PacketAnalysedEvent(IPacketMessage packetMessage);
    public event PacketAnalysedEvent? PacketAnalysed;
    private          int              _invalidCount   = 0;
    private readonly MessageTracker   _messageTracker = new();
    
    public IPacketMessage Decode(byte[] packet) {
        return Decode(new PacketData(packet));
    }

    public IPacketMessage Decode(string packet) {
        return Decode(new PacketData(packet.Replace(" ", "").ToHexArray()));
    }
    
    public IPacketMessage Decode(PacketData packet) {

        if (!packet.IsValidPacket && ++_invalidCount > 10) throw new Exception("Invalid packet data consistently recieved");

        // Start by working out what type of Address the packet is addressed to
        // and return an instance/object that represents the type of thing we are 
        // sending messages to. Then, decode the rest of the message in that object. 
        // ----------------------------------------------------------------------------
        try {
            var baseMessage   = DeterminePacketType(packet);
            var decodedMessage = ProcessRemainingPacket(baseMessage);
            if (_messageTracker.IsMessageDuplicated(decodedMessage)) return new DuplicateMessage(packet);
            PacketAnalysed?.Invoke(decodedMessage);
            return decodedMessage;
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
    internal IPacketMessage DeterminePacketType(PacketData packetData) {
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
    
    private IPacketMessage ProcessRemainingPacket(IPacketMessage packetMessage) {
        return packetMessage.AddressType switch {
            AddressTypeEnum.Accessory => ProcessAccessoryPacket(packetMessage),
            AddressTypeEnum.Signal    => ProcessSignalPacket(packetMessage),
            AddressTypeEnum.Short     => ProcessDecoderPacket(packetMessage),
            AddressTypeEnum.Long      => ProcessDecoderPacket(packetMessage),
            AddressTypeEnum.Broadcast => ProcessDecoderPacket(packetMessage),
            _                         => packetMessage
        };
    }

    private SignalMessage ProcessSignalPacket(IPacketMessage packetMessage) {
        var dataByte = packetMessage.PacketData.Next();
        if (!dataByte.GetBit(7)) { // Basic Accessory Decoder
            return new SignalMessage(packetMessage,(SignalAspectEnums)(byte)(dataByte & 0b00011111));
        } else {
            throw new Exception("We really should not be here. This should be have been an Accessory not Signal.");
        }
    }

    private AccessoryMessage ProcessAccessoryPacket(IPacketMessage packetMessage) {
        var dataByte = packetMessage.PacketData.Next();
        if (dataByte.GetBit(7)) { // Basic Accessory Decoder
            var activated        = dataByte.GetBit(3);
            var newAddress       = (((dataByte >> 4) & 0b00000111) << 6) | packetMessage.Address;
            var output           = dataByte.GetBit(0);
            var outputPair       = (dataByte & 0b00000110) >> 1;
            return new AccessoryMessage(packetMessage, dataByte.GetBit(0) ? AccessoryStateEnum.Normal : AccessoryStateEnum.Reversed);
        } else {
            throw new Exception("We really should not be here. This should be have been an Signal not Accessory.");
        }
    }

    /// <summary>
    /// Get the multifunction data from the packet and update the decoded data object.
    /// </summary>
    private IPacketMessage ProcessDecoderPacket(IPacketMessage packetMessage) {
        if (!packetMessage.PacketData.IsAtLeastLength(3)) throw new Exception("Invalid Data to process. Must have at least 3 elements: " + packetMessage.PacketData.ToBinary);

        // Decoder Control is in the form CCCDDDDD where 
        // CCC = command and DDDDD = data to apply
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

    internal IPacketMessage DecoderAndConsistControl(IPacketMessage packetMessage, byte dataByte) {
        // Format: xxx0CCCF
        // If the first bit of C is 0 it is a Decoder control message
        // If the first bit of C is 1 it is a Consist control message
        if (!dataByte.GetBit(4)) {
            var instructionByte = (byte)((dataByte & 0b00001110) >> 1);
            var instructionData = (byte)(dataByte & 0b00000001);
            return new ControlMessage(packetMessage) {
                MessageType = instructionByte switch {
                    0b000 => instructionData == 0 ? ControlMessageTypeEnum.Reset : ControlMessageTypeEnum.HardReset,
                    0b001 => ControlMessageTypeEnum.FactoryTest,
                    0b010 => ControlMessageTypeEnum.Reserved,
                    0b011 => ControlMessageTypeEnum.DecoderFlags,
                    0b100 => ControlMessageTypeEnum.Reserved,
                    0b101 => ControlMessageTypeEnum.AdvancedAddressing,
                    0b110 => ControlMessageTypeEnum.Reserved,
                    0b111 => instructionData == 0 ? ControlMessageTypeEnum.None : ControlMessageTypeEnum.DecoderAck,
                    _     => ControlMessageTypeEnum.None
                }
            };
        }

        return (dataByte & 0b00001111) switch {
            0b0010 => new ConsistMessage(packetMessage, packetMessage.PacketData.Next() & 0b01111111, DirectionEnum.Reverse),
            0b0011 => new ConsistMessage(packetMessage, packetMessage.PacketData.Next() & 0b01111111, DirectionEnum.Forward),
            _      => new ErrorMessage(packetMessage, "Invalid Consist Control Message.")
        };
    }

    internal IPacketMessage AdvancedOperationInstructions(IPacketMessage packetMessage, byte dataByte) {
        switch (dataByte) {
        case 0b00011111: // 128 Speed Step Control
            DirectionEnum direction;
            var speed = (byte)(packetMessage.PacketData.Next() & 0b01111111);
            switch (speed) {
            case 0:
                direction = DirectionEnum.Stop;
                break;
            case 1:
                speed     -= 1;
                direction =  DirectionEnum.EStop;
                break;
            default:
                speed     -= 1;
                direction =  packetMessage.PacketData.Next().GetBit(7) ? DirectionEnum.Forward : DirectionEnum.Reverse;
                break;
            }
            return new SpeedAndDirectionMessage(packetMessage,speed,direction);
        
        case 0b00011110: // Restricted Speed Step
            var speedStepsMessage = new SpeedStepsMessage(packetMessage) {
                SpeedStepsData  = (byte)(packetMessage.PacketData.Next() & 0b01111111),
                RestrictedSpeed = packetMessage.PacketData.Current().GetBit(7)
            };
            return speedStepsMessage;
        case 0b00011101: // Analog Function Group
            break;
        }
        return new ErrorMessage(packetMessage, "Invalid Advanced Operation Instruction.");
    }

    internal IPacketMessage SpeedAndDirectionForReverse(IPacketMessage packetMessage, byte dataByte) {
        var speed = (byte)(((dataByte & 0B00001111) << 1) - 3 + (dataByte.GetBit(4) ? 1 : 0));
        var direction = speed switch {
            253 or 254 => DirectionEnum.Stop,
            255        => DirectionEnum.EStop,
            _          => DirectionEnum.Reverse
        };
        return new SpeedAndDirectionMessage(packetMessage,speed,direction);
    }

    internal IPacketMessage SpeedAndDirectionForForward(IPacketMessage packetMessage, byte dataByte) {
        var speed = (byte)(((dataByte & 0B00001111) << 1) - 3 + (dataByte.GetBit(4) ? 1 : 0));
        var direction = speed switch {
            253 or 254 => DirectionEnum.Stop,
            255        => DirectionEnum.EStop,
            _          => DirectionEnum.Forward
        };
        return new SpeedAndDirectionMessage(packetMessage,speed,direction);
    }

    internal IPacketMessage FunctionGroupOneInstructions(IPacketMessage packetMessage, byte dataByte) {
        return new FunctionsMessage(packetMessage);
    }

    internal IPacketMessage FunctionGroupTwoInstructions(IPacketMessage packetMessage, byte dataByte) {
        return dataByte.GetBit(4) ? // F5-F8
            new FunctionsMessage(packetMessage,dataByte,FunctionsGroupEnum.FunctionsF5F8) :
            new FunctionsMessage(packetMessage,dataByte,FunctionsGroupEnum.FunctionsF9F12);
    }

    internal IPacketMessage ExtendedFunctions(IPacketMessage packetMessage, byte instByte) {
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

    internal IPacketMessage ConfigurationVariableAccess(IPacketMessage packetMessage, byte dataByte) {
        // Format for short form is xxx1CCCC DDDDDDDD
        // Format for long form is xxx0CCVV VVVVVVVV DDDDDDDD
        if (dataByte.GetBit(4)) {
            return (dataByte & 0b00001111) switch {
                0b0000 => new ErrorMessage(packetMessage, "Invalid Short form CV Variable Access (00)"),
                0b0010 => new MomentumMessage(packetMessage, MomentumTypeEnum.Acceleration, packetMessage.PacketData.Next()),
                0b0011 => new MomentumMessage(packetMessage, MomentumTypeEnum.Deceleration, packetMessage.PacketData.Next()),
                0b1001 => new MomentumMessage(packetMessage, MomentumTypeEnum.Lock, packetMessage.PacketData.Next()),
                _      => new ErrorMessage(packetMessage, "Invalid Short form CV Variable Access")
            };
        } else {
            return (dataByte & 0b00001100) switch {
                0b00 => new ErrorMessage(packetMessage, "Invalid Long form CV Variable Access (00)"),
                0b01 => new ConfigCVMessage(packetMessage, ConfigCVTypeEnum.Verify, ((dataByte & 00000011) << 8) | packetMessage.PacketData.Next(), packetMessage.PacketData.Next()),
                0b11 => new ConfigCVMessage(packetMessage, ConfigCVTypeEnum.Write, ((dataByte & 00000011) << 8) | packetMessage.PacketData.Next(), packetMessage.PacketData.Next()),
                0b10 => new ConfigCVMessage(packetMessage, ConfigCVTypeEnum.BitManipulate, ((dataByte & 00000011) << 8) | packetMessage.PacketData.Next(), packetMessage.PacketData.Next()),
                _    => new ErrorMessage(packetMessage, "Invalid Long form CV Variable Access")
            };
        }
    }
}