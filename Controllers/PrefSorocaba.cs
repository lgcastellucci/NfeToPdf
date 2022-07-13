using NfeToPdf.Models;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Mvc;

namespace NfeToPdf.Controllers
{
    public class PrefSorocaba : Controller
    {
        //readonly string url = "https://notafiscal.sorocaba.sp.gov.br/notafiscal/paginas/portal/#/autenticidade";
        readonly string url = "https://notafiscal.sorocaba.sp.gov.br/notafiscal-ws/servico/notafiscal/autenticacao";
        public Resposta Executar(string codAcesso, string tomadorCNPJ, string prestadorIM, string nroNota, string codVerificacao)
        {
            Resposta resposta = new Resposta();
            resposta.Sucesso = false;

            HttpService httpService = new HttpService(codAcesso);
            HttpService.Retorno retHttp;

            #region Passo 1 - Na pagina de autenticação já existe um get do PDF
            httpService.UrlSet(url + "/cpfCnpj/" + tomadorCNPJ + "/inscricaoMunicipal/" + prestadorIM + "/numeroNota/" + nroNota + "/codigoVerificacao/" + codVerificacao);
            httpService.ResultByteSet();
            retHttp = httpService.ExecuteGet();
            if ((retHttp.Erro) || (retHttp.HttpStatusCode != HttpStatusCode.OK))
            {
                return resposta;
            }

            if ((Encoding.UTF8.GetString(retHttp.BodyArrayByte).Contains("PDF-1.4")) || (Encoding.UTF8.GetString(retHttp.BodyArrayByte).Contains("PDF-1.5")))
            {
                resposta.Sucesso = true;
                resposta.PdfArrayBytes = retHttp.BodyArrayByte;
                resposta.PdfBase64 = Convert.ToBase64String(retHttp.BodyArrayByte);

                return resposta;
            }
            #endregion


            return resposta;
        }

    }
}
