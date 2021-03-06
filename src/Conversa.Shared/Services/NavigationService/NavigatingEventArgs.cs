﻿// Licensed under the The Apache License Version 2.0, January 2004
// ---------------------------------------------------------------
// https://github.com/Windows-XAML/Template10
// ---------------------------------------------------------------

using Windows.UI.Xaml.Navigation;

namespace Conversa.Services.NavigationService
{
    // DOCS: https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-NavigationService
    public class NavigatingEventArgs: NavigatedEventArgs
    {
        public NavigatingEventArgs() { }
        public NavigatingEventArgs(NavigatingCancelEventArgs e)
        {
            this.NavigationMode = e.NavigationMode;
            this.PageType = e.SourcePageType;
            this.Parameter = e.Parameter?.ToString();
        }
        public bool Cancel { get; set; } = false;
        public bool Suspending { get; set; } = false;
    }
}