﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Risk.Shared
{
    public static class MessageTypes
    {
        public const string ReceiveMessage = "ReceiveMessage";
        public const string SendMessage = "SendMessage";

        public const string YourTurnToDeploy = "YourTurnToDeploy";
        public const string GetStatus = "GetStatus";
        public const string SendStatus = "SendStatus";
        public const string StartGame = "StartGame";
        public const string DeployRequest = "DeployRequest";
        public const string AttackRequest = "AttackRequest";
        public const string Signup = "Signup";
    }
}
