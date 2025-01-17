﻿using EmailProvider.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailProvider.Contexts;
public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<FormDataEntity> FormDatas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FormDataEntity>().ToContainer("FormDatas").HasPartitionKey(x => x.PartitionKey);
    }
}
