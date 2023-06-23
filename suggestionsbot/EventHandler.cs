using DisCatSharp.Enums;
using DisCatSharp.EventArgs;
using DisCatSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisCatSharp.Entities;
using System.ComponentModel;

namespace suggestionsbot
{
    [EventHandler]
    public class MyEventHandler
    {
        [Event]
        private static async Task MessageCreated(DiscordClient s, MessageCreateEventArgs e) 
        {
            ulong submission = 720695699737673768;
            ulong review = 1083692771631702016;
            ulong cacheaudit = 1121855303466614984;

            DiscordChannel reviewchannel = s.GetChannelAsync(review).Result;
            DiscordChannel cachechannel = s.GetChannelAsync(cacheaudit).Result;

            DiscordAttachment attachment = e.Message.Attachments.FirstOrDefault();
            
            var x = DownloadFileAndSaveInCache(attachment.Url);
            DiscordMessageBuilder cachemessage = new DiscordMessageBuilder();
            cachemessage.WithFile($"{e.Author}_{e.Message.Id}", x.Result);
            var fromCache = cachemessage.SendAsync(cachechannel);

            if (e.Channel.Id == submission && !e.Author.IsBot)
            {
               DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
                embed.Title = e.Message.Author.UsernameWithGlobalName;
                embed.Description = e.Message.Content;
                embed.ImageUrl = fromCache.Result.Content.AttachedImageUrl();
                
                await s.SendMessageAsync(reviewchannel, embed);
            }

        }

        static async Task<MemoryStream> DownloadFileAndSaveInCache(string fileUrl)
        {
            byte[] fileContent;
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(fileUrl))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        using (Stream stream = await response.Content.ReadAsStreamAsync())
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memoryStream);
                            //fileContent = memoryStream.ToArray();
                            //return fileContent;
                            return memoryStream;
                        }
                    }
                    else
                    {
                        // Handle the case where the file download was not successful
                        Console.WriteLine("File download failed. Status code: " + response.StatusCode);
                        return null;
                    }
                }
            }
        }


    }

}
