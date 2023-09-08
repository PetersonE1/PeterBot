﻿using Discord;
using PeterBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterBot.Modules.Commands
{
    public class Echo : ISlashCommand
    {
        public SlashCommandBuilder Build()
        {
            var globalCommand = new SlashCommandBuilder()
                .WithName("echo")
                .WithDescription("Echoes back the given message")
                .AddOption("message", ApplicationCommandOptionType.String, "Message to be echoed", isRequired: true);
            return globalCommand;
        }
    }
}