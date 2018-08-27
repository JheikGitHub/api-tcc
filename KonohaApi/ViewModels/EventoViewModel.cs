using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KonohaApi.ViewModels
{
    public class EventoViewModel
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nome do evento")]
        public string Nome { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Required]
        [Display(Name = "Local")]
        public string Local { get; set; }

        [Required]
        [Display(Name = "Data de inicio")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date, ErrorMessage = "Data em formato inválido")]
        public DateTime DataInicio { get; set; }

        [Required]
        [Display(Name = "Data de encerramento")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date, ErrorMessage = "Data em formato inválido")]
        public DateTime DataEncerramento { get; set; }

        [Required]
        [Display(Name = "Apresentador")]
        public string Apresentador { get; set; }

        [Required]
        [Display(Name = "Carga horária")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date, ErrorMessage = "Data em formato inválido")]
        public DateTime CargaHoraria { get; set; }

        [Required]
        [Display(Name = "Número")]
        public int NumeroVagas { get; set; }

        [Required]
        [Display(Name = "Imagem")]
        public string PathImagem { get; set; }

        [Required]
        [Display(Name = "Tipo do evento")]
        public string TipoEvento { get; set; }

        [Required]
        [Display(Name = "Evento Cancelado")]
        public bool Cancelado { get; set; }

        [Required]
        [Display(Name = "Agenda de eventos")]
        public int AgendaEventoId { get; set; }

        public ICollection<FuncionarioViewModel> Funcionario { get; set; }
    }
}