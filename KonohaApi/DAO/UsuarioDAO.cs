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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

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
                    throw new Exception("Usuario ja cadastrado com mesmo e-mail ou CPF");

                var usuarioModel = Mapper.Map<UsuarioViewModel, Usuario>(entity);

                if (usuarioModel.PathFotoPerfil != "")
                {
                    string path = HttpContext.Current.Server.MapPath("~/Imagens/Usuario/");

                    var bits = Convert.FromBase64String(usuarioModel.PathFotoPerfil);

                    string nomeImagem = Guid.NewGuid().ToString() + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".jpg";

                    string imgPath = Path.Combine(path, nomeImagem);

                    File.WriteAllBytes(imgPath, bits);

                    usuarioModel.PathFotoPerfil = nomeImagem;
                }

                usuarioModel.DataCadastro = DateTime.Now;
                usuarioModel.Ativo = true;

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
                var usuarioModel = Mapper.Map<UsuarioViewModel, Usuario>(entity);

                Db.Usuario.Attach(usuarioModel);

                var usuarioExists = Db.Usuario.Count(x => x.UserName == entity.UserName || x.Cpf == entity.Cpf && x.Id != entity.Id) > 1;

                if (usuarioExists)
                    throw new Exception("Usuario ja cadastrado com mesmo UserName ou CPF.");

                Db.Entry(usuarioModel).State = EntityState.Modified;
                Db.SaveChanges();
                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string AltararFoto(int idUsuario, string pathFoto)
        {
            try
            {
                var usuario = Db.Usuario.Find(idUsuario);

                if (pathFoto != "")
                {
                    string path = HttpContext.Current.Server.MapPath("~/Imagens/Usuario/");

                    var bits = Convert.FromBase64String(pathFoto);

                    string nomeImagem = Guid.NewGuid().ToString() + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".jpg";

                    string imgPath = Path.Combine(path, nomeImagem);

                    File.WriteAllBytes(imgPath, bits);
                    var existeFoto = Path.Combine(path, usuario.PathFotoPerfil);

                    if (File.Exists(existeFoto))
                        File.Delete(existeFoto);

                    usuario.PathFotoPerfil = nomeImagem;
                }
                else
                {
                    string path = HttpContext.Current.Server.MapPath("~/Imagens/Usuario/");
                    var existeFoto = Path.Combine(path, usuario.PathFotoPerfil);

                    if (File.Exists(existeFoto))
                        File.Delete(existeFoto);

                    usuario.PathFotoPerfil = pathFoto;
                }
                Db.Entry(usuario).State = EntityState.Modified;
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
            var usuario = Db.Usuario.ToList();

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

                if(usuarioModel.Perfil.ToLower() == "participante" || usuarioModel.Perfil.ToLower() == "aluno")
                {
                    var participante = Db.Usuario.Include("Participante").Single(x => x.Id == id);
                    Db.Usuario.Remove(participante);
                }
                else
                {
                    var funcionario = Db.Usuario.Include("Funcionario").Single(x => x.Id == id);
                    Db.Usuario.Remove(funcionario);
                }

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
            var usuarioViewModel = Mapper.Map<Usuario, UsuarioViewModel>(Db.Usuario.FirstOrDefault(x => x.UserName == username));

            return usuarioViewModel;
        }

        public UsuarioViewModel BuscaPorCpf(MudaSenhaCpf Cpf)
        {
            var usuarioModel = Db.Usuario.FirstOrDefault(x => x.Cpf == Cpf.Cpf);
            if (usuarioModel == null)
                return null;

            var usuarioViewModel = Mapper.Map<Usuario, UsuarioViewModel>(usuarioModel);

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

                string codigo = CodigoUnico();

                ParticipanteEvento alunoEvento = new ParticipanteEvento()
                {
                    InscricaoPrevia = true,
                    ConfirmacaoPresenca = false,
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
            byte[] data = new byte[tamanho];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(tamanho);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        private string CodigoUnico()
        {
           string codigo = AlfanumericoAleatorio(3) + "-" + new Random().Next(00000, 99999).ToString() + "-" + AlfanumericoAleatorio(3);

            if (Db.ParticipanteEvento.Count(x => x.CodigoValidacao == codigo) > 0)
                CodigoUnico();

           return codigo;
        }

        public bool VerificarInscricao(InscricaoParticipanteEvento inscricao)
        {
            return Db.ParticipanteEvento.Count(x => x.ParticipanteId == inscricao.ParticipanteId && x.EventoId == inscricao.EventoId) > 0;
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


                Db.ParticipanteEvento.Remove(resultado);
                Db.SaveChanges();

                evento.NumeroVagas = evento.NumeroVagas + 1;
                var eventoViewModel = Mapper.Map<Evento, EventoViewModel>(evento);

                EventoDAO eventoDAO = new EventoDAO();

                eventoDAO.Editar(eventoViewModel);
                Db.SaveChanges();
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
                var eventoModel = Db.Evento.Include("AgendaEvento").FirstOrDefault(x => x.Id == item.EventoId);
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


            graphics.DrawImage(XImage.FromFile(@"C:\Users\Markim\Documents\KonohaAPI\KonohaApi\bg.png")
                , 0, 0, page.Width + 60, page.Height);


            textFormatter.DrawString("Certificado Konoha ", fontCertificado, XBrushes.Black,
                new XRect(160, 110, page.Width, page.Height));

            textFormatter.Alignment = PdfSharp.Drawing.Layout.XParagraphAlignment.Justify;
            textFormatter.DrawString($"Certificamos para os devidos fins que {usuario.Nome}, CPF n.º {usuario.Cpf} participou do (a) {evento.TipoEvento} " +
                $"'{evento.Nome}' ministrado(a) pelo(a) {evento.Apresentador}, durante a {agenda.Nome}, no período de {agenda.DataInicio.Day} a " +
                $"{agenda.DateEncerramento.Day} de {agenda.DataInicio.ToString("MMMM")} de {agenda.DataInicio.Year} com carga horária de {evento.CargaHoraria} horas."
                , new XFont("Arial", 14, XFontStyle.Regular)
             , XBrushes.Black, new XRect(30, 180, page.Width - 60, page.Height - 60));

            textFormatter.DrawString("Código de validação:", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(397, 370, page.Width - 60, page.Height - 60));
            textFormatter.DrawString(codigo.CodigoValidacao, new XFont("Arial", 10, XFontStyle.Regular), XBrushes.Black, new XRect(500, 370, page.Width - 60, page.Height - 60));
            string caminho = @"C:\Users\Markim\Documents\KonohaAPI\KonohaApi\Certificados\Certificado" + codigo.CodigoValidacao + ".pdf";
            doc.Save(caminho);

            return caminho;

        }

        public ParticipanteEventoViewModel VerficarCodigoCertificado(string codigo)
        {
            var inscrição = Db.ParticipanteEvento.FirstOrDefault(x => x.CodigoValidacao == codigo && x.ConfirmacaoPresenca == true);

            if (inscrição == null)
                return null;

            var inscricaoViewModel = Mapper.Map<ParticipanteEvento, ParticipanteEventoViewModel>(inscrição);

            return inscricaoViewModel;
        }

        public ConfirmacaoPresencaValidacao BuscarUsuarioParaConfirmacaoPresenca(DadosConfirmacaoPresencaWeb dados)
        {
            ConfirmacaoPresencaValidacao result = new ConfirmacaoPresencaValidacao();

            var user = Db.Usuario.FirstOrDefault(x => x.Cpf == dados.CpfUsuario);
            var evento = Db.Evento.FirstOrDefault(x => x.Id == dados.IdEvento);

            if(user == null)
            {
                return null;
            }

            var presencasConfirmadasParticipante = Db.ParticipanteEvento.Where(x => x.ConfirmacaoPresenca == true && x.ParticipanteId == user.Id).ToList();

            var usuarioViewModel = Mapper.Map<Usuario, UsuarioViewModel>(user);

            result.Usuario = usuarioViewModel;
            result.StatusConfirmacao = "Ok";

            if(Db.ParticipanteEvento.Count(x => x.ParticipanteId == user.Id && x.EventoId == dados.IdEvento && x.ConfirmacaoPresenca == true) > 0)
            {
                result.StatusConfirmacao = "FE6";//Participante já teve sua presença confirmada neste evento
            }
            else
            if(presencasConfirmadasParticipante.Count() > 0)
            {
                foreach (var item in presencasConfirmadasParticipante)
                {
                    if((evento.DataInicio >= item.Evento.DataInicio && evento.DataInicio <= item.Evento.DataEncerramento) ||
                       (evento.DataEncerramento >= item.Evento.DataInicio && evento.DataEncerramento <= item.Evento.DataEncerramento))
                    {
                        result.StatusConfirmacao = "FE5"; //Participante já teve presença confirmada em outro que ocorreu no mesmo periodo
                    }
                }
            }

            return result;

        }

        public void Dispose()
        {
            Db.Dispose();
        }

        #endregion
    }
}