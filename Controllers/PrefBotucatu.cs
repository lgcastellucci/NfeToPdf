using NfeToPdf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Mvc;

namespace NfeToPdf.Controllers
{
    public class PrefBotucatu : Controller
    {
        readonly string url = "http://botucatu.bsit-br.com.br/nfse/rps-validation.jsf";
        public Resposta Executar(string tomadorCNPJ, string rpsNumero)
        {
            Resposta resposta = new Resposta();
            resposta.Sucesso = false;


            string cookie = "";
            string userId = "";


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

                //RegistraLogService.Log(messageException);
                return resposta;
            }
            if (!string.IsNullOrEmpty(cookie) && !string.IsNullOrEmpty(userId))
            {
                //RegistraLogService.Log("Cookie: " + cookie);
            }
            else
            {
                //RegistraLogService.Log(responseBody1);
                return resposta;
            }

            HttpClient httpClient2 = new HttpClient(new HttpClientHandler { UseCookies = false });
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

                //RegistraLogService.Log(messageException);
                return resposta;
            }

            if (responseBody2.Contains("INFORMAÇÕES PARA VALIDAÇÃO NFSE"))
            {
                //RegistraLogService.Log("INFORMAÇÕES PARA VALIDAÇÃO NFSE");
            }
            else
            {
                //RegistraLogService.Log(responseBody2);
                return resposta;
            }




            HttpClient httpClient3 = new HttpClient(new HttpClientHandler { UseCookies = false });
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
            stringConteudo3 += "&validationNFSEForm%3ApersonCNPJ=" + tomadorCNPJ;
            stringConteudo3 += "&validationNFSEForm%3AverificationCode=" + rpsNumero;
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

                //RegistraLogService.Log(messageException);
                return resposta;
            }
            if (responseBody3.Contains("Foi encontrada a Nota com os dados informados"))
            {
                //RegistraLogService.Log("Foi encontrada a Nota com os dados informados");

                string campoRazaoTomador = "<span id=\"basicNFSEForm:name\" style=\"margin:0px 0px 0px 0px; float: left;\">";
                if (responseBody3.Contains(campoRazaoTomador))
                {
                    campoRazaoTomador = responseBody3.Substring(responseBody3.IndexOf(campoRazaoTomador) + campoRazaoTomador.Length, 100);
                    campoRazaoTomador = campoRazaoTomador.Substring(0, campoRazaoTomador.IndexOf("</span>"));

                    //RegistraLogService.Log(campoRazaoTomador);
                }
            }
            else if (responseBody3.Contains("É necessário informar o CPF ou CNPJ válido do prestador"))
            {
                //RegistraLogService.Log("É necessário informar o CPF ou CNPJ válido do prestador");
            }
            else
            {
                //RegistraLogService.Log(responseBody3);
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

                //string caminhoArquivo = AppContext.BaseDirectory;
                //System.IO.File.WriteAllBytes(Path.Combine(caminhoArquivo, rpsNumero + ".pdf"), responseBody4);
                //RegistraLogService.Log(Path.Combine(caminhoArquivo, numeroRPS + ".pdf"));

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
