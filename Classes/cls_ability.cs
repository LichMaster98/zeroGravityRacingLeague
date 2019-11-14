using System.Collections.Generic;
using System.Linq;
using JsonFlatFileDataStore;
using Newtonsoft.Json;

namespace zgrl.Classes
{

    public partial class Ability
    {
        [JsonProperty("id")]
        public long ID { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("mechanical-descrpition")]
        public string mechDesc { get; set;}
    }

    public partial class Ability
    {
        public static Ability[] FromJson(string json) => JsonConvert.DeserializeObject<Ability[]>(json, Converter.Settings);

        public override string ToString() {
            return "ID: (" + ID + ") **" + Title + "**: " + Description;
        }
        public static List<Ability> get_ability (string location) {
            var store = new DataStore (location);

            var rtrnr = store.GetCollection<Ability> ().AsQueryable ().ToList();
            store.Dispose();

            // Get employee collection
            return rtrnr;
        }

        public static Ability get_ability (string location, int id) {
            var store = new DataStore (location);

            // Get employee collection
            var rtrnr = store.GetCollection<Ability> ().AsQueryable ().FirstOrDefault (e => e.ID == id);
            store.Dispose();
            return rtrnr;
        }

        public static Ability get_ability (string location, string name) {
            var store = new DataStore (location);

            // Get employee collection
            var rtrnr = store.GetCollection<Ability> ().AsQueryable ().FirstOrDefault (e => e.Title == name);
            store.Dispose();
            return rtrnr;
        }

        public static void insert_ability (string location, Ability ability) {
            var store = new DataStore (location);

            // Get employee collection
            store.GetCollection<Ability> ().InsertOneAsync (ability);

            store.Dispose();
        }

        public static void replace_ability (string location, Ability ability) {
            var store = new DataStore (location);

            store.GetCollection<Ability> ().ReplaceOneAsync (e => e.ID == ability.ID, ability);
            store.Dispose();
        }

        public static void delete_card (string location, Ability ability) {
            var store = new DataStore (location);

            store.GetCollection<Card> ().DeleteOne (e => e.ID == ability.ID);
            store.Dispose();
        }
    }

}
