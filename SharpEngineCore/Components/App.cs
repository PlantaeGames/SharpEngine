using SharpEngineCore.Graphics;

namespace SharpEngineCore.Components;

internal sealed class App
{
    private class AppContext : ApplicationContext
    {
        private const string WINDOW_NAME = "Sharp Engine";

        private Logger _logger;

        public AppContext()
        {
            _logger = new Logger();

            Application.ApplicationExit += OnExit;
            Application.Idle += OnIdle;

            CreateWindow();
        }

        private void OnIdle(object? sender, EventArgs e)
        {
            // we r ready to work here.
            _logger.LogMessage("Tick.");
        }

        private void CreateWindow()
        {
            var mainWindow = new MainWindow(WINDOW_NAME, new Point(0,0), new Size(800, 600));
            mainWindow.FormClosed += OnWindowClosed;

            _logger.LogMessage("Main Window Created.");
            mainWindow.Show();
        }

        private void OnWindowClosed(object? sender, FormClosedEventArgs e)
        {
            _logger.LogMessage("Main Window Closed.");
            ExitThread();
        }

        private void OnExit(object? sender, EventArgs e)
        {
            _logger.LogMessage("Application Exiting...");
        }
    }

    public ApplicationContext Context => _context;
    private AppContext _context;

    public App()
    {
        _context = new AppContext();
    }
}