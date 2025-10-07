using EMRSystem.Appointments.Dto;
using EMRSystem.Doctor.Dto;
using EMRSystem.Medicines.Dto;
using EMRSystem.Patients.Dto;
using EMRSystem.Pharmacist.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Home.Dto
{
    public class DashboardSummaryDto
    {
        public int TotalAppointments { get; set; }
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalNurses { get; set; }

        // New properties for charts
        public List<ChartDataDto> PatientsChart { get; set; } = new();
        public List<ChartDataDto> AppointmentsChart { get; set; } = new();
        public List<ChartDataDto> DoctorsChart { get; set; } = new();
        public List<ChartDataDto> NursesChart { get; set; } = new();
        public List<ChartDataDto> DepartmentWiseAppointments { get; set; } = new();



        public List<AppointmentDto> TotalAppointmentList { get; set; }
        public List<PatientDto> TotalPatientList { get; set; }
        public List<DoctorDto> TotalDoctorList { get; set; }
        public List<MedicineMasterDto> TotalMedicineList { get; set; }
    }

    public class ChartDataDto
    {
        public string Label { get; set; } = ""; // e.g., "Jan", "Feb", or a category like "Cardiology"
        public int Value { get; set; }
    }
}
