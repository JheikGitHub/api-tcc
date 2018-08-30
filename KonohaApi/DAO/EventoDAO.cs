﻿using AutoMapper;
using KonohaApi.Models;
using KonohaApi.ViewModels;
using KonohaApi.ViewModels.ModelosDeAjuda;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using static KonohaApi.ViewModels.MandaEmail;

namespace KonohaApi.DAO
{
    public class EventoDAO : BaseDAO, IDisposable
    {
        public string Adicionar(EventoViewModel entity)
        {
            try
            {
                var eventoExiste = Db.Evento.Count(x => x.Nome == entity.Nome) > 0;

                if (Db.AgendaEvento.Count(x => x.Id == entity.AgendaEventoId) > 0)
                {
                    if (!eventoExiste)
                    {
                        var eventoModel = Mapper.Map<EventoViewModel, Evento>(entity);

                        Db.Evento.Add(eventoModel);
                        Db.SaveChanges();

                        foreach (var item in eventoModel.Funcionario)
                        {
                            eventoModel.Funcionario.Add(item);
                        }
                        Db.SaveChanges();
                        return "OK";
                    }
                    throw new Exception("Evento existente.");
                }
                throw new Exception("Agenda não existe.");
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public EventoViewModel BuscaPorId(int id)
        {
            var eventoViewModel = Mapper.Map<Evento, EventoViewModel>(Db.Evento.Find(id));
            return eventoViewModel;
        }

        public void Dispose()
        {
            Db.Dispose();
        }

        public string Editar(EventoViewModel entity)
        {
            try
            {
                var eventoModel = Mapper.Map<EventoViewModel, Evento>(entity);

                Db.Entry(eventoModel).State = EntityState.Modified;
                Db.SaveChanges();
                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public ICollection<EventoViewModel> ListaTodos()
        {
            var eventos = Db.Evento.Where(x => x.Cancelado == false).ToList();

            var eventoViewModel = Mapper.Map<ICollection<Evento>, ICollection<EventoViewModel>>(eventos);

            return eventoViewModel;
        }

        public string Remove(int id)
        {
            try
            {
                var evento = Db.Evento.Find(id);

                if (evento == null)
                    throw new Exception("Evento não encotrado.");

                foreach (var m in Db.ParticipanteEvento.Where(f => f.EventoId == evento.Id))
                {
                    Db.ParticipanteEvento.Remove(m);
                }

                Db.SaveChanges();

                Db.Evento.Remove(evento);
                Db.SaveChanges();
                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        public ICollection<EventoViewModel> FiltraEventos(string pesquisa)
        {
            var eventosViewModels = Mapper.Map<ICollection<Evento>, ICollection<EventoViewModel>>
                (Db.Evento.Where((x => x.Nome.Contains(pesquisa) || x.TipoEvento.Contains(pesquisa) && x.Cancelado == false)).ToList());

            return eventosViewModels;
        }

        public string CancelarEvento(MotivoCancelamentoEvento motivo)
        {
            try
            {
                bool result = false;
                var eventoSelecionado = Db.Evento.Find(motivo.EventoId);

                if (eventoSelecionado == null)
                    throw new Exception("Evento não encontrado.");

                eventoSelecionado.Cancelado = true;
                var eventoViewModel = Mapper.Map<Evento, EventoViewModel>(eventoSelecionado);

                string resultado = Editar(eventoViewModel);

                if (resultado.Equals("OK"))
                {
                    var eventosParticipante = Db.ParticipanteEvento.Where(x => x.EventoId == eventoSelecionado.Id).ToList();

                    foreach (var item in eventosParticipante)
                    {
                        var usuario = Db.Usuario.Find(item.ParticipanteId);

                        GmailEmailService gmail = new GmailEmailService();
                        EmailMessage msg = new EmailMessage
                        {
                            Body = $"<html><head> </head> <body>  <form> <h1>Aviso</h1><h3>Olá {usuario.Nome}</h3><p>{motivo.Descricao}</p> </form></body> </html>",
                            IsHtml = true,
                            Subject = "Cancelamento de Evento",
                            ToEmail = usuario.Email
                        };

                        result = gmail.SendEmailMessage(msg);
                    }
                }
                if (result)
                    return "OK";

                throw new Exception("Não foi possivel cancelar o evento.");
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public bool AlteraçoesEvento(MotivoCancelamentoEvento motivo)
        {
            bool result = false;
            var eventoSelecionado = Db.Evento.Find(motivo.EventoId);

            var eventosParticipante = Db.ParticipanteEvento.Where(x => x.EventoId == eventoSelecionado.Id).ToList();

            if (eventoSelecionado == null)
                return false;

            foreach (var item in eventosParticipante)
            {
                var usuario = Db.Usuario.Find(item.ParticipanteId);

                GmailEmailService gmail = new GmailEmailService();
                EmailMessage msg = new EmailMessage
                {
                    Body = $"<html><head> </head> <body>  <form> <h1>Aviso</h1><h3>Olá {usuario.Nome}</h3><p>{motivo.Descricao}</p> </form></body> </html>",
                    IsHtml = true,
                    Subject = "Cancelamento de Evento",
                    ToEmail = usuario.Email
                };

                result = gmail.SendEmailMessage(msg);
            }

            if (result)
                return true;

            return false;
        }
        
        public ICollection<ParticipanteViewModel> ListaParticipanteDoEventos(int idEvento)
        {
            ICollection<ParticipanteViewModel> participantes = new List<ParticipanteViewModel>();

            var resultado = Db.ParticipanteEvento.Where(x => x.EventoId == idEvento && x.InscricaoPrevia == true).ToList();

            foreach (var item in resultado)
            {
                var participante = Mapper.Map<Participante, ParticipanteViewModel>(Db.Participante.Find(item.ParticipanteId));
                participantes.Add(participante);
            }

            return participantes;
        }

        public string ConfimacaoPresenca(ConfimacaoParticipanteEvento confimacao)
        {
            try
            {
                var participante = Db.Participante.Find(confimacao.ParticipanteId);

                var usuario = Db.Usuario.Find(participante.Id);

                var evento = Db.Evento.Find(confimacao.EventoId);

                var agenda = Db.AgendaEvento.Find(evento.AgendaEventoId);

                if (usuario == null)
                    throw new Exception("Usuario não encontrado.");
                if (evento == null)
                    throw new Exception("Evento não encontrado.");
                if (agenda == null)
                    throw new Exception("Agenda não encontrada.");

                var ExisteParticpanteInscrito = Db.ParticipanteEvento.Count(x => x.ParticipanteId == confimacao.ParticipanteId && x.EventoId == confimacao.EventoId) > 0;

                if (ExisteParticpanteInscrito)
                {
                    if (participante.CodCarteirinha.Equals(confimacao.CodigoCarteiraEstudantil))
                    {
                        var eventoIscricao = Db.ParticipanteEvento.First(x => x.ParticipanteId == confimacao.ParticipanteId && x.EventoId == confimacao.EventoId);

                        eventoIscricao.ConfirmacaoPresenca = true;
                        Db.Entry(eventoIscricao).State = EntityState.Modified;
                        Db.SaveChanges();

                        GmailEmailService gmail = new GmailEmailService();
                        EmailMessage msg = new EmailMessage
                        {
                            Body = $"<html><head> </head> <body>  <form method='POST'><h1>Aviso</h1><h3>Olá { usuario.Nome}</h3><p>Sua confimarçao de presença no evento {evento.Nome} foi realizada com sucesso!</p><p>Seu certificado ja esta pronto pra download.</p> </form></body> </html>",
                            IsHtml = true,
                            Subject = "Confirmação de presença",
                            ToEmail = usuario.Email
                        };
                        gmail.SendEmailMessage(msg);
                        return "OK";
                    }
                    throw new Exception("Codigo de carteira estudantil não encontrada.");
                }
                throw new Exception("Usuario não inscrito nesse evento.");
            }

            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}