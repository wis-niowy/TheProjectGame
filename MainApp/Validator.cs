﻿using Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Messages;

namespace MainApp
{
    public static class Validator
    {

        public static string ValidateSettings(GameMasterSettings settings)
        {
            var gameDefinitions = settings.GameDefinition;
            var errors = new StringBuilder();
            var message = ValidateShamProbability(gameDefinitions.ShamProbability);

            if (!string.IsNullOrEmpty(message))
                errors.AppendLine(message);

            message = ValidatePiecesFrequency(gameDefinitions.PlacingNewPiecesFrequency);
            if (!string.IsNullOrEmpty(message))
                errors.AppendLine(message);

            message = ValidateInitialNumberOfPieces(gameDefinitions.InitialNumberOfPieces, gameDefinitions.TaskAreaLength, gameDefinitions.BoardWidth);
            if (!string.IsNullOrEmpty(message))
                errors.AppendLine(message);

            message = ValidateBoardWidth(gameDefinitions.BoardWidth,gameDefinitions.TaskAreaLength, gameDefinitions.NumberOfPlayersPerTeam);
            if (!string.IsNullOrEmpty(message))
                errors.AppendLine(message);

            message = ValidateTaskAreaLength(gameDefinitions.TaskAreaLength);
            if (!string.IsNullOrEmpty(message))
                errors.AppendLine(message);

            message = ValidateGoalAreaLength(gameDefinitions.GoalAreaLength);
            if (!string.IsNullOrEmpty(message))
                errors.AppendLine(message);

            message = ValidatePlayers(gameDefinitions.NumberOfPlayersPerTeam, gameDefinitions.TaskAreaLength, gameDefinitions.BoardWidth);
            if (!string.IsNullOrEmpty(message))
                errors.AppendLine(message);

            message = ValidateGameName(gameDefinitions.GameName);
            if (!string.IsNullOrEmpty(message))
                errors.AppendLine(message);

            message = ValidateGoals(gameDefinitions.Goals,gameDefinitions.GoalAreaLength,gameDefinitions.TaskAreaLength,gameDefinitions.BoardWidth);
            if (!string.IsNullOrEmpty(message))
                errors.AppendLine(message);

            message = ValidateGameName(gameDefinitions.GameName);
            if (!string.IsNullOrEmpty(message))
                errors.AppendLine(message);

            return errors.ToString();
        }

        public static string ValidateShamProbability(double shamProbability)
        {
            if (shamProbability >= 0 && shamProbability < 1)
                return "";
            return ValidatorMessages.INVALID_SHAM_PROBABILITY;
        }
        public static string ValidatePiecesFrequency(uint piecesFrequency)
        {
            if (piecesFrequency > 0)
                return "";
            return ValidatorMessages.INVALID_PIECES_FREQUENCY;
        }

        public static string ValidateInitialNumberOfPieces(uint initialNumberOfPieces, uint length, uint width)
        {
            if (initialNumberOfPieces <= (length * width))
                return "";
            return ValidatorMessages.INVALID_INITIAL_NUMBER;
        }

        public static string ValidateBoardWidth(uint width, uint TaskArealength, uint playersPerTeam)
        {
            if (width > 1 && width * TaskArealength >= playersPerTeam * 2)
                return "";
            return ValidatorMessages.INVALID_BOARD_WIDTH;
        }

        public static string ValidateTaskAreaLength(uint TaskArealength)
        {
            if (TaskArealength >= 1)
                return "";
            return ValidatorMessages.INVALID_TASK_AREA_LENGTH;
        }

        public static string ValidateGoalAreaLength(uint GoalArealength)
        {
            if (GoalArealength >= 1)
                return "";
            return ValidatorMessages.INVALID_GOAL_AREA_LENGTH;
        }

        // players per team
        public static string ValidatePlayers(uint PlayersNumber, uint taskAreaLength, uint boardWidth)
        {
            if (PlayersNumber == 0)
                return ValidatorMessages.NUMBER_OF_PLAYERS_EQUALS_ZERO;
            if (PlayersNumber * 2 > taskAreaLength * boardWidth)
                return ValidatorMessages.NUMBER_OF_PLAYERS_TOO_BIG;
            return "";
        }

        // game name
        public static string ValidateGameName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length != (name.Trim().Length))
                return ValidatorMessages.INVALID_GAMENAME;
            return "";
        }

        // goals
        public static string ValidateGoals(GoalField[] goals, uint goalAreaLength, uint taskAreaLength, uint boardWidth)
        {
            if (goals == null)
                return ValidatorMessages.NULL_GOALFIELD_ARRAY;
            foreach (var g in goals)
            {
                if (g == null)
                    return ValidatorMessages.NULL_GOALFIELD;
            }
            if (goals.Select(q=> new { x = q.x, y = q.y }).Distinct().Count() != goals.Count()) // override equals()
                return ValidatorMessages.GOALS_ARE_NOT_UNIQUE;
            if (!goals.Where(q => q.team == TeamColour.blue).Any())
                return ValidatorMessages.BLUE_TEAM_HAS_NO_GOAL;
            if (!goals.Where(q => q.team == TeamColour.red).Any())
                return ValidatorMessages.RED_TEAM_HAS_NO_GOAL;

            foreach (var g in goals)
            {
                if ((g.y >= goalAreaLength && g.y < taskAreaLength + goalAreaLength) || (g.x >= boardWidth) || g.y >= taskAreaLength + 2 * goalAreaLength)
                    return ValidatorMessages.GOALS_OUTSIDE_GOAL_AREA;
                if (g.team == TeamColour.blue && g.y >= goalAreaLength)
                    return ValidatorMessages.BLUE_GOAL_IN_RED_GOAL_AREA;
                if (g.team == TeamColour.red && g.y < goalAreaLength)
                    return ValidatorMessages.RED_GOAL_IN_BLUE_GOAL_AREA;
            }

            return "";
        }

        public static string ValidateActionCosts(GameMasterSettingsActionCosts settings)
        {
            if (settings == null)
                return ValidatorMessages.ACTION_COSTS_NULL;
            return "";
        }
    }
}
