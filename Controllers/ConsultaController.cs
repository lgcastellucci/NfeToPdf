using NfeToPdf.Models;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;

namespace NfeToPdf.Controllers
{
    public class ConsultaController : ApiController
    {
        /// <summary>
        /// Classe utilizada retornar o PDF da Nfe
        /// </summary>
        public HttpResponseMessage Get(string prestadorMunicipio = null, string tomadorCNPJ = null, string rps = null, string nroNota = null, string codVerificacao = null)
        {
            HttpRequest httpRequest = HttpContext.Current.Request;

            Acessos acessos = new Acessos();
            string codAcesso = acessos.Inserir("GET", httpRequest.Url.ToString(), null, null, httpRequest.UserHostAddress);

            #region Validando parametros obrigatórios para cada prefeitura
            if (prestadorMunicipio == "3507506") //botucatu
            {
                StringBuilder msgCamposObrigatorios = new StringBuilder();
                if (string.IsNullOrEmpty(prestadorMunicipio))
                    msgCamposObrigatorios.AppendLine(msgRetorno("0001"));
                if (string.IsNullOrEmpty(tomadorCNPJ))
                    msgCamposObrigatorios.AppendLine(msgRetorno("0002"));
                if (string.IsNullOrEmpty(rps))
                    msgCamposObrigatorios.AppendLine(msgRetorno("0003"));

                if (!string.IsNullOrEmpty(msgCamposObrigatorios.ToString()))
                    return Retorno(msgCamposObrigatorios.ToString());
            }
            else if (prestadorMunicipio == "3511102") //catanduca
            {
                StringBuilder msgCamposObrigatorios = new StringBuilder();
                if (string.IsNullOrEmpty(prestadorMunicipio))
                    msgCamposObrigatorios.AppendLine(msgRetorno("0001"));
                if (string.IsNullOrEmpty(tomadorCNPJ))
                    msgCamposObrigatorios.AppendLine(msgRetorno("0002"));
                if (string.IsNullOrEmpty(nroNota))
                    msgCamposObrigatorios.AppendLine(msgRetorno("0004"));
                if (string.IsNullOrEmpty(codVerificacao))
                    msgCamposObrigatorios.AppendLine(msgRetorno("0005"));

                if (!string.IsNullOrEmpty(msgCamposObrigatorios.ToString()))
                    return Retorno(msgCamposObrigatorios.ToString());
            }
            else
            {
                return Retorno(msgRetorno("0006"));
            }
            #endregion

            #region Consultas de modelo
            if ((prestadorMunicipio == "3507506") && tomadorCNPJ == "12345678901234" && (rps == "123456"))
            {
                var retorno = new HttpResponseMessage();
                retorno.StatusCode = HttpStatusCode.NotFound;

                FileStream fileStream = new FileStream(HttpContext.Current.Server.MapPath("~/Arquivos/NfeToPdf.pdf"), FileMode.Open, FileAccess.Read);
                try
                {
                    int length = (int)fileStream.Length;  // get file length
                    byte[] buffer = new byte[length];     // create buffer
                    int count;                            // actual number of bytes read
                    int sum = 0;                          // total number of bytes read

                    // read until Read method returns 0 (end of the stream has been reached)
                    while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                        sum += count;  // sum is a buffer offset for next reading

                    retorno.StatusCode = HttpStatusCode.OK;
                    retorno.Content = new ByteArrayContent(buffer);
                    retorno.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                }
                finally
                {
                    fileStream.Close();
                }

                return retorno;
            }

            if ((prestadorMunicipio == "3511102") && tomadorCNPJ == "12345678901234" && (nroNota == "123456") && (codVerificacao == "123KEY123"))
            {
                var retorno = new HttpResponseMessage();
                retorno.StatusCode = HttpStatusCode.NotFound;

                FileStream fileStream = new FileStream(HttpContext.Current.Server.MapPath("~/Arquivos/NfeToPdf.pdf"), FileMode.Open, FileAccess.Read);
                try
                {
                    int length = (int)fileStream.Length;  // get file length
                    byte[] buffer = new byte[length];     // create buffer
                    int count;                            // actual number of bytes read
                    int sum = 0;                          // total number of bytes read

                    // read until Read method returns 0 (end of the stream has been reached)
                    while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                        sum += count;  // sum is a buffer offset for next reading

                    retorno.StatusCode = HttpStatusCode.OK;
                    retorno.Content = new ByteArrayContent(buffer);
                    retorno.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                }
                finally
                {
                    fileStream.Close();
                }

                return retorno;
            }
            #endregion

            if (prestadorMunicipio == "3507506")
            {
                PrefBotucatu pref = new PrefBotucatu();
                Resposta respostaPref = pref.Executar(codAcesso, tomadorCNPJ, rps);

                if (respostaPref.Sucesso)
                {
                    var retorno = new HttpResponseMessage();
                    retorno.StatusCode = HttpStatusCode.OK;
                    retorno.Content = new ByteArrayContent(respostaPref.PdfArrayBytes);
                    retorno.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                    return retorno;
                }
                else
                {
                    return Retorno("0007");
                }
            }
            else if (prestadorMunicipio == "3511102")
            {
                PrefCatanduva pref = new PrefCatanduva();
                Resposta respostaPref = pref.Executar(codAcesso, tomadorCNPJ, nroNota, codVerificacao);

                if (respostaPref.Sucesso)
                {
                    var retorno = new HttpResponseMessage();
                    retorno.StatusCode = HttpStatusCode.OK;
                    retorno.Content = new ByteArrayContent(respostaPref.PdfArrayBytes);
                    retorno.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                    return retorno;
                }
                else
                {
                    return Retorno("0008");
                }
            }

            return Retorno("0009");
        }

