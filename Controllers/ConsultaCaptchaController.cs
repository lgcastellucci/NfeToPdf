using NfeToPdf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;

namespace NfeToPdf.Controllers
{
    public class ConsultaCaptchaController : ApiController
    {
        /// <summary>
        /// Classe utilizada retornar o PDF da Nfe
        /// </summary>
        public HttpResponseMessage Get(int imgDiferentes = 1, int imgIguais = 1)
        {
            HttpService httpService = new HttpService("");
            HttpService.Retorno retHttp;

            string ViewState = "";
            string ViewStateGenerator = "";
            string ViewStateValidation = "";
            string Cookie = "";

            int imgDiferentesGeradas = 0;
            while (imgDiferentesGeradas < imgDiferentes)
            {

                string url = "https://piracicaba.simplissweb.com.br/contrib/app/nfse/relatorio";
                httpService.HeaderAcceptAdd(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                httpService.HeaderClear();
                httpService.ResultStringSet();
                httpService.UrlSet(url);
                retHttp = httpService.ExecuteGet();
                if ((retHttp.Erro) || (retHttp.HttpStatusCode != HttpStatusCode.OK))
                {
                    return Retorno("0001");
                }

                foreach (KeyValuePair<string, string> header in retHttp.Headers)
                {
                    if (header.Key == "Set-Cookie")
                        Cookie = header.Value;
                }

                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(retHttp.Body);
                foreach (HtmlAgilityPack.HtmlNode input in doc.DocumentNode.SelectNodes("//input"))
                {
                    if (input.Attributes["name"] != null && input.Attributes["name"].Value == "__VIEWSTATE")
                        ViewState = input.Attributes["value"].Value;
                    if (input.Attributes["name"] != null && input.Attributes["name"].Value == "__VIEWSTATEGENERATOR")
                        ViewStateGenerator = input.Attributes["value"].Value;
                    if (input.Attributes["name"] != null && input.Attributes["name"].Value == "__EVENTVALIDATION")
                        ViewStateValidation = input.Attributes["value"].Value;
                }

                string nomeArquivo = DateTime.Now.ToString("yyyyMMddHHmmss");
                int imgIguaisGeradas = 0;
                while (imgIguaisGeradas < imgIguais)
                {
                    string urlCaptcha = "https://piracicaba.simplissweb.com.br/contrib/app/nfse/captcha";
                    httpService.HandlerSet(new HttpClientHandler { UseCookies = false });
                    httpService.HeaderAcceptAdd(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                    httpService.HeaderAdd("cookie", Cookie);
                    httpService.ResultByteSet();
                    httpService.UrlSet(urlCaptcha);
                    retHttp = httpService.ExecuteGet();
                    if ((retHttp.Erro) || (retHttp.HttpStatusCode != HttpStatusCode.OK))
                    {
                        //return Retorno("0002");
                    }
                    else if (Encoding.UTF8.GetString(retHttp.BodyArrayByte).Contains("JFIF"))
                    {
                        var retorno = new HttpResponseMessage();
                        retorno.StatusCode = HttpStatusCode.OK;
                        retorno.Content = new ByteArrayContent(retHttp.BodyArrayByte);
                        retorno.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

                        File.WriteAllBytes(HttpContext.Current.Server.MapPath("~/Arquivos/Imagens/" + nomeArquivo + "_" + imgIguaisGeradas.ToString().PadLeft(2, '0') + ".jpeg"), retHttp.BodyArrayByte);

                        if (imgIguais == 1)
                            return retorno;

                        imgIguaisGeradas++;
                    }

                    Thread.Sleep(1000);
                }

                imgDiferentesGeradas++;
            }

            if (imgDiferentes == imgDiferentesGeradas)
                return Retorno("Arquivos gravados");

            return Retorno("0003");
        }

        private static HttpResponseMessage Retorno(string codRetorno)
        {
            StringBuilder responseHtml = new StringBuilder();
            responseHtml.Append("<html>");
            responseHtml.Append(" <body>");

            responseHtml.Append("  <center>" + codRetorno + "</center><br>");



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
