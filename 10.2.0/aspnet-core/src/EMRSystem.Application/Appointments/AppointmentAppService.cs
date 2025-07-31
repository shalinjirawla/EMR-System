using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.Collections.Extensions;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.UI;
using EMRSystem.AppointmentReceipt.Dto;
using EMRSystem.Appointments.Dto;
using EMRSystem.Authorization.Users;
using EMRSystem.Doctor;
using EMRSystem.Doctors;
using EMRSystem.Invoices;
using EMRSystem.IpdChargeEntry;
using EMRSystem.LabReports;
using EMRSystem.Nurse;
using EMRSystem.Patients;
using EMRSystem.Prescriptions;
using EMRSystem.Prescriptions.Dto;
using EMRSystem.Users.Dto;
using EMRSystem.Vitals;
using EMRSystem.Vitals.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using PaymentMethod = EMRSystem.Invoices.PaymentMethod;


namespace EMRSystem.Appointments
{
    //AbpAuthorize(PermissionNames.Pages_Billing)]
    public class AppointmentAppService : AsyncCrudAppService<EMRSystem.Appointments.Appointment, AppointmentDto, long, PagedAppoinmentResultRequestDto, CreateUpdateAppointmentDto, CreateUpdateAppointmentDto>,
  IAppointmentAppService
    {
        private readonly IDoctorAppService _doctorAppService;
        private readonly IRepository<EMRSystem.Doctors.Doctor, long> _doctorRepository;
        private readonly IConfiguration _configuration;
        private readonly INurseAppService _nurseAppService;
        private readonly UserManager _userManager;
        private readonly IRepository<EMRSystem.AppointmentReceipt.AppointmentReceipt, long> _receiptRepository;
        private readonly IRepository<EMRSystem.DoctorMaster.DoctorMaster, long> _doctorMasterRepository;
        private readonly IRepository<Patient, long> _patientRepository; 
        private readonly IRepository<EMRSystem.IpdChargeEntry.IpdChargeEntry, long> _ipdChargeEntryRepository;
        public AppointmentAppService(
                IRepository<Appointment, long> repository,
                IConfiguration configuration,
                IDoctorAppService doctorAppService,
                INurseAppService nurseAppService,
                UserManager userManager,
                IRepository<EMRSystem.AppointmentReceipt.AppointmentReceipt, long> receiptRepository,
                IRepository<EMRSystem.DoctorMaster.DoctorMaster, long> doctorMasterRepository,
                IRepository<Patient, long> patientRepository, // Add this
                IRepository<EMRSystem.Doctors.Doctor,long> doctorRepository,
                IRepository<EMRSystem.IpdChargeEntry.IpdChargeEntry, long> ipdChargeEntryRepository)
    : base(repository)
        {
                _doctorAppService = doctorAppService;
                _configuration = configuration;
                _nurseAppService = nurseAppService;
                _userManager = userManager;
                _receiptRepository = receiptRepository;
                _doctorMasterRepository = doctorMasterRepository;
                _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
                _ipdChargeEntryRepository = ipdChargeEntryRepository;
        }
        protected override IQueryable<Appointment> CreateFilteredQuery(PagedAppoinmentResultRequestDto input)
        {
            var userId = AbpSession.UserId;
            var doctor = _doctorAppService.GetDoctorDetailsByAbpUserID(userId.Value);
            var nurse = _nurseAppService.GetNurseDetailsByAbpUserID(userId.Value);

            // Start with base query without projection
            var baseQuery = Repository
                .GetAll()
                .Include(x => x.Patient)
                .Include(x => x.Doctor);
                //.Include(x => x.Nurse);

            var filteredQuery = baseQuery.Where(x => x.TenantId == AbpSession.TenantId.Value).AsQueryable(); // Explicitly convert to IQueryable

            if (doctor != null)
            {
                filteredQuery = filteredQuery.Where(i => i.DoctorId == doctor.Id);
            }
            //else if (nurse != null)
            //{
            //    filteredQuery = filteredQuery.Where(i => i.NurseId == nurse.Id);
            //}

            if (!input.Keyword.IsNullOrWhiteSpace())
            {
                filteredQuery = filteredQuery.Where(x =>
                    (x.Patient.FullName != null && x.Patient.FullName.Contains(input.Keyword)) ||
                    (x.Doctor.FullName != null && x.Doctor.FullName.Contains(input.Keyword)));
            }

            if (input.Status.HasValue)
            {
                filteredQuery = filteredQuery.Where(x => x.Status == input.Status.Value);
            }

            // Apply projection after all filtering
            var result = filteredQuery.Select(x => new Appointment
            {
                Id = x.Id,
                TenantId = x.TenantId,
                AppointmentDate = x.AppointmentDate,
                //StartTime = x.StartTime,
                //EndTime = x.EndTime,
                ReasonForVisit = x.ReasonForVisit,
                Status = x.Status,
                IsFollowUp = x.IsFollowUp,
                IsPaid = x.IsPaid,
                PatientId = x.PatientId,
                DoctorId = x.DoctorId,
                //NurseId = x.NurseId,
                AppointmentTypeId=x.AppointmentTypeId,
                AppointmentType = x.AppointmentType==null?null:new EMRSystem.AppointmentType.AppointmentType
                {
                    Id = x.AppointmentType.Id,
                    Name = x.AppointmentType.Name
                },

                Patient = x.Patient == null ? null : new Patient
                {
                    Id = x.Patient.Id,
                    FullName = x.Patient.FullName,
                    IsAdmitted = x.Patient.IsAdmitted
                },
                Doctor = x.Doctor == null ? null : new EMRSystem.Doctors.Doctor
                {
                    Id = x.Doctor.Id,
                    FullName = x.Doctor.FullName
                },
                //Nurse = x.Nurse == null ? null : new EMRSystem.Nurses.Nurse
                //{
                //    Id = x.Nurse.Id,
                //    FullName = x.Nurse.FullName
                //},
            });

            return result;
        }
        protected override IQueryable<Appointment> ApplySorting(IQueryable<Appointment> query, PagedAppoinmentResultRequestDto input)
        {
            if (!string.IsNullOrWhiteSpace(input.Sorting))
            {
                var sorting = input.Sorting;

                if (sorting.Contains("patientName", StringComparison.OrdinalIgnoreCase))
                    sorting = sorting.Replace("patientName", "Patient.FullName", StringComparison.OrdinalIgnoreCase);

                if (sorting.Contains("doctorName", StringComparison.OrdinalIgnoreCase))
                    sorting = sorting.Replace("doctorName", "Doctor.FullName", StringComparison.OrdinalIgnoreCase);

                return query.OrderBy(sorting);
            }

            return base.ApplySorting(query, input);
        }
        public ListResultDto<AppointmentDto> GetPatientAppointment(long patientId, long doctorId)
        {
            var appointments = Repository.GetAllIncluding(x => x.Patient)
                    .WhereIf(doctorId > 0, x => x.DoctorId == doctorId)
                    .Where(x => x.PatientId == patientId)
                    .ToList();

            var mapped = ObjectMapper.Map<List<AppointmentDto>>(appointments);
            return new ListResultDto<AppointmentDto>(mapped);
        }
        public async Task<ListResultDto<AppointmentDto>> GetByPatient(long patientId)
        {
            // Get all appointments including Patient and Doctor
            var appointments = await Repository.GetAllIncluding(x => x.Patient, x => x.Doctor)
                .Where(x => x.PatientId == patientId)
                .OrderByDescending(x => x.AppointmentDate)
                .ToListAsync();

            // Map to DTO with doctor information
            var appointmentDtos = ObjectMapper.Map<List<AppointmentDto>>(appointments);

            return new ListResultDto<AppointmentDto>(appointmentDtos);
        }
        public async Task<CreateUpdateAppointmentDto> GetAppointmentDetailsById(long id)
        {
            var details = await Repository.GetAllIncludingAsync(x => x.Patient, x => x.Doctor).Result
              .Select(x => new Appointment
              {
                  Id = x.Id,
                  TenantId = x.TenantId,
                  AppointmentDate = x.AppointmentDate,
                  //StartTime = x.StartTime,
                  //EndTime = x.EndTime,
                  ReasonForVisit = x.ReasonForVisit,
                  Status = x.Status,
                  IsFollowUp = x.IsFollowUp,
                  PatientId = x.PatientId,
                  DoctorId = x.DoctorId,
                  //NurseId = x.NurseId,
                  AppointmentTypeId=x.AppointmentTypeId,
                  Patient = x.Patient == null ? null : new Patient
                  {
                      Id = x.Patient.Id,
                      FullName = x.Patient.FullName
                  },
                  Doctor = x.Doctor == null ? null : new EMRSystem.Doctors.Doctor
                  {
                      Id = x.Doctor.Id,
                      FullName = x.Doctor.FullName
                  },
                  //Nurse = x.Nurse == null ? null : new EMRSystem.Nurses.Nurse
                  //{
                  //    Id = x.Nurse.Id,
                  //    FullName = x.Nurse.FullName
                  //},
              })
                .FirstOrDefaultAsync(x => x.Id == id);

            if (details == null)
            {
                throw new EntityNotFoundException(typeof(Appointment), id);
            }
            var prescription = ObjectMapper.Map<CreateUpdateAppointmentDto>(details);
            return prescription;
        }
        //public async Task CreateAppoinment(CreateUpdateAppointmentDto dto)
        //{
        //    var appointment = ObjectMapper.Map<Appointment>(dto);
        //    await Repository.InsertAsync(appointment);
        //    CurrentUnitOfWork.SaveChanges();
        //}

        //public async Task<AppointmentReceiptDto> CreateAppoinment(CreateUpdateAppointmentDto dto)
        //{
        //    // 1. Map and create appointment (automatically ignores PaymentMethod)
        //    var appointment = ObjectMapper.Map<Appointment>(dto);
        //    await Repository.InsertAsync(appointment);
        //    await CurrentUnitOfWork.SaveChangesAsync();

        //    // 2. Handle payment if specified

        //    await GenerateAppointmentReceipt(appointment.Id, dto.PaymentMethod.ToString());


        //    return null;
        //}

        public async Task<AppointmentCreationResultDto> CreateAppoinment(CreateUpdateAppointmentDto dto)
        {
            var patient = await _patientRepository
       .GetAllIncluding(p => p.Admissions)
       .FirstOrDefaultAsync(p => p.Id == dto.PatientId);
            var doctor = await _doctorRepository.GetAsync(dto.DoctorId);

            if (patient == null)
                throw new UserFriendlyException("Patient not found");

            if (patient.IsAdmitted)
            {
                // Verify admissions exist
                if (!patient.Admissions.Any())
                    throw new UserFriendlyException("Patient is marked as admitted but has no admission records");

                // Get most recent ACTIVE admission
                var admission = patient.Admissions
                    .Where(a => !a.IsDischarged)
                    .OrderByDescending(a => a.AdmissionDateTime)
                    .FirstOrDefault();

                if (admission == null)
                    throw new UserFriendlyException("No active admission found for patient");

                // Rest of your IPD logic...
                var appointment = ObjectMapper.Map<Appointment>(dto);
                appointment.IsPaid = true;
                await Repository.InsertAsync(appointment);

                var doctorFee = await _doctorMasterRepository.FirstOrDefaultAsync(dm =>
                    dm.DoctorId == appointment.DoctorId &&
                    dm.TenantId == appointment.TenantId);

                var chargeEntry = new EMRSystem.IpdChargeEntry.IpdChargeEntry
                {
                    AdmissionId = admission.Id,
                    PatientId = patient.Id,
                    ChargeType = ChargeType.Appointment,
                    Description = $"Consultation - Dr. {doctor.FullName}",
                    Amount = doctorFee?.Fee ?? 0,
                    //ReferenceId = appointment.Id
                };

                await _ipdChargeEntryRepository.InsertAsync(chargeEntry);

                return new AppointmentCreationResultDto
                {
                    IsStripeRedirect = false,
                    Message = "IPD appointment created. Charge will be deducted from deposit."
                };
            }
            else
            {
                // OPD Patient - Handle payment
                var appointment = ObjectMapper.Map<Appointment>(dto);
                appointment.IsPaid = (dto.PaymentMethod != PaymentMethod.Card);
                await Repository.InsertAsync(appointment);
                await CurrentUnitOfWork.SaveChangesAsync();

                if (dto.PaymentMethod == PaymentMethod.Card)
                {
                    var doctorFee = await _doctorMasterRepository.FirstOrDefaultAsync(dm =>
                        dm.DoctorId == appointment.DoctorId &&
                        dm.TenantId == appointment.TenantId);

                    if (doctorFee == null)
                        throw new UserFriendlyException("Doctor fee configuration not found");

                    var stripeUrl = await CreateStripeCheckoutSessionForAppointment(
                        appointment,
                        doctorFee.Fee,
                        "http://localhost:4200/app/nurse/appointments",
                        "http://localhost:4200/app/nurse/appointments");

                    return new AppointmentCreationResultDto
                    {
                        IsStripeRedirect = true,
                        StripeSessionUrl = stripeUrl
                    };
                }
                else // Cash payment
                {
                    var receipt = await GenerateAppointmentReceipt(appointment.Id, dto.PaymentMethod.ToString());
                    return new AppointmentCreationResultDto
                    {
                        IsStripeRedirect = false,
                        Receipt = receipt
                    };
                }
            }
        }
        public async Task<string> InitiatePaymentForAppointment(long appointmentId)
        {
            var appointment = await Repository.GetAsync(appointmentId);
            if (appointment == null)
                throw new EntityNotFoundException(typeof(Appointment), appointmentId);

            if (appointment.IsPaid)
                throw new UserFriendlyException("Appointment is already paid");

            var doctorFee = await _doctorMasterRepository.FirstOrDefaultAsync(dm =>
                dm.DoctorId == appointment.DoctorId &&
                dm.TenantId == appointment.TenantId)
                ?? throw new UserFriendlyException("Doctor fee not found");

            return await CreateStripeCheckoutSessionForAppointment(
                appointment,
                doctorFee.Fee,
                "http://localhost:4200/app/nurse/appointments",
                "http://localhost:4200/app/nurse/appointments"
            );
        }
        private async Task<string> CreateStripeCheckoutSessionForAppointment(Appointment appointment,decimal amount,string successUrl,string cancelUrl)
        {
            // 1. Add null checks
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment));

