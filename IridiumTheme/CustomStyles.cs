using System;
using System.IO;
using System.Text;
using MediaBrowser.Library.Configuration;
using MediaBrowser.Library.Logging;
using Microsoft.MediaCenter.UI;

namespace Iridium
{
    public class CustomStyles : ModelItem
    {
        //private ConfigData data; 
        static bool isValid;

        public const string STYLE_RANDOM = "[Random]";

        const string StylesCoreTag = "CORE VERSION (DO NOT EDIT):";
        const string StylesVersionTag = "STYLE VERSION:";
        const string StylesEndTag = "-->";
        const int StylesCoreVersion = 6;

        public static readonly string StylesFolderPath = Path.Combine(ApplicationPaths.AppPluginPath, @"Iridium\IridiumStyles");
        public static readonly Choice InstalledStyles = new Choice();
        
        public static void InitStyles()
        {
            if (!Directory.Exists(StylesFolderPath))
            {
                Logger.ReportInfo("Iridium - Creating styles folder: " + StylesFolderPath);
                Directory.CreateDirectory(StylesFolderPath);
            }

            VerifyBundledStyle(Path.Combine(StylesFolderPath, "Shaeff.mcml"), Resources.Shaeff);
            

            Logger.ReportInfo("Iridium - Validating installed styles"); 

            string[] styleFiles = Directory.GetFiles(StylesFolderPath, "*.mcml");

            foreach (string styleFile in styleFiles)
            {
                Logger.ReportInfo("Iridium - Checking installed style: " + styleFile);

                try
                {
                    StreamReader styleReader = new StreamReader(styleFile);
                    string line = styleReader.ReadLine();
                    int nPos = -1;
                    int nStart = -1;
                    int nCoreVersion = -1;

                    styleReader.Close();

                    nPos = line.IndexOf(StylesCoreTag, StringComparison.Ordinal);

                    if (nPos > -1)
                    {
                        nStart = nPos + StylesCoreTag.Length;
                        nPos = line.IndexOf(StylesEndTag, nStart, StringComparison.Ordinal);

                        if (nPos > -1)
                        {
                            nCoreVersion = Convert.ToInt32(line.Substring(nStart, nPos - nStart).Trim());

                            if (nCoreVersion == StylesCoreVersion)
                                isValid = true;
                            else
                                Logger.ReportError("Iridium - Core version mismatch: " + styleFile);
                        }
                        else
                        {
                            Logger.ReportError("Iridium - Core version not found: " + styleFile);
                        }
                    }
                    else
                    {
                        Logger.ReportError("Iridium - Core version not found: " + styleFile);
                    }
                }
                catch
                {
                    /* NOP */
                }

                if (isValid == false)
                {
                    string backupPath = styleFile + ".ERR";

                    if (File.Exists(backupPath))
                        File.Delete(backupPath);

                    File.Move(styleFile, backupPath);
                }
            }
        }

        private static void VerifyBundledStyle (string styleFile, byte[] bundledStyle)
        {
            bool    installStyle = true;

            Logger.ReportInfo ("Iridium - Verifying bundled style: " + styleFile);

            if (File.Exists (styleFile))
            {
                Encoding        encoder = new UTF8Encoding ();
                StreamReader    styleReader = new StreamReader (styleFile);
                string          lineCore = styleReader.ReadLine ();
                string          lineVersion = styleReader.ReadLine ();
                int             nPos = -1;
                int             nStart = -1;
                int             nCoreVersion = -1;
                int             nStyleVersion = -1;
                int             nBundledStyleVersion = -1;

                styleReader.Close ();

                nPos = lineCore.IndexOf(StylesCoreTag, StringComparison.Ordinal);

                if (nPos > -1)
                {
                    nStart = nPos + StylesCoreTag.Length;
                    nPos = lineCore.IndexOf(StylesEndTag, nStart, StringComparison.Ordinal);

                    if (nPos > -1)
                    {
                        nCoreVersion = Convert.ToInt32 (lineCore.Substring (nStart, nPos - nStart).Trim ());

                        if (nCoreVersion == StylesCoreVersion)
                        {
                            nPos = lineVersion.IndexOf(StylesVersionTag, StringComparison.Ordinal);

                            if (nPos > -1)
                            {
                                nStart = nPos + StylesVersionTag.Length;
                                nPos = lineVersion.IndexOf(StylesEndTag, nStart, StringComparison.Ordinal);

                                if (nPos > -1)
                                {
                                    string  bundledData = encoder.GetString (bundledStyle);

                                    nStyleVersion = Convert.ToInt32 (lineVersion.Substring (nStart, nPos - nStart).Trim ());

                                    nPos = bundledData.IndexOf(StylesVersionTag, StringComparison.Ordinal);

                                    if (nPos > -1)
                                    {
                                        nStart = nPos + StylesVersionTag.Length;
                                        nPos = bundledData.IndexOf(StylesEndTag, nStart, StringComparison.Ordinal);

                                        if (nPos > -1)
                                        {
                                            nBundledStyleVersion = Convert.ToInt32 (bundledData.Substring (nStart, nPos - nStart).Trim ());

                                            if (nStyleVersion >= nBundledStyleVersion)
                                                installStyle = false;
                                        }
                                        else
                                        {
                                            Logger.ReportError ("Iridium - Bundled style version not found: " + styleFile);
                                        }
                                    }
                                    else
                                    {
                                        Logger.ReportError ("Iridium - Bundled style version not found: " + styleFile);
                                    }
                                }
                                else
                                {
                                    Logger.ReportError ("Iridium - Style version not found: " + styleFile);
                                }
                            }
                            else
                            {
                                Logger.ReportError ("Iridium - Style version not found: " + styleFile);
                            }
                        }
                        else
                        {
                            Logger.ReportInfo ("Iridium - Core version mismatch: " + styleFile);
                        }
                    }
                    else
                    {
                        Logger.ReportError ("Iridium - Core version not found: " + styleFile);
                    }
                }
                else
                {
                    Logger.ReportError ("Iridium - Core version not found: " + styleFile);
                }
            }

            if (installStyle)
            {
                Logger.ReportInfo ("Iridium - Installing bundled style: " + styleFile);

                try
                {
                    if (File.Exists (styleFile))
                    {
                        DateTime    now = DateTime.Now;
                        string      backupFile = Path.Combine (Path.GetDirectoryName (styleFile),
                                                               Path.GetFileNameWithoutExtension (styleFile) +
                                                                    "_" +
                                                                    now.Year +
                                                                    now.Month.ToString ("D2") +
                                                                    now.Day.ToString ("D2") +
                                                                    now.Hour.ToString ("D2") +
                                                                    now.Minute.ToString ("D2") +
                                                                    now.Second.ToString ("D2") +
                                                                    ".BAK");

                        Logger.ReportInfo ("Iridium - Backing up existing style to: " + backupFile);

                        if (File.Exists (backupFile))
                            File.Delete (backupFile);

                        File.Move (styleFile, backupFile);
                    }

                    BinaryWriter    stylesWriter = new BinaryWriter (File.Open (styleFile, FileMode.OpenOrCreate));

                    stylesWriter.Write (bundledStyle);
                    stylesWriter.Close ();
                }
                catch
                {
                    /* NOP */
                }
            }
            else
            {
                Logger.ReportInfo ("Iridium - Skipping bundled style: " + styleFile);
            }
        }

        
    }
}
