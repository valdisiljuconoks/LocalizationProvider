// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

namespace DbLocalizationProvider.AdminUI.Models;

/// <summary>
/// Options of the AdminUI
/// </summary>
public class UiOptions
{
    /// <summary>
    /// Is current user powerful enough?
    /// </summary>
    public bool AdminMode { get; set; }

    /// <summary>
    /// Do we need to include also language that nobody speaks or writes in?
    /// </summary>
    public bool ShowInvariantCulture { get; set; }

    /// <summary>
    /// If there are some hidden treasure - that would shown also.
    /// </summary>
    public bool ShowHiddenResources { get; set; }

    /// <summary>
    /// Show only empty resources - somebody will have some night shifts to fill them up.
    /// </summary>
    public bool ShowOnlyEmptyResources { get; set; }

    /// <summary>
    /// Should we also allow to TNT any of resources?
    /// </summary>
    public bool ShowDeleteButton { get; set; }
}
