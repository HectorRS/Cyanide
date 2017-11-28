using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Cyanide.Modules.User_Interaction
{
    public class CyanCheck : ModuleBase<SocketCommandContext>
    {
        [Command("soar")]
        [Summary("Soars like a beast.")]
        public async Task SoarAsync()
        {
            await ReplyAsync("...Meow.");
        }

    }
}