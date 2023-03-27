using System;
using System.Windows;
using Microsoft.Win32;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.UI.Common.Classes

{
    public class FolderSelectDialog
    {
        private readonly OpenFileDialog _ofd;

        public FolderSelectDialog()
        {
            _ofd = new OpenFileDialog
            {
                Title = "Select a folder",
                Filter = "Folders|\n",
                AddExtension = false,
                CheckFileExists = false,
                DereferenceLinks = true,
                Multiselect = false,
                InitialDirectory = Environment.CurrentDirectory
            };
        }

        public string Description
        {
            get => _ofd.Title;
            set => _ofd.Title = value;
        }

        public string InitialDirectory
        {
            get => _ofd.Title;
            set => _ofd.InitialDirectory = value;
        }

        public string SelectedPath { get; private set; }

        private IntPtr WindowToIntPtr(Window win)
        {
            if (win == null) return IntPtr.Zero;

            var interopHelper = new System.Windows.Interop.WindowInteropHelper(win);
            return interopHelper.Handle;
        }

        public bool ShowDialog(Window owner)
        {
#pragma warning disable CS0219 // Variable is assigned but its value is never used
            //S'assurer que WindowsBase est loader dans l'assembly.
            var tmp = new System.Windows.Interop.MSG();
#pragma warning restore CS0219 // Variable is assigned but its value is never used

            var pf = new Reflector("PresentationFramework");
            var wb = new Reflector("WindowsBase");

            var typeIFileDialog = pf.GetType("MS.Internal.AppModel.IFileDialog");
            var dialog = pf.Call(_ofd, "CreateVistaDialog");
            pf.Call(_ofd, "PrepareVistaDialog", dialog);

            var options = (uint)(int)pf.Call(_ofd, "get_Options");
            options |= (uint)wb.GetEnum("MS.Internal.Interop.FOS", "PICKFOLDERS");
            pf.CallAs(typeIFileDialog, dialog, "SetOptions", options);

            var ownerHandle = WindowToIntPtr(owner);

            var result = pf.CallAs(typeIFileDialog, dialog, "Show", ownerHandle);
            var flag = ((int)pf.Get(result, "Code") == 0);

            if (flag)
            {
                var folder = pf.CallAs(typeIFileDialog, dialog, "GetResult");
                var typeIShellItem = pf.GetType("MS.Internal.AppModel.IShellItem");
                var sigdn = wb.GetEnum("MS.Internal.Interop.SIGDN", "DESKTOPABSOLUTEPARSING");
                SelectedPath = (string)wb.CallAs(typeIShellItem, folder, "GetDisplayName", sigdn);
            }

            return flag;
        }
    }
}
