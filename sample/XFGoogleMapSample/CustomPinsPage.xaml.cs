using System;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using XFGoogleMapSample.Views;

namespace XFGoogleMapSample
{
    public partial class CustomPinsPage : ContentPage
    {
        bool _dirty;

        // default marker colors
        readonly Tuple<string, Color>[] _colors =
        {
            new Tuple<string, Color>("Green", Color.Green),
            new Tuple<string, Color>("Pink", Color.Pink),
            new Tuple<string, Color>("Aqua", Color.Aqua)
        };

        // bundle(Android:Asset, iOS:Bundle) images
        readonly string[] _bundles =
        {
            "image01.png",
            "image02.png",
            "image03.png"
        };

        // PCL side embedded resources
        readonly string[] _streams =
        {
            "marker01.png",
            "marker02.png",
            "marker03.png"
        };

        // The pin
        readonly Pin _pinTokyo = new Pin()
        {
            Type = PinType.Place,
            Label = "Tokyo SKYTREE",
            Address = "Sumida-ku, Tokyo, Japan",
            Position = new Position(35.71d, 139.81d)
        };

        // The second pin
        readonly Pin _pinTokyo2 = new Pin()
        {
            Icon = BitmapDescriptorFactory.DefaultMarker(Color.Gray),
            Type = PinType.Place,
            Label = "Second Pin",
            Position = new Position(35.71d, 139.815d),
            ZIndex = 5
        };

        public CustomPinsPage()
        {
            InitializeComponent();
            Switch[] switches = ToggleSwitches();

            OnPinColorChanged();
            OnPinImageChanged();
            OnPinStreamChanged();
            OnPinNullClicked(switches);
            OnPinDraggableClicked();
            OnPinRotationClicked();
            OnPinTransparencyChanged();

            // ZIndex
            buttonMoveToFront.Clicked += (sender, e) =>
            {
                map.SelectedPin = null;
                _pinTokyo.ZIndex = _pinTokyo2.ZIndex + 1;
            };
            buttonMoveToBack.Clicked += (sender, e) =>
            {
                map.SelectedPin = null;
                _pinTokyo.ZIndex = _pinTokyo2.ZIndex - 1;
            };

            // MapToolbarEnabled
            map.UiSettings.MapToolbarEnabled = true;
            switchMapToolbarEnabled.Toggled += (sender, e) =>
            {
                map.UiSettings.MapToolbarEnabled = e.Value;
            };
            switchMapToolbarEnabled.IsToggled = map.UiSettings.MapToolbarEnabled;

            map.PinDragStart += (_, e) => labelDragStatus.Text = $"DragStart - {PrintPin(e.Pin)}";
            map.PinDragging += (_, e) => labelDragStatus.Text = $"Dragging - {PrintPin(e.Pin)}";
            map.PinDragEnd += (_, e) => labelDragStatus.Text = $"DragEnd - {PrintPin(e.Pin)}";

            switchIsDraggable.IsToggled = true;

            switchPinColor.IsToggled = true;

            _pinTokyo.IsDraggable = true;
            map.Pins.Add(_pinTokyo);
            map.Pins.Add(_pinTokyo2);
            map.SelectedPin = _pinTokyo;
            map.MoveToRegion(MapSpan.FromCenterAndRadius(_pinTokyo.Position, Distance.FromMeters(10000)));
        }

        private void OnPinTransparencyChanged()
        {
            // Pin Transparency
            sliderTransparency.ValueChanged += (sender, e) =>
            {
                _pinTokyo.Transparency = (float)(e.NewValue / 10f);
            };
            _pinTokyo.Transparency = (float)(sliderTransparency.Value / 10f);
        }

        private void OnPinRotationClicked()
        {
            // Pin Rotation
            sliderRotation.ValueChanged += (sender, e) =>
            {
                _pinTokyo.Rotation = (float)e.NewValue;

                if (_pinTokyo.Rotation >= 0 && _pinTokyo.Rotation <= 60)
                {
                    _pinTokyo.InfoWindowAnchor = new Point(0.5, 0.0);
                }

                if (_pinTokyo.Rotation > 60 && _pinTokyo.Rotation <= 120)
                {
                    _pinTokyo.InfoWindowAnchor = new Point(0.0, 0.5);
                }

                if (_pinTokyo.Rotation > 120 && _pinTokyo.Rotation <= 210)
                {
                    _pinTokyo.InfoWindowAnchor = new Point(0.5, 1.0);
                }

                if (_pinTokyo.Rotation > 210 && _pinTokyo.Rotation < 270)
                {
                    _pinTokyo.InfoWindowAnchor = new Point(1.0, 0.25);
                }

                if (_pinTokyo.Rotation > 270 && _pinTokyo.Rotation < 360)
                {
                    _pinTokyo.InfoWindowAnchor = new Point(0.5, 0.0);
                }
            };
        }

