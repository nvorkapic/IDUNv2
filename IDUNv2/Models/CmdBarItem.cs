using IDUNv2.Common;
using System;
using Windows.UI.Xaml.Controls;

namespace IDUNv2.Models
{
    public class CmdBarItem
    {
        public AppBarButton Btn { get; private set; }

        public CmdBarItem(Symbol sym, string label, Action<object> cmdFunc, object cmdParam = null)
        {
            Btn = new AppBarButton
            {
                Icon = new SymbolIcon(sym),
                Label = label,
                Command = new ActionCommand<object>(cmdFunc),
                CommandParameter = cmdParam
            };
        }
    }
}
