using System.IO;
using System.Threading.Tasks;

namespace BotFramework.Localization;

public interface ITranlationExporter
{
    string FormatName { get; }

    Task<MemoryStream> Export();
}