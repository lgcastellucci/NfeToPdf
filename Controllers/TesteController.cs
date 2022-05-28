using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NfeToPdf.EmissoresNFE;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Results;

namespace NfeToPdf.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TesteController : ControllerBase
    {
        private readonly ILogger<ConsultarController> _logger;

        public TesteController(ILogger<ConsultarController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        public IActionResult Get(string prestadorMunicipio, string tomadorCNPJ, string rps)
        {
            var content = "<html><body><h1>Hello World</h1><p>Some text</p></body></html>";
            return new ContentResult()
            {
                Content = content,
                ContentType = "text/html",
            };
            

            PrefBotucatuController.Resposta respostaPref = new();
            PrefBotucatuController pref = new();
            respostaPref = pref.Executar(tomadorCNPJ, rps);

            return File(respostaPref.PdfArrayBytes, "application/pdf", "NFE.pdf");



        }


    }
}
