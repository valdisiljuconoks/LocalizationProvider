// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Sync;

namespace DbLocalizationProvider.Storage.MongoDb;

public class SchemaUpdater : ICommandHandler<UpdateSchema.Command>
{
    public void Execute(UpdateSchema.Command command)
    {
    }
}
