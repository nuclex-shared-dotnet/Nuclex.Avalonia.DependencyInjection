#region Apache License 2.0
/*
Nuclex Foundation libraries for .NET
Copyright (C) 2002-2025 Markus Ewald / Nuclex Development Labs

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion // Apache License 2.0

using System;

using Microsoft.Extensions.DependencyInjection;

using Nuclex.Avalonia.AutoBinding;
using Nuclex.Avalonia.CommonDialogs;
using Nuclex.Avalonia.Messages;

namespace Nuclex.Avalonia.DependencyInjection {

  /// <summary>Sets up the service bindings for an MVVM-based application</summary>
  public static class MvvmExtensions {

    /// <summary>Registers all MVVM supporting services for a WinForms application</summary>
    /// <param name="services">Service collection the services will be registered to</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddMvvm(this IServiceCollection services) {

      // Use the convention-over-configuration binder as the default auto binder
      services.AddSingleton<IAutoBinder, ConventionBinder>();

      // The window manager keeps track of which Window is in the foreground
      // and handles opening modal or modeless windows for which it either
      // binds provided view models or requests new instances.
      services.AddSingleton<WindowManager, DependencyInjectedWindowManager>();

      // The IWindowManager is the main interface that should be used to
      // create new windows and dialogs
      services.AddSingleton<IWindowManager>(
        sp => sp.GetRequiredService<WindowManager>()
      );

      // The IActiveWindowTracker is a very simple interface to let Windows Forms
      // extensions that need to display message boxes or other things query for
      // the currently active top-level window.
      services.AddSingleton<IActiveWindowTracker>(
        sp => sp.GetRequiredService<WindowManager>()
      );

      // The auto binder uses convention-over-configuration to automatically
      // establish data bindings or call view model methods that share their name
      // with button controls.
      services.AddSingleton<IAutoBinder, ConventionBinder>();

      // The file dialogs are Avalonia's variant of the standard file and directory
      // picker dialogs, designed to be able to work on classic desktop platforms with
      // a user-facing file system as well as on app store and web applications where
      // local file storage may need to go through custom platform-specific APIs.
      services.AddSingleton<AvaloniaFileDialogs>();

      // Provide the file and directory picker services tjhrough our AvaloniaFileDialogs
      // service which handles both implementations.
      services.AddSingleton<IFilePickerService>(
        sp => sp.GetRequiredService<AvaloniaFileDialogs>()
      );
      services.AddSingleton<IDirectoryPickerService>(
        sp => sp.GetRequiredService<AvaloniaFileDialogs>()
      );

      return services;

    }

    /// <summary>Registers all MVVM supporting services for a WinForms application</summary>
    /// <param name="services">Service collection the services will be registered to</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddMessageDisplay(this IServiceCollection services) {

      // Allow displaying message boxes via the AvaloniaMessagePresenter which wraps
      // the MessageBox.Avalonia library
      services.AddSingleton<IMessageService, AvaloniaMessagePresenter>();

      return services;

    }

  }

} // namespace Nuclex.Avalonia.DependencyInjection
