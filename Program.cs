using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;


using GoogleHelper;


namespace DiscordRandomPicBot
{
    public class Program
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        private DriveHelper _driveHelper;
        const ulong _guildId = 848289290295443456;
        const ulong _targetChannelId = 1190739342554042419;
        const ulong _nsfwChannelId = 1192445188401008701;
        const ulong _picturesChannelId = 1121286759997775932;

        static void Main() => new Program().RunBotAsync().GetAwaiter().GetResult();
        public async Task RunBotAsync()
        {
            var config = new DiscordSocketConfig()
            {
                // Other config options can be presented here.
                GatewayIntents = GatewayIntents.All
            };

            _client = new DiscordSocketClient(config);
            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            await RegisterCommandsAsync();

            _client.SlashCommandExecuted += SlashCommandHandler;

            _client.Log += Log;

            _client.Ready += ClientReady;

            string discordKey = "";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                //TODO
            }
            else
            {
                //TODO Change
                using (FileStream fStream = new FileStream("C:\\Users\\Oleg\\source\\repos\\DiscordRandomPicBot\\discordKey.txt", FileMode.Open, FileAccess.Read))
                {
                    byte[] buffer = new byte[fStream.Length];
                    await fStream.ReadAsync(buffer);
                    discordKey = Encoding.Default.GetString(buffer);
                }
            }

            await _client.LoginAsync(TokenType.Bot, discordKey);

            await _client.StartAsync();

            _driveHelper = new DriveHelper();

            await _driveHelper.Start();

            await RegisterTimerMessagingAsync();

