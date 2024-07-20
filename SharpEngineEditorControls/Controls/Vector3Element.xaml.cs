using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
    /// Interaction logic for Vector3Element.xaml
    /// </summary>
    public partial class Vector3Element : UserControl
    {
        public event Action<Vector3Element, Vector3> OnChange;

        public Vector3 Value
        {
            get => (Vector3)GetValue(ValueProperty);
            set
            {
                SetValue(ValueProperty, value);

                var vector = (Vector3)value;

                X.Value = Convert.ToDouble(vector.X);
                Y.Value = Convert.ToDouble(vector.Y);
                Z.Value = Convert.ToDouble(vector.Z);
            }
        }

        public static readonly DependencyProperty ValueProperty = 
            DependencyProperty.Register(nameof(Value), typeof(Vector3), typeof(Vector3Element));

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            X.Minimum = float.MinValue;
            X.Maximum = float.MaxValue;

            Y.Minimum = float.MinValue;
            Y.Maximum = float.MaxValue;

            Z.Minimum = float.MinValue;
            Z.Maximum = float.MaxValue;

            X.ValueChanged += OnValueChanged;
            Y.ValueChanged += OnValueChanged;
            Z.ValueChanged += OnValueChanged;
        }

        private void OnValueChanged(object sender, HandyControl.Data.FunctionEventArgs<double> e)
        {
            var x = (float)X.Value;
            var y = (float)Y.Value;
            var z = (float)Z.Value;

            var vector = new Vector3(x, y, z);

            Value = vector;
            OnChange?.Invoke(this, Value);
        }

        public Vector3Element()
        {
            InitializeComponent();
        }
    }
}
