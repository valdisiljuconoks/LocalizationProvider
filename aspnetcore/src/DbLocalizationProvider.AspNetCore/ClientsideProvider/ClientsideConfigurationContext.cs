// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace DbLocalizationProvider.AspNetCore.ClientsideProvider;

internal class ClientsideConfigurationContext
{
    public static string DeepMergeScriptName = "deep-merge.js";

    public static string RootPath { get; private set; }

    public static string DefaultAlias { get; private set; }

    internal static void SetRootPath(string path)
    {
        DefaultAlias = path.StartsWith("/", StringComparison.OrdinalIgnoreCase) ? path.Substring(1, path.Length - 1) : path;
        RootPath = path.StartsWith("/", StringComparison.OrdinalIgnoreCase) ? path : '/' + path;
    }
}
