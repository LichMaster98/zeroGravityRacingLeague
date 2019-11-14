using System.Collections.Generic;
using System.Linq;
using JsonFlatFileDataStore;
using Newtonsoft.Json;
using Discord.Commands;
using Discord;

public enum CardLegality {
    BLUE = 0,
    YELLOW = 1,
    RED = 2,
    INVALID = 3
}

public enum CardType {
    Utility = 0,
    Defensive = 1,
    Offensive = 2,
    TechAttack = 3,
    DefensiveOffensive = 4,
    INVALID = 5
}

namespace zgrl.Classes {

    public partial class Card {
        [JsonProperty ("ID")]
        public int ID { get; set; }
        [JsonProperty ("count")]
        public int count { get; set; } = 1;

        [JsonProperty ("title")]
        public string title { get; set; } = "";
        [JsonProperty("customtitle")]
        public string customTitle { get; set; }

        [JsonProperty ("description")] 
        public string description { get; set; } = "";
        [JsonProperty("customLore")]
        public string customLore {get; set;}
        [JsonProperty("img")]
        public string img { get; set; } = "";

        [JsonProperty("success")]
        public string success { get; set; } = "";

        [JsonProperty("failure")]
        public string failure { get; set; } = "";

        [JsonProperty("cardLegality")]
        public CardLegality cardLegality { get; set; } = CardLegality.INVALID;
        [JsonProperty("cardType")]
        public CardType cardType { get; set; } = CardType.INVALID;

        public Embed embed() {
            var embed = new EmbedBuilder();

            embed.WithTitle(embedTitle());
            embed.WithDescription(completeDescription());
            embed.WithThumbnailUrl(img);
            embed.AddField("ID",ID.ToString(),true);
            embed.AddField("Card Legality",Card.cardLegalityString(cardLegality), true);
            embed.AddField("Card Type", Card.cardTypeString(cardType), true);
            if (customLore != null) embed.AddField("Lore", customLore, true);
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

            return embed.Build();
        }

        public string completeDescription() {
            var rtrner = description;
            if (!success.Equals("")) {
                rtrner += System.Environment.NewLine + "**Success:**" + System.Environment.NewLine + success;
            }
            if (!failure.Equals("")) {
                rtrner += System.Environment.NewLine + "**Failure:**" + System.Environment.NewLine + failure;
            }
            return rtrner;
        }

        public bool update(Dictionary<string, string> inputs, out string error) {
            error = "";
            foreach (string token in validUpdateCardKeys) {
                if (inputs.ContainsKey(token)) {
                    switch(token) {
                        case "title":
                            customTitle = inputs[token];
                        break;
                        case "lore":
                            customLore = inputs[token];
                        break;
                        case "img":
                            img = inputs[token];
                        break;
                        default:
                            error = error + System.Environment.NewLine + "Unrecognized Key: " + token + " Value: " + inputs[token];
                        break;
                    }
                }
            }
            return true;
        }

        public bool updateNewCard(Dictionary<string, string> inputs, out string error) {
            error = "";
            foreach (string token in validNewCardKeys) {
                if (inputs.ContainsKey(token)) {
                    switch(token) {
                        case "title":
                            title = inputs[token];
                        break;
                        case "description":
                            description = inputs[token];
                        break;
                        case "lore":
                            customLore = inputs[token];
                        break;
                        case "success":
                            success = inputs[token];
                        break;
                        case "failure":
                            failure = inputs[token];
                        break;
                        case "legality":
                            cardLegality = Card.stringToCardLegality(inputs[token]);
                            if (cardLegality == CardLegality.INVALID) {
                                error = "Card Legality input valid. Received: " + inputs[token];
                                return false;
                            }
                        break;
                        case "type":
                            cardType = Card.stringToCardType(inputs[token]);
                            if (cardType == CardType.INVALID) {
                                error = "Card Type input valid. Received: " + inputs[token];
                                return false;
                            }
                        break;
                        default:
                            error = error + System.Environment.NewLine + "Unrecognized Key: " + token + " Value: " + inputs[token];
                        break;
                    }
                }
            }
            return true;
        }
    }
    public partial class Card
    {
        public static string[] validUpdateCardKeys = {"lore", "title", "img"};
        public static string[] validNewCardKeys = {"title", "description", "lore", "success", "failure", "legality", "type"};
        public static Card[] FromJson(string json) => JsonConvert.DeserializeObject<Card[]>(json, Converter.Settings);

