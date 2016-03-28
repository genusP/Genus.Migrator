﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations
{
    public sealed class DropIndex:TableOperation
    {
        public string IndexName { get; set; }

        public override int OrderHint
            => 1000;
    }
}
