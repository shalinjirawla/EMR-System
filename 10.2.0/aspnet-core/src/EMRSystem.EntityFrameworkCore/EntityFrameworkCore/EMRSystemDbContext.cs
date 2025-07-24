using Abp.Zero.EntityFrameworkCore;
using EMRSystem.Appointments;
using EMRSystem.Authorization.Roles;
using EMRSystem.Authorization.Users;
using EMRSystem.Billings;
using EMRSystem.Departments;
using EMRSystem.Doctors;
using EMRSystem.Invoices;
using EMRSystem.LabReports;
using EMRSystem.LabReportsTypes;
using EMRSystem.MultiTenancy;
using EMRSystem.Nurses;
using EMRSystem.Patients;
using EMRSystem.Pharmacists;
using EMRSystem.Prescriptions;
using EMRSystem.Room;
using EMRSystem.RoomMaster;
using EMRSystem.Visits;
using EMRSystem.Vitals;
using Microsoft.EntityFrameworkCore;    // NEW


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
    public DbSet<Visit> Visits { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<EMRSystem.MedicineOrder.MedicineOrder> MedicineOrders { get; set; }
    public DbSet<EMRSystem.MedicineOrder.MedicineOrderItem> MedicineOrderItems { get; set; }
    public DbSet<EMRSystem.Room.Room> Rooms { get; set; }
    public DbSet<RoomTypeMaster> RoomTypes { get; set; }
    public DbSet<RoomFacilityMaster> RoomFacilitiesMaster { get; set; }
    public DbSet<RoomTypeFacility> RoomTypeFacilities { get; set; }
    public DbSet<EMRSystem.Admission.Admission> Admissions { get; set; }
    public DbSet<EMRSystem.Deposit.Deposit> Deposits { get; set; }
    public DbSet<EMRSystem.DoctorMaster.DoctorMaster> DoctorMasters { get; set; }
    public DbSet<EMRSystem.AppointmentType.AppointmentType> AppointmentTypes { get; set; }
    public DbSet<EMRSystem.AppointmentReceipt.AppointmentReceipt> AppointmentReceipts { get; set; }



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

        modelBuilder.Entity<EMRSystem.DoctorMaster.DoctorMaster>()
                .HasOne(dm => dm.Doctor)
                .WithMany() // or `.WithOne()` if one-to-one
                .HasForeignKey(dm => dm.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

        //modelBuilder.Entity<Patient>()
        //        .HasOne(s => s.Doctors)
        //        .WithMany(e => e.Patients)
        //        .HasForeignKey(s => s.AssignedDoctorId)
        //        .OnDelete(DeleteBehavior.NoAction); // or NoAction

        //modelBuilder.Entity<Patient>()
        //       .HasOne(s => s.Nurses)
        //       .WithMany(e => e.Patients)
        //       .HasForeignKey(s =>s.AssignedNurseId )
        //       .OnDelete(DeleteBehavior.NoAction); // or NoAction

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

        //modelBuilder.Entity<Appointment>()
        //    .HasOne(a => a.Nurse)
        //    .WithMany(n => n.Appointments)
        //    .HasForeignKey(a => a.NurseId)
        //    .OnDelete(DeleteBehavior.NoAction); // OR Restrict

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
        // MedicineOrder - Nurse relationship
        modelBuilder.Entity<EMRSystem.MedicineOrder.MedicineOrder>()
            .HasOne(m => m.Nurse)
            .WithMany(n => n.MedicineOrders)
            .HasForeignKey(m => m.NurseId)
            .OnDelete(DeleteBehavior.NoAction);

        // MedicineOrder - Patient relationship
        modelBuilder.Entity<EMRSystem.MedicineOrder.MedicineOrder>()
            .HasOne(m => m.Patient)
            .WithMany(p => p.MedicineOrders)
            .HasForeignKey(m => m.PatientId)
            .OnDelete(DeleteBehavior.NoAction);

        // MedicineOrderItem - MedicineOrder relationship
        modelBuilder.Entity<EMRSystem.MedicineOrder.MedicineOrderItem>()
            .HasOne(m => m.MedicineOrder)
            .WithMany(o => o.Items)
            .HasForeignKey(m => m.MedicineOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // MedicineOrderItem - Medicine (PharmacistInventory)
        modelBuilder.Entity<EMRSystem.MedicineOrder.MedicineOrderItem>()
            .HasOne(m => m.Medicine)
            .WithMany() // Assuming no navigation from inventory → order items
            .HasForeignKey(m => m.MedicineId)
            .OnDelete(DeleteBehavior.Restrict);

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

        modelBuilder.Entity<EMRSystem.Room.Room>(b =>
        {
            b.ToTable("Rooms");

            b.HasOne(r => r.RoomTypeMaster)             // FK to master
             .WithMany()
             .HasForeignKey(r => r.RoomTypeMasterId)
             .OnDelete(DeleteBehavior.Restrict);

            b.Property(r => r.Status)
             .HasConversion<string>()
             .HasMaxLength(20);
        });


        modelBuilder.Entity<RoomFacilityMaster>(b =>
        {
            b.ToTable("RoomFacilityMasters");
            b.HasIndex(x => new { x.TenantId, x.FacilityName }).IsUnique();
        });
        modelBuilder.Entity<RoomTypeMaster>(b =>
        {
            b.ToTable("RoomTypeMasters");
            b.Property(x => x.DefaultPricePerDay).HasColumnType("decimal(18,2)");
            b.HasIndex(x => new { x.TenantId, x.TypeName }).IsUnique();
        });
        modelBuilder.Entity<EMRSystem.AppointmentType.AppointmentType>(b =>
        {
            b.ToTable("AppointmentTypes");
            b.Property(x => x.Fee).HasColumnType("decimal(18,2)");
            b.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();
        });
        modelBuilder.Entity<Appointment>()
    .HasOne(a => a.AppointmentType)
    .WithMany()
    .HasForeignKey(a => a.AppointmentTypeId)
    .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<RoomTypeFacility>(b =>
        {
            b.ToTable("RoomTypeFacilities");

            b.HasIndex(x => new { x.TenantId, x.RoomTypeMasterId, x.RoomFacilityMasterId }).IsUnique();

            b.HasOne(rt => rt.RoomTypeMaster)
             .WithMany(rtm => rtm.Facilities)
             .HasForeignKey(rt => rt.RoomTypeMasterId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(rt => rt.RoomFacilityMaster)
             .WithMany()
             .HasForeignKey(rt => rt.RoomFacilityMasterId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EMRSystem.Admission.Admission>(b =>
        {
            b.ToTable("Admissions");
            b.HasOne(a => a.Patient)
                .WithMany(p => p.Admissions)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            b.HasOne(a => a.Doctor)
                .WithMany(d => d.Admissions)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.NoAction);

            b.HasOne(a => a.Nurse)
                .WithMany(n => n.Admissions)
                .HasForeignKey(a => a.NurseId)
                .OnDelete(DeleteBehavior.NoAction);

            b.HasOne(a => a.Room)
                .WithMany(r => r.Admissions)
                .HasForeignKey(a => a.RoomId)
                .OnDelete(DeleteBehavior.NoAction);
        });
        // In OnModelCreating method
        modelBuilder.Entity<EMRSystem.AppointmentReceipt.AppointmentReceipt>(b =>
        {
            b.ToTable("AppointmentReceipts");

            // Configure decimal precision
            b.Property(x => x.ConsultationFee)
                .HasColumnType("decimal(18,2)");
            b.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            // Configure enum as string
            b.Property(e => e.PaymentMethod)
                .HasConversion<string>()
                .HasMaxLength(20);

            // Relationships
            b.HasOne(ar => ar.Appointment)
                .WithMany()
                .HasForeignKey(ar => ar.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(ar => ar.Patient)
                .WithMany()
                .HasForeignKey(ar => ar.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(ar => ar.Doctor)
                .WithMany()
                .HasForeignKey(ar => ar.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index for receipt number
            b.HasIndex(ar => ar.ReceiptNumber)
                .IsUnique();
        });

        // STEP 4‑b Deposit configuration
        modelBuilder.Entity<EMRSystem.Deposit.Deposit>(b =>
        {
            b.ToTable("Deposits");

            b.Property(d => d.Amount)
             .HasColumnType("decimal(18,2)");

            b.Property(d => d.PaymentMethod)
             .HasConversion<string>()
             .HasMaxLength(20);

            b.HasOne(d => d.Patient)
             .WithMany(a => a.Deposits)
             .HasForeignKey(d => d.PatientId)
             .OnDelete(DeleteBehavior.NoAction);

            b.Property(e => e.BillingMethod)
            .HasConversion<string>()
            .HasMaxLength(30);
        });

        modelBuilder.Entity<Visit>()
           .HasOne(s => s.Patient)
           .WithMany(e => e.Visit)
           .HasForeignKey(s => s.PatientId)
           .OnDelete(DeleteBehavior.NoAction); // or NoAction

        modelBuilder.Entity<Visit>()
          .HasOne(s => s.Nurse)
          .WithMany(e => e.Visits)
          .HasForeignKey(s => s.NurseId)
          .OnDelete(DeleteBehavior.NoAction); // or NoAction

        modelBuilder.Entity<Visit>()
          .HasOne(s => s.Doctor)
          .WithMany(e => e.Visits)
          .HasForeignKey(s => s.DoctorId)
          .OnDelete(DeleteBehavior.NoAction); // or NoAction

        modelBuilder.Entity<Visit>()
          .HasOne(s => s.Department)
          .WithMany(e => e.Visits)
          .HasForeignKey(s => s.DepartmentId)
          .OnDelete(DeleteBehavior.NoAction); // or NoAction
    }
}
