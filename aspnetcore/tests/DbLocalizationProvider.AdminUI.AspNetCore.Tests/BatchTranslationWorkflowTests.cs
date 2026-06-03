#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.AdminUI.AspNetCore.Translation;
using DbLocalizationProvider.AdminUI.Models;
using DbLocalizationProvider.Commands;
using Xunit;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Tests;

public class BatchTranslationWorkflowTests
{
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;
    private static readonly CultureInfo German = new("de");
    private static readonly CultureInfo English = new("en");

    [Fact]
    public async Task Preview_TranslatesInvariantSource_AndExecutesNoCommands()
    {
        var commands = new RecordingCommandExecutor();
        var workflow = new BatchTranslationWorkflow(
            QueryExecutorFor(Resource("key1", ("", "Hello")), Resource("key2", ("", "World"))),
            commands);

        var results = await workflow.PreviewAsync(new FakeTranslator(), ["key1", "key2"], Invariant, German, onlyEmpty: false);

        Assert.Equal(2, results.Count);
        Assert.Equal("key1", results[0].Key);
        Assert.Equal("Hello", results[0].SourceText);
        Assert.Equal("de:Hello", results[0].Translation);
        Assert.True(results[0].Success);
        Assert.Empty(commands.Commands);
    }

    [Fact]
    public async Task Preview_OnlyEmpty_SkipsResourcesThatAlreadyHaveTargetTranslation()
    {
        var workflow = new BatchTranslationWorkflow(
            QueryExecutorFor(
                Resource("key1", ("", "Hello")),
                Resource("key2", ("", "World"), ("de", "Welt"))),
            new RecordingCommandExecutor());

        var results = await workflow.PreviewAsync(new FakeTranslator(), ["key1", "key2"], Invariant, German, onlyEmpty: true);

        Assert.Single(results);
        Assert.Equal("key1", results[0].Key);
    }

    [Fact]
    public async Task Preview_SkipsEmptyOrWhitespaceSourceText()
    {
        var workflow = new BatchTranslationWorkflow(
            QueryExecutorFor(
                Resource("empty", ("", "")),
                Resource("whitespace", ("", "   ")),
                Resource("ok", ("", "Hi"))),
            new RecordingCommandExecutor());

        var results = await workflow.PreviewAsync(new FakeTranslator(), ["empty", "whitespace", "ok"], Invariant, German, onlyEmpty: false);

        Assert.Single(results);
        Assert.Equal("ok", results[0].Key);
    }

    [Fact]
    public async Task Preview_NonInvariantSource_UsesThatLanguageValue()
    {
        var workflow = new BatchTranslationWorkflow(
            QueryExecutorFor(Resource("key1", ("", "Hello"), ("en", "Hello-en"))),
            new RecordingCommandExecutor());

        var results = await workflow.PreviewAsync(new FakeTranslator(), ["key1"], English, German, onlyEmpty: false);

        Assert.Single(results);
        Assert.Equal("Hello-en", results[0].SourceText);
        Assert.Equal("de:Hello-en", results[0].Translation);
    }

    [Fact]
    public async Task Preview_SourceEqualsTarget_ReturnsEmptyAndDoesNotCallTranslator()
    {
        var translator = new FakeTranslator();
        var workflow = new BatchTranslationWorkflow(
            QueryExecutorFor(Resource("key1", ("de", "Hallo"))),
            new RecordingCommandExecutor());

        var results = await workflow.PreviewAsync(translator, ["key1"], German, German, onlyEmpty: false);

        Assert.Empty(results);
        Assert.Empty(translator.BatchCalls);
    }

    [Fact]
    public async Task Preview_FailedTranslation_IsReportedWithError()
    {
        var workflow = new BatchTranslationWorkflow(
            QueryExecutorFor(Resource("key1", ("", "Hello"))),
            new RecordingCommandExecutor());

        var results = await workflow.PreviewAsync(
            new FakeTranslator(_ => TranslationResult.Failed("boom")), ["key1"], Invariant, German, onlyEmpty: false);

        Assert.Single(results);
        Assert.False(results[0].Success);
        Assert.Null(results[0].Translation);
        Assert.Equal("boom", results[0].Error);
    }

    [Fact]
    public void Apply_IssuesOneCreateOrUpdateCommandPerItem()
    {
        var commands = new RecordingCommandExecutor();
        var workflow = new BatchTranslationWorkflow(QueryExecutorFor(), commands);

        workflow.Apply(German, new[]
        {
            new BatchTranslateApplyItem { Key = "key1", Translation = "Welt" },
            new BatchTranslateApplyItem { Key = "key2", Translation = "Hallo" }
        });

        Assert.Equal(2, commands.Commands.Count);
        Assert.All(commands.Commands, c => Assert.Equal("de", c.Language.Name));
        Assert.Equal("key1", commands.Commands[0].Key);
        Assert.Equal("Welt", commands.Commands[0].Translation);
        Assert.Equal("key2", commands.Commands[1].Key);
        Assert.Equal("Hallo", commands.Commands[1].Translation);
    }

    private static LocalizationResource Resource(string key, params (string Language, string Value)[] translations)
    {
        var resource = new LocalizationResource(key, false)
        {
            Translations = new LocalizationResourceTranslationCollection(false)
        };

        foreach (var (language, value) in translations)
        {
            resource.Translations.Add(new LocalizationResourceTranslation { Language = language, Value = value });
        }

        return resource;
    }

    private static FakeQueryExecutor QueryExecutorFor(params LocalizationResource[] resources) =>
        new(resources.ToDictionary(r => r.ResourceKey, r => r, StringComparer.Ordinal));

    private sealed class FakeQueryExecutor(Dictionary<string, LocalizationResource> resources) : IQueryExecutor
    {
        public TResult? Execute<TResult>(IQuery<TResult?> query) => (TResult?)(object)resources;
    }

    private sealed class RecordingCommandExecutor : ICommandExecutor
    {
        public List<CreateOrUpdateTranslation.Command> Commands { get; } = [];

        public void Execute(ICommand command)
        {
            if (command is CreateOrUpdateTranslation.Command translationCommand)
            {
                Commands.Add(translationCommand);
            }
        }

        public bool CanBeExecuted(ICommand command) => true;
    }

    private sealed class FakeTranslator(Func<string, TranslationResult>? map = null) : ITranslatorService
    {
        private readonly Func<string, TranslationResult> _map = map ?? (s => TranslationResult.Ok("de:" + s));

        public List<IReadOnlyList<string>> BatchCalls { get; } = [];

        public Task<TranslationResult> TranslateAsync(string inputText, CultureInfo targetLanguage, CultureInfo sourceLanguage) =>
            Task.FromResult(_map(inputText));

        public Task<IReadOnlyList<TranslationResult>> TranslateAsync(
            IReadOnlyList<string> inputTexts,
            CultureInfo targetLanguage,
            CultureInfo sourceLanguage)
        {
            BatchCalls.Add(inputTexts);
            IReadOnlyList<TranslationResult> results = inputTexts.Select(_map).ToList();
            return Task.FromResult(results);
        }
    }
}
