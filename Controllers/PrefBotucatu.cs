using NfeToPdf.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Mvc;

namespace NfeToPdf.Controllers
{
    public class PrefBotucatu : Controller
    {
        readonly string url = "http://botucatu.bsit-br.com.br/nfse/rps-validation.jsf";
        public Resposta Executar(string codAcesso, string prestadorCNPJ, string rpsNumero)
        {
            Resposta resposta = new Resposta();
            resposta.Sucesso = false;

            HttpService httpService = new HttpService(codAcesso);
            HttpService.Retorno retHttp;

            string cookie = "";
            string userId = "";
            string conteudoPost;

            #region Passo 1 - Dar um get na pagina para receber um cookie
            httpService.HeaderAcceptAdd(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            httpService.UrlSet(url);
            retHttp = httpService.ExecuteGet();
            if ((retHttp.Erro) || (retHttp.HttpStatusCode != HttpStatusCode.OK))
            {
                return resposta;
            }

            foreach (KeyValuePair<string, string> header in retHttp.Headers)
            {
                if (header.Key == "Set-Cookie")
                    cookie = header.Value;
                if (header.Key == "userId")
                    userId = header.Value;
            }
            #endregion

            #region Passo 2 -Validando o cookie
            httpService.HeaderAcceptClear();
            httpService.HeaderClear();
            httpService.HeaderAcceptAdd(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            httpService.HeaderAdd("cookie", cookie);
            httpService.HeaderAdd("userId", userId);
            httpService.UrlSet(url);
            conteudoPost = "";
            conteudoPost += "AJAXREQUEST=_viewRoot";
            conteudoPost += "&validationNFSEForm=validationNFSEForm";
            conteudoPost += "&validationNFSEForm%3AtypePerson=J";
            conteudoPost += "&validationNFSEForm%3ApersonCPF=";
            conteudoPost += "&validationNFSEForm%3AverificationCode=";
            conteudoPost += "&javax.faces.ViewState=j_id1";
            conteudoPost += "&validationNFSEForm%3Aj_id24=validationNFSEForm%3Aj_id24&";
            httpService.PayLoadSet(conteudoPost, Encoding.UTF8, "application/x-www-form-urlencoded");
            retHttp = httpService.ExecutePost();
            if ((retHttp.Erro) || (retHttp.HttpStatusCode != HttpStatusCode.OK))
            {
                return resposta;
            }

            if (!retHttp.Body.Contains("INFORMAÇÕES PARA VALIDAÇÃO NFSE"))
            {
                return resposta;
            }
            #endregion

            #region Passo 3 -Validando dados da nota (cnpj e rps)
            httpService.HeaderAcceptClear();
            httpService.HeaderClear();
            httpService.HeaderAcceptAdd(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            httpService.HeaderAdd("cookie", cookie);
            httpService.HeaderAdd("userId", userId);
            conteudoPost = "";
            conteudoPost += "AJAXREQUEST=_viewRoot";
            conteudoPost += "&validationNFSEForm=validationNFSEForm";
            conteudoPost += "&validationNFSEForm%3AtypePerson=J";
            conteudoPost += "&validationNFSEForm%3ApersonCNPJ=" + prestadorCNPJ;
            conteudoPost += "&validationNFSEForm%3AverificationCode=" + rpsNumero;
            conteudoPost += "&javax.faces.ViewState=j_id1";
            conteudoPost += "&validationNFSEForm%3Averify=validationNFSEForm%3Averify&";
            httpService.PayLoadSet(conteudoPost, Encoding.UTF8, "application/x-www-form-urlencoded");
            retHttp = httpService.ExecutePost();
            if ((retHttp.Erro) || (retHttp.HttpStatusCode != HttpStatusCode.OK))
            {
                return resposta;
            }

            if (!retHttp.Body.Contains("Foi encontrada a Nota com os dados informados"))
            {
                return resposta;
            }
            #endregion

            #region Passo 4 = pegando o pdf da nota
            httpService.HeaderAcceptClear();
            httpService.HeaderClear();
            httpService.HeaderAcceptAdd(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            httpService.HeaderAdd("cookie", cookie);
            httpService.HeaderAdd("userId", userId);
            conteudoPost = "";
            conteudoPost += "basicNFSEForm=basicNFSEForm";
            conteudoPost += "&javax.faces.ViewState=j_id1";
            conteudoPost += "&basicNFSEForm%3AnfseGenerate=basicNFSEForm%3AnfseGenerate";
            httpService.PayLoadSet(conteudoPost, Encoding.UTF8, "application/x-www-form-urlencoded");
            httpService.ResultByteSet();
            retHttp = httpService.ExecutePost();
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
