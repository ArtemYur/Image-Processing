using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EvilDICOM.Core;
using EvilDICOM.Core.Element;

namespace ImageProcessing2
{
    public partial class MainForm : Form
    {
        private int _maxWidth;
        private int _maxHeight;

        public MainForm()
        {
            InitializeComponent();

            _maxWidth = 1300;
            _maxHeight = 700;
        }
    }
}
