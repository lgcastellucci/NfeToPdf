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
        public HttpResponseMessage Get(string prestadorMunicipio = null, string tomadorCNPJ = null, string rps = null)
        {
            HttpRequest httpRequest = HttpContext.Current.Request;

            Acessos acessos = new Acessos();
            string codAcesso = acessos.Inserir("GET", httpRequest.Url.ToString(), null, null, httpRequest.UserHostAddress);

            if (string.IsNullOrEmpty(prestadorMunicipio) && string.IsNullOrEmpty(tomadorCNPJ) && string.IsNullOrEmpty(rps))
                return Retorno("0001");
            if (string.IsNullOrEmpty(prestadorMunicipio) && string.IsNullOrEmpty(tomadorCNPJ))
                return Retorno("0002");
            if (string.IsNullOrEmpty(prestadorMunicipio) && string.IsNullOrEmpty(rps))
                return Retorno("0003");
            if (string.IsNullOrEmpty(prestadorMunicipio))
                return Retorno("0004");
            if (string.IsNullOrEmpty(tomadorCNPJ) && string.IsNullOrEmpty(rps))
                return Retorno("0005");
            if (string.IsNullOrEmpty(tomadorCNPJ))
                return Retorno("0006");
            if (string.IsNullOrEmpty(rps))
                return Retorno("0007");

            if (prestadorMunicipio != "3507506")
            {
                return Retorno("0008");
            }

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

            if (prestadorMunicipio == "3507506")
            {
                PrefBotucatu pref = new PrefBotucatu();
                Resposta respostaPref = pref.Executar(tomadorCNPJ, rps);

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
                    return Retorno("0009");
                }
            }

            return Retorno("0010");
        }

        private static HttpResponseMessage Retorno(string codRetorno)
        {
            StringBuilder responseHtml = new StringBuilder();
            responseHtml.Append("<html>");
            responseHtml.Append(" <body>");
            if (codRetorno == "0001")
            {
                responseHtml.Append("  <center>Municipio do prestador é obrigatorio</center><br>");
                responseHtml.Append("  <center>CNPJ do tomador é obrigatorio</center><br>");
                responseHtml.Append("  <center>RPS é obrigatorio</center><br>");
            }
            else if (codRetorno == "0002")
            {
                responseHtml.Append("  <center>Municipio do prestador é obrigatorio</center><br>");
                responseHtml.Append("  <center>CNPJ do tomador é obrigatorio</center><br>");
            }
            else if (codRetorno == "0003")
            {
                responseHtml.Append("  <center>Municipio do prestador é obrigatorio</center><br>");
                responseHtml.Append("  <center>RPS é obrigatorio</center><br>");
            }
            else if (codRetorno == "0004")
            {
                responseHtml.Append("  <center>Municipio do prestador é obrigatorio</center><br>");
            }
            else if (codRetorno == "0005")
            {
                responseHtml.Append("  <center>CNPJ do tomador é obrigatorio</center><br>");
                responseHtml.Append("  <center>RPS é obrigatorio</center><br>");
            }
            else if (codRetorno == "0006")
            {
                responseHtml.Append("  <center>CNPJ do tomador é obrigatorio</center><br>");
            }
            else if (codRetorno == "0007")
            {
                responseHtml.Append("  <center>RPS é obrigatorio</center><br>");
            }
            else if (codRetorno == "0008")
            {
                responseHtml.Append("  <center>Municipio não aceito ou código do IBGE do inválido</center><br>");
                responseHtml.Append("  <center>Caso o código não seja aceito informe a nfetopdf@castellucci.net.br</center><br>");
            }
            else if (codRetorno == "0009")
            {
                responseHtml.Append("  <center>Erro ao consultar NFE</center><br>");
                responseHtml.Append("  <center>Não foi possivel acessar a prefeitura</center><br>");
            }
            else if (codRetorno == "0010")
            {
                responseHtml.Append("  <center>Erro ao consultar NFE</center><br>");
                responseHtml.Append("  <center>Contate nfetopdf@castellucci.net.br</center>");
            }
            else
            {
                responseHtml.Append("  <center>Erro ao consultar NFE</center><br>");
                responseHtml.Append("  <center>Contate nfetopdf@castellucci.net.br</center>");
            }

            responseHtml.Append(" </body>");
            responseHtml.Append("</html>");

            var retorno = new HttpResponseMessage();
            retorno.StatusCode = HttpStatusCode.OK;
            retorno.Content = new StringContent(responseHtml.ToString());
            retorno.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return retorno;

        }
    }
}
