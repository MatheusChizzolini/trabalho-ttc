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

        public static byte[] PegarVizinhosCeguinho(byte[,] matrizBinaria, Point pixelAtual)
        {
            byte[] vizinhos = [
                matrizBinaria[pixelAtual.Y, pixelAtual.X + 1],
                matrizBinaria[pixelAtual.Y - 1, pixelAtual.X + 1],
                matrizBinaria[pixelAtual.Y - 1, pixelAtual.X],
                matrizBinaria[pixelAtual.Y - 1, pixelAtual.X - 1],
                matrizBinaria[pixelAtual.Y, pixelAtual.X - 1],
                matrizBinaria[pixelAtual.Y + 1, pixelAtual.X - 1],
                matrizBinaria[pixelAtual.Y + 1, pixelAtual.X],
                matrizBinaria[pixelAtual.Y + 1, pixelAtual.X + 1],
            ];

            return vizinhos;
        }

        public static Point GerarNovaCoordenada(Point pixelAtual, int indice)
        {
            switch (indice)
            {
                case 0: return new Point(pixelAtual.X + 1, pixelAtual.Y);
                case 1: return new Point(pixelAtual.X + 1, pixelAtual.Y - 1);
                case 2: return new Point(pixelAtual.X, pixelAtual.Y - 1);
                case 3: return new Point(pixelAtual.X - 1, pixelAtual.Y - 1);
                case 4: return new Point(pixelAtual.X - 1, pixelAtual.Y);
                case 5: return new Point(pixelAtual.X - 1, pixelAtual.Y + 1);
                case 6: return new Point(pixelAtual.X, pixelAtual.Y + 1);
                case 7: return new Point(pixelAtual.X + 1, pixelAtual.Y + 1);
                default: return new Point(pixelAtual.X, pixelAtual.Y);
            }
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
            for (int linha = 1;  linha < altura - 1; linha++)
            {
                for (int coluna = 1; coluna < largura - 2; coluna++)
                {
                    if (matrizBinaria[linha, coluna] == 0 && matrizBinaria[linha, coluna + 1] == 1)
                    {
                        List<Point> contorno = new List<Point>();
                        Point pixelAtual = new Point(coluna, linha);
                        contorno.Add(pixelAtual);
                        byte[] vizinhos = PegarVizinhosCeguinho(matrizBinaria, pixelAtual);
                        int indiceVizinhoAnterior = 4;
                        bool isInicio = false;

                        int passos = 0;
                        int maxPassos = altura * largura * 4;

                        while (!isInicio && passos < maxPassos)
                        {
                            passos++;
                            int indiceObjeto = -1;
                            bool flag = false;
                            for (int i = 0; i < vizinhos.Length; i++)
                            {
                                int indice = (indiceVizinhoAnterior + 1 + i) % 8;
                                if (vizinhos[indice] == 1 && !flag)
                                {
                                    indiceObjeto = indice;
                                    flag = true;
                                }
                            }

                            if (indiceObjeto != -1)
                            {
                                int indiceFundo = (indiceObjeto + 7) % 8;
                                Point proximoFundo = GerarNovaCoordenada(pixelAtual, indiceFundo);
                                contorno.Add(proximoFundo);
                                pixelAtual = proximoFundo;
                                indiceVizinhoAnterior = indiceFundo;
                                vizinhos = PegarVizinhosCeguinho(matrizBinaria, pixelAtual);
                                if (pixelAtual == contorno[0])
                                {
                                    isInicio = true;
                                }
                            }
                            else
                            {
                                isInicio = true;
                            }
                        }
                        
                        foreach (Point p in contorno)
                        {
                            if (p.Y >= 0 && p.Y < altura && p.X >= 0 && p.X < largura)
                            {
                                matrizContorno[p.Y, p.X] = 1;
                            }
                        }
                    }
                }
            }

            TransformarMatrizEmBitmap(matrizContorno, bitmapDestino);
        }
    }
}
