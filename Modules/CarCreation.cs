using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using zgrl.Classes;

namespace zgrl.Commands
{
  public class CarCreation : ModuleBase<SocketCommandContext>
  {
    [Command("createcar")]
    public async Task createCarAsync(params string[] inputs) {
      var r = racer.get_racer(Context.Message.Author.Id, Context.Guild.Id);

      if(r == null) {
        await ReplyAsync("No pilot found for you, you can't create a car without a pilot. Use `zg!createpilot` to make one");
        return;
      }

      var cars = Car.get_Car(Context.Message.Author.Id, Context.Guild.Id);
      int id = -1;
      if (cars[0] == null) id = 0;
      else if (cars[1] == null) id = 1;
      else {
        await ReplyAsync("You can't create a new car while you have two cars made. Use `zg!deletecar [0|1]` to remove one or `zg!updatecar` to change the configuration of a car");
        return;
      }

      var string_to_value = helpers.parseInputs(inputs);
      var car = new Car();
      if (!car.update(string_to_value, out string error)) {
        await ReplyAsync(Context.User.Mention + ". Car creation failed with error message: " + error);
        return;
      }

      car.player_discord_id = Context.Message.Author.Id;
      car.server_discord_id = Context.Guild.Id;
      car.player_count = id;

      Car.insert_Car(car);

      var car_saved = Car.get_Car(Context.Message.Author.Id, Context.Guild.Id, id);

      r.cars.Add(car_saved);
      racer.update_racer(r);

      await ReplyAsync(Context.User.Mention + ", you've created your car. Use `zg!car " + car_saved.ID + "` to see your completed car.");
    }

    [Command("updatecar")]
    public async Task updateCarAsync(params string[] inputs) {
      var r = racer.get_racer(Context.Message.Author.Id, Context.Guild.Id);

      if(r == null) {
        await ReplyAsync("No pilot found for you, you can't update a car without a pilot. Use `zg!createpilot` to make one");
        return;
      }

      var string_to_value = helpers.parseInputs(inputs);
      if (string_to_value.ContainsKey("id")) {
        if (!int.TryParse(string_to_value["id"], out int Id)) {
          await ReplyAsync(Context.User.Mention + ", id didn't contain a number.");
          return;
        } else if (Id < 0 || Id > 1) {
          await ReplyAsync(Context.User.Mention + ", id wasn't `0` or `1`.");
          return;
        }
        var car = Car.get_Car(Context.User.Id, Context.Guild.Id, Id);
        if (!car.update(string_to_value, out string error)) {
          await ReplyAsync(Context.User.Mention + ". Car update failed with error message: " + error);
          return;
        }
        r.cars[Id] = car;
        racer.update_racer(r);
        Car.replace_Car(car);

        await ReplyAsync(Context.User.Mention + ", you've updated a car. Use `zg!car " + car.ID +"` to see your completed car.");

      } else {
        await ReplyAsync(Context.User.Mention + ", I am unsure which car of yours to update. Please provide `id=[0|1]` in the update command.");
        return;
      }

    }

    [Command("removeCar")]
    public async Task removeCarAsync(int i) {
      var r = racer.get_racer(Context.Message.Author.Id, Context.Guild.Id);

      if(r == null) {
        await ReplyAsync("No pilot found for you, you can't remove a car without a pilot. Use `zg!createpilot` to make one");
        return;
      }

      var car = Car.get_Car(Context.User.Id, Context.Guild.Id, i);
      
      if (car == null) {
        await ReplyAsync(Context.User.Mention + ", you can't remove a car in slot ID: " + i + ". Which doesn't exist!");
        return;
      }

      r.cars.RemoveAt(i);
      Car.delete_Car(car);
      racer.update_racer(r);

      await ReplyAsync(Context.User.Mention + ", successfully removed " + car.Title);
    }

    [Command("car")]
    public async Task showCarAsync(int id = -1) {
      List<Car> cars = new List<Car>();
      if (id < 0) {
        var r = racer.get_racer(Context.Message.Author.Id, Context.Guild.Id);
        if ( r == null ) {
          await ReplyAsync(Context.User.Mention + ", you don't have a current pilot or this pilot doesn't exist in the database.");
          return;
        }

        cars.AddRange(Car.get_Car(Context.Message.Author.Id, Context.Guild.Id));
      } else {
        var c = Car.get_Car(id);
        if (c == null) {
          await ReplyAsync(Context.User.Mention + ", this car ID doesn't exist in the databse.");
          return;
        }
        cars.Add(c);
      }

      foreach (Car car in cars) {
        if (car == null) continue;

        var embed = new EmbedBuilder();

        embed.Title = "Car Name: " + car.Title;
        embed.WithDescription(car.description);
        embed.WithThumbnailUrl(car.img);
        embed.AddField("ID",car.ID.ToString(),true);
        embed.AddField(
          "Attributes","Agility: " + car.Agility + System.Environment.NewLine + "Armor: " + car.Armor 
          + System.Environment.NewLine + "Attack: " + car.Attack + System.Environment.NewLine + "Hull"+ car.Hull 
          + System.Environment.NewLine + "Speed" + car.Speed + System.Environment.NewLine + "Tech" + car.Tech, true
        );
        if( id < 0 ) {
          embed.AddField("Player",Context.User.Mention,true);
        } else {
          var usr = Context.Guild.GetUser(car.player_discord_id);
          embed.AddField("Player",usr.Mention,true);
        }

        await Context.Channel.SendMessageAsync("", false, embed.Build(), null);
      }

    }

    [Command("listcars")]
    public async Task listCarsAsync() {
      var s = new List<string>();
      s.Add("Cars!");
      var rcrs = Car.get_Car();
      foreach(Classes.Car r in rcrs) {
          if (r.server_discord_id == Context.Guild.Id) s.Add(r.racerEmbed());
      }
      helpers.output(Context.User, s);
    }
  }

}