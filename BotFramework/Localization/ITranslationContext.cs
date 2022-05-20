using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BotFramework.Localization;

public interface ITranslationContext
{
    public DbSet<BotFrameworkTranslation> BotFrameworkTranslations { get; }
    public DbSet<BotFrameworkLocale>      BotFrameworkLocales      { get; }

    public Task<int> SaveChangesAsync();
}