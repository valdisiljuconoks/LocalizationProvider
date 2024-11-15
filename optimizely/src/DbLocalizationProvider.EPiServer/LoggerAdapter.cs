// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using EPiServer.Logging;
using ILogger = DbLocalizationProvider.Logging.ILogger;

namespace DbLocalizationProvider.EPiServer;

/// <inheritdoc />
public class LoggerAdapter : ILogger
{
    private readonly global::EPiServer.Logging.ILogger _logger = LogManager.GetLogger(typeof(LoggerAdapter));

    /// <inheritdoc />
    public void Debug(string message, params object?[] args)
    {
        _logger.Debug(message, args);
    }

    /// <inheritdoc />
    public void Info(string message, params object?[] args)
    {
        _logger.Information(message, args);
    }

    /// <inheritdoc />
    public void Error(string message, params object?[] args)
    {
        _logger.Error(message, args);
    }

    /// <inheritdoc />
    public void Error(string message, Exception exception, params object?[] args)
    {
        _logger.Error(message, exception, args);
    }
}
