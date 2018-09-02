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

namespace KonohaApi.DAO
{
    public class AgendaDAO : BaseDAO, ICrud<AgendaViewModel>, IDisposable
    {
        public string Adicionar(AgendaViewModel entity)
        {
            try
            {
                bool agendaExistente = Db.AgendaEvento.Count(x => x.Nome == entity.Nome) > 0;

                if ((Db.Funcionario.Count(x => x.Id == entity.FuncionarioId) > 0)
                    && (Db.Faculdade.Count(x => x.Id == entity.FaculdadeId) > 0))
                    throw new Exception("Funcionario ou Faculdade não existente.");

                if (agendaExistente)
                    throw new Exception("Agenda ja cadastrada.");

                var agendaModel = Mapper.Map<AgendaViewModel, AgendaEvento>(entity);

                //converter base64 em imagem 

                byte[] imageBytes = Convert.FromBase64String(agendaModel.PathImagem);

                MemoryStream ms = new MemoryStream(imageBytes, 0,
                  imageBytes.Length);

                ms.Write(imageBytes, 0, imageBytes.Length);
                Image image = Image.FromStream(ms, true);

                var codigo = Guid.NewGuid();
                string caminho = @"C:\KonohaApi\KonohaApi\Imagens\Agenda\" + codigo;

                image.Save(caminho);

                agendaModel.PathImagem = caminho;
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
                var usuario = Db.Usuario.Count(x => x.Nome == entity.Nome && x.Id != entity.Id) > 0;

                if (usuario)
                    throw new Exception("Usuario ja cadastrado com mesmo UserName ou CPF.");

                var agendaModel = Mapper.Map<AgendaViewModel, AgendaEvento>(entity);

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

            var agenda = Db.AgendaEvento.First(x => x.Nome == nomeAgenda);

            if (agenda != null)
            {
                eventos = Mapper.Map<ICollection<Evento>, ICollection<EventoViewModel>>(agenda.Evento.ToList());

                return eventos;
            }
            return eventos;
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