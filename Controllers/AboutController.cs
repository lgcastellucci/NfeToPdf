using Microsoft.Win32;
using System;
using System.Reflection;
using System.Web.Mvc;

namespace NfeToPdf.Controllers
{
    public class AboutController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                object[] targetFrameworkAttribute = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(System.Runtime.Versioning.TargetFrameworkAttribute), false);
                ViewBag.Ambiente = ((System.Runtime.Versioning.TargetFrameworkAttribute)targetFrameworkAttribute[0]).FrameworkDisplayName;
            }
            catch (Exception ex)
            {
                ViewBag.Ambiente = ex.Message;
            }
            return View();
        }

        // Checking the version using >= will enable forward compatibility, 
        // however you should always compile your code on newer versions of
        // the framework to ensure your app works the same.
        private string CheckFor45DotVersion(int releaseKey)
        {
            if (releaseKey >= 528040)
            {
                return "4.8";
            }
            else if (releaseKey >= 461808)
            {
                return "4.7.2";
            }
            else if (releaseKey >= 461308)
            {
                return "4.7.1";
            }
            else if (releaseKey >= 460798)
            {
                return "4.7";
            }
            else if (releaseKey >= 394802)
            {
                return "4.6.2";
            }
            else if (releaseKey >= 394254)
            {
                return "4.6.1";
            }
            else if (releaseKey >= 393295)
            {
                return "4.6";
            }
            else if ((releaseKey >= 379893))
            {
                return "4.5.2";
            }
            else if ((releaseKey >= 378675))
            {
                return "4.5.1";
            }
            else if ((releaseKey >= 378389))
            {
                return "4.5";
            }
            else
            {
                return "version not detected";
            }
        }

        private string Get45or451FromRegistry()
        {
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {
                    return "Version: " + CheckFor45DotVersion((int)ndpKey.GetValue("Release"));
                }
                else
                {
                    return "Version 4.5 or later is not detected.";
                }
            }
        }

    }
}
