using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BotFramework.Localization;

public class CsvTranslationImporter : ITranslationImporter
{
    private readonly ITranslationsService _translationsService;
    public           string               FormatName => ".csv";

    public CsvTranslationImporter(ITranslationsService translationsService)
    {
        _translationsService = translationsService;
    }

    private const char s = ';';

    public async Task Import(MemoryStream stream)
    {
        var text  = Encoding.UTF8.GetString(stream.ToArray());
        var lines = text.Split('\n');

        var locales = lines[0].Split(s).Skip(1).Select(a => a.Trim()).Where(a => !string.IsNullOrWhiteSpace(a)).ToList();

        var result = new List<TransaltionItem>();

        foreach (var line in lines.Skip(1).Where(a => !string.IsNullOrWhiteSpace(a)))
        {
            var words        = line.Split(s);
            var keyname      = words[0];
            var translations = words[1..];

            var i = 0;
            foreach (var locale in locales)
            {
                result.Add(new TransaltionItem(locale, keyname.Trim(), translations[i].Trim()));
                i++;
            }
        }

        await _translationsService.ImportAsync(new TranslationExport(result));
    }
}

public class CsvTranslationExporter : ITranlationExporter
{
    private readonly ITranslationsService _translationsService;
    public           string               FormatName => ".csv";

    public CsvTranslationExporter(ITranslationsService translationsService)
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

        //header
        await writer.WriteAsync($"Key");
        foreach (var locale in locales)
        {
            await writer.WriteAsync($"{s}{locale}");
        }

        await writer.WriteLineAsync();

        //body
        foreach (var key in export.Values.GroupBy(a => a.Key))
        {
            await writer.WriteAsync($"{key.Key}");
            foreach (var locale in locales)
            {
                var v    = key.FirstOrDefault(a => a.Locale == locale);
                var text = v == default ? "" : v.Value;
                await writer.WriteAsync($"{s}{text}");
            }

            await writer.WriteLineAsync();
        }

        await writer.FlushAsync();
        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }
}

public class TranslationsService : ITranslationsService
{
    private static readonly object _containerLock = new();

    public static    TranslationContainer?        Container;
    private readonly ITranslationContext          _context;
    private readonly ILogger<TranslationsService> _logger;

    public TranslationsService(ITranslationContext context, ILogger<TranslationsService> logger)
    {
        _context = context;
        _logger  = logger;
        if (Container == null!)
        {
            Container = GetTranslationContainer().Result;
        }
    }

    private async Task DetectNewKeysAsync()
    {
        if (Container == null)
        {
            return;
        }

        var dbKeys  = await _context.BotFrameworkTranslations.GroupBy(a => a.KeyName).Select(a => a.Key).ToListAsync();
        var locales = await _context.BotFrameworkLocales.Select(a => a.LocaleCode).ToListAsync();
        var keys    = new List<string>();

        foreach (var pair in Container.Languages)
        {
            keys.AddRange(pair.Value.Translations.Where(a => a.Key == a.Value && !dbKeys.Contains(a.Key)).Select(a => a.Key));
        }

        var translations = locales.SelectMany(a => keys.Select(k => new BotFrameworkTranslation()
        {
            LocaleCode = a,
            Value      = k,
            KeyName    = k
        }));

        _context.BotFrameworkTranslations.AddRange(translations);
        await _context.SaveChangesAsync();
    }

    public async Task ReloadTranslations()
    {
        await DetectNewKeysAsync();
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
                                   a.Value.Translations.Select(c => new TransaltionItem()
                                   { Locale = a.Key, Key = c.Key, Value = c.Value }))
                                   .ToList();

        return new TranslationExport(keyLocales);
    }

    public async Task ImportAsync(TranslationExport export)
    {
        var dbLocales      = await _context.BotFrameworkLocales.ToListAsync();
        var dbTranslations = await _context.BotFrameworkTranslations.ToListAsync();

        var locales = export.Locales.Select(a => new BotFrameworkLocale() { LocaleCode = a }).ToList();
        var translations = export.Values
                                 .Select(x => new BotFrameworkTranslation()
                                 {
                                     Value      = x.Value,
                                     LocaleCode = x.Locale,
                                     KeyName    = x.Key
                                 })
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
public readonly record struct TranslationExport(List<TransaltionItem> Values)
{
    public List<string> Locales => Values.GroupBy(a => a.Locale).Select(a => a.Key).OrderBy(a => a).ToList();
    public List<string> Keys    => Values.GroupBy(a => a.Key).Select(a => a.Key).OrderBy(a => a).ToList();
}

public readonly record struct TransaltionItem(string Locale, string Key, string Value);

public static class LocalizationExtensions
{
    public static void UseLocalization<T>(this IAppBuilder builder) where T : class, ITranslationContext
    {
        builder.Services.AddSingleton<ITranslationsService, TranslationsService>();
        builder.Services.AddSingleton<TranslationContainer>(_ => TranslationsService.Container!);
        builder.Services.AddScoped<Texts>();
        builder.Services.AddScoped<ITranslationContext, T>();
    }

    public static void UseLocalizationManagment(this IAppBuilder builder)
    {
        builder.Services.AddSingleton<ITranslationImporter, CsvTranslationImporter>();
        builder.Services.AddSingleton<ITranlationExporter, CsvTranslationExporter>();
    }
}