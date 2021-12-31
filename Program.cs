using System;
using XCommas.Net;
using XCommas.Net.Objects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bot_rotator
{
    class Program
    {
        //rotate through bots every 2 minutes that have these names (disable all that are enabled and then enable one at a time)
        static HashSet<string> botNames = new HashSet<string> { "KC200 QFL", "KC200 TV", "KC100 AI" };

        static string key = "xxx";
        static string secret = "xxx";
        static XCommasApi api;

        static void Main() { MainAsync().GetAwaiter().GetResult(); }
        static async Task MainAsync()
        {
            api = new XCommasApi(key, secret, default, UserMode.Real);
            while (true)
            {
                try
                {
                    var accounts = await api.GetAccountsAsync();
                    foreach (string botName in botNames)
                    {

                        //disable all bots in botNames that are enabled
                        foreach (var account in accounts.Data)
                        {
                            var bots = await api.GetBotsAsync(limit: 1000, accountId: account.Id);
                            if (bots.Data == null) continue;
                            foreach (Bot bot in bots.Data) 
                                if (botNames.Contains(bot.Name) && bot.IsEnabled)
                                {
                                    var res = await api.DisableBotAsync(bot.Id);
                                    if (res.IsSuccess) Console.WriteLine($"Successfully disabled {bot.Name} under {bot.AccountName}");
                                    else Console.WriteLine($"Disable failed: {res.Error}");
                                }
                        }

                        //enable the next bot in the botNames hashset
                        foreach (var account in accounts.Data)
                        {
                            var bots = await api.GetBotsAsync(limit: 1000, accountId: account.Id);
                            if (bots.Data == null) continue;
                            foreach (Bot bot in bots.Data) 
                                if (botName == bot.Name && !bot.IsEnabled)
                                {
                                    var res = await api.EnableBotAsync(bot.Id);
                                    if (res.IsSuccess) Console.WriteLine($"Successfully enabled {bot.Name} under {bot.AccountName}");
                                    else Console.WriteLine($"Disable failed: {res.Error}");
                                }
                        }
                        await Task.Delay(1000 * 60 * 2); //wait a couple minutes before going to the next bot
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                    api = new XCommasApi(key, secret, default, UserMode.Real);
                }
            }
        }
    }
}
