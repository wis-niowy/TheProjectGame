using Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Messages;
using System;

namespace MainApp.Tests
{
    [TestClass]
    public class ValidatorTests
    {
        GameMasterSettings defaultSettings = new GameMasterSettings()
        {
            ActionCosts = GameMasterSettingsActionCosts.GetDefaultCosts(),
            GameDefinition =  GameMasterSettingsGameDefinition.GetDefaultGameDefinition()
        };

        [TestMethod]
        public void ShamProbabiltyValid()
        {
            defaultSettings.GameDefinition.ShamProbability = 0.33d;
            var message = Validator.ValidateShamProbability(defaultSettings.GameDefinition.ShamProbability);
            Assert.IsTrue(string.IsNullOrEmpty(message));
        }

        [TestMethod]
        public void ShamProbabilityNegative()
        {
            defaultSettings.GameDefinition.ShamProbability = -1;
            var message = Validator.ValidateShamProbability(defaultSettings.GameDefinition.ShamProbability);
            Assert.IsFalse(string.IsNullOrEmpty(message));
            Assert.AreEqual(ValidatorMessages.INVALID_SHAM_PROBABILITY, message);
        }

        [TestMethod]
        public void ShamProbabilityOverOne()
        {
            defaultSettings.GameDefinition.ShamProbability = 2;
            var message = Validator.ValidateShamProbability(defaultSettings.GameDefinition.ShamProbability);
            Assert.IsFalse(string.IsNullOrEmpty(message));
            Assert.AreEqual(ValidatorMessages.INVALID_SHAM_PROBABILITY, message);
        }

        //PlacingNewPiecesFrequency
        [TestMethod]
        public void PieceFrequencyZero()
        {
            defaultSettings.GameDefinition.PlacingNewPiecesFrequency = 0;
            var message = Validator.ValidatePiecesFrequency(defaultSettings.GameDefinition.PlacingNewPiecesFrequency);
            Assert.IsFalse(string.IsNullOrEmpty(message));
            Assert.AreEqual(ValidatorMessages.INVALID_PIECES_FREQUENCY, message);
        }

        // Initial Number of Pieces
        [TestMethod]
        public void InitialNumberOfPiecesValid()
        {
            defaultSettings.GameDefinition.InitialNumberOfPieces = 4;
            var message = Validator.ValidateInitialNumberOfPieces(defaultSettings.GameDefinition.InitialNumberOfPieces, defaultSettings.GameDefinition.TaskAreaLength, defaultSettings.GameDefinition.BoardWidth);
            Assert.IsTrue(string.IsNullOrEmpty(message));
        }

        [TestMethod]
        public void InitialNumberOfPiecesEqualToTaskArea()
        {
            defaultSettings.GameDefinition.InitialNumberOfPieces = defaultSettings.GameDefinition.TaskAreaLength * defaultSettings.GameDefinition.BoardWidth;
            var message = Validator.ValidateInitialNumberOfPieces(defaultSettings.GameDefinition.InitialNumberOfPieces, defaultSettings.GameDefinition.TaskAreaLength, defaultSettings.GameDefinition.BoardWidth);
            Assert.IsTrue(string.IsNullOrEmpty(message));
        }

        [TestMethod]
        public void InitialNumberOfPiecesGreaterThanTaskArea()
        {
            defaultSettings.GameDefinition.InitialNumberOfPieces = defaultSettings.GameDefinition.TaskAreaLength * defaultSettings.GameDefinition.BoardWidth + 2;
            var message = Validator.ValidateInitialNumberOfPieces(defaultSettings.GameDefinition.InitialNumberOfPieces, defaultSettings.GameDefinition.TaskAreaLength, defaultSettings.GameDefinition.BoardWidth);
            Assert.IsFalse(string.IsNullOrEmpty(message));
            Assert.AreEqual(ValidatorMessages.INVALID_INITIAL_NUMBER, message);
        }

        // board width 
        [TestMethod]
        public void BoardWidthValid()
        {
            defaultSettings.GameDefinition.BoardWidth = 4;
            var message = Validator.ValidateBoardWidth(defaultSettings.GameDefinition.BoardWidth, defaultSettings.GameDefinition.TaskAreaLength, defaultSettings.GameDefinition.NumberOfPlayersPerTeam);
            Assert.IsTrue(string.IsNullOrEmpty(message));
        }

