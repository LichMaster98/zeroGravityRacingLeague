using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using zgrl.Classes;

namespace zgrl.Commands
{
  public class DeckCommands : ModuleBase<SocketCommandContext>
  {
    [Command("showdeck")]
    public async Task showDeckAsync(int id = -1) {
      if (id < 0) {
        var decks = Deck.get_deck();
        var strs = new List<string>();
        strs.Add("Decks");
        foreach (var deck in decks) {
          strs.Add("ID " + deck.ID + ": **" + deck.Title + "**: " + deck.deckShort());
        }

        helpers.output(Context.User, strs);
      } else {
        Deck deck = Deck.get_deck(id);
        if (deck == null) {
          await ReplyAsync(Context.User.Mention + ", this deck doesn't exist in the database");
          return;
        }
        await ReplyAsync(null, false, deck.embed());
      }

    }

    [Command("deck")]
    public async Task showPersonalDeckAsync(int car = -1, string deckLegality = "invalid") {
      var r = Racer.get_racer(Context.Message.Author.Id, Context.Guild.Id);
      if(r == null) {
          await ReplyAsync("You don't have a pilot. Please use `zg!createpilot` to create one first");
          return;
      }
      Deck deck;

      if (car < 0 && Card.stringToCardLegality(deckLegality) == CardLegality.INVALID) {
        var rtrnr = new List<string>();
        rtrnr.Add("**Decks**");
        foreach (var DeckLegalityToDeckID in r.carToDeckLegalityToDeckID.Values) {
          foreach (var DeckID in DeckLegalityToDeckID.Values) {
            deck = Deck.get_deck(DeckID);
            rtrnr.Add("ID " + deck.ID + ": **" + deck.Title + "**: " + deck.deckShort());
          }
        }
        helpers.output(Context.User, rtrnr);
      } else {
        deck = Deck.get_deck(r.carToDeckLegalityToDeckID[car][Card.stringToCardLegality(deckLegality)]);
        await Context.User.SendMessageAsync(null, false, deck.embed());
      }
    }

    [Command("createdeck")]
    public async Task createNewDeckAsync(params string[] inputs) {
      var r = Racer.get_racer(Context.Message.Author.Id, Context.Guild.Id);
      if(r == null) {
        await ReplyAsync("You don't have a pilot! Please use `zg!createpilot` instead");
        return;
      }
      var input_dict = helpers.parseInputs(inputs);
      int carID;
      CardLegality legality;

      if (!input_dict.ContainsKey("carid")) {
        await ReplyAsync(Context.User.Mention + ", you didn't include the mandatory key `carid` in your input.");
        return;
      }
      if (!int.TryParse(input_dict["carid"], out carID)) {
        await ReplyAsync(Context.User.Mention + ", the `carid` wasn't a valid integer");
        return;
      }
      var car = Car.get_Car(carID);
      if (car == null) {
        await ReplyAsync(Context.User.Mention + ", the provided `carid: " + carID + "` does not exist.");
        return;
      }
      if (!input_dict.ContainsKey("legality")) {
        await ReplyAsync(Context.User.Mention + ", you didn't include mandatory key `legality` in your input.");
        return;
      }
      legality = Card.stringToCardLegality(input_dict["legality"]);
      if (legality == CardLegality.INVALID) {
        await ReplyAsync(Context.User.Mention + ", you didn't provide a valid legality input. `red`, `blue`, or `yellow`");
        return;
      }
      if (r.carToDeckLegalityToDeckID.ContainsKey(carID)) {
        if (r.carToDeckLegalityToDeckID[carID].ContainsKey(legality)) {
          await ReplyAsync(Context.User.Mention + ", you can't edit a deck using this command. Try `zg!updatedeck`");
          return;
        }
      }
      Deck d;
      if (!Deck.fromDictionary(input_dict, car, r, out string error, out d)) {
        if (d != null) {
          await ReplyAsync(Context.User.Mention + ", failed to make deck with error: " + error, false, d.embed());
          return;
        } else {
          await ReplyAsync(Context.User.Mention + ", failed to make deck with error: " + error);
          return;
        }
      }
      d.player_discord_id = Context.User.Id;
      d.server_discord_id = Context.Guild.Id;
      Deck.insert_deck(d);
      var deck = Deck.get_deck(d.Title);
      if (r.carToDeckLegalityToDeckID.ContainsKey(carID)) {
        r.carToDeckLegalityToDeckID[carID].Add(legality, deck.ID);
      } else {
        var dict = new Dictionary<CardLegality, long>();
        dict.Add(legality, deck.ID);
        r.carToDeckLegalityToDeckID.Add(carID, dict);
      }
      Racer.update_racer(r);
      await ReplyAsync(Context.User.Mention + ", made deck as shown below. To update use `zg!updatedeck id=" + deck.ID + " [other updates]`", false, deck.embed());
      
    }
  }
}