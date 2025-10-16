//Class used in configuration of EF Core

using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using ToDo.Models.Models;

namespace ToDo.DataAccess.Data
{
    public class AppDataContext : DbContext
    {
        public AppDataContext(DbContextOptions<AppDataContext> options) : base(options)
        {
        }

        public DbSet<TDTask> TDTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            //Data Seed For testing purposes
            DateTime date = new DateTime(2025,10, 16,0,0,0,DateTimeKind.Utc);
            modelBuilder.Entity<TDTask>().HasData(

            new TDTask
            {
                Id = 1,
                Title = "Task1",
                Description = "Description1",
                CompletionPercentage = 30,
                ExpirationDate = date
            },
            new TDTask
            {
                Id = 2,
                Title = "Task2",
                Description = "Description2",
                CompletionPercentage = 40,
                ExpirationDate = date
            },
            new TDTask
            {
                Id = 3,
                Title = "Task3",
                Description = "Description3",
                CompletionPercentage = 50,
                ExpirationDate = date.AddDays(1)
            },
            new TDTask
            {
                Id = 4,
                Title = "Task4",
                Description = "Description4",
                CompletionPercentage = 60,
                ExpirationDate = date.AddDays(1)
            },
            new TDTask
            {
                Id = 5,
                Title = "Task5",
                Description = "Description5",
                CompletionPercentage = 70,
                ExpirationDate = date.AddDays(3)
            },
            new TDTask
            {
                Id = 6,
                Title = "Task6",
                Description = "Description6",
                CompletionPercentage = 80,
                ExpirationDate = date.AddDays(3)
            }
                );
        }
        
    }
}
