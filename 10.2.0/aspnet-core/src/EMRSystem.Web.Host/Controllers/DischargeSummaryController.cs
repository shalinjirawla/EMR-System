using Abp.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using EMRSystem.Patient_Discharge;
using System;
using iText.Html2pdf;
using iText.Layout.Font;
using iText.StyledXmlParser.Css.Media;
using iText.Html2pdf.Resolver.Font;

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
            // Load your HTML template
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "DischargeSummary.html");
            var html = await System.IO.File.ReadAllTextAsync(templatePath);

            html = html.Replace("{{fullName}}", (dto.PatientDetails.FirstName + " " + dto.PatientDetails.LastName) ?? "")
                  .Replace("{{dateOfBirth}}", dto.PatientDetails.Dob.ToString("dd MM yyyy") ?? "")
                  .Replace("{{gender}}", dto.PatientDetails.Gender.ToString() ?? "")
                  .Replace("{{patientId}}", dto.PatientDetails.PatientId.ToString() ?? "")
                  .Replace("{{admissionDate}}", dto.PatientDetails.AdmissionDateTime.ToString("dd MM yyyy") ?? "")
                  .Replace("{{dischargeDate}}", dto.PatientDetails.AdmissionDateTime.ToString("dd MM yyyy") ?? "")
                  .Replace("{{diagnosis}}", dto.PatientDetails.FirstName ?? "")
                  .Replace("{{physicianName}}", dto.PatientDetails.FirstName ?? "")
                  .Replace("{{physicianContact}}", dto.PatientDetails.FirstName ?? "")
                  .Replace("{{hospitalCourseDescription}}", dto.PatientDetails.FirstName ?? "")
                  .Replace("{{treatmentName}}", dto.PatientDetails.FirstName ?? "")
                  .Replace("{{treatmentDuration}}", dto.PatientDetails.FirstName ?? "")
                  .Replace("{{treatmentNotes}}", dto.PatientDetails.FirstName ?? "")
                  .Replace("{{medicationName}}", dto.PatientDetails.FirstName ?? "")
                  .Replace("{{dosage}}", dto.PatientDetails.FirstName ?? "")
                  .Replace("{{frequency}}", dto.PatientDetails.FirstName ?? "")
                  .Replace("{{medicationDuration}}", dto.PatientDetails.FirstName ?? "")
                  .Replace("{{instruction}}", dto.PatientDetails.FirstName  )
                  .Replace("{{appointmentDate}}", dto.PatientDetails.FirstName ?? "")
                  .Replace("{{department}}", dto.PatientDetails.FirstName   )
                  .Replace("{{provider}}", dto.PatientDetails.FirstName ?? "")
                  .Replace("{{additionalNotes}}", dto.PatientDetails.FirstName ?? "")
                  .Replace("{{emergencyContactName}}", dto.PatientDetails.FirstName ?? "")
                  .Replace("{{emergencyContactPhone}}", dto.PatientDetails.FirstName ?? "")
                  .Replace("{{emergencyContactRelation}}", dto.PatientDetails.FirstName ?? "")
                  .Replace("{{bedStatus}}", dto.PatientDetails.FirstName ?? "")
                  .Replace("{{generatedDate}}", DateTime.Now.ToString("dd/MM/yyyy HH:mm") ?? "");

            byte[] pdfBytes;
            var props = new ConverterProperties();
            props.SetFontProvider(new DefaultFontProvider(true, true, true));
            props.SetMediaDeviceDescription(new MediaDeviceDescription(MediaType.SCREEN));
            try
            {
                string css = @"
                        body { font-family: Arial, sans-serif; font-size: 14px; color: #111827; margin:0; padding:20px; background:#fff; }
                        h1, h2 { color: #0b5ed7; }
                        table { width: 100%; border-collapse: collapse; font-size: 14px; margin-bottom: 12px; }
                        th, td { padding: 8px; border: 1px solid #e6e9ee; text-align: left; }
                        th { background-color: #f8fafc; font-weight: bold; }
                        .section { margin-top: 18px; }
                        .footer { margin-top: 20px; border-top: 1px dashed #e6e9ee; padding-top: 12px; font-size: 13px; color:#6b7280; }
                        ";

                if (!html.Contains("<style>"))
                {
                    html = html.Replace("</head>", $"<style>{css}</style></head>");
                }

                using (var ms = new MemoryStream())
                {
                    HtmlConverter.ConvertToPdf(html, ms, props);
                    pdfBytes = ms.ToArray();
                }
                return File(pdfBytes, "application/pdf", $"DischargeSummary_{patientId}.pdf");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
