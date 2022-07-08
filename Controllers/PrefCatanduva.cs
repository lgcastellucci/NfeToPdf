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
    public class PrefCatanduva : Controller
    {
        readonly string url = "http://catanduva.sp.gov.br:58080/issweb/paginas/public/consulta/autenticidade";
        public Resposta Executar(string codAcesso, string tomadorCNPJ, string nroNota, string codVerificacao)
        {
            Resposta resposta = new Resposta();
            resposta.Sucesso = false;

            HttpService httpService = new HttpService(codAcesso);
            HttpService.Retorno retHttp;

            string cookie = "";
            string userId = "";
            string conteudoPost = "";

            //passo 1 - dando um get na pagins para receber um cookie
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
            }
            
            string viewState = "";  //<input type="hidden" name="javax.faces.ViewState" id="javax.faces.ViewState" value="-2933921452764066748:-2529331640149675970" autocomplete="off" />
            if (retHttp.Body.Contains("javax.faces.ViewState"))
            {
                viewState = "<input type=\"hidden\" name=\"javax.faces.ViewState\" id=\"javax.faces.ViewState\" value=\"";
                if (retHttp.Body.Contains(viewState))
                {
                    viewState = retHttp.Body.Substring(retHttp.Body.IndexOf(viewState) + viewState.Length, 1000);
                    viewState = viewState.Substring(0, viewState.IndexOf("\""));
                }
            }

            //passo 2 = autenticando os dados da nota
            httpService.HeaderAcceptClear();
            httpService.HeaderClear();
            httpService.HeaderAcceptAdd(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            httpService.HeaderAdd("cookie", cookie);
            httpService.UrlSet(url);
            conteudoPost = "";
            conteudoPost += "javax.faces.partial.ajax=true&";
            conteudoPost += "javax.faces.source=formAutenticidade%";
            conteudoPost += "3Aj_idt122&";
            conteudoPost += "javax.faces.partial.execute=%40all&";
            conteudoPost += "javax.faces.partial.render=formAutenticidade&";
            conteudoPost += "formAutenticidade%3Aj_idt122=formAutenticidade%3Aj_idt122&formAutenticidade=formAutenticidade&";
            conteudoPost += "formAutenticidade%3AitCpfCnpj=" + tomadorCNPJ + "&formAutenticidade%3AitNumeroNfse=" + nroNota + "&formAutenticidade%3AsorAutenticacao=V&formAutenticidade%3AitChaveAcesso=" + codVerificacao + "&";
            conteudoPost += "javax.faces.ViewState=" + viewState.Replace(":", "%3A");

            httpService.PayLoadSet(conteudoPost, Encoding.UTF8, "application/x-www-form-urlencoded");
            retHttp = httpService.ExecutePost();
            if ((retHttp.Erro) || (retHttp.HttpStatusCode != HttpStatusCode.OK))
            {
                return resposta;
            }

            if (!retHttp.Body.Contains("Confirmação de Autencidade da NFS-e"))
            {
                return resposta;
            }

            //passo 3 = pegando o pdf da nota
            httpService.HeaderAcceptClear();
            httpService.HeaderClear();
            httpService.HeaderAcceptAdd(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            httpService.HeaderAdd("cookie", cookie);

            request.AddParameter("formAutenticidade", "formAutenticidade");
            request.AddParameter("formAutenticidade:j_idt136", "");
            request.AddParameter("javax.faces.ViewState", "-2933921452764066748:-2529331640149675970");

            
            conteudoPost = "";
            conteudoPost += "AJAXREQUEST=_viewRoot";
            conteudoPost += "&validationNFSEForm=validationNFSEForm";
            conteudoPost += "&validationNFSEForm%3AtypePerson=J";
            conteudoPost += "&validationNFSEForm%3ApersonCNPJ=" + tomadorCNPJ;
            conteudoPost += "&validationNFSEForm%3AverificationCode=" + nroNota;
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
