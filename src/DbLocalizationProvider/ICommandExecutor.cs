// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider
{
    /// <summary>
    /// The executor
    /// </summary>
    public interface ICommandExecutor
    {
        /// <summary>
        /// </summary>
        /// <param name="command"></param>
        Task Execute(ICommand command);

        /// <summary>
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        bool CanBeExecuted(ICommand command);
    }
}
