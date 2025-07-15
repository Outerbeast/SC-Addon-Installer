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
#pragma warning disable IDE1006

using SCAddonInstaller.src;
using System.ComponentModel;
namespace SCAddonInstaller;

public partial class GUI : Form
{
    private const string
            AppName = "SCAddonInstaller",
            AppVersion = "2.0.0",
            AppDescription = "A simple addon installer for Sven Co-op, supporting various archive formats and direct downloads.",
            AppAuthor = "Outerbeast",
            Credits = "Thank you for using this app!\nIf you'd like to give feedback feel free to put them here: https://github.com/Outerbeast/SC-Addon-Installer/issues";
    private string HelpText = "Drag and drop addon files into the list, or use the buttons to add, remove, clear, or install addons. You can also download addons by adding download links via the ''Add download'' option.";

    private readonly ToolTip hints = new()
    {
        AutoPopDelay = 5000,
        InitialDelay = 1000,
        ReshowDelay = 500
    };

    public StatusStrip statusStrip = new();
    public ToolStripStatusLabel statusLabel = new()
    {
        Name = "toolStripStatusLabel1",
        Text = "No addons selected.",
        Spring = false // Fills available space
    };

    private BindingList<string> addonData = [];

    public GUI()// Constructor
    {
        InitializeComponent();
        this.AcceptButton = btnInstall;// Makes Install the main/default button
        // Show the default help button in the title bar
        this.HelpButton = true;
        this.MaximizeBox = this.MinimizeBox = false;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;

        listAddons.AllowDrop = true;
        listAddons.DragEnter += listAddons_DragEnter;
        listAddons.DragDrop += listAddons_DragDrop;
        this.FormClosing += GUI_FormClosing;

        hints.SetToolTip( btnAdd, "Add addon archives to the list" );
        hints.SetToolTip( btnRemove, "Remove selected addons from the list" );
        hints.SetToolTip( btnClear, "Clear all addons from the list" );
        hints.SetToolTip( btnInstall, "Install all listed addons to the selected directory" );
        hints.SetToolTip( btnDownload, "Download an addon from a URL and add it to the list" );
        hints.SetToolTip( lblInstallPath, "Click to change the addon install path" );

        statusStrip.Items.Add( statusLabel );
        this.Controls.Add( statusStrip );

        addonData.ListChanged += Data_ListChanged;
        listAddons.DataSource = addonData;

        HelpText += "\nSupports the following archive formats:\n";

        foreach ( var archiveType in Unpack.STR_ARCHIVE_TYPES )
            HelpText += $"- {archiveType}\n";

        HelpText += "\nSupports map files:\n";

        foreach ( var mapFileType in Unpack.STR_MAP_FILES )
            HelpText += $"- {mapFileType}\n";
    }

