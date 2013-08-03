using CruiseControl.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CruiseControl.Models
{
    public class BoardStatus
    {
        // Board properties
        public int RoundNumber { get; set; }
        public int ActiveVesselCount { get; set; }
        public List<TeamVesselInformation> VesselCountPerTeamId { get; set; }
        public int SunkVesselCount { get; set; }
        public Coordinate BoardMinCoordinate { get; set; }
        public Coordinate BoardMaxCoordinate { get; set; }
        public int TurnsUntilBoardShrink { get; set; }
        public bool ClusterMissleIsOnGameBoard { get; set; }
        public Coordinate ClusterMissleLocation { get; set; }

        // Player specific properties
        public List<Coordinate> HitCoordinates { get; set; }
        public List<Coordinate> MissCoordinates { get; set; }
        public List<VesselStatus> MyVesselStatuses { get; set; }
        public List<PowerUpType> MyPowerUps { get; set; }
        public int MyTeamId { get; set; }

        // Truce specific properties
        public List<int> TruceOfferedBy { get; set; }
        public List<Truce> ActiveTruces { get; set; }
        public List<Coordinate> CoordinatesFromTruces { get; set; }
        public List<Coordinate> CoordinatesOfAllies { get; set; }
        public List<VesselStatus> SonarReportsOfAllies { get; set; }
    }
}