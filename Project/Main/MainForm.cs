using System.IO;
using System.Windows.Forms;
using MetroSuite;
using Obfusio.Engine;
using Obfusio.Engine.Helpers;

namespace Obfusio.Main
{
    public partial class MainForm : MetroForm
    {
        public MainForm()
        {
            InitializeComponent();

            guna2TextBox1.DragEnter += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    e.Effect = DragDropEffects.Copy;
                }
            };

            guna2TextBox1.DragDrop += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    guna2TextBox1.Text = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
                }
            };
        }

        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            SendKeys.Send("{TAB}");
            Focus();
        }

        private void guna2GradientButton1_Click(object sender, System.EventArgs e)
        {
            openFileDialog1.FileName = "";

            if (openFileDialog1.ShowDialog().Equals(DialogResult.OK))
            {
                if (!Utils.IsValidManagedAssembly(openFileDialog1.FileName))
                {
                    MessageBox.Show("The selected file is not a valid .NET assembly. Please, try with another one.", "Obfusio", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                guna2TextBox1.Text = openFileDialog1.FileName;
            }
        }

        private void guna2GradientButton2_Click(object sender, System.EventArgs e)
        {
            if (!File.Exists(guna2TextBox1.Text))
            {
                MessageBox.Show("The specified assembly file does not exist.", "Obfusio", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Path.GetExtension(guna2TextBox1.Text).ToLower().Equals(".exe") && !Path.GetExtension(guna2TextBox1.Text).ToLower().Equals(".dll"))
            {
                MessageBox.Show("The specified assembly file extension does not exist.", "Obfusio", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Utils.IsValidManagedAssembly(guna2TextBox1.Text))
            {
                MessageBox.Show("The selected file is not a valid .NET assembly. Please, try with another one.", "Obfusio", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Path.GetExtension(guna2TextBox1.Text).ToLower().Equals(".exe"))
            {
                saveFileDialog1.FileName = "";

                if (saveFileDialog1.ShowDialog().Equals(DialogResult.OK))
                {
                    Core.Protect(guna2TextBox1.Text, saveFileDialog1.FileName, guna2CheckBox1.Checked, guna2CheckBox2.Checked, guna2CheckBox3.Checked, guna2CheckBox4.Checked, guna2CheckBox5.Checked, guna2CheckBox6.Checked, guna2CheckBox7.Checked, guna2CheckBox8.Checked, guna2CheckBox9.Checked, guna2CheckBox10.Checked, guna2CheckBox11.Checked, guna2CheckBox12.Checked, guna2CheckBox13.Checked, guna2CheckBox14.Checked, guna2CheckBox15.Checked, guna2CheckBox16.Checked, guna2CheckBox17.Checked);
                }
            }
            else
            {
                saveFileDialog2.FileName = "";
                if (saveFileDialog2.ShowDialog().Equals(DialogResult.OK))
                {
                    Core.Protect(guna2TextBox1.Text, saveFileDialog2.FileName, guna2CheckBox1.Checked, guna2CheckBox2.Checked, guna2CheckBox3.Checked, guna2CheckBox4.Checked, guna2CheckBox5.Checked, guna2CheckBox6.Checked, guna2CheckBox7.Checked, guna2CheckBox8.Checked, guna2CheckBox9.Checked, guna2CheckBox10.Checked, guna2CheckBox11.Checked, guna2CheckBox12.Checked, guna2CheckBox13.Checked, guna2CheckBox14.Checked, guna2CheckBox15.Checked, guna2CheckBox16.Checked, guna2CheckBox17.Checked);
                }
            }

            MessageBox.Show("Succsfully protected your .NET assembly!", "Obfusio", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}