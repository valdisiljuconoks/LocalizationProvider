// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider
{
    /// <summary>
    /// </summary>
    public interface ICommandExecutor
    {
        /// <summary>
        /// </summary>
        /// <param name="command"></param>
        void Execute(ICommand command);

        /// <summary>
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        bool CanBeExecuted(ICommand command);
    }
}
