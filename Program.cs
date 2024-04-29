using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace WCS_LINQ_Entity_Framework
{
    public class AnimalContext : DbContext
    {
        public DbSet<Animal> Animals { get; set; }
        public DbSet<Species> Species { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Utiliser une base de données en mémoire pour cet exemple
            optionsBuilder.UseInMemoryDatabase("AnimalDatabase");
        }
    }

    public class Animal
    {
        public Guid AnimalId { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Guid SpeciesId { get; set; }
        public Species Species { get; set; }
    }

    public class Species
    {
        public Guid SpeciesId { get; set; }
        public string Name { get; set; }
    }

    public class Program
    {
        public static void Main()
        {
            using (var context = new AnimalContext())
            {
                // Créer et ajouter les espèces
                var whiteCougar = new Species { SpeciesId = Guid.NewGuid(), Name = "Cougars blancs" };
                var whiteTiger = new Species { SpeciesId = Guid.NewGuid(), Name = "Tigres blancs" };
                var albinoTurtle = new Species { SpeciesId = Guid.NewGuid(), Name = "Tortues albinos" };

                context.Species.AddRange(whiteCougar, whiteTiger, albinoTurtle);
                context.SaveChanges();

                // Nombre d'animaux à créer pour chaque espèce
                var animalsToCreate = new Dictionary<Species, int>
                {
                    { whiteCougar, 3 }, // 3 cougars blancs
                    { whiteTiger, 100 }, // 100 tigres blancs
                    { albinoTurtle, 15 } // 15 tortues albinos
                };

                // Créer les animaux pour chaque espèce
                foreach (var kvp in animalsToCreate)
                {
                    var species = kvp.Key;
                    var count = kvp.Value;

                    for (int i = 1; i <= count; i++)
                    {
                        context.Animals.Add(new Animal
                        {
                            AnimalId = Guid.NewGuid(),
                            Name = $"{species.Name} {i}",
                            DateOfBirth = DateTime.Now,
                            SpeciesId = species.SpeciesId
                        });
                    }
                }

                context.SaveChanges();

                // Interroger la base de données pour obtenir le nombre d'animaux par espèce
                var animalCounts = context.Animals
                    .GroupBy(a => a.Species.Name)
                    .Select(g => new { Species = g.Key, Count = g.Count() });

                // Afficher les résultats
                foreach (var count in animalCounts)
                {
                    Console.WriteLine($"Les {count.Species} - Il en reste {count.Count}.");
                }
            }
        }
    }
}

