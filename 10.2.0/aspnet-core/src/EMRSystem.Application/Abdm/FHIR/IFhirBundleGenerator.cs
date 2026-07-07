using Hl7.Fhir.Model;
using System.Threading.Tasks;

namespace EMRSystem.Abdm.FHIR
{
    public interface IFhirBundleGenerator
    {
        /// <summary>
        /// Generates a FHIR Bundle for an Outpatient Prescription.
        /// Includes Patient, Practitioner, Encounter, Condition, and MedicationRequest resources.
        /// </summary>
        Task<Bundle> GeneratePrescriptionBundleAsync(long prescriptionId);

        /// <summary>
        /// Generates a FHIR Bundle for a Lab Report.
        /// Includes Patient, Practitioner, Encounter, DiagnosticReport, and Observation resources.
        /// </summary>
        Task<Bundle> GenerateDiagnosticReportBundleAsync(long labReportReceiptId);
    }
}