        public static string cardLegalityString(CardLegality cardLegality) {
            switch (cardLegality) {
                case CardLegality.BLUE:
                    return "Blue";
                case CardLegality.YELLOW:
                    return "Yellow";
                case CardLegality.RED:
                    return "Red";
                default:
                    return "INVALID";
            }
        }


        public static CardLegality stringToCardLegality(string input) {
            switch(input.ToLowerInvariant()) {
                case "blue":
                    return CardLegality.BLUE;
                case "red":
                    return CardLegality.RED;
                case "yellow":
                    return CardLegality.YELLOW;
                default:
                    return CardLegality.INVALID;
            }
        }

        public static string cardTypeString(CardType cardType) {
            switch(cardType) {
                case CardType.Defensive:
                    return "Defensive";
                case CardType.DefensiveOffensive:
                    return "Defensive/Offensive";
                case CardType.Offensive:
                    return "Offensive";
                case CardType.TechAttack:
                    return "Tech Attack";
                case CardType.Utility:
                    return "Utility";
                default:
                    return "INVALID";
            }
        }

        public static CardType stringToCardType(string input) {
            switch(input.ToLowerInvariant()) {
                case "defensive":
                    return CardType.Defensive;
                case "defensive/offensive":
                    return CardType.DefensiveOffensive;
                case "offensive":
                    return CardType.Offensive;
                case "tech attack":
                    return CardType.TechAttack;
                case "utility":
                    return CardType.Utility;
                default:
                    return CardType.INVALID;
            }
        }

        public override string ToString() {
            if (customTitle is null) {
                return "**" + title + "** (Legality: " + Card.cardLegalityString(cardLegality) + ") *Type: " + Card.cardTypeString(cardType) + "* Description: " + description;               
            } else {
                return "**" + customTitle + "** (Legality: " + Card.cardLegalityString(cardLegality) + ") *Type: " + Card.cardTypeString(cardType) + "* Description: " + description; 
            }
        }

        private string embedTitle() {
            if (customTitle is null) {
                return "**" + title + "** (Legality: " + Card.cardLegalityString(cardLegality) + ") *Type: " + Card.cardTypeString(cardType) + "*";
            } else {
                return "**" + customTitle + "** (Legality: " + Card.cardLegalityString(cardLegality) + ") *Type: " + Card.cardTypeString(cardType) + "*";
            }
        }

        public static List<Card> get_card (string location) {
            var store = new DataStore (location);

            // Get employee collection
            var rtrner = store.GetCollection<Card> ().AsQueryable ().ToList();
            store.Dispose();
            return rtrner;
        }

        public static Card get_card (string location, int id) {
            var store = new DataStore (location);

            // Get employee collection
            var rtrner = store.GetCollection<Card> ().AsQueryable ().FirstOrDefault (e => e.ID == id);
            store.Dispose();
            return rtrner;
        }

        public static Card get_card (string location, string name) {
            var store = new DataStore (location);

            // Get employee collection
            var rtrner = store.GetCollection<Card> ().AsQueryable ().FirstOrDefault (e => e.title == name);
            store.Dispose();
            return rtrner;
        }

        public static void insert_card (string location, Card card) {
            var store = new DataStore (location);

            // Get employee collection
            store.GetCollection<Card> ().InsertOneAsync (card);

            store.Dispose();
        }

        public static void update_card (string location, Card card) {
            var store = new DataStore (location);

            store.GetCollection<Card> ().ReplaceOneAsync (e => e.ID == card.ID, card);
            store.Dispose();
        }

        public static void delete_card (string location, Card card) {
            var store = new DataStore (location);

            store.GetCollection<Card> ().DeleteOne (e => e.ID == card.ID);
            store.Dispose();
        }
    }

}