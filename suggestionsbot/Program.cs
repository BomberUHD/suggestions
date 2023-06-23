using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.Enums;
using Microsoft.Extensions.DependencyInjection;
using suggestionsbot;
using System.Reflection;

namespace MyFirstBot
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = "NDg1MTYzNTg4OTMyMjA2NjE0.GjmQgG.6z__6G4y--2KT42_2t13y26GdoMPnlASQ80O8k",
                TokenType = TokenType.Bot,
                ServiceProvider = new ServiceCollection().AddScoped<Program>().AddSingleton<MyEventHandler>().BuildServiceProvider(),
                Intents = DiscordIntents.All
                //Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContent
            });
            
            //MyEventHandler handler = new();
            //discord.RegisterEventHandler(handler);
            discord.RegisterEventHandlers(Assembly.GetExecutingAssembly());
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
