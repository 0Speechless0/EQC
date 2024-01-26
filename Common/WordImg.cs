using System;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace EQC.Common
{
    public class WordImg
    {
        public static Paragraph GetPicInBody(WordprocessingDocument docx, string fileName)
        {
            MainDocumentPart mainPart = docx.MainDocumentPart;

            ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);

            using (FileStream stream = new FileStream(fileName, FileMode.Open))
            {
                imagePart.FeedData(stream);
            }
            return GetImageToBodyB(docx, mainPart.GetIdOfPart(imagePart));
        }

        public static Paragraph GetImageToBodyB(WordprocessingDocument wordDoc, string relationshipId)
        {
            int iWidth = 1200;
            int iHeight = 1200;
            //string imagePath = @"C:\Users\SAKS\Picture3.png";
            //using (System.Drawing.Bitmap bmp = new System.Drawing.Image.FromFile(imagePath))
            //{
            //    iHeight = bmp.Height;
            //    iWidth = bmp.Width;
            //}
            iWidth = (int)Math.Round((decimal)iWidth * 4000);
            iHeight = (int)Math.Round((decimal)iHeight * 4000);
            // Define the reference of the image.
            var element =
                new Drawing(
                    new DW.Inline(
                        new DW.Extent()
                        {
                            Cx = iWidth,
                            Cy = iHeight
                        },
                        new DW.EffectExtent()
                        {
                            LeftEdge = 0,
                            TopEdge = 0,
                            RightEdge = 0,
                            BottomEdge = 0
                        },
                        new DW.DocProperties()
                        {
                            Id = (UInt32Value)1,
                            Name = "Picture 1"
                        },
                        //new DW.NonVisualGraphicFrameDrawingProperties(
                        //    new A.GraphicFrameLocks() { NoChangeAspect = true }),
                        new A.Graphic(
                            new A.GraphicData(
                                new PIC.Picture(
                                    new PIC.NonVisualPictureProperties(
                                        new PIC.NonVisualDrawingProperties()
                                        {
                                            Id = (UInt32Value)0,
                                            Name = "New Bitmap Image.jpg"
                                        },
                                        new PIC.NonVisualPictureDrawingProperties()),
                                    new PIC.BlipFill(
                                        new A.Blip(
                                            new A.BlipExtensionList(
                                                new A.BlipExtension()
                                                {
                                                    Uri =
                                                        "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                })
                                        )
                                        {
                                            Embed = relationshipId,
                                            CompressionState =
                                                A.BlipCompressionValues.Print
                                        },
                                        new A.Stretch(
                                            new A.FillRectangle())),
                                    new PIC.ShapeProperties(
                                        new A.Transform2D(
                                            new A.Offset()
                                            {
                                                X = 0,
                                                Y = 0
                                            },
                                            new A.Extents()
                                            {
                                                Cx = iWidth,
                                                Cy = iHeight
                                            }),
                                        new A.PresetGeometry(
                                            new A.AdjustValueList()
                                        )
                                        {
                                            Preset = A.ShapeTypeValues.Rectangle
                                        }))
                            )
                            {
                                Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture"
                            })
                    )
                    {
                        DistanceFromTop = (UInt32Value)0,
                        DistanceFromBottom = (UInt32Value)0,
                        DistanceFromLeft = (UInt32Value)0,
                        DistanceFromRight = (UInt32Value)0,
                        EditId = "50D07946"
                    });

            // Append the reference to body, the element should be in a Run.
            return new Paragraph(new Run(element));
        }
    }
}