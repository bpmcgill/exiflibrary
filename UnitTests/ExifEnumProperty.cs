using ExifLibrary;
using Xunit;

namespace UnitTests
{
    public class ExifEnumProperty
    {
        [Fact]
        public void Orientation()
        {
            for (var i = 1; i <= 8; i++)
            {
                var img = ImageFile.FromFile(TestHelpers.TestImagePath("Orientation", "Orientation_" + i.ToString() + ".jpg"));
                var orientation = img.Properties.Get<ExifEnumProperty<Orientation>>(ExifTag.Orientation);
                Assert.Equal((Orientation)i, orientation);
            }
        }
    }
}
