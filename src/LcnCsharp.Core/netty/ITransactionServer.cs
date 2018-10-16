﻿using System;
using System.Collections.Generic;
using System.Text;
using LcnCsharp.Core.framework.task;

namespace LcnCsharp.Core.netty
{
    public interface ITransactionServer
    {
        void CreateTransactionGroup(string groupId);

        TxTaskGroup AddTransactionGroup(string groupId, String taskId);

        int CloseTransactionGroup(string groupId, int state);
    }
}