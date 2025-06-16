using Abp.Zero.EntityFrameworkCore;
using EMRSystem.Appointments;
using EMRSystem.Authorization.Roles;
using EMRSystem.Authorization.Users;
using EMRSystem.Billings;
using EMRSystem.Doctors;
using EMRSystem.LabReports;
using EMRSystem.MultiTenancy;
using EMRSystem.Nurses;
using EMRSystem.Patients;
using EMRSystem.Pharmacists;
using EMRSystem.Prescriptions;
using EMRSystem.Vitals;
using Microsoft.EntityFrameworkCore;

namespace EMRSystem.EntityFrameworkCore;

public class EMRSystemDbContext : AbpZeroDbContext<Tenant, Role, User, EMRSystemDbContext>
{
    public EMRSystemDbContext(DbContextOptions<EMRSystemDbContext> options)
      : base(options)
    { }
    public DbSet<Bill> Billing { get; set; }
    public DbSet<BillItem> BillItem { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<LabReport> Lab { get; set; }
    public DbSet<LabTechnician> LabTechnician { get; set; }
    public DbSet<Nurse> Nurses { get; set; }
    public DbSet<Pharmacist> Pharmacists { get; set; }
    public DbSet<Appointment> Appointments { get; set; }


    public DbSet<Patient> Patients { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Vital> Vitals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Bill>()
                .HasOne(s => s.AbpUser)
                .WithMany(e => e.Bills)
                .HasForeignKey(s => s.AbpUserId)
                .OnDelete(DeleteBehavior.NoAction); // or NoAction

        modelBuilder.Entity<BillItem>()
              .HasOne(s => s.Bill)
              .WithMany(e => e.Items)
              .HasForeignKey(s => s.BillId)
              .OnDelete(DeleteBehavior.NoAction); // or NoAction


        modelBuilder.Entity<Doctor>()
               .HasOne(s => s.AbpUser)
               .WithMany(e => e.Doctors)
               .HasForeignKey(s => s.AbpUserId)
               .OnDelete(DeleteBehavior.NoAction); // or NoAction

        modelBuilder.Entity<LabReport>()
                .HasOne(s => s.LabTechnicians)
                .WithMany(e => e.LabReports)
                .HasForeignKey(s => s.LabTechnicianId)
                .OnDelete(DeleteBehavior.NoAction); // or NoAction

        modelBuilder.Entity<LabTechnician>()
                .HasOne(s => s.AbpUser)
                .WithMany(e => e.LabTechnicians)
                .HasForeignKey(s => s.AbpUserId)
                .OnDelete(DeleteBehavior.NoAction); // or NoAction

        modelBuilder.Entity<Nurse>()
              .HasOne(s => s.AbpUser)
              .WithMany(e => e.Nurses)
              .HasForeignKey(s => s.AbpUserId)
              .OnDelete(DeleteBehavior.NoAction); // or NoAction

        modelBuilder.Entity<Pharmacist>()
              .HasOne(s => s.AbpUser)
              .WithMany(e => e.Pharmacists)
              .HasForeignKey(s => s.AbpUserId)
              .OnDelete(DeleteBehavior.NoAction); // or NoAction

        modelBuilder.Entity<Patient>()
                .HasOne(s => s.AbpUser)
                .WithMany(e => e.Patients)
                .HasForeignKey(s => s.AbpUserId)
                .OnDelete(DeleteBehavior.NoAction); // or NoAction

        modelBuilder.Entity<Patient>()
                .HasOne(s => s.Doctors)
                .WithMany(e => e.Patients)
                .HasForeignKey(s => s.AssignedDoctorId)
                .OnDelete(DeleteBehavior.NoAction); // or NoAction

        modelBuilder.Entity<Prescription>()
              .HasOne(s => s.Appointment)
              .WithMany(e => e.Prescriptions)
              .HasForeignKey(s => s.AppointmentId)
              .OnDelete(DeleteBehavior.NoAction); // or NoAction


        modelBuilder.Entity<Prescription>()
              .HasOne(s => s.Doctor)
              .WithMany(e => e.Prescriptions)
              .HasForeignKey(s => s.DoctorId)
              .OnDelete(DeleteBehavior.NoAction); // or NoAction

        modelBuilder.Entity<Prescription>()
            .HasOne(s => s.Patient)
            .WithMany(e => e.Prescriptions)
            .HasForeignKey(s => s.PatientId)
            .OnDelete(DeleteBehavior.NoAction); // or NoAction

        modelBuilder.Entity<Vital>()
           .HasOne(s => s.Patient)
           .WithMany(e => e.Vitals)
           .HasForeignKey(s => s.PatientId)
           .OnDelete(DeleteBehavior.NoAction); // or NoAction

        modelBuilder.Entity<Vital>()
          .HasOne(s => s.Nurse)
          .WithMany(e => e.Vitals)
          .HasForeignKey(s => s.NurseId)
          .OnDelete(DeleteBehavior.NoAction); // or NoAction

        modelBuilder.Entity<Appointment>()
    .HasOne(a => a.Patient)
    .WithMany(p => p.Appointments)
    .HasForeignKey(a => a.PatientId)
    .OnDelete(DeleteBehavior.NoAction); // OR Restrict

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.NoAction); // OR Restrict

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Nurse)
            .WithMany(n => n.Appointments)
            .HasForeignKey(a => a.NurseId)
            .OnDelete(DeleteBehavior.NoAction); // OR Restrict


    }
}
