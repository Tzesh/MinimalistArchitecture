﻿using System;
using Microsoft.EntityFrameworkCore;

namespace minimalistArchitecture.Todo
{
    public class TodoDb : DbContext
    {
        public TodoDb(DbContextOptions<TodoDb> options) : base(options) { }
        public DbSet<Todo> Todos => Set<Todo>();
    }
}
