using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using zgrl.Classes;

namespace zgrl.Commands
{
    public class RacerCreation : ModuleBase<SocketCommandContext>
    {

        [Command("createpilot")]
        public async Task NewracerAsync(params string[] inputs)
        {
            var r = racer.get_racer(Context.Message.Author.Id, Context.Guild.Id);
            if(r != null) {
                await ReplyAsync("You already have a pilot. Please use `zg!deletepilot` to remove your old one first");
                return;
            }
            var string_to_value = helpers.parseInputs(inputs);
            r = new racer();

            if (!r.update(string_to_value, out string error)) {
                await ReplyAsync(Context.User.Mention + ". Pilot creation failed with error message: " + error);
                return;
            }

            r.player_discord_id = Context.Message.Author.Id;
            r.server_discord_id = Context.Guild.Id;

            racer.insert_racer(r);

            await ReplyAsync(Context.User.Mention + ", you've created your pilot. Use `zg!pilot` to see your complete pilot.");
        }

        [Command("updatepilot")]
        public async Task updateRacerAsync(params string[] inputs)
        {
            var r = racer.get_racer(Context.Message.Author.Id, Context.Guild.Id);
            if(r == null) {
                await ReplyAsync("You don't have a pilot! Please use `zg!createpilot` instead");
                return;
            }
            var string_to_value = helpers.parseInputs(inputs);

            if (!r.update(string_to_value, out string error)) {
                await ReplyAsync(Context.User.Mention + ". Racer creation failed with error message: " + error);
                return;
            }

            racer.update_racer(r);

            await ReplyAsync(Context.User.Mention + ", you've updated your pilot. Use `zg!pilot` to see your changes.");
        }

        [Command("deletepilot")]
        public async Task DeleteRacerAsync()
        {
            var r = racer.get_racer(Context.Message.Author.Id, Context.Guild.Id);

            if(r == null) {
                await ReplyAsync("No racer found for you");
            } else {

                Classes.racer.delete_racer(r);
                await ReplyAsync("Racer Deleted.");
            }
        }

        [Command("pilot")]
        public async Task showRacerAsync(int i = -1) {
            racer r;
            if (i < 0) r = racer.get_racer(Context.Message.Author.Id, Context.Guild.Id);
            else r = racer.get_racer(i);

            if ( r == null ) {
                await ReplyAsync(Context.User.Mention + ", you don't have a current pilot or this pilot doesn't exist in the database.");
                return;
            }            

            await Context.Channel.SendMessageAsync("", false, r.embed(i, Context), null);
        }

        [Command("updateability")]
        public async Task UpdateAbilityAsync(int ID, int v = -1) {
            var r = racer.get_racer(Context.Message.Author.Id, Context.Guild.Id);

            if(r == null) {
                await ReplyAsync("No pilot found for you. Use `zg!createpilot` to make one");
                return;
            }

            if (r.inGame) {
                await ReplyAsync("You can't modify your pilot while racing!");
                return;
            }


            racer.replace_racer(r);

            await ReplyAsync(Context.User.Mention + ", Ability changed to ");
        }

        [Command("showabilities")]
        public async Task DisplayAbilitiesAsync(int v = -1) {
            Server sr = Server.get_Server(Context.Guild.Id);
            if (sr == null) v = 0;
            var abilities = Ability.get_ability("abilities.json");
            var str = new List<string>();
            str.Add("**Special Abilities**");
            foreach(Ability ability in abilities) {
                str.Add(ability.ToString());
            }
            helpers.output(Context.User, str);
        }

        [Command("listpilots")]
        public async Task ListRacersAsync()
        {
            var s = new List<string>();
            s.Add("Pilots!");
            var rcrs = racer.get_racer();
            foreach(Classes.racer r in rcrs) {
                if (r.server_discord_id == Context.Guild.Id) s.Add("ID: #" + r.ID + " | " + r.name);
            }
            helpers.output(Context.User, s);
        }

    }
}