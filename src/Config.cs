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

public static class Config
{
    private static string? AddonPath;

    public static string strAddonPath
    {
        get
        {
            if ( string.IsNullOrEmpty( AddonPath ) )
                throw new InvalidOperationException( "Addon path is not set." );

            return AddonPath;
        }
        set
        {
            if ( string.IsNullOrEmpty( value ) )
                throw new ArgumentException( "Addon path cannot be null or empty." );

            AddonPath = value;
        }
    }

    public static string GetAddonPathConfigFile()
    {
        string dir = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData ), "SCAddonInstaller" );

        if ( string.IsNullOrEmpty( dir ) )
        {
            Console.WriteLine( "Application data directory is not available." );
            throw new InvalidOperationException( "Cannot determine application data directory." );
        }

        Directory.CreateDirectory( dir );

        return Path.Combine( dir, "scaddonpath.txt" );
    }

    public static void SaveAddonPath()
    {
        File.WriteAllText( GetAddonPathConfigFile(), AddonPath ?? "" );
    }

    public static bool LoadAddonPath()
    {
        string fileConfig = GetAddonPathConfigFile();

        if ( string.IsNullOrEmpty( fileConfig ) )
        {
            Console.WriteLine( "Addon path config file is not available." );
            return false;
        }

        if ( File.Exists( fileConfig ) )
        {
            try
            {
                strAddonPath = File.ReadAllText( fileConfig ).Trim();
                return true;
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"Error reading addon path from config file: {ex.Message}" );
                strAddonPath = string.Empty;
                return false;
            }
        }

        return false;
    }

    public static void Setup()
    {
        if ( !LoadAddonPath() )
            Console.WriteLine( "No addon path found in config, trying to find svencoop_addon directory." );

        if ( string.IsNullOrEmpty( strAddonPath ) )
        {
            // Try to find the svencoop.exe file on any drive
            Console.WriteLine( "Searching for svencoop.exe..." );
            string? found = FindFileOnAnyDrive( "svencoop", ".exe" );

            if ( !string.IsNullOrEmpty( found ) )
            {
                string? addonDir = Path.Combine( Path.GetDirectoryName( found ), "svencoop_addon" );
                AddonPath = addonDir;
            }
        }

        Console.WriteLine( "SCAddonInstaller initialised." );
    }

    public static string? FindFileOnAnyDrive(string fileName, string extension)
    {
        if ( string.IsNullOrEmpty( fileName ) || string.IsNullOrEmpty( extension ) )
            throw new ArgumentException( "File name or extension cannot be null or empty." );

        foreach ( var drive in DriveInfo.GetDrives().Where( d => d.IsReady ) )
        {
            try
            {
                string? result = FindFileRecursive( drive.RootDirectory.FullName, fileName, extension );

                if ( result is not null )
                    return result;
            }
            catch
            {
                // Ignore drives we can't access
            }
        }

        return null;
    }

    private static string? FindFileRecursive(string directory, string fileName, string extension)
    {
        if ( string.IsNullOrEmpty( directory ) || string.IsNullOrEmpty( fileName ) || string.IsNullOrEmpty( extension ) )
            throw new ArgumentException( "Directory, file name, or extension cannot be null or empty." );

        try// Let's try to access the directory and its contents
        {
            foreach ( var file in Directory.EnumerateFiles( directory, $"{fileName}{extension}", SearchOption.TopDirectoryOnly ) )
                return file;

            foreach ( var dir in Directory.EnumerateDirectories( directory ) )
            {
                string? result = FindFileRecursive( dir, fileName, extension );

                if ( result is not null )
                    return result;
            }
        }
        catch
        {
            // Ignore folders we can't access
        }

        return null;
    }
};
