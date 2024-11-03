Sometimes you need to have some resources localizable but not visible by default in administration user interface.
No additional permissions are applied for hidden resources - they are just hidden by default from the UI.
If you need hidden & **restricted resources** - please ping me, will see what we can do about that.

To register hidden resource - you just apply `[Hidden]` attribute to particular resource.

You can apply attribute to specific member (only that resource will be hidden from UI):

```csharp
[LocalizedModel]
public class SomeModelWithHiddenProperty
{
    [Hidden]
    public string SomeProperty { get; set; }
}

[LocalizedResource]
public enum SomeEnumWithHiddenResources
{
    None,
    [Hidden] Some,
    Another
}
```

Or you can actually apply to whole class as well (then all discovered resources from that type will be hidden):

```cshrap
[LocalizedModel]
[Hidden]
public class SomeModelWithHiddenPropertyOnClassLevel
{
    public string SomeProperty { get; set; }
}

[LocalizedResource]
[Hidden]
public enum SomeEnumWithAllHiddenResources
{
    None,
    Some,
    Another
}
```

Translation still works even for hidden resources.
Also, `[Hidden]` attribute can be applied to existing resources - metadata in the database will be updated to match the source code.
