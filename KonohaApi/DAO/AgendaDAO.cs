using AutoMapper;
using KonohaApi.Interfaces;
using KonohaApi.Models;
using KonohaApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace KonohaApi.DAO
{
    public class AgendaDAO : BaseDAO, ICrud<AgendaViewModel>, IDisposable
    {
        public string Adicionar(AgendaViewModel entity)
        {
            try
            {
                bool agendaExistente = Db.AgendaEvento.Count(x => x.Nome == entity.Nome) > 0;

                var funcionario = Db.Funcionario.Find(entity.FuncionarioId);

                if (funcionario == null)
                    throw new Exception("funcionario não encontrado.");

                if (agendaExistente)
                    throw new Exception("Agenda ja cadastrada.");

                var agendaModel = Mapper.Map<AgendaViewModel, AgendaEvento>(entity);

                string path = HttpContext.Current.Server.MapPath("~/Imagens/Agenda/");

                var bits = Convert.FromBase64String(agendaModel.PathImagem);

                string nomeImagem = Guid.NewGuid().ToString() + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".jpg";

                string imgPath = Path.Combine(path, nomeImagem);

                File.WriteAllBytes(imgPath, bits);

                agendaModel.FaculdadeId = 1;
                agendaModel.PathImagem = nomeImagem;

                Db.AgendaEvento.Add(agendaModel);
                Db.SaveChanges();

                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public AgendaViewModel BuscaPorId(int id)
        {
            var agenda = Db.AgendaEvento.First(x => x.Id == id);
            var agendaViewModel = Mapper.Map<AgendaEvento, AgendaViewModel>(agenda);

            return agendaViewModel;
        }

       
        public string Editar(AgendaViewModel entity)
        {
            try
            {
                string path = HttpContext.Current.Server.MapPath("~/Imagens/Agenda/");

                var bits = Convert.FromBase64String(entity.PathImagem);

                string nomeImagem = Guid.NewGuid().ToString() + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".jpg";

                string imgPath = Path.Combine(path, nomeImagem);

                File.WriteAllBytes(imgPath, bits);
               
                entity.PathImagem = nomeImagem;
                var agendaModel = Mapper.Map<AgendaViewModel, AgendaEvento>(entity);

                Db.AgendaEvento.Attach(agendaModel);

                var agenda = Db.AgendaEvento.FirstOrDefault(x => x.Id == entity.Id);
                var existeFoto = Path.Combine(path, agenda.PathImagem);

                if (File.Exists(existeFoto))
                    File.Delete(existeFoto);

                Db.Entry(agendaModel).State = EntityState.Modified;
                Db.SaveChanges();
               
                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /*
        public ICollection<AgendaViewModel> ListaTodos(int page, int count)
        {
            var agenda = Db.AgendaEvento.Skip((page - 1)* count).Take(count).ToList();

            var agendaViewModel = Mapper.Map<ICollection<AgendaEvento>, ICollection<AgendaViewModel>>(agenda);

            return agendaViewModel;
        }*/

        public ICollection<EventoViewModel> BuscaEventosDaAgenda(string nomeAgenda)
        {
            ICollection<EventoViewModel> eventos = new Collection<EventoViewModel>();

            var agenda = Db.AgendaEvento.FirstOrDefault(x => x.Nome.ToLower() == nomeAgenda.ToLower());

            if (agenda != null)
            {
                var ev = Db.Evento.Where(x => x.AgendaEventoId == agenda.Id).ToList();
                eventos = Mapper.Map<ICollection<Evento>, ICollection<EventoViewModel>>(ev);

                return eventos;
            }
            return eventos;
        }

        public AgendaViewModel FiltrarAgenda(string nomeAgenda)
        {
            var agendaModel = Db.AgendaEvento.FirstOrDefault(x => x.Nome.ToLower().Equals(nomeAgenda.ToLower()));
            if (agendaModel != null)
            {
                var agendaViewModel = Mapper.Map<AgendaEvento, AgendaViewModel>(agendaModel);
                return agendaViewModel;
            }
            return null;
        }

        public string Remove(int id)
        {
            try
            {
                var AgendaModel = Db.AgendaEvento.Find(id);

                if (AgendaModel == null)
                    throw new Exception("Agenda nao existente.");

                Db.AgendaEvento.Remove(AgendaModel);
                Db.SaveChanges();
                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public bool NomeExistente(string nomeAgenda)
        {
            var AgendaExistente = Db.AgendaEvento.Count(x => x.Nome == nomeAgenda) > 0;

            if (AgendaExistente)
                return true;

            return false;
        }

        public void Dispose()
        {
            Db.Dispose();
        }

        public ICollection<AgendaViewModel> ListaTodos()
        {

            var agendaViewModel = Mapper.Map<ICollection<AgendaEvento>, ICollection<AgendaViewModel>>(Db.AgendaEvento.ToList());

            return agendaViewModel;
        }
    }
}