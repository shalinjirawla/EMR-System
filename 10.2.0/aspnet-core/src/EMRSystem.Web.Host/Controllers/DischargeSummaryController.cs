using Abp.AspNetCore.Mvc.Controllers;
using Abp.UI;
using EMRSystem.Patient_Discharge;
using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.Layout.Font;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EMRSystem.Web.Host.Controllers
{
    public class DischargeSummaryController : AbpController
    {
        private readonly IPatientDischargeAppService _dischargeAppService;
        public DischargeSummaryController(IPatientDischargeAppService dischargeAppService)
        {
            _dischargeAppService = dischargeAppService;
        }

        [HttpGet("Download")]
        public async Task<IActionResult> DownloadDischarge(long patientId)
        {
            var dto = await _dischargeAppService.PatientDischargeSummaryAsync(patientId);

            // Load HTML template
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "DischargeSummary.html");
            var html = await System.IO.File.ReadAllTextAsync(templatePath);

            // Generate TreatmentSummary from SelectedEmergencyProcedures
            string treatmentSummaryHtml = "<ul>";
            if (dto.SelectedEmergencyProcedures != null && dto.SelectedEmergencyProcedures.Any())
            {
                foreach (var procedure in dto.SelectedEmergencyProcedures)
                {
                    treatmentSummaryHtml += $"<li>{procedure.ProcedureName}</li>";
                }
            }
            else
            {
                treatmentSummaryHtml += "<li>No procedures performed.</li>";
            }
            treatmentSummaryHtml += "</ul>";

            // Replace placeholders
            html = html.Replace("{{PatientName}}", dto.PatientDetails.FullName)
                       .Replace("{{PatientId}}", dto.PatientDetails.PatientId.ToString())
                       .Replace("{{AgeGender}}", $"{dto.PatientDetails.Age} / {dto.PatientDetails.Gender}")
                       .Replace("{{AdmissionNumber}}", dto.PatientDetails.AdmissionId.ToString())
                       .Replace("{{AdmissionDate}}", dto.PatientDetails.AdmissionDateTime.ToString("dd/MM/yyyy"))
                       .Replace("{{DischargeDate}}", dto.PatientDischarge.DischargeDate?.ToString("dd/MM/yyyy") ?? "")
                       .Replace("{{DoctorName}}", dto.PatientDischarge.DoctorName ?? "")
                       .Replace("{{WardBed}}", $"{dto.PatientDetails.Room} / {dto.PatientDetails.BedNumber}")
                       .Replace("{{ChiefComplaint}}", dto.PatientDetails.ReasonForAdmit ?? "")
                       .Replace("{{ProvisionalDiagnosis}}", dto.PatientDischarge.ProvisionalDiagnosis ?? "")
                       .Replace("{{FinalDiagnosis}}", dto.PatientDischarge.FinalDiagnosis ?? "")
                       .Replace("{{TreatmentSummary}}", treatmentSummaryHtml)
                       .Replace("{{InvestigationSummary}}", dto.PatientDischarge.InvestigationSummary ?? "")
                       .Replace("{{ConditionAtDischarge}}", dto.PatientDischarge.ConditionAtDischarge ?? "")
                       .Replace("{{DietAdvice}}", dto.PatientDischarge.DietAdvice ?? "")
                       .Replace("{{ActivityAdvice}}", dto.PatientDischarge.Activity ?? "")
                       .Replace("{{FollowUpDate}}", dto.PatientDischarge.FollowUpDate?.ToString("dd/MM/yyyy") ?? "")
                       .Replace("{{FollowUpDoctor}}", dto.PatientDischarge.FollowUpDoctorName ?? "")
                       .Replace("{{DoctorQualification}}", dto.PatientDischarge.DoctorQualification ?? "")
                       .Replace("{{GeneratedDate}}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"))
                       .Replace("{{HospitalName}}", "Helthspan Hospital"); // Add dynamically if available

            // Convert to PDF
            byte[] pdfBytes;
            var props = new ConverterProperties();
            props.SetFontProvider(new DefaultFontProvider(true, true, true));

            try
            {
                using (var ms = new MemoryStream())
                {
                    HtmlConverter.ConvertToPdf(html, ms, props);
                    pdfBytes = ms.ToArray();
                }

                return File(pdfBytes, "application/pdf", $"DischargeSummary_{patientId}.pdf");
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("PDF generation failed: " + ex.Message);
            }
        }
    }
}
