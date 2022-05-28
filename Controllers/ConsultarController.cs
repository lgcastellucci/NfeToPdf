using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NfeToPdf.EmissoresNFE;
using System.Net.Http;
using System.Text;

namespace NfeToPdf.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConsultarController : ControllerBase
    {
        private readonly ILogger<ConsultarController> _logger;

        public ConsultarController(ILogger<ConsultarController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        public IActionResult Get(string prestadorMunicipio, string tomadorCNPJ, string rps)
        {

            if (string.IsNullOrEmpty(prestadorMunicipio))
            {
                return Retorno("0001");
            }
            if (string.IsNullOrEmpty(tomadorCNPJ))
            {
                return Retorno("0002");
            }
            if (string.IsNullOrEmpty(rps))
            {
                return Retorno("0003");
            }

            if (prestadorMunicipio != "3507506")
            {
                return Retorno("0004");
            }

            if (prestadorMunicipio == "3507506")
            {
                PrefBotucatuController pref = new();
                PrefBotucatuController.Resposta respostaPref = pref.Executar(tomadorCNPJ, rps);

                if (respostaPref.Sucesso)
                {
                    return File(respostaPref.PdfArrayBytes, "application/pdf", "NFE_" + rps + ".pdf");
                }
                else
                {
                    return Retorno("0005");
                }
            }

            return Retorno("0006");
        }

        private static ContentResult Retorno(string codRetorno)
        {
            StringBuilder responseHtml = new();
            responseHtml.Append("<html>");
            responseHtml.Append(" <body>");
            if (codRetorno == "0001")
            {
                responseHtml.Append("  <center>Municipio do prestador é obrigatorio</center><br>");
            }
            else if (codRetorno == "0002")
            {
                responseHtml.Append("  <center>CNPJ do tomador é obrigatorio</center><br>");
            }
            else if (codRetorno == "0003")
            {
                responseHtml.Append("  <center>RPS é obrigatorio</center><br>");
            }
            else if (codRetorno == "0004")
            {
                responseHtml.Append("  <center>Municipio não aceito ou código do IBGE do inválido</center><br>");
                responseHtml.Append("  <center>Caso o código não seja aceito informe a nfetopdf@castellucci.net.br</center><br>");
            }
            else if (codRetorno == "0005")
            {
                responseHtml.Append("  <center>Erro ao consultar NFE</center><br>");
                responseHtml.Append("  <center>Não foi possivel acessar a prefeitura</center><br>");
            }
            else if (codRetorno == "0006")
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

            ContentResult retorno = new()
            {
                StatusCode = 200,
                Content = responseHtml.ToString(),
                ContentType = "text/html"
            };


            return retorno;
        }
    }
}
