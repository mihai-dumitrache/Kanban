﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kanban.Models
{
    public class MyContext : DbContext
    {
        public DbSet<Board> Board { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Task> Tasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=. \\SQLEXPRESS;Initial Catalog=Kanban;Integrated Security=True");
        }
    }
}