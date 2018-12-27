using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Telegram.Bot.Types;

namespace TelegramBotCore.Telegram.Commands
{
    public class MainCommand : Command
    {
        public override AccountStatus Status => AccountStatus.Free;

        public override async void Execute(Message message, Bot client, Account account)
        {

            if (message.Document != null || message.Photo != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    if (message.Document != null)
                        await client.GetInfoAndDownloadFileAsync(message.Document.FileId, ms);
                    else
                        await client.GetInfoAndDownloadFileAsync(message.Photo[0].FileId, ms);
                    Bitmap bm = new Bitmap(ms);
                    double ratio = 512 / bm.Height > bm.Width ? bm.Height : bm.Width;
                    bm = new Bitmap(bm, (int)(bm.Height * ratio), (int)(bm.Width * ratio));
                    ms.Seek(0, SeekOrigin.Begin);
                    bm.Save(ms, ImageFormat.Png);
                    await client.SendPhotoAsync(account.ChatId, new InputMedia(ms, "photo.png"));
                    bm.Dispose();
                }
            }

        }
    }
}
