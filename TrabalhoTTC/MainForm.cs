namespace TrabalhoTTC
{
    public partial class MainForm : Form
    {
        private Image image;
        private Bitmap bitmap;

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnAbrirImagem_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = "";
            openFileDialog.Filter = "Arquivos de Imagem (*.jpg;*.bmp;*.png)|*.jpg;*.bmp;*.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                image = Image.FromFile(openFileDialog.FileName);
                pictureBox1.Image = image;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            pictureBox2.Image = null;
        }

        private void btnAplicarAlgoritmo_Click(object sender, EventArgs e)
        {

        }
    }
}
