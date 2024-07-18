using SharpEngineEditorControls.Constants;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace SharpEngineEditorControls.Controls
{
    /// <summary>
    /// Interaction logic for GameObjectElement.xaml
    /// </summary>
    public partial class GameObjectElement : UserControl
    {
        private SolidColorBrush _bgBrushNormal;
        private SolidColorBrush _bgBrush;

        private SolidColorBrush _bgBrushDark;
        private SolidColorBrush _bgBrushBlack;
        private SolidColorBrush _bgBrushUnactive;

        private string _name = "Game Object";

        public bool Selected { get; private set; }
        public bool IsActive { get; private set; } = true;
        public string TextName
        {
            get
            {
                return _name;
            }
            set
            {
                Debug.Assert(value != null);

                NameText.Text = value;

                OnNameTextChanged?.Invoke(this, value);
            }
        }

        public event Action<GameObjectElement> OnDeleteClicked;
        public event Action<GameObjectElement, bool> OnActiveStateChanged;
        public event Action<GameObjectElement, string> OnNameTextChanged;

        public void SetActive(bool active)
        {
            IsActive = active;

            if (active)
            {
                _bgBrush = _bgBrushNormal;
                UpdateBackground();
            }
            else
            {
                _bgBrush = _bgBrushUnactive;
                UpdateBackground();
            }

            OnActiveStateChanged?.Invoke(this, active);
        }

        private void UpdateBackground()
        {
            UpdateBackground(_bgBrush);
        }
        private void UpdateBackground(SolidColorBrush brush)
        {
            Debug.Assert(brush != null);

            Background = brush;
        }

        public GameObjectElement()
        {
            InitializeComponent();

            _bgBrushDark = (SolidColorBrush)FindResource(ResourceKeys.GAMEOBJECT_ELEMENT_ONMOUSE_ENTER_SOLID_BRUSH_KEY);
            _bgBrushBlack = (SolidColorBrush)FindResource(ResourceKeys.GAMEOBJECT_ELEMENT_ONMOUSE_CLICK_SOLID_BRUSH_KEY);
            _bgBrushNormal = (SolidColorBrush)FindResource(ResourceKeys.ELEMENT_BACKGROUND_SOLID_BRUSH_KEY);
            _bgBrushUnactive = (SolidColorBrush)FindResource(ResourceKeys.GAMEOBJECT_ELEMENT_UNACTIVE__SOLID_BRUSH_KEY);

            _bgBrush = _bgBrushNormal;

            ContextMenu.Opened += OnContextMenuOpen;
            ContextMenu.Closed += OnContextMenuClosed;
        }

        private void OnContextMenuClosed(object sender, RoutedEventArgs e)
        {}

        private void OnContextMenuOpen(object sender, RoutedEventArgs e)
        {}

        private void OnContextMenuDeleteClicked(object sender, RoutedEventArgs e)
        {
            OnDeleteClicked?.Invoke(this);
        }
    }
}
