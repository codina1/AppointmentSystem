using Microsoft.EntityFrameworkCore;
using AppointmentSystem.Shared.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace AppointmentSystem.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<WorkSchedule> WorkSchedules { get; set; }
        public DbSet<SMSNotification> SMSNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Doctor
            builder.Entity<Doctor>()
                .HasIndex(d => d.Email)
                .IsUnique();

            builder.Entity<Doctor>()
                .HasIndex(d => d.PhoneNumber)
                .IsUnique();

            // Configure Patient
            builder.Entity<Patient>()
                .HasIndex(p => p.Email)
                .IsUnique();

            builder.Entity<Patient>()
                .HasIndex(p => p.PhoneNumber)
                .IsUnique();

            // Configure Appointment
            builder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure WorkSchedule
            builder.Entity<WorkSchedule>()
                .HasOne(w => w.Doctor)
                .WithMany()
                .HasForeignKey(w => w.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure SMSNotification
            builder.Entity<SMSNotification>()
                .HasOne(s => s.Appointment)
                .WithMany()
                .HasForeignKey(s => s.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
} 