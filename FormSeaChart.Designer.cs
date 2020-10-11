using System.Drawing;
using System.Windows.Forms;
using System;
using SeaChart.Properties;

namespace SeaChart {
    public partial class FormSeaChart {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent () {
            SuspendLayout();
            ClientSize = new Size(640, 480);
            Name = "FormSeaChart";
            MouseClick += new MouseEventHandler(this.Form_MouseClick);
            Load += new EventHandler(this.Form_Load);
            FormClosing += new FormClosingEventHandler(this.Form_FormClosing);
            KeyDown += new KeyEventHandler(Form_KeyDown);
            ResumeLayout(false);
        }

        #endregion
    }
}
