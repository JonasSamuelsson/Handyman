using System;

namespace Handyman.Wpf
{
    public interface ICommandManager
    {
        event EventHandler RequerySuggested;
        void InvalidateRequerySuggested();
    }
}