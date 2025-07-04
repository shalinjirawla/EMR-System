using Abp.Application.Services;
using EMRSystem.Patients.Dto;
using EMRSystem.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using EMRSystem.Doctor;
using EMRSystem.Nurse;
using EMRSystem.Visits.Dto;
using Abp.Application.Services.Dto;
using EMRSystem.Appointments.Dto;
using EMRSystem.Appointments;
using Microsoft.EntityFrameworkCore;
using Abp.Extensions;

namespace EMRSystem.Visits
{
    public class VisitAppService : AsyncCrudAppService<Visit, VisitListDto, long, PagedAndSortedResultRequestDto, CreateUpdateVisitDto, CreateUpdateVisitDto>,
    IVisitAppService
    {
        private readonly IDoctorAppService _doctorAppService;
        private readonly INurseAppService _nurseAppService;
        public VisitAppService(IRepository<Visit, long> repository
             , IDoctorAppService doctorAppService, INurseAppService nurseAppService
            ) : base(repository)
        {
            _doctorAppService = doctorAppService;
            _nurseAppService = nurseAppService;
        }
        public async Task CreateVisit(CreateUpdateVisitDto dto)
        {
            var appointment = ObjectMapper.Map<Visit>(dto);
            await Repository.InsertAsync(appointment);
            CurrentUnitOfWork.SaveChanges();
        }

        protected override IQueryable<Visit> CreateFilteredQuery(PagedAndSortedResultRequestDto input)
        {
            var userId = AbpSession.UserId;
            var doctor = _doctorAppService.GetDoctorDetailsByAbpUserID(userId.Value);
            var nurse = _nurseAppService.GetNurseDetailsByAbpUserID(userId.Value);

            // Start with base query without projection
            var baseQuery = Repository
                .GetAll()
                .Include(x => x.Patient)
                .Include(x => x.Doctor)
                .Include(x => x.Nurse)
                .Include(x => x.Department);

            var filteredQuery = baseQuery.AsQueryable(); // Explicitly convert to IQueryable

            if (doctor != null)
            {
                filteredQuery = filteredQuery.Where(i => i.DoctorId == doctor.Id);
            }
            else if (nurse != null)
            {
                filteredQuery = filteredQuery.Where(i => i.NurseId == nurse.Id);
            }

            //if (!input.Keyword.IsNullOrWhiteSpace())
            //{
            //    filteredQuery = filteredQuery.Where(x =>
            //        (x.Patient.FullName != null && x.Patient.FullName.Contains(input.Keyword)) ||
            //        (x.Doctor.FullName != null && x.Doctor.FullName.Contains(input.Keyword)));
            //}

            //if (input.Status.HasValue)
            //{
            //    filteredQuery = filteredQuery.Where(x => x.Status == input.Status.Value);
            //}

            // Apply projection after all filtering
            var result = filteredQuery.Select(x => new Visit
            {
                Id = x.Id,
                TenantId = x.TenantId,
                PatientId = x.PatientId,
                DepartmentId = x.DepartmentId,
                NurseId = x.NurseId,
                DoctorId = x.DoctorId,
                DateOfVisit = x.DateOfVisit,
                TimeOfVisit = x.TimeOfVisit,
                ReasonForVisit = x.ReasonForVisit,
                PaymentMode = x.PaymentMode,
                ConsultationFee = x.ConsultationFee,
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
                Nurse = x.Nurse == null ? null : new EMRSystem.Nurses.Nurse
                {
                    Id = x.Nurse.Id,
                    FullName = x.Nurse.FullName
                },
                Department = x.Department == null ? null : new EMRSystem.Departments.Department
                {
                    Id = x.Department.Id,
                    Name = x.Department.Name
                },
            });

            return result;
        }
    }
}
