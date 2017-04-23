
Sometimes in your model you need to reference resource from the other - usually common shared resources.

Meaning that there are models with properties that are common for the project (for instance - `Yes`, `No`, `First name`, etc.). If you will just annotate view model with `[LocalizedModel]` and will have multiple models with property `FirstName` - by default all properties from all models will become localizable resources.
This might pollute your resource list with unnecessary items you would want to avoid.

Now you can "reference" different resource and use that instead of annotated resource.

For example, you have some common resources:

```csharp
namespace DbLocalizationProvider.Sample
{
    [LocalizedResource]
    public class CommonResources
    {
        public static string CommonProp => "Common Value";
    }
}
```

Using new attribute you can decorate your model's property with `[UseResource]` attribute (you can also just reference resource using string - `"CommonProp"`, I prefer `nameof`):


```csharp
namespace DbLocalizationProvider.Sample
{
    [LocalizedModel]
    public class ModelWithOtherResourceUsage
    {
        [UseResource(typeof(CommonResources),
                     nameof(CommonResources.CommonProp))]
        public string SomeProperty { get; set; }
    }
}
```

So, now if you will try to localize `SomeProperty` for this view model - common resource will be returned:

```csharp
@model ModelWithOtherResourceUsage

...
@Html.TranslateFor(m => m.SomeProperty)
```

Instead of `ModelWithOtherResourceUsage.SomeProperty` resource, `CommonResources.CommonProp` key will be used.
There will be no resource for key `ModelWithOtherResourceUsage.SomeProperty` registered in database.
Whenever you will change translation for `CommonResources.CommonProp` resource - it will be used everywhere you referenced it using `[UseResource]` attribute.
