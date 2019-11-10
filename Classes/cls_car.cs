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
    [JsonProperty("img")]
    public string img {get; set;}
    [JsonProperty("player_id")]
    public ulong player_discord_id { get; set; }
    [JsonProperty("server_id")]
    public ulong server_discord_id { get ; set; }
    [JsonProperty("player_count")]
    public int player_count { get; set; }
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
      return Title + " | ID: " + ID + " | Slot: " + player_count;
    }

    public bool update(Dictionary<string, string> inputs, out string error) {
      int result;
      foreach(var token in validKeys) {
        if (inputs.ContainsKey(token.ToLowerInvariant())){
          switch(token.ToLowerInvariant()) {
            case "name":
              Title = inputs[token];
            break;
            case "desc":
              description = inputs[token];
            break;
            case "img":
              img = inputs[token];
            break;
            case "agility":
              if (int.TryParse(inputs[token], out result))
              {
                if (result > 7) {
                  error = "The maximum a car stat can be is 7. Agility input was " + result;
                  return false;
                }
                Agility = result;
              } else {
                error = "You didn't provide a valid number for your adaptability level";
                return false;
              }
            break;
            case "armor":
              if (int.TryParse(inputs[token], out result))
              {
                if (result > 7) {
                  error = "The maximum a car stat can be is 7. Armor input was " + result;
                  return false;
                }
                Agility = result;
              } else {
                error = "You didn't provide a valid number for your armor level";
                return false;
              }
            break;
            case "attack":
              if (int.TryParse(inputs[token], out result))
              {
                if (result > 7) {
                  error = "The maximum a car stat can be is 7. Attack input was " + result;
                  return false;
                }
                Agility = result;
              } else {
                error = "You didn't provide a valid number for your attack level";
                return false;
              }
            break;
            case "hull":
              if (int.TryParse(inputs[token], out result))
              {
                if (result > 7) {
                  error = "The maximum a car stat can be is 7. Hull input was " + result;
                  return false;
                }
                Agility = result;
              } else {
                error = "You didn't provide a valid number for your hull level";
                return false;
              }
            break;
            case "speed":
              if (int.TryParse(inputs[token], out result))
              {
                if (result > 7) {
                  error = "The maximum a car stat can be is 7. Speed input was " + result;
                  return false;
                }
                Agility = result;
              } else {
                error = "You didn't provide a valid number for your speed level";
                return false;
              }
            break;
            case "tech":
              if (int.TryParse(inputs[token], out result))
              {
                if (result > 7) {
                  error = "The maximum a car stat can be is 7. Tech input was " + result;
                  return false;
                }
                Agility = result;
              } else {
                error = "You didn't provide a valid number for your tech level";
                return false;
              }
            break;
          }
        }
      }
      // Logic to verify the car isn't spending more than 16 points on stats
      int count = 0;
      for (int i = 1; count <= 16; i++) {
        bool less = true;
        if (i <= Agility) {
            count += i;
            less = less && false;
        } else {
          less = less && true;
        }
        if (i <= Armor) {
            count += i;
            less = less && false;
        } else {
          less = less && true;
        }
        if (i <= Attack) {
            count += i;
            less = less && false;
        } else {
          less = less && true;
        }
        if (i <= Hull) {
            count += i;
            less = less && false;
        } else {
          less = less && true;
        }
        if (i <= Speed) {
            count += i;
            less = less && false;
        } else {
          less = less && true;
        }
        if (i <= Tech) {
            count += i;
            less = less && false;
        } else {
          less = less && true;
        }
        if (less) {
          error = "Your total spend in car attributes is less than the alotted amount of 16.";
          return false;
        }
      }
      if (count > 16) {
        error = "Your total spend in car attributes is greater than the alotted amount of 62.";
        return false;
      }
      error = "";
      return true;
    }

    public int sensorRange() {
      return 5 * Tech;
    }
  }

  public partial class Car
  {
    public static string[] validKeys = {"name", "desc", "img", "agility", "armor", "attack", "hull", "speed", "tech"};
    public static Car[] FromJson(string json) => JsonConvert.DeserializeObject<Car[]>(json, Converter.Settings);
    public static List<Car> get_Car () {
      var store = new DataStore ("car.json");

      var rtrnr = store.GetCollection<Car> ().AsQueryable ().ToList();
      store.Dispose();

      // Get employee collection
      return rtrnr;
    }

    public static Car get_Car(ulong id_player, ulong id_server, int i) {
      var store = new DataStore ("car.json");

      // Get employee collection
      var rtrnr = store.GetCollection<Car> ().AsQueryable ().FirstOrDefault(e => e.player_discord_id == id_player && e.server_discord_id == id_server && e.player_count == i);
      store.Dispose();
      return rtrnr;
    }

    public static Car[] get_Car(ulong id_player, ulong id_server) {
      var store = new DataStore ("car.json");

      // Get employee collection
      Car[] rtrnr = new Car[2];
      rtrnr[0] = store.GetCollection<Car> ().AsQueryable ().FirstOrDefault(e => e.player_discord_id == id_player && e.server_discord_id == id_server && e.player_count == 0);
      rtrnr[1] = store.GetCollection<Car> ().AsQueryable ().FirstOrDefault(e => e.player_discord_id == id_player && e.server_discord_id == id_server && e.player_count == 1);
      store.Dispose();
      return rtrnr;
    }

    public static Car get_Car (int id) {
      var store = new DataStore ("car.json");

      // Get employee collection
      var rtrnr = store.GetCollection<Car> ().AsQueryable ().FirstOrDefault (e => e.ID == id);
      store.Dispose();
      return rtrnr;
    }

    public static Car get_Car (string name) {
      var store = new DataStore ("car.json");

      // Get employee collection
      var rtrnr = store.GetCollection<Car> ().AsQueryable ().FirstOrDefault (e => e.Title == name);
      store.Dispose();
      return rtrnr;
    }

    public static void insert_Car (Car Car) {
      var store = new DataStore ("car.json");

      // Get employee collection
      store.GetCollection<Car> ().InsertOneAsync (Car);

      store.Dispose();
    }

    public static void replace_Car (Car Car) {
      var store = new DataStore ("car.json");

      store.GetCollection<Car> ().ReplaceOneAsync (e => e.ID == Car.ID, Car);
      store.Dispose();
    }

    public static void delete_Car (Car Car) {
      var store = new DataStore ("car.json");

      store.GetCollection<Card> ().DeleteOne (e => e.ID == Car.ID);
      store.Dispose();
    }
  }
}