/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *                            
 *  ///////////////////////////////////////////////////////////////////////////////////  *
 *  //--███████--███████---███████--██---██--███████----------████████--████████-----//  *
 *  //-███------██-----██-██-----██-██--██--██-------------------██--------██--------//  * 
 *  //-██-------██-----██-██-----██-████---- ██████ -------------██--------██--------//  * 
 *  //-███------██-----██-██-----██-██--██--------██-------------██--------██--------//  *
 *  //--███████--███████---███████--██---██- ██████-----██----████████-----██--------//  *
 *  //===============================================================================//  *
 *  ///////////////////////////////////////////////////////////////////////////////////  *
 *  // Script: CSettingsManager.cs                                                   //  *
 *  // Author: Brandon Cook                                                          //  *
 *  //                                                                               //  *
 *  ///////////////////////////////////////////////////////////////////////////////////  *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Win32;

namespace ProjectTrack
{
    static internal class CSettingsManager
    {
        // BASE CONFIG FILES
        // * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *//
        private static string CONFIG_FILE = String.Format("{0}config\\settings.xml", AppDomain.CurrentDomain.BaseDirectory);
        // XML ELEMENTS //
        // * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *//
        const string
            X_MS_ServerName   = "settings/mysql/server",
            X_MS_DatabaseName = "settings/mysql/database",
            X_MS_UserName     = "settings/mysql/user",
            X_MS_Port         = "settings/mysql/port",
            X_MS_Password     = "settings/mysql/password",
            X_MS_Ssl          = "settings/mysql/ssl",

            X_EM_S_ServerName = "settings/email/send/server",
            X_EM_S_Port       = "settings/email/send/port",
            X_EM_S_UserName   = "settings/email/send/username",
            X_EM_S_Password   = "settings/email/send/password",

            X_EM_R_ServerName = "settings/email/receive/server",
            X_EM_R_Port       = "settings/email/receive/port",
            X_EM_R_UserName   = "settings/email/receive/username",
            X_EM_R_Password   = "settings/email/receive/password",

            X_EM_Name         = "settings/email/message/fromname",
            x_EM_Subject      = "settings/email/message/subject",

            x_EM_Server      = "settings/email/message/subject",
            x_EM_Port        = "settings/email/message/subject",
            x_EM_Subject     = "settings/email/message/subject",


            REG_SUBKEY        = @"SOFTWARE\ERSTracking",
            REG_VALUE1        = "Setting1";

        // XML Property Grabs //
        // * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *//

        static public string MYSQLServerAddress { get => xmlSettingsDoc.XPathSelectElement(X_MS_ServerName  ).Value; }
        static public string MYSQLDatabase      { get => xmlSettingsDoc.XPathSelectElement(X_MS_DatabaseName).Value; }
        static public string MYSQLUser          { get => xmlSettingsDoc.XPathSelectElement(X_MS_UserName    ).Value; }
        static public string MYSQLPassword      { get => xmlSettingsDoc.XPathSelectElement(X_MS_Password    ).Value; }
        static public string MYSQLPort          { get => xmlSettingsDoc.XPathSelectElement(X_MS_Port        ).Value; }
        static public string MYSQLSsl           { get => xmlSettingsDoc.XPathSelectElement(X_MS_Ssl         ).Value; }

        static public string EMAIL_S_Server     { get => xmlSettingsDoc.XPathSelectElement(X_EM_S_ServerName).Value; }
        static public string EMAIL_S_Port       { get => xmlSettingsDoc.XPathSelectElement(X_EM_S_Port      ).Value; }
        static public string EMAIL_S_User       { get => xmlSettingsDoc.XPathSelectElement(X_EM_S_UserName  ).Value; }
        static public string EMAIL_S_Password   { get => xmlSettingsDoc.XPathSelectElement(X_EM_S_Password  ).Value; }

        static public string EMAIL_R_Server     { get => xmlSettingsDoc.XPathSelectElement(X_EM_R_ServerName).Value; }
        static public string EMAIL_R_Port       { get => xmlSettingsDoc.XPathSelectElement(X_EM_R_Port      ).Value; }
        static public string EMAIL_R_User       { get => xmlSettingsDoc.XPathSelectElement(X_EM_R_UserName  ).Value; }
        static public string EMAIL_R_Password   { get => xmlSettingsDoc.XPathSelectElement(X_EM_R_Password  ).Value; }

        static public string EMAIL_Name         { get => xmlSettingsDoc.XPathSelectElement(X_EM_Name        ).Value; }
        static public string EMAIL_subject      { get => xmlSettingsDoc.XPathSelectElement(x_EM_Subject     ).Value; }
        static public string EMAIL_subject      { get => xmlSettingsDoc.XPathSelectElement(x_EM_Subject     ).Value; }

        static public string EMAIL_subject      { get => xmlSettingsDoc.XPathSelectElement(x_EM_Subject     ).Value; }
        static public string EMAIL_subject      { get => xmlSettingsDoc.XPathSelectElement(x_EM_Subject     ).Value; }
        static public string EMAIL_subject      { get => xmlSettingsDoc.XPathSelectElement(x_EM_Subject     ).Value; }
        static public string EMAIL_subject      { get => xmlSettingsDoc.XPathSelectElement(x_EM_Subject     ).Value; }

        static XDocument xmlSettingsDoc;

        
        /// <summary>
        /// Initialize Settings
        /// </summary>
        public static void LoadSettings()

        {
            if (File.Exists(CONFIG_FILE))
                xmlSettingsDoc = XDocument.Load(CONFIG_FILE);
            else throw new ApplicationException("Could No Find Configuration File!");
        }

    }
}