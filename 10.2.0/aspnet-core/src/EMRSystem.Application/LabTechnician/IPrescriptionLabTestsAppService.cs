using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.Appointments.Dto;
using EMRSystem.LabTechnician.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabTechnician
{
    public interface IPrescriptionLabTestsAppService : IAsyncCrudAppService<
   LabRequestListDto, long, PagedAndSortedResultRequestDto, CreateUpdateLabRequestDto, CreateUpdateLabRequestDto>
    {
        EMRSystem.LabReports.PrescriptionLabTest GetPrescriptionLabTestDetailsById(long id);
        EMRSystem.LabReports.PrescriptionLabTest GetPrescriptionLabTestDetailsForViewReportById(long id);
        Task MakeInprogressReport(long prescriptionLabTestId);
    }
}
