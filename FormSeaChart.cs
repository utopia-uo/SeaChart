using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SeaChart.Properties;
using Ultima;

namespace SeaChart {
    /// <summary>
    /// [Windows Form] The main application form
    /// </summary>
    public partial class FormSeaChart : Form {
        
        /// <summary>
        /// Indicates if we're moving a dot
        /// </summary>
        private bool targetingMoveDot = false;
        
        /// <summary>
        /// The dot targettd to move
        /// </summary>
        private PictureBox targetedDot;
        
        /// <summary>
        /// The chart information
        /// </summary>
        private Chart chart;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormSeaChart"/> class.
        /// </summary>
        public FormSeaChart () {
            InitializeComponent();

            //Chart selection
            chart = Chart.Britannia;

            //Adapt the UI to the chart
            Text = "SeaChart - " + chart.Name;
            BackgroundImage = chart.Image;
            ClientSize = chart.Image.Size;
        }

        #region Windows Forms events
        /// <summary>
        /// Handles the Load event of the Form control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Form_Load (object sender, EventArgs e) {
            try {
                //Reads saved dots and adds it
                foreach (Point point in Program.Options.Dots.Keys) {
                    AddDot(point, Program.Options.Dots[point]);
                }
            } catch (Exception exception) {
                MessageBox.Show(exception.Message, "Preferences reading error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        /// <summary>
        /// Handles the FormClosing event of the Form control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
        private void Form_FormClosing (object sender, FormClosingEventArgs e) {
            //Export our dots, so we can reload them at next program run
            Program.Options.Dots = GetDots();
        }

        /// <summary>
        /// Handles the MouseClick event of the Form control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void Form_MouseClick (object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                //New default dot
                AddDot(e.Location);
            } else if (e.Button == MouseButtons.Left) {
                //Closes a move operation, if needed.
                if (targetingMoveDot) {
                    //End of a dot move 
                    if (targetedDot != null) {
                        targetedDot.Location = e.Location;
                    }
                    targetingMoveDot = false;
                }
                //Print coords
                PrintCoords(e.Location);
            }
        }

        /// <summary>
        /// Handles the KeyDown event of the Form control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void Form_KeyDown (object sender, KeyEventArgs e) {
            if (e.KeyData == Keys.X) {
                //X deletes all the dots
                RemoveAllDots();
            } else if (e.KeyData == Keys.O) {
                //O adds a dot in current location in uo client
                AddDotAtCurrentPosition();
            }
        }

        /// <summary>
        /// Handles the MouseClick event of the pictureBoxDot control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void pictureBoxDot_MouseClick (object sender, MouseEventArgs e) {
            PictureBox box = (PictureBox)sender;
            if (e.Button == MouseButtons.Right) {
                //Right click, deletes the clicked dot
                RemoveControl(box);
            } else if (e.Button == MouseButtons.Left) {
                //Left click, starts a move operation
                //The dot will be moved at the position of the next form click location (cf. Form_MouseClick)
                targetedDot = box;
                targetingMoveDot = true;
            }
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the pictureBoxDot control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void pictureBoxDot_MouseDoubleClick (object sender, MouseEventArgs e) {
            PictureBox box = (PictureBox)sender;
            
            //We swap between blue (carré bleu) and red (carré bordeau) dots
            //Dots are identified by a tag "Carrebleu" or "CarreRouge" in the options file
            if (box.Tag.ToString() == "CarreBleu") {
                box.Image = Resources.CarreBordeau;
                box.Tag = "CarreBordeau";
            } else {
                box.Image = Resources.CarreBleu;
                box.Tag = "CarreBleu";
            }

            //pictureBoxDot_MouseClick is also handled but user doesn't want to move it
            targetingMoveDot = false; 
            
            //Print coords
            PrintCoords((new Point(box.Location.X + 5, box.Location.Y + 5)));
        }
        #endregion

        #region Dots related methods
        /// <summary>
        /// Adds a blue dot at specified location.
        /// </summary>
        /// <param name="centerLocation">The center location.</param>
        private void AddDot (Point centerLocation) {
            AddDot(new Point(centerLocation.X - 5, centerLocation.Y - 5), "CarreBleu");
        }

        /// <summary>
        /// Adds a dot at specified location and tag.
        /// </summary>
        /// <param name="location">The dot location.</param>
        /// <param name="tag">The dot tag.</param>
        private void AddDot (Point location, string tag) {
            //Creates a new PictureBox control, with a blue or red dot as image
            PictureBox box = new PictureBox();
            box.Image = (tag == "CarreBleu") ? Resources.CarreBleu : Resources.CarreBordeau;
            box.Tag = tag;

            //Picture box properties
            box.Size = box.Image.Size;
            box.Location = location;
            box.Visible = true;

            //We attach to the picturebox two event handlers, to handle mouse clicks
            box.MouseClick += new MouseEventHandler(pictureBoxDot_MouseClick);
            box.MouseDoubleClick += new MouseEventHandler(pictureBoxDot_MouseDoubleClick);
            Controls.Add(box);
        }

