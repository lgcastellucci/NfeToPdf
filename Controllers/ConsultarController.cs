using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;
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
        public ContentResult Get1(string prestadorMunicipio, string tomadorCNPJ, string RpsNumero)
        {
            ContentResult response = new ContentResult();
            if (string.IsNullOrEmpty(prestadorMunicipio))
            {
                StringBuilder responseHtml = new StringBuilder();
                responseHtml.Append("<html>");
                responseHtml.Append(" <body>");
                responseHtml.Append("  <br><center>Municipio do prestador é obrigatorio</center>");
                responseHtml.Append(" </body>");
                responseHtml.Append("</html>");

                response.Content = responseHtml.ToString();
                response.ContentType = "text/html";
                return response;
            }
            if (string.IsNullOrEmpty(tomadorCNPJ))
            {
                StringBuilder responseHtml = new StringBuilder();
                responseHtml.Append("<html>");
                responseHtml.Append(" <body>");
                responseHtml.Append("  <br><center>CNPJ do tomador deve é obrigatorio</center>");
                responseHtml.Append(" </body>");
                responseHtml.Append("</html>");

                response.Content = responseHtml.ToString();
                response.ContentType = "text/html";
                return response;
            }
            if (string.IsNullOrEmpty(RpsNumero))
            {
                StringBuilder responseHtml = new StringBuilder();
                responseHtml.Append("<html>");
                responseHtml.Append(" <body>");
                responseHtml.Append("  <br><center>Numero do RPS é obrigatorio</center>");
                responseHtml.Append(" </body>");
                responseHtml.Append("</html>");

                response.Content = responseHtml.ToString();
                response.ContentType = "text/html";
                return response;
            }

            if (prestadorMunicipio != "3507506")
            {
                StringBuilder responseHtml = new StringBuilder();
                responseHtml.Append("<html>");
                responseHtml.Append(" <body>");
                responseHtml.Append("  <br><center>Municipio não aceito, utilize o código do IBGE do municipio</center>");
                responseHtml.Append("  <br><center>Caso o código não seja aceito informe a nfetopdf@castellucci.net.br</center>");
                responseHtml.Append(" </body>");
                responseHtml.Append("</html>");

                response.Content = responseHtml.ToString();
                response.ContentType = "text/html";
                return response;
            }

            response.Content = "<html><body>Contate nfetopdf@castellucci.net.br</body></html>";
            response.ContentType = "text/html";
            return response;


        }

        /*
        static void PesquisaBotucatu()
        {
            string cookie = "";
            string userId = "";
            string url = "http://botucatu.bsit-br.com.br/nfse/rps-validation.jsf";
            string cnpjTomador = "06305147000176";
            string numeroRPS = "253745";

            HttpClient httpClient1 = new HttpClient();
            HttpResponseMessage response1 = null;
            string responseBody1 = "";
            httpClient1.BaseAddress = new Uri(url);
            httpClient1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            try
            {
                response1 = httpClient1.GetAsync(url).Result;
                responseBody1 = response1.Content.ReadAsStringAsync().Result;

                if (response1.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> valuesCookie))
                    cookie = valuesCookie.FirstOrDefault();
                if (response1.Headers.TryGetValues("userId", out IEnumerable<string> valuesUserId))
                    userId = valuesUserId.FirstOrDefault();
            }
            catch (Exception ex)
            {
                string messageException = "";
                messageException = messageException + "Message --- " + ex.Message + "\r\n";
                messageException = messageException + "HelpLink --- " + ex.HelpLink + "\r\n";
                messageException = messageException + "Source --- " + ex.Source + "\r\n";
                messageException = messageException + "StackTrace --- " + ex.StackTrace + "\r\n";
                messageException = messageException + "TargetSite --- " + ex.TargetSite + "\r\n";

                RegistraLogService.Log(messageException);
                return;
            }
            if (!string.IsNullOrEmpty(cookie) && !string.IsNullOrEmpty(userId))
            {
                RegistraLogService.Log("Cookie: " + cookie);
            }
            else
            {
                RegistraLogService.Log("responseBody1");
                RegistraLogService.Log(responseBody1);
            }



            HttpClient httpClient2 = new HttpClient();
            HttpResponseMessage response2 = null;
            string responseBody2 = "";
            httpClient2.BaseAddress = new Uri(url);
            httpClient2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            httpClient2.DefaultRequestHeaders.Add("cookie", cookie);
            httpClient2.DefaultRequestHeaders.Add("userId", userId);
            string stringConteudo2 = "";
            stringConteudo2 += "AJAXREQUEST=_viewRoot";
            stringConteudo2 += "&validationNFSEForm=validationNFSEForm";
            stringConteudo2 += "&validationNFSEForm%3AtypePerson=J";
            stringConteudo2 += "&validationNFSEForm%3ApersonCPF=";
            stringConteudo2 += "&validationNFSEForm%3AverificationCode=";
            stringConteudo2 += "&javax.faces.ViewState=j_id1";
            stringConteudo2 += "&validationNFSEForm%3Aj_id24=validationNFSEForm%3Aj_id24&";
            StringContent stringContent2 = new StringContent(stringConteudo2, Encoding.UTF8, "application/x-www-form-urlencoded");
            try
            {
                response2 = httpClient2.PostAsync(url, stringContent2).Result;
                responseBody2 = response2.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                string messageException = "";
                messageException = messageException + "Message --- " + ex.Message + "\r\n";
                messageException = messageException + "HelpLink --- " + ex.HelpLink + "\r\n";
                messageException = messageException + "Source --- " + ex.Source + "\r\n";
                messageException = messageException + "StackTrace --- " + ex.StackTrace + "\r\n";
                messageException = messageException + "TargetSite --- " + ex.TargetSite + "\r\n";

                RegistraLogService.Log(messageException);
                return;
            }

            if (responseBody2.Contains("INFORMAÇÕES PARA VALIDAÇÃO NFSE"))
            {
                RegistraLogService.Log("INFORMAÇÕES PARA VALIDAÇÃO NFSE");
            }
            else
            {
                RegistraLogService.Log("responseBody2");
                RegistraLogService.Log(responseBody2);
            }




            HttpClient httpClient3 = new HttpClient();
            HttpResponseMessage response3 = null;
            string responseBody3 = "";
            httpClient3.BaseAddress = new Uri(url);
            httpClient3.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            httpClient3.DefaultRequestHeaders.Add("cookie", cookie);
            httpClient3.DefaultRequestHeaders.Add("userId", userId);
            string stringConteudo3 = "";
            stringConteudo3 += "AJAXREQUEST=_viewRoot";
            stringConteudo3 += "&validationNFSEForm=validationNFSEForm";
            stringConteudo3 += "&validationNFSEForm%3AtypePerson=J";
            stringConteudo3 += "&validationNFSEForm%3ApersonCNPJ=" + cnpjTomador;
            stringConteudo3 += "&validationNFSEForm%3AverificationCode=" + numeroRPS;
            stringConteudo3 += "&javax.faces.ViewState=j_id1";
            stringConteudo3 += "&validationNFSEForm%3Averify=validationNFSEForm%3Averify&";
            StringContent stringContent3 = new StringContent(stringConteudo3, Encoding.UTF8, "application/x-www-form-urlencoded");
            try
            {
                response3 = httpClient3.PostAsync(url, stringContent3).Result;
                responseBody3 = response3.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                string messageException = "";
                messageException = messageException + "Message --- " + ex.Message + "\r\n";
                messageException = messageException + "HelpLink --- " + ex.HelpLink + "\r\n";
                messageException = messageException + "Source --- " + ex.Source + "\r\n";
                messageException = messageException + "StackTrace --- " + ex.StackTrace + "\r\n";
                messageException = messageException + "TargetSite --- " + ex.TargetSite + "\r\n";

                RegistraLogService.Log(messageException);
                return;
            }
            if (responseBody3.Contains("Foi encontrada a Nota com os dados informados"))
            {
                RegistraLogService.Log("Foi encontrada a Nota com os dados informados");

                string campoRazaoTomador = "<span id=\"basicNFSEForm:name\" style=\"margin:0px 0px 0px 0px; float: left;\">";
                if (responseBody3.Contains(campoRazaoTomador))
                {
                    campoRazaoTomador = responseBody3.Substring(responseBody3.IndexOf(campoRazaoTomador) + campoRazaoTomador.Length, 100);
                    campoRazaoTomador = campoRazaoTomador.Substring(0, campoRazaoTomador.IndexOf("</span>"));

                    RegistraLogService.Log(campoRazaoTomador);
                }
            }
            else if (responseBody3.Contains("É necessário informar o CPF ou CNPJ válido do prestador"))
            {
                RegistraLogService.Log("É necessário informar o CPF ou CNPJ válido do prestador");
            }
            else
            {
                RegistraLogService.Log("responseBody3");
                RegistraLogService.Log(responseBody3);
            }


            HttpClient httpClient4 = new HttpClient();
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

                RegistraLogService.Log(messageException);
                return;
            }

            if (Encoding.UTF8.GetString(responseBody4).Contains("PDF-1.4"))
            {
                string caminhoArquivo = AppContext.BaseDirectory;
                File.WriteAllBytes(Path.Combine(caminhoArquivo, numeroRPS + ".pdf"), responseBody4);
                RegistraLogService.Log(Path.Combine(caminhoArquivo, numeroRPS + ".pdf"));
            }
            else
            {
                RegistraLogService.Log("responseBody4");
                RegistraLogService.Log(Encoding.UTF8.GetString(responseBody4));
            }
        }
        */
    }
}
