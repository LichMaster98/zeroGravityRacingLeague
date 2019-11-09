using System;
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
                await ReplyAsync("You already have a racer. Please use `zg!deletepilot` to remove your old one first");
                return;
            }
            var string_to_value = helpers.parseInputs(inputs);
            r = new racer();

            if (!r.update(string_to_value, out string error)) {
                await ReplyAsync(Context.User.Mention + ". Racer creation failed with error message: " + error);
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

        [Command("racer")]
        public async Task showRacerAsync(int i = -1) {
            racer r = new racer();
            if (i < 0) r = racer.get_racer(Context.Message.Author.Id, Context.Guild.Id);
            else r = racer.get_racer(i);

            if ( r == null ) {
                await ReplyAsync(Context.User.Mention + ", you don't have a current racer or this racer doesn't exist in the database.");
                return;
            }


            var embed = new EmbedBuilder();

            embed.Title = "Racer Name: " + r.name;
            embed.WithDescription(r.descr);
            embed.WithThumbnailUrl(r.img);
            embed.AddField("ID",r.ID.ToString(),true);
            embed.AddField("Sponsor",r.faction, true);
            embed.AddField("Attributes","Adaptability: " + r.adaptability + System.Environment.NewLine + "Intelligence: " + r.intel + System.Environment.NewLine + "Skill: " + r.skill, true);
            foreach (Classes.Ability ability in r.abilities) {
                embed.AddField("Ability:" + ability.Title, ability.Description, true);
            }
            foreach (Classes.Car car in r.cars) {
                embed.AddField(car.racerEmbed(), car.description, true);
            }
            if( i < 0 ) {
                embed.AddField("Player",Context.User.Mention,true);
            } else {
                var usr = Context.Guild.GetUser(r.player_discord_id);
                embed.AddField("Player",usr.Mention,true);
            }

            await Context.Channel.SendMessageAsync("", false, embed.Build(), null);
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
                str.Add("**#" + ability.ID + ":** " + ability.Title + " - *" +ability.Description + "*");
            }
            helpers.output(Context.User, str);
        }

        [Command("showcards")]
        public async Task showCardsAsync() {
            var cards = Card.get_card("card.json");
            var str = new List<string>();
            str.Add("**Card List**");
            foreach (Classes.Card card in cards) {
                str.Add("**" + card.title + "** - " + card.cardTypeString() + " - " + card.description);
            }
            helpers.output(Context.User,str);
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

        [Command("listpilots")]
        public async Task ListRacersAsync()
        {
            var s = new List<string>();
            s.Add("Racers!");
            var rcrs = racer.get_racer();
            foreach(Classes.racer r in rcrs) {
                if (r.server_discord_id == Context.Guild.Id) s.Add("ID: #" + r.ID + " | " + r.name);
            }
            helpers.output(Context.User, s);
        }

        [Command("resetpilot")]
        public async Task resetOneRacer(int i) {
            Server s = Server.get_Server(Context.Guild.Id);
            if (s == null) { 
                await Context.Channel.SendMessageAsync(Context.User.Mention + ", you aren't authorized on this server.");
                return;
            }
            if (!s.isAdmin(Context.Guild.GetUser(Context.User.Id))) {
                await Context.Channel.SendMessageAsync(Context.User.Mention + ", you aren't listed as an authorized user for this server.");
                return;
            }
            var r = racer.get_racer(i);
            if (r == null) {
                await ReplyAsync("No pilot with that ID");
                return;
            }
            r.reset();
            racer.replace_racer(r);
            await ReplyAsync(r.nameID() + " has been reset");
        }

        [Command("resetpilots")]
        public async Task resetAllRacers() {
            Server s = Server.get_Server(Context.Guild.Id);
            if (s == null) { 
                await Context.Channel.SendMessageAsync(Context.User.Mention + ", you aren't authorized on this server.");
                return;
            }
            if (!s.isAdmin(Context.Guild.GetUser(Context.User.Id))) {
                await Context.Channel.SendMessageAsync(Context.User.Mention + ", you aren't listed as an authorized user for this server.");
                return;
            }
            var rcs = racer.get_racer();
            rcs.ForEach(e=>{
                if ( e.server_discord_id == Context.Guild.Id ) {
                    e.inGame = false;
                    racer.update_racer(e);
                }
            });
            await ReplyAsync("All Pilots on this Server Reset");
        }

    }
}