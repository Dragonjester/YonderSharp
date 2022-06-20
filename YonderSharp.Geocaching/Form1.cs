namespace YonderSharp.Geocaching
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        EhemaligerLandkreisWaldeck ehamligerLandKreisWaldeck = new EhemaligerLandkreisWaldeck();

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox2.Text = ehamligerLandKreisWaldeck.Solve(textBox1.Text);
        }
    }
}