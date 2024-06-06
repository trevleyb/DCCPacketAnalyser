using DCCPacketAnalyser.Analyser.Base;
using DCCPacketAnalyser.Analyser.Helpers;

namespace DCCPacketAnalyser.Analyser.Messages;

public class FunctionsMessage : PacketMessage, IEquatable<FunctionsMessage> {
    private int                _from;
    private int                _to;
    private bool[]             Functions { get; } = new bool[69];
    public  FunctionsGroupEnum Group     { get; }
    public  byte               BitValues { get; private set; }

    public FunctionsMessage(IPacketMessage packet, byte dataByte, FunctionsGroupEnum group) : base(packet.PacketData, packet.AddressType, packet.Address) {
        Group = group;
        _ = group switch {
            FunctionsGroupEnum.F0F4   => SetFunction(dataByte, 1, 4),
            FunctionsGroupEnum.F5F8   => SetFunction(dataByte, 5, 8),
            FunctionsGroupEnum.F9F12  => SetFunction(dataByte, 9, 12),
            FunctionsGroupEnum.F13F20 => SetFunction(dataByte, 13, 20),
            FunctionsGroupEnum.F21F28 => SetFunction(dataByte, 21, 28),
            FunctionsGroupEnum.F29F36 => SetFunction(dataByte, 29, 36),
            FunctionsGroupEnum.F37F44 => SetFunction(dataByte, 37, 44),
            FunctionsGroupEnum.F45F52 => SetFunction(dataByte, 45, 52),
            FunctionsGroupEnum.F53F60 => SetFunction(dataByte, 53, 60),
            FunctionsGroupEnum.F61F68 => SetFunction(dataByte, 61, 68),
            _                         => throw new ArgumentOutOfRangeException(nameof(packet), "Invalid Function Block")
        };

        // Special Case for F0 which is held in the 4th bit
        // ------------------------------------------------
        if (group == FunctionsGroupEnum.F0F4) {
            _from        = 0;
            Functions[0] = dataByte.GetBit(4);
        }
    }

    public override string Summary => $"{AddressAsString} {Group}:{OutputFunctions(_from, _to)}";

    public override string ToString() {
        return FormatHelper.FormatMessage("FUNCTIONS", base.ToString(), PacketData, ("Group", Group.ToString()), ("Functions", OutputFunctions(_from, _to)));
    }

    /// <summary>
    /// Function to reformat the Functions so they can be displayed. Format as X-X-X-X- where X is On, - is OFF
    /// </summary>
    /// <param name="fromF">First Function Number (0)</param>
    /// <param name="toF">Last Function Number (68)</param>
    /// <returns>A string representing the state of the functions. </returns>
    // ReSharper disable once MemberCanBePrivate.Global
    public string OutputFunctions(int fromF, int toF) {
        if (fromF < 0 || fromF > Functions.Length - 1 || toF < 0 || toF > Functions.Length - 1 || toF < fromF) return "--------";
        var functionBlock = "";
        for (var i = fromF; i <= toF; i++) {
            functionBlock += Functions[i] ? "●" : "○";
        }

        return functionBlock;
    }

    private bool SetFunction(byte dataByte, byte start, byte end) {
        _from     = start;
        _to       = end;
        BitValues = 0;
        for (var func = start; func <= end; func++) {
            Functions[func] = dataByte.GetBit(func - start);
            BitValues       = BitValues.SetBit(func - start, Functions[func]);
        }

        return true;
    }

    public bool Equals(FunctionsMessage? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Address != other.Address) return false;
        if (Group != other.Group) return false;
        return !Functions.Where((t, i) => t != other.Functions[i]).Any();
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((FunctionsMessage)obj);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Address, (int)Group, Functions);
    }
}

public enum FunctionsGroupEnum {
    F0F4,
    F5F8,
    F9F12,
    F13F20,
    F21F28,
    F29F36,
    F37F44,
    F45F52,
    F53F60,
    F61F68
}