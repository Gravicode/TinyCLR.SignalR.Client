﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace TinyCLR.SignalR.Client
{
    internal class InvocationBlockingSendMessage : InvocationSendMessage
    {
        public string invocationId { get; set; }
    }
}
