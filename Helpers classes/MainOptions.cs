using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Drawing;

namespace SeaChart {
    /// <summary>
    /// Main options class, serialized into preferences.xml
    /// </summary>
    [Serializable]
    public class MainOptions {
        #region Dots
        private SerializableDictionary<Point, string> dots = new SerializableDictionary<Point, string>();

        /// <summary>
        /// Gets or sets the Dictionary containing the dots positions and tags.
        /// </summary>
        /// <value>The dots dictionnary.</value>
        public SerializableDictionary<Point, string> Dots {
            get { return dots; }
            set { dots = value; }
        }

        /// <summary>
        /// Gets the dots count.
        /// </summary>
        /// <value>The dots count.</value>
        public int DotsCount {
            get {
                return dots.Count;
            }
        }
        #endregion

        #region Serialization
        /// <summary>
        /// Creates a new options file.
        /// </summary>
        /// <param name="optionsFile">The options file path.</param>
        /// <returns>A new instance of this class</returns>
        public static MainOptions CreateNewOptionsFile (string optionsFile) {
            //Gets the directory, and create it if it doesn't exist yet
            string directoryName = Path.GetDirectoryName(DefaultOptionsFile);
            if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName)) {
                Directory.CreateDirectory(directoryName);
            }
            //Returns a new instance of the class, and serializes it into the specified XML file.
            MainOptions options = new MainOptions();
            options.Save(optionsFile);
            return options;
        }

        /// <summary>
        /// Loads the default options file.
        /// </summary>
        /// <returns>The deserialized instance of this class</returns>
        public static MainOptions Load () {
            return Load(DefaultOptionsFile);
        }

        /// <summary>
        /// Loads the specified options file.
        /// </summary>
        /// <param name="optionsFile">The options file.</param>
        /// <returns>The deserialized instance of this class</returns>
        public static MainOptions Load (string optionsFile) {
            MainOptions options = null; //the instance to return

            if (File.Exists(optionsFile)) {
                //Initializes a new instance of the class, to deserialize the options file
                XmlSerializer serializer = new XmlSerializer(typeof(MainOptions));

                //Read option files
                StreamReader streamReader = new StreamReader(optionsFile);
                XmlTextReader xmlReader = new XmlTextReader(streamReader);

                //Deserializes it
                if (serializer.CanDeserialize(xmlReader)) {
                    options = (MainOptions)serializer.Deserialize(xmlReader);
                }

                //Closes resources
                xmlReader.Close();
                streamReader.Close();
            }

            //If the file doesn't exist or contains error, create a new one.
            return options ?? CreateNewOptionsFile(optionsFile);
        }

        /// <summary>
        /// Saves this instance, serializing it into the default options file.
        /// </summary>
        public void Save () {
            Save(DefaultOptionsFile);
        }

        /// <summary>
        /// Saves this instance, serializing it into the specified file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void Save (string filename) {
            //The file stream to write
            StreamWriter writer = new StreamWriter(filename);

            //Serializes the class
            XmlSerializer serializer = new XmlSerializer(GetType());
            serializer.Serialize(writer, this);

            //Clears all buffers for the current writer and causes any buffered data to
            //be written to the underlying stream and closes the file stream.
            writer.Flush();
            writer.Close();
        }

        /// <summary>
        /// Gets the default options file.
        /// </summary>
        /// <value>The default options file.</value>
        public static string DefaultOptionsFile {
            get {
                return "Preferences.xml";
            }
        }
        #endregion
    }
}
