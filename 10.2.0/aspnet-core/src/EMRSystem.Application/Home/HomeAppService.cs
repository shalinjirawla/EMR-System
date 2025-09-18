using Abp.Application.Services;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using EMRSystem.Appointments.Dto;
using EMRSystem.Authorization.Users;
using EMRSystem.Doctor;
using EMRSystem.Doctor.Dto;
using EMRSystem.Home.Dto;
using EMRSystem.Patients;
using EMRSystem.Patients.Dto;
using EMRSystem.Pharmacist.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Home
{
    public class HomeAppService : ApplicationService, IHomeAppService
    {
        private readonly IRepository<Patient, long> _patientRepository;
        private readonly IRepository<EMRSystem.Appointments.Appointment, long> _appointmentRepository;
        private readonly IRepository<EMRSystem.Nurses.Nurse, long> _nurseRepository;
        private readonly IRepository<EMRSystem.Doctors.Doctor, long> _doctorRepository;
        private readonly IRepository<EMRSystem.Pharmacists.PharmacistInventory, long> _pharmacistInventoryRepository;
        private readonly UserManager _userManager;
        private readonly IDoctorAppService _doctorAppService;
        public HomeAppService(IRepository<Patient, long> patientRepository,
            IRepository<Appointments.Appointment, long> appointmentRepository
            , IRepository<EMRSystem.Nurses.Nurse, long> nurseRepository,
            IRepository<Doctors.Doctor, long> doctorRepository, UserManager userManager,
            IRepository<EMRSystem.Pharmacists.PharmacistInventory, long> pharmacistInventoryRepository,
        IDoctorAppService doctorAppService)
        {
            _patientRepository = patientRepository;
            _appointmentRepository = appointmentRepository;
            _nurseRepository = nurseRepository;
            _doctorRepository = doctorRepository;
            _userManager = userManager;
            _doctorAppService = doctorAppService;
            _pharmacistInventoryRepository = pharmacistInventoryRepository;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            var user = await _userManager.GetUserByIdAsync(AbpSession.UserId.Value);
            var roles = await _userManager.GetRolesAsync(user);
            var totalAppointmentList = await _appointmentRepository.GetAllIncluding(x => x.Patient, x => x.Doctor, x => x.Department).OrderByDescending(x => x.Id).ToListAsync();
            var totalPatientList = await _patientRepository.GetAllIncluding(x => x.AbpUser).OrderByDescending(x => x.Id).ToListAsync();
            var totalDoctorList = await _doctorRepository.GetAllIncluding(x => x.AbpUser, x => x.Department).OrderByDescending(x => x.Id).ToListAsync();
            var medicineList = await _pharmacistInventoryRepository.GetAll().OrderByDescending(x => x.Id).ToListAsync();
            var dto = new DashboardSummaryDto();

            if (roles.Contains("Admin"))
            {
                dto.TotalAppointments = await _appointmentRepository.CountAsync();
                dto.TotalPatients = await _patientRepository.CountAsync();
                dto.TotalDoctors = await _doctorRepository.CountAsync();
                dto.TotalNurses = await _nurseRepository.CountAsync();
                dto.TotalAppointmentList = ObjectMapper.Map<List<AppointmentDto>>(totalAppointmentList.Take(5));
                dto.TotalPatientList = ObjectMapper.Map<List<PatientDto>>(totalPatientList.Take(5));
                dto.TotalDoctorList = ObjectMapper.Map<List<DoctorDto>>(totalDoctorList.Take(5));
                dto.TotalMedicineList = ObjectMapper.Map<List<PharmacistInventoryDto>>(medicineList.Take(8));
                var departmentWise = totalAppointmentList
                                    .Where(a => a.Department != null)
                                    .GroupBy(a => a.Department.DepartmentName)
                                    .Select(g => new ChartDataDto
                                    {
                                        Label = g.Key,
                                        Value = g.Count()
                                    })
                                    .ToList();

                dto.DepartmentWiseAppointments = departmentWise;

                // Historical monthly data for last 6 months
                var today = DateTime.Today;
                for (int i = 5; i >= 0; i--)
                {
                    var month = today.AddMonths(-i);
                    var start = new DateTime(month.Year, month.Month, 1);
                    var end = start.AddMonths(1).AddDays(-1);

                    dto.PatientsChart.Add(new ChartDataDto
                    {
                        Label = start.ToString("MMM"),
                        Value = await _patientRepository.CountAsync(p => p.AbpUser.CreationTime >= start && p.AbpUser.CreationTime <= end)
                    });

                    dto.AppointmentsChart.Add(new ChartDataDto
                    {
                        Label = start.ToString("MMM"),
                        Value = await _appointmentRepository.CountAsync(a => a.AppointmentDate >= start && a.AppointmentDate <= end)
                    });

                    dto.DoctorsChart.Add(new ChartDataDto
                    {
                        Label = start.ToString("MMM"),
                        Value = await _doctorRepository.CountAsync(d => d.AbpUser.CreationTime >= start && d.AbpUser.CreationTime <= end)
                    });

                    dto.NursesChart.Add(new ChartDataDto
                    {
                        Label = start.ToString("MMM"),
                        Value = await _nurseRepository.CountAsync(n => n.AbpUser.CreationTime >= start && n.AbpUser.CreationTime <= end)
                    });
                }
            }
            else if (roles.Contains("Doctor"))
            {
                var doctor = _doctorAppService.GetDoctorDetailsByAbpUserID(AbpSession.UserId.Value);
                dto.TotalAppointments = await _appointmentRepository.CountAsync(x => x.DoctorId == doctor.Id);
                dto.TotalPatients = await _patientRepository.CountAsync(x => x.Appointments.Any(a => a.DoctorId == doctor.Id));
                var appoinmentList = totalAppointmentList.Where(x => x.DoctorId == doctor.Id).ToList();
                dto.TotalAppointmentList = ObjectMapper.Map<List<AppointmentDto>>(appoinmentList.Take(5));

                var departmentWise = appoinmentList
                                    .Where(a => a.Department != null)
                                    .GroupBy(a => a.Department.DepartmentName)
                                    .Select(g => new ChartDataDto
                                    {
                                        Label = g.Key,
                                        Value = g.Count()
                                    })
                                    .ToList();

                dto.DepartmentWiseAppointments = departmentWise;

                // Optional: doctor-specific monthly trend
                var today = DateTime.Today;
                for (int i = 5; i >= 0; i--)
                {
                    var month = today.AddMonths(-i);
                    var start = new DateTime(month.Year, month.Month, 1);
                    var end = start.AddMonths(1).AddDays(-1);

                    dto.PatientsChart.Add(new ChartDataDto
                    {
                        Label = start.ToString("MMM"),
                        Value = await _patientRepository.CountAsync(p => p.Appointments.Any(a => a.DoctorId == doctor.Id && a.AppointmentDate >= start && a.AppointmentDate <= end))
                    });

                    dto.AppointmentsChart.Add(new ChartDataDto
                    {
                        Label = start.ToString("MMM"),
                        Value = await _appointmentRepository.CountAsync(a => a.DoctorId == doctor.Id && a.AppointmentDate >= start && a.AppointmentDate <= end)
                    });
                }
            }

            return dto;
        }

    }
}
