﻿//------------------------------------------------------------------------------
// <copyright file="MigrateResourcesCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Reflection;
using EnvDTE;
using System.IO;

namespace ResourceMigratorExtension
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class MigrateResourcesCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("73d5da5f-0b3d-4af1-ad0f-7f0729dce8f1");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrateResourcesCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private MigrateResourcesCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static MigrateResourcesCommand Instance {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider {
            get {
                return package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new MigrateResourcesCommand(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            // Get the current project's Solution Directory
            string solutionDirectory = null;
            try
            {
                DTE dte = (DTE) ServiceProvider.GetService(typeof(DTE));
                solutionDirectory = Path.GetDirectoryName(dte.Solution.FullName);
            }
            catch
            {
                VsShellUtilities.ShowMessageBox(
                    ServiceProvider,
                    "It doesn't appear that a Solution is currently open.",
                    "No Solution Error",
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST
                );
            }

            // If we failed to get the Solution Directory, quit
            if(solutionDirectory == null)
            {
                return;
            }

            // Try to run the ResourceMigrator
            try
            {
                var assemblyVersion = Assembly.GetAssembly(typeof(MigrateResourcesCommand)).GetName().Version.ToString();
                new ResourceMigrator.ResourceMigrator(assemblyVersion, "ResourceMigrator VisualStudio Extension", solutionDirectory);
            }
            catch(Exception ex)
            {
                VsShellUtilities.ShowMessageBox(
                    ServiceProvider,
                    $"ResourceMigrator encountered an exception: {ex.Message}",
                    "ResourceMigrator Failed",
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST
                );
            }
        }
    }
}
