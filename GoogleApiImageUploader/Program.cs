using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace GoogleApiImageUploader
{
    class Program
    {
        static string[] Scopes = { 
            DriveService.Scope.DriveFile,
            DriveService.Scope.DriveAppdata, 
            DriveService.Scope.Drive 
        };

        static string ApplicationName = "Drive API .NET Quickstart";

        static string folderName = @"wwwroot/ImageUploads";
        static string folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

        static void Main(string[] args)
        {
          
            // Authenticate the app
            UserCredential credential;

            credential = GetCredentials();

            // Create Drive API service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });


            //path to image or image file
            var newpath = @"C:\\Users\\Mameyaw\\source\\repos\\GoogleApiImageUploader\\GoogleApiImageUploader\\wwwroot\\ImageUploads\\finePersol.jpg";//folderName + "/hcmB.png";

            UploadBasicImage(newpath, service);

            string pageToken = null;

            do
            {
                ListFiles(service, ref pageToken);

            } while (pageToken != null);

            Console.WriteLine("Done");
            Console.Read();

        }

        //list files
        private static void ListFiles(DriveService service, ref string pageToken)
        {
            // Define parameters of request.
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.PageSize = 1;
            //listRequest.Fields = "nextPageToken, files(id, name)";
            listRequest.Fields = "nextPageToken, files(name)";
            listRequest.PageToken = pageToken;
            listRequest.Q = "mimeType='image/jpeg'";

            // List files.
            var request = listRequest.Execute();


            if (request.Files != null && request.Files.Count > 0)
            {


                foreach (var file in request.Files)
                {
                    Console.WriteLine("{0}", file.Name);
                }

                pageToken = request.NextPageToken;

                if (request.NextPageToken != null)
                {
                    Console.WriteLine("Press any key to conti...");
                    Console.ReadLine();



                }


            }
            else
            {
                Console.WriteLine("No files found.");

            }


        }

        // upload file
        private static void UploadBasicImage(string path, DriveService service)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File();
            fileMetadata.Name = Path.GetFileName(path);
            fileMetadata.MimeType = "image/jpeg";
            FilesResource.CreateMediaUpload request;

            using (var stream = new FileStream(path, FileMode.Open))
            {
                request = service.Files.Create(fileMetadata, stream, "image/jpeg");
                request.Fields = "id";
                request.Upload();
            }

            var file = request.ResponseBody;

            var storagePathOnGoogleDrive = request.Path;
            
            Console.WriteLine("File ID: " + file.Id);

        }

        // authenticate method
        private static UserCredential GetCredentials()
        {
            UserCredential credential;

            using (var stream =
               new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            return credential;
        }

    }
}