        [TestMethod]
        public void BoardWidthTooSmall()
        {
            defaultSettings.GameDefinition.BoardWidth = 1;
            var message = Validator.ValidateBoardWidth(defaultSettings.GameDefinition.BoardWidth, defaultSettings.GameDefinition.TaskAreaLength, defaultSettings.GameDefinition.NumberOfPlayersPerTeam);
            Assert.IsFalse(string.IsNullOrEmpty(message));
            Assert.AreEqual(ValidatorMessages.INVALID_BOARD_WIDTH, message);
        }

        [TestMethod]
        public void BoardWidthEqualsZero()
        {
            defaultSettings.GameDefinition.BoardWidth = 0;
            var message = Validator.ValidateBoardWidth(defaultSettings.GameDefinition.BoardWidth, defaultSettings.GameDefinition.TaskAreaLength, defaultSettings.GameDefinition.NumberOfPlayersPerTeam);
            Assert.IsFalse(string.IsNullOrEmpty(message));
            Assert.AreEqual(ValidatorMessages.INVALID_BOARD_WIDTH, message);
        }

        // task area length
        [TestMethod]
        public void TaskAreaLengthValid()
        {
            defaultSettings.GameDefinition.TaskAreaLength = 2;
            var message = Validator.ValidateTaskAreaLength(defaultSettings.GameDefinition.TaskAreaLength);
            Assert.IsTrue(string.IsNullOrEmpty(message));
        }

        [TestMethod]
        public void TaskAreaLengthEqualsZero()
        {
            defaultSettings.GameDefinition.TaskAreaLength = 0;
            var message = Validator.ValidateTaskAreaLength(defaultSettings.GameDefinition.TaskAreaLength);
            Assert.IsFalse(string.IsNullOrEmpty(message));
            Assert.AreEqual(ValidatorMessages.INVALID_TASK_AREA_LENGTH, message);
        }

        // goal area length
        [TestMethod]
        public void GoalAreaLengthValid()
        {
            defaultSettings.GameDefinition.GoalAreaLength = 2;
            var message = Validator.ValidateGoalAreaLength(defaultSettings.GameDefinition.GoalAreaLength);
            Assert.IsTrue(string.IsNullOrEmpty(message));
        }

        [TestMethod]
        public void GoalAreaLengthEqualsZero()
        {
            defaultSettings.GameDefinition.GoalAreaLength = 0;
            var message = Validator.ValidateGoalAreaLength(defaultSettings.GameDefinition.GoalAreaLength);
            Assert.IsFalse(string.IsNullOrEmpty(message));
            Assert.AreEqual(ValidatorMessages.INVALID_GOAL_AREA_LENGTH, message);
        }

        // number of players 
        [TestMethod]
        public void NumberOfPlayersEqualsValid()
        {
            defaultSettings.GameDefinition.NumberOfPlayersPerTeam = 3;
            var message = Validator.ValidatePlayers(defaultSettings.GameDefinition.NumberOfPlayersPerTeam, defaultSettings.GameDefinition.TaskAreaLength, defaultSettings.GameDefinition.BoardWidth);
            Assert.IsTrue(string.IsNullOrEmpty(message));
        }

        [TestMethod]
        public void NumberOfPlayersEqualsZero()
        {
            defaultSettings.GameDefinition.NumberOfPlayersPerTeam = 0;
            var message = Validator.ValidatePlayers(defaultSettings.GameDefinition.NumberOfPlayersPerTeam, defaultSettings.GameDefinition.TaskAreaLength, defaultSettings.GameDefinition.BoardWidth);
            Assert.IsFalse(string.IsNullOrEmpty(message));
            Assert.AreEqual(ValidatorMessages.NUMBER_OF_PLAYERS_EQUALS_ZERO, message);
        }

        [TestMethod]
        public void NumberOfPlayersExceeded()
        {
            defaultSettings.GameDefinition.NumberOfPlayersPerTeam = 123;
            var message = Validator.ValidatePlayers(defaultSettings.GameDefinition.NumberOfPlayersPerTeam, defaultSettings.GameDefinition.TaskAreaLength, defaultSettings.GameDefinition.BoardWidth);
            Assert.IsFalse(string.IsNullOrEmpty(message));
            Assert.AreEqual(ValidatorMessages.NUMBER_OF_PLAYERS_TOO_BIG, message);
        }

        [TestMethod]
        public void GameNameValid()
        {
            defaultSettings.GameDefinition.GameName = "best game ever";
            var message = Validator.ValidateGameName(defaultSettings.GameDefinition.GameName);
            Assert.IsTrue(string.IsNullOrEmpty(message));
        }

