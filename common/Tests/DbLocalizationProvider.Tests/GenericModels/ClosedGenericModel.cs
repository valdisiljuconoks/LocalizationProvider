using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.GenericModels;

[LocalizedModel]
public class ClosedGenericModel : OpenGenericModel<SampleImpl> { }
