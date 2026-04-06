using DbLocalizationProvider.Optimizely.Sandbox.Models.Blocks;

namespace DbLocalizationProvider.Optimizely.Sandbox.Models.ViewModels;

public class PageListModel(PageListBlock block)
{
    public string Heading { get; set; } = block.Heading;

    public IEnumerable<PageData> Pages { get; set; }

    public bool ShowIntroduction { get; set; } = block.IncludeIntroduction;

    public bool ShowPublishDate { get; set; } = block.IncludePublishDate;
}
