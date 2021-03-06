﻿using AutoMapper;
using KonohaApi.Models;
using KonohaApi.ViewModels;
using KonohaApi.ViewModels.ModelosDeAjuda;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using static KonohaApi.ViewModels.MandaEmail;

namespace KonohaApi.DAO
{
    public class EventoDAO : BaseDAO, IDisposable
    {
        public string Adicionar(EventoViewModel entity)
        {
            try
            {
                var eventoExiste = Db.Evento.Count(x => x.Nome == entity.Nome && x.AgendaEventoId == entity.AgendaEventoId) > 0;

                if (Db.AgendaEvento.Count(x => x.Id == entity.AgendaEventoId) > 0)
                {
                    if (!eventoExiste)
                    {
                        var eventoModel = Mapper.Map<EventoViewModel, Evento>(entity);

                        string path = HttpContext.Current.Server.MapPath("~/Imagens/Eventos/");

                        var bits = Convert.FromBase64String(eventoModel.PathImagem);

                        string nomeImagem = Guid.NewGuid().ToString() + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".jpg";

                        string imgPath = Path.Combine(path, nomeImagem);

                        File.WriteAllBytes(imgPath, bits);

                        eventoModel.PathImagem = nomeImagem;
                        eventoModel.Cancelado = false;

                        Db.Evento.Add(eventoModel);

                        foreach (var item in entity.Funcionario)
                        {

                            EventoFuncionario e = new EventoFuncionario
                            {
                                EventoId = eventoModel.Id,
                                FuncionarioId = item.Id
                            };

                            Db.EventoFuncionario.Add(e);
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
            var evento = Db.Evento.Find(id);
            if (evento == null)
                return null;
            var eventoViewModel = Mapper.Map<Evento, EventoViewModel>(evento);

            var res = Db.EventoFuncionario.Where(x => x.EventoId == evento.Id).ToList();

            ICollection<Funcionario> lista = new List<Funcionario>();
            foreach (var item in res)
            {
                lista.Add(Db.Funcionario.Include("Usuario").FirstOrDefault(x => x.Id == item.FuncionarioId));
            }
            var funcionarios = Mapper.Map<ICollection<Funcionario>, ICollection<FuncionarioViewModel>>(lista);

            eventoViewModel.Funcionario = funcionarios;

            return eventoViewModel;
        }

        public EventoViewModel BuscaPorNome(string nome)
        {
            var evento = Db.Evento.FirstOrDefault(x => x.Nome == nome);
            if (evento == null)
                return null;

            var eventoViewModel = Mapper.Map<Evento, EventoViewModel>(evento);

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
                string path = HttpContext.Current.Server.MapPath("~/Imagens/Eventos/");

                var bits = Convert.FromBase64String(entity.PathImagem);

                string nomeImagem = Guid.NewGuid().ToString() + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".jpg";

                string imgPath = Path.Combine(path, nomeImagem);

                File.WriteAllBytes(imgPath, bits);

                entity.PathImagem = nomeImagem;

                var eventoModel = entity;

                var eventoSalvo = Db.Evento.FirstOrDefault(x => x.Id == entity.Id);
                var eventosParticipante = Db.ParticipanteEvento.Where(x => x.EventoId == entity.Id).ToList();
    

                if (eventoSalvo.Local != eventoModel.Local ||
                   eventoSalvo.DataInicio != eventoModel.DataInicio)
                {
                        var descricao = "O evento <b>" + eventoSalvo.Nome + "</b>, no qual você está inscrito, teve alteração na sua data e/ou local. Ele agora ocorrerá <b>" +
                            eventoModel.DataInicio.ToLongDateString() + "</b> no(a) <b>" + eventoModel.Local +"</b>. Estaremos esperando por você :)";

                        foreach (var item in eventosParticipante)
                        {
                            var usuario = Db.Usuario.Find(item.ParticipanteId);

                            GmailEmailService gmail = new GmailEmailService();
                            EmailMessage msg = new EmailMessage
                            {
                                Body = $"<html><head> </head> <body>  <form> <h1>Notificação Konoha</h1><h3>Olá {usuario.Nome}</h3><p>{descricao}</p> </form></body> </html>",
                                IsHtml = true,
                                Subject = "Notificação sobre o evento: "+ eventoModel.Nome,
                                ToEmail = usuario.Email
                            };

                        gmail.SendEmailMessage(msg);

                    }

                    }

                var existeFoto = Path.Combine(path, eventoSalvo.PathImagem);

                eventoSalvo.Local = eventoModel.Local;
                eventoSalvo.Nome = eventoModel.Nome;
                eventoSalvo.NumeroVagas = eventoModel.NumeroVagas;
                eventoSalvo.PathImagem = eventoModel.PathImagem;
                eventoSalvo.Descricao = eventoModel.Descricao;
                eventoSalvo.DataInicio = eventoModel.DataInicio;
                eventoSalvo.DataEncerramento = eventoModel.DataEncerramento;
                eventoSalvo.CargaHoraria = eventoModel.CargaHoraria;
                eventoSalvo.Apresentador = eventoModel.Apresentador;
                eventoSalvo.AgendaEventoId = eventoModel.AgendaEventoId;


                foreach (var item in entity.Funcionario)
                {

                    EventoFuncionario e = new EventoFuncionario
                    {
                        EventoId = eventoModel.Id,
                        FuncionarioId = item.Id
                    };

                    var isModerador = Db.EventoFuncionario.Where(x => x.EventoId == eventoModel.Id && x.FuncionarioId == item.Id).ToList();

                    if (isModerador.Count() == 0 )
                    {
                        Db.EventoFuncionario.Add(e);
                        Db.SaveChanges();
                    }
                }


                if (File.Exists(existeFoto))
                    File.Delete(existeFoto);

                //Db.Entry(eventoModel).State = EntityState.Modified;
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

                foreach (var m in Db.ParticipanteEvento.Where(f => f.EventoId == evento.Id).ToList())
                {
                    Db.ParticipanteEvento.Remove(m);
                }
                foreach (var m in Db.EventoFuncionario.Where(f => f.EventoId == evento.Id).ToList())
                {
                    Db.EventoFuncionario.Remove(m);
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
          
                Db.Evento.Attach(eventoSelecionado);
                Db.Entry(eventoSelecionado).State = EntityState.Modified;
                Db.SaveChanges();


                var eventosParticipante = Db.ParticipanteEvento.Where(x => x.EventoId == eventoSelecionado.Id).ToList();

                foreach (var item in eventosParticipante)
                {
                    var usuario = Db.Usuario.Find(item.ParticipanteId);

                    GmailEmailService gmail = new GmailEmailService();
                    EmailMessage msg = new EmailMessage
                    {
                        Body = $"<html><head> </head> <body>  <form> <h1>Aviso</h1><h3>Olá {usuario.Nome}</h3><p>{motivo.Descricao}</p> </form></body> </html>",
                        IsHtml = true,
                        Subject = "O Evento " + eventoSelecionado.Nome + "foi cancelado",
                        ToEmail = usuario.Email
                    };

                    result = gmail.SendEmailMessage(msg);
                }

                return "OK";

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
                var participante = Db.Participante.Find(confimacao.UsuarioId);

                var usuario = Db.Usuario.Find(participante.Id);

                var evento = Db.Evento.Find(confimacao.EventoId);

                var agenda = Db.AgendaEvento.Find(evento.AgendaEventoId);

                if (usuario == null)
                    throw new Exception("Usuario não encontrado.");
                if (evento == null)
                    throw new Exception("Evento não encontrado.");
                if (agenda == null)
                    throw new Exception("Agenda não encontrada.");

                var IsConfimacao = Db.ParticipanteEvento.Count(x => x.ParticipanteId == confimacao.UsuarioId
               && x.EventoId == confimacao.EventoId && x.ConfirmacaoPresenca == true) > 0;

                if (IsConfimacao)
                    throw new Exception("Presença já confimada no evento.");

                var ExisteParticpanteInscrito = Db.ParticipanteEvento.Count(x => x.ParticipanteId == confimacao.UsuarioId
                && x.EventoId == confimacao.EventoId && x.InscricaoPrevia == true) > 0;

                if (ExisteParticpanteInscrito)
                {
                    var eventoIscricao = Db.ParticipanteEvento.FirstOrDefault(x => x.ParticipanteId == confimacao.UsuarioId
                    && x.EventoId == confimacao.EventoId);

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
                throw new Exception("Usuario não inscrito nesse evento.");
            }

            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string CancelarConfimacaoPresenca(ConfimacaoParticipanteEvento confimacao)
        {
            try
            {
                var participante = Db.Participante.Find(confimacao.UsuarioId);

                var usuario = Db.Usuario.Find(participante.Id);

                var evento = Db.Evento.Find(confimacao.EventoId);

                var agenda = Db.AgendaEvento.Find(evento.AgendaEventoId);

                if (usuario == null)
                    throw new Exception("Usuario não encontrado.");
                if (evento == null)
                    throw new Exception("Evento não encontrado.");
                if (agenda == null)
                    throw new Exception("Agenda não encontrada.");

                var ExisteParticpanteInscrito = Db.ParticipanteEvento.Count(x => x.ParticipanteId == confimacao.UsuarioId
                && x.EventoId == confimacao.EventoId && x.InscricaoPrevia == true) > 0;

                if (ExisteParticpanteInscrito)
                {
                    var eventoIscricao = Db.ParticipanteEvento.FirstOrDefault(x => x.ParticipanteId == confimacao.UsuarioId
                    && x.EventoId == confimacao.EventoId);

                    eventoIscricao.ConfirmacaoPresenca = false;
                    Db.Entry(eventoIscricao).State = EntityState.Modified;
                    Db.SaveChanges();

                    GmailEmailService gmail = new GmailEmailService();
                    EmailMessage msg = new EmailMessage
                    {
                        Body = $"<html><head> </head> <body>  <form method='POST'><h1>Aviso</h1><h3>Olá { usuario.Nome}</h3><p>Sua confimarçao de presença no evento {evento.Nome} foi cancelada!</p></form></body> </html>",
                        IsHtml = true,
                        Subject = "Confirmação de presença",
                        ToEmail = usuario.Email
                    };
                    gmail.SendEmailMessage(msg);
                    return "OK";
                }
                throw new Exception("Usuario não inscrito nesse evento.");
            }

            catch (Exception e)
            {
                return e.Message;
            }
        }

        public ICollection<EventoViewModel> EventosModerador(int id)
        {
            var res = Db.EventoFuncionario.Where(x => x.FuncionarioId == id).ToList();
            ICollection<Evento> lista = new List<Evento>();

            foreach (var item in res)
            {
                lista.Add(Db.Evento.Find(item.EventoId));
            }
            var eventos = Mapper.Map<ICollection<Evento>, ICollection<EventoViewModel>>(lista);
            return eventos;
        }

        public ICollection<UsuarioViewModel> ModeradoresEvento(int id)
        {
            var res = Db.EventoFuncionario.Where(x => x.EventoId == id).ToList();
            ICollection<Usuario> lista = new List<Usuario>();

            foreach (var item in res)
            {
                lista.Add(Db.Usuario.Find(item.FuncionarioId));
            }
            var eventos = Mapper.Map<ICollection<Usuario>, ICollection<UsuarioViewModel>>(lista);
            return eventos;
        }

        public bool VerficarNomeEventoExistente(VerificaNomeEvento evento)
        {
            if (evento.IdEvento == 0)
                return Db.Evento.Count(x => x.Nome == evento.NomeEvento.TrimStart().TrimEnd() && x.AgendaEventoId == evento.IdAgenda) > 0; // Create evento
            else
                return Db.Evento.Count(x => x.Nome == evento.NomeEvento && x.Id != evento.IdEvento && x.AgendaEventoId == evento.IdAgenda) > 0; // Edit evento
        }

    }
}