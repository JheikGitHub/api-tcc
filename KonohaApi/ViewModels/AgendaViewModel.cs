using System;
using System.ComponentModel.DataAnnotations;


namespace KonohaApi.ViewModels
{
    public class AgendaViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Required]
        [Display(Name = "Data de inicio")]
        public DateTime DataInicio { get; set; }

        [Required]
        [Display(Name = "Data de encerramento")]
        public DateTime DateEncerramento { get; set; }

        [Required]
        [Display(Name = "Imagem")]
        public string PathImagem { get; set; }

        [Display(Name = "ID da Faculdade")]
        public int FaculdadeId { get; set; }

        [Display(Name = "ID do Funcionario")]
        public int FuncionarioId { get; set; }
    }
}