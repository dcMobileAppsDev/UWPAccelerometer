using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace accelerometer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // accelerometer object
        Accelerometer _accel;
        uint _desiredReportInterval;
        double moveInterval = 2;

        public MainPage()
        {
            this.InitializeComponent();
            setupAccelerometer();
        }

        private void setupAccelerometer()
        {
            // try and get a handle to the accelerometer
            // if it doesn't exist, then tell the user.
            _accel = Accelerometer.GetDefault();
            // if it doesn't exist, then _accel is null
            if( _accel == null)
            {
                tblStatusX.Text = "There is no accelerometer on this device - cheapie!";
                
            }
            else
            {
                // there is an accelerometer
                // set the report interval
                // register for the readingChanged event
                tblStatusX.Text = "Setting up the accelerometer!";
                uint minReportInterval = _accel.MinimumReportInterval;
                // set at 100 ms 
                _desiredReportInterval = 
                    minReportInterval > 100 ? minReportInterval : 100;
                _accel.ReportInterval = _desiredReportInterval;
                _accel.ReadingChanged += _accel_ReadingChanged;
            }
        } // end setupAcelerometer

        private async void _accel_ReadingChanged(Accelerometer sender, 
                        AccelerometerReadingChangedEventArgs args)
        {
            // need to put some stuff on the screen
            // just to see the values for now.
            AccelerometerReading reading = args.Reading;
            // update the UI with some information from reading
            // this is on a background thread
            // need a delegate function, using await/async
            await Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal,
                () => 
                {
                    // lambda expression (anonymous function)
                    updateUIInformation(reading);
                }

                );

        }

        private void updateUIInformation(AccelerometerReading reading)
        {
            tblStatusX.Text = "X Value: " + 
                            reading.AccelerationX.ToString("0.0000");
            tblStatusY.Text = "Y Value: " +
                            reading.AccelerationY.ToString("0.0000");
            tblStatusZ.Text = "Z Value: " +
                            reading.AccelerationZ.ToString("0.0000");

            // to move left or right - decide using Xvalue - -ve, +ve
            // every 100ms, check the value of x, move the ellipse 
            // as needed.

            #region move the ball on X
            if (reading.AccelerationX <= 0)
            {
                // move left
                if ((double)elMoveThis.GetValue(Canvas.LeftProperty) > 0)
                {
                    // set the canvas.left by decreasing by speed
                    elMoveThis.SetValue(Canvas.LeftProperty,
                        (double)elMoveThis.GetValue(Canvas.LeftProperty)
                        - moveInterval);
                }
            }
            else
            {
                // figure the right boundary for the canvas
                if (((double)elMoveThis.GetValue(Canvas.LeftProperty)
                    + elMoveThis.Width) < (cvsMaze.ActualWidth)
                    )
                {
                    // move right
                    elMoveThis.SetValue(Canvas.LeftProperty,
                        (double)elMoveThis.GetValue(Canvas.LeftProperty)
                        + moveInterval);
                }
            }
            #endregion

            #region move the ball on Y
            if (reading.AccelerationY > 0) // requires testing to check
            {
                // move up
                if ((double)elMoveThis.GetValue(Canvas.TopProperty) > 0)
                {
                    // set the canvas.left by decreasing by speed
                    elMoveThis.SetValue(Canvas.TopProperty,
                        (double)elMoveThis.GetValue(Canvas.TopProperty)
                        - moveInterval);
                }
            }
            else
            {
                // figure the bottom boundary for the canvas
                if (((double)elMoveThis.GetValue(Canvas.TopProperty)
                    + elMoveThis.Height) < (cvsMaze.ActualHeight)
                    )
                {
                    // move down

                    elMoveThis.SetValue(Canvas.TopProperty,
                    (double)elMoveThis.GetValue(Canvas.TopProperty)
                    + moveInterval);
                }
            }
            #endregion
        }

    }
}
