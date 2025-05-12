using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AppointmentSystem.Shared.Models;

namespace AppointmentSystem.API.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            // Apply any pending migrations
            await context.Database.MigrateAsync();

            // Check if we already have data
            if (context.Doctors.Any() || context.Patients.Any())
                return;

            // Create roles
            var roles = new[] { "Admin", "Doctor", "Patient" };
            foreach (var role in roles)
            {
                if (!await context.Roles.AnyAsync(r => r.Name == role))
                {
                    await context.Roles.AddAsync(new IdentityRole { Name = role, NormalizedName = role.ToUpper() });
                }
            }
            await context.SaveChangesAsync();

            // Create admin user
            var adminEmail = "admin@appointmentsystem.com";
            var adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Create sample doctors
            var doctors = new[]
            {
                new Doctor
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Specialty = "Cardiology",
                    PhoneNumber = "1234567890",
                    Email = "john.doe@example.com",
                    Address = "123 Medical Center Dr",
                    VisitPrice = 100,
                    WorkHours = "{\"Monday\":{\"Start\":\"09:00\",\"End\":\"17:00\"},\"Tuesday\":{\"Start\":\"09:00\",\"End\":\"17:00\"},\"Wednesday\":{\"Start\":\"09:00\",\"End\":\"17:00\"},\"Thursday\":{\"Start\":\"09:00\",\"End\":\"17:00\"},\"Friday\":{\"Start\":\"09:00\",\"End\":\"17:00\"}}",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new Doctor
                {
                    FirstName = "Jane",
                    LastName = "Smith",
                    Specialty = "Pediatrics",
                    PhoneNumber = "0987654321",
                    Email = "jane.smith@example.com",
                    Address = "456 Children's Hospital Ave",
                    VisitPrice = 80,
                    WorkHours = "{\"Monday\":{\"Start\":\"10:00\",\"End\":\"18:00\"},\"Tuesday\":{\"Start\":\"10:00\",\"End\":\"18:00\"},\"Wednesday\":{\"Start\":\"10:00\",\"End\":\"18:00\"},\"Thursday\":{\"Start\":\"10:00\",\"End\":\"18:00\"},\"Friday\":{\"Start\":\"10:00\",\"End\":\"18:00\"}}",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            };

            foreach (var doctor in doctors)
            {
                var user = new IdentityUser
                {
                    UserName = doctor.Email,
                    Email = doctor.Email,
                    EmailConfirmed = true
                };

                if (await userManager.FindByEmailAsync(doctor.Email) == null)
                {
                    var result = await userManager.CreateAsync(user, "Doctor123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Doctor");
                        doctor.UserId = user.Id;
                        await context.Doctors.AddAsync(doctor);
                    }
                }
            }

            await context.SaveChangesAsync();

            // Create sample patients
            var patients = new[]
            {
                new Patient
                {
                    FirstName = "Alice",
                    LastName = "Johnson",
                    PhoneNumber = "5551234567",
                    Email = "alice.johnson@example.com",
                    DateOfBirth = new DateTime(1985, 5, 15),
                    Address = "789 Patient St",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new Patient
                {
                    FirstName = "Bob",
                    LastName = "Wilson",
                    PhoneNumber = "5559876543",
                    Email = "bob.wilson@example.com",
                    DateOfBirth = new DateTime(1990, 8, 22),
                    Address = "321 Health Ave",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            };

            foreach (var patient in patients)
            {
                var user = new IdentityUser
                {
                    UserName = patient.Email,
                    Email = patient.Email,
                    EmailConfirmed = true
                };

                if (await userManager.FindByEmailAsync(patient.Email) == null)
                {
                    var result = await userManager.CreateAsync(user, "Patient123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Patient");
                        patient.UserId = user.Id;
                        await context.Patients.AddAsync(patient);
                    }
                }
            }

            await context.SaveChangesAsync();
        }
    }
} 