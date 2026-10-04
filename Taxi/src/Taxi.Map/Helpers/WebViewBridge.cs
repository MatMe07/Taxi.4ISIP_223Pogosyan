using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Wpf;

namespace Taxi.src.Taxi.Map.Helpers

{
    internal class WebViewBridge
    {
        private readonly WebView2 _webView;

        public WebViewBridge(WebView2 webView)
        {
            _webView = webView;
        }

        public Task<string> CallJsAsync(string function, string args = "")
        {
            if (_webView?.CoreWebView2 == null)
                return null;

            return _webView.CoreWebView2.ExecuteScriptAsync($"{function}({args})");
        }


        public async Task ShowNoticeAsync(string message)
        {
            await CallJsAsync("showNotice", JsHelper.JsStr(message));
        }

        public Task SetPointAsync(string which, RoutePoint point, ref RoutePoint _from, ref RoutePoint _to)
        {
            if (which == "from")
                _from = point;
            else
                _to = point;
            var jsArgs = $"{JsHelper.JsStr(which)},{JsHelper.JsNum(point.Lat)},{JsHelper.JsNum(point.Lon)},{JsHelper.JsStr(point.Address)}";
            return CallJsAsync("setPoint", jsArgs);
        }
    }
}
