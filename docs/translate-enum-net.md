# Translate System.Enum

It's quite often that you do have a enumeration in the domain to ensure that your entity might have value only from predefined list of values - like `Document.Status` or `PurchaseOrder.Shipment.Status`. These values are usually defined as `System.Enum`. And it's also quite often case when you need to render a list of these available values on the page or anywhere else for the user of the application to choose from. Now in 2.0 enumeration translation is easily available.

```csharp
[LocalizedResource]
public enum DocumentStatus
{
    None,
    New,
    Pending,
    Active,
    Closed
}

[LocalizedModel]
public class Document
{
    ...

    public DocumentStatus Status { get; }
}
```

Now you can use following snippet to give end-user localized dropdown list of available document statuses:

```
@using DbLocalizationProvider
@model Document

@{
    var statuses = Enum.GetValues(typeof(DocumentStatus))
                       .Cast<DocumentStatus>()
                       .Select(s => new SelectListItem
                                        {
                                            Value = s.ToString(),
                                            Text = s.Translate()
                                        });
}

@Html.DropDownListFor(m => m.Status, statuses)
```

Or if you just need to output current status of the document to the end-user:

```
@using DbLocalizationProvider
@model Document

@Model.Status.Translate()
```

## Specify Translation for Enum

By default name of the `Enum` member is taken as translation of the localizable resource. If you need to specify default translation for the resource, you can use `[Display]` attribute from `DataAnnotations` namespace:


```csharp
[LocalizedResource]
public enum DocumentStatus
{
    None,
    New,
    [Display(Name = "Pending document...")] Pending,
    Active,
    Closed
}
```

## Resource Keys for Enum

Sometimes (usage for me was EPiServer visitor groups) you just need to control naming for each individual `Enum` item. Now you can do that with `[ResourceKey]` attribute. Hope code explains how it might look:

```csharp
[LocalizedResource(KeyPrefix = "/this/is/prefix/")]
public enum ThisIsMyStatus
{
    [ResourceKey("nothing")]
    None,

    [ResourceKey("something")]
    Some,

    [ResourceKey("anything")]
    Any
}
```

For instance you have following visitor group criteria:

```csharp

namespace My.Project.Namespace
{

[VisitorGroupCriterion(..,
    LanguagePath = "/visitorgroupscriterias/usernamecriterion")]
public class UsernameCriterion : CriterionBase<UsernameCriterionModel>
{
    ...
}

public class UsernameCriterionModel : CriteriaPackModelBase
{
    [Required]
    [DojoWidget(SelectionFactoryType = typeof(EnumSelectionFactory))]
    public UsernameValueCondition Condition { get; set; }

    public string Value { get; set; }
}

public enum UsernameValueCondition
{
    Matches,
    StartsWith,
    EndsWith,
    Contains
}
```

So if anybody will look after resources with following key:

```xml
/enums/my/project/namespace/usernamecriterion/usernamevaluecondition
```

and then name of the `Enum`. So you can control this and decorate each enum member with `ResourceKey` attribute to generate specific keys if needed.

```csharp
namespace My.Project.Namespace
{
    [LocalizedResource(KeyPrefix = "/enums/my/project/namespace/usernamecriterion/usernamevaluecondition")]
    public enum UsernameValueCondition
    {
        [ResourceKey("matches")]
        Matches,
        [ResourceKey("startswith")]
        StartsWith,
        [ResourceKey("endswith")]
        EndsWith,
        [ResourceKey("contains")]
        Contains
    }
}
```

Maybe it's worth just to create new attribute - like `[EnumResource]` or something - namespace and member resource key calculations would be done for me?! I know - I'm lazy.. Sorry..

## Tanslating Enums from different assemblies

Sometimes you need to add translations for `System.Enum` types from different assemblies where you don't own or have source code. For this case you can use [foreign resources](foreign-resources.md) concept.