            if (string.IsNullOrWhiteSpace(successUrl))
                throw new ArgumentException("Success URL cannot be empty", nameof(successUrl));

            if (string.IsNullOrWhiteSpace(cancelUrl))
                throw new ArgumentException("Cancel URL cannot be empty", nameof(cancelUrl));

            // 2. Ensure Doctor is loaded
            if (appointment.Doctor == null)
            {
                appointment = await Repository.GetAll()
                    .Include(a => a.Doctor)
                    .FirstOrDefaultAsync(a => a.Id == appointment.Id);

                if (appointment?.Doctor == null)
                    throw new UserFriendlyException("Doctor information not found for appointment");
            }

            // 3. Safely get doctor name
            var doctorName = appointment.Doctor?.FullName ?? "Unknown Doctor";

            // 4. Configure Stripe
            StripeConfiguration.ApiKey = _configuration?["Stripe:SecretKey"]
                ?? throw new ConfigurationException("Stripe:SecretKey is not configured");

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new()
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(amount * 100),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = $"Consultation Fee - Dr. {doctorName}",
                        Description = appointment.ReasonForVisit ?? "Medical Consultation"
                    },
                },
                Quantity = 1,
            },
        },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                Metadata = new Dictionary<string, string>
        {
            { "appointmentId", appointment.Id.ToString() },
            { "tenantId", appointment.TenantId.ToString() },
            { "purpose", "appointment" }
        }
            };

            try
            {
                var service = new SessionService();
                var session = await service.CreateAsync(options);
                return session.Url;
            }
            catch (StripeException ex)
            {
                Logger.Error("Stripe session creation failed", ex);
                throw new UserFriendlyException("Payment processing error. Please try again.");
            }
        }
        public async Task UpdateAppoinment(CreateUpdateAppointmentDto input)
        {
            var appointment = ObjectMapper.Map<Appointment>(input);
            await Repository.UpdateAsync(appointment);
            CurrentUnitOfWork.SaveChanges();
        }
        [HttpGet]
        public async Task<List<string>> GetCurrentUserRolesAsync()
        {
            var user = await _userManager.GetUserByIdAsync(AbpSession.UserId.Value);
            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }
        public async Task MarkAsAction(long id, AppointmentStatus appointmentStatus)
        {
            var data = await Repository.GetAsync(id);
            if (data == null)
                return;
            data.Status = appointmentStatus;
            await Repository.UpdateAsync(data);
            CurrentUnitOfWork.SaveChanges();
        }

        public async Task<AppointmentReceiptDto> GenerateAppointmentReceipt(long appointmentId, string paymentMethod)
        {
            var appointment = await Repository.GetAll()
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
                throw new EntityNotFoundException(typeof(Appointment), appointmentId);

            // Get doctor's current fee
            var doctorFee = await _doctorMasterRepository.FirstOrDefaultAsync(dm =>
                dm.DoctorId == appointment.DoctorId &&
                dm.TenantId == appointment.TenantId);

            if (doctorFee == null)
                throw new UserFriendlyException("Doctor fee configuration not found");

            var receipt = new EMRSystem.AppointmentReceipt.AppointmentReceipt
            {
                TenantId = appointment.TenantId,
                AppointmentId = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                ConsultationFee = doctorFee.Fee,
                PaymentMethod = (PaymentMethod)Enum.Parse(typeof(PaymentMethod), paymentMethod),
                ReceiptNumber = await GenerateReceiptNumberAsync(),
                Status = InvoiceStatus.Paid,
                PaymentDate = DateTime.Now
            };

            await _receiptRepository.InsertAsync(receipt);
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<AppointmentReceiptDto>(receipt);
        }

        private async Task<string> GenerateReceiptNumberAsync()
        {
            var today = DateTime.Today.ToString("yyyyMMdd");
            var lastReceipt = await _receiptRepository.GetAll()
                .Where(r => r.ReceiptNumber.StartsWith($"REC-{today}"))
                .OrderByDescending(r => r.ReceiptNumber)
                .FirstOrDefaultAsync();

            int sequence = 1;
            if (lastReceipt != null)
            {
                var parts = lastReceipt.ReceiptNumber.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int lastSeq))
                {
                    sequence = lastSeq + 1;
                }
            }

            return $"REC-{today}-{sequence.ToString().PadLeft(3, '0')}";
        }

        public async Task<AppointmentReceiptDto> GetReceiptForAppointment(long appointmentId)
        {
            var receipt = await _receiptRepository.GetAll()
                .Include(r => r.Patient)
                .Include(r => r.Doctor)
                .Include(r => r.Appointment)
                .FirstOrDefaultAsync(r => r.AppointmentId == appointmentId);

            if (receipt == null)
                throw new EntityNotFoundException(typeof(EMRSystem.AppointmentReceipt.AppointmentReceipt), appointmentId);

            return ObjectMapper.Map<AppointmentReceiptDto>(receipt);
        }

        public async Task<ListResultDto<AppointmentReceiptDto>> GetPatientReceipts(long patientId)
        {
            var receipts = await _receiptRepository.GetAll()
                .Include(r => r.Doctor)
                .Include(r => r.Appointment)
                .Where(r => r.PatientId == patientId)
                .OrderByDescending(r => r.PaymentDate)
                .ToListAsync();

            return new ListResultDto<AppointmentReceiptDto>(
                ObjectMapper.Map<List<AppointmentReceiptDto>>(receipts)
            );
        }
    }
}
