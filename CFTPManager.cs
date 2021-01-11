/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *                             
*  ///////////////////////////////////////////////////////////////////////////////////  *
*  //--███████--███████---███████--██---██--███████----------████████--████████-----//  *
*  //-███------██-----██-██-----██-██--██--██-------------------██--------██--------//  * 
*  //-██-------██-----██-██-----██-████---- ██████ -------------██--------██--------//  * 
*  //-███------██-----██-██-----██-██--██--------██-------------██--------██--------//  *
*  //--███████--███████---███████--██---██- ██████-----██----████████-----██--------//  *
*  //===============================================================================//  *
*  ///////////////////////////////////////////////////////////////////////////////////  *
*  // Script: CFTPManager.cs                                                        //  *
*  // Author: Brandon Cook                                                          //  *
*  //                                                                               //  *
*  ///////////////////////////////////////////////////////////////////////////////////  *
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Security;
using System.IO;
using FluentFTP;

namespace ProjectTrack
{
   internal abstract class FTPManager
    {
        FtpClient client;
        int port = 0;
        
        public async void CreateNewFile(string fileName)
        { }
        public async void DownloadFile(string servFile, string localDest)
        { }
        public async void DownloadFile(string[] servFiles,string[] localDest)
        { }
        public async void UploadFile(string localFile,string servDest)
        { }
        public async void UploadFile(string[] localFiles,string[] servDest)
        { }

        public async string GetDirectoryListing(string folder)
        { throw new NotImplementedException(); }
        public async string GetDirFileListing(string folder)
        { throw new NotImplementedException(); }
        public async bool GetFileExist(string fileName)
        { throw new NotImplementedException(); }

        private void Connect(ref FtpClient client)
        {
            SecureString password = new SecureString();
            foreach(char c in CSettingsManager.)
            client.Credentials = new System.Net.NetworkCredential()
        }

    }
}