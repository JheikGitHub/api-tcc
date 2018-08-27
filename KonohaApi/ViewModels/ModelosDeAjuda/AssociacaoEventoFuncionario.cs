using System.ComponentModel.DataAnnotations;

namespace KonohaApi.ViewModels
{
    public class AssociacaoEventoFuncionario
    {
        [Required]
        [Display(Name = "IdEvento")]
        public int IdEvento { get; set; }

        [Required]
        [Display(Name = "IdFuncionario")]
        public int IdFuncionario { get; set; }
    }
}