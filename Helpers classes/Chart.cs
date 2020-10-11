using System;
using System.Collections.Generic;
using System.Drawing;
using SeaChart.Properties;

namespace SeaChart {
    /// <summary>
    /// A chart class
    /// </summary>
    public class Chart {

        //Private members
        //cf. public properties for documentationa about them
        private string name;
        private Image image;
        private int xWidth, yHeight;
        private int xZero, yZero ;
        private int xStart, xCenter, xEnd;
        private int yStart, yCenter, yEnd;
        private List<int> map;

        #region Chart main properties
        /// <summary>
        /// Gets or sets the chart name.
        /// </summary>
        /// <value>The chart name.</value>
        public string Name { get { return name; } set { name = value; } }

        /// <summary>
        /// Gets or sets the chart image.
        /// </summary>
        /// <value>The chart image.</value>
        public Image Image { get { return image; } set { image = value; } }

        /// <summary>
        /// Gets or sets the width of the map in the game.
        /// </summary>
        /// <value>The width of the map.</value>
        public int XWidth { get { return xWidth; } set { xWidth = value; } }

        /// <summary>
        /// Gets or sets the height of the map in the game.
        /// </summary>
        /// <value>The height of the map.</value>
        public int YHeight { get { return yHeight; } set { yHeight = value; } }

        /// <summary>
        /// Gets or sets the 0° point X coordinate in the game.
        /// </summary>
        /// <value>The 0° point X coordinate.</value>
        public int XZero { get { return xZero; } set { xZero = value; } }

        /// <summary>
        /// Gets or sets the 0° point Y coordinate in the game
        /// </summary>
        /// <value>The 0° point Y  coordinate.</value>
        public int YZero { get { return yZero; } set { yZero = value; } }

        /// <summary>
        /// Gets or sets the UO map id.
        /// </summary>
        /// <value>The UO map id.</value>
        /// <example>{0, 1} for Trammel/Fel</example>
        /// <seealso cref="SeaChart.Chart.IsCurrentFacet"/>
        public int[] Map { get { return map.ToArray(); } set { map = new List<int>(value); } }
        #endregion

        #region X, Y start, center and end values
        /// <summary>
        /// Gets or sets the horizontal left start coordinate.
        /// </summary>
        /// <value>The X start coordinate.</value>
        public int XStart { get { return xStart; } set { xStart = value; } }
        /// <summary>
        /// Gets or sets the horizontal center coordinate.
        /// </summary>
        /// <value>The X center coordinate.</value>
        public int XCenter { get { return xCenter; } set { xCenter = value; } }
        /// <summary>
        /// Gets or sets the horizontal right end coordinate.
        /// </summary>
        /// <value>The X end coordinate.</value>
        public int XEnd { get { return xEnd; } set { xEnd = value; } }

        /// <summary>
        /// Gets or sets the vertical top start coordinate.
        /// </summary>
        /// <value>The Y start coordinate.</value>
        public int YStart { get { return yStart; } set { yStart = value; } }
        /// <summary>
        /// Gets or sets the vertical center coordinate.
        /// </summary>
        /// <value>The Y center coordinate.</value>
        public int YCenter { get { return yCenter; } set { yCenter = value; } }
        /// <summary>
        /// Gets or sets the vertical bottom end coordinate.
        /// </summary>
        /// <value>The Y end coordinate.</value>
        public int YEnd { get { return yEnd; } set { yEnd = value; } }
        #endregion

        #region Helper method
        /// <summary>
        /// Determines whether the UO current facet is one of the chart specified ones.
        /// </summary>
        /// <param name="facet">The current facet in UO.</param>
        /// <returns>
        /// 	<c>true</c> if the UO current facet is one of the chart specified ones.; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCurrentFacet (int facet) {
            return map.Contains(facet);
        }
        #endregion

        #region Predefined chart

        //If you want to adapt SeaChart for your map:
        // (1) add a picture of your map in resources
        // (2) copy the public static Chart Britannia property and edit values

        /// <summary>
        /// Gets the Britannia chart.
        /// </summary>
        /// <value>The Britannia chart.</value>
        public static Chart Britannia {
            get {
                Chart chart = new Chart();

                chart.Name = "Britannia";
                chart.image = Resources.BritanniaChart;

                chart.map = new List<int>();
                chart.map.Add(0); //Felucca
                chart.map.Add(1); //Trammel

                chart.xWidth = 5120; //after it's donj, lost worlds or ML areas
                chart.yHeight = 4096;
                chart.xZero = 1323;
                chart.yZero = 1624;

                chart.xStart = 3;
                chart.xCenter = 325;
                chart.xEnd = 642;

                chart.yStart = 4;
                chart.yCenter = 261;
                chart.yEnd = 513;
                
                return chart;
            }
        }
        #endregion
    }
}