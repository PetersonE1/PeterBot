using Discord;
using Discord.WebSocket;
using PeterBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PeterBot.Modules.Commands
{
    public class Roll : ISlashCommand
    {
        public SlashCommandBuilder Build()
        {
            var globalCommand = new SlashCommandBuilder()
                .WithName("roll")
                .WithDescription("Rolls dice")
                .AddOption("command", ApplicationCommandOptionType.String, "Dice roll command", isRequired: true)
                .AddOption("modifiers", ApplicationCommandOptionType.String, "Modifiers", isRequired: false);
            return globalCommand;
        }

        public async Task Execute(SocketSlashCommand command)
        {
            string? args = null;
            if (command.Data.Options.Count > 1)
                args = (string)command.Data.Options.ElementAt(1).Value;
            DiceRoll diceRoll = ProcessInput((string)command.Data.Options.ElementAt(0).Value, args);
            if (diceRoll.Count <= 0 || diceRoll.Sides <= 0)
            {
                await command.RespondAsync("Unable to process command");
                return;
            }
            if (diceRoll.Count > 100000 || diceRoll.Sides > 100000)
            {
                await command.RespondAsync("Values too large to process");
                return;
            }
            
            int[] rolls = new int[diceRoll.Count];
            for (int i = 0;  i < diceRoll.Count; i++)
            {
                rolls[i] = RandomNumberGenerator.GetInt32(diceRoll.Sides) + 1;
                if (diceRoll.IsAdv)
                {
                    int num2 = RandomNumberGenerator.GetInt32(diceRoll.Sides) + 1;
                    if (num2 > rolls[i])
                        rolls[i] = num2;
                }
            }

            if (diceRoll.Type == DiceType.Normal)
            {
                long result = 0;
                foreach (int roll in rolls)
                    result += roll;

                string modifierString = diceRoll.Modifier == 0 ? string.Empty :
                    (diceRoll.Modifier < 0 ? $" - {diceRoll.Modifier * -1}" : $" + {diceRoll.Modifier}");

                string rollsString = string.Empty;
                for (int i = 0; i < rolls.Length; i++)
                {
                    rollsString += $"`{rolls[i]}` ";
                    if (i >= 50)
                    {
                        rollsString += "...";
                        break;
                    }
                }

                string display = $"> ### {command.User.GlobalName}" + 
                    $"\r\n> Dice rolled: {diceRoll.Count} `1-{diceRoll.Sides}`{modifierString}" +
                    $"\r\n> **Sum**\r\n> {result}{modifierString} = {result + diceRoll.Modifier}" +
                    $"\r\n> **Rolls**\r\n> {rollsString}";
                if (diceRoll.IsAdv)
                    display += "\r\n> **Modifiers**\r\n> Advantage";

                await command.RespondAsync(display);
                return;
            }

            if (diceRoll.Type == DiceType.Fate)
            {
                string rollsString = string.Empty;

                foreach (int i in rolls)
                {
                    switch (i)
                    {
                        case 1: rollsString += "`[-]` "; break;
                        case 2: rollsString += "`[ ]` "; break;
                        case 3: rollsString += "`[+]` "; break;
                        default: break;
                    }
                }

                string display = $"> ### {command.User.GlobalName}" +
                    $"\r\n> Dice rolled: **{diceRoll.Count}**" +
                    $"\r\n> **Result**\r\n> {rollsString}";

                await command.RespondAsync(display);
                return;
            }

            await command.RespondAsync("Unable to process command");
            return;
        }

        private DiceRoll ProcessInput(string input, string? arguments)
        {
            bool subtract = false;
            string[] parts = new string[2];
            if (input.Contains('+'))
            {
                parts = input.Split('+');
            }
            else if (input.Contains('-'))
            {
                parts = input.Split('-');
                subtract = true;
            }
            else
                parts[0] = input;
                
            string[] split = parts[0].Split('d');
            if (split.Length != 2) return new DiceRoll();
            if (split[0].Length == 0 || split[1].Length == 0) return new DiceRoll();

            if (!int.TryParse(split[0], out int count)) return new DiceRoll();
            bool sidesSuccess = int.TryParse(split[1], out int sides);
            if (split[1] != "f" && !sidesSuccess) return new DiceRoll();

            DiceType type = DiceType.Normal;
            if (split[1] == "f")
            {
                sides = 3;
                type = DiceType.Fate;
            }

            int modifier = 0;
            if (parts[1] != null && parts[1].Length > 0)
            {
                if (!int.TryParse(parts[1], out modifier)) return new DiceRoll();

                if (subtract)
                    modifier *= -1;
            }

            bool isAdv = false;
            if (arguments == "adv")
                isAdv = true;

            DiceRoll roll = new DiceRoll()
            {
                Count = count,
                Sides = sides,
                Type = type,
                Modifier = modifier,
                IsAdv = isAdv
            };
            return roll;
        }

        private class DiceRoll
        {
            public int Count { get; set; }
            public int Sides { get; set; }
            public DiceType Type { get; set; }
            public int Modifier { get; set; }
            public bool IsAdv {  get; set; }
        }

        private enum DiceType
        {
            Normal,
            Fate
        }
    }
}
