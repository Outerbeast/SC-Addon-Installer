<# Sven Co-op Addon Installer Version 1.0
by Outerbeast #>
Add-Type -Name Window -Namespace Console -MemberDefinition '
[DllImport("Kernel32.dll")]
public static extern IntPtr GetConsoleWindow();
[DllImport("user32.dll")]
public static extern bool ShowWindow(IntPtr hWnd, Int32 nCmdShow);
'
$consolePtr = [Console.Window]::GetConsoleWindow()
[Console.Window]::ShowWindow( $consolePtr, 0 )
[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms")
[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Drawing")

Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName PresentationFramework

$host.ui.RawUI.WindowTitle = "Sven Co-op Addon Installer"

class PathData
{
    static [string] $strAddonsPath
}

$strDataPath = [Environment]::GetFolderPath('LocalApplicationData')
$STR_FILETYPES = @( ".zip", ".7z", ".rar", ".bsp", ".wad", ".res", ".cfg" )

$gui = New-Object System.Windows.Forms.Form
$gui.Text = "Sven Co-op Addon Installer"
$gui.Size = '515, 365'
$gui.StartPosition = "CenterScreen"
$gui.MinimumSize = $gui.Size
$gui.MaximizeBox = $False
$gui.Topmost = $True
 
$label = New-Object Windows.Forms.Label
$label.Location = '20, 20'
$label.AutoSize = $True
$label.Text = "Drop addon files here (zip, 7z, rar, bsp, wad, res, cfg):"

$showpath = New-Object Windows.Forms.Label
$showpath.Location = '20, 260'
$showpath.AutoSize = $True
$showpath.Text = "Path not set."
 
$boxSelectedFiles = New-Object Windows.Forms.ListBox
$boxSelectedFiles.Location = '20, 40'
$boxSelectedFiles.Height = 220
$boxSelectedFiles.Width = 460
$boxSelectedFiles.Anchor = ([System.Windows.Forms.AnchorStyles]::Bottom -bor [System.Windows.Forms.AnchorStyles]::Left -bor [System.Windows.Forms.AnchorStyles]::Right -bor [System.Windows.Forms.AnchorStyles]::Top)
$boxSelectedFiles.IntegralHeight = $False
$boxSelectedFiles.AllowDrop = $True

$btnChangePath = New-Object System.Windows.Forms.Button
$btnChangePath.Location = '20, 280'
$btnChangePath.Size = '75, 25'
$btnChangePath.Text = "Select path"
$btnChangePath.Add_Click( { ChangePath } )

$btnAdd = New-Object System.Windows.Forms.Button
$btnAdd.Location = '235, 280'
$btnAdd.Size = '75, 25'
$btnAdd.Text = "Add files"
$btnAdd.Add_Click( { AddFiles } )

$btnClear = New-Object System.Windows.Forms.Button
$btnClear.Location = '320, 280'
$btnClear.Size = '75, 25'
$btnClear.Text = "Clear"
$btnClear.Add_Click( { $boxSelectedFiles.Items.Clear(); $statusBar.Text = ( "Maps selected: $( $boxSelectedFiles.Items.Count )" ) } )

$btnInstall = New-Object System.Windows.Forms.Button
$btnInstall.Location = '405, 280'
$btnInstall.Size = '75, 25'
$btnInstall.Text = "Install"
$btnInstall.Add_Click( { Install } )

$btnHelp = New-Object System.Windows.Forms.Button
$btnHelp.Location = '453, 10'
$btnHelp.Size = '25, 25'
$btnHelp.Text = "?"
$btnHelp.Add_Click( { ShowHelp } )

$statusBar = New-Object System.Windows.Forms.StatusBar
$statusBar.Text = "No maps selected"
 
$gui.SuspendLayout()
$gui.Controls.AddRange( @( $btnInstall, $btnClear, $btnAdd, $btnHelp, $btnChangePath, $label, $showpath, $boxSelectedFiles, $statusBar ) )
$gui.ResumeLayout()

function ShowHelp
{
    $strCredit = "Sven Co-op Addon Installer Version 1.0`nCreated by Outerbeast`n`n"
    $strHelpInfo1 = "Easy installer for Sven Co-op addons.`n"
    $strHelpInfo2 = "`n`n-Usage Information-`nDrag and drop Sven Co-op addons files and click Install to install them to the game.`nSupports file formats zip, 7z, rar, bsp, wad, res, cfg"
    $strThanks = "`n`nThank you for using this app!`nIf you'd like to give feedback feel free to put them here: https://github.com/Outerbeast/SC-Addon-Installer/issues"
    [System.Windows.MessageBox]::Show( "$strCredit$strHelpInfo1$strHelpInfo2$strThanks", "Help", "OK", "Question" )
}

function InitSetup
{
    $SetupMsgBox = [System.Windows.Forms.Form]@{
        Text = "Initial Setup               "
        Size = New-Object System.Drawing.Size( 270, 100 )
        Location = New-Object System.Drawing.Size( 20, 20 )
        StartPosition = "CenterScreen"
    }

    $SetupMsgText = [System.Windows.Forms.Label]@{
        Location = New-Object System.Drawing.Size( 20, 20 )
        Text = "Searching for Sven Co-op install location,`nplease wait..."
        AutoSize = $True
        AutoEllipsis = $True
    }

    $SetupMsgBox.Controls.Add( $SetupMsgText )

    if( Test-Path -Path "$strDataPath\sc_install_path.txt"  )
    {
        [PathData]::strAddonsPath = ( Get-Content "$strDataPath\sc_install_path.txt" -Raw ).Trim()

        if( ![PathData]::strAddonsPath )
        {
            SearchSCInstall
        }
    }
    else
    {
        SearchSCInstall
    }

    if( [PathData]::strAddonsPath )
    {
        $path = [PathData]::strAddonsPath
        $showpath.Text = "Installing to: $path"
    }
}

function SearchSCInstall
{
    $SetupMsgBox.Show()
    Start-Sleep -Seconds 1 # otherwise the message label does not draw properly

    $strInstallID ="svencoop.exe"
    $drives = ( get-psdrive | Where-Object { $_.provider -match 'FileSystem' } ).root
    $strSCLocation = Get-ChildItem -Path $drives -Filter $strInstallID -Recurse -ErrorAction SilentlyContinue -Force

    if( !$strSCLocation )
    {
        [System.Windows.MessageBox]::Show( "Sven Co-op install path not found.`nPlease manually set the path to your Sven Co-op addons folder, or reinstall Sven Co-op and try again.", "ERROR", "OK", "Error" )
    }
    else
    {
        [PathData]::strAddonsPath = $strSCLocation.Directory.FullName + "\svencoop_addon"
        [PathData]::strAddonsPath | Out-File -FilePath "$strDataPath\sc_install_path.txt"
    }

    $SetupMsgBox.Close()

    if( [PathData]::strAddonsPath )
    {
        $showpath.Text = "Installing to: $([PathData]::strAddonsPath)"
    }
}
function ChangePath
{
    $FolderBrowser = [System.Windows.Forms.FolderBrowserDialog]@{
        Description = "Select folder"
        ShowNewFolderButton = $True
        SelectedPath = [PathData]::strAddonsPath
    }

    $FolderBrowser.ShowDialog()

    if( $FolderBrowser.SelectedPath )
    {
        [PathData]::strAddonsPath = $FolderBrowser.SelectedPath
        [PathData]::strAddonsPath | Out-File -FilePath "$strDataPath\sc_install_path.txt"
        $showpath.Text = "Installing to: $( [PathData]::strAddonsPath )"
    }
}

function AddFiles
{
    $FileBrowser = [Windows.Forms.OpenFileDialog]@{ 
        InitialDirectory = [System.Environment]::CurrentDirectory
        Filter = "Archive|*.zip;*.7z;*.rar|Map files|*.bsp;*.wad;*.res;*.cfg"
        Multiselect = $True
    }

    $FileBrowser.ShowDialog()

    if( !$FileBrowser.Filenames )
    {
        return
    }

    foreach( $filename in $FileBrowser.Filenames )
    {
        if( !$filename -or $boxSelectedFiles.Items -contains $filename )
        {
            continue;
        }

        $boxSelectedFiles.Items.Add( $filename )
    }

    $statusBar.Text = ( "Maps selected: $( $boxSelectedFiles.Items.Count )" )
}

function Unpack($strArchiveName)
{
    if( !$strArchiveName )
    {
        return
    }

    switch( [System.IO.Path]::GetExtension( $strArchiveName ) )
    {
        ".bsp" { }
        ".cfg" { }
        ".res"
        { 
            Copy-Item -Path $strArchiveName -Destination "$( [PathData]::strAddonsPath )\maps" -Force -Verbose
            break
        }

        ".wad"
        {
            Copy-Item -Path $strArchiveName -Destination "$( [PathData]::strAddonsPath )" -Force -Verbose
            break
        }

        ".7z" { }
        ".rar"
        {
            if( !( Get-Module -Name 7Zip4PowerShell -ListAvailable ) )
            {
                $cInstall7z = [System.Windows.MessageBox]::Show( "7z/rar archives require additonal support installed.`nDo you wish to install support?", "Confirm Action", "YesNo", "Information" )
    
                if( $cInstall7z -eq "Yes" )
                {
                    try
                    {
                        Install-Module 7Zip4PowerShell -Scope CurrentUser -Force
                    }
                    catch
                    {
                        [System.Windows.MessageBox]::Show( "7zip not installed.", "ERROR", "OK", "Error" )
                        return
                    }
                }
                else
                {
                    [System.Windows.MessageBox]::Show( "Skipping install of $strArchivename", "File was skipped", "OK", "Warning" )
                    return
                }
            }
    
            try
            {
                Expand-7Zip -ArchiveFileName $strArchiveName -TargetPath $( [PathData]::strAddonsPath ) -Verbose
            }
            catch
            {
                [System.Windows.MessageBox]::Show( "Mappack file $strArchiveName not found or invalid filetype.", "ERROR", "OK", "Error" )
            }

            break
        }

        ".zip"
        {
            try
            {
                Expand-Archive -LiteralPath $strArchiveName -DestinationPath $( [PathData]::strAddonsPath ) -Force -Verbose
            }
            catch
            {
                [System.Windows.MessageBox]::Show( "Mappack file $strArchiveName not found or invalid filetype.", "ERROR", "OK", "Error" )
            }
        }
    }
}
# Not yet ready for this
<# function DownloadAddon($strURL, $strExt)
{
    $strDownloadedFile = "[PathData]::strAddonsPath\addon.$strExt"
    Invoke-WebRequest $strURL -OutFile $strDownloadedFile -UseBasicParsing
    #(New-Object System.Net.WebClient).DownloadFile( $strURL, $strDownloadedFile )
    Unpack( $strDownloadedFile ) # I need to shove the downloaded filepath and file into this func

    Remove-Item -Path $strDownloadedFile -Force
} #>

function Install
{
    $iAddonsInstalled = 0
    
    if( ![PathData]::strAddonsPath )
    {
        [System.Windows.MessageBox]::Show( "No path set for files.`nPlease select a path to install files.", "No valid path set", "OK", "Information" )
        return
    }

    if( !$boxSelectedFiles.Items )
    {
        [System.Windows.MessageBox]::Show( "No files were added.`nDrag files into to the box or use the Add files button.", "Add files to install", "OK", "Information" )
        return
    }

	foreach( $item in $boxSelectedFiles.Items )
    {
        Unpack( $item )
        $iAddonsInstalled++
	}
 
    $statusBar.Text = ( "Maps selected: $( $boxSelectedFiles.Items.Count )" )
    [System.Windows.MessageBox]::Show( "$iAddonsInstalled items were installed.", "Finished installation", "OK", "Information" )
}

$boxSelectedFiles_DragOver = [System.Windows.Forms.DragEventHandler] {
	if( $_.Data.GetDataPresent([Windows.Forms.DataFormats]::FileDrop ) )
	{
	    $_.Effect = 'Copy'
	}
	else
	{
	    $_.Effect = 'None'
	}
}
	
$boxSelectedFiles_DragDrop = [System.Windows.Forms.DragEventHandler] {
	foreach( $filename in $_.Data.GetData( [Windows.Forms.DataFormats]::FileDrop ) )
    {
        if( $boxSelectedFiles.Items -contains $filename -or !$STR_FILETYPES.Contains( [System.IO.Path]::GetExtension( $filename ) ) )
        {
            continue
        }

        $boxSelectedFiles.Items.Add( $filename )       
	}

    $statusBar.Text = ( "Maps selected: $( $boxSelectedFiles.Items.Count )" )
}

function Close
{
    try
    {
        $boxSelectedFiles.remove_Click($btnInstall_Click)
		$boxSelectedFiles.remove_DragOver($boxSelectedFiles_DragOver)
		$boxSelectedFiles.remove_DragDrop($boxSelectedFiles_DragDrop)
	}
	catch [Exception] { }
}

$gui.Add_FormClosed( { Close } )
$boxSelectedFiles.Add_DragOver($boxSelectedFiles_DragOver)
$boxSelectedFiles.Add_DragDrop($boxSelectedFiles_DragDrop)

InitSetup
$gui.ShowDialog()
