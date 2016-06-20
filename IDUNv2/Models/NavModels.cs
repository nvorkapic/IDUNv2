using System;
using Windows.UI.Xaml.Controls;

namespace IDUNv2.Models
{
    public class NavLinkItem
    {
        public string Title { get; set; }
        public Type PageType { get; set; }
        public object Param { get; set; }

        public NavLinkItem(string title, Type pageType, object param = null)
        {
            Title = title;
            PageType = pageType;
            Param = param;
        }
    }

    public class NavMenuItem
    {
        public string Label { get; set; }
        public Symbol Symbol { get; set; }
        public char SymbolChar { get { return (char)this.Symbol; } }
        public Type PageType { get; set; }
    }
}
