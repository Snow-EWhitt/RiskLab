﻿using Akka.Actor;
using Risk.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Risk.Akka.Actors
{
    public class IOActor : ReceiveActor
    {
        private readonly IRiskIOBridge riskIOBridge;
        Dictionary<IActorRef, string> players;
        public IOActor(IRiskIOBridge riskIOBridge)
        {
            this.riskIOBridge = riskIOBridge;
            players = new Dictionary<IActorRef, string>();
            Become(Active);
        }

        public void Active()
        {
            Receive<SignupMessage>(msg =>
            {
                var assignedPlayerName = msg.RequestedName;

                if (players.ContainsValue(msg.ConnectionId))
                {
                    riskIOBridge.JoinFailed(msg.ConnectionId);
                    Sender.Tell(new UnableToJoinMessage());
                    return;
                }
                var newPlayer = Context.ActorOf(Props.Create(() => new PlayerActor(assignedPlayerName, msg.ConnectionId)), ActorConstants.PlayerActorName);
                players.Add(newPlayer, msg.ConnectionId);
                Sender.Tell(new ConfirmPlayerSignup(assignedPlayerName));
            });

            Receive<JoinGameResponse>(msg =>
            {
                riskIOBridge.JoinConfirmation(msg.AssignedName, msg.ConnectionId);
            });

            Receive<UnableToJoinMessage>(msg =>
            {
                riskIOBridge.JoinFailed(players[Sender]);
                players.Remove(Sender);
            });
        }

        
    }

}