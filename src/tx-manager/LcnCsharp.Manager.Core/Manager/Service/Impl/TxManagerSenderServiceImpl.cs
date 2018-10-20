﻿using System.Collections.Generic;
using LcnCsharp.Common.Logging;
using LcnCsharp.Manager.Core.Config;
using LcnCsharp.Manager.Core.Model;
using LcnCsharp.Manager.Core.Netty.Model;
using LcnCsharp.Manager.Core.Redis.Service;
using LcnCsharp.Manager.Core.Utils;
using Microsoft.Extensions.Logging;

namespace LcnCsharp.Manager.Core.Manager.Service.Impl
{
    public class TxManagerSenderServiceImpl:ITxManagerSenderService
    {
        private readonly ILogger _logger =
            LcnCsharpLogManager.LoggerFactory.CreateLogger(typeof(TxManagerSenderServiceImpl));

        private ITxManagerService txManagerService;
        private readonly IRedisServerService _redisServerService;
        private readonly ConfigReader configReader;
        public int Confirm(TxGroup @group)
        {
            //绑定管道对象，检查网络
            SetChannel(@group.GetList());
            //事务不满足直接回滚事务
            if (@group.State == 0)
            {
                Transaction(@group, 0);
                return 0;
            }

            if (@group.Rollback == 1)
            {
                Transaction(@group, 0);
                return -1;
            }

            bool hasOk = Transaction(@group, 1);
            txManagerService.DealTxGroup(@group, hasOk);
            return hasOk ? 1 : 0;

        }

        private bool Transaction(TxGroup txGroup, int index)
        {

            return false;
        }

        public string SendMsg(string model, string msg, int delay)
        {
            throw new System.NotImplementedException();
        }

        public string SendCompensateMsg(string model, string groupId, string data, int startState)
        {
            throw new System.NotImplementedException();
        }

         /**
         * 匹配管道
         *
         * @param list
         */
        private void SetChannel(List<TxInfo> list)
        {
            foreach (TxInfo info in list)
            {
                if (Constants.Address.Equals(info.Address))
                {
                    var channel = SocketManager.GetInstance().GetChannelByModelName(info.ChannelAddress);
                    if (channel != null && channel.Active)
                    {
                        ChannelSender sender = new ChannelSender();
                        sender.Channel = channel;

                        info.Channel=(sender);
                    }
                }
                else
                {
                    var sender = new ChannelSender();
                    sender.Address=info.Address;
                    sender.ModelName=info.ChannelAddress;

                    info.Channel = sender;
                }
            }
        }
    }
}