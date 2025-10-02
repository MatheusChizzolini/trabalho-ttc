using System.Drawing.Imaging;

namespace TrabalhoTTC
{
    public partial class MainForm : Form
    {
        private Image imagemOrigem;
        private Bitmap bitmapOrigem;

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnAbrirImagem_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = "";
            openFileDialog.Filter = "Arquivos de Imagem (*.jpg;*.bmp;*.png)|*.jpg;*.bmp;*.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                imagemOrigem = Image.FromFile(openFileDialog.FileName);
                pictureBox1.Image = imagemOrigem;
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
            Bitmap bitmapDestino = new Bitmap(imagemOrigem);
            Bitmap bitmapDestino2 = new Bitmap(imagemOrigem);
            bitmapOrigem = (Bitmap) imagemOrigem;
            List<List<Point>> contornos = new List<List<Point>>();
            Algoritmos.AfinamentoZhangSuen(bitmapOrigem, bitmapDestino);
            contornos = Algoritmos.Ceguinho(bitmapDestino, bitmapDestino2);
            Algoritmos.RetanguloMinimo(contornos, bitmapDestino2);
            pictureBox2.Image = bitmapDestino2;
            //bitmapDestino2.Save("C:\\Users\\Matheus\\Faculdade\\6Termo\\TTC\\final.png", ImageFormat.Png);
        }
    }
}
