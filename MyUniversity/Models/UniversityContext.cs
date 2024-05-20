using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUniversity.Models
{
    /// <summary>
    /// Класс представляет контекст базы данных университета.
    /// </summary>
    public class UniversityContext : DbContext
    {
        /// <summary>
        /// Получает или задает набор данных студентов.
        /// </summary>
        public DbSet<Student> Students { get; set; } = null!;

        /// <summary>
        /// Получает или задает набор данных предметов.
        /// </summary>
        public DbSet<Subject> Subjects { get; set; } = null!;


        /// <summary>
        /// Конфигурирует параметры подключения к базе данных.
        /// </summary>
        /// <param name="optionsBuilder">Построитель параметров конфигурации.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=University.db");
        }

        /// <summary>
        /// Конфигурирует модель данных и устанавливает связи между сущностями.
        /// </summary>
        /// <param name="modelBuilder">Построитель модели данных.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subject>()
                .HasOne(s => s.Student)
                .WithMany(st => st.Subjects)
                .HasForeignKey(s => s.StudentId);
        }
    }
}
