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

public static class Install
{
    public static byte countSuccess, total;
    public static async Task RunAsync
    (
        ListBox.ObjectCollection ITEMS,
        string addonPath,
        Action<int> reportProgress,
        Action<string, string, MessageBoxIcon> showMessage
    )
    {
        countSuccess = 0;
        total = (byte) ITEMS.Count;

        await Task.Run( () => // I'd rather not have to use a lambda here, but it seems necessary to avoid blocking the UI thread
        {
            foreach ( var item in ITEMS )
            {   // Skip null or empty items
                if ( item is null || string.IsNullOrEmpty( item.ToString() ) )
                    continue;

                string? addonFile = item.ToString();

                if ( string.IsNullOrEmpty( addonFile ) )
                    continue;

                try
                {
                    if ( Unpack.IsSupportedArchive( addonFile ) )
                    {
                        Unpack.ExtractArchive( addonFile, addonPath );
                        countSuccess++;
                    }
                    else if ( Unpack.IsSupportedMapFile( addonFile ) )
                    {
                        Unpack.CopyMapFile( addonFile, addonPath + ( !addonFile.EndsWith( ".wad" ) || !addonFile.EndsWith( ".fgd" ) ? "\\maps" : "" ) );
                        countSuccess++;
                    }
                    else if ( Download.IsValidUrl( addonFile ) )
                    {
                        if ( Download.DownloadFile( addonFile, Path.Combine( addonPath, Path.GetFileName( addonFile ) ) ) )
                            countSuccess++;
                    }
                    else
                        showMessage?.Invoke( $"Unsupported file format: {addonFile}", "Warning", MessageBoxIcon.Exclamation );
                }
                catch ( Exception ex )
                {
                    showMessage?.Invoke( $"Error processing {addonFile}: {ex.Message}", "Error", MessageBoxIcon.Error );
                }
                // Report progress
                reportProgress?.Invoke( Math.Min( (int) ( ( countSuccess + 1 ) * 100.0 / total ), 100 ) );
            }
        } );
    }
};
