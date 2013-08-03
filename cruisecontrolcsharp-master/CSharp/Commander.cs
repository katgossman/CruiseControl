using CruiseControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

                foreach(VesselStatus vessel in _currentBoard.MyVesselStatuses)
                {
                    cmds.Add(new Command { vesselid = vessel.Id, action = "load_countermeasures"});
                }

                isFirstRound = false;
            }


            foreach(VesselStatus vessel in _currentBoard.MyVesselStatuses)
            {
                //Move if close to edge
                int minX = lowerRight.X, minY = lowerRight.Y, maxX = 0, maxY = 0;
                foreach(Coordinate section in vessel.Location)
                {
                    if(section.X < minX) minX = section.X;
                    if(section.X > maxX) maxX = section.X;
                    if(section.Y < minY) minY = section.Y;
                    if(section.Y > maxY) maxY = section.Y;
                }
                string direction = "";
                if(minX < _currentBoard.BoardMinCoordinate.X + 2)
                    direction = "east";
                if(minY < _currentBoard.BoardMinCoordinate.Y + 2)
                    direction = "south";
                if(maxX > _currentBoard.BoardMaxCoordinate.X - 2)
                    direction = "west";
                if(maxY > _currentBoard.BoardMaxCoordinate.Y - 2)
                    direction = "north";
                if(minX < _currentBoard.BoardMinCoordinate.X + 1)
                    direction = "east";
                if(minY < _currentBoard.BoardMinCoordinate.Y + 1)
                    direction = "south";
                if(maxX > _currentBoard.BoardMaxCoordinate.X - 1)
                    direction = "west";
                if(maxY > _currentBoard.BoardMaxCoordinate.Y - 1)
                    direction = "north";
                if(!string.IsNullOrEmpty(direction))
                {
                    string command = "move:" + direction;
                    cmds.Add(new Command { vesselid = vessel.Id, action = command});
                }

                //Repair if needed
                if(vessel.Health < (vessel.MaxHealth - 19) && vessel.AllowRepair)
                {
                    int damagedSection = -1;
                    for(int i = 0; i < vessel.DamagedSections.Count; i++)
                    { if(vessel.DamagedSections[i]) damagedSection = i; }
                    if(damagedSection != -1) 
                        cmds.Add(new Command { vesselid = vessel.Id, action = "repair", coordinate = vessel.Location[damagedSection]});
                }
            }
            
            foreach(VesselStatus vessel in _currentBoard.MyVesselStatuses)
            {
                if(!vessel.CounterMeasuresLoaded)
                    cmds.Add(new Command { vesselid = vessel.Id, action = "load_countermeasures"});
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