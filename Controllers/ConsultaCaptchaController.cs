using NfeToPdf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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

            Guid guid = Guid.NewGuid();
            string nomeArquivo = guid.ToString();

            string ViewState = "";
            string ViewStateGenerator = "";
            string ViewStateValidation = "";
            string Cookie = "";
            StringBuilder arquivos = new StringBuilder();

            if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/Arquivos/Imagens")))
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Arquivos/Imagens"));

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
                        string nomeArquivoJpeg = nomeArquivo + "_" + imgDiferentesGeradas.ToString().PadLeft(3, '0') + "_" + imgIguaisGeradas.ToString().PadLeft(3, '0') + ".jpeg";
                        arquivos.AppendLine(HttpContext.Current.Server.MapPath("~/Arquivos/Imagens/" + nomeArquivoJpeg));
                        File.WriteAllBytes(HttpContext.Current.Server.MapPath("~/Arquivos/Imagens/" + nomeArquivoJpeg), retHttp.BodyArrayByte);

                        imgIguaisGeradas++;
                    }

                    Thread.Sleep(1000);
                }

                imgDiferentesGeradas++;
            }

            if ((imgDiferentes == 1) && (imgIguais == 1))
            {
                byte[] arquivoRetorno = File.ReadAllBytes(arquivos.ToString().Replace("\n","").Replace("\r",""));
                var retorno = new HttpResponseMessage();
                retorno.StatusCode = HttpStatusCode.OK;
                retorno.Content = new ByteArrayContent(arquivoRetorno);
                retorno.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                return retorno;
            }

            if (imgDiferentes == imgDiferentesGeradas)
            {
                string nomeArquivoZip = HttpContext.Current.Server.MapPath("~/Arquivos/Imagens/" + nomeArquivo + ".zip");
                string[] delim = { Environment.NewLine, "\n" };
                string[] lines = arquivos.ToString().Split(delim, StringSplitOptions.None);

                using (var stream = File.OpenWrite(nomeArquivoZip))
                {
                    using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create))
                    {
                        foreach (string line in lines)
                        {
                            if (!string.IsNullOrEmpty(line))
                                archive.CreateEntryFromFile(line, Path.GetFileName(line), CompressionLevel.Optimal);
                        }
                    }
                }

                byte[] arquivoRetorno = File.ReadAllBytes(nomeArquivoZip);
                var retorno = new HttpResponseMessage();
                retorno.StatusCode = HttpStatusCode.OK;
                retorno.Content = new ByteArrayContent(arquivoRetorno);
                retorno.Content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
                return retorno;
            }

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
