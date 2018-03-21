using Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MainApp.Tests
{
    [TestClass]
    public class ValidatorTests
    {
        GameMasterSettings defaultSettings = new GameMasterSettings()
        {
            ActionCosts = new GameMasterSettingsActionCosts(),
            GameDefinition = new GameMasterSettingsGameDefinition()
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
            Messages.GoalField goalField1 = new Messages.GoalField(1, 3) { type = Messages.GoalFieldType.goal };
            Messages.GoalField goalField2 = new Messages.GoalField(1, 3) { type = Messages.GoalFieldType.goal };
            Assert.AreEqual(goalField1, goalField2);

            goalField1.playerId = 10;
            goalField2.playerId = 9;
            Assert.AreNotEqual(goalField1, goalField2);
            goalField2.playerId = 10;
            Assert.AreEqual(goalField1, goalField2);
            //TODO: more tests for equal
        }
        //TODO: tests for goalfields validation
    }
}
