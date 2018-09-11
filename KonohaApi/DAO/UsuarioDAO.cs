using AutoMapper;
using KonohaApi.Interfaces;
using KonohaApi.Models;
using KonohaApi.ViewModels;
using KonohaApi.ViewModels.ModelosDeAjuda;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Web;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;

namespace KonohaApi.DAO
{
    public class UsuarioDAO : BaseDAO, ICrud<UsuarioViewModel>, IDisposable
    {
        #region CRUD usuario

        public string Adicionar(UsuarioViewModel entity)
        {
            bool usuarioExistente = Db.Usuario.Count(x => x.UserName == entity.UserName || x.Cpf == entity.Cpf) > 0;
            try
            {
                if (usuarioExistente)
                    throw new Exception("Usuario ja cadastrado");

                var usuarioModel = Mapper.Map<UsuarioViewModel, Usuario>(entity);
                string path = "C:/KonohaApi/KonohaApi/Imagens/Usuario/";
                string caminhoArquivo = Path.Combine(@path, Path.GetFileName(usuarioModel.PathFotoPerfil));
                File.CreateText(caminhoArquivo);

                usuarioModel.DataCadastro = DateTime.Now;
                usuarioModel.Ativo = true;
                usuarioModel.PathFotoPerfil = caminhoArquivo;
                Db.Usuario.Add(usuarioModel);
                Db.SaveChanges();

                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public UsuarioViewModel BuscaPorId(int id)
        {
            var usuarioViewModel = Mapper.Map<Usuario, UsuarioViewModel>(Db.Usuario.First(x => x.Id == id));

            return usuarioViewModel;
        }

        public string Editar(UsuarioViewModel entity)
        {
            try
            {
                var usuario = Db.Usuario.Count(x => x.UserName == entity.UserName || x.Cpf == entity.Cpf && x.Id != entity.Id) > 1;

                if (usuario)
                    throw new Exception("Usuario ja cadastrado com mesmo UserName ou CPF.");

                var usuarioModel = Mapper.Map<UsuarioViewModel, Usuario>(entity);

                Db.Entry(usuarioModel).State = EntityState.Modified;
                Db.SaveChanges();
                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public ICollection<UsuarioViewModel> ListaTodos()
        {
            var usuario = Db.Usuario.Where(x => x.Ativo == true).ToList();

            var usuarioViewModel = Mapper.Map<ICollection<Usuario>, ICollection<UsuarioViewModel>>(usuario);

            return usuarioViewModel;
        }

        public string Remove(int id)
        {
            try
            {
                var usuarioModel = Db.Usuario.Find(id);

                if (usuarioModel == null)
                    throw new Exception("Usuario não encontrado.");

                Db.Usuario.Remove(usuarioModel);
                Db.SaveChanges();
                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string DesativaUsuario(int id)
        {
            try
            {
                var usuario = Db.Usuario.First(x => x.Id == id);

                if (usuario == null)
                    throw new Exception("Usuario não encontrado.");

                usuario.Ativo = false;
                Db.Entry(usuario).State = EntityState.Modified;
                Db.SaveChanges();
                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        public string AtivaUsuario(int id)
        {

            try
            {
                var usuario = Db.Usuario.First(x => x.Id == id);

                if (usuario == null)
                    throw new Exception("Usuario não encontrado.");

                //caso o usuario esteja ja ativo retornara um false 
                if (usuario.Ativo)
                    throw new Exception("Usuario já esta Ativo.");

                usuario.Ativo = true;
                Db.Entry(usuario).State = EntityState.Modified;
                Db.SaveChanges();

                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        #endregion

        #region complementos

        public UsuarioViewModel GetUser(string username)
        {
            var usuarioViewModel = Mapper.Map<Usuario, UsuarioViewModel>(Db.Usuario.First(x => x.UserName == username));

            return usuarioViewModel;
        }

        public UsuarioViewModel BuscaPorCpf(string cpf)
        {
            var usuarioViewModel = Mapper.Map<Usuario, UsuarioViewModel>(Db.Usuario.First(x => x.Cpf == cpf));

            return usuarioViewModel;
        }

        public string MudaSenha(int id, string senha)
        {

            try
            {
                var usuario = Db.Usuario.Find(id);
                if (usuario == null)
                    throw new Exception("Usuario não encontrado.");

                usuario.Senha = senha;
                Db.Entry(usuario).State = EntityState.Modified;
                Db.SaveChanges();
                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string InscricaoEvento(InscricaoParticipanteEvento model)
        {
            try
            {
                var evento = Db.Evento.Find(model.EventoId);

                var eventoAluno = Db.ParticipanteEvento.Where(x => x.ParticipanteId == model.ParticipanteId).ToList();

                if (evento == null)
                    throw new Exception("Evento não encotrado.");
                else if (evento.NumeroVagas == 0)
                    throw new Exception("Numero de vagas esgotadas");
                else if (evento.Cancelado == true)
                    throw new Exception("Evento cancelado, nâo e possivel realizar a inscrição.");

                foreach (var item in eventoAluno)
                {
                    if (item.Evento.DataInicio == evento.DataInicio)
                        throw new Exception("Não e possivel realizar a inscrição em eventos que occoram na mesma hora.");
                }

                string codigo = AlfanumericoAleatorio(3) + "-" + new Random().Next(00000, 99999).ToString() + "-" + AlfanumericoAleatorio(3);

                ParticipanteEvento alunoEvento = new ParticipanteEvento()
                {
                    InscricaoPrevia = true,
                    ParticipanteId = model.ParticipanteId,
                    EventoId = model.EventoId,
                    CodigoValidacao = codigo
                };

                evento.NumeroVagas = evento.NumeroVagas - 1;
                var eventoViewModel = Mapper.Map<Evento, EventoViewModel>(evento);
                EventoDAO eventoDAO = new EventoDAO();
                eventoDAO.Editar(eventoViewModel);

                Db.ParticipanteEvento.Add(alunoEvento);
                Db.SaveChanges();

                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }


        }

        public string AlfanumericoAleatorio(int tamanho)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, tamanho)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }

        public string CancelaInscricaoEvento(InscricaoParticipanteEvento inscricao)
        {
            try
            {
                var evento = Db.Evento.Find(inscricao.EventoId);
                var participante = Db.Participante.Find(inscricao.ParticipanteId);

                if (evento == null)
                    throw new Exception("evento nao encontrado.");
                if (participante == null)
                    throw new Exception("Particpante não encontrado.");

                var resultado = Db.ParticipanteEvento.First(x => x.ParticipanteId == inscricao.ParticipanteId && x.EventoId == inscricao.EventoId);

                if (resultado == null)
                    throw new Exception("Não a inscricao desse participante nesse evento.");

                resultado.InscricaoPrevia = false;
                Db.Entry(resultado).State = EntityState.Modified;
                Db.SaveChanges();

                evento.NumeroVagas = evento.NumeroVagas + 1;
                var eventoViewModel = Mapper.Map<Evento, EventoViewModel>(evento);

                EventoDAO eventoDAO = new EventoDAO();

                eventoDAO.Editar(eventoViewModel);

                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        public ICollection<EventoViewModel> ListaEventosInscritos(int IdPartipante)
        {
            ICollection<EventoViewModel> eventos = new List<EventoViewModel>();

            var resultado = Db.ParticipanteEvento.Where(x => x.ParticipanteId == IdPartipante && x.InscricaoPrevia == true).ToList();

            foreach (var item in resultado)
            {
                var eventoModel = Db.Evento.Find(item.EventoId);
                var evento = Mapper.Map<Evento, EventoViewModel>(eventoModel);
                eventos.Add(evento);
            }

            return eventos;
        }

        public ICollection<EventoViewModel> ListaEventosComCorfimacaoPresenca(int IdPartipante)
        {
            ICollection<EventoViewModel> eventos = new List<EventoViewModel>();

            var resultado = Db.ParticipanteEvento.Where(x => x.ParticipanteId == IdPartipante && x.ConfirmacaoPresenca == true).ToList();

            foreach (var item in resultado)
            {
                var eventoModel = Db.Evento.Find(item.EventoId);
                var evento = Mapper.Map<Evento, EventoViewModel>(eventoModel);
                eventos.Add(evento);
            }

            return eventos;
        }

        public string GerarPDF(EmitiCertificado certificado)
        {
            var codigo = Db.ParticipanteEvento.FirstOrDefault
                (x => x.ParticipanteId == certificado.UsuarioId && x.EventoId == certificado.EventoId && x.ConfirmacaoPresenca == true);

            var usuario = Db.Usuario.Find(certificado.UsuarioId);

            var participante = Db.Participante.Find(usuario.Id);

            var evento = Db.Evento.Find(certificado.EventoId);

            var agenda = Db.AgendaEvento.Find(evento.AgendaEventoId);

            if (usuario == null)
                return "Usuario não encontrado.";
            if (evento == null)
                return "Evento não encontrado.";
            if (agenda == null)
                return "Agenda não encontrada.";


            //gera certificado
            var doc = new PdfDocument();

            var page = doc.AddPage();

            page.Height = XUnit.FromInch(5.5);

            var graphics = XGraphics.FromPdfPage(page);

            var textFormatter = new PdfSharp.Drawing.Layout.XTextFormatter(graphics);

            var fontCertificado = new XFont("Arial", 30, XFontStyle.Bold);


            graphics.DrawImage(XImage.FromFile(@"C:\KonohaApi\KonohaApi\certificado-backgroung.jpg")
                , 0, 0, page.Width + 60, page.Height);

            textFormatter.DrawString("Certificado Konoha ", fontCertificado, XBrushes.Black,
                new XRect(160, 110, page.Width, page.Height));

            textFormatter.Alignment = PdfSharp.Drawing.Layout.XParagraphAlignment.Justify;
            textFormatter.DrawString($"Certificamos para os devidos fins que {usuario.Nome}, CPF n.º {usuario.Cpf} participou do (a) Evento " +
                $"{evento.Nome} ministrado(a) pelo(a) {evento.Apresentador}, durante a {agenda.Nome},no período de {agenda.DataInicio.Day} a " +
                $"{agenda.DateEncerramento.Day} de {agenda.DataInicio.ToString("MMMM")} de {agenda.DataInicio.Year} com carga horária de {evento.CargaHoraria}"
                , new XFont("Arial", 14, XFontStyle.Regular)
             , XBrushes.Black, new XRect(30, 180, page.Width - 60, page.Height - 60));

            textFormatter.DrawString("Código de validação:", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(397, 370, page.Width - 60, page.Height - 60));
            textFormatter.DrawString(codigo.CodigoValidacao, new XFont("Arial", 10, XFontStyle.Regular), XBrushes.Black, new XRect(500, 370, page.Width - 60, page.Height - 60));
            string caminho = @"C:\KonohaApi\KonohaApi\Certificados\Certificado" + codigo.CodigoValidacao + ".pdf";
            doc.Save(caminho);

            return caminho;

        }

        public void Dispose()
        {
            Db.Dispose();
        }

        #endregion
    }
}