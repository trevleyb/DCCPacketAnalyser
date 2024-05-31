namespace DCCPacketAnalyser.Analyser.Base;

public class PacketData {

    private readonly byte[] _packetData;
    private int    _currentOffset = -1;

    public PacketData(byte[] packetData) {
        _packetData = packetData;
        if (!IsValidPacket) throw new Exception("Packet Data is invalid.");
    }
    
    /// <summary>
    /// Get the next Packet in the sequence of packets and increment the offset counter
    /// </summary>
    /// <returns>A byte being the next byte in the packet</returns>
    public byte GetNext() {
        if (_currentOffset < 0) _currentOffset = 0;
        else _currentOffset++;
        return GetAt(_currentOffset);
    }

    /// <summary>
    /// Get the packet byte that we are currently pointing to
    /// </summary>
    /// <returns>A byte being the current packet offset byte</returns>
    public byte GetCurrent() {
        return _currentOffset < 0 ? GetNext() : GetAt(_currentOffset);
    }

    /// <summary>
    /// Look forward 1 element in the packet array but do not increment the packet counter
    /// </summary>
    /// <returns>A byte being the next packet in the array</returns>
    public byte PeekNext() {
        return GetAt(_currentOffset++);
    }

    /// <summary>
    /// Resets the packet offset pointer to 0;
    /// </summary>
    public void Reset() => _currentOffset = -1; 
    
    /// <summary>
    /// Gets a byte from the Packet Data but ensures that the byte exists first
    /// </summary>
    /// <param name="offset">the offset in the array that we want to get the data from</param>
    /// <returns>A byte from that position in the array</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public byte GetAt(int offset) {
        if (_packetData.Length > offset) return _packetData[offset];
        throw new ArgumentOutOfRangeException(nameof(offset),"Invalid Packet: Not enough Packet Data for provided Offset.");
    }

    /// <summary>
    /// Check if the data in the packet is at least as long as length
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public bool IsAtLeastLength(int length) => _packetData.Length >= length;
    
    /// <summary>
    /// Check if the packet is valid by performing a checksum calculation.
    /// XOR bytes 1 & 2 and then XOR the result with the next byte through
    /// to the last byte which is the error bit byte. The result should
    /// always be zero. 
    /// </summary>
    /// <returns>True if the packet is valid, otherwise false</returns>
    public bool IsValidPacket => _packetData.Aggregate(0, (current, t) => current ^ t) == 0;
    
    /// <summary>
    /// Return the Packet Data as a set of Binary Numbers separated by -
    /// </summary>
    public string ToBinary => string.Join("-", _packetData.Select(part => Convert.ToString(part, 2).PadLeft(8, '0')));
}