using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CruiseControl.Models
{
    public class VesselStatus
    {
        public VesselStatus()
        {
            Location = new List<Coordinate>();
            SonarReport = new List<Coordinate>();
            DamagedSections = new List<bool>();
            AllowedToFireTo = new List<Coordinate>();
            AllowedToMoveTo = new List<List<Coordinate>>();
        }

        public int Id { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Size { get; set; }
        public int CollisionPointsGiven { get; set; }
        public int CollisionPointsReceived { get; set; }
        public int MissileRange { get; set; }
        public int SonarRange { get; set; }
        public int CounterMeasures { get; set; }
        public int MovesUntilRepair { get; set; }
        public bool CounterMeasuresLoaded { get; set; }
        public bool AllowRepair { get; set; }

        public List<Coordinate> Location { get; set; }
        public List<Coordinate> SonarReport { get; set; }
        public List<Coordinate> AllowedToFireTo { get; set; }
        public List<List<Coordinate>> AllowedToMoveTo { get; set; }
        public List<bool> DamagedSections { get; set; }
    }
}