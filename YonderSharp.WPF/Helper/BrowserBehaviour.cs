using System.Windows;
using System.Windows.Controls;

namespace YonderSharp.WPF.Helper {
    //https://stackoverflow.com/a/4204350/1171328
    /// <summary>
    /// For MVM binding of an html string to a webbrowser element
    /// <WebBrowser local:BrowserBehavior.Html="{Binding MyHtmlString}" />
    /// </summary>
    public class BrowserBehavior {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
                "Html",
                typeof(string),
                typeof(BrowserBehavior),
                new FrameworkPropertyMetadata(OnHtmlChanged));

        [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
        public static string GetHtml(WebBrowser d) {
            return (string)d.GetValue(HtmlProperty);
        }

        public static void SetHtml(WebBrowser d, string value) {
            d.SetValue(HtmlProperty, value);
        }

        static void OnHtmlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e) {
            WebBrowser webBrowser = dependencyObject as WebBrowser;
            if(webBrowser != null)
                webBrowser.NavigateToString(e.NewValue as string ?? "&nbsp;");
        }
    }
}