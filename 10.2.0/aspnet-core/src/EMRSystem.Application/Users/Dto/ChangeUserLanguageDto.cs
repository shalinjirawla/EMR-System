using System.ComponentModel.DataAnnotations;

namespace EMRSystem.Users.Dto;

public class ChangeUserLanguageDto
{
    [Required]
    public string LanguageName { get; set; }
}