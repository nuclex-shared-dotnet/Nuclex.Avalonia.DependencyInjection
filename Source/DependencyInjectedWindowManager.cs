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

namespace Nuclex.Avalonia.DependencyInjection {

  /// <summary>
  ///   Window manager that is using Microsoft's dependency injection interfaces
  /// </summary>
  public class DependencyInjectedWindowManager : WindowManager {

    #region class WindowScope

    /// <summary>Manages a window-specific service scope</summary>
    private class WindowScope : IWindowScope, IDisposable {

      /// <summary>Initializes a new service scope for the window</summary>
      /// <param name="serviceProvider">
      ///   Service provider in which to create a scope
      /// </param>
      public WindowScope(IServiceProvider serviceProvider) {
        this.serviceScope = serviceProvider.CreateScope();
      }

      /// <summary>Creates an instance of the specified type in the scope</summary>
      /// <param name="type">Type an instance will be created of</param>
      /// <returns>The created instance</returns>
      /// <remarks>
      ///   Use this to wire up your dependency injection container. By default,
      ///   the Activator class will be used to create instances which only works
      ///   if all of your view models are concrete classes.
      /// </remarks>
      public object CreateInstance(Type type) {
        return this.serviceScope.ServiceProvider.GetRequiredService(type);
      }

      /// <summary>Immediately destroys all services owned by the scope</summary>
      public void Dispose() {
        if(this.serviceScope != null) {
          this.serviceScope.Dispose();
          this.serviceScope = null;
        }
      }

      /// <summary>Service scope that will be used to create instances</summary>
      private IServiceScope serviceScope;

    }

    #endregion // class WindowScope

    /// <summary>Initializes a new window manager</summary>
    ///	<param name="serviceProvider">
    ///	  Dependency injector the window manager uses to construct view models
    ///	</param>
    /// <param name="autoBinder">
    ///   View model binder that will be used to bind all created views to their models
    /// </param>
    public DependencyInjectedWindowManager(
      IServiceProvider serviceProvider, IAutoBinder autoBinder = null
    ) :
      base(autoBinder) {
      this.serviceProvider = serviceProvider;
    }

    /// <summary>Creates an instance of the specified type</summary>
    /// <param name="type">Type an instance will be created of</param>
    /// <returns>The created instance</returns>
    /// <remarks>
    ///   Use this to wire up your dependency injection container. By default,
    ///   the Activator class will be used to create instances which only works
    ///   if all of your view models are concrete classes.
    /// </remarks>
    protected override object CreateInstance(Type type) {
      return this.serviceProvider.GetRequiredService(type);
    }

    /// <summary>Creates a new scope in which window-specific services will live</summary>
    /// <returns>The new scope managing the lifetime of window-specific services</returns>
    protected override IWindowScope CreateWindowScope() {
      return new WindowScope(this.serviceProvider);
    }

    /// <summary>The service provider used to create new instances</summary>
    private readonly IServiceProvider serviceProvider;

  }

} // namespace Nuclex.Avalonia.DependencyInjection
