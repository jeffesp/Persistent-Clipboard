using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace PersistentClipboard.Tests
{
    public class ClippedItemFile_InitializeTests
    {
        Mock<IContentEncoder> contentEncoderMock;
        Mock<IDataStreamProvider> dataStreamMock;

        public ClippedItemFile_InitializeTests()
        {
            contentEncoderMock = new Mock<IContentEncoder>();
            contentEncoderMock.Setup(ce => ce.Encode(It.IsAny<byte[]>())).Returns(new byte[] {});
            contentEncoderMock.Setup(ce => ce.Decode(It.IsAny<byte[]>())).Returns(new byte[] {});

        }

        [Fact]
        public void Initialization_Will_SetBufferSize()
        {
            using (var ms = SetupDataStreamMock())
            {
                var target = new ClippedItemFile(contentEncoderMock.Object, dataStreamMock.Object);
                Assert.Equal(4 * 4096, ms.Length);
            }
        }

        [Fact]
        public void Initialization_Will_WriteHeaderData()
        {
            using (var ms = SetupDataStreamMock()) { 
                var target = new ClippedItemFile(contentEncoderMock.Object, dataStreamMock.Object);

                ms.Seek(0, SeekOrigin.Begin);
                byte[] buffer = new byte[4];
                ms.Read(buffer, 0, 4);
                Assert.Equal(4, BitConverter.ToInt32(buffer, 0));
            }
        }

        private MemoryStream SetupDataStreamMock()
        {
            var ms = new MemoryStream();
            dataStreamMock = new Mock<IDataStreamProvider>();
            dataStreamMock.Setup(ds => ds.GetStream()).Returns(ms);

            return ms;
        }

    }
}
