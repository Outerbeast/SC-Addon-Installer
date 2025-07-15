/* Sven Co-op Addon Installer - Sven Co-op addon installation made easy
Copyright (C) 2025 Outerbeast
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <https://www.gnu.org/licenses/>.
*/

namespace SCAddonInstaller.src;

public static class Download
{
    public static bool IsValidUrl(string url)
    {
        if ( string.IsNullOrEmpty( url ) )
            return false;

        return
            Uri.TryCreate( url, UriKind.Absolute, out var uriResult ) &&
            ( uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps );
    }

    public static bool DownloadFile(string url, string destinationPath)
    {
        if ( string.IsNullOrEmpty( url ) || string.IsNullOrEmpty( destinationPath ) )
            throw new ArgumentException( "URL or destination path cannot be null or empty." );

        using ( var client = new HttpClient() )
        {   // Attach progress event handler before starting the download
            if ( client is null ) // How can the client be null?
            {
                Console.WriteLine( "WebClient instance is null!" );
                return false;
            }
            //Download the file asynchronously
            Console.WriteLine( $"Downloading file from {url} to {destinationPath}" );
            // Note: HttpClient does not have a DownloadFile method, so we use GetAsync and then save the content
            var response = client.GetAsync( url ).Result; // Use .Result to block until the task completes
            if ( !response.IsSuccessStatusCode )
            {
                Console.WriteLine( $"Failed to download file: {response.ReasonPhrase}" );
                return false;
            }
            var contentStream = response.Content.ReadAsStreamAsync().Result; // Get the content stream
            using var fileStream = new FileStream( destinationPath, FileMode.Create, FileAccess.Write, FileShare.None );
            contentStream.CopyTo( fileStream ); // Copy the content stream to the file stream
        }

        if ( File.Exists( destinationPath ) )
        {   // The file has been downloaded and exists at 'destination'
            var fileDownloaded = new FileInfo( destinationPath );

            if ( fileDownloaded is null || fileDownloaded.Length == 0 )
            {
                Console.WriteLine( "Downloaded file is empty or does not exist." );
                return false;
            }

            Console.WriteLine( $"Downloaded file size: {fileDownloaded.Length}" );
            Unpack.ExtractArchive( destinationPath, Config.strAddonPath );
            //Delete the downloaded file after extraction
            File.Delete( destinationPath );

            return true;
        }

        return false;
    }
};
