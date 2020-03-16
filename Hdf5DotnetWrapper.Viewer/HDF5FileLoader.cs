using System;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Hdf5DotnetWrapper.Viewer
{
    public partial class HDF5FileLoader : UserControl
    {
        public HDF5FileLoader()
        {
            InitializeComponent();
        }

        private void btnHDF5Browse_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "Hdf5 files (*.h5)|*.h5",
            };
            if (of.ShowDialog() == DialogResult.OK)
            {
                txtbHDF5.Text = of.FileName;
            }
        }

        private void btnHDF5ReadFile_Click(object sender, EventArgs e)
        {
           Hdf5File.ReadFileStructure(txtbHDF5.Text);

        }
    }
}
