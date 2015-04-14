﻿using System;
using System.Windows;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.ServiceLocation;

namespace Finances
{
	class Bootstrapper : UnityBootstrapper
	{
		protected override DependencyObject CreateShell ()
		{
			return ServiceLocator.Current.GetInstance<Shell>();
		}

		//------------------------------------------------------------------
		protected override void InitializeShell ()
		{
			base.InitializeShell();

			Application.Current.MainWindow = (Window) Shell;
			Application.Current.MainWindow.Show();
		}

		//------------------------------------------------------------------
		protected override void ConfigureModuleCatalog ()
		{
			base.ConfigureModuleCatalog();

			Type tracker = typeof(Tracker.Module);

			ModuleCatalog.AddModule(
			  new ModuleInfo()
			  {
				  ModuleName = tracker.Name,
				  ModuleType = tracker.AssemblyQualifiedName,
			  });
		}
	}
}