using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Extensions;
using BotFramework.Services.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BotFramework.Localization;

public interface ITranlationExporter
{
    string FormatName { get; }

    Task<MemoryStream> Export();
}

public interface ITranslationImporter
{
    string FormatName { get; }

    Task Import(MemoryStream stream);
}

public class CsvTranslationImporter : ITranslationImporter
{
    private readonly TranslationsService _translationsService;
    public           string              FormatName => ".csv";

    public CsvTranslationImporter(TranslationsService translationsService)
    {
        _translationsService = translationsService;
    }

    private const char s = ';';

    public async Task Import(MemoryStream stream)
    {
        var text  = Encoding.UTF8.GetString(stream.ToArray());
        var lines = text.Split('\n');

        var locales = lines[0].Split(s).Skip(1).Where(a => !string.IsNullOrWhiteSpace(a)).ToList();

        var result = locales.ToDictionary(locale => locale, _ => new Dictionary<string, string>());

        foreach (var line in lines)
        {
            var words        = line.Split(s);
            var keyname      = words[0];
            var translations = words[1..];

            var i = 0;
            foreach (var locale in locales)
            {
                result[locale][keyname] = translations[i];
                i++;
            }
        }

        await _translationsService.ImportAsync(new TranslationExport(result));
    }
}

public class CsvTranslationExporter : ITranlationExporter
{
    private readonly TranslationsService _translationsService;
    public           string              FormatName => ".csv";

    public CsvTranslationExporter(TranslationsService translationsService)
    {
        _translationsService = translationsService;
    }

    private const char s = ';';

    public async Task<MemoryStream> Export()
    {
        var export = await _translationsService.ExportAsync();

        var locales = export.Locales;

        var             memoryStream = new MemoryStream();
        await using var writer       = new StreamWriter(memoryStream, leaveOpen: true);

        await writer.WriteAsync($"Key");
        foreach (var locale in locales)
        {
            await writer.WriteAsync($"{s}{locale}");
        }

        await writer.WriteLineAsync();

        foreach (var (key, translations) in export.Values)
        {
            await writer.WriteAsync($"{key}");
            foreach (var t in translations.OrderBy(a => a.Key))
            {
                await writer.WriteAsync($"{s}{t.Value}");
            }
        }

        await writer.FlushAsync();

        return memoryStream;
    }
}

public class TranslationsService
{
    private static readonly object _containerLock = new();

    public static    TranslationContainer? Container;
    private readonly ITranslationContext   _context;

    public TranslationsService(ITranslationContext context)
    {
        _context = context;
        if (Container == null!)
        {
            Container = GetTranslationContainer().Result;
        }
    }

    public async Task ReloadTranslations()
    {
        var container = await GetTranslationContainer();
        lock (_containerLock)
        {
            Container = container;
        }
    }

    public async Task<TranslationExport> ExportAsync()
    {
        await ReloadTranslations();

        var keyLocales = Container!.Languages.SelectMany(a =>
                                   a.Value.Translations.Select(c => new { Locale = a.Key, Key = c.Key, Value = c.Value }))
                                   .GroupBy(a => a.Key)
                                   .ToDictionary(arg => arg.Key,
                                       grouping => grouping.OrderBy(a => a.Locale).ToDictionary(c => c.Locale, c => c.Value));

        return new TranslationExport(keyLocales);
    }

    public async Task ImportAsync(TranslationExport export)
    {
        var dbLocales      = await _context.BotFrameworkLocales.ToListAsync();
        var dbTranslations = await _context.BotFrameworkTranslations.ToListAsync();

        var locales = export.Locales.Select(a => new BotFrameworkLocale() { LocaleCode = a }).ToList();
        var translations = export.Values.SelectMany(a =>
                                 a.Value.Select(x => new BotFrameworkTranslation()
                                 {
                                     Value      = x.Value,
                                     LocaleCode = a.Key,
                                     KeyName    = x.Key
                                 }))
                                 .ToList();

        _context.BotFrameworkLocales.UpdateManyToMany(dbLocales, locales, a => a.LocaleCode,
            (locale, frameworkLocale) => locale.LocaleCode = frameworkLocale.LocaleCode);
        _context.BotFrameworkTranslations.UpdateManyToMany(dbTranslations, translations,
            translation => new { translation.KeyName, translation.LocaleCode },
            (translation, frameworkTranslation) =>
            {
                translation.Value      = frameworkTranslation.Value;
                translation.LocaleCode = frameworkTranslation.LocaleCode;
            });

        await _context.SaveChangesAsync();
        await ReloadTranslations();
    }

    private async Task<TranslationContainer> GetTranslationContainer()
    {
        var translations = await _context.BotFrameworkTranslations.ToListAsync();
        var locales      = await _context.BotFrameworkLocales.OrderBy(a => a.LocaleCode).ToListAsync();

        var container = new TranslationContainer(locales.ToDictionary(a => a.LocaleCode,
            a => new TranlsationLanguage(a.LocaleCode)
            {
                Translations = new ConcurrentDictionary<string, string>(translations.Where(c => c.LocaleCode == a.LocaleCode)
                                                                                    .ToDictionary(a => a.KeyName, a => a.Value))
            }));
        return container;
    }
}

//locale key value
public readonly record struct TranslationExport(Dictionary<string, Dictionary<string, string>> Values)
{
    public List<string> Locales => Values.Select(a => a.Key).OrderBy(a => a).ToList();
}

public static class LocalizationExtensions
{
    public static void UseLocalization<T>(this IAppBuilder builder) where T : ITranslationContext
    {
        builder.Services.AddSingleton<TranslationsService>();
        builder.Services.AddSingleton<TranslationContainer>(_ => TranslationsService.Container!);
        builder.Services.AddScoped<Texts>();
    }

    public static void UseLocalizationManagment(this IAppBuilder builder)
    {
        builder.Services.AddSingleton<ITranslationImporter, CsvTranslationImporter>();
        builder.Services.AddSingleton<ITranlationExporter, CsvTranslationExporter>();
    }
}

