using System.ComponentModel.DataAnnotations;

namespace KonohaApi.ViewModels
{
    public class TopicoViewModel
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Required]
        [Display(Name = "Id do evento")]
        public int EventoId { get; set; }
    }
}