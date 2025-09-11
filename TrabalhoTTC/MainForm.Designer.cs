namespace TrabalhoTTC
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            btnAbrirImagem = new Button();
            btnLimpar = new Button();
            btnAplicarAlgoritmo = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(40, 105);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(612, 512);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.Location = new Point(700, 105);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(612, 512);
            pictureBox2.TabIndex = 1;
            pictureBox2.TabStop = false;
            // 
            // btnAbrirImagem
            // 
            btnAbrirImagem.Location = new Point(40, 655);
            btnAbrirImagem.Margin = new Padding(3, 3, 16, 3);
            btnAbrirImagem.Name = "btnAbrirImagem";
            btnAbrirImagem.Size = new Size(128, 32);
            btnAbrirImagem.TabIndex = 2;
            btnAbrirImagem.Text = "Abrir imagem";
            btnAbrirImagem.UseVisualStyleBackColor = true;
            btnAbrirImagem.Click += btnAbrirImagem_Click;
            // 
            // btnLimpar
            // 
            btnLimpar.Location = new Point(187, 655);
            btnLimpar.Margin = new Padding(3, 3, 16, 3);
            btnLimpar.Name = "btnLimpar";
            btnLimpar.Size = new Size(128, 32);
            btnLimpar.TabIndex = 3;
            btnLimpar.Text = "Limpar";
            btnLimpar.UseVisualStyleBackColor = true;
            btnLimpar.Click += btnLimpar_Click;
            // 
            // btnAplicarAlgoritmo
            // 
            btnAplicarAlgoritmo.Location = new Point(334, 655);
            btnAplicarAlgoritmo.Name = "btnAplicarAlgoritmo";
            btnAplicarAlgoritmo.Size = new Size(128, 32);
            btnAplicarAlgoritmo.TabIndex = 4;
            btnAplicarAlgoritmo.Text = "Aplicar algoritmo";
            btnAplicarAlgoritmo.UseVisualStyleBackColor = true;
            btnAplicarAlgoritmo.Click += btnAplicarAlgoritmo_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1350, 729);
            Controls.Add(btnAplicarAlgoritmo);
            Controls.Add(btnLimpar);
            Controls.Add(btnAbrirImagem);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox1);
            Name = "MainForm";
            Text = "Afinamento, Extração de Contornos e Retângulo Mínimo";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private Button btnAbrirImagem;
        private Button btnLimpar;
        private Button btnAplicarAlgoritmo;
        private OpenFileDialog openFileDialog;
    }
}