        /// <summary>
        /// Adds a dot at the UO client current position.
        /// </summary>
        private void AddDotAtCurrentPosition () {
            try {
                if (Client.Running) {
                    int x = 0, y = 0, z = 0, facet = 0;

                    //Calibrates to get correct coords
                    Client.Calibrate();

                    //Get location (if possible) and then check if the uo map id is our own map
                    if (Client.FindLocation(ref x, ref y, ref z, ref facet) && chart.IsCurrentFacet(facet)) {
                        AddDotAtCoords(x, y); //Adds a dot at those coords
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error getting your current position", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Gets a dictionary containing all the dots position and tags.
        /// </summary>
        /// <returns></returns>
        private SerializableDictionary<Point, string> GetDots () {
            SerializableDictionary<Point, string> dict = new SerializableDictionary<Point, string>();
            foreach (Control control in base.Controls) {
                if (control is PictureBox) {
                    dict.Add(control.Location, control.Tag.ToString());
                }
            }
            return dict;
        }

        /// <summary>
        /// Removes the specified control from the form.
        /// </summary>
        /// <param name="control">The control to remove.</param>
        private void RemoveControl (Control control) {
            //Makes the control not showed
            control.Visible = false;
            //Removes it from our form controls collection and disposes all associated resources 
            Controls.Remove(control);
            control.Dispose();
        } 

        /// <summary>
        /// Removes all dots.
        /// </summary>
        private void RemoveAllDots () {
            Controls.Clear();

            /*
             * If you add other controls than the PictureBox dots,
             * use instead this code not to delete them.
             * And if you add non deletable PictureBox, think to check tags:
             * if (Controls[i] is PictureBox and !String.IsNullOrEmpty(Controls[i].Tag.ToString())
            for (int i = Controls.Count - 1 ; i >= 0 ; i--) {
                if (Controls[i] is PictureBox) {
                    RemoveControl(Controls[i]);
                }
            }
            */
        }
        #endregion

        #region Coords methods
        /// <summary>
        /// Print the coordinates of the specified point
        /// </summary>
        /// <param name="point">A point in the Form client area</param>
        private void PrintCoords (Point point) {
            SetTitle(GetCoords(point));
        }

        /// <summary>
        /// Gets the coordinates of the specified point
        /// </summary>
        /// <param name="point">A point in the Form client area</param>
        /// <returns>A string containing the coordinates</returns>
        private string GetCoords (Point point) {
            string coords;
            
            //Latitude
            if (point.Y <= chart.YCenter) {
                //We're bottom the center, so in South, \x00b0 = °
                coords = string.Format("{0}\x00b0 S", Math.Round(((double)(point.Y - chart.YStart) / (chart.YCenter - chart.YStart)) * 180.0));
            } else {
                //We're up the center, so in North
                coords = string.Format("{0}\x00b0 N", Math.Round((((double)(chart.YEnd - point.Y) + 1.0) / (chart.YEnd - chart.YCenter)) * 180.0));
            }

            //Longitude
            if (point.X <= chart.XCenter) {
                //We're at the left of our center, so east (the UO maps paradox)
                coords += string.Format(" {0}\x00b0 E", Math.Round(((double)(point.X - chart.XStart) / (chart.XCenter - chart.XStart)) * 180.0));
            } else {
                //We're at the right of our center, so west (the UO maps paradox)
                coords += string.Format(" {0}\x00b0 W", Math.Round(((double)(chart.XEnd - point.X) / (chart.XEnd - chart.XCenter)) * 180.0));
            }

            return coords;
        }

        /// <summary>
        /// Adds a dot at specified uo coords.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void AddDotAtCoords (int x, int y) {
            int xLong = 0, yLat = 0;
            int xMins = 0, yMins = 0;

            //Are we in borns ?
            if (x < 0 || y < 0 || x >= chart.XWidth || y >= chart.YHeight) {
                return;
            }

            //Gets the longitude and latitude on [0, 360] scale
            double absLong = (double)((x - chart.XZero) * 360) / chart.XWidth;
            double absLat = (double)((y - chart.YZero) * 360) / chart.YHeight;

            //[-180, 180]
            if (absLong > 180.0)
                absLong = -180.0 + (absLong % 180.0);

            if (absLat > 180.0)
                absLat = -180.0 + (absLat % 180.0);

            //Are we east or west ? Are we south or north ?
            bool east = (absLong >= 0), south = (absLat >= 0);

            //Now let's take the absolute value
            if (absLong < 0.0)
                absLong = -absLong;

            if (absLat < 0.0)
                absLat = -absLat;

            //Keeps the integer part
            xLong = (int)absLong;
            yLat = (int)absLat;

            //Convert the decimal part in minutes
            xMins = (int)((absLong % 1.0) * 60);
            yMins = (int)((absLat % 1.0) * 60);

            //We've the coordinate, sets the title
            SetTitle(
                String.Format(
                    "{0}° {1}'{2}, {3}° {4}'{5}",
                    yLat, yMins, south ? "S" : "N",
                    xLong, xMins, east ? "E" : "W"
                )
            );

            //Converts the coordinates to our map position and adds a dot
            //It's the GetCoords(Point point) equation, reversed
            AddDot(
                new Point(
                    east ? chart.XStart + (chart.XCenter - chart.XStart) * xLong / 180 : chart.XEnd - xLong * (chart.XEnd - chart.XCenter) / 180,
                    south ? chart.YStart + (chart.YCenter - chart.YStart) * yLat / 180 : chart.YEnd + 1 - yLat * (chart.YEnd - chart.YCenter) / 180
                ),
                "CarreBordeau"
            );
        }
        #endregion

        #region Helpers methods
        /// <summary>
        /// Sets the specified title in the title bar, prepend by app and charts name.
        /// </summary>
        /// <param name="title">The new title</param>
        private void SetTitle (string title) {
            Text = String.Format("SeaChart - {0} - {1}", chart.Name, title);
        }
        #endregion
    }
}
