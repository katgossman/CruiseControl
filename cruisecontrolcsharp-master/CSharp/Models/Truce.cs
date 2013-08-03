using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CruiseControl.Models
{
    public class Truce
    {
        public int Team1Id { get; set; }
        public int Team2Id { get; set; }

        public int AllyTeamId { get; set; }

        // A decrementing count of how many rounds the truce is good for
        public int EffectiveRoundsLeft { get; set; }
    }
}