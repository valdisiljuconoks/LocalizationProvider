// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

namespace DbLocalizationProvider.Import
{
    /// <summary>
    /// Type of the change detected while importing resources
    /// </summary>
    public enum ChangeType
    {
        /// <summary>
        /// No one knows when this could be detected
        /// </summary>
        None,

        /// <summary>
        /// Resource is going to be added
        /// </summary>
        Insert,

        /// <summary>
        /// Updating the resource
        /// </summary>
        Update,

        /// <summary>
        /// Astalavista baby!
        /// </summary>
        Delete
    }
}
