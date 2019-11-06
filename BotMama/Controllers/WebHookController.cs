using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BotMama.Controllers
{
    [ApiController]
    [Route("WebHook")]
    public class WebHookController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            DbLoggerCategory.Update update;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                update = JsonConvert.DeserializeObject<DbLoggerCategory.Update>(await reader.ReadToEndAsync());
            }

            //todo send update to correct bot
            return Ok();
        }
    }
}