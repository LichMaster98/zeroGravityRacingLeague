using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using zgrl.Classes;

namespace zgrl.Commands
{
    public class CardCommands : ModuleBase<SocketCommandContext>
    {


        [Command("cards")]
        public async Task showCardsAsync() {
            var cards = Card.get_card("card.json");
            var str = new List<string>();
            str.Add("**Card List**");
            foreach (Classes.Card card in cards) {
                str.Add(card.ToString());
            }
            helpers.output(Context.User,str);
        }

        [Command("cards")]
        public async Task showOneCardAsync(int i) {
            var card = Card.get_card("card.json", i);
            await ReplyAsync(null, false, card.embed());
        }

    }
}