        private static HttpResponseMessage Retorno(string conteudo)
        {
            StringBuilder responseHtml = new StringBuilder();
            responseHtml.Append("<html>");
            responseHtml.Append(" <body>");
            responseHtml.Append(conteudo);
            responseHtml.Append(" </body>");
            responseHtml.Append("</html>");

            var retorno = new HttpResponseMessage();
            retorno.StatusCode = HttpStatusCode.OK;
            retorno.Content = new StringContent(responseHtml.ToString());
            retorno.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return retorno;

        }

        private static string msgRetorno(string codRetorno)
        {
            if (codRetorno == "0001")
            {
                return "<center>Municipio do prestador é obrigatorio</center><br>";
            }
            else if (codRetorno == "0002")
            {
                return "<center>CNPJ do tomador é obrigatorio</center><br>";
            }
            else if (codRetorno == "0003")
            {
                return "<center>RPS é obrigatorio</center><br>";
            }
            else if (codRetorno == "0004")
            {
                return "<center>Numero da NF é obrigatorio</center><br>";
            }
            else if (codRetorno == "0005")
            {
                return "<center>Codigo de verificação é obrigatorio</center><br>";
            }
            else if (codRetorno == "0006")
            {
                return "  <center>Municipio não aceito ou código do IBGE do inválido</center><br>" +
                       "  <center>Caso o código não seja aceito informe a nfetopdf@castellucci.net.br</center><br>";
            }
            else if (codRetorno == "0007")
            {
                return "  <center>Erro ao consultar NFE</center><br>" +
                       "  <center>Não foi possivel acessar a prefeitura</center><br>";
            }
            else if (codRetorno == "0008")
            {
                return "  <center>Erro ao consultar NFE</center><br>" +
                       "  <center>Contate nfetopdf@castellucci.net.br</center>";
            }
            else
            {
                return "  <center>Erro ao consultar NFE</center><br>" +
                       "  <center>Contate nfetopdf@castellucci.net.br</center>";
            }
        }
    }
}
