using System.Collections.Generic;
using Discord;
using System.Linq;
using JsonFlatFileDataStore;
using Newtonsoft.Json;

namespace zgrl.Classes
{
    public partial class Deck
    {
      [JsonProperty("id")]
      public long ID { get; set; }
      [JsonProperty("title")]
      public string Title { get; set; }
      [JsonProperty("description")]
      public string description { get; set; }
      [JsonProperty("player_id")]
      public ulong player_discord_id { get; set; }
      [JsonProperty("server_id")]
      public ulong server_discord_id { get ; set; }
      [JsonProperty("listCards")]
      public List<Card> cards { get; set; } = new List<Card>();
      [JsonProperty("cardLegality")]
      public CardLegality cardLegality { get; set; } = CardLegality.INVALID;
      [JsonProperty("carMapped")]
      public int carMapped { get; set; } = -1;

      private List<Card> freshDeck() {
        var rtrnr = new List<Card>();
        foreach (var card in cards) {
          for(int i = 0; i < card.count; i++) {
            rtrnr.Add(card);
          }
        }
        return rtrnr;
      }

      public Stack<Card> deck() {
        return shuffledDeck(freshDeck());
      }

      public override string ToString() {
        return "ID: " + ID + ": **" + Title + "**" + " (" + Card.cardLegalityString(cardLegality);
      }

      public string deckShort(int length = 50) {
        if (description != null) {
          if ( description.Length >= length) {
            return description; 
          } else {
            return description.Substring(0,length);
          }
        } else {
          return "No description";
        }
      }

      public Embed embed() {
        var embed = new EmbedBuilder();

        embed.WithTitle("ID " + ID + ": " + Title);
        embed.WithDescription(description);
        switch(cardLegality) {
          case CardLegality.BLUE:
            embed.WithColor(Color.Blue);
          break;
          case CardLegality.RED:
            embed.WithColor(Color.Red);
          break;
          case CardLegality.YELLOW:
            embed.WithColor(Color.Gold);
          break;
        }
        foreach (var type in Card.CardTypes) {
          foreach (var legality in Card.CardLegalities) {
            var filtered_cards = cards.Where(e => e.cardType == type && e.cardLegality == legality);
            if (filtered_cards.Count() != 0 ) {
              var strs = new List<string>();
              foreach (var card in filtered_cards) {
                strs.Add(card.count + " " + card.title);
              }
              embed.AddField(Card.cardTypeString(type) + " - " + Card.cardLegalityString(legality), string.Join(System.Environment.NewLine, strs), true);
            }
          }
        }
        embed.AddField("Card Count:", cards.Count().ToString());

        return embed.Build();
      }

      public bool addCard(Card card, Car car, Racer racer) {
        foreach (var condition in card.conditions) {
          if (!condition.check(car, racer)) {
            return false;
          }
        }
        cards.Add(card);
        return true;
      }

    }

  public partial class Deck
  {
    private static bool verifyComplete(Deck deck) {
      var count = 0;
      foreach (var card in deck.cards) {
        count += card.count;
      }
      return count == 20;
    }

    public static bool fromDictionary(Dictionary<string, string> inputs, Car car, Racer r, out string error, out Deck deck) {
      int carId;
      CardLegality legality;
      string title, desc;
      deck = null;
      error = null;

      if (inputs.ContainsKey("carid")) {
        if (!int.TryParse(inputs["carid"], out carId)) {
          error = "Required key `carid` did not provide an integer value";
          return false;
        }
        inputs.Remove("carid");
      } else {
        error = "Required key `carid` not found.";
        return false;
      }

      if (inputs.ContainsKey("legality")) {
        legality = Card.stringToCardLegality(inputs["legality"]);
        inputs.Remove("legality");
      } else {
        error = "Required key `legality` not found";
        return false;
      }

      if (inputs.ContainsKey("title")) {
        title = inputs["title"];
        inputs.Remove("title");
      } else {
        error = "Required key `title` not found";
        return false;
      }

      if (inputs.ContainsKey("description")) {
        desc = inputs["description"];
        inputs.Remove("description");
      } else {
        desc = "No Description";
      }

      deck = new Deck() {
        Title = title,
        description = desc,
        carMapped = carId,
        cardLegality = legality,
      };

      // Process cards
      foreach (var key in inputs.Keys) {
        if(int.TryParse(key, out int keyInt)) {
          if(int.TryParse(inputs[key], out int count)) {
            var card = Card.get_card("card.json", keyInt);
            card.count = count;
            if (!deck.addCard(card, car, r)) {
              error = "Failed to add card " + card.ToString();
              return false;
            }
          } else {
            deck = null;
            error = "Failed to get an int for key: " + key + ". Got " + inputs[key];
            return false;
          }
        } else {
          if (error == null) {
            error = "Failed to process key(s): " + key;
          } else {
            error += ", " + key;
          }
        }
      }

      if (error != null) {
        deck = null;
        return false;
      }

      if (!Deck.verifyComplete(deck)) {
        error = "Deck does not have twenty cards. Returning deck so user can see output";
        return false;
      }

      return true;

    }
    public static Stack<Card> shuffledDeck(List<Card> cards) {
      var rtrnr = new Stack<Card>();
      while (cards.Count != 0) {
        var num = Program.rand.Next(cards.Count);
        rtrnr.Append(cards[num]);
        cards.RemoveAt(num);
      }
      return rtrnr;
    }
    public static Deck[] FromJson(string json) => JsonConvert.DeserializeObject<Deck[]>(json, Converter.Settings);
    public static List<Deck> get_deck () {
      var store = new DataStore ("deck.json");

      var rtrnr = store.GetCollection<Deck> ().AsQueryable ().ToList();
      store.Dispose();

      // Get employee collection
      return rtrnr;
    }

    public static List<Deck> get_deck(ulong id_player, ulong id_server) {
      var store = new DataStore ("deck.json");

      // Get employee collection
      var rtrnr = new List<Deck>();
      rtrnr.AddRange(store.GetCollection<Deck> ().AsQueryable ().Where(e => e.player_discord_id == id_player && e.server_discord_id == id_server ));
      store.Dispose();
      return rtrnr;
    }

    public static Deck get_deck (long id) {
      var store = new DataStore ("deck.json");

      // Get employee collection
      var rtrnr = store.GetCollection<Deck> ().AsQueryable ().FirstOrDefault (e => e.ID == id);
      store.Dispose();
      return rtrnr;
    }

    public static Deck get_deck (string name) {
      var store = new DataStore ("deck.json");

      // Get employee collection
      var rtrnr = store.GetCollection<Deck> ().AsQueryable ().FirstOrDefault (e => e.Title == name);
      store.Dispose();
      return rtrnr;
    }

    public static void insert_deck (Deck deck) {
      var store = new DataStore ("deck.json");

      // Get employee collection
      store.GetCollection<Deck> ().InsertOneAsync (deck);

      store.Dispose();
    }

    public static void replace_deck (Deck deck) {
      var store = new DataStore ("deck.json");

      store.GetCollection<Deck> ().ReplaceOneAsync (e => e.ID == deck.ID, deck);
      store.Dispose();
    }

    public static void delete_deck (Deck deck) {
      var store = new DataStore ("deck.json");

      store.GetCollection<Deck> ().DeleteOne (e => e.ID == deck.ID);
      store.Dispose();
    }
  }
}