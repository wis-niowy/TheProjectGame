﻿using GameArea;
using GameArea.Parsers;
using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Player
{
    public partial class Player
    {
        public T PrepareMessageObject<T>(string guid, ulong gameId) where T: GameMessage, new()
        {
            T msg = new T
            {
                playerGuid = guid,
                gameId = gameId
            };
            return msg;
        }

        /// <summary>
        /// Method to send request to test the piece
        /// </summary>
        /// <param name="gameMaster">Addressee of the request</param>
        /// <returns>True - request was valid; False - request was not valid</returns>
        public bool TestPiece(IGameMaster gameMaster)
        {
            if (GetPiece != null)
                ConsoleWriter.Show(GUID + " tries to test piece: " + GetPiece.id + " on location: " + GetLocation);
            TestPiece msg = PrepareMessageObject<TestPiece>(this.GUID, this.gameId);
            string xmlResponse = gameMaster.HandleActionRequest(MessageParser.SerializeObjectToXml<TestPiece>(msg));
            Data responseMessage = MessageParser.DeserializeXmlToObject<Data>(xmlResponse);
            if (responseMessage.gameFinished)
                gameFinished = true;

            return UpdateLocalBoard(responseMessage, ActionType.TestPiece);
        }

        /// <summary>
        /// Method to send a request to place the piece
        /// </summary>
        /// <param name="gameMaster"></param>
        /// <returns></returns>
        public bool PlacePiece(IGameMaster gameMaster)
        {
            ConsoleWriter.Show(guid + " places piece: " + piece.id + " of type: " + piece.type + " on location: " + GetLocation);
            // should we check if received location is the same as the actual one?
            PlacePiece msg = PrepareMessageObject<PlacePiece>(this.GUID, this.GameId);
            string xmlResponse = gameMaster.HandleActionRequest(MessageParser.SerializeObjectToXml<PlacePiece>(msg));
            Data responseMessage = MessageParser.DeserializeXmlToObject<Data>(xmlResponse);
            if (responseMessage.gameFinished)
                gameFinished = true;

            var receivedLocation = responseMessage.PlayerLocation;

            return UpdateLocalBoard(responseMessage, ActionType.PlacePiece);
        }

        public bool PickUpPiece(IGameMaster gameMaster)
        {
            ConsoleWriter.Show(guid + " picks up piece on location: " + GetLocation);
            PickUpPiece msg = PrepareMessageObject<PickUpPiece>(this.GUID, this.GameId);
            string xmlResponse = gameMaster.HandleActionRequest(MessageParser.SerializeObjectToXml<PickUpPiece>(msg));
            Data responseMessage = MessageParser.DeserializeXmlToObject<Data>(xmlResponse);
            if (responseMessage.gameFinished)
                gameFinished = true;

            return UpdateLocalBoard(responseMessage, ActionType.PickUpPiece);
        }

        public bool Move(IGameMaster gameMaster, MoveType direction)
        {
            ConsoleWriter.Show(guid + " wants to move from: " + GetLocation + " in direction: " + direction);
            Move msg = PrepareMessageObject<Move>(this.GUID, this.GameId);
            msg.direction = direction;
            msg.directionSpecified = true;
            string xmlResponse = gameMaster.HandleActionRequest(MessageParser.SerializeObjectToXml<Move>(msg));
            Data responseMessage = MessageParser.DeserializeXmlToObject<Data>(xmlResponse);
            if (responseMessage.gameFinished)
                gameFinished = true;

            return MoveUpdate(responseMessage, direction); // ---- dla tego sypia sie 3 testy - do sprawdzenia pozniej!
        }


        public void Discover(IGameMaster gameMaster)
        {
            ConsoleWriter.Show(guid + " discovers on location: " + GetLocation);
            Discover msg = PrepareMessageObject<Discover>(this.GUID, this.GameId);
            string xmlResponse = gameMaster.HandleActionRequest(MessageParser.SerializeObjectToXml<Discover>(msg));
            Data responseMessage = MessageParser.DeserializeXmlToObject<Data>(xmlResponse);
            if (responseMessage.gameFinished)
                gameFinished = true;
            UpdateLocalBoard(responseMessage, ActionType.Discover);
        }

        public bool Destroy(IGameMaster gameMaster)
        {
            ConsoleWriter.Show(guid + " tries to destroy piece: " + piece.id + " which is: " + piece.type + "on location: " + GetLocation);
            DestroyPiece msg = PrepareMessageObject<DestroyPiece>(this.GUID, this.GameId);
            string xmlResponse = gameMaster.HandleActionRequest(MessageParser.SerializeObjectToXml<DestroyPiece>(msg));
            Data responseMessage = MessageParser.DeserializeXmlToObject<Data>(xmlResponse);
            if (responseMessage.gameFinished)
                gameFinished = true;
            return UpdateLocalBoard(responseMessage, ActionType.Destroy);
        }
        
        // additional methods

        private bool UpdateLocalBoard(Data responseMessage, ActionType action, MoveType direction = MoveType.up)
        {
            bool result = false;

            var gameFinished = responseMessage.gameFinished;
            var playerId = responseMessage.playerId;

            if (playerId == this.ID && !gameFinished)
            {
                switch (action)
                {
                    case ActionType.TestPiece:
                        result = TestPieceUpdate(responseMessage);
                        break;
                    case ActionType.PlacePiece:
                        result = PlacePieceUpdate(responseMessage);
                        break;
                    case ActionType.PickUpPiece:
                        result = PickUpPieceUpdate(responseMessage);
                        break;
                    case ActionType.Move:
                        result = MoveUpdate(responseMessage, direction);
                        break;
                    case ActionType.Discover:
                        DiscoverUpdate(responseMessage);
                        break;
                    case ActionType.Destroy:
                        result = DestroyUpdate(responseMessage);
                        break;
                }
            }

            ConsoleWriter.Show("Updated state by: " + guid + " from action: " + action);

            return result;
        }

        private bool DestroyUpdate(Data responseMessage)
        {
            var resultValue = false;

            var piecesArray = responseMessage.Pieces;
            if (piecesArray != null && piecesArray.Length > 0 && piecesArray[0] == null && HasPiece)
                resultValue =  true; //poprawna akcja, miał piece, usunął
            piece = null;
            return resultValue; //akcja bez sensu - nie miał piece
        }

        private bool TestPieceUpdate(Data responseMessage)
        {
            bool resultValue = false;

            var taskFieldsArray = responseMessage.TaskFields;
            var goalFieldsArray = responseMessage.GoalFields;
            var piecesArray = responseMessage.Pieces;

            if (piecesArray != null && piecesArray.Length > 0 && piecesArray[0] != null) // otzymano informacje o kawalku
                                                                                         // piecesArray[0] != null oznacza, ze akcja byla poprawna
            {
                var receivedPiece = piecesArray[0];
                if (receivedPiece.type != PieceType.unknown && this.GetPiece.id == receivedPiece.id)  // testowano kawalek -- wynik normal lub sham
                {
                    this.piece = receivedPiece; // aktualizacja lokalnego kawalka
                    this.piece.timestamp = DateTime.Now;
                    resultValue = true;
                }
            }

            return resultValue;
        }
        private bool PlacePieceUpdate(Data responseMessage)
        {
            bool resultValue = false;

            var taskFieldsArray = responseMessage.TaskFields;
            var goalFieldsArray = responseMessage.GoalFields;
            var piecesArray = responseMessage.Pieces;

            if (taskFieldsArray != null && taskFieldsArray.Length > 0 && taskFieldsArray[0] != null) // Player kladzie kawalek na wolne pole
                                                                                                     // jezeli taskFieldsArray[0] == null to probowano polozyc na zajetym TaskField
            {
                var receivedField = taskFieldsArray[0];
                var field = this.PlayerBoard.GetTaskField(location.x, location.y);
                field.SetPiece(this.GetPiece); // odkladamy kawalek
                field.UpdateTimeStamp(DateTime.Now);
                this.SetPiece(null);
                resultValue = true;
            }
            else if (goalFieldsArray != null && goalFieldsArray.Length > 0 && goalFieldsArray[0] != null) // Player kladzie kawalek na GoalField                                                                                              
            {
                var receivedField = goalFieldsArray[0];
                if (receivedField.type == GoalFieldType.goal) // Player trafil gola - puszcza kawalek
                {
                    this.SetPiece(null);
                    resultValue = true;
                }
                else if (receivedField.type == GoalFieldType.nongoal) // Player chybil probujac kawalkiem 'normal'
                {
                    var field = PlayerBoard.GetGoalField(location.x, location.y);
                    field.GoalType = GoalFieldType.nongoal;
                    field.UpdateTimeStamp(DateTime.Now);
                    resultValue = false;
                }
                else // (receivedField.type == GoalFieldType.unknown) -- polozono sham na GoalField - zadnych info o polu, wiec wiemy ze Player ma typ sham
                {
                    this.GetPiece.type = PieceType.sham;
                    this.GetPiece.timestamp = DateTime.Now;
                    resultValue = false;
                }
            }

            return resultValue;
        }
        private bool PickUpPieceUpdate(Data responseMessage)
        {
            bool resultValue = false;

            var taskFieldsArray = responseMessage.TaskFields;
            var goalFieldsArray = responseMessage.GoalFields;
            var piecesArray = responseMessage.Pieces;

            if (piecesArray != null && piecesArray.Length > 0 && piecesArray[0] != null)
            {
                var receivedPiece = piecesArray[0];
                this.SetPiece(receivedPiece);
                this.GetPiece.timestamp = DateTime.Now;
                PlayerBoard.GetTaskField(location.x, location.y).SetPiece(null);
                resultValue = true;
            }
            else
            {
                ConsoleWriter.Warning(guid + " has not picked a piece on location:" + location);
            }

            return resultValue;
        }
        private bool MoveUpdate(Data responseMessage, MoveType direction)
        {
            bool resultValue = false;
            // MoveUpdate oraz gameMaster.HandleMoveRequest updatuja lokacje Playera przez to potrafi ruszyc sie 2 razy
            var futureLocation = CalculateFutureLocation(this.location, direction);
            var currentLocation = responseMessage.PlayerLocation;
            var taskFieldsArray = responseMessage.TaskFields;
            var goalFieldsArray = responseMessage.GoalFields;
            var piecesArray = responseMessage.Pieces;

            // an attempt to exceed board's boundaries or to enter an opponent's GoalArea
            if (responseMessage.TaskFields != null && responseMessage.TaskFields.Length == 0)
            {
                this.location = responseMessage.PlayerLocation;
                resultValue = false;
            }
            // future position is a TaskField
            else if (responseMessage.TaskFields != null && responseMessage.TaskFields.Length > 0)
            {
                // an Player attempted to enter an occupied TaskField
                if (this.location.Equals(responseMessage.PlayerLocation))
                {
                    // add encountered stranger Player to this Player's view
                    var stranger = new Messages.Player()
                    {
                        id = (ulong)responseMessage.TaskFields[0].playerId,
                    };
                    PlayerBoard.GetField(futureLocation.x, futureLocation.y).Player = stranger;

                    resultValue = false;
                }
                // an action was valid
                else
                {
                    this.location = responseMessage.PlayerLocation;
                    resultValue = true;
                }
            }
            // future position is a GoalField
            else if (responseMessage.GoalFields != null && responseMessage.GoalFields.Length > 0)
            {
                // an Player attempted to enter an occupied GoalField
                if (this.location.Equals(responseMessage.PlayerLocation))
                {
                    // add encountered stranger Player to this Player's view
                    var stranger = new Messages.Player()
                    {
                        id = (ulong)responseMessage.GoalFields[0].playerId,
                    };
                    PlayerBoard.GetField(futureLocation.x, futureLocation.y).Player = stranger;

                    resultValue = false;
                }
                // an action was valid
                else
                {
                    this.location = responseMessage.PlayerLocation;
                    resultValue = true;
                }
            }
            return resultValue;
        }
        private void DiscoverUpdate(Data responseMessage)
        {
            var taskFieldsArray = responseMessage.TaskFields;
            var goalFieldsArray = responseMessage.GoalFields;
            var piecesArray = responseMessage.Pieces;

            if (taskFieldsArray != null && taskFieldsArray.Length > 0)
            {
                foreach (var respField in taskFieldsArray)
                {
                    if (respField != null)
                    {
                        var coordX = respField.x;
                        var coordY = respField.y;

                        GameArea.TaskField updatedField = PlayerBoard.GetField(respField.x, respField.y) as GameArea.TaskField;
                        updatedField.UpdateTimeStamp(respField.timestamp);
                        updatedField.Distance = respField.distanceToPiece;


                        if (respField.playerIdSpecified)
                            updatedField.Player = new Messages.Player()
                            {
                                id = respField.playerId
#warning Wymaga napisania metody do otrzymywania listy wszystkich graczy
                                //team = myTeam.Union(otherTeam).First(p => p.id == respField.playerId).team,
                                //type = myTeam.Union(otherTeam).First(p => p.id == respField.playerId).type,
                            };

                        if (respField.pieceIdSpecified)
                            updatedField.SetPiece(new Piece(PieceType.unknown, respField.pieceId));
                    }
                }
            }
            if (goalFieldsArray != null && goalFieldsArray.Length > 0)
            {
                foreach (var respField in goalFieldsArray)
                {
                    if (respField != null)
                    {
                        var coordX = respField.x;
                        var coordY = respField.y;

                        GameArea.GoalField updatedField = PlayerBoard.GetField(respField.x, respField.y) as GameArea.GoalField;
                        updatedField.UpdateTimeStamp(respField.timestamp);

                        if (respField.playerIdSpecified)
                            updatedField.Player = new Messages.Player()
                            {
                                id = respField.playerId
#warning Wymaga napisania metody do otrzymywania listy wszystkich graczy
                                //team = myTeam.Union(otherTeam).First(p => p.id == respField.playerId).team,
                                //type = myTeam.Union(otherTeam).First(p => p.id == respField.playerId).type,
                            };
                    }
                }
            }

        }

        // helpers ---------------------

        private Location CalculateFutureLocation(Location oldLocation, MoveType direction)
        {
            Location newLocation = null;
            switch (direction)
            {
                case MoveType.up:
                    newLocation = new Location(oldLocation.x, oldLocation.y + 1);
                    break;
                case MoveType.down:
                    newLocation = new Location(oldLocation.x, oldLocation.y - 1);
                    break;
                case MoveType.left:
                    newLocation = new Location(oldLocation.x - 1, oldLocation.y);
                    break;
                case MoveType.right:
                    newLocation = new Location(oldLocation.x + 1, oldLocation.y);
                    break;
            }
            return newLocation;
        }
    }
}
