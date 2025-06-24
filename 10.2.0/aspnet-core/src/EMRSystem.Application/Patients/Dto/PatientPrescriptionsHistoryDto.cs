using EMRSystem.Prescriptions.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Patients.Dto
{
    public class PatientPrescriptionsHistoryDto
    {
        public long DoctorId { get; set; }
        public string DoctorName { get; set; }
        public List<PatientPrescriptionsItemHistoryDto> Items { get; set; }
    }
}
