using System.Collections.Generic;
using System.Linq;
using JsonFlatFileDataStore;
using Newtonsoft.Json;

enum CardType {
    BLUE = 0,
    YELLOW = 1,
    RED = 2
}

namespace zgrl.Classes {

    public partial class Card {
        [JsonProperty ("ID")]
        public int ID { get; set; }
        [JsonProperty ("count")]
        public int count { get; set; }

        [JsonProperty ("title")]
        public string title { get; set; }

        [JsonProperty ("description")] 
        public string description { get; set; }

        [JsonProperty("success")]
        public string success { get; set; }

        [JsonProperty("failure")]
        public string failure { get; set; }

        [JsonProperty("cardType")]
        public int cardType { get; set; }

        public string cardTypeString() {
            switch (cardType) {
                case 0:
                    return "Blue";
                case 1:
                    return "Yellow";
                case 2:
                    return "Red";
            }
            return "UNDEFINED";
        }


    }
    public partial class Card
    {
        public static Card[] FromJson(string json) => JsonConvert.DeserializeObject<Card[]>(json, Converter.Settings);

        public override string ToString() {
            return "**" + title + "** : " + description;
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