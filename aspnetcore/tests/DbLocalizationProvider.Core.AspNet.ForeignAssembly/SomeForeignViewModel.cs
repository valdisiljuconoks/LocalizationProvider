using System.ComponentModel.DataAnnotations;

namespace DbLocalizationProvider.Core.AspNet.ForeignAssembly;

public class SomeForeignViewModel
{
    [Display(Name = "Property 1")]
    public string Property1 { get; set; } = "Property 1";
}
