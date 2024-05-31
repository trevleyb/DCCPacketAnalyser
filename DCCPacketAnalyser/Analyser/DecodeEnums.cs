namespace DCCPacketAnalyser.Analyser;

public enum SpeedStepsEnum {
    Unknown = 0,
    Steps14 = 14,
    Steps28 = 28, 
    Steps128 = 128
}

public enum DirectionEnum {
    Forward, 
    Reverse,
    Stop,
    EStop,
    Unknown
}

public enum AccessoryStateEnum {
    Normal   = 0,
    Reversed = 1,
    Off      = 0, 
    On       = 1
}

public enum ControlMessageTypeEnum {
    None,
    Reserved,
    DecoderAck,
    DecoderFlags, 
    AdvancedAddressing,
    Reset,
    HardReset,
    FactoryTest,
    Error,
    Consist
}

public enum DecoderMessageTypeEnum {
    DecoderAndConsist,
    AdvancedOperation,
    SpeedAndDirectionForReverse,
    SpeedAndDirectionForForward,
    FunctionGroupOne,
    FunctionGroupTwo,
    ExtendedFunctions,
    ConfigurationVariables
}

public enum DecoderMessageSubTypeEnum {
    Reserved, 
    SpeedStepControl, 
    RestrictedSpeedStep,
    AnalogFunctionGroup,
    BinaryStateLong, 
    BinaryStateShort,
    FunctionsF0_F4,
    FunctionsF5_F8,
    FunctionsF9_F12,
    FunctionsF13_F20,
    FunctionsF21_F28,
    FunctionsF29_F36,
    FunctionsF37_F44,
    FunctionsF45_F52,
    FunctionsF53_F60,
    FunctionsF61_F68,
    
}


public enum AddressTypeEnum {
    Unknown,
    Idle,
    Broadcast,
    Short,
    Long, 
    Accessory,
    Signal,
    Error,
    Control
}

public enum SignalAspectEnums {
    Red             = 0,
    Yellow          = 1,
    Green           = 2,
    FlashRed        = 3,
    FlashYellow     = 4,
    FlashGreen      = 5,
    RedYellow       = 6,
    FlashRedYellow  = 7,
    RedFlashYellow  = 8,
    RedGreen        = 9,
    FlashRedGreen   = 10,
    RedFlashGreen   = 11,
    YellowGreen     = 12,
    FlashYellowGreen = 13,
    YellowFlashGreen = 14,
    AllOn           = 15,
    AllFlash        = 30,
    AllOff          = 31
}