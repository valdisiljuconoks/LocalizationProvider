// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Threading.Tasks;
using DbLocalizationProvider.AdminUI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DbLocalizationProvider.AdminUI.AspNetCore;

public class ApiResponse(BaseApiModel response) : IActionResult
{
    private static readonly JsonSerializerSettings _jsonSerializerSettings =
        new() { ContractResolver = new CamelCasePropertyNamesContractResolver() };

    public Task ExecuteResultAsync(ActionContext context)
    {
        context.HttpContext.Response.ContentType = "application/json";

        return context.HttpContext.Response.WriteAsync(
            JsonConvert.SerializeObject(
                response,
                _jsonSerializerSettings));
    }
}
