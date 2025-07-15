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
using SharpCompress.Archives;
using SharpCompress.Common;

namespace SCAddonInstaller.src;

public static class Unpack
{
    public readonly static string[] STR_ARCHIVE_TYPES =
    [   // Essential
        ".zip",
        ".7z",
        ".rar",
        // Non-essential
        ".tar",
        ".gz",
        ".bz2",
        ".xz",
        ".zst"
    ];

    public readonly static string[] STR_MAP_FILES =
    [
        ".bsp",
        ".cfg",
        ".res",
        ".wad",
        ".fgd"
    ];

    public static bool IsSupportedArchive(string filePath)
    {
        if ( Download.IsValidUrl( filePath ) )
            return false;

        return STR_ARCHIVE_TYPES.Contains( filePath[filePath.LastIndexOf( '.' )..], StringComparer.OrdinalIgnoreCase );
    }

    public static bool IsSupportedMapFile(string filePath)
    {
        return STR_MAP_FILES.Contains( filePath[filePath.LastIndexOf( '.' )..], StringComparer.OrdinalIgnoreCase );
    }

    public static void ExtractArchive(string archivePath, string destinationPath)
    {
        if ( string.IsNullOrEmpty( archivePath ) || string.IsNullOrEmpty( destinationPath ) )
            throw new ArgumentException( "Archive path or destination path cannot be null or empty." );

        if ( !File.Exists( archivePath ) )
            throw new FileNotFoundException( $"Archive not found: {archivePath}" );

        if ( !IsSupportedArchive( archivePath ) )
            throw new ArgumentException( $"Unsupported archive format: {archivePath[archivePath.LastIndexOf( '.' )..]}" );

        using var archive = ArchiveFactory.Open( archivePath );
        foreach ( var entry in archive.Entries.Where( e => !e.IsDirectory ) )
        {
            entry.WriteToDirectory( destinationPath, new ExtractionOptions
            {
                ExtractFullPath = true,
                Overwrite = true
            } );
        }
    }

    public static void CopyMapFile(string filePath, string destinationDirectory)
    {
        if ( string.IsNullOrEmpty( filePath ) || string.IsNullOrEmpty( destinationDirectory ) )
            throw new ArgumentException( "File path and destination directory cannot be null or empty." );

        if ( !File.Exists( filePath ) )
            throw new FileNotFoundException( $"File not found: {filePath}" );

        if ( !IsSupportedMapFile( filePath ) )
            throw new ArgumentException( $"Unsupported map file type: {filePath.LastIndexOf( '.' )}" );

        Directory.CreateDirectory( destinationDirectory ); // In case the directory does not exist, make sure to create it
        File.Copy( filePath, Path.Combine( destinationDirectory, Path.GetFileName( filePath ) ), true );
    }
};
