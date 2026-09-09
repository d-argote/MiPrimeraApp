namespace MiPrimeraApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		try
		{
			return new Window(new MainPage());
		}
		catch (Exception ex)
		{
			return new Window(new ContentPage { Content = new Label { Text = ex.ToString(), TextColor = Colors.Red } });
		}
	}
}