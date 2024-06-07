using System;
using System.Diagnostics;
using System.Windows.Input;
using ReactiveUI;

namespace KeepMeOnline.ViewModel
{
	public class AboutViewModel : ReactiveObject
	{
		public ICommand HyperLinkCommand { get; }

        public AboutViewModel()
		{
			HyperLinkCommand = ReactiveCommand.Create(OpenHyperLink);
		}

		private void OpenHyperLink()
		{
			var url = "mailto:bhushan7668@gmail.com";

			Process.Start(new ProcessStartInfo
			{
				FileName = url,
				UseShellExecute = true
			});
		}
	}
}

