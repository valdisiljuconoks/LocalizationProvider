// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using Microsoft.Extensions.Logging;
using ILogger = DbLocalizationProvider.Logging.ILogger;

namespace DbLocalizationProvider.AspNetCore;

/// <summary>
/// 
/// </summary>
public class LoggerAdapter : ILogger
{
    private readonly Microsoft.Extensions.Logging.ILogger _logger;

    /// <summary>
    /// Initializes new instance of the logger adapter.
    /// </summary>
    /// <param name="logger">Original logger to adapt to</param>
    public LoggerAdapter(Microsoft.Extensions.Logging.ILogger logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public void Debug(string message, params object?[] args)
    {
        _logger?.LogDebug(message, args);
    }

    /// <inheritdoc />
    public void Info(string message, params object?[] args)
    {
        _logger?.LogInformation(message, args);
    }

    /// <inheritdoc />
    public void Error(string message, params object?[] args)
    {
        _logger?.LogError(message, args);
    }

    /// <inheritdoc />
    public void Error(string message, Exception exception, params object?[] args)
    {
        _logger?.LogError(message, exception, args);
    }
}