        [TestMethod]
        public void GameNameInvalid()
        {
            defaultSettings.GameDefinition.GameName = "   \n";
            var message = Validator.ValidateGameName(defaultSettings.GameDefinition.GameName);
            Assert.IsFalse(string.IsNullOrEmpty(message));
            Assert.AreEqual(ValidatorMessages.INVALID_GAMENAME, message);
        }

        [TestMethod]
        public void GameNameInvalid2()
        {
            defaultSettings.GameDefinition.GameName = "   game   ";
            var message = Validator.ValidateGameName(defaultSettings.GameDefinition.GameName);
            Assert.IsFalse(string.IsNullOrEmpty(message));
            Assert.AreEqual(ValidatorMessages.INVALID_GAMENAME, message);
        }

        [TestMethod]
        public void GoalEqual()
        {
            GoalField goalField1 = new GoalField(1, 3,TeamColour.red) { type = Messages.GoalFieldType.goal };
            GoalField goalField2 = new GoalField(1, 3,TeamColour.red) { type = Messages.GoalFieldType.goal };
            Assert.AreEqual(goalField1, goalField2);

            goalField1.playerId = 10;
            goalField2.playerId = 9;
            Assert.AreNotEqual(goalField1, goalField2);
            goalField2.playerId = 10;
            Assert.AreEqual(goalField1, goalField2);
            //TODO: more tests for equal

        }

        [TestMethod]
        public void GoalFieldEqual()
        {
            GoalField goalField1 = new GoalField(1, 3, TeamColour.red) { type = Messages.GoalFieldType.goal };
            GoalField goalField2 = new GoalField(1, 3, TeamColour.red) { type = Messages.GoalFieldType.goal };
            var dt = DateTime.Now;
            goalField2.timestamp = dt;
            goalField1.timestamp = dt;

            Assert.AreEqual(goalField1, goalField2);
        }

        [TestMethod]
        public void GoalFieldLocationEqual()
        {
            GoalField goalField1 = new GoalField(1, 3, TeamColour.red) { type = Messages.GoalFieldType.goal };
            GoalField goalField2 = new GoalField(1, 3, TeamColour.red) { type = Messages.GoalFieldType.goal };
            goalField2.x = 0;
            goalField2.y = 1;
            goalField1.x = 0;
            goalField1.y = 1;

            Assert.AreEqual(goalField1, goalField2);
        }

        [TestMethod]
        public void GoalFieldLocationUnequal()
        {
            GoalField goalField1 = new GoalField(1, 3, TeamColour.red) { type = Messages.GoalFieldType.goal };
            GoalField goalField2 = new GoalField(1, 3, TeamColour.red) { type = Messages.GoalFieldType.goal };
            goalField2.x = 0;
            goalField2.y = 1;
            goalField1.x = 0;
            goalField1.y = 2;

            Assert.AreNotEqual(goalField1, goalField2);
        }


        [TestMethod]
        public void GoalFieldsArrayIsNull()
        {
            defaultSettings.GameDefinition.Goals = null;
            var message = Validator.ValidateGoals(defaultSettings.GameDefinition.Goals, 
                defaultSettings.GameDefinition.GoalAreaLength, 
                defaultSettings.GameDefinition.TaskAreaLength, 
                defaultSettings.GameDefinition.BoardWidth);

            Assert.IsFalse(string.IsNullOrEmpty(message));
            Assert.AreEqual(ValidatorMessages.NULL_GOALFIELD_ARRAY, message);
        }

        [TestMethod]
        public void GoalFieldsArrayContainsNullGoal()
        {
            defaultSettings.GameDefinition.Goals = new GoalField[]{ new GoalField(1, 2, TeamColour.red), null};
            var message = Validator.ValidateGoals(defaultSettings.GameDefinition.Goals,
                defaultSettings.GameDefinition.GoalAreaLength,
                defaultSettings.GameDefinition.TaskAreaLength,
                defaultSettings.GameDefinition.BoardWidth);

            Assert.IsFalse(string.IsNullOrEmpty(message));
            Assert.AreEqual(ValidatorMessages.NULL_GOALFIELD, message);
        }

