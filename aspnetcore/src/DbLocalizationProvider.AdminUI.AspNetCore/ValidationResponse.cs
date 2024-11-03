// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Threading.Tasks;
using DbLocalizationProvider.Import;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DbLocalizationProvider.AdminUI.AspNetCore;

public class ValidationResponse : IActionResult
{
    private static readonly JsonSerializerSettings _jsonSerializerSettings =
        new() { ContractResolver = new CamelCasePropertyNamesContractResolver() };

    private readonly ICollection<DetectedImportChange> _response;

    public ValidationResponse(ICollection<DetectedImportChange> response)
    {
        _response = response;
    }

    public Task ExecuteResultAsync(ActionContext context)
    {
        return context.HttpContext.Response.WriteAsync(
            JsonConvert.SerializeObject(
                _response,
                _jsonSerializerSettings));
    }
}