        private void OnPinDraggableClicked()
        {
            // Pin Draggable
            switchIsDraggable.Toggled += (sender, e) =>
            {
                _pinTokyo.IsDraggable = switchIsDraggable.IsToggled;
            };

            switchFlat.Toggled += (sender, e) =>
            {
                _pinTokyo.Flat = switchFlat.IsToggled;
            };
        }

        private void OnPinNullClicked(Switch[] switches)
        {
            // Set to null
            buttonPinSetToNull.Clicked += (sender, e) =>
            {
                _pinTokyo.Icon = null;
                foreach (var sw in switches)
                {
                    sw.IsToggled = false;
                }
            };
        }

        private void OnPinStreamChanged()
        {
            // Stream Images
            foreach (var stream in _streams)
            {
                buttonPinStream.Items.Add(stream);
            }

            buttonPinStream.SelectedIndexChanged += (_, e) =>
            {
                UpdatePinIcon();
            };
            buttonPinStream.SelectedIndex = 0;
        }

        private void OnPinImageChanged()
        {
            // Bundle Images
            foreach (var bundle in _bundles)
            {
                buttonPinBundle.Items.Add(bundle);
            }

            buttonPinBundle.SelectedIndexChanged += (_, e) =>
            {
                UpdatePinIcon();
            };
            buttonPinBundle.SelectedIndex = 0;
        }

        private void OnPinColorChanged()
        {
            // Default colors
            foreach (var c in _colors)
            {
                buttonPinColor.Items.Add(c.Item1);
            }

            buttonPinColor.SelectedIndexChanged += (_, e) =>
            {
                buttonPinColor.BackgroundColor = _colors[buttonPinColor.SelectedIndex].Item2;
                UpdatePinIcon();
            };
            buttonPinColor.SelectedIndex = 0;
        }

        private Switch[] ToggleSwitches()
        {
            try
            {
                // Switch contols as toggle
                var switches = new Xamarin.Forms.Switch[] { switchPinColor, switchPinBundle, switchPinStream, fromViewSwitch };
                foreach (var sw in switches)
                {
                    sw.Toggled += (sender, e) =>
                    {
                        if (!e.Value || _dirty)
                            return;

                        _dirty = true;
                        foreach (var s in switches)
                        {
                            if (!object.ReferenceEquals(s, sender))
                                s.IsToggled = false;
                        }
                        _dirty = false;

                        UpdatePinIcon();
                    };
                }

                return switches;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        private string PrintPin(Pin pin)
        {
            return $"{pin.Label}({pin.Position.Latitude.ToString("0.000")},{pin.Position.Longitude.ToString("0.000")})";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        void UpdatePinIcon()
        {
            try
            {
                if (switchPinColor.IsToggled)
                {
                    _pinTokyo.Icon = BitmapDescriptorFactory.DefaultMarker(_colors[buttonPinColor.SelectedIndex].Item2);
                }
                else if (switchPinBundle.IsToggled)
                {
                    _pinTokyo.Icon = BitmapDescriptorFactory.FromBundle(buttonPinBundle.Items[buttonPinBundle.SelectedIndex]);
                }
                else if (switchPinStream.IsToggled)
                {
                    var assembly = typeof(CustomPinsPage).GetTypeInfo().Assembly;
                    var file = buttonPinStream.Items[buttonPinStream.SelectedIndex];
                    var stream = assembly.GetManifestResourceStream($"XFGoogleMapSample.{file}") ?? assembly.GetManifestResourceStream($"XFGoogleMapSample.local.{file}");
                    _pinTokyo.Icon = BitmapDescriptorFactory.FromStream(stream, id: file);
                }
                else if (fromViewSwitch.IsToggled)
                {
                    _pinTokyo.Icon = BitmapDescriptorFactory.FromView(new RoundPinView());
                }

                map.MoveToRegion(MapSpan.FromCenterAndRadius(_pinTokyo.Position, Distance.FromMeters(5000)));
            }
            catch(Exception ex)
            {

            }
        }
   }
}

