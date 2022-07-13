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
        public Resposta Executar(string codAcesso, string prestadorCNPJ, string nroNota, string codVerificacao)
        {
            Resposta resposta = new Resposta();
            resposta.Sucesso = false;

            HttpService httpService = new HttpService(codAcesso);
            HttpService.Retorno retHttp;

            string cookie = "";
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
            #endregion

            #region Passo 2 = autenticando os dados da nota
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
            conteudoPost += "formAutenticidade%3AitCpfCnpj=" + prestadorCNPJ + "&formAutenticidade%3AitNumeroNfse=" + nroNota + "&formAutenticidade%3AsorAutenticacao=V&formAutenticidade%3AitChaveAcesso=" + codVerificacao + "&";
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
            #endregion

            #region Passo 3 = pegando o pdf da nota
            httpService.HeaderAcceptClear();
            httpService.HeaderClear();
            httpService.HeaderAcceptAdd(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            httpService.HeaderAdd("cookie", cookie);

            List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("formAutenticidade", "formAutenticidade"));
            keyValues.Add(new KeyValuePair<string, string>("formAutenticidade:j_idt136", ""));
            keyValues.Add(new KeyValuePair<string, string>("javax.faces.ViewState", viewState));
            string urlEncodedString = new FormUrlEncodedContent(keyValues).ReadAsStringAsync().Result;

            httpService.ResultByteSet();
            httpService.PayLoadSet(urlEncodedString, Encoding.UTF8, "application/x-www-form-urlencoded");
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
