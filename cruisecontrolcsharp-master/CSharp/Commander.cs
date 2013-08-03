using CruiseControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CruiseControl.Enums;

namespace CruiseControl
{
    public class Commander
    {
        public BoardStatus _currentBoard;
        private bool isFirstRound;
        private string[][] boardVisual;
        private Coordinate lowerRight;

        public Commander()
        {
            _currentBoard = new BoardStatus();
            lowerRight = new Coordinate();
        }

        // Do not alter/remove this method signature
        public List<Command> GiveCommands()
        {
            var cmds = new List<Command>();

            if (isFirstRound)
            {
                int xSize = _currentBoard.BoardMaxCoordinate.X + 1;
                int ySize = _currentBoard.BoardMaxCoordinate.Y + 1;
                boardVisual = new string[xSize][];
                for(int i = 0; i < xSize; i++)
                {
                    boardVisual[i] = new string[ySize];
                }
                lowerRight = new Coordinate {X = _currentBoard.BoardMaxCoordinate.X, Y = _currentBoard.BoardMaxCoordinate.Y};

                isFirstRound = false;
            }

            int haveInstantRepair = -1;
            int haveExtraCounterMeasure = -1;
            int haveClusterMissle = -1;
            int powerUpIndex = 0;
            foreach (PowerUpType powerUp in _currentBoard.MyPowerUps)
            {
                switch (powerUp)
                {
                    case PowerUpType.ClusterMissle:
                        haveClusterMissle = powerUpIndex;
                        break;
                    case PowerUpType.InstantRepair:
                        haveInstantRepair = powerUpIndex;
                        break;
                    case PowerUpType.BoostRadar:

                        break;
                    case PowerUpType.ExtraCounterMeasures:
                        haveExtraCounterMeasure = powerUpIndex;
                        break;
                }
                powerUpIndex++;
            }

            if (haveClusterMissle > -1)
            {
                int minX, maxX, minY, maxY;
                minX = _currentBoard.BoardMinCoordinate.X;
                maxX = _currentBoard.BoardMaxCoordinate.X;
                minY = _currentBoard.BoardMinCoordinate.Y;
                maxY = _currentBoard.BoardMaxCoordinate.Y;
                for(int i = 0; i <= lowerRight.X; i++)
                {
                    for(int j = 0; j <= lowerRight.Y; j++)
                    {
                        boardVisual[i][j] = "NO";
                    }
                }
                foreach (VesselStatus vessel in _currentBoard.MyVesselStatuses)
                {
                    foreach (Coordinate section in vessel.Location)
                    {
                        boardVisual[section.X - 1][section.Y - 1] = "NO";
                        boardVisual[section.X - 1][section.Y] = "NO";
                        boardVisual[section.X - 1][section.Y + 1] = "NO";
                        boardVisual[section.X][section.Y - 1] = "NO";
                        boardVisual[section.X][section.Y] = "NO";
                        boardVisual[section.X][section.Y + 1] = "NO";
                        boardVisual[section.X + 1][section.Y - 1] = "NO";
                        boardVisual[section.X + 1][section.Y] = "NO";
                        boardVisual[section.X + 1][section.Y + 1] = "NO";
                    }
                }
                for (int i = 0; i <= lowerRight.X; i++)
                {
                    for (int j = 0; j <= lowerRight.Y; j++)
                    {
                        if (!string.Equals(boardVisual[i][j], "NO"))
                        {
                            cmds.Add(new Command { vesselid = _currentBoard.MyVesselStatuses[0].Id, action = "power_up:" + haveClusterMissle.ToString(), coordinate = new Coordinate { X = i, Y = j } });
                        }
                    }
                }
            }


            //TEST CODE
            //foreach (VesselStatus vessel in _currentBoard.MyVesselStatuses)
            //{
            //    cmds.Add(new Command { vesselid = vessel.Id, action = "move:south" });
                
            //}




            foreach(VesselStatus vessel in _currentBoard.MyVesselStatuses)
            {
                if (vessel.MovesUntilRepair == -1)
                {

                    //Move if close to edge
                    int minX = lowerRight.X, minY = lowerRight.Y, maxX = 0, maxY = 0;
                    foreach (Coordinate section in vessel.Location)
                    {
                        if (section.X < minX) minX = section.X;
                        if (section.X > maxX) maxX = section.X;
                        if (section.Y < minY) minY = section.Y;
                        if (section.Y > maxY) maxY = section.Y;
                    }

                    string direction = "";

                    bool vertical = false;
                    if (minX == maxX) vertical = true;
                    if (vessel.Health < vessel.MaxHealth)
                    {
                        if (vertical) direction = "east";
                        else direction = "south";
                    }

                    if (minX < _currentBoard.BoardMinCoordinate.X + 2)
                        direction = "east";
                    if (minY < _currentBoard.BoardMinCoordinate.Y + 2)
                        direction = "south";
                    if (maxX > _currentBoard.BoardMaxCoordinate.X - 2)
                        direction = "west";
                    if (maxY > _currentBoard.BoardMaxCoordinate.Y - 2)
                        direction = "north";
                    if (minX < _currentBoard.BoardMinCoordinate.X + 1)
                        direction = "east";
                    if (minY < _currentBoard.BoardMinCoordinate.Y + 1)
                        direction = "south";
                    if (maxX > _currentBoard.BoardMaxCoordinate.X - 1)
                        direction = "west";
                    if (maxY > _currentBoard.BoardMaxCoordinate.Y - 1)
                        direction = "north";
                    if (!string.IsNullOrEmpty(direction))
                    {
                        string command = "move:" + direction;
                        cmds.Add(new Command { vesselid = vessel.Id, action = command });
                    }

                    //Repair if needed
                    if (vessel.Health < (vessel.MaxHealth - 19) && vessel.AllowRepair)
                    {
                        int damagedSection = -1;
                        for (int i = 0; i < vessel.DamagedSections.Count; i++)
                        { if (vessel.DamagedSections[i]) damagedSection = i; }
                        if (damagedSection != -1)
                        {
                            if (haveInstantRepair > -1)
                            {
                                cmds.Add(new Command { vesselid = vessel.Id, action = "power_up:" + haveInstantRepair.ToString(), coordinate = vessel.Location[damagedSection] });
                            }
                            else
                            {
                                cmds.Add(new Command { vesselid = vessel.Id, action = "repair", coordinate = vessel.Location[damagedSection] });
                            }
                        }

                    }

                }
            }
            
            foreach(VesselStatus vessel in _currentBoard.MyVesselStatuses)
            {
                if (!vessel.CounterMeasuresLoaded)
                {
                    if (haveExtraCounterMeasure > -1)
                    {
                        cmds.Add(new Command { vesselid = vessel.Id, action = "power_up:" + haveExtraCounterMeasure.ToString() });
                    }
                    else
                    {
                        cmds.Add(new Command { vesselid = vessel.Id, action = "load_countermeasures" });
                    }
                }
            }

            // Add Commands Here.
            // You can only give as many commands as you have un-sunk vessels. Powerup commands do not count against this number. 
            // You are free to use as many powerup commands at any time. Any additional commands you give (past the number of active vessels) will be ignored.
			//cmds.Add(new Command { vesselid = 1, action = "fire", coordinate = new Coordinate { X = 1, Y = 1 } });

            return cmds;
        }

        // Do NOT modify or remove! This is where you will receive the new board status after each round.
        public void GetBoardStatus(BoardStatus board)
        {
            _currentBoard = board;
        }

        // This method runs at the start of a new game, do any initialization or resetting here 
        public void Reset()
        {
            isFirstRound = true;
        }
    }
}