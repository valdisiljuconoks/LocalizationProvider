using Xunit;

namespace DbLocalizationProvider.Tests;

public class MessageFormatingTests
{
    [Fact]
    public void FormatMessage_WithNamedPlaceholders_WithObject()
    {
        var message = "Hello, {FirstName}";
        var model = new Customer { FirstName = "John" };

        var result = LocalizationProvider.Format(message, model);

        Assert.Equal("Hello, John", result);
    }

    [Fact]
    public void FormatMessage_WithNamedPlaceholders_WithAnonymousObject()
    {
        var message = "Hello, {FirstName}";

        var result = LocalizationProvider.Format(message, new { FirstName = "John" });

        Assert.Equal("Hello, John", result);
    }

    [Fact]
    public void FormatMessage_WithNamedPlaceholders_PassingNothing()
    {
        var message = "Hello, {FirstName}";

        var result = LocalizationProvider.Format(message);

        Assert.Equal(message, result);
    }

    [Fact]
    public void FormatMessage_WithNamedPlaceholders_PassingNull()
    {
        var message = "Hello, {FirstName}";

        var result = LocalizationProvider.Format(message, null);

        Assert.Equal(message, result);
    }

    [Fact]
    public void FormatMessage_WithNamedPlaceholders_WithRicherAnonymousObject()
    {
        var message = "Hello, {FirstName}";

        var result = LocalizationProvider.Format(message, new { FirstName = "John", Surname = "Smith" });

        Assert.Equal("Hello, John", result);
    }

    [Fact]
    public void FormatMessage_WithNamedMixedOrderPlaceholders_WithAnonymousObject()
    {
        var message = "Hello, {Surname} {FirstName}";

        var result = LocalizationProvider.Format(message, new { FirstName = "John", Surname = "Smith" });

        Assert.Equal("Hello, Smith John", result);
    }

    [Fact]
    public void FormatMessage_WithNonExistingNamedPlaceholders_WithAnonymousObject()
    {
        var message = "Hello, {Surname} {FirstName}";

        var result = LocalizationProvider.Format(message, new { FirstName = "John" });

        Assert.Equal("Hello, {Surname} John", result);
    }

    [Fact]
    public void FormatMessage_WithIncorrectCasingPlaceholders_WithAnonymousObject()
    {
        var message = "Hello, {SurName} {FirstName}";

        var result = LocalizationProvider.Format(message, new { FirstName = "John", Surname = "Smith" });

        Assert.Equal("Hello, {SurName} John", result);
    }

    [Fact]
    public void FormatMessage_WithInvalidPlaceholder_WithAnonymousObject()
    {
        var message = "Hello, {Sur Name} {FirstName}";

        var result = LocalizationProvider.Format(message, new { FirstName = "John" });

        Assert.Equal("Hello, {Sur Name} John", result);
    }

    [Fact]
    public void FormatMessage_WithDuplicatePlaceholders_WithAnonymousObject_BothShouldBeReplaced()
    {
        var message = "Hello, {Prop1}, {Prop1} {Prop2}";

        var result = LocalizationProvider.Format(message, new { Prop1 = "James", Prop2 = "Bond" });

        Assert.Equal("Hello, James, James Bond", result);
    }

    [Fact]
    public void FormatMessage_WithIndexedPlaceholders()
    {
        var message = "Hello, {0}";

        var result = LocalizationProvider.Format(message, "John");

        Assert.Equal("Hello, John", result);
    }

    [Fact]
    public void FormatMessage_WithIndexedPlaceholders_AnonymousObject()
    {
        var message = "Hello, {0}";

        var result = LocalizationProvider.Format(message, new { FirstName = "John" });

        Assert.Equal("Hello, {0}", result);
    }

    private class Customer
    {
        public string FirstName { get; set; }
    }
}
