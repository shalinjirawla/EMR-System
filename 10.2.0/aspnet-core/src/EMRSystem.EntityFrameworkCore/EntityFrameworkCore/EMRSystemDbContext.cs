using Abp.Zero.EntityFrameworkCore;
using EMRSystem.Appointments;
using EMRSystem.Authorization.Roles;
using EMRSystem.Authorization.Users;
using EMRSystem.Billings;
using EMRSystem.Doctors;
using EMRSystem.Invoices;
using EMRSystem.LabReports;
using EMRSystem.LabReportsTypes;
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
    public DbSet<LabReportResultItem> LabReportResultItems { get; set; }
    public DbSet<LabTechnician> LabTechnician { get; set; }
    public DbSet<Nurse> Nurses { get; set; }
    public DbSet<Pharmacist> Pharmacists { get; set; }
    public DbSet<PharmacistInventory> PharmacistInventory { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<LabReportsType> LabReportsTypes { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Vital> Vitals { get; set; }
    //public DbSet<PrescriptionItem> PrescriptionItems { get; set; }
    public DbSet<PrescriptionLabTest> PrescriptionLabTests { get; set; }
    public DbSet<Invoice> Invoices{ get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }


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

        modelBuilder.Entity<Patient>()
               .HasOne(s => s.Nurses)
               .WithMany(e => e.Patients)
               .HasForeignKey(s =>s.AssignedNurseId )
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

        modelBuilder.Entity<PrescriptionLabTest>().ToTable("PrescriptionLabTests");

        // Prescription relationship
        modelBuilder.Entity<PrescriptionLabTest>()
            .HasOne(plt => plt.Prescription)
            .WithMany(p => p.LabTests) // Now this will work
            .HasForeignKey(plt => plt.PrescriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        // LabReportsType relationship  
        modelBuilder.Entity<PrescriptionLabTest>()
            .HasOne(plt => plt.LabReportsType)
            .WithMany(lrt => lrt.PrescriptionLabTests) // Optional: Add if you want reverse navigation
            .HasForeignKey(plt => plt.LabReportsTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LabReportResultItem>()
           .HasOne(plt => plt.PrescriptionLabTest)
           .WithMany(lrt => lrt.LabReportResultItems) // Optional: Add if you want reverse navigation
           .HasForeignKey(plt => plt.PrescriptionLabTestId)
           .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Invoice>(b =>
        {
            b.ToTable("Invoices");

            // Decimal precision
            b.Property(x => x.SubTotal).HasColumnType("decimal(18,2)");
            b.Property(x => x.GstAmount).HasColumnType("decimal(18,2)");
            b.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");

            // Enum conversions
            b.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            b.Property(e => e.PaymentMethod)
                .HasConversion<string?>()
                .HasMaxLength(30);

            // Relationships
            b.HasOne(i => i.Patient)
                .WithMany()
                .HasForeignKey(i => i.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(i => i.Appointment)
                .WithMany()
                .HasForeignKey(i => i.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InvoiceItem>(b =>
        {
            b.ToTable("InvoiceItems");

            // Configure decimal precision
            b.Property(x => x.UnitPrice)
                .HasColumnType("decimal(18,2)");

            // Configure enum as string
            b.Property(e => e.ItemType)
                .HasConversion<string>()
                .HasMaxLength(20);

            // Relationship with Invoice
            b.HasOne(ii => ii.Invoice)
                .WithMany(i => i.Items)
                .HasForeignKey(ii => ii.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Optional: Add indexes for performance
            b.HasIndex(ii => ii.InvoiceId);
        });

    }
}
