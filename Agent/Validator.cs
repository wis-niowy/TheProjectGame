using Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Messages;
using GameArea.AppConfiguration;

namespace Player
{
    public static class Validator
    {

        public static string ValidateSettings(PlayerSettingsConfiguration settings)
        {
            var gameDefinitions = settings;
            var errors = new StringBuilder();

            var message = ValidatorMessages.INVALID_KEEP_ALIVE_INTERVAL;
            if (settings.KeepAliveInterval < 0)
                errors.AppendLine(message);

            message = ValidatorMessages.INVALID_RETRY_JOIN_GAME_INTERVAL;
            if (settings.RetryJoinGameInterval < 0)
                errors.Append(message);

            return errors.ToString();
        }
    }
}