            await Task.Delay(-1);
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task RunPeriodicallyAsync(
            Func<Task> func,
            TimeSpan interval,
            CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(interval, cancellationToken);
                await func();
                await Console.Out.WriteLineAsync("Timer triggered");
            }
        }

        public async Task RegisterTimerMessagingAsync()
        {
            //int delayInMilliseconds = new Random().Next(420000, 900000);

            //int delayInMilliseconds = 3600000;

            TimeSpan delay = TimeSpan.FromHours(1);

            bool isFirstRun = true;

            var targetChannel = _client.GetChannel(_picturesChannelId) as IMessageChannel;

            while (true)
            {
                if (isFirstRun)
                {
                    await SendRandomFileFromGoogleDrive();
                    isFirstRun = false;
                }
                else
                {
                    await RunPeriodicallyAsync(SendRandomFileFromGoogleDrive, delay, CancellationToken.None);
                }
            }
        }

        public async Task SendRandomFileFromGoogleDrive()
        {
            using var client = new HttpClient();

            var file = await GetRandomFileFromGoogleDrive();

            var fileBytes = await client.GetByteArrayAsync(file.WebContentLink);

            using var stream = new MemoryStream(fileBytes);

            var targetChannel = _client.GetChannel(_picturesChannelId) as IMessageChannel;

            await targetChannel!.SendFileAsync(stream, file.Name, $"От {await _driveHelper.GetFileNameByID(file.Parents.First())}\n{file.Name}");
        }

        public async Task<Google.Apis.Drive.v3.Data.File> GetRandomFileFromGoogleDrive()
        {
            var file = await _driveHelper.GetRandomFile(Folder.Pictures);

            string pathToTxt;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                pathToTxt = Path.Combine("discord-bot", "uniqueID.txt");
            }
            else
            {
                var path = Directory.GetCurrentDirectory();

                pathToTxt = Path.Combine(path, "uniqueID.txt");
            }



            using (StreamReader streamReader = new StreamReader(pathToTxt, Encoding.Default))
            {
                string? line;

                while ((line = streamReader.ReadLine()) != null)
                {
                    if (line == file.Id)
                    {
                        return await GetRandomFileFromGoogleDrive();
                    }
                }
            }

            using (StreamWriter streamWriter = new StreamWriter(pathToTxt, true, Encoding.Default))
            {
                streamWriter.WriteLine(file.Id);
            }

            return file;
        }

        public async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);

            if (message.Author.IsBot) return;

            int argPos = 0;
            if (message.HasStringPrefix("!", ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
            }
        }

        public async Task SlashCommandHandler(SocketSlashCommand command)
        {
            await command.RespondAsync($"You executed {command.Data.Name}");
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg.ToString());
            return Task.CompletedTask;
        }

        public async Task ClientReady()
        {
            // Let's build a guild command! We're going to need a guild so lets just put that in a variable.
            var guild = _client.GetGuild(_guildId);

            // Next, lets create our slash command builder. This is like the embed builder but for slash commands.
            var guildCommand = new SlashCommandBuilder();

            // Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
            guildCommand.WithName("rand_pic");

            // Descriptions can have a max length of 100.
            guildCommand.WithDescription("Return a random picture!");

            // Let's do our global command
            var globalCommand = new SlashCommandBuilder();
            globalCommand.WithName("rand_pic");
            globalCommand.WithDescription("Return a random picture!");

            try
            {
                // Now that we have our builder, we can call the CreateApplicationCommandAsync method to make our slash command.
                await guild.CreateApplicationCommandAsync(guildCommand.Build());

                // With global commands we don't need the guild.
                await _client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
                // Using the ready event is a simple implementation for the sake of the example. Suitable for testing and development.
                // For a production bot, it is recommended to only run the CreateGlobalApplicationCommandAsync() once for each command.
            }
            catch (ApplicationCommandException exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                Console.WriteLine(json);
            }
        }
    }

    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("hello")]
        public async Task HelloCommand()
        {
            await ReplyAsync("Hello, I'm a bot!");
        }

        [Command("rand_any")]
        public async Task RandomAnyCommand()
        {
            var file = FromFolder.RandomFile();
            var targetChannel = Context.Client.GetChannel(Context.Channel.Id) as IMessageChannel;

            if (File.Exists(file))
            {
                await targetChannel.SendFileAsync(file);
            }

            //await SendImageFromFolder(); await _program.SendImageFromFolder(file, Context.Channel.Id);
            //var photo = await NekosAPIImageController.RandomAnimeImage();
            //await ReplyAsync(photo.Items.First().Image_Url.ToString());


            Console.WriteLine($"Rand_any send to {targetChannel}");
        }

        [Command("rand_pic")]
        public async Task RandomPictureCommand()
        {
            var file = FromFolder.RandomPicture();
            var targetChannel = Context.Client.GetChannel(Context.Channel.Id) as IMessageChannel;

            if (File.Exists(file))
            {
                await targetChannel.SendFileAsync(file);
            }

            Console.WriteLine($"Rand_pic send to {targetChannel}");
        }

        [Command("rand_vid")]
        public async Task RandomVideoCommand()
        {
            var file = FromFolder.RandomVideo();
            var targetChannel = Context.Client.GetChannel(Context.Channel.Id) as IMessageChannel;

            if (File.Exists(file))
            {
                await targetChannel.SendFileAsync(file);
            }

            Console.WriteLine($"Rand_vid send to {targetChannel}");
        }

        [Command("rand_gif")]
        public async Task RandomGif()
        {
            var file = FromFolder.RandomGif();
            var targetChannel = Context.Client.GetChannel(Context.Channel.Id) as IMessageChannel;

            if (File.Exists(file))
            {
                await targetChannel.SendFileAsync(file);
            }

            Console.WriteLine($"Rand_gif send to {targetChannel}");
        }

        [Command("tenor_gif")]
        public async Task RandomTenorGif()
        {
            var url = FromTenor.RandomGif(await FromTenor.SearchContent());
            var targetChannel = Context.Client.GetChannel(Context.Channel.Id) as IMessageChannel;

            using (var client = new HttpClient())
            {
                var content = await client.GetByteArrayAsync(url.ToString());

                var fileFormat = FileFormat.GetFileFormat(content);

                if (fileFormat == ImageFormat.gif)
                {
                    var stream = new MemoryStream(content);

                    await (Context.Channel as IMessageChannel).SendFileAsync(stream, "image.gif");
                }
                else
                {
                    var stream = new MemoryStream(content);

                    await (Context.Channel as IMessageChannel).SendFileAsync(stream, "image.png");
                }
            }

            Console.WriteLine($"RandTenor_gif send to {targetChannel}");
        }

        [Command("tenor_gif")]
        public async Task RandomTenorGif(string search)
        {
            var url = FromTenor.RandomGif(await FromTenor.SearchContent(search));
            var targetChannel = Context.Client.GetChannel(Context.Channel.Id) as IMessageChannel;

            using (var client = new HttpClient())
            {
                var content = await client.GetByteArrayAsync(url.ToString());

                var fileFormat = FileFormat.GetFileFormat(content);

                if (fileFormat == ImageFormat.gif)
                {
                    var stream = new MemoryStream(content);

                    await (Context.Channel as IMessageChannel).SendFileAsync(stream, "image.gif");
                }
                else
                {
                    var stream = new MemoryStream(content);

                    await (Context.Channel as IMessageChannel).SendFileAsync(stream, "image.png");
                }
            }

            Console.WriteLine($"RandTenor_gif send to {targetChannel}");
        }

        [Command("nsfw")]
        public async Task RandomNSFW()
        {
            var targetChannel = Context.Client.GetChannel(Context.Channel.Id) as IMessageChannel;

            if (targetChannel.Id != 1192445188401008701)
            {
                await ReplyAsync("Not allowed in this channel");
                return;
            }

            var answer = await Task.Run(FromR34.RandomNSFW);

            using (var client = new HttpClient())
            {
                var content = await client.GetByteArrayAsync(answer.Item1);

                var fileFormat = FileFormat.GetFileFormat(content);

                if (fileFormat == ImageFormat.unknown)
                {
                    await ReplyAsync(answer.Item1.ToString() + "\n" + $"Tags: {answer.Item2}");
                }
                else if (fileFormat == ImageFormat.gif)
                {
                    var stream = new MemoryStream(content);

                    await (Context.Channel as IMessageChannel).SendFileAsync(stream, "image.gif", $"\n Tags: {answer.Item2}");
                }
                else
                {
                    var stream = new MemoryStream(content);

                    await (Context.Channel as IMessageChannel).SendFileAsync(stream, "image.png", $"\n Tags: {answer.Item2}");
                }
            }

            //await ReplyAsync(answer.Item1.ToString());

            Console.WriteLine($"Rand_NSFW send to {targetChannel}");
        }

        [Command("+tag")]
        public async Task AppendTag(string tag)
        {
            var targetChannel = Context.Client.GetChannel(Context.Channel.Id) as IMessageChannel;

            if (targetChannel.Id != 1192445188401008701)
            {
                await ReplyAsync("Not allowed in this channel");
                return;
            }

            var answer = await FromR34.ConfirmTag(tag);

            if (!answer.Item1)
            {
                await ReplyAsync(answer.Item2);
                Console.WriteLine($"tag {tag} not added {targetChannel}");
                return;
            }

            await ReplyAsync($"Tag {tag} was added to tag list");

            Console.WriteLine($"tag {tag} added {targetChannel}");
        }

        [Command("-tag")]
        public async Task RemoveTag(string tag)
        {
            var targetChannel = Context.Client.GetChannel(Context.Channel.Id) as IMessageChannel;

            if (targetChannel.Id != 1192445188401008701)
            {
                await ReplyAsync("Not allowed in this channel");
                return;
            }

            var answer = FromR34.RemoveTag(tag);

            if (!answer)
            {
                await ReplyAsync($"Tag {tag} not in list");
                Console.WriteLine($"Tag not removed {targetChannel}");
                return;
            }

            await ReplyAsync($"Tag {tag} was removed from tag list");

            Console.WriteLine($"Tag {tag} removed {targetChannel}");

        }

        [Command("tags")]
        public async Task ShowTags()
        {
            var targetChannel = Context.Client.GetChannel(Context.Channel.Id) as IMessageChannel;

            if (targetChannel.Id != 1192445188401008701)
            {
                await ReplyAsync("Not allowed in this channel");
                return;
            }

            var tags = FromR34.CurrentTags();

            await ReplyAsync("Current tags" + tags);


            //if (File.Exists(file))
            //{
            //    await targetChannel.SendFileAsync(file);
            //}

            Console.WriteLine($"NSFW Tags send to {targetChannel}");
        }

        [Command("tags")]
        public async Task ShowTags(string query)
        {
            var targetChannel = Context.Client.GetChannel(Context.Channel.Id) as IMessageChannel;

            if (targetChannel.Id != 1192445188401008701)
            {
                await ReplyAsync("Not allowed in this channel");
                return;
            }

            var tags = await FromR34.TagsSearch(query);

            await ReplyAsync(tags.Item2);

            //if (File.Exists(file))
            //{
            //    await targetChannel.SendFileAsync(file);
            //}

            Console.WriteLine($"NSFW Tags send to {targetChannel}");
        }

        [Command("animepic")]
        public async Task RandomSafebooru()
        {
            var targetChannel = Context.Client.GetChannel(Context.Channel.Id) as IMessageChannel;

            var answer = await Task.Run(FromSafebooru.RandomContent);

            using (var client = new HttpClient())
            {
                var content = await client.GetByteArrayAsync(answer.Item1);

                var fileFormat = FileFormat.GetFileFormat(content);

                if (fileFormat == ImageFormat.unknown)
                {
                    await ReplyAsync(answer.Item1.ToString() + "\n" + $"Tags: {answer.Item2}");
                }
                else if (fileFormat == ImageFormat.gif)
                {
                    var stream = new MemoryStream(content);

                    await (Context.Channel as IMessageChannel).SendFileAsync(stream, "image.gif", $"\n Tags: {answer.Item2}");
                }
                else
                {
                    var stream = new MemoryStream(content);

                    await (Context.Channel as IMessageChannel).SendFileAsync(stream, "image.png", $"\n Tags: {answer.Item2}");
                }
            }

            //await ReplyAsync(answer.Item1.ToString());

            Console.WriteLine($"Rand_Safebooru send to {targetChannel}");
        }

        [Command("+a_tag")]
        public async Task AppendTagSafebooru(string tag)
        {
            var targetChannel = Context.Client.GetChannel(Context.Channel.Id) as IMessageChannel;

            var answer = await FromSafebooru.ConfirmTag(tag);

            if (!answer.Item1)
            {
                await ReplyAsync(answer.Item2);
                Console.WriteLine($"tag {tag} not added {targetChannel}");
                return;
            }

            await ReplyAsync($"Tag {tag} was added to tag list");

            Console.WriteLine($"tag {tag} added {targetChannel}");
        }

        [Command("-a_tag")]
        public async Task RemoveTagSafebooru(string tag)
        {
            var targetChannel = Context.Client.GetChannel(Context.Channel.Id) as IMessageChannel;

            var answer = FromSafebooru.RemoveTag(tag);

            if (!answer)
            {
                await ReplyAsync($"Tag {tag} not in list");
                Console.WriteLine($"Tag not removed {targetChannel}");
                return;
            }

            await ReplyAsync($"Tag {tag} was removed from tag list");

            Console.WriteLine($"Tag {tag} removed {targetChannel}");

        }

        [Command("a_tags")]
        public async Task ShowTagsSafebooru()
        {
            var targetChannel = Context.Client.GetChannel(Context.Channel.Id) as IMessageChannel;

            var tags = FromSafebooru.CurrentTags();

            await ReplyAsync("Current tags" + tags);


            //if (File.Exists(file))
            //{
            //    await targetChannel.SendFileAsync(file);
            //}

            Console.WriteLine($"Safebooru Tags send to {targetChannel}");
        }

        [Command("a_tags")]
        public async Task ShowTagsSafebooru(string query)
        {
            var targetChannel = Context.Client.GetChannel(Context.Channel.Id) as IMessageChannel;

            var tags = await FromSafebooru.TagsSearch(query);

            await ReplyAsync(tags.Item2);

            //if (File.Exists(file))
            //{
            //    await targetChannel.SendFileAsync(file);
            //}

            Console.WriteLine($"Safebooru Tags send to {targetChannel}");
        }
    }
}

