namespace KonohaApi.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DataContext : DbContext
    {
        public DataContext()
            : base("name=DataContext")
        {
        }

        public virtual DbSet<AgendaEvento> AgendaEvento { get; set; }
        public virtual DbSet<Cidade> Cidade { get; set; }
        public virtual DbSet<Comentario> Comentario { get; set; }
        public virtual DbSet<Estado> Estado { get; set; }
        public virtual DbSet<Evento> Evento { get; set; }
        public virtual DbSet<Faculdade> Faculdade { get; set; }
        public virtual DbSet<Funcionario> Funcionario { get; set; }
        public virtual DbSet<Participante> Participante { get; set; }
        public virtual DbSet<ParticipanteEvento> ParticipanteEvento { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<TopicoDiscucao> TopicoDiscucao { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AgendaEvento>()
                .Property(e => e.Nome)
                .IsUnicode(false);

            modelBuilder.Entity<AgendaEvento>()
                .Property(e => e.Descricao)
                .IsUnicode(false);

            modelBuilder.Entity<AgendaEvento>()
                .Property(e => e.PathImagem)
                .IsUnicode(false);

            modelBuilder.Entity<AgendaEvento>()
                .HasMany(e => e.Evento)
                .WithRequired(e => e.AgendaEvento)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Cidade>()
                .Property(e => e.Nome)
                .IsUnicode(false);

            modelBuilder.Entity<Cidade>()
                .HasMany(e => e.Faculdade)
                .WithRequired(e => e.Cidade)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Comentario>()
                .Property(e => e.Texto)
                .IsUnicode(false);

            modelBuilder.Entity<Comentario>()
                .HasMany(e => e.Comentario1)
                .WithOptional(e => e.Comentario2)
                .HasForeignKey(e => e.ParentId);

            modelBuilder.Entity<Estado>()
                .Property(e => e.Nome)
                .IsUnicode(false);

            modelBuilder.Entity<Estado>()
                .HasMany(e => e.Cidade)
                .WithRequired(e => e.Estado)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Evento>()
                .Property(e => e.Nome)
                .IsUnicode(false);

            modelBuilder.Entity<Evento>()
                .Property(e => e.Descricao)
                .IsUnicode(false);

            modelBuilder.Entity<Evento>()
                .Property(e => e.Local)
                .IsUnicode(false);

            modelBuilder.Entity<Evento>()
                .Property(e => e.Apresentador)
                .IsUnicode(false);

            modelBuilder.Entity<Evento>()
                .Property(e => e.CargaHoraria)
                .IsUnicode(false);

            modelBuilder.Entity<Evento>()
                .Property(e => e.PathImagem)
                .IsUnicode(false);

            modelBuilder.Entity<Evento>()
                .Property(e => e.TipoEvento)
                .IsUnicode(false);

            modelBuilder.Entity<Evento>()
                .HasMany(e => e.ParticipanteEvento)
                .WithRequired(e => e.Evento)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Evento>()
                .HasMany(e => e.TopicoDiscucao)
                .WithRequired(e => e.Evento)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Evento>()
                .HasMany(e => e.Funcionario)
                .WithMany(e => e.Evento)
                .Map(m => m.ToTable("EventoFuncionario").MapLeftKey("EventoId").MapRightKey("FuncionarioId"));

            modelBuilder.Entity<Faculdade>()
                .Property(e => e.Nome)
                .IsUnicode(false);

            modelBuilder.Entity<Faculdade>()
                .Property(e => e.Telefone)
                .IsUnicode(false);

            modelBuilder.Entity<Faculdade>()
                .Property(e => e.Endereco)
                .IsUnicode(false);

            modelBuilder.Entity<Faculdade>()
                .Property(e => e.Cep)
                .IsUnicode(false);

            modelBuilder.Entity<Faculdade>()
                .Property(e => e.Cnpj)
                .IsUnicode(false);

            modelBuilder.Entity<Faculdade>()
                .HasMany(e => e.AgendaEvento)
                .WithRequired(e => e.Faculdade)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Funcionario>()
                .HasMany(e => e.AgendaEvento)
                .WithRequired(e => e.Funcionario)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Participante>()
                .Property(e => e.Matricula)
                .IsUnicode(false);

            modelBuilder.Entity<Participante>()
                .Property(e => e.CodCarteirinha)
                .IsUnicode(false);

            modelBuilder.Entity<Participante>()
                .HasMany(e => e.ParticipanteEvento)
                .WithRequired(e => e.Participante)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ParticipanteEvento>()
                .Property(e => e.CodigoValidacao)
                .IsUnicode(false);

            modelBuilder.Entity<TopicoDiscucao>()
                .Property(e => e.Nome)
                .IsUnicode(false);

            modelBuilder.Entity<TopicoDiscucao>()
                .Property(e => e.Descricao)
                .IsUnicode(false);

            modelBuilder.Entity<TopicoDiscucao>()
                .HasMany(e => e.Comentario)
                .WithRequired(e => e.TopicoDiscucao)
                .HasForeignKey(e => e.TopicoId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Usuario>()
                .Property(e => e.Nome)
                .IsUnicode(false);

            modelBuilder.Entity<Usuario>()
                .Property(e => e.Cpf)
                .IsUnicode(false);

            modelBuilder.Entity<Usuario>()
                .Property(e => e.PathFotoPerfil)
                .IsUnicode(false);

            modelBuilder.Entity<Usuario>()
                .Property(e => e.Genero)
                .IsUnicode(false);

            modelBuilder.Entity<Usuario>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Usuario>()
                .Property(e => e.UserName)
                .IsUnicode(false);

            modelBuilder.Entity<Usuario>()
                .Property(e => e.Senha)
                .IsUnicode(false);

            modelBuilder.Entity<Usuario>()
                .Property(e => e.Perfil)
                .IsUnicode(false);

            modelBuilder.Entity<Usuario>()
                .HasMany(e => e.Comentario)
                .WithRequired(e => e.Usuario)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Usuario>()
                .HasOptional(e => e.Funcionario)
                .WithRequired(e => e.Usuario);

            modelBuilder.Entity<Usuario>()
                .HasOptional(e => e.Participante)
                .WithRequired(e => e.Usuario);
        }
    }
}
