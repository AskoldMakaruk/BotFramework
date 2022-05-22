using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace BotFramework.Tests.Localization;

public class ExportTests
{
    private CsvTranslationExporter     _exporter;
    private Mock<ITranslationsService> _translationServiceMock;

    [SetUp]
    public void Setup()
    {
        _translationServiceMock = new Mock<ITranslationsService>();
        _translationServiceMock.Setup(a => a.ExportAsync())
                               .ReturnsAsync(() => new TranslationExport(new List<TransaltionItem>()
                               {
                                   new("en", "1", "en_1"),
                                   new("en", "2", "en_2"),
                                   new("uk", "2", "uk_2"),
                                   new("uk", "1", "uk_1"),
                                   new("fr", "2", "fr_2")
                               }));

        _exporter = new CsvTranslationExporter(_translationServiceMock.Object);
    }

    [Test]
    public async Task Test1()
    {
        await using var file = await _exporter.Export();
        await File.WriteAllBytesAsync("export.csv", file.ToArray());
    }
}

public class ImportTests
{
    private CsvTranslationImporter     _importer;
    private Mock<ITranslationsService> _translationServiceMock;

    [SetUp]
    public void Setup()
    {
        _translationServiceMock = new Mock<ITranslationsService>();
        _importer               = new CsvTranslationImporter(_translationServiceMock.Object);
    }

    [Test]
    public async Task Test1()
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        await writer.WriteAsync(
            "Key ;en   ;fr   ;uk\n"
            + "1 ;en_1 ;     ;uk_1\n"
            + "2 ;en_2 ;fr_2 ;uk_2\n");
        await writer.FlushAsync();
        stream.Position = 0;

        TranslationExport? export = null;

        _translationServiceMock.Setup(a => a.ImportAsync(It.IsAny<TranslationExport>()))
                               .Callback((TranslationExport e) => { export = e; });
        await _importer.Import(stream);

        Assert.IsNotNull(export);
        Assert.AreEqual(new List<string>() { "en", "fr", "uk" }, export.Value.Locales);
        Assert.AreEqual(new List<string>() { "1", "2" },         export.Value.Keys);
    }
}

public class TestTranslationContext : DbContext, ITranslationContext
{
    public DbSet<BotFrameworkTranslation> BotFrameworkTranslations { get; set; }
    public DbSet<BotFrameworkLocale>      BotFrameworkLocales      { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
    }

    public Task<int> SaveChangesAsync()
    {
        return base.SaveChangesAsync();
    }
}

public class TranlsationServiceTests
{
    private TranslationsService    _service;
    private TestTranslationContext _context;

    [SetUp]
    public async Task Setup()
    {
        _context = new();
        _service = new TranslationsService(_context, NullLogger<TranslationsService>.Instance);
        _context.BotFrameworkLocales.AddRange("en");
        await _context.SaveChangesAsync();
        TranslationsService.Container = new(new Dictionary<string, TranlsationLanguage>());
    }

    [Test]
    public async Task ExportTest()
    {
        var _ = TranslationsService.Container["en"]["key_1"];
        _ = TranslationsService.Container["uk"]["key_2"];
        var export = await _service.ExportAsync();

        //do not append new locales
        Assert.AreEqual(new List<string>() { "en" }, export.Locales);

        //do append new keys even if locale if missing
        Assert.AreEqual(new List<string>() { "key_1", "key_2" }, export.Keys);
    }

    [Test]
    public async Task ImportTest()
    {
        var export = new TranslationExport(new List<TransaltionItem>
        {
            new("en", "1", "en_1"),
            new("en", "2", "en_2"),
            new("uk", "2", "uk_2"),
            new("uk", "1", "uk_1"),
            new("fr", "2", "fr_2")
        });
        await _service.ImportAsync(export);

        Assert.AreEqual(new List<string> { "en", "fr", "uk" },
            TranslationsService.Container.Languages.Select(a => a.Key).OrderBy(a => a));
        Assert.AreEqual(new List<string> { "1", "2" },
            TranslationsService.Container.Languages["en"].Translations.Keys.OrderBy(a => a));
        Assert.AreEqual(new List<string> { "en_1", "en_2" },
            TranslationsService.Container.Languages["en"].Translations.Values.OrderBy(a => a));
    }
}