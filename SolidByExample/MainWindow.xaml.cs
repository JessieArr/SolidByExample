using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RestSharp;
using SolidByExample.Logging;

namespace SolidByExample
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private IRestService _restService;
		private const string _ApiUrl = "http://jsonplaceholder.typicode.com";
		public MainWindow()
		{
			InitializeComponent();
			// The application now controls the log name and API URL. This would
			// enable consumers of our RestService to change between testing and
			// production endpoints, and control our log name!
			_restService = new RestService(new RestClient(_ApiUrl), new NLogLogHelper("logName"));
        }

		private void PostButton_Click(object sender, RoutedEventArgs e)
		{
			var postNumberToFetch = InputTextBox.Text;
			int numericValue;
			var succeeded = Int32.TryParse(postNumberToFetch, out numericValue);
			if (!succeeded)
			{
				ErrorLabel.Visibility = Visibility.Visible;
				return;
			}

			var post = _restService.GetPost(numericValue);
			var user = _restService.GetUser(post.userId);
			 
			TitleLabel.Content = post.title;
			BodyLabel.Content = post.body;

			UserLabel.Content = "Posted by: " + user.name + " - " + user.email;
		}
	}
}
