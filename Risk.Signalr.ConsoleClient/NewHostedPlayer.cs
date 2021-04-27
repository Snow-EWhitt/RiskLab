using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Risk.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Risk.Signalr.ConsoleClient
{
    public class NewHostedPlayer : HostedPlayerLogic
    {
        private IEnumerable<BoardTerritory> nonEdgeTerritories;

        public NewHostedPlayer(IConfiguration configuration, IHostApplicationLifetime applicationLifetime) : base(configuration, applicationLifetime)
        {
        }

        public override string MyPlayerName { get; set; } = "Caleb and Ethan";

        public override Location WhereDoYouWantToDeploy(IEnumerable<BoardTerritory> board)
        {
            var totalColumn = board.Select(t => t.Location.Column).Distinct().Count();
            var totalRow = board.Select(t => t.Location.Row).Distinct().Count();
            nonEdgeTerritories = board.Where(t => t.Location.Column > 0 && t.Location.Row > 0 && t.Location.Column < (totalColumn - 1) && t.Location.Row < (totalRow - 1));
            var evenTerritories = nonEdgeTerritories.Where( t => (t.Location.Row % 2 == 1) && (t.Location.Column % 2 == 1));
            var deployableTerritories = evenTerritories.Where(t => t.OwnerName == MyPlayerName || t.OwnerName == null).OrderBy(t => t.Armies).First();
        

            return deployableTerritories.Location;
        }


        public override (Location from, Location to) WhereDoYouWantToAttack(IEnumerable<BoardTerritory> board)
        {
            foreach (var myTerritory in board.Where(t => t.OwnerName == MyPlayerName).OrderByDescending(t => t.Armies))
            {
                var myNeighbors = GetNeighbors(myTerritory, board);
                var destination = myNeighbors.Where(t => t.OwnerName != MyPlayerName).OrderBy(t => t.Armies).FirstOrDefault();
                if (myNeighbors.Last().Armies != 1)
                {
                     if (myTerritory.Armies <= 3)
                    {
                        continue;
                    }
                }
   
                if (destination != null)
                {
                    return (myTerritory.Location, destination.Location);
                }
            }
            throw new Exception("Unable to find place to attack");
        }
    }
}
