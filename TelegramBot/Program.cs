using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Args;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Extensions;
using System.Diagnostics;

namespace TelegramBot
{
    internal class Program
    {
        static TelegramBotClient bot;
        static void Main(string[] args)
        {
            string token = File.ReadAllText(@"C:\Users\Ivanovsv\Desktop\Lessons\Lesson_9\token.txt");

            bot = new TelegramBotClient(token);
            bot.StartReceiving(Update, Error);
            Console.ReadLine();
        }

        async static Task Update(ITelegramBotClient borClient, Telegram.Bot.Types.Update update, CancellationToken token)
        {
            var message = update.Message;
            if (message.Text != null)
            {
                Console.WriteLine($"{message.Chat.FirstName}: {message.Text}");
                if(message.Text.ToLower().Contains("здорова"))
                {
                  await  bot.SendTextMessageAsync(message.Chat.Id, "Здоровей видали");
                    return;
                }    
            }
            if (message.Document != null)
            {
                var failId = update.Message.Document.FileId;
                var fileInfo = await borClient.GetFileAsync(failId);
                var filePath = fileInfo.FilePath;
                //Скачивание
                string destinationFilePath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\{message.Document.FileName}";
                await using FileStream fileStream = System.IO.File.OpenWrite(destinationFilePath);
                await borClient.DownloadFileAsync(filePath, fileStream);
                fileStream.Close();

                //для обработки фото
                Process.Start(@"C:\Program Files\totalcmd\TOTALCMD64.EXE",$@"""{destinationFilePath}""");
                //время на обработку
                await Task.Delay(2500);

                //отправка
                await using Stream stream = System.IO.File.OpenRead(destinationFilePath);
                await borClient.SendDocumentAsync(message.Chat.Id, new InputOnlineFile(stream, message.Document.FileName.Replace(".jpg", " (edited).jpg"));
            }

        }

        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }
    }
}