namespace KonohaApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EventoFuncionario")]
    public partial class EventoFuncionario
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int EventoId { get; set; }

        public int FuncionarioId { get; set; }

        public virtual Evento Evento { get; set; }

        public virtual Funcionario Funcionario { get; set; }
    }
}
