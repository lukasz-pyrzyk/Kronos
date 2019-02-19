﻿using Kronos.Core.Messages;
using Kronos.Core.Storage;

namespace Kronos.Core.Processing
{
    public class ClearProcessor : CommandProcessor<ClearRequest, ClearResponse>
    {
        public override ClearResponse Reply(ClearRequest request, IStorage storage)
        {
            int deleted = storage.Clear();

            return new ClearResponse { Deleted = deleted };
        }
    }
}
