using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrabalhoTTC
{
    class Algoritmos
    {
        // Esta função percorre a imagem e povoa a matriz binária, que representa os pretos e brancos da imagem orignal
        public static byte[,] GerarMatrizBinaria(Bitmap bitmapOrigem)
        {
            int altura = bitmapOrigem.Height;
            int largura = bitmapOrigem.Width;
            int tamanhoPixel = 3;
            byte[,] matrizBinaria = new byte[altura, largura];

            BitmapData bitmapDataOrigem = bitmapOrigem.LockBits(
                new Rectangle(0, 0, largura, altura),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb
            );

            int padding = bitmapDataOrigem.Stride - (largura * tamanhoPixel);

            unsafe
            {
                byte *origem = (byte *)bitmapDataOrigem.Scan0.ToPointer();
                for (int linha = 0; linha < altura; linha++)
                {
                    for (int coluna = 0; coluna < largura; coluna++)
                    {
                        if (*origem < 127 && *(origem + 1) < 127 && *(origem + 2) < 127)
                        {
                            matrizBinaria[linha, coluna] = 1;
                        }
                        else
                        {
                            matrizBinaria[linha, coluna] = 0;
                        }

                        origem += tamanhoPixel;
                    }

                    origem += padding;
                }
            }

            bitmapOrigem.UnlockBits(bitmapDataOrigem);
            return matrizBinaria;
        }

        // Esta função transforma de volta a matriz binária em bitmap
        public static void TransformarMatrizEmBitmap(byte[,] matrizBinaria, Bitmap bitmapDestino)
        {
            int altura = matrizBinaria.GetLength(0);
            int largura = matrizBinaria.GetLength(1);
            int tamanhoPixel = 3;

            BitmapData bitmapDataDestino = bitmapDestino.LockBits(
                new Rectangle(0, 0, largura, altura),
                ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb
            );

            int padding = bitmapDataDestino.Stride - (largura * tamanhoPixel);

            unsafe
            {
                byte *destino = (byte *)bitmapDataDestino.Scan0.ToPointer();
                for (int linha = 0; linha < altura; linha++)
                {
                    for (int coluna = 0; coluna < largura; coluna++)
                    {
                        if (matrizBinaria[linha, coluna] == 1)
                        {
                            *(destino)++ = 0;
                            *(destino)++ = 0;
                            *(destino)++ = 0;
                        }
                        else
                        {
                            *(destino)++ = 255;
                            *(destino)++ = 255;
                            *(destino)++ = 255;
                        }
                    }

                    destino += padding;
                }
            }

            bitmapDestino.UnlockBits(bitmapDataDestino);
        }

        public static void AfinamentoZhangSuen(Bitmap bitmapOrigem, Bitmap bitmapDestino)
        {
            int altura = bitmapOrigem.Height;
            int largura = bitmapOrigem.Width;
            byte[,] matrizBinaria = GerarMatrizBinaria(bitmapOrigem);

            BitmapData bitmapDataOrigem = bitmapOrigem.LockBits(
                new Rectangle(0, 0, largura, altura),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb    
            );

            unsafe
            {
                bool afinamento = true;
                while (afinamento)
                {
                    afinamento = false;
                    List<Point> marcados = [];
                    // Primeira sub-iteração
                    for (int linha = 1; linha < altura - 1; linha++)
                    {
                        for (int coluna = 1; coluna < largura - 1; coluna++)
                        {
                            if (matrizBinaria[linha, coluna] == 1)
                            {
                                byte[] vizinhos = new byte[8];
                                vizinhos[0] = matrizBinaria[linha - 1, coluna];
                                vizinhos[1] = matrizBinaria[linha - 1, coluna + 1];
                                vizinhos[2] = matrizBinaria[linha, coluna + 1];
                                vizinhos[3] = matrizBinaria[linha + 1, coluna + 1];
                                vizinhos[4] = matrizBinaria[linha + 1, coluna];
                                vizinhos[5] = matrizBinaria[linha + 1, coluna - 1];
                                vizinhos[6] = matrizBinaria[linha, coluna - 1];
                                vizinhos[7] = matrizBinaria[linha - 1, coluna - 1];

                                // Primeira regra
                                int transicao = 0;
                                for (int i = 0; i < vizinhos.Length - 1; i++)
                                {
                                    if (vizinhos[i] == 0 && vizinhos[i + 1] == 1)
                                    {
                                        transicao++;
                                    }
                                }

                                if (vizinhos[7] == 0 && vizinhos[0] == 1)
                                {
                                    transicao++;
                                }

                                if (transicao == 1)
                                {
                                    // Segunda regra
                                    int quantidadePretos = 0;
                                    for (int i = 0; i < vizinhos.Length; i++)
                                    {
                                        quantidadePretos += vizinhos[i];
                                    }

                                    if (quantidadePretos >= 2 && quantidadePretos <= 6)
                                    {
                                        // Terceira regra
                                        if (vizinhos[0] == 0 || vizinhos[2] == 0 || vizinhos[6] == 0)
                                        {
                                            // Quarta regra
                                            if (vizinhos[0] == 0 || vizinhos[4] == 0 || vizinhos[6] == 0)
                                            {
                                                marcados.Add(new Point(coluna, linha));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Remoção dos pixels após a primeira sub-iteração
                    foreach (Point p in marcados)
                    {
                        matrizBinaria[p.Y, p.X] = 0;
                    }

                    if (marcados.Count > 0)
                    {
                        afinamento = true;
                    }

                    marcados.Clear();
                    // Segunda sub-iteração
                    for (int linha = 1; linha < altura - 1; linha++)
                    {
                        for (int coluna = 1; coluna < largura - 1; coluna++)
                        {
                            if (matrizBinaria[linha, coluna] == 1)
                            {
                                byte[] vizinhos = new byte[8];
                                vizinhos[0] = matrizBinaria[linha - 1, coluna];
                                vizinhos[1] = matrizBinaria[linha - 1, coluna + 1];
                                vizinhos[2] = matrizBinaria[linha, coluna + 1];
                                vizinhos[3] = matrizBinaria[linha + 1, coluna + 1];
                                vizinhos[4] = matrizBinaria[linha + 1, coluna];
                                vizinhos[5] = matrizBinaria[linha + 1, coluna - 1];
                                vizinhos[6] = matrizBinaria[linha, coluna - 1];
                                vizinhos[7] = matrizBinaria[linha - 1, coluna - 1];

                                int transicao = 0;
                                for (int i = 0; i < vizinhos.Length - 1; i++)
                                {
                                    if (vizinhos[i] == 0 && vizinhos[i + 1] == 1)
                                    {
                                        transicao++;
                                    }
                                }

                                if (vizinhos[7] == 0 && vizinhos[0] == 1)
                                {
                                    transicao++;
                                }

                                if (transicao == 1)
                                {
                                    int quantidadePretos = 0;
                                    for (int i = 0; i < vizinhos.Length; i++)
                                    {
                                        quantidadePretos += vizinhos[i];
                                    }

                                    if (quantidadePretos >= 2 && quantidadePretos <= 6)
                                    {
                                        if (vizinhos[0] == 0 || vizinhos[2] == 0 || vizinhos[4] == 0)
                                        {
                                            if (vizinhos[2] == 0 || vizinhos[4] == 0 || vizinhos[6] == 0)
                                            {
                                                marcados.Add(new Point(coluna, linha));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Remoção dos pixels após a segunda sub-iteração
                    foreach (Point p in marcados)
                    {
                        matrizBinaria[p.Y, p.X] = 0;
                    }

                    if (marcados.Count > 0)
                    {
                        afinamento = true;
                    }
                }

                TransformarMatrizEmBitmap(matrizBinaria, bitmapDestino);
            }

            bitmapOrigem.UnlockBits(bitmapDataOrigem);
        }

        public static void Ceguinho(Bitmap bitmapOrigem, Bitmap bitmapDestino)
        {
            int altura = bitmapOrigem.Height;
            int largura = bitmapOrigem.Width;
            byte[,] matrizBinaria = GerarMatrizBinaria(bitmapOrigem);
            byte[,] matrizContorno = new byte[altura, largura];

            for (int linha = 1; linha < altura - 1; linha++)
            {
                for (int coluna = 1; coluna < largura - 1; coluna++)
                {
                    if (matrizBinaria[linha, coluna] == 0 && matrizBinaria[linha, coluna + 1] == 1 && matrizContorno[linha, coluna] == 0)
                    {
                        matrizContorno[linha, coluna] = 1;
                        Point inicio = new Point(coluna, linha);
                        Point atual = inicio;
                        int direcao = 4;
                        bool voltou = false;
                        while (!voltou)
                        {
                            if (atual.Y < 0 || atual.Y >= altura || atual.X < 0 || atual.X >= largura)
                            {
                                voltou = true;
                            }
                            else
                            {
                                Point[] vizinhos = new Point[8];
                                vizinhos[0] = new Point(atual.X + 1, atual.Y);
                                vizinhos[1] = new Point(atual.X + 1, atual.Y - 1);
                                vizinhos[2] = new Point(atual.X, atual.Y - 1);
                                vizinhos[3] = new Point(atual.X - 1, atual.Y - 1);
                                vizinhos[4] = new Point(atual.X - 1, atual.Y);
                                vizinhos[5] = new Point(atual.X - 1, atual.Y + 1);
                                vizinhos[6] = new Point(atual.X, atual.Y + 1);
                                vizinhos[7] = new Point(atual.X + 1, atual.Y + 1);

                                bool encontrou = false;
                                for (int i = 0; i < 8 && !encontrou; i++)
                                {
                                    int indice = (direcao + 1 + i) % 8;
                                    Point p = vizinhos[indice];
                                    if (p.Y >= 0 && p.Y < altura && p.X >= 0 && p.X < largura)
                                    {
                                        if (matrizBinaria[p.Y, p.X] == 1)
                                        {
                                            matrizContorno[atual.Y, atual.X] = 1;
                                            atual = vizinhos[(indice + 7) % 8];
                                            direcao = (indice + 6) % 8;
                                            encontrou = true;
                                        }
                                    }
                                }

                                if (atual == inicio)
                                {
                                    voltou = true;
                                }
                                else if (!encontrou)
                                {
                                    voltou = true;
                                }
                            }
                        }
                    }
                }
            }

            TransformarMatrizEmBitmap(matrizContorno, bitmapDestino);
        }
    }
}
