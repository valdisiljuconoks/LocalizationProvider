using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DbLocalizationProvider.AdminUI.AspNetCore;

/// <inheritdoc />
public class NewtonJsonModelBinder : IModelBinder
{
    /// <inheritdoc />
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        using (var reader = new StreamReader(bindingContext.HttpContext.Request.Body))
        {
            var body = await reader.ReadToEndAsync().ConfigureAwait(false);

            // Do something
            var value = JsonConvert.DeserializeObject(body,
                                                      bindingContext.ModelType,
                                                      new JsonSerializerSettings
                                                      {
                                                          NullValueHandling = NullValueHandling.Ignore,
                                                          ContractResolver = new CamelCasePropertyNamesContractResolver(),
                                                          ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                                                          PreserveReferencesHandling = PreserveReferencesHandling.Objects
                                                      });

            bindingContext.Result = ModelBindingResult.Success(value);
        }
    }
}
