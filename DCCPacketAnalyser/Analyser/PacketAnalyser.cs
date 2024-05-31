using System.Text;
using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;
using DCCPacketAnalyser.Analyser.Messages;

namespace DCCPacketAnalyser.Analyser;

public class PacketAnalyser {

    public static IPacketMessage Decode(byte[] packet) {
        return Decode(new PacketData(packet));
    }

    public static IPacketMessage Decode(PacketData packet) {

        // Start by working out what type of Address the packet is addressed to
        // Note that a Signal will come back as an Accessory and will be updated
        // when the data is decoded. The address will also change after the Data
        // decode part of the process. 
        // --------------------------------------------------------------------
        try {
            var decodedPacket = DeterminePacketType(packet);
            decodedPacket.ProcessRemainingPacket();
            return decodedPacket;
        } catch (Exception ex) {
            return new ErrorPacket(packet, "Could not determine the packet type: " + ex.Message);
        }
    }

    // Look at the first and second bytes and work out the type of address 
    // and what the actual address is. The first bits of the first byte of the 
    // packet determines the type and that also determines if we need the 2nd 
    // byte of the packet to calculate the addrress. 
    // This also increments the 'offset' so that we can proceed to use byte 2 or 3
    // in future calculations. 
    internal static IPacketMessage DeterminePacketType(PacketData packetData) {

        // Validate that we can access the Packet Array without an Index Out of Range
        // --------------------------------------------------------------------------
        if (!packetData.IsAtLeastLength(2)) throw new IndexOutOfRangeException("Packet size is < 2 so is an invalid packet.");
        
        var typeByte = packetData.GetNext();      // Get Byte #1
        var dataByte = packetData.GetNext();      // Get Byte #2

        if (typeByte == 0b11111111) return new IdlePacket(packetData);
        if (typeByte == 0b00000000) return new BroadcastPacket(packetData);
        
        // Short Address Decoder is represented as 00xxxxx0 where xxxxx0 is the address
        // -------------------------------------------------------------------------------------
        if (!typeByte.GetBit(7) && !typeByte.GetBit(6) && typeByte.GetBit(0)) {
            var address = typeByte & 0b00111111;
            return new DecoderPacket(packetData, AddressTypeEnum.Short, address);
        } 
        
        // Long Address Decoder is represented by 10xxxxxx - xxxxxx is high range of the address
        // -------------------------------------------------------------------------------------
        if (typeByte.GetBit(7) && typeByte.GetBit(6)) {
            var address = 256 * (typeByte & 0b00111111) + dataByte;
            return new DecoderPacket(packetData, AddressTypeEnum.Long, address);
        } 
        
        // Accessory is represented by 10xxxxxx 1xxxxxxx 
        // -------------------------------------------------------------------------------------
        if (typeByte.GetBit(7) && !typeByte.GetBit(6) && dataByte.GetBit(7)) {
            var address = ((~dataByte & 0b01110000) << 2) | (typeByte & 0b00111111);
            return new AccessoryPacket(packetData, address);
        } 
        
        // Signal is represented by 10xxxxxx 0xxx0xx1
        // -------------------------------------------------------------------------------------
        if (typeByte.GetBit(7) && !typeByte.GetBit(6) && !dataByte.GetBit(7)) {
            // The extended address is calculated as a normal address (as per Accessory)
            // and then shifted 2 bits up and bits 1 & 2 of the dataByte added.
            // This works, but I don't think it is the right solution.
            var address = ((~dataByte & 0b01110000) << 2) | (typeByte & 0b00111111);
            var extrByte = (dataByte & 0b00000110) >> 1;
            address = (((address - 1) << 2) | extrByte) + 1;
            return new SignalPacket(packetData, address);
        } 
        return new ErrorPacket(packetData, "Unable to determine the type of packet.");
    }
}