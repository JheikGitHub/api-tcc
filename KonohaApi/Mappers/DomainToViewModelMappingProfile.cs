using KonohaApi.Models;
using KonohaApi.ViewModels;
using AutoMapper;

namespace KonohaApi.Mappers
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<Usuario, UsuarioViewModel>();
            CreateMap<Evento, EventoViewModel>();
            CreateMap<Funcionario, FuncionarioViewModel>();
            CreateMap<AgendaEvento, AgendaViewModel>();
            CreateMap<Comentario, ComentarioViewModel>();
            CreateMap<TopicoDiscucao, TopicoViewModel>();

        }

    }
}