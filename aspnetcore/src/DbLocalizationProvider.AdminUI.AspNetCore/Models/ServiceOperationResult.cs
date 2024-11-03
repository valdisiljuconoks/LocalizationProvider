// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Microsoft.AspNetCore.Mvc;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Models;

public class ServiceOperationResult
{
    private ServiceOperationResult(string status)
    {
        Status = status;
    }

    public string Status { get; set; }

    public static JsonResult Ok => new(new ServiceOperationResult("Ok"));
}
