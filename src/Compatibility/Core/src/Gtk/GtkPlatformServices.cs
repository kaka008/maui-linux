using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Gtk;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Graphics;
using Action = System.Action;

namespace Microsoft.Maui.Controls.Compatibility
{

	public class GtkPlatformServices : IPlatformServices
	{

		public bool IsInvokeRequired => Thread.CurrentThread.IsBackground;

		public void BeginInvokeOnMainThread(Action action)
		{
			MauiGtkApplication.Invoke(action);
		}
		public Assembly[] GetAssemblies()
		{
			return AppDomain.CurrentDomain.GetAssemblies();
		}

		public string GetHash(string input)
		{
			return Internals.Crc64.GetHash(input);
		}

		public double GetNamedSize(NamedSize size, Type targetElementType, bool useOldSizes)
		{
			switch (size)
			{
				case NamedSize.Default:
					return 11;
				case NamedSize.Micro:
				case NamedSize.Caption:
					return 12;
				case NamedSize.Medium:
					return 17;
				case NamedSize.Large:
					return 22;
				case NamedSize.Small:
				case NamedSize.Body:
					return 14;
				case NamedSize.Header:
					return 46;
				case NamedSize.Subtitle:
					return 20;
				case NamedSize.Title:
					return 24;
				default:
					throw new ArgumentOutOfRangeException(nameof(size));
			}
		}

		public Color GetNamedColor(string name)
		{
			throw new NotImplementedException();
		}

		public OSAppTheme RequestedTheme { get; set; }

		public async Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken)

		{
			using var client = new System.Net.Http.HttpClient();

			// Do not remove this await otherwise the client will dispose before
			// the stream even starts
			var result = await StreamWrapper.GetStreamAsync(uri, cancellationToken, client).ConfigureAwait(false);

			return result;

		}

		public IIsolatedStorageFile GetUserStoreForApplication()
		{
			throw new NotImplementedException();
		}

		public void OpenUriAction(Uri uri)
		{
			throw new NotImplementedException();
		}

		public void StartTimer(TimeSpan interval, Func<bool> callback)
		{
			GLib.Timeout.Add((uint)interval.TotalMilliseconds, () =>
			{
				var result = callback();

				return result;
			});
		}

		public string RuntimePlatform => Device.GTK;

		public void QuitApplication()
		{
			((GLib.Application)MauiGtkApplication.CurrentGtkApplication).Quit();
		}

		public SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
		{
			if (view.Handler?.NativeView is Widget w)
			{
				return view.Handler.GetDesiredSize(widthConstraint, heightConstraint);
			}

			return new SizeRequest();
		}

	}

}