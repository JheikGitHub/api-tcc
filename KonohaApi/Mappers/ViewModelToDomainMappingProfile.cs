using KonohaApi.Models;
using KonohaApi.ViewModels;
using AutoMapper;

namespace KonohaApi.Mappers
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<UsuarioViewModel, Usuario>();
            CreateMap<EventoViewModel, Evento>();
            CreateMap<AgendaViewModel, AgendaEvento>();
            CreateMap<FuncionarioViewModel, Funcionario>();
            CreateMap<ComentarioViewModel, Comentario>();
            CreateMap<TopicoViewModel, TopicoDiscucao>();
        }
    }
}