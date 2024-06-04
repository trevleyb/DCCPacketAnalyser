using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class FunctionsMessage (IPacketMessage packet) : PacketMessage(packet.PacketData, packet.AddressType, packet.Address), IEquatable<FunctionsMessage> {

    private int                from;
    private int                to;
    public  FunctionsGroupEnum Group     { get; init; }
    public  bool[]             Functions { get; } = new bool[69];

    public FunctionsMessage(IPacketMessage packet, byte dataByte, FunctionsGroupEnum group) : this(packet) {
        Group = group;
        _ = group switch {
            FunctionsGroupEnum.FunctionsF0F4   => SetFunction(dataByte, 1, 4),
            FunctionsGroupEnum.FunctionsF5F8   => SetFunction(dataByte, 5, 8),
            FunctionsGroupEnum.FunctionsF9F12  => SetFunction(dataByte, 9, 12),
            FunctionsGroupEnum.FunctionsF13F20 => SetFunction(dataByte, 13, 20),
            FunctionsGroupEnum.FunctionsF21F28 => SetFunction(dataByte, 21, 28),
            FunctionsGroupEnum.FunctionsF29F36 => SetFunction(dataByte, 29, 36),
            FunctionsGroupEnum.FunctionsF37F44 => SetFunction(dataByte, 37, 44),
            FunctionsGroupEnum.FunctionsF45F52 => SetFunction(dataByte, 45, 52),
            FunctionsGroupEnum.FunctionsF53F60 => SetFunction(dataByte, 53, 60),
            FunctionsGroupEnum.FunctionsF61F68 => SetFunction(dataByte, 61, 68),
            _                                  => throw new ArgumentOutOfRangeException(nameof(packet),"Invalid Function Block")
        };

        // Special Case for F0 which is held in the 4th bit
        if (group == FunctionsGroupEnum.FunctionsF0F4) {
            Functions[0]   = dataByte.GetBit(4);
        }
    }

    public override string ToString() {
        return FormatHelper.FormatMessage("FUNCTIONS", base.ToString(), PacketData, ("Group",Group.ToString()),("Functions",OutputFunctions(from,to)) );
    }
    
    /// <summary>
    /// Function to reformat the Functions so they can be displayed. Format as X-X-X-X- where X is On, - is OFF
    /// </summary>
    /// <param name="fromF">First Function Number (0)</param>
    /// <param name="toF">Last Function Number (68)</param>
    /// <returns>A string representing the state of the functions. </returns>
    public string OutputFunctions(int fromF, int toF) {
        if (fromF <= 0 || fromF > Functions.Length - 1 || toF < 0 || toF > Functions.Length - 1 || toF < fromF) return "--------";
        var functionBlock = "";
        for (var i = fromF; i <= toF; i++) {
            functionBlock += Functions[i] ? "X" : "-";
        }
        return functionBlock;
    }

    private bool SetFunction(byte dataByte, byte start, byte end) {
        from = start;
        to = end;
        for (var func = start; func <= end; func++) {
            Functions[func] = dataByte.GetBit(func - start);
        }
        return true;
    }

    public bool Equals(FunctionsMessage? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Address != other.Address) return false;
        return Group == other.Group && Functions.Equals(other.Functions);
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((FunctionsMessage)obj);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Address,(int)Group, Functions);
    }
}

public enum FunctionsGroupEnum {
    FunctionsF0F4,
    FunctionsF5F8,
    FunctionsF9F12,
    FunctionsF13F20,
    FunctionsF21F28,
    FunctionsF29F36,
    FunctionsF37F44,
    FunctionsF45F52,
    FunctionsF53F60,
    FunctionsF61F68,
}
