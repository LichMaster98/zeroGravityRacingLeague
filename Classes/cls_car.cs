using System.Collections.Generic;
using System.Linq;
using JsonFlatFileDataStore;
using Newtonsoft.Json;

namespace zgrl.Classes
{

  public partial class Car
  {
    [JsonProperty("id")]
    public long ID { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("description")]
    public string description { get; set; }
    [JsonProperty("agility")]
    public int Agility { get; set;}
    [JsonProperty("armor")]
    public int Armor {get; set;}
    [JsonProperty("attack")]
    public int Attack { get; set;}
    [JsonProperty("damage")]
    public int Damage {get; set;}
    [JsonProperty("health")]
    public int Health {get; set;}
    [JsonProperty("hull")]
    public int Hull {get; set;}
    [JsonProperty("speed")]
    public int Speed {get; set;}
    [JsonProperty("tech")]
    public int Tech {get; set;}

    public string racerEmbed() {
      return Title + " | ID: " + ID;
    }
  }

  public partial class Car
  {
    public static Car[] FromJson(string json) => JsonConvert.DeserializeObject<Car[]>(json, Converter.Settings);
    public static List<Car> get_Car (string location) {
      var store = new DataStore (location);

      var rtrnr = store.GetCollection<Car> ().AsQueryable ().ToList();
      store.Dispose();

      // Get employee collection
      return rtrnr;
    }

    public static Car get_Car (string location, int id) {
      var store = new DataStore (location);

      // Get employee collection
      var rtrnr = store.GetCollection<Car> ().AsQueryable ().FirstOrDefault (e => e.ID == id);
      store.Dispose();
      return rtrnr;
    }

    public static Car get_Car (string location, string name) {
      var store = new DataStore (location);

      // Get employee collection
      var rtrnr = store.GetCollection<Car> ().AsQueryable ().FirstOrDefault (e => e.Title == name);
      store.Dispose();
      return rtrnr;
    }

    public static void insert_Car (string location, Car Car) {
      var store = new DataStore (location);

      // Get employee collection
      store.GetCollection<Car> ().InsertOneAsync (Car);

      store.Dispose();
    }

    public static void replace_Car (string location, Car Car) {
      var store = new DataStore (location);

      store.GetCollection<Car> ().ReplaceOneAsync (e => e.ID == Car.ID, Car);
      store.Dispose();
    }

    public static void delete_card (string location, Car Car) {
      var store = new DataStore (location);

      store.GetCollection<Card> ().DeleteOne (e => e.ID == Car.ID);
      store.Dispose();
    }
  }
}