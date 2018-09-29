using AutoMapper;
using KonohaApi.Models;
using KonohaApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace KonohaApi.DAO
{
    public class ForumDAO : BaseDAO, IDisposable
    {
        #region Topico

        public TopicoViewModel BuscaPorNome(string nome)
        {
            var topico = Db.TopicoDiscucao.FirstOrDefault(x => x.Nome == nome);
            if (topico == null)
                return null;

            var topicoViewModel = Mapper.Map<TopicoDiscucao, TopicoViewModel>(topico);

            return topicoViewModel;
        }

        public string AdicionaTopico(TopicoViewModel topicoViewModel)
        {
            try
            {
                bool evento = Db.Evento.Count(x => x.Id == topicoViewModel.EventoId) > 0;

                if (!evento)
                    throw new Exception("Evento não encotrado");

                var topico = Mapper.Map<TopicoViewModel, TopicoDiscucao>(topicoViewModel);

                Db.TopicoDiscucao.Add(topico);
                Db.SaveChanges();

                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string EditaTopico(TopicoViewModel topicoViewModel)
        {
            try
            {
                var topico = Mapper.Map<TopicoViewModel, TopicoDiscucao>(topicoViewModel);

                Db.Entry(topico).State = EntityState.Modified;
                Db.SaveChanges();
                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string RemoveTopico(int id)
        {
            try
            {
                var topico = Db.TopicoDiscucao.Find(id);

                if (topico == null)
                    throw new Exception("Topico não encotrado.");

                Db.TopicoDiscucao.Remove(topico);
                Db.SaveChanges();

                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public ICollection<TopicoViewModel> BuscaTodosTopicos()
        {
            var topicos = Mapper.Map<ICollection<TopicoDiscucao>, ICollection<TopicoViewModel>>(Db.TopicoDiscucao.ToList());
            return topicos;
        }

        public ICollection<TopicoViewModel> BuscaTopicosDoEvento(int EventoId)
        {
            var topicos = Db.TopicoDiscucao.Where(x => x.EventoId == EventoId).ToList();

            if (topicos == null)
                return null;

            var topicoViewModels = Mapper.Map<ICollection<TopicoDiscucao>, ICollection<TopicoViewModel>>(topicos);

            return topicoViewModels;
        }

        public ICollection<TopicoViewModel> BuscaTopicosDoEventoPorNome(string nomeEvento)
        {
            var topicos = Db.TopicoDiscucao.Where(x => x.Evento.Nome == nomeEvento).ToList();

            if (topicos == null)
                return null;

            var topicoViewModels = Mapper.Map<ICollection<TopicoDiscucao>, ICollection<TopicoViewModel>>(topicos);

            return topicoViewModels;
        }

        #endregion

        #region Comentario
        public string AdicionaComentario(ComentarioViewModel comentarioViewModel)
        {
            try
            {
                bool topico = Db.TopicoDiscucao.Count(x => x.Id == comentarioViewModel.TopicoIDiscucaoId) > 0;

                if (!topico)
                    throw new Exception("Evento não encotrado");
                var comentario = Mapper.Map<ComentarioViewModel, Comentario>(comentarioViewModel);

                Db.Comentario.Add(comentario);
                Db.SaveChanges();

                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string EditaComentario(ComentarioViewModel comentarioViewModel)
        {
            try
            {
                var comentario = Mapper.Map<ComentarioViewModel, Comentario>(comentarioViewModel);

                Db.Entry(comentario).State = EntityState.Modified;
                Db.SaveChanges();
                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string RemoveComentario(int id)
        {
            try
            {

                var comentario = Db.Comentario.Find(id);
                if (comentario == null)
                    throw new Exception("Comentario não encotrado.");

                Db.Comentario.Remove(comentario);
                Db.SaveChanges();
                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public ICollection<ComentarioViewModel> BuscaTodoComentarioPorTopico(int TopicoId)
        {
            var comentarios = Mapper.Map<ICollection<Comentario>, ICollection<ComentarioViewModel>>(Db.Comentario.Where(x => x.TopicoId == TopicoId).ToList());
            return comentarios;
        }

        public ICollection<ComentarioViewModel> BuscaComentariosFilho(int ComentarioPaiId)
        {
            var comentarios = Mapper.Map<ICollection<Comentario>, ICollection<ComentarioViewModel>>(Db.Comentario.Where(x => x.ParentId == ComentarioPaiId).ToList());
            return comentarios;
        }


        #endregion

        public void Dispose()
        {
            Db.Dispose();
        }
    }
}