﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations
{
    public class DropSchema:MigrationOperation
    {
        public string Schema { get; set; }
    }
}