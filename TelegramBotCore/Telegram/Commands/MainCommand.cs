using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using TelegramBotCore.DB.Model;
using TelegramBotCore.Telegram.Bot;

namespace TelegramBotCore.Telegram.Commands
{
    public class MainCommand : Command
    {
        public override int Suitability(Message message, Account account)
        {
            var res = 0;
            if (account.Status == AccountStatus.Free) res++;
            return res;
        }

        public override Response Execute(Message message, Client client, Account account)
        {
            if (message.Document != null || message.Photo != null)
            {
                using (var ms = new MemoryStream())
                {
                    if (message.Document != null)
                        client.GetInfoAndDownloadFileAsync(message.Document.FileId, ms).RunSynchronously();
                    else
                        client.GetInfoAndDownloadFileAsync(message.Photo[0].FileId, ms).RunSynchronously();
                    var bm = new Bitmap(ms);

                    var ratio = 512 / (double) (bm.Height > bm.Width ? bm.Height : bm.Width);
                    bm = new Bitmap(bm, (int) (bm.Height * ratio), (int) (bm.Width * ratio));
                    ms.Seek(0, SeekOrigin.Begin);
                    bm.Save(ms, ImageFormat.Png);
                    ms.Seek(0, SeekOrigin.Begin);
                    var res = Response.SendDocument(account, new InputOnlineFile(ms, "photo.png"));
                    bm.Dispose();
                    return res;
                }
            }

            return Response.TextMessage(account, "send photo");
        }
    }
}