﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hive5
{
    public class CallMessage : SpiderRequestMessage
    {
        public string ProcedureUri { get; set; }

        public CallOptions Options { get; set; }

        public CallMessage()
        {
            this.MessageCode = (int)WampMessageCode.CALL;
            this.Options = new CallOptions();
        }

        public override string ToJson()
        {
            List<object> messageObjects = new List<object>();
            messageObjects.Add(this.MessageCode);
            messageObjects.Add(this.RequestId);
            messageObjects.Add(this.Options);
            messageObjects.Add(this.ProcedureUri);

            string jsonMessage = JsonHelper.ToJson(messageObjects);
            return jsonMessage;
        }
    }
}
