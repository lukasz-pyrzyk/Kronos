﻿using Google.Protobuf;
using Kronos.Server.Storage;

namespace Kronos.Server.Processing
{
    abstract class CommandProcessor<TRequest, TResponse>
        where TRequest : IMessage
        where TResponse : IMessage
    {
        public abstract TResponse Reply(TRequest request, InMemoryStorage storage);
    }
}
