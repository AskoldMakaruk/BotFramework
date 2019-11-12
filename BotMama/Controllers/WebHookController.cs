using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace BotMama.Controllers
{
    [ApiController]
    [Route("WebHook")]
    public class WebHookController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            Update update;
            using(var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                update = JsonConvert.DeserializeObject<Update>(await reader.ReadToEndAsync());
            }

            //todo send update to correct bot
            return Ok();
        }
    }
}