    private void GUI_Load(object sender, EventArgs e)
    {
        Form splashSetupInfo = new();

        if ( OperatingSystem.IsWindowsVersionAtLeast( 6, 1 ) ) // Check platform compatibility  
        {
            splashSetupInfo.FormBorderStyle = FormBorderStyle.None;
            splashSetupInfo.TopMost = true; // Only set TopMost if the platform supports it
            splashSetupInfo.StartPosition = FormStartPosition.CenterScreen; // Only set StartPosition if the platform supports it
            splashSetupInfo.Width = 400;
            splashSetupInfo.Height = 120;
            splashSetupInfo.ShowInTaskbar = false; // Only set ShowInTaskbar if the platform supports it
        }
        else
        {
            throw new PlatformNotSupportedException( "This application requires Windows 6.1 or later." );
        }

        splashSetupInfo.Controls.Add( new Label
        {
            Text = "Searching for Sven Co-op Addon Directory, please wait...\n\nIf you have not set the addon path, the installer will search for it automatically.\nYou can also set it manually later if needed.",
            Dock = DockStyle.Fill,
            TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        } );

        splashSetupInfo.Show();
        Application.DoEvents();

        try
        {
            Config.Setup();
        }
        catch ( Exception ex )
        {
            MessageBox.Show( "Error during setup: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
        }
        finally
        {
            splashSetupInfo.Close();
        }

        if ( string.IsNullOrEmpty( Config.strAddonPath ) )
        {
            MessageBox.Show( "No Sven Co-op Addon Directory found. Please set it manually.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning );
            lblInstallPath.Text = "Please set the addon path";
        }
        else
        {
            lblInstallPath.Text = "Installing to " + Config.strAddonPath;
        }
    }

    protected override void OnHelpButtonClicked(CancelEventArgs e)
    {
        MessageBox.Show( HelpText, AppName + " - Help", MessageBoxButtons.OK, MessageBoxIcon.Question );
        e.Cancel = true;// Prevents the default behavior
        base.OnHelpButtonClicked( e );
    }

    public void AddFilesFromArgs(string[] FILES)
    {
        if ( FILES is null || FILES.Length == 0 )
            return;

        foreach ( var file in FILES )
        {
            if ( string.IsNullOrEmpty( file ) )
                continue;

            if ( !Unpack.IsSupportedMapFile( file ) && !Unpack.IsSupportedArchive( file ) )
                continue;

            if ( !addonData.Contains( file ) )
                addonData.Add( file );
        }
    }

    public bool ParseURLList(string[] URLS)
    {
        if ( URLS is null || URLS.Length == 0 )
            return false;

        foreach ( string file in URLS )
        {
            if ( string.IsNullOrEmpty( file ) )
                continue;

            try
            {
                foreach ( var line in System.IO.File.ReadAllLines( file ) )
                {
                    var currentLine = line?.Trim();

                    if ( string.IsNullOrEmpty( currentLine ) )
                        continue;

                    if ( !addonData.Contains( currentLine ) && Download.IsValidUrl( currentLine ) )
                        addonData.Add( currentLine );
                }

                return true;
            }
            catch ( Exception ex )
            {
                MessageBox.Show( $"Failed to read file: {file}\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return false;
            }
        }

        return false;
    }

    private void lblInstallPath_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        using var browseInstallPath = new FolderBrowserDialog();
        browseInstallPath.Description = "Select the Sven Co-op Addon Directory";
        // Set the initial directory to the current addon path
        if ( Config.strAddonPath != string.Empty )
            browseInstallPath.SelectedPath = Config.strAddonPath;

        if ( browseInstallPath.ShowDialog() == DialogResult.OK )
        {
            Config.strAddonPath = browseInstallPath.SelectedPath;
            lblInstallPath.Text = "Installing to " + Config.strAddonPath;
            MessageBox.Show( $"Addon path set to: {Config.strAddonPath}", "Path Set", MessageBoxButtons.OK, MessageBoxIcon.Information );
        }
        else
            MessageBox.Show( "No path selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information );
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        using OpenFileDialog findAddonFile = new();
        findAddonFile.Title = "Select files to install";
        findAddonFile.InitialDirectory = System.Environment.CurrentDirectory;
        findAddonFile.Multiselect = true;
        // Hurts my eyes, but it works
        findAddonFile.Filter = "Addon Archives (*.zip;*.7z;*.rar;*.tar;*.gz;*.bz2;*.xz;*.zst)|" +
                                        "*.zip;*.7z;*.rar;*.tar;*.gz;*.bz2;*.xz;*.zst|" +
                                        "Map Files (*.bsp;*.wad;*.cfg;*.res)|*.bsp;*.wad;*.cfg;*.res|" +
                                        "All Files (*.*)|*.*";

        if ( findAddonFile.ShowDialog() != DialogResult.OK )
            return;// User cancelled the dialog

        foreach ( string file in findAddonFile.FileNames )
        {
            if ( string.IsNullOrEmpty( file ) )
                continue;

            if ( !listAddons.Items.Contains( file ) )
                addonData.Add( file );
            else
                MessageBox.Show( $"Addon already added: {file}", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information );
        }
    }

    private void btnRemove_Click(object sender, EventArgs e)
    {
        if ( listAddons.Items is null || listAddons.SelectedItems.Count < 1 )
        {
            MessageBox.Show( "Please select addons to remove.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information );
            return;
        }
        // Remove all the selected items
        if ( MessageBox.Show( "Are you sure you want to remove the selected addons?", "Confirm Remove", MessageBoxButtons.YesNo, MessageBoxIcon.Warning ) == DialogResult.Yes )
        {
            if ( listAddons.DataSource is BindingList<string> bindingList )
            {   // Copy selected items to a separate list
                var SELECTED_ITEMS = new List<string>();

                foreach ( var item in listAddons.SelectedItems )
                    SELECTED_ITEMS.Add( item.ToString() );

                if ( SELECTED_ITEMS is null || SELECTED_ITEMS.Count < 1 )
                {
                    MessageBox.Show( "No addons selected to remove.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information );
                    return;
                }
                // Remove each selected item
                foreach ( var item in SELECTED_ITEMS )
                    bindingList.Remove( item );
            }
        }

        statusLabel.Text = listAddons.Items.Count > 0 ? $"{listAddons.Items.Count} addon(s) selected." : "No addons selected.";
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
        if ( listAddons.Items is null || listAddons.Items.Count < 1 )
        {
            MessageBox.Show( "No addons to clear.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information );
            return;
        }

        if ( MessageBox.Show( "Are you sure you want to clear all addons?", "Confirm Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Warning ) == DialogResult.Yes )
        {
            addonData.Clear();
            statusLabel.Text = "No addons selected.";
        }
    }

    private async void btnInstall_Click(object sender, EventArgs e)
    {
        if ( listAddons.Items is null || listAddons.Items.Count < 1 )
        {
            MessageBox.Show( "Please add addons to install.", "No addons selected", MessageBoxButtons.OK, MessageBoxIcon.Warning );
            return;
        }

        this.Enabled = false;
        statusLabel.Text = "Installing, please wait...";

        await Install.RunAsync(
            listAddons.Items,
            Config.strAddonPath,
            null,// No need to report progress
            (msg, caption, icon) => this.Invoke( (Action) ( () => MessageBox.Show( this, msg, caption, MessageBoxButtons.OK, icon ) ) )
        );

        statusLabel.Text = Install.countSuccess < Install.total ?
            $"Processed {Install.countSuccess} out of {Install.total} items successfully." :
            $"All {Install.total} items processed successfully.";

        this.Enabled = true;
    }

    private void listAddons_DragEnter(object sender, DragEventArgs e)
    {   // Check if the data being dragged is a file drop
        e.Effect = e.Data is null || !e.Data.GetDataPresent( DataFormats.FileDrop ) ? DragDropEffects.None : DragDropEffects.Copy;
    }

    private void listAddons_DragDrop(object sender, DragEventArgs e)
    {
        if ( e.Data is null || !e.Data.GetDataPresent( DataFormats.FileDrop ) )
            return;

        string[] FILES = (string[]) e.Data.GetData( DataFormats.FileDrop );

        if ( FILES is null || FILES.Length < 1 )
        {
            MessageBox.Show( "No files dropped.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information );
            return;
        }

        foreach ( string file in FILES )
        {
            if ( string.IsNullOrEmpty( file ) )
                continue; // Skip empty file paths

            if ( !addonData.Contains( file ) )
                addonData.Add( file );
            else
                MessageBox.Show( $"Addon already added: {file}", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information );
        }
    }

    private void listAddons_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    private void btnDownload_Click(object sender, EventArgs e)
    {   // Open dialog to enter URL
        using Form urlForm = new()
        {
            Text = "Add Download",
            StartPosition = FormStartPosition.CenterParent,
            Width = 400,
            Height = 140,
            MinimizeBox = false,
            MaximizeBox = false,
            ShowInTaskbar = false,
            FormBorderStyle = FormBorderStyle.FixedDialog
        };

        Label label = new() { Text = "Enter URL:", Dock = DockStyle.Top };
        TextBox textBox = new() { Dock = DockStyle.Top };
        Button downloadButton = new() { Text = "Add download URL", Dock = DockStyle.Bottom };
        Button addDownloadListButton = new() { Text = "Add download list", Dock = DockStyle.Bottom };

        downloadButton.Click += (s, args) =>
        {
            if ( Download.IsValidUrl( textBox.Text ) )
            {
                addonData.Add( textBox.Text );
                textBox.Clear(); // Clear the text box after adding
            }
            else
                MessageBox.Show( "Invalid URL.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
        };

        addDownloadListButton.Click += (s, args) =>
        {
            using OpenFileDialog dialog = new();
            dialog.Title = "Add download list";
            dialog.InitialDirectory = System.Environment.CurrentDirectory;
            dialog.Multiselect = true;
            dialog.Filter = "Text Files (*.txt;*.csv;*.log;*.json;*.xml)|*.txt;*.csv;*.log;*.json;*.xml|All Files (*.*)|*.*";
            // User cancelled the dialog
            if ( dialog.ShowDialog() != DialogResult.OK )
                return;
            // Parse the txt files for links then add them to the listbox
            if ( ParseURLList( dialog.FileNames ) )
                urlForm.Close();
        };

        urlForm.Controls.Add( downloadButton );
        urlForm.Controls.Add( addDownloadListButton );
        urlForm.Controls.Add( textBox );
        urlForm.Controls.Add( label );
        urlForm.ShowDialog();
    }

    private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        using FolderBrowserDialog browseInstallPath = new();
        browseInstallPath.Description = "Select the Sven Co-op Addon Directory";

        if ( Config.strAddonPath != string.Empty )
            browseInstallPath.SelectedPath = Config.strAddonPath;

        if ( browseInstallPath.ShowDialog() == DialogResult.OK )
        {
            Config.strAddonPath = browseInstallPath.SelectedPath;
            lblInstallPath.Text = $"Installing to {Config.strAddonPath}";
            MessageBox.Show( $"Addon path set to: {Config.strAddonPath}", "Path Set", MessageBoxButtons.OK, MessageBoxIcon.Information );
        }
        else if ( Config.strAddonPath == string.Empty )
            MessageBox.Show( "No path selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
    }

    private void Data_ListChanged(object sender, ListChangedEventArgs e)
    {
        statusLabel.Text = listAddons.Items.Count > 0
            ? $"{listAddons.Items.Count} addon(s) selected."
            : "No addons selected.";
    }

    private void GUI_FormClosing(object sender, FormClosingEventArgs e)
    {
        Config.SaveAddonPath();
    }

    ~GUI() // Destructor
    {
        hints.Dispose();
        statusStrip.Dispose();
        statusLabel.Dispose();
        addonData.ListChanged -= Data_ListChanged;
    }
};
