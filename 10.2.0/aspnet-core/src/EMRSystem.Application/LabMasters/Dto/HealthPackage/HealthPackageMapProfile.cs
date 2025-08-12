// HealthPackageMapProfile.cs
using AutoMapper;
using EMRSystem.LabMasters;
using EMRSystem.LabMasters.Dto.HealthPackage;
using EMRSystem.LabReportsTypes;
using EMRSystem.LabReportsType.Dto;   // adjust if your DTO namespace differs
using EMRSystem.LabMasters.Dto.LabTest;
using System.Linq;
using System.Collections.Generic;

namespace EMRSystem.LabMasters.Dto.HealthPackage
{
    public class HealthPackageMapProfile : Profile
    {
        public HealthPackageMapProfile()
        {
            // 1) LabTest -> LabTestDto (element mapping)
            CreateMap<EMRSystem.LabMasters.LabTest, LabTestDto>();

            // 2) Map LabReportTypeItem -> LabTestDto so ReportTypeItems -> Tests works
            CreateMap<EMRSystem.LabMasters.LabReportTypeItem, LabTestDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.LabTest != null ? src.LabTest.Id : 0))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.LabTest != null ? src.LabTest.Name : string.Empty));

            // 3) LabReportsType -> LabReportsTypeDto, map ReportTypeItems -> Tests
            CreateMap<EMRSystem.LabReportsTypes.LabReportsType, LabReportsTypeDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.ReportType, opt => opt.MapFrom(s => s.ReportType))
                .ForMember(d => d.ReportPrice, opt => opt.MapFrom(s => s.ReportPrice))
                // AutoMapper will map each ReportTypeItem -> LabTestDto using map defined above
                .ForMember(d => d.Tests, opt => opt.MapFrom(s => s.ReportTypeItems ?? new List<EMRSystem.LabMasters.LabReportTypeItem>()));

            // 4) Map join-entity -> DTO (includes LabReportsTypeDto)
            CreateMap<HealthPackageLabReportsType, HealthPackageLabReportsTypeDto>()
                .ForMember(d => d.LabReportsType, opt => opt.MapFrom(src => src.LabReportsType))
                .ForMember(d => d.LabReportsTypeId, opt => opt.MapFrom(src => src.LabReportsTypeId))
                .ForMember(d => d.HealthPackageId, opt => opt.MapFrom(src => src.HealthPackageId))
                .ForMember(d => d.TenantId, opt => opt.MapFrom(src => src.TenantId));

            // 5) HealthPackage -> HealthPackageDto (single CreateMap; map both collections here)
            CreateMap<EMRSystem.LabMasters.HealthPackage, HealthPackageDto>()
                // map join-entities to join-DTOs (each will include LabReportsTypeDto)
                .ForMember(dest => dest.LabReportsTypes,
                           opt => opt.MapFrom(src => src.PackageReportTypes ?? new List<HealthPackageLabReportsType>()))
                // map a simple list of IDs extracted from join-entities
                .ForMember(dest => dest.LabReportsTypeIds,
                           opt => opt.MapFrom(src =>
                               src.PackageReportTypes == null
                                   ? new List<long>()
                                   : src.PackageReportTypes.Select(pr => pr.LabReportsTypeId).ToList()
                           ));

            // 6) Create/Update mappings (ignore navigation collection during mapping from DTO -> entity)
            CreateMap<CreateUpdateHealthPackageDto, EMRSystem.LabMasters.HealthPackage>()
                .ForMember(dest => dest.PackageReportTypes, opt => opt.Ignore());

            CreateMap<CreateUpdateHealthPackageLabReportsTypeDto, HealthPackageLabReportsType>()
                .ForMember(d => d.LabReportsTypeId, opt => opt.MapFrom(s => s.LabReportsTypeId))
                .ForMember(d => d.TenantId, opt => opt.MapFrom(s => s.TenantId));
        }
    }
}
