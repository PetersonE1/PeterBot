﻿using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterBot.Models
{
    public interface ISlashCommand
    {
        SlashCommandBuilder Build();
    }
}
