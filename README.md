# DCCPacketAnalyser

This is a C# project to take a block of bytes which are DCC messages from a DCC bus. 
This was built to take the output from a NCE Packet Analyser and analyse the bytes 
to return objects that represent the different messages. 

The reason to build this was (a) because it was interesting and (b) I want to use the
NCE Packet Analyser (or in fact any packet analyser should work such as a Arduino 
analyser) and then use that to feed into the Railway Controller I am building so that 
messages send from other devices will be detected and reported on (NCE does not report
through the RS232 port or USB port messages that are put onto the bus through other 
devices.)