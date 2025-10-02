using System.Drawing.Imaging;

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
                byte* origem = (byte*)bitmapDataOrigem.Scan0.ToPointer();
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
                byte* destino = (byte*)bitmapDataDestino.Scan0.ToPointer();
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

        public static List<List<Point>> Ceguinho(Bitmap bitmapOrigem, Bitmap bitmapDestino)
        {
            int altura = bitmapOrigem.Height;
            int largura = bitmapOrigem.Width;
            int tamanhoPixel = 3;
            byte[,] matrizContorno = new byte[altura, largura];
            List<List<Point>> contornos = new List<List<Point>>();

            BitmapData bitmapDataOrigem = bitmapOrigem.LockBits(
                new Rectangle(0, 0, largura, altura),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb
            );

            int stride = bitmapDataOrigem.Stride;

            unsafe
            {
                byte*[] OitoVizinhos(byte* atual)
                {
                    byte*[] vizinhos = new byte*[8];

                    vizinhos[0] = atual + tamanhoPixel;
                    vizinhos[1] = atual - stride + tamanhoPixel;
                    vizinhos[2] = atual - stride;
                    vizinhos[3] = atual - stride - tamanhoPixel;
                    vizinhos[4] = atual - tamanhoPixel;
                    vizinhos[5] = atual + stride - tamanhoPixel;
                    vizinhos[6] = atual + stride;
                    vizinhos[7] = atual + stride + tamanhoPixel;

                    return vizinhos;
                }

                byte*[] QuatroVizinhos(byte* atual)
                {
                    byte*[] vizinhos = new byte*[4];

                    vizinhos[0] = atual + tamanhoPixel;
                    vizinhos[1] = atual - stride;
                    vizinhos[2] = atual - tamanhoPixel;
                    vizinhos[3] = atual + stride;

                    return vizinhos;
                }

                byte* origem = (byte*) bitmapDataOrigem.Scan0.ToPointer();
                // Binariza a imagem
                for (int y = 0; y < altura; y++)
                {
                    for (int x = 0; x < largura; x++)
                    {
                        byte* aux = origem + y * stride + x * tamanhoPixel;
                        if ((aux[0] + aux[1] + aux[2]) / 3 > 127)
                        {
                            aux[0] = 255;
                            aux[1] = 255;
                            aux[2] = 255;
                        }
                        else
                        {
                            aux[0] = 0;
                            aux[1] = 0;
                            aux[2] = 0;
                        }
                    }
                }

                for (int y = 1; y < altura - 1; y++)
                {
                    for (int x = 1; x < largura - 1; x++)
                    {
                        byte* atual = origem + y * stride + x * tamanhoPixel;
                        if (*atual == 255 && *(atual + tamanhoPixel) == 0 && matrizContorno[y, x] == 0)
                        {
                            List<Point> contornoAtual = new List<Point>();
                            Stack<IntPtr> pilha = new Stack<IntPtr>();
                            matrizContorno[y, x] = 1;
                            contornoAtual.Add(new Point(x, y));
                            byte*[] oitoVizinhos = OitoVizinhos(atual);
                            for (int i = 0; i < 8; i++)
                            {
                                if (*oitoVizinhos[i] == 255)
                                {
                                    byte*[] quatroVizinhos = QuatroVizinhos(oitoVizinhos[i]);
                                    if (*quatroVizinhos[0] == 0 || *quatroVizinhos[1] == 0 || *quatroVizinhos[2] == 0 || *quatroVizinhos[3] == 0)
                                    {
                                        pilha.Push((IntPtr) oitoVizinhos[i]);
                                    }
                                }
                            }

                            while (pilha.Count > 0)
                            {
                                byte* p = (byte*) pilha.Pop();
                                oitoVizinhos = OitoVizinhos(p);
                                long offset = p - origem;
                                int yp = (int) (offset / stride);
                                int xp = (int) ((offset % stride) / tamanhoPixel);
                                for (int i = 0; i < 8; i++)
                                {
                                    if (*oitoVizinhos[i] == 255 && matrizContorno[yp, xp] == 0)
                                    {
                                        byte*[] quatroVizinhos = QuatroVizinhos(oitoVizinhos[i]);
                                        if (*quatroVizinhos[0] == 0 || *quatroVizinhos[1] == 0 || *quatroVizinhos[2] == 0 || *quatroVizinhos[3] == 0)
                                        {
                                            pilha.Push((IntPtr) oitoVizinhos[i]);
                                        }
                                    }
                                }

                                matrizContorno[yp, xp] = 1;
                                contornoAtual.Add(new Point(xp, yp));
                            }

                            contornos.Add(contornoAtual);
                        }
                    }
                }

                TransformarMatrizEmBitmap(matrizContorno, bitmapDestino);
            }

            bitmapOrigem.UnlockBits(bitmapDataOrigem);
            return contornos;
        }

        public static void RetanguloMinimo(List<List<Point>> contornos, Bitmap bitmapDestino)
        {
            int altura = bitmapDestino.Height;
            int largura = bitmapDestino.Width;
            int tamanhoPixel = 3;

            BitmapData bitmapDataDestino = bitmapDestino.LockBits(
                new Rectangle(0, 0, largura, altura),
                ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb
            );

            int stride = bitmapDataDestino.Stride;

            unsafe
            {
                byte *destino = (byte *) bitmapDataDestino.Scan0.ToPointer();
                foreach (List<Point> contorno in contornos)
                {
                    if (contorno.Count != 0)
                    {
                        int minX = contorno[0].X;
                        int maxX = contorno[0].X;
                        int minY = contorno[0].Y;
                        int maxY = contorno[0].Y;

                        foreach (Point p in contorno)
                        {
                            if (p.X < minX)
                            {
                                minX = p.X;
                            }

                            if (p.X > maxX)
                            {
                                maxX = p.X;
                            }

                            if (p.Y < minY)
                            {
                                minY = p.Y;
                            }

                            if (p.Y > maxY)
                            {
                                maxY = p.Y;
                            }
                        }

                        byte *aux;
                        for (int x = minX; x <= maxX; x++)
                        {
                            if (x >= 0 && x < largura && minY >= 0 && minY < altura)
                            {
                                aux = destino + (minY * stride) + (x * tamanhoPixel);
                                *(aux)++ = 0;
                                *(aux)++ = 0;
                                *(aux)++ = 255;
                            }
                        }

                        for (int x = minX; x <= maxX; x++)
                        {
                            if (x >= 0 && x < largura && maxY >= 0 && maxY < altura)
                            {
                                aux = destino + (maxY * stride) + (x * tamanhoPixel);
                                *(aux)++ = 0;
                                *(aux)++ = 0;
                                *(aux)++ = 255;
                            }
                        }

                        for (int y = minY; y <= maxY; y++)
                        {
                            if (minX >= 0 && minX < largura && y >= 0 && y < altura)
                            {
                                aux = destino + (y * stride) + (minX * tamanhoPixel);
                                *(aux)++ = 0;
                                *(aux)++ = 0;
                                *(aux)++ = 255;
                            }
                        }

                        for (int y = minY; y <= maxY; y++)
                        {
                            if (maxX >= 0 && maxX < largura && y >= 0 && y < altura)
                            {
                                aux = destino + (y * stride) + (maxX * tamanhoPixel);
                                *(aux)++ = 0;
                                *(aux)++ = 0;
                                *(aux)++ = 255;
                            }
                        }
                    }
                }
            }

            bitmapDestino.UnlockBits(bitmapDataDestino);
        }
    }
}
