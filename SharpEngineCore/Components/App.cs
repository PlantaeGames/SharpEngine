namespace SharpEngineCore.Components;

internal class App
{
    private const string NAME = "Sharp Engine";

    private class AppContext : ApplicationContext
    {
        public AppContext()
        {
            Application.ApplicationExit += OnExit;

            var window = new Form();
            window.Text = NAME;

            window.FormClosed += OnWindowClosed;

            window.Show();
        }

        private void OnWindowClosed(object? sender, FormClosedEventArgs e)
        {

            Console.WriteLine("LOG: Window Closed.");

            ExitThread();
        }

        private void OnExit(object? sender, EventArgs e)
        {
            Console.WriteLine("LOG: Application Exited.");
        }
    }

    public ApplicationContext Context => _context;
    private AppContext _context;

    public App()
    {
        _context = new AppContext();
    }
}