using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Xml;
using System.Collections;
using System.Security.Cryptography;
using System.IO;

namespace DBAccessProc
{
    public class ConfigurationManager
    {
        #region Member Variables

        private static NameValueCollection m_appSettings = new NameValueCollection();
        private static string m_configFileName = "";
        private static bool isNVCInitialized = false;
        private static bool isUnderDev = true;
        private static byte[] _cryptKey = null;
        private static byte[] _cryptIV = null;

        #endregion

        #region Constructor

        static ConfigurationManager()
        {
        }

        #endregion

        #region Properties

        public static string ConfigFileName
        {
            get
            {
                return m_configFileName;
            }
            set
            {
                m_configFileName = value;
            }
        }

        public static NameValueCollection AppSettings
        {
            get
            {
                if (!isNVCInitialized)
                    IntializeAppSettings();

                return m_appSettings;
            }
        }

        public static bool IsUnderDev
        {
            get { return isUnderDev; }
        }

        #endregion

        #region Public Member Methods

        public static void IntializeAppSettings()
        {
            XmlDocument xmlDoc = new XmlDocument();

            try
            {
                m_appSettings.Clear();

                // The %FORMWARE% environment variable may be in this string. Make sure to expand it.
                m_configFileName = System.Environment.ExpandEnvironmentVariables(m_configFileName);

                xmlDoc.Load(m_configFileName);
                XmlNode configModeNode = xmlDoc.DocumentElement.SelectSingleNode("/configuration/ENCRYPTIONS/OPTIONS/add[@key='UNDER_DEVELOPMENT']");
                string underDevelopment = configModeNode.Attributes["value"].Value.ToString();
                if (underDevelopment.ToUpper() == "FALSE")
                    isUnderDev = false;
                else
                    isUnderDev = true;

                ArrayList encryptKeys = new ArrayList();

                XmlNodeList encryptKeyNodeList = xmlDoc.DocumentElement.SelectNodes("/configuration/ENCRYPTIONS/KEYS/add");

                foreach (XmlNode encryptKeyNode in encryptKeyNodeList)
                {

                    string keyName = encryptKeyNode.Attributes["key"].Value.ToString();
                    encryptKeys.Add(keyName);
                }

                NameValueCollection nvcKeyValues = new NameValueCollection();

                XmlNodeList allKeyNodeList = xmlDoc.DocumentElement.SelectNodes("/configuration/appSettings/add");

                foreach (XmlNode keyNode in allKeyNodeList)
                {
                    string keyName = keyNode.Attributes["key"].Value.ToString();
                    string keyValue = keyNode.Attributes["value"].Value.ToString();

                    if (!isUnderDev)
                    {
                        if (encryptKeys.Count > 0)
                        {
                            if (encryptKeys.IndexOf(keyName) >= 0)
                            {
                                keyValue = ConfigurationManager.DecryptConfigInfo(keyValue);
                            }
                        }
                    }

                    m_appSettings.Add(keyName, keyValue);
                }

                isNVCInitialized = true;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        ///  Encrypts the supplied string
        /// </summary>
        /// <param name="configString"></param>
        /// <returns></returns>
        public static string EncryptConfigInfo(string configString)
        {
            string rtn;

            TripleDES tripleDESAlg = TripleDES.Create();
            ICryptoTransform transformer = tripleDESAlg.CreateEncryptor(GetCryptKey(), GetCryptIV());

            // Get bytes
            byte[] configBytes = Encoding.Unicode.GetBytes(configString);
            // Encrypt and get Base64 string
            rtn = Convert.ToBase64String(CryptoTransform(configBytes, transformer));

            return rtn;
        }

        /// <summary>
        /// Decrypts the supplied string
        /// </summary>
        /// <param name="configString"></param>
        /// <returns></returns>
        public static string DecryptConfigInfo(string configString)
        {
            string rtn;

            TripleDES tripleDESAlg = TripleDES.Create();
            ICryptoTransform transformer = tripleDESAlg.CreateDecryptor(GetCryptKey(), GetCryptIV());

            // Get bytes
            byte[] configBytes = Convert.FromBase64String(configString);
            // Decrypt and get string
            rtn = Encoding.Unicode.GetString(CryptoTransform(configBytes, transformer));

            return rtn;
        }

        #endregion

        #region Private Member Methods

        /// <summary>
        /// Retrieve configuration file name and path from registry.
        /// </summary>
        /// <returns></returns>

        //region ConfiguragionCryptography

        /// <summary>
        /// Encrypts/decrypts a byte array using the supplied transformer.
        /// </summary>
        /// <param name="bDataToEncrypt"></param>
        /// <param name="transformer"></param>
        /// <returns></returns>
        private static byte[] CryptoTransform(byte[] bDataToEncrypt, ICryptoTransform transformer)
        {
            byte[] rtn;

            // Memory stream to hold transformed bytes.
            MemoryStream memStream = new MemoryStream();
            CryptoStream cryptStream = new CryptoStream(memStream, transformer, CryptoStreamMode.Write);

            try
            {
                cryptStream.Write(bDataToEncrypt, 0, bDataToEncrypt.Length);
                cryptStream.FlushFinalBlock();
                cryptStream.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Error during encryption:\n" + ex.Message);
            }

            rtn = memStream.ToArray();
            return rtn;
        }

        /// <summary>
        /// Returns a cryptography key.
        /// </summary>
        /// <returns></returns>
        private static byte[] GetCryptKey()
        {
            if (_cryptKey == null)
            {
                GetEncryptMetrics();
            }

            return _cryptKey;
        }

        /// <summary>
        /// Returns a cryptography initialization vector.
        /// </summary>
        private static byte[] GetCryptIV()
        {
            if (_cryptIV == null)
            {
                GetEncryptMetrics();
            }

            return _cryptIV;
        }

        /// <summary>
        /// Initializes cryptography metrics.
        /// </summary>
        private static void GetEncryptMetrics()
        {
            // Use a fixed key at first to encrypt the assembly name.
            _cryptKey = Encoding.ASCII.GetBytes("HehTg9eKlNfEAJ6T");
            _cryptIV = Encoding.ASCII.GetBytes("s8haFdG2");
            string encryptedAssemblyName;
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            encryptedAssemblyName = EncryptConfigInfo(asm.FullName);

            // Now that we have a totally unique string that will change based
            // on the assembly name, set the key and initialization vectors.
            char[] encryptedAssemblyNameChars = encryptedAssemblyName.ToCharArray();
            _cryptKey = Encoding.ASCII.GetBytes(encryptedAssemblyNameChars, 0, 16);
            _cryptIV = Encoding.ASCII.GetBytes(encryptedAssemblyNameChars, 0, 8);
        }

        #endregion
    }
}
