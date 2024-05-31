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
    ConfigurationVariables,
    Error
}

public enum DecoderMessageSubTypeEnum {
    Reserved, 
    SpeedStepControl, 
    RestrictedSpeedStep,
    AnalogFunctionGroup,
    BinaryStateLong, 
    BinaryStateShort,
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
    Acceleration,
    Deceleration,
    Lock,
    VerifyCV,
    WriteCV,
    BitManipulate
    
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