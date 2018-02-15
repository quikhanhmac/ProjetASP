using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoList.Models
{
    public class TodoListContext : DbContext
    {
        public DbSet<Tache> Taches { get; set; }

        public TodoListContext(DbContextOptions<TodoListContext> options) :
            base(options)
        {
        }
    }
}
