using System.Threading.Tasks;

namespace BotFramework.Localization;

public interface ITranslationsService
{
    Task                    ReloadTranslations();
    Task<TranslationExport> ExportAsync();
    Task                    ImportAsync(TranslationExport export);
}