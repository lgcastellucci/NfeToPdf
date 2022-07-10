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
        public Resposta Executar(string codAcesso, string tomadorCNPJ, string rpsNumero)
        {
            Resposta resposta = new Resposta();
            resposta.Sucesso = false;

            HttpService httpService = new HttpService(codAcesso);
            HttpService.Retorno retHttp;

            string cookie = "";
            string userId = "";
            string conteudoPost 

            //passo 1 - Dar um get na pagina para receber um cookie
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


            httpService.HeaderAcceptClear();
            httpService.HeaderClear();
            httpService.HeaderAcceptAdd(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            httpService.HeaderAdd("cookie", cookie);
            httpService.HeaderAdd("userId", userId);
            conteudoPost = "";
            conteudoPost += "AJAXREQUEST=_viewRoot";
            conteudoPost += "&validationNFSEForm=validationNFSEForm";
            conteudoPost += "&validationNFSEForm%3AtypePerson=J";
            conteudoPost += "&validationNFSEForm%3ApersonCNPJ=" + tomadorCNPJ;
            conteudoPost += "&validationNFSEForm%3AverificationCode=" + rpsNumero;
            conteudoPost += "&javax.faces.ViewState=j_id1";
            conteudoPost += "&validationNFSEForm%3Averify=validationNFSEForm%3Averify&";
            httpService.PayLoadSet(conteudoPost, Encoding.UTF8, "application/x-www-form-urlencoded");
            retHttp = httpService.ExecutePost();
            if ((retHttp.Erro) || (retHttp.HttpStatusCode != HttpStatusCode.OK))
            {
                return resposta;
            }

            if (retHttp.Body.Contains("Foi encontrada a Nota com os dados informados"))
            {
                string campoRazaoTomador = "<span id=\"basicNFSEForm:name\" style=\"margin:0px 0px 0px 0px; float: left;\">";
                if (retHttp.Body.Contains(campoRazaoTomador))
                {
                    campoRazaoTomador = retHttp.Body.Substring(retHttp.Body.IndexOf(campoRazaoTomador) + campoRazaoTomador.Length, 100);
                    campoRazaoTomador = campoRazaoTomador.Substring(0, campoRazaoTomador.IndexOf("</span>"));
                }
            }
            else if (retHttp.Body.Contains("É necessário informar o CPF ou CNPJ válido do prestador"))
            {
                return resposta;
            }
            else
            {
                return resposta;
            }


            HttpClient httpClient4 = new HttpClient(new HttpClientHandler { UseCookies = false });
            HttpResponseMessage response4 = null;
            byte[] responseBody4;
            httpClient4.BaseAddress = new Uri(url);
            httpClient4.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            httpClient4.DefaultRequestHeaders.Add("cookie", cookie);
            httpClient4.DefaultRequestHeaders.Add("userId", userId);
            string stringConteudo4 = "";
            stringConteudo4 += "basicNFSEForm=basicNFSEForm";
            stringConteudo4 += "&javax.faces.ViewState=j_id1";
            stringConteudo4 += "&basicNFSEForm%3AnfseGenerate=basicNFSEForm%3AnfseGenerate";
            StringContent stringContent4 = new StringContent(stringConteudo4, Encoding.UTF8, "application/x-www-form-urlencoded");
            try
            {
                response4 = httpClient4.PostAsync(url, stringContent4).Result;
                responseBody4 = response4.Content.ReadAsByteArrayAsync().Result;
            }
            catch (Exception ex)
            {
                string messageException = "";
                messageException = messageException + "Message --- " + ex.Message + "\r\n";
                messageException = messageException + "HelpLink --- " + ex.HelpLink + "\r\n";
                messageException = messageException + "Source --- " + ex.Source + "\r\n";
                messageException = messageException + "StackTrace --- " + ex.StackTrace + "\r\n";
                messageException = messageException + "TargetSite --- " + ex.TargetSite + "\r\n";

                //RegistraLogService.Log(messageException);
                return resposta;
            }

            if (Encoding.UTF8.GetString(responseBody4).Contains("PDF-1.4"))
            {
                resposta.Sucesso = true;
                resposta.PdfArrayBytes = responseBody4;
                resposta.PdfBase64 = Convert.ToBase64String(responseBody4);

                return resposta;
            }
            else
            {

                //RegistraLogService.Log(Encoding.UTF8.GetString(responseBody4));
                return resposta;
            }

            return resposta;
        }

    }
}
