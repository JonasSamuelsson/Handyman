using System;
using Shouldly;
using Xunit;

namespace Handyman.Wpf.Tests
{
    public class RelayCommandTests
    {
        [Fact]
        public void ReassigningTheObservableShouldTriggerCanExecuteChangedEvent()
        {
            var observable1 = new Observable<bool>();
            var command1 = new RelayCommand(delegate { }, observable1) { CommandManager = new TestCommandManager() };
            var canExecuteChanged1 = false;
            command1.CanExecuteChanged += delegate { canExecuteChanged1 = true; };
            observable1.Value = true;
            canExecuteChanged1.ShouldBe(true);

            var observable2 = new Observable<bool>();
            var command2 = new RelayCommand(delegate { }, observable2) { CommandManager = new TestCommandManager() };
            var canExecuteChanged2 = false;
            command2.CanExecuteChanged += delegate { canExecuteChanged2 = true; };
            observable2.Value = true;
            canExecuteChanged2.ShouldBe(true);
        }

        private class TestCommandManager : ICommandManager
        {
            public event EventHandler RequerySuggested = delegate { };

            public void InvalidateRequerySuggested()
            {
                RequerySuggested(null, EventArgs.Empty);
            }
        }
    }
}