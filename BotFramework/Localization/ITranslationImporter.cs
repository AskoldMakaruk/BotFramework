using System.IO;
using System.Threading.Tasks;

namespace BotFramework.Localization;

public interface ITranslationImporter
{
    string FormatName { get; }

    Task Import(MemoryStream stream);
}