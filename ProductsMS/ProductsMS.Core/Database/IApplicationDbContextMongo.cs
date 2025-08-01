﻿using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserMs.Core.Database
{
    public interface IApplicationDbContextMongo
    {
        IMongoDatabase Database { get; }
        IClientSessionHandle BeginTransaction();
    }
}

