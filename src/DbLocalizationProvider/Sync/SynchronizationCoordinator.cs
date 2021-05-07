// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace DbLocalizationProvider.Sync
{
    /// <summary>
    ///     Synchronization coordinator (used to control sync process in clustered environments).
    /// </summary>
    public class SynchronizationCoordinator
    {
        /// <summary>
        /// Gets or sets action callback just before synchronization process is about to begin.
        /// </summary>
        /// <value>
        /// The callback.
        /// </value>
        /// <remarks>Use this callback to "block" sync process on cluster node(-s) where you want to skip synchronization. Underlying implementation usually should involve some reliable distributed lock mechanism (Redis, Sql, whatever).</remarks>
        public Action BeforeSyncCallback { get; set; }

        /// <summary>
        /// Occurs when synchronization process is completed.
        /// </summary>
        public event EmptyEventHandler OnSyncCompleted;

        /// <summary>
        /// Used to signal that synchronization process is completed.
        /// </summary>
        internal void SyncCompleted()
        {
            OnSyncCompleted?.Invoke();
        }
    }
}
