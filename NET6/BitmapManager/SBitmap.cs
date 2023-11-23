using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace BitmapManager
{
    public static class SBitmap
    {
        public static Bitmap CopyToSquareCanvas(this Bitmap sourceBitmap, int canvasWidthLenght)
        {
            // 1. 이미지의 비율을 유지하면서 캔버스에 이미지의 가장 긴축이 들어가도록 가로세로 길이 조정
            // 가로가 긴 이미지는 캔버스의 가로에 딱 맞도록 비율을 조정하고, 세로는 조정한 비율만큼 똑같이 조정한다.
            // 세로가 긴 이미지는 캔버스의 세로에 딱 맞도록 비율을 조정하고, 가로는 조정한 비율만큼 똑같이 조정한다.
            // 2. 조정한 비율로 비어있는 Bitmap 객체를 생성한다.
            float ratio = 1.0f;
            int maxSide = sourceBitmap.Width > sourceBitmap.Height ?
                          sourceBitmap.Width : sourceBitmap.Height;

            ratio = (float)maxSide / (float)canvasWidthLenght;

            Bitmap bitmapResult = (sourceBitmap.Width > sourceBitmap.Height ?
                                    new Bitmap(canvasWidthLenght, (int)(sourceBitmap.Height / ratio))
                                    : new Bitmap((int)(sourceBitmap.Width / ratio), canvasWidthLenght));

            // 3. 비어있는 Bitmap 객체에 소스이미지를 복사한다.
            using (Graphics graphicsResult = Graphics.FromImage(bitmapResult))
            {
                graphicsResult.CompositingQuality = CompositingQuality.HighQuality;
                graphicsResult.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsResult.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphicsResult.DrawImage(sourceBitmap,
                                        new Rectangle(0, 0,
                                            bitmapResult.Width, bitmapResult.Height),
                                        new Rectangle(0, 0,
                                            sourceBitmap.Width, sourceBitmap.Height),
                                            GraphicsUnit.Pixel);
                graphicsResult.Flush();
            }

            // 4. 비율조정이 완료된 Bitmap 이미지를 반환한다.
            return bitmapResult;
        }

        private static Bitmap ConvolutionFilter(Bitmap sourceBitmap, double[,] filterMatrix, double factor = 1, int bias = 0, bool grayscale = false)
        {
            // 1. 비트맵 데이터를 바이트 배열로 변환
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            // * BitmapData의 width값은 width + stride 이다.
            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            sourceBitmap.UnlockBits(sourceData);

            // 2. 그레이 스케일 이미지로 변환
            if (grayscale == true)
            {
                float rgb = 0;

                for (int k = 0; k < pixelBuffer.Length; k += 4)
                {
                    rgb = pixelBuffer[k] * 0.11f;       // B
                    rgb += pixelBuffer[k + 1] * 0.59f;  // G
                    rgb += pixelBuffer[k + 2] * 0.3f;   // R

                    pixelBuffer[k] = (byte)rgb;
                    pixelBuffer[k + 1] = pixelBuffer[k];
                    pixelBuffer[k + 2] = pixelBuffer[k];
                    pixelBuffer[k + 3] = 255;
                }
            }

            double blue = 0.0;
            double green = 0.0;
            double red = 0.0;

            int filterWidth = filterMatrix.GetLength(1);
            int filterHeight = filterMatrix.GetLength(0);

            int filterOffset = (filterWidth - 1) / 2;
            int calcOffset = 0;

            int byteOffset = 0;

            // 3. 컨볼루션 필터 적용
            // source byte[]에 접근 ( 3x3필터는 1칸, 5x5필터는 2칸 양옆으로 뛰어서 동작한다 )
            for (int offsetY = filterOffset; offsetY < sourceBitmap.Height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX < sourceBitmap.Width - filterOffset; offsetX++)
                {
                    blue = 0;
                    green = 0;
                    red = 0;

                    // 다음 픽셀 시작점 계산
                    byteOffset = offsetY * sourceData.Stride + offsetX * 4;

                    // 4. 대상 픽셀을 중심으로 상하좌우 영역 계산
                    for (int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                        {
                            calcOffset = byteOffset + (filterX * 4) + (filterY * sourceData.Stride);

                            // 필터값 적용해서 계산후 RGB 값에 각각 더하기
                            blue += (double)(pixelBuffer[calcOffset]) * filterMatrix[filterY + filterOffset, filterX + filterOffset];
                            green += (double)(pixelBuffer[calcOffset + 1]) * filterMatrix[filterY + filterOffset, filterX + filterOffset];
                            red += (double)(pixelBuffer[calcOffset + 2]) * filterMatrix[filterY + filterOffset, filterX + filterOffset];
                        }
                    }

                    // 5. RGB 값 각각에 factor와 bias 적용
                    blue = factor * blue + bias;
                    green = factor * green + bias;
                    red = factor * red + bias;

                    // 6. 바이트 범위에 벗어나는 값 다듬기
                    if (blue > 255) { blue = 255; }
                    else if (blue < 0) { blue = 0; }

                    if (green > 255) { green = 255; }
                    else if (green < 0) { green = 0; }

                    if (red > 255) { red = 255; }
                    else if (red < 0) { red = 0; }

                    // 7. 계산된 필터 적용 결과
                    // R, G, B 채널에 저장하며, 알파 채널은 255로 설정
                    resultBuffer[byteOffset] = (byte)(blue);
                    resultBuffer[byteOffset + 1] = (byte)(green);
                    resultBuffer[byteOffset + 2] = (byte)(red);
                    resultBuffer[byteOffset + 3] = 255;
                }
            }

            // 8. 수정된 이미지 값으로 Bitmap 새로 생성
            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);
            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            // 9. 반환
            return resultBitmap;
        }

        public static Bitmap ConvolutionFilter(this Bitmap sourceBitmap, double[,] xFilterMatrix, double[,] yFilterMatrix, double factor = 1, int bias = 0, bool grayscale = false)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            sourceBitmap.UnlockBits(sourceData);

            if (grayscale == true)
            {
                float rgb = 0;

                for (int k = 0; k < pixelBuffer.Length; k += 4)
                {
                    rgb = pixelBuffer[k] * 0.11f;
                    rgb += pixelBuffer[k + 1] * 0.59f;
                    rgb += pixelBuffer[k + 2] * 0.3f;

                    pixelBuffer[k] = (byte)rgb;
                    pixelBuffer[k + 1] = pixelBuffer[k];
                    pixelBuffer[k + 2] = pixelBuffer[k];
                    pixelBuffer[k + 3] = 255;
                }
            }

            double blueX = 0.0;
            double greenX = 0.0;
            double redX = 0.0;

            double blueY = 0.0;
            double greenY = 0.0;
            double redY = 0.0;

            double blueTotal = 0.0;
            double greenTotal = 0.0;
            double redTotal = 0.0;

            int filterOffset = 1;
            int calcOffset = 0;

            int byteOffset = 0;

            for (int offsetY = filterOffset; offsetY < sourceBitmap.Height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX < sourceBitmap.Width - filterOffset; offsetX++)
                {
                    blueX = greenX = redX = 0;
                    blueY = greenY = redY = 0;
                    blueTotal = greenTotal = redTotal = 0.0;

                    byteOffset = offsetY * sourceData.Stride + offsetX * 4;

                    for (int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                        {
                            calcOffset = byteOffset + (filterX * 4) + (filterY * sourceData.Stride);

                            blueX += (double)(pixelBuffer[calcOffset]) * xFilterMatrix[filterY + filterOffset, filterX + filterOffset];
                            greenX += (double)(pixelBuffer[calcOffset + 1]) * xFilterMatrix[filterY + filterOffset, filterX + filterOffset];
                            redX += (double)(pixelBuffer[calcOffset + 2]) * xFilterMatrix[filterY + filterOffset, filterX + filterOffset];

                            blueY += (double)(pixelBuffer[calcOffset]) * yFilterMatrix[filterY + filterOffset, filterX + filterOffset];
                            greenY += (double)(pixelBuffer[calcOffset + 1]) * yFilterMatrix[filterY + filterOffset, filterX + filterOffset];
                            redY += (double)(pixelBuffer[calcOffset + 2]) * yFilterMatrix[filterY + filterOffset, filterX + filterOffset];
                        }
                    }

                    blueTotal = Math.Sqrt((blueX * blueX) + (blueY * blueY));
                    greenTotal = Math.Sqrt((greenX * greenX) + (greenY * greenY));
                    redTotal = Math.Sqrt((redX * redX) + (redY * redY));

                    if (blueTotal > 255) { blueTotal = 255; }
                    else if (blueTotal < 0) { blueTotal = 0; }

                    if (greenTotal > 255) { greenTotal = 255; }
                    else if (greenTotal < 0) { greenTotal = 0; }

                    if (redTotal > 255) { redTotal = 255; }
                    else if (redTotal < 0) { redTotal = 0; }

                    resultBuffer[byteOffset] = (byte)(blueTotal);
                    resultBuffer[byteOffset + 1] = (byte)(greenTotal);
                    resultBuffer[byteOffset + 2] = (byte)(redTotal);
                    resultBuffer[byteOffset + 3] = 255;
                }
            }

            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);
            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }
               
        public static Bitmap Laplacian3x3Filter(this Bitmap sourceBitmap, bool grayscale = true)
        {
            Bitmap resultBitmap = SBitmap.ConvolutionFilter(sourceBitmap, SMatrix.Laplacian3x3, 1.0, 0, grayscale);
            return resultBitmap;
        }
               
        public static Bitmap Laplacian5x5Filter(this Bitmap sourceBitmap, bool grayscale = true)
        {
            Bitmap resultBitmap = SBitmap.ConvolutionFilter(sourceBitmap, SMatrix.Laplacian5x5, 1.0, 0, grayscale);
            return resultBitmap;
        }
               
        public static Bitmap LaplacianOfGaussianFilter(this Bitmap sourceBitmap)
        {
            Bitmap resultBitmap = SBitmap.ConvolutionFilter(sourceBitmap, SMatrix.LaplacianOfGaussian, 1.0, 0, true);
            return resultBitmap;
        }
               
        public static Bitmap Laplacian3x3OfGaussian3x3Filter(this Bitmap sourceBitmap)
        {
            Bitmap resultBitmap = SBitmap.ConvolutionFilter(sourceBitmap, SMatrix.Gaussian3x3, 1.0 / 16.0, 0, true);
            resultBitmap = SBitmap.ConvolutionFilter(resultBitmap, SMatrix.Laplacian3x3, 1.0, 0, false);
            return resultBitmap;
        }
               
        public static Bitmap Laplacian3x3OfGaussian5x5Filter1(this Bitmap sourceBitmap)
        {
            Bitmap resultBitmap = SBitmap.ConvolutionFilter(sourceBitmap, SMatrix.Gaussian5x5Type1, 1.0 / 159.0, 0, true);
            resultBitmap = SBitmap.ConvolutionFilter(resultBitmap, SMatrix.Laplacian3x3, 1.0, 0, false);
            return resultBitmap;
        }
               
        public static Bitmap Laplacian3x3OfGaussian5x5Filter2(this Bitmap sourceBitmap)
        {
            Bitmap resultBitmap = SBitmap.ConvolutionFilter(sourceBitmap, SMatrix.Gaussian5x5Type2, 1.0 / 256.0, 0, true);
            resultBitmap = SBitmap.ConvolutionFilter(resultBitmap, SMatrix.Laplacian3x3, 1.0, 0, false);
            return resultBitmap;
        }
               
        public static Bitmap Laplacian5x5OfGaussian3x3Filter(this Bitmap sourceBitmap)
        {
            Bitmap resultBitmap = SBitmap.ConvolutionFilter(sourceBitmap, SMatrix.Gaussian3x3, 1.0 / 16.0, 0, true);
            resultBitmap = SBitmap.ConvolutionFilter(resultBitmap, SMatrix.Laplacian5x5, 1.0, 0, false);
            return resultBitmap;
        }
               
        public static Bitmap Laplacian5x5OfGaussian5x5Filter1(this Bitmap sourceBitmap)
        {
            Bitmap resultBitmap = SBitmap.ConvolutionFilter(sourceBitmap, SMatrix.Gaussian5x5Type1, 1.0 / 159.0, 0, true);
            resultBitmap = SBitmap.ConvolutionFilter(resultBitmap, SMatrix.Laplacian5x5, 1.0, 0, false);
            return resultBitmap;
        }
               
        public static Bitmap Laplacian5x5OfGaussian5x5Filter2(this Bitmap sourceBitmap)
        {
            Bitmap resultBitmap = SBitmap.ConvolutionFilter(sourceBitmap, SMatrix.Gaussian5x5Type2, 1.0 / 256.0, 0, true);
            resultBitmap = SBitmap.ConvolutionFilter(resultBitmap, SMatrix.Laplacian5x5, 1.0, 0, false);
            return resultBitmap;
        }
               
        public static Bitmap Sobel3x3Filter(this Bitmap sourceBitmap, bool grayscale = true)
        {
            Bitmap resultBitmap = SBitmap.ConvolutionFilter(sourceBitmap, SMatrix.Sobel3x3Horizontal, SMatrix.Sobel3x3Vertical, 1.0, 0, grayscale);
            return resultBitmap;
        }
               
        public static Bitmap PrewittFilter(this Bitmap sourceBitmap, bool grayscale = true)
        {
            Bitmap resultBitmap = SBitmap.ConvolutionFilter(sourceBitmap, SMatrix.Prewitt3x3Horizontal, SMatrix.Prewitt3x3Vertical, 1.0, 0, grayscale);
            return resultBitmap;
        }
               
        public static Bitmap KirschFilter(this Bitmap sourceBitmap, bool grayscale = true)
        {
            Bitmap resultBitmap = SBitmap.ConvolutionFilter(sourceBitmap, SMatrix.Kirsch3x3Horizontal, SMatrix.Kirsch3x3Vertical, 1.0, 0, grayscale);
            return resultBitmap;
        }
    }
}