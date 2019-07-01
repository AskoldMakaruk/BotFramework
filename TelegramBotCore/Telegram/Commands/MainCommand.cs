using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using StickerMemeBot.Telegram.Commands;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using TelegramBotCore.DB.Model;

namespace TelegramBotCore.Telegram.Commands
{
    public class MainCommand : Command
    {
        public MainCommand(Message message, Bot Client, Account Account) : base(message, Client, Account) { }

        public override int Suitability()
        {
            int res = 0;
            if (Account.Status == AccountStatus.Free) res++;
            if (Message.Text != null) res++;
            return res;
        }
        public override async void Execute()
        {

            if (Message.Document != null || Message.Photo != null)
            {
                using(MemoryStream ms = new MemoryStream())
                {
                    if (Message.Document != null)
                        await Client.GetInfoAndDownloadFileAsync(Message.Document.FileId, ms);
                    else
                        await Client.GetInfoAndDownloadFileAsync(Message.Photo[0].FileId, ms);
                    Bitmap bm = new Bitmap(ms);
                    double ratio = 512 / (double) (bm.Height > bm.Width ? bm.Height : bm.Width);
                    bm = new Bitmap(bm, (int) (bm.Height * ratio), (int) (bm.Width * ratio));
                    ms.Seek(0, SeekOrigin.Begin);
                    bm.Save(ms, ImageFormat.Png);
                    ms.Seek(0, SeekOrigin.Begin);
                    await Client.SendDocumentAsync(Account.ChatId, new InputOnlineFile(ms, "photo.png"));
                    bm.Dispose();
                }
            }

        }
    }
}