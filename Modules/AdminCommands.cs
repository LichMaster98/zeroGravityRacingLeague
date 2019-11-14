using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using zgrl.Classes;

namespace zgrl.Commands
{
  public class AdminCommands : ModuleBase<SocketCommandContext>
  {

    [Command("resetpilot")]
    public async Task resetOneRacer(int i) {
      var s = Server.get_Server(Context.Guild.Id);
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
      var s = Server.get_Server(Context.Guild.Id);
      if (s == null) { 
        createServerObject(Context); 
        s = Server.get_Server(Context.Guild.Id);
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

    [Command("newcard")]
    public async Task addNewCardAsync(params string[] inputs) {
      var s = Server.get_Server(Context.Guild.Id);
      if (s == null) {
        createServerObject(Context); 
        s = Server.get_Server(Context.Guild.Id);
      }
      if (!s.isAdmin(Context.Guild.GetUser(Context.User.Id))) {
        await Context.Channel.SendMessageAsync(Context.User.Mention + ", you aren't listed as an authorized user for this server.");
        return;
      }
      
      var string_to_value = helpers.parseInputs(inputs);
      var c = new Card();
      if (!c.updateNewCard(string_to_value, out string error)) {
        await ReplyAsync(Context.User.Mention + "Error while building new card: " + error);
      } else {
        Card.insert_card("card.json", c);
        var card = Card.get_card("card.json", c.title);

        await ReplyAsync(Context.User.Mention + ", made a card. " + error, false, card.embed());
      }
    }

    [Command("updatecard")]
    public async Task updateCardAsync(params string[] inputs) {
      var s = Server.get_Server(Context.Guild.Id);
      if (s == null) {
        createServerObject(Context); 
        s = Server.get_Server(Context.Guild.Id);
      }
      if (!s.isAdmin(Context.Guild.GetUser(Context.User.Id))) {
        await Context.Channel.SendMessageAsync(Context.User.Mention + ", you aren't listed as an authorized user for this server.");
        return;
      }

      var string_to_value = helpers.parseInputs(inputs);
      if (!string_to_value.ContainsKey("id")) {
        await ReplyAsync(Context.User.Mention + ", you didn't specify the card ID to update.");
        return;
      }
      int id;
      if(!int.TryParse(string_to_value["id"], out id) || id < 0) {
        await ReplyAsync(Context.User.Mention + ", you didn't provide a valid number for ID.");
        return;
      }

      var c = Card.get_card("card.json", id);
      if (c == null) {
        await ReplyAsync(Context.User.Mention + ", this card id (" + id + ") doesn't exist in the database.");
        return;
      }

      string_to_value.Remove("id");
      if (!c.updateNewCard(string_to_value, out string error)) {
        await ReplyAsync(Context.User.Mention + ", this card was unable to be updated with error: " + error);
        return;
      } else {
        Card.update_card("card.json", c);
        await ReplyAsync(Context.User.Mention + ", updated a card. **WARNING** - Mechanical implementation in code of this card has not changed." + error, false, c.embed());
      }
    }

    [Command("removeAuthorized")]
    public async Task removeAuthorizedAsync(IGuildUser User) {
      var s = Server.get_Server(Context.Guild.Id);
      if (s == null) { 
        createServerObject(Context); 
        s = Server.get_Server(Context.Guild.Id);
      }
      if (!s.isAdmin(Context.Guild.GetUser(Context.User.Id))) {
        await Context.Channel.SendMessageAsync(Context.User.Mention + ", you aren't listed as an authorized user for this server.");
        return;
      }
      s.adminSnowflakes.Remove(User.Id);
      Server.replace_Server(s);
      await Context.Channel.SendMessageAsync(Context.User.Mention + ", removed " + User.Mention + ", from the authorized users on this server.");
    }

    [Command("addAuthorized")]
    public async Task addAuthorizedAsync(IGuildUser User) {
      var s = Server.get_Server(Context.Guild.Id);
      if (s == null) { 
        createServerObject(Context); 
        s = Server.get_Server(Context.Guild.Id);
      }
      if (!s.isAdmin(Context.Guild.GetUser(Context.User.Id))) {
        await Context.Channel.SendMessageAsync(Context.User.Mention + ", you aren't listed as an authorized user for this server.");
        return;
      }
      s.adminSnowflakes.Add(User.Id);
      Server.replace_Server(s);
      await Context.Channel.SendMessageAsync(Context.User.Mention + ", added " + User.Mention + ", to the authorized users on this server.");
    }

    private static void createServerObject(SocketCommandContext Context) {
      var s = new Server();
      s.snowflake = Context.Guild.Id;
      s.Title = Context.Guild.Name;
      s.adminSnowflakes.Add(Context.Guild.OwnerId);
      Server.insert_Server(s);
    }

  }
}