using System.Collections.Generic;
using System.Linq;
using JsonFlatFileDataStore;
using Newtonsoft.Json;

namespace zgrl.Classes
{

  public partial class Track
  {
    [JsonProperty("id")]
    public long ID { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
  }

  public partial class Track
  {
    public static Track[] FromJson(string json) => JsonConvert.DeserializeObject<Track[]>(json, Converter.Settings);

    public static List<Track> get_Track (string location) {
        var store = new DataStore (location);

        // Get employee collection
        var rtrner = store.GetCollection<Track> ().AsQueryable ().ToList();
        store.Dispose();
        return rtrner;
    }

    public static Track get_Track (string location, int id) {
        var store = new DataStore (location);

        // Get employee collection
        var rtrner = store.GetCollection<Track> ().AsQueryable ().FirstOrDefault (e => e.ID == id);
        store.Dispose();
        return rtrner;
    }

    public static Track get_Track (string location, string name) {
        var store = new DataStore (location);

        // Get employee collection
        var rtrner = store.GetCollection<Track> ().AsQueryable ().FirstOrDefault (e => e.Title == name);
        store.Dispose();
        return rtrner;
    }

    public static void insert_Track (string location, Track Track) {
        var store = new DataStore (location);

        // Get employee collection
        store.GetCollection<Track> ().InsertOneAsync (Track);

        store.Dispose();
    }

    public static void update_Track (string location, Track Track) {
        var store = new DataStore (location);

        store.GetCollection<Track> ().ReplaceOneAsync (e => e.ID == Track.ID, Track);
        store.Dispose();
    }

    public static void delete_Track (string location, Track Track) {
        var store = new DataStore (location);

        store.GetCollection<Track> ().DeleteOne (e => e.ID == Track.ID);
        store.Dispose();
    }
  }
}