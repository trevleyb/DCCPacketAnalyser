using DCCPacketAnalyser.Analyser.Base;

namespace DCCPacketAnalyser.Analyser.Helpers;

public class MessageTracker {
    private Dictionary<Type, MessageData> _messages = new Dictionary<Type, MessageData>();

    public int Count(IPacketMessage message) => _messages[message.GetType()]?.Count ?? 0; 
    
    public bool IsMessageDuplicated(IPacketMessage message) {
        if (_messages.ContainsKey(message.GetType())) {
            _messages[message.GetType()].Count++;
            var previousMessage = _messages[message.GetType()].Message;
            if (previousMessage.Equals(message)) return true;
            _messages[message.GetType()].Message = message;
            return false;
        }
        _messages.Add(message.GetType(), new MessageData(message));
        return false;
    }
}

public class MessageData(IPacketMessage message) {
    public IPacketMessage Message { get; set; } = message;
    public int            Count   { get; set; } = 0;
}