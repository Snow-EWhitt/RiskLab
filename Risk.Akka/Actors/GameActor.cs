﻿using Akka.Actor;
using Risk.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Risk.Akka.Actors
{
    public class GameActor : ReceiveActor
    {
        private string secretCode { get; set; }
        private List<string> players { get; set; }
        public GameActor(string secretCode)
        {
            this.secretCode = secretCode;
            players = new();
            Become(Starting);
        }

        public void Starting()
        {
            Receive<JoinGameMessage>(msg =>
            {
                var assignedName = AssignName(msg.RequestedName);
                players.Add(assignedName);
                Sender.Tell(new JoinGameResponse(assignedName, msg.ConnectionId));
            });

            Receive<StartGameMessage>(msg =>
            {
                if(secretCode == msg.SecretCode)
                {
                    Become(Running);
                    Sender.Tell(new GameStartingMessage());
                }
                else
                {
                    Sender.Tell(new CannotStartGameMessage());
                }
            });

            
        }

        public void Running()
        {
            Receive<JoinGameMessage>(_ =>
            {
                Sender.Tell(new UnableToJoinMessage());
            });
        }

        private string AssignName(string requestedName)
        {
            int sameNames = 0;
            var assignedPlayerName = requestedName;
            while (players.Contains(assignedPlayerName))
            {
                assignedPlayerName = string.Concat(requestedName, sameNames.ToString());
                sameNames++;
            }
            return assignedPlayerName;
        }


    }
}