        [TestMethod]
        public void GoalFieldsNotDistinct()
        {
            defaultSettings.GameDefinition.Goals = new GoalField[] { new GoalField(10,2,TeamColour.red), new GoalField(1, 2,TeamColour.blue), new GoalField(1,2,TeamColour.blue)};
            var message = Validator.ValidateGoals(defaultSettings.GameDefinition.Goals,
                defaultSettings.GameDefinition.GoalAreaLength,
                defaultSettings.GameDefinition.TaskAreaLength,
                defaultSettings.GameDefinition.BoardWidth);

            Assert.IsFalse(string.IsNullOrEmpty(message));
            Assert.AreEqual(ValidatorMessages.GOALS_ARE_NOT_UNIQUE, message);
        }
        [TestMethod]
        public void GoalFieldsRedTeamNoGoals()
        {
            defaultSettings.GameDefinition.Goals = new GoalField[] { new GoalField(1, 3, TeamColour.blue), new GoalField(1, 2, TeamColour.blue), new GoalField(0, 3, TeamColour.blue) };
            var message = Validator.ValidateGoals(defaultSettings.GameDefinition.Goals,
                defaultSettings.GameDefinition.GoalAreaLength,
                defaultSettings.GameDefinition.TaskAreaLength,
                defaultSettings.GameDefinition.BoardWidth);

            Assert.IsFalse(string.IsNullOrEmpty(message));
            Assert.AreEqual(ValidatorMessages.RED_TEAM_HAS_NO_GOAL, message);
        }

        [TestMethod]
        public void GoalFieldsBlueTeamNoGoals()
        {
            defaultSettings.GameDefinition.Goals = new GoalField[] { new GoalField(11, 2, TeamColour.red), new GoalField(10, 2, TeamColour.red), new GoalField(10, 3, TeamColour.red) };
            var message = Validator.ValidateGoals(defaultSettings.GameDefinition.Goals,
                defaultSettings.GameDefinition.GoalAreaLength,
                defaultSettings.GameDefinition.TaskAreaLength,
                defaultSettings.GameDefinition.BoardWidth);

            Assert.IsFalse(string.IsNullOrEmpty(message));
            Assert.AreEqual(ValidatorMessages.BLUE_TEAM_HAS_NO_GOAL, message);
        }

        [TestMethod]
        public void GoalInTaskArea()
        {
            defaultSettings.GameDefinition.Goals = new GoalField[] { new GoalField(6, 2, TeamColour.red), new GoalField(10, 2, TeamColour.blue) };
            var message = Validator.ValidateGoals(defaultSettings.GameDefinition.Goals,
                defaultSettings.GameDefinition.GoalAreaLength,
                defaultSettings.GameDefinition.TaskAreaLength,
                defaultSettings.GameDefinition.BoardWidth);

            Assert.IsFalse(string.IsNullOrEmpty(message));
            Assert.AreEqual(ValidatorMessages.GOALS_OUTSIDE_GOAL_AREA, message);
        }

        [TestMethod]
        public void RedGoalInBlueGoalArea()
        {
            defaultSettings.GameDefinition.Goals = new GoalField[] 
            {
              new GoalField(1, 2, TeamColour.red),
              new GoalField(1,3,TeamColour.blue),
              new GoalField(2, 10, TeamColour.red)
            };
            var message = Validator.ValidateGoals(defaultSettings.GameDefinition.Goals,
                defaultSettings.GameDefinition.GoalAreaLength,
                defaultSettings.GameDefinition.TaskAreaLength,
                defaultSettings.GameDefinition.BoardWidth);

            Assert.IsFalse(string.IsNullOrEmpty(message));
            Assert.AreEqual(ValidatorMessages.RED_GOAL_IN_BLUE_GOAL_AREA, message);
        }

        [TestMethod]
        public void BlueGoalInRedGoalArea()
        {
            defaultSettings.GameDefinition.Goals = new GoalField[] 
            {
                new GoalField(1, 2, TeamColour.blue),
                new GoalField(1, 10, TeamColour.blue),
                new GoalField(2, 10, TeamColour.red)
            };
            var message = Validator.ValidateGoals(defaultSettings.GameDefinition.Goals,
                defaultSettings.GameDefinition.GoalAreaLength,
                defaultSettings.GameDefinition.TaskAreaLength,
                defaultSettings.GameDefinition.BoardWidth);

            Assert.IsFalse(string.IsNullOrEmpty(message));
            Assert.AreEqual(ValidatorMessages.BLUE_GOAL_IN_RED_GOAL_AREA, message);
        }

        [TestMethod]
        public void ActionCostsNull()
        {
            defaultSettings.ActionCosts = null;
            var message = Validator.ValidateActionCosts(defaultSettings.ActionCosts);
            Assert.IsFalse(string.IsNullOrEmpty(message));
            Assert.AreEqual(ValidatorMessages.ACTION_COSTS_NULL, message);
        }

    }
}

