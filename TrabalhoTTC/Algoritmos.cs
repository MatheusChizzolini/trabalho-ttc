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
        public static void AfinamentoZhangSuen(Bitmap bitmapOrigem, Bitmap bitmapDestino)
        {
            int altura = bitmapOrigem.Height;
            int largura = bitmapOrigem.Width;
            int tamanhoPixel = 3;
            byte[,] matrizBinaria = new byte[altura, largura];

            BitmapData bitmapDataOrigem = bitmapOrigem.LockBits(
                new Rectangle(0, 0, largura, altura), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb    
            );

            BitmapData bitmapDataDestino = bitmapDestino.LockBits(
                new Rectangle(0, 0, largura, altura), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb    
            );

            int padding = bitmapDataOrigem.Stride - (largura * tamanhoPixel);

            unsafe
            {
                // Esta parte percorre a imagem e povoa a matriz binária, que representa os pretos e brancos da imagem orignal
                byte* origem = (byte *) bitmapDataOrigem.Scan0.ToPointer();
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

                bool afinamento = true;
                while (afinamento)
                {
                    afinamento = false;
                    List<Point> marcados = new List<Point>();
                    // Primeira sub-iteração
                    for (int linha = 1; linha < altura - 1; linha++)
                    {
                        for (int coluna = 1; coluna < largura - 1; coluna++)
                        {
                            if (matrizBinaria[linha, coluna] == 1)
                            {
                                byte[] vizinhos =
                                [
                                    matrizBinaria[linha - 1, coluna],
                                    matrizBinaria[linha - 1, coluna + 1],
                                    matrizBinaria[linha, coluna + 1],
                                    matrizBinaria[linha + 1, coluna + 1],
                                    matrizBinaria[linha + 1, coluna],
                                    matrizBinaria[linha + 1, coluna - 1],
                                    matrizBinaria[linha, coluna - 1],
                                    matrizBinaria[linha - 1, coluna - 1],
                                ];

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
                                byte[] vizinhos =
                                [
                                    matrizBinaria[linha - 1, coluna],
                                    matrizBinaria[linha - 1, coluna + 1],
                                    matrizBinaria[linha, coluna + 1],
                                    matrizBinaria[linha + 1, coluna + 1],
                                    matrizBinaria[linha + 1, coluna],
                                    matrizBinaria[linha + 1, coluna - 1],
                                    matrizBinaria[linha, coluna - 1],
                                    matrizBinaria[linha - 1, coluna - 1],
                                ];

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

                // Transformar de volta a matriz em bitmap
                byte* destino = (byte *) bitmapDataDestino.Scan0.ToPointer();
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

            bitmapOrigem.UnlockBits(bitmapDataOrigem);
            bitmapDestino.UnlockBits(bitmapDataDestino);
        }
    }
}
