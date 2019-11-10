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