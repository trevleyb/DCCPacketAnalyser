namespace DCCPacketAnalyser.Analyser.Base;

public class PacketData(byte[] packetData) : IEquatable<PacketData> {
    private readonly byte[] _packetData    = packetData;
    private          int    _currentOffset = -1;

    /// <summary>
    /// Get the next Packet in the sequence of packets and increment the offset counter
    /// </summary>
    /// <returns>A byte being the next byte in the packet</returns>
    public byte Next() {
        if (_currentOffset < 0) {
            _currentOffset = 0;
        } else {
            _currentOffset++;
        }

        return GetAt(_currentOffset);
    }

    /// <summary>
    /// Get the packet byte that we are currently pointing to
    /// </summary>
    /// <returns>A byte being the current packet offset byte</returns>
    public byte Current() {
        return _currentOffset < 0 ? Next() : GetAt(_currentOffset);
    }

    /// <summary>
    /// Get the packet byte that we are currently pointing to
    /// </summary>
    /// <returns>A byte being the current packet offset byte</returns>
    public byte First() {
        Reset();
        return Next();
    }

    /// <summary>
    /// Look forward 1 element in the packet array but do not increment the packet counter
    /// </summary>
    /// <returns>A byte being the next packet in the array</returns>
    public byte Peek() {
        return GetAt(_currentOffset + 1);
    }

    /// <summary>
    /// Skip the next byte in the packet and return the one following
    /// </summary>
    /// <returns>A byte being the next packet in the array</returns>
    public byte Skip() {
        Next();
        return Next();
    }

    /// <summary>
    /// Resets the packet offset pointer to 0;
    /// </summary>
    public void Reset() {
        _currentOffset = -1;
    }

    /// <summary>
    /// Gets a byte from the Packet Data but ensures that the byte exists first
    /// </summary>
    /// <param name="offset">the offset in the array that we want to get the data from</param>
    /// <returns>A byte from that position in the array</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public byte GetAt(int offset) {
        if (_packetData.Length > offset) return _packetData[offset];
        throw new ArgumentOutOfRangeException(nameof(offset), "Invalid Packet: Not enough Packet Data for provided Offset.");
    }

    /// <summary>
    /// Returns the number of elements in the Data Packet
    /// </summary>
    public int Elements => _packetData.Length;
    
    /// <summary>
    /// Check if the data in the packet is at least as long as length
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public bool IsAtLeastLength(int length) {
        return _packetData.Length >= length;
    }
    
    /// <summary>
    /// Check if the packet is valid by performing a checksum calculation.
    /// XOR bytes 1 & 2 and then XOR the result with the next byte through
    /// to the last byte which is the error bit byte. The result should
    /// always be zero. 
    /// </summary>
    /// <returns>True if the packet is valid, otherwise false</returns>
    public bool IsValidPacket => IsAtLeastLength(2) && _packetData.Aggregate(0, (current, t) => current ^ t) == 0;

    /// <summary>
    /// Return the Packet Data as a set of Binary Numbers separated by -
    /// </summary>
    public string ToBinary => string.Join("-", _packetData.Select(part => Convert.ToString(part, 2).PadLeft(8, '0')));

    public bool Equals(PacketData? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return _packetData.Equals(other._packetData);
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((PacketData)obj);
    }

    public override int GetHashCode() {
        return HashCode.Combine(_packetData);
    